using Microsoft.Graphics.Canvas.Effects;
using SamplesCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ColorBoomDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            Init();
        }

        ContainerVisual container = null;
        Compositor compositor = null;
        Visual visual = null;
        public void Init()
        {
            visual = ElementCompositionPreview.GetElementVisual(root);
            compositor = visual.Compositor;
            container = compositor.CreateContainerVisual();

            ElementCompositionPreview.SetElementChildVisual(root, container);

            ImageLoader.Initialize(compositor);

        }
        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var rect = (Rectangle)e.ClickedItem;
            var color = ((SolidColorBrush)rect.Fill).Color;

            

            var position = rect.TransformToVisual(root).TransformPoint(new Point(0,0));
            var spiteVisual=compositor.CreateSpriteVisual();

            container.Children.InsertAtTop(spiteVisual);

            spiteVisual.Size = new System.Numerics.Vector2((float)rect.ActualWidth, (float)rect.ActualHeight);
            spiteVisual.Offset = new System.Numerics.Vector3((float)position.X, (float)position.Y, 0);
            spiteVisual.AnchorPoint = new System.Numerics.Vector2(0.5f, 0.5f);


            var compositeEffect = new Microsoft.Graphics.Canvas.Effects.CompositeEffect()
            {
                Mode = Microsoft.Graphics.Canvas.CanvasComposite.DestinationIn,
                Sources =
                {
                     new ColorSourceEffect()
                    {
                        Color = color
                    },
                    new CompositionEffectSourceParameter("mask")
                }
            };
            var factory=compositor.CreateEffectFactory(compositeEffect);
            var compositeBursh=factory.CreateBrush();
            
            var surface = ImageLoader.Instance.LoadCircle(200, Colors.White);
            compositeBursh.SetSourceParameter("mask", surface.Brush);

            spiteVisual.Brush = compositeBursh;

            var oldDiameter = Math.Sqrt(rect.ActualHeight * rect.ActualHeight + rect.ActualWidth * rect.ActualWidth);
            var newDiameter = Math.Sqrt(root.ActualHeight * root.ActualHeight + root.ActualWidth * root.ActualWidth);
            //var oldDiameter = Math.Max(rect.ActualWidth, rect.ActualHeight);
            //var newDiameter = Math.Min(root.ActualWidth, root.ActualHeight);
            var scaleFactor = Math.Round(newDiameter / oldDiameter, MidpointRounding.AwayFromZero) *2;


            var scaleAnimation=compositor.CreateScalarKeyFrameAnimation();
            var ease=compositor.CreateCubicBezierEasingFunction(new System.Numerics.Vector2(0.1f,0.94f),new System.Numerics.Vector2(0.39f,0.97f));
            scaleAnimation.InsertKeyFrame(1f, (float)scaleFactor, ease);
            scaleAnimation.Duration = TimeSpan.FromMilliseconds(800);


            

            var batch=compositor.CreateScopedBatch(Windows.UI.Composition.CompositionBatchTypes.Animation);
            
            spiteVisual.StartAnimation("Scale.X", scaleAnimation);
            spiteVisual.StartAnimation("Scale.Y", scaleAnimation);

            batch.Completed += Batch_Completed;
            batch.End();
            
        }

        private void Batch_Completed(object sender, Windows.UI.Composition.CompositionBatchCompletedEventArgs args)
        {
            container.Children.RemoveAll();
        }
    }
}
