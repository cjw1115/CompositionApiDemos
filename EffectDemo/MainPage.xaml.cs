using SamplesCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace EffectDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;

        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var visual = ElementCompositionPreview.GetElementVisual(grid);
            var compositor = visual.Compositor;

            var rootVisual=compositor.CreateContainerVisual();
            ElementCompositionPreview.SetElementChildVisual(grid, rootVisual);

            ImageLoader.Initialize(compositor);

            ManagedSurface managedSurface = ImageLoader.Instance.LoadFromUri(new Uri("ms-appx:///assets/demo.jpg"));
            
            var imageBrush=compositor.CreateSurfaceBrush(managedSurface.Surface);

          
            var effect = new Microsoft.Graphics.Canvas.Effects.HueRotationEffect()
            {
                Name = "hueRotation",
                Source = new CompositionEffectSourceParameter("image")
                //Source = new CompositionEffectSourceParameter("image")
            };

            var gauss = new Microsoft.Graphics.Canvas.Effects.GaussianBlurEffect()
            {
                Name="gauss",
                BlurAmount = 10,
                Source = effect
            };

            var effectFactory = compositor.CreateEffectFactory(gauss, new string[] { "hueRotation.Angle","gauss.BlurAmount"});
            
            var effectBrush = effectFactory.CreateBrush();
            effectBrush.SetSourceParameter("image", imageBrush);

           
            var rotationAnimation = compositor.CreateScalarKeyFrameAnimation();
            var linearEase = compositor.CreateLinearEasingFunction();
            rotationAnimation.InsertKeyFrame(0f, 0);
            rotationAnimation.InsertKeyFrame(1f, (float)(2 * Math.PI), linearEase);
            rotationAnimation.Duration = TimeSpan.FromMilliseconds(4000);
            rotationAnimation.IterationBehavior = Windows.UI.Composition.AnimationIterationBehavior.Forever;
            effectBrush.StartAnimation("hueRotation.Angle", rotationAnimation);

            var blurAnimation = compositor.CreateScalarKeyFrameAnimation();
            blurAnimation.InsertKeyFrame(0f, 0);
            blurAnimation.InsertKeyFrame(1f, 10);
            blurAnimation.Direction = Windows.UI.Composition.AnimationDirection.AlternateReverse;
            blurAnimation.Duration = TimeSpan.FromMilliseconds(4000);
            blurAnimation.IterationBehavior = Windows.UI.Composition.AnimationIterationBehavior.Forever;
            effectBrush.StartAnimation("gauss.BlurAmount", blurAnimation);

            var spriteVisual = compositor.CreateSpriteVisual();
            spriteVisual.Brush = effectBrush;

            rootVisual.Children.InsertAtTop(spriteVisual);
            //ElementCompositionPreview.SetElementChildVisual(grid, spriteVisual);



            var sizeAnimation = compositor.CreateExpressionAnimation("visual.Size");
            sizeAnimation.SetReferenceParameter("visual", visual);
            spriteVisual.StartAnimation("Size", sizeAnimation);


            var compositeEffect = new Microsoft.Graphics.Canvas.Effects.CompositeEffect();
            compositeEffect.Mode = Microsoft.Graphics.Canvas.CanvasComposite.SourceIn;
            compositeEffect.Sources.Add(new CompositionEffectSourceParameter("mask"));

            compositeEffect.Sources.Add(new CompositionEffectSourceParameter("image"));

            var profileVisual = compositor.CreateSpriteVisual();

            var compositeEffectFactory = compositor.CreateEffectFactory(compositeEffect);
            var compositeBrush = compositeEffectFactory.CreateBrush();

         
            compositeBrush.SetSourceParameter("image", imageBrush);

            var mask = ImageLoader.Instance.LoadFromUri(new Uri("ms-appx:///assets/CircleMask.png"));
            
            compositeBrush.SetSourceParameter("mask", mask.Brush);

            profileVisual.Brush = compositeBrush;

            rootVisual.Children.InsertAtTop(profileVisual);
            //ElementCompositionPreview.SetElementChildVisual(grid, profileVisual);
            var profileSizeAnimation = compositor.CreateExpressionAnimation("visual.Size");
            profileSizeAnimation.SetReferenceParameter("visual", visual);
            profileVisual.StartAnimation("Size", profileSizeAnimation);
        }
    }
}
