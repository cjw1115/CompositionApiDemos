using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace NeatTrick_CrossPageVisual
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SecondPage : Page
    {
        public SecondPage()
        {
            NavigationCacheMode = NavigationCacheMode.Enabled;
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var visual= ElementCompositionPreview.GetElementChildVisual(Frame);
            ElementCompositionPreview.SetElementChildVisual(Frame, null);
            ElementCompositionPreview.SetElementChildVisual(this, visual);

            var offsetAnimation=visual.Compositor.CreateVector3KeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(0, visual.Offset);
            offsetAnimation.InsertKeyFrame(1f, new System.Numerics.Vector3((float)(Frame.ActualHeight - 200), visual.Offset.Y, visual.Offset.Z));
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(3000);
            visual.StartAnimation(nameof(visual.Offset), offsetAnimation);

            
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var visual = ElementCompositionPreview.GetElementChildVisual(this);
            ElementCompositionPreview.SetElementChildVisual(this, null);
            ElementCompositionPreview.SetElementChildVisual(Frame, visual);

            Frame.GoBack();
        }
    }
}
