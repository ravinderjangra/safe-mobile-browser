using Plugin.CurrentActivity;
using SafeMobileBrowser.Droid;
using SafeMobileBrowser.Effects;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("MyCompany")]
[assembly: ExportEffect(typeof(GridShadowEffect), "GridShadowEffect")]
namespace SafeMobileBrowser.Droid
{
    public class GridShadowEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            try
            {
                //var control = Control as Android.Views.ViewGroup;
                var control = Control as Android.Widget.TextView;
                var effect = (GridShadowEffect)Element.Effects.FirstOrDefault(e => e is GridShadowEffect);
                if (effect != null)
                {
                    //float radius = effect.Radius;
                    //float distanceX = effect.DistanceX;
                    //float distanceY = effect.DistanceY;
                    //Android.Graphics.Color color = effect.Color.ToAndroid();
                    Android.Graphics.Color color = Color.Black.ToAndroid();
                    //control.SetShadowLayer(radius, distanceX, distanceY, color);
                    control.SetOutlineAmbientShadowColor(color);
                    control.SetOutlineSpotShadowColor(color);
                    control.SetFadingEdgeLength(20);                    
                    //Control.SetBackground(dw);
                    //Control.SetBackgroundResource(Resource.Drawable.background_with_drawable);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannnot set property on attached control. Error: ", ex.Message);
            }
        }

        protected override void OnDetached()
        {
            
        }
    }
}