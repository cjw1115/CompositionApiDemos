using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace XamlLightDemo
{
    public class FactorValueConverter : IValueConverter
    {
        public double Factor
        {
            get; set;
        } = 1;
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var originValue = (float)(double)value;
            return originValue * (float)Factor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class CustomSpotLight : XamlLight
    {
        // Register an attached property that enables apps to set a UIElement or Brush as a target for this light type in markup.
        public static readonly DependencyProperty IsTargetProperty =
            DependencyProperty.RegisterAttached(
            "IsTarget",
            typeof(bool),
            typeof(CustomSpotLight),
            new PropertyMetadata(null, OnIsTargetChanged)
        );
        public static void SetIsTarget(DependencyObject target, bool value)
        {
            target.SetValue(IsTargetProperty, value);
        }
        public static Boolean GetIsTarget(DependencyObject target)
        {
            return (bool)target.GetValue(IsTargetProperty);
        }

        // Handle attached property changed to automatically target and untarget UIElements and Brushes.
        private static void OnIsTargetChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var isAdding = (bool)e.NewValue;

            if (isAdding)
            {
                if (obj is UIElement)
                {
                    XamlLight.AddTargetElement(GetIdStatic(), obj as UIElement);
                }
                else if (obj is Brush)
                {
                    XamlLight.AddTargetBrush(GetIdStatic(), obj as Brush);
                }
            }
            else
            {
                if (obj is UIElement)
                {
                    XamlLight.RemoveTargetElement(GetIdStatic(), obj as UIElement);
                }
                else if (obj is Brush)
                {
                    XamlLight.RemoveTargetBrush(GetIdStatic(), obj as Brush);
                }
            }
        }

        protected override void OnConnected(UIElement newElement)
        {
            if (CompositionLight == null)
            {
                // OnConnected is called when the first target UIElement is shown on the screen. This enables delaying composition object creation until it's actually necessary.
                var spotLight = Window.Current.Compositor.CreateSpotLight();
                spotLight.InnerConeColor = Colors.Red;
                spotLight.OuterConeColor = Colors.White;
                spotLight.InnerConeAngleInDegrees = 0;
                spotLight.OuterConeAngleInDegrees = 45;
                spotLight.Offset = new System.Numerics.Vector3(250, 250, 500);
                CompositionLight = spotLight;
            }
        }

        protected override void OnDisconnected(UIElement oldElement)
        {
            // OnDisconnected is called when there are no more target UIElements on the screen. The CompositionLight should be disposed when no longer required.
            if (CompositionLight != null)
            {
                CompositionLight.Dispose();
                CompositionLight = null;
            }
        }

        protected override string GetId()
        {
            return GetIdStatic();
        }

        private static string GetIdStatic()
        {
            // This specifies the unique name of the light. In most cases you should use the type's FullName.
            return typeof(CustomSpotLight).FullName;
        }

        public static readonly DependencyProperty InnerConeAngleProperty = DependencyProperty.RegisterAttached("InnerConeAngle", typeof(float), typeof(CustomSpotLight), new PropertyMetadata(0d, (o, e) =>
              {
                  var light = o as CustomSpotLight;
                  var spotLight = light.CompositionLight as SpotLight;
                  spotLight.InnerConeAngleInDegrees = (float)e.NewValue;
              }));
        public static float GetInnerConeAngle(DependencyObject o)
        {
            return (float)o.GetValue(InnerConeAngleProperty);
        }
        public static void SetInnerConeAngle(DependencyObject o,float value)
        {
            o.SetValue(InnerConeAngleProperty, value);
        }

        public static readonly DependencyProperty OutterConeAngleProperty = DependencyProperty.RegisterAttached("OutterConeAngle", typeof(float), typeof(CustomSpotLight), new PropertyMetadata(0d, (o, e) =>
        {
            var light = o as CustomSpotLight;
            var spotLight = light.CompositionLight as SpotLight;
            spotLight.OuterConeAngleInDegrees = (float)e.NewValue;
        }));
        public static float GetOutterConeAngle(DependencyObject o)
        {
            return (float)o.GetValue(OutterConeAngleProperty);
        }
        public static void SetOutterConeAngle(DependencyObject o, float value)
        {
            o.SetValue(OutterConeAngleProperty, value);
        }
    }
}
