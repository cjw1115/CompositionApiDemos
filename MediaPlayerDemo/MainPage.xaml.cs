using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MediaPlayerDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.Stop();
            player.Pause();
               FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".mp4");
            picker.SuggestedStartLocation = PickerLocationId.Desktop;
            var file=await picker.PickSingleFileAsync();
            if (file != null)
            {
                PlayVideo(file);
            }

        }
        MediaPlayer player = new MediaPlayer();
        public void PlayVideo(StorageFile file)
        {

           
            player.Source = MediaSource.CreateFromStorageFile(file);
            player.Play();

            
            var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            var surface=player.GetSurface(compositor);
            var surfaceBrush = compositor.CreateSurfaceBrush(surface.CompositionSurface);

            var hueEffect = new Microsoft.Graphics.Canvas.Effects.HueRotationEffect()
            {
                Name = "Hue",
                Source = new Windows.UI.Composition.CompositionEffectSourceParameter("source")
            };

            var hueFactory=compositor.CreateEffectFactory(hueEffect,new string[] { "Hue.Angle" });
            var hueBrush=hueFactory.CreateBrush();
            hueBrush.SetSourceParameter("source", surfaceBrush);



            var spriteVisual =compositor.CreateSpriteVisual();

            spriteVisual.Brush = surfaceBrush;

            ElementCompositionPreview.SetElementChildVisual(GridContent,spriteVisual);
            GridContent.SizeChanged += (o, e) =>
            {
                spriteVisual.Size = new System.Numerics.Vector2((float)e.NewSize.Width, (float)e.NewSize.Height);
            };

            var hueAnimation = compositor.CreateScalarKeyFrameAnimation();
            hueAnimation.Duration = TimeSpan.FromMilliseconds(1000);
            hueAnimation.InsertKeyFrame(0f, (float)(0));
            hueAnimation.InsertKeyFrame(1f, (float)(2 * Math.PI));
            hueAnimation.IterationBehavior = Windows.UI.Composition.AnimationIterationBehavior.Forever;
            hueAnimation.StopBehavior = Windows.UI.Composition.AnimationStopBehavior.SetToInitialValue;

            //hueBrush.Properties.StartAnimation("Hue.Angle", hueAnimation);

        }

        private async void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            mediaElement.Stop();
            player.Pause();
            mediaElement.Play();
        }
    }
}
