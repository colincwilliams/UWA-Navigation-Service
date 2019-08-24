// <copyright file="SuspensionManager.cs" company="Colin C. Williams">
// Copyright (c) Colin C. Williams. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ColinCWilliams.UwaNavigationService
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Windows.Storage;
    using Windows.Storage.Streams;
    using Windows.UI.Xaml.Controls;

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

        private readonly HashSet<Type> knownTypes = new HashSet<Type>()
        {
            // Internal types that should be known for serialization
            typeof(NavigationContextServiceState),
            typeof(NavigationContextStore),
            typeof(FrameState),
            typeof(PageState),
        };

        /// <summary>
        /// Gets a List of custom types provided to the <see cref="DataContractSerializer"/> when
        /// reading and writing session state.
        /// </summary>
        public IReadOnlyCollection<Type> KnownTypes
        {
            get { return this.knownTypes; }
        }

        /// <summary>
        /// Gets or sets global session state for the current session.  This state is
        /// serialized by <see cref="SaveAsync"/> and restored by
        /// <see cref="RestoreAsync"/>, so values must be serializable by
        /// <see cref="DataContractSerializer"/> and should be as compact as possible.  Strings
        /// and other self-contained data types are strongly recommended.
        /// </summary>
        private Dictionary<string, FrameState> SessionState { get; set; } = new Dictionary<string, FrameState>();

        /// <summary>
        /// Save the current SessionState for the provided navigationServices. All NavigationServices
        /// will also preserve their current navigation stack, which in turn gives their active
        /// <see cref="Page"/> an opportunity to save its state.
        /// </summary>
        /// <param name="navigationServices">The Navigation Services to save state for.</param>
        /// <returns>An asynchronous task that reflects when session state has been saved.</returns>
        public async Task SaveAsync(IEnumerable<NavigationService> navigationServices)
        {
            try
            {
                // Save the navigation state for all registered frames
                foreach (NavigationService service in navigationServices)
                {
                    this.SessionState[service.Name] = service.SaveState();
                }

                // Serialize the session state synchronously to avoid asynchronous access to shared state
                using (MemoryStream sessionData = new MemoryStream())
                {
                    DataContractSerializer serializer = new DataContractSerializer(this.SessionState.GetType(), this.KnownTypes);
                    serializer.WriteObject(sessionData, this.SessionState);

                    // Get an output stream for the SessionState file and write the state asynchronously
                    StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(SessionStateFilename, CreationCollisionOption.ReplaceExisting);
                    using (Stream fileStream = await file.OpenStreamForWriteAsync().ConfigureAwait(false))
                    {
                        sessionData.Seek(0, SeekOrigin.Begin);
                        await sessionData.CopyToAsync(fileStream).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception e)
            {
                throw new SuspensionManagerException(e);
            }
        }

        /// <summary>
        /// Restores previously saved session state.
        /// </summary>
        /// <returns>An asynchronous task that reflects when session state has been read.  The
        /// content of SessionState should not be relied upon until this task
        /// completes.</returns>
        public async Task RestoreAsync()
        {
            this.SessionState = new Dictionary<string, FrameState>();

            try
            {
                // Get the input stream for the SessionState file
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(SessionStateFilename);
                using (IInputStream inStream = await file.OpenSequentialReadAsync())
                {
                    // Deserialize the Session State
                    DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<string, FrameState>), this.KnownTypes);
                    this.SessionState = (Dictionary<string, FrameState>)serializer.ReadObject(inStream.AsStreamForRead());
                }
            }
            catch (Exception e)
            {
                throw new SuspensionManagerException(e);
            }
        }

        /// <summary>
        /// Restores the state of a NavigationService.
        /// </summary>
        /// <param name="name">The name of the NavigationService to get state for.</param>
        /// <returns>The state of the NavigationService if it was previously restored, otherwise null.</returns>
        public FrameState GetState(string name)
        {
            FrameState state = null;
            if (this.SessionState.ContainsKey(name))
            {
                state = this.SessionState[name];
            }

            return state;
        }

        /// <summary>
        /// Deletes the restored session state for the NavigationService with the given name.
        /// </summary>
        /// <param name="name">Name of the NavigationService to delete state for.</param>
        public void DeleteState(string name)
        {
            this.SessionState.Remove(name);
        }

        /// <summary>
        /// Adds a type to the known types list for serialization.
        /// </summary>
        /// <param name="type">The type to add.</param>
        public void AddKnownType(Type type)
        {
            this.knownTypes.Add(type);
        }
    }
}
