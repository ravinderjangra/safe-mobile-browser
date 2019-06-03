using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using SafeMobileBrowser.Effects;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("MyCompany")]
[assembly: ExportEffect(typeof(ShadowEffect), "GridShadowEffect")]
namespace SafeMobileBrowser.iOS
{
    public class GridShadowEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            try
            {
                var effect = (ShadowEffect)Element.Effects.FirstOrDefault(e => e is ShadowEffect);
                if (effect != null)
                {
                    Control.Layer.CornerRadius = effect.Radius;
                    Control.Layer.ShadowColor = effect.Color.ToCGColor();
                    Control.Layer.ShadowOffset = new CoreGraphics.CGSize(effect.DistanceX, effect.DistanceY);
                    Control.Layer.ShadowOpacity = 1.0f;
                }
            } catch (Exception ex)
            {
                Console.WriteLine("Cannot set property on attached control. Error: ", ex.Message);
            }
        }

        protected override void OnDetached()
        {
            throw new NotImplementedException();
        }
    }
}