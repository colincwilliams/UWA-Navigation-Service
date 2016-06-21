// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SampleMultipleFrames
{
    using ColinCWilliams.CSharpNavigationService;
    using SampleCommon;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Container : Page
    {
        public Container()
        {
            this.InitializeComponent();

            this.Loaded += this.Container_Loaded;
        }

        private void Container_Loaded(object sender, RoutedEventArgs e)
        {
            NavigationService.RegisterFrame(this.FrameLeft, typeof(MainPage));
            NavigationService.RegisterFrame(this.FrameRight, typeof(MainPage));
        }
    }
}
