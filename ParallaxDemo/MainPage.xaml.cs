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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ParallaxDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ViewModel VM;
        public MainPage()
        {

            this.InitializeComponent();
            VM = new ViewModel();
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {

            CreateParralaxEffect();
            CreateGaussBlurEffect();
        }
        public void CreateHeaderOffsetEffect()
        {
            var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            var backgroundVisual = ElementCompositionPreview.GetElementVisual(imgBackground);


            var headerOffsetAnimaton = compositor.CreateExpressionAnimation();
            var progress = "Clamp(visual.Offset.Y/-100,0,1)";
            headerOffsetAnimaton.Expression = "Lerp(200,0," + progress + ")";
            headerOffsetAnimaton.SetReferenceParameter("visual", backgroundVisual);
            var headerVisual = ElementCompositionPreview.GetElementVisual(tbHeader);
            headerVisual.StartAnimation("Offset.Y", headerOffsetAnimaton);
        }
        public void CreateParralaxEffect()
        {
            var scrollViewer = listView.GetFirstDescendantOfType<ScrollViewer>();
            var scrollViewerPropset = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scrollViewer);

            var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;


            var imgOffsetYAnimation = compositor.CreateExpressionAnimation("Clamp(visual.Translation.Y*factor,-450,1000)");
            imgOffsetYAnimation.SetScalarParameter("factor", 0.4f);
            imgOffsetYAnimation.SetReferenceParameter("visual", scrollViewerPropset);
            //imgOffsetYAnimation.Target = "Offset.Y";

            ElementCompositionPreview.SetIsTranslationEnabled(imgBackground, true);
            var backgroundVisual = ElementCompositionPreview.GetElementVisual(imgBackground);
            backgroundVisual.StartAnimation("Translation.Y", imgOffsetYAnimation);

        }
        public void CreateGaussBlurEffect()
        {
            var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            var backgroundVisual = ElementCompositionPreview.GetElementVisual(imgBackground);

            var blurEffect = new Microsoft.Graphics.Canvas.Effects.GaussianBlurEffect()
            {
                Name = "Blur",
                Source = new Windows.UI.Composition.CompositionEffectSourceParameter("blur"),
                BlurAmount = 10
            };

            var backdropBrush = compositor.CreateBackdropBrush();
            var blurEffectFactory = compositor.CreateEffectFactory(blurEffect, new string[] { "Blur.BlurAmount" });
            var blurBrush = blurEffectFactory.CreateBrush();
            blurBrush.SetSourceParameter("blur", backdropBrush);

            var blurAnimation = compositor.CreateExpressionAnimation("");
            var blurProgress = "Clamp(visual.Translation.Y/-450,0,1)";
            blurAnimation.Expression = ("Lerp(0,10," + blurProgress + ")");
            blurAnimation.SetReferenceParameter("visual", backgroundVisual);
            blurBrush.Properties.StartAnimation("Blur.BlurAmount", blurAnimation);

            var spriteVisual = compositor.CreateSpriteVisual();
            spriteVisual.Brush = blurBrush;

            ElementCompositionPreview.SetElementChildVisual(imgBackground, spriteVisual);

            var sizeAnimation = compositor.CreateExpressionAnimation("visual.Size");
            sizeAnimation.SetReferenceParameter("visual", backgroundVisual);
            spriteVisual.StartAnimation("Size", sizeAnimation);
        }
    }
    public class ViewModel
    {
        public List<string> Items { get; set; }
        public ViewModel()
        {
            Items = new List<string>();
            foreach (var item in Enum.GetNames(typeof(DayOfWeek)))
            {
                Items.Add(item);
                Items.Add(item);
                Items.Add(item);
                Items.Add(item);
                Items.Add(item);
            }       
        }
    }

    public static class ExpandHelperMethods
    {
        public static T GetFirstDescendantOfType<T>(this Windows.UI.Xaml.FrameworkElement parent) where T : class
        {
            Queue<Windows.UI.Xaml.FrameworkElement> elementQueue = new Queue<Windows.UI.Xaml.FrameworkElement>();
            elementQueue.Enqueue(parent);
            while (elementQueue.Count > 0)
            {
                var element = elementQueue.Dequeue();
                if (element is T)
                {
                    return element as T;
                }
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
                {

                    var child = VisualTreeHelper.GetChild(element, i);
                    var t = child as T;
                    if (t != null)
                    {
                        return t;
                    }
                    elementQueue.Enqueue((Windows.UI.Xaml.FrameworkElement)child);
                }
            }
            return null;
        }
        public static T GetElementOfTypeByName<T>(this Windows.UI.Xaml.FrameworkElement parent, string elementName) where T : FrameworkElement
        {
            Queue<Windows.UI.Xaml.FrameworkElement> elementQueue = new Queue<Windows.UI.Xaml.FrameworkElement>();
            elementQueue.Enqueue(parent);
            while (elementQueue.Count > 0)
            {
                var element = elementQueue.Dequeue();
                if (element is T)
                {
                    if (element.Name == elementName)
                    {
                        return element as T;
                    }
                }
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
                {

                    var child = VisualTreeHelper.GetChild(element, i);
                    var t = child as T;
                    if (t != null && t.Name == elementName)
                    {
                        return t;
                    }
                    elementQueue.Enqueue((Windows.UI.Xaml.FrameworkElement)child);
                }
            }
            return null;
        }
    }
}
