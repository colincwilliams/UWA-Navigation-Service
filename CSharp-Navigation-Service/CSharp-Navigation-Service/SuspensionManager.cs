//-----------------------------------------------------------------------
// <summary>A class to manage app suspension and restoration.</summary>
// <copyright file="SuspensionManager.cs" company="Colin C. Williams">
//     Copyright (c) Colin C. Williams. All rights reserved.
// </copyright>
// <author>Colin Williams</author>
//-----------------------------------------------------------------------

namespace ColinCWilliams.CSharpNavigationService
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Windows.Storage;
    using Windows.Storage.Streams;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// A class to manage app suspension and restoration.
    /// </summary>
    public interface ISuspensionManager
    {
        /// <summary>
        /// Adds a type to the known types list for serialization.
        /// </summary>
        /// <param name="type">The type to add.</param>
        void AddKnownType(Type type);

        /// <summary>
        /// Registers a <see cref="Frame"/> instance to allow its navigation history to be saved to
        /// and restored from <see cref="SessionState"/>.  Frames should be registered once
        /// immediately after creation if they will participate in session state management.  Upon
        /// registration if state has already been restored for the specified key
        /// the navigation history will immediately be restored.  Subsequent invocations of
        /// <see cref="RestoreAsync"/> will also restore navigation history.
        /// </summary>
        /// <param name="frame">An instance whose navigation history should be managed by
        /// <see cref="SuspensionManager"/></param>
        /// <param name="sessionStateKey">A unique key into <see cref="SessionState"/> used to
        /// store navigation-related information.</param>
        /// <param name="sessionBaseKey">An optional key that identifies the type of session.
        /// This can be used to distinguish between multiple application launch scenarios.</param>
        void RegisterFrame(Frame frame, string sessionStateKey, string sessionBaseKey = null);

        /// <summary>
        /// Restores previously saved <see cref="SessionState"/>.  Any <see cref="Frame"/> instances
        /// registered with <see cref="RegisterFrame"/> will also restore their prior navigation
        /// state, which in turn gives their active <see cref="Page"/> an opportunity restore its
        /// state.
        /// </summary>
        /// <param name="sessionBaseKey">An optional key that identifies the type of session.
        /// This can be used to distinguish between multiple application launch scenarios.</param>
        /// <returns>An asynchronous task that reflects when session state has been read.  The
        /// content of <see cref="SessionState"/> should not be relied upon until this task
        /// completes.</returns>
        Task RestoreAsync(string sessionBaseKey = null);

        /// <summary>
        /// Save the current <see cref="SessionState"/>.  Any <see cref="Frame"/> instances
        /// registered with <see cref="RegisterFrame"/> will also preserve their current
        /// navigation stack, which in turn gives their active <see cref="Page"/> an opportunity
        /// to save its state.
        /// </summary>
        /// <returns>An asynchronous task that reflects when session state has been saved.</returns>
        Task SaveAsync();

        /// <summary>
        /// Provides storage for session state associated with the specified <see cref="Frame"/>.
        /// Frames that have been previously registered with <see cref="RegisterFrame"/> have
        /// their session state saved and restored automatically as a part of the global
        /// <see cref="SessionState"/>.  Frames that are not registered have transient state
        /// that can still be useful when restoring pages that have been discarded from the
        /// navigation cache.
        /// </summary>
        /// <remarks>Apps may choose to rely on <see cref="NavigationHelper"/> to manage
        /// page-specific state instead of working with frame session state directly.</remarks>
        /// <param name="frame">The instance for which session state is desired.</param>
        /// <returns>A collection of state subject to the same serialization mechanism as
        /// <see cref="SessionState"/>.</returns>
        Dictionary<string, object> SessionStateForFrame(Frame frame);
    }

    /// <summary>
    /// SuspensionManager captures global session state to simplify process lifetime management
    /// for an application.  Note that session state will be automatically cleared under a variety
    /// of conditions and should only be used to store information that would be convenient to
    /// carry across sessions, but that should be discarded when an application crashes or is
    /// upgraded.
    /// </summary>
    internal class SuspensionManager : ISuspensionManager
    {
        private const string SessionStateFilename = "_sessionState.xml";
        private const string NavigationContextStateKey = "__navigationContextState__";
        private const string FrameNavigationKey = "Navigation";

        private static ISuspensionManager instance;

        private static DependencyProperty frameSessionStateKeyProperty =
            DependencyProperty.RegisterAttached("_FrameSessionStateKey", typeof(string), typeof(SuspensionManager), null);

        private static DependencyProperty frameSessionBaseKeyProperty =
            DependencyProperty.RegisterAttached("_FrameSessionBaseKeyParams", typeof(string), typeof(SuspensionManager), null);

        private static DependencyProperty frameSessionStateProperty =
            DependencyProperty.RegisterAttached("_FrameSessionState", typeof(Dictionary<string, object>), typeof(SuspensionManager), null);

        private static List<WeakReference<Frame>> registeredFrames = new List<WeakReference<Frame>>();

        private Dictionary<string, object> sessionState = new Dictionary<string, object>();
        private List<Type> knownTypes = new List<Type>()
        {
            // Add internal types that should be known for serialization
            typeof(NavigationContextServiceState),
            typeof(NavigationContextStore)
        };

        private SuspensionManager()
        {
        }

        /// <summary>
        /// Gets the singleton instance of <see cref="SuspensionManager" />.
        /// </summary>
        public static ISuspensionManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SuspensionManager();
                }

                return instance;
            }
        }

        /// <summary>
        /// Gets global session state for the current session.  This state is
        /// serialized by <see cref="SaveAsync"/> and restored by
        /// <see cref="RestoreAsync"/>, so values must be serializable by
        /// <see cref="DataContractSerializer"/> and should be as compact as possible.  Strings
        /// and other self-contained data types are strongly recommended.
        /// </summary>
        public Dictionary<string, object> SessionState
        {
            get { return this.sessionState; }
        }

        /// <summary>
        /// Gets a List of custom types provided to the <see cref="DataContractSerializer"/> when
        /// reading and writing session state.  Initially empty, additional types may be
        /// added to customize the serialization process.
        /// </summary>
        private List<Type> KnownTypes
        {
            get { return this.knownTypes; }
        }

        /// <summary>
        /// Save the current <see cref="SessionState"/>.  Any <see cref="Frame"/> instances
        /// registered with <see cref="RegisterFrame"/> will also preserve their current
        /// navigation stack, which in turn gives their active <see cref="Page"/> an opportunity
        /// to save its state.
        /// </summary>
        /// <returns>An asynchronous task that reflects when session state has been saved.</returns>
        public async Task SaveAsync()
        {
            try
            {
                // Save the navigation state for all registered frames
                foreach (var weakFrameReference in registeredFrames)
                {
                    Frame frame;
                    if (weakFrameReference.TryGetTarget(out frame))
                    {
                        this.SaveFrameNavigationState(frame);
                    }
                }

                // Serialize the session state synchronously to avoid asynchronous access to shared state
                MemoryStream sessionData = new MemoryStream();
                DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<string, object>), this.KnownTypes);
                serializer.WriteObject(sessionData, this.sessionState);

                // Get an output stream for the SessionState file and write the state asynchronously
                StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(SessionStateFilename, CreationCollisionOption.ReplaceExisting);
                using (Stream fileStream = await file.OpenStreamForWriteAsync())
                {
                    sessionData.Seek(0, SeekOrigin.Begin);
                    await sessionData.CopyToAsync(fileStream);
                }
            }
            catch (Exception e)
            {
                throw new SuspensionManagerException(e);
            }
        }

        /// <summary>
        /// Restores previously saved <see cref="SessionState"/>.  Any <see cref="Frame"/> instances
        /// registered with <see cref="RegisterFrame"/> will also restore their prior navigation
        /// state, which in turn gives their active <see cref="Page"/> an opportunity restore its
        /// state.
        /// </summary>
        /// <param name="sessionBaseKey">An optional key that identifies the type of session.
        /// This can be used to distinguish between multiple application launch scenarios.</param>
        /// <returns>An asynchronous task that reflects when session state has been read.  The
        /// content of <see cref="SessionState"/> should not be relied upon until this task
        /// completes.</returns>
        public async Task RestoreAsync(string sessionBaseKey = null)
        {
            this.sessionState = new Dictionary<string, object>();

            try
            {
                // Get the input stream for the SessionState file
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(SessionStateFilename);
                using (IInputStream inStream = await file.OpenSequentialReadAsync())
                {
                    // Deserialize the Session State
                    DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<string, object>), this.KnownTypes);
                    this.sessionState = (Dictionary<string, object>)serializer.ReadObject(inStream.AsStreamForRead());
                }

                // Restore any registered frames to their saved state
                foreach (var weakFrameReference in registeredFrames)
                {
                    Frame frame;
                    if (weakFrameReference.TryGetTarget(out frame) && (string)frame.GetValue(frameSessionBaseKeyProperty) == sessionBaseKey)
                    {
                        frame.ClearValue(frameSessionStateProperty);
                        this.RestoreFrameNavigationState(frame);
                    }
                }
            }
            catch (Exception e)
            {
                throw new SuspensionManagerException(e);
            }
        }

        /// <summary>
        /// Adds a type to the known types list for serialization.
        /// </summary>
        /// <param name="type">The type to add.</param>
        public void AddKnownType(Type type)
        {
            if (!this.KnownTypes.Contains(type))
            {
                this.KnownTypes.Add(type);
            }
        }

        /// <summary>
        /// Registers a <see cref="Frame"/> instance to allow its navigation history to be saved to
        /// and restored from <see cref="SessionState"/>.  Frames should be registered once
        /// immediately after creation if they will participate in session state management.  Upon
        /// registration if state has already been restored for the specified key
        /// the navigation history will immediately be restored.  Subsequent invocations of
        /// <see cref="RestoreAsync"/> will also restore navigation history.
        /// </summary>
        /// <param name="frame">An instance whose navigation history should be managed by
        /// <see cref="SuspensionManager"/></param>
        /// <param name="sessionStateKey">A unique key into <see cref="SessionState"/> used to
        /// store navigation-related information.</param>
        /// <param name="sessionBaseKey">An optional key that identifies the type of session.
        /// This can be used to distinguish between multiple application launch scenarios.</param>
        public void RegisterFrame(Frame frame, string sessionStateKey, string sessionBaseKey = null)
        {
            if (frame == null)
            {
                throw new ArgumentNullException("frame");
            }

            if (frame.GetValue(frameSessionStateKeyProperty) != null)
            {
                throw new InvalidOperationException("Frames can only be registered to one session state key");
            }

            if (frame.GetValue(frameSessionStateProperty) != null)
            {
                throw new InvalidOperationException("Frames must be either be registered before accessing frame session state, or not registered at all");
            }

            if (!string.IsNullOrEmpty(sessionBaseKey))
            {
                frame.SetValue(frameSessionBaseKeyProperty, sessionBaseKey);
                sessionStateKey = sessionBaseKey + "_" + sessionStateKey;
            }

            // Use a dependency property to associate the session key with a frame, and keep a list of frames whose
            // navigation state should be managed
            frame.SetValue(frameSessionStateKeyProperty, sessionStateKey);
            registeredFrames.Add(new WeakReference<Frame>(frame));

            // Check to see if navigation state can be restored
            this.RestoreFrameNavigationState(frame);
        }

        /// <summary>
        /// Disassociates a <see cref="Frame"/> previously registered by <see cref="RegisterFrame"/>
        /// from <see cref="SessionState"/>.  Any navigation state previously captured will be
        /// removed.
        /// </summary>
        /// <param name="frame">An instance whose navigation history should no longer be
        /// managed.</param>
        public void UnregisterNavigationService(Frame frame)
        {
            // Remove session state and remove the frame from the list of frames whose navigation
            // state will be saved (along with any weak references that are no longer reachable)
            this.SessionState.Remove((string)frame.GetValue(frameSessionStateKeyProperty));
            registeredFrames.RemoveAll((weakFrameReference) =>
            {
                Frame testFrame;
                return !weakFrameReference.TryGetTarget(out testFrame) || testFrame == frame;
            });
        }

        /// <summary>
        /// Provides storage for session state associated with the specified <see cref="Frame"/>.
        /// Frames that have been previously registered with <see cref="RegisterFrame"/> have
        /// their session state saved and restored automatically as a part of the global
        /// <see cref="SessionState"/>.  Frames that are not registered have transient state
        /// that can still be useful when restoring pages that have been discarded from the
        /// navigation cache.
        /// </summary>
        /// <remarks>Apps may choose to rely on <see cref="NavigationHelper"/> to manage
        /// page-specific state instead of working with frame session state directly.</remarks>
        /// <param name="frame">The instance for which session state is desired.</param>
        /// <returns>A collection of state subject to the same serialization mechanism as
        /// <see cref="SessionState"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Only intended to be used with Frames.")]
        public Dictionary<string, object> SessionStateForFrame(Frame frame)
        {
            if (frame == null)
            {
                throw new ArgumentNullException("frame");
            }

            var frameState = (Dictionary<string, object>)frame.GetValue(frameSessionStateProperty);

            if (frameState == null)
            {
                var frameSessionKey = (string)frame.GetValue(frameSessionStateKeyProperty);
                if (frameSessionKey != null)
                {
                    // Registered frames reflect the corresponding session state
                    if (!this.sessionState.ContainsKey(frameSessionKey))
                    {
                        this.sessionState[frameSessionKey] = new Dictionary<string, object>();
                    }

                    frameState = (Dictionary<string, object>)this.sessionState[frameSessionKey];
                }
                else
                {
                    // Frames that aren't registered have transient state
                    frameState = new Dictionary<string, object>();
                }

                frame.SetValue(frameSessionStateProperty, frameState);
            }

            return frameState;
        }

        private void RestoreFrameNavigationState(Frame frame)
        {
            var frameState = this.SessionStateForFrame(frame);
            if (frameState.ContainsKey(FrameNavigationKey))
            {
                frame.SetNavigationState((string)frameState[FrameNavigationKey]);
            }

            if (frameState.ContainsKey(NavigationContextStateKey))
            {
                INavigationService navigationService = NavigationService.GetNavigationService(frame);
                navigationService.ContextService.RestoreState((NavigationContextServiceState)frameState[NavigationContextStateKey]);
            }
        }

        private void SaveFrameNavigationState(Frame frame)
        {
            var frameState = this.SessionStateForFrame(frame);
            frameState[FrameNavigationKey] = frame.GetNavigationState();
            frameState[NavigationContextStateKey] = NavigationService.GetNavigationService(frame).ContextService.SaveState();
        }
    }
}
