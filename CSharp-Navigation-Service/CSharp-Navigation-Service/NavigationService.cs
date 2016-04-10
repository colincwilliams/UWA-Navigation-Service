//-----------------------------------------------------------------------
// <summary>
// A navigation service which facilitates Frame navigation and suspension
// with passing of complex types.
// </summary>
// <copyright file="NavigationService.cs" company="Colin C. Williams">
//     Copyright (c) Colin C. Williams. All rights reserved.
// </copyright>
// <author>Colin Williams</author>
//-----------------------------------------------------------------------

namespace ColinCWilliams.CSharpNavigationService
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// The interface for a NavigationService that supports passing complex types.
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Gets the context service associated with this NavigationService.
        /// </summary>
        INavigationContextService ContextService { get; }

        /// <summary>
        /// Navigate back a page in the back stack.
        /// </summary>
        void GoBack();

        /// <summary>
        /// Determines if you calling <see cref="GoBack" /> will success.
        /// </summary>
        /// <returns>True if back navigation can occur, false otherwise.</returns>
        bool CanGoBack();

        /// <summary>
        /// Navigate forward a page in the forward stack.
        /// </summary>
        void GoForward();

        /// <summary>
        /// Determines if you calling <see cref="GoForward" /> will success.
        /// </summary>
        /// <returns>True if forward navigation can occur, false otherwise.</returns>
        bool CanGoForward();

        /// <summary>
        /// Navigate to the specified page with the provided context.
        /// </summary>
        /// <param name="pageType">The type of page to navigate to.</param>
        /// <param name="context">The context that should be provided to the page after navigation.</param>
        void Navigate(Type pageType, NavigationContextBase context = null);
    }

    /// <summary>
    /// A navigation service which facilitates Frame navigation and suspension
    /// with passing of complex types.
    /// </summary>
    public class NavigationService : INavigationService
    {
        private static readonly Dictionary<Frame, INavigationService> NavigationServices = new Dictionary<Frame, INavigationService>();
        
        private readonly INavigationContextService contextService;

        private NavigationService(Frame frame, INavigationContextService contextService)
        {
            this.contextService = contextService;
            this.RootFrame = frame;
        }

        /// <summary>
        /// Gets the context service associated with this NavigationService.
        /// </summary>
        public INavigationContextService ContextService
        {
            get { return this.contextService; }
        }

        private Frame RootFrame
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a new NavigationService for a Frame, registering the frame and NavigationService
        /// for future retrieval and registering the frame with the NavigationContextService.
        /// </summary>
        /// <param name="frame">The frame to register.</param>
        /// <param name="frameKey">The unique identifying key for this frame; used for restoring state.</param>
        /// <returns>The NavigationService for the registered frame.</returns>
        public static INavigationService RegisterFrame(Frame frame, string frameKey)
        {
            if (frame == null)
            {
                throw new ArgumentNullException("frame");
            }

            if (string.IsNullOrWhiteSpace(frameKey))
            {
                throw new ArgumentNullException("frameKey");
            }

            if (NavigationServices.ContainsKey(frame))
            {
                throw new InvalidOperationException("Frame already associated with a NavigationService.");
            }

            INavigationService navigationService = new NavigationService(frame, new NavigationContextService());
            NavigationServices.Add(frame, navigationService);
            SuspensionManager.Instance.RegisterFrame(frame, frameKey);

            return navigationService;
        }

        /// <summary>
        /// Unregisters a Frame and its NavigationService, removing all references to both the
        /// frame and navigation service. Unregisters the frame from the NavigationContextService.
        /// </summary>
        /// <param name="frame">The frame to unregister.</param>
        public static void UnregisterFrame(Frame frame)
        {
            if (frame == null)
            {
                throw new ArgumentNullException("frame");
            }

            if (!NavigationServices.Remove(frame))
            {
                Debug.Assert(false, "Unregistered frame attempted to be unregistered.");
            }
        }

        /// <summary>
        /// Retrieves the NavigationService for a registered Frame.
        /// </summary>
        /// <param name="frame">The Frame to get the NavigationService for.</param>
        /// <returns>The NavigationService for the provided frame.</returns>
        public static INavigationService GetNavigationService(Frame frame)
        {
            if (frame == null)
            {
                throw new ArgumentNullException("frame");
            }

            if (!NavigationServices.ContainsKey(frame))
            {
                throw new InvalidOperationException("Frame not associated with a NavigationService.");
            }

            return NavigationServices[frame];
        }

        /// <summary>
        /// Saves the current navigation state in preparation for
        /// app suspension.
        /// </summary>
        /// <returns>The task for this operation.</returns>
        public static async Task SaveStateAsync()
        {
            await SuspensionManager.Instance.SaveAsync();
        }

        /// <summary>
        /// Restores the current navigation state from disk.
        /// </summary>
        /// <returns>The task for this operation.</returns>
        public static async Task RestoreStateAsync()
        {
            await SuspensionManager.Instance.RestoreAsync();
        }

        /// <summary>
        /// Adds the provided types to a known type list so that they
        /// can be serialized when <see cref="SaveStateAsync" /> is called.
        /// </summary>
        /// <param name="types">The known types to add.</param>
        public static void AddKnownTypes(List<Type> types)
        {
            if (types != null)
            {
                foreach (Type type in types)
                {
                    SuspensionManager.Instance.AddKnownType(type);
                }
            }
        }

        /// <summary>
        /// Navigate to the specified page with the provided context.
        /// </summary>
        /// <param name="pageType">The type of page to navigate to.</param>
        /// <param name="context">The context that should be provided to the page after navigation.</param>
        public void Navigate(Type pageType, NavigationContextBase context = null)
        {
            if (pageType == null)
            {
                throw new ArgumentNullException("pageType");
            }

            if (context != null && !SuspensionManager.Instance.KnownTypes.Contains(context.GetType()))
            {
                throw new ArgumentException(
                    "Cannot navigate with a context that has not been registered as a Known Type. Use NavigationService.AddKnownTypes to register this context type before use.",
                    "context");
            }

            object parameter = null;
            
            if (context != null)
            {
                parameter = this.contextService.Add(context);
            }

            this.RootFrame.Navigate(pageType, parameter);
        }

        /// <summary>
        /// Determines if you calling <see cref="GoBack" /> will success.
        /// </summary>
        /// <returns>True if back navigation can occur, false otherwise.</returns>
        public bool CanGoBack()
        {
            return this.RootFrame != null && this.RootFrame.CanGoBack;
        }

        /// <summary>
        /// Determines if you calling <see cref="GoForward" /> will success.
        /// </summary>
        /// <returns>True if forward navigation can occur, false otherwise.</returns>
        public bool CanGoForward()
        {
            return this.RootFrame != null && this.RootFrame.CanGoForward;
        }

        /// <summary>
        /// Navigate back a page in the back stack.
        /// </summary>
        public void GoBack()
        {
            if (this.CanGoBack())
            {
                this.RootFrame.GoBack();
            }
        }

        /// <summary>
        /// Navigate forward a page in the forward stack.
        /// </summary>
        public void GoForward()
        {
            if (this.CanGoForward())
            {
                this.RootFrame.GoForward();
            }
        }
    }
}
