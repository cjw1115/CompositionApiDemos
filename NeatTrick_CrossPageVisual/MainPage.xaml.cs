using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace NeatTrick_CrossPageVisual
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            NavigationCacheMode = NavigationCacheMode.Enabled;
            this.InitializeComponent();

            var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            var spriteVisual=compositor.CreateSpriteVisual();
            var brush=compositor.CreateColorBrush(Colors.Red);
            spriteVisual.Brush = brush;
            spriteVisual.Size = new System.Numerics.Vector2(100, 100);
            spriteVisual.Offset = new System.Numerics.Vector3(100, 100, 0);

            var rotationAnimation=compositor.CreateScalarKeyFrameAnimation();
            var ease=compositor.CreateLinearEasingFunction();
            rotationAnimation.InsertKeyFrame(0f, 0);
            rotationAnimation.InsertKeyFrame(1f, 360, ease);
            
            rotationAnimation.Duration = TimeSpan.FromMilliseconds(2000);
            rotationAnimation.IterationBehavior = Windows.UI.Composition.AnimationIterationBehavior.Forever;

            spriteVisual.CenterPoint = new System.Numerics.Vector3(50, 50, 0);
            spriteVisual.StartAnimation(nameof(spriteVisual.RotationAngleInDegrees), rotationAnimation);

            ElementCompositionPreview.SetElementChildVisual(this, spriteVisual);
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            var visual = ElementCompositionPreview.GetElementChildVisual(Frame);
            if (visual != null)
            {
                ElementCompositionPreview.SetElementChildVisual(Frame, null);
                ElementCompositionPreview.SetElementChildVisual(this, visual);

                var offsetAnimation = visual.Compositor.CreateVector3KeyFrameAnimation();
                offsetAnimation.InsertKeyFrame(0, visual.Offset);
                offsetAnimation.InsertKeyFrame(1f, new System.Numerics.Vector3((float)(100), visual.Offset.Y, visual.Offset.Z));
                offsetAnimation.Duration = TimeSpan.FromMilliseconds(3000);
                visual.StartAnimation(nameof(visual.Offset), offsetAnimation);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            

            var visual=ElementCompositionPreview.GetElementChildVisual(this);
            ElementCompositionPreview.SetElementChildVisual(this, null);
            ElementCompositionPreview.SetElementChildVisual(Frame, visual);

            Frame.Navigate(typeof(SecondPage));
        }
    }
}
