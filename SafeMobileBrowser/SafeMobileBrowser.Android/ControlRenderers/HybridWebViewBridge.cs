using System;
using Android.Webkit;
using Java.Interop;
using SafeMobileBrowser.Controls;

namespace SafeMobileBrowser.Droid.ControlRenderers
{
    public class HybridWebViewBridge : Java.Lang.Object
    {
        readonly WeakReference<HybridWebViewRenderer> Reference;

        public HybridWebViewBridge(HybridWebViewRenderer renderer)
        {
            Reference = new WeakReference<HybridWebViewRenderer>(renderer);
        }

        [JavascriptInterface]
        [Export("invokeAction")]
        public void InvokeAction(string data)
        {
            if (Reference == null || !Reference.TryGetTarget(out HybridWebViewRenderer renderer)) return;
            if (renderer.Element == null) return;

            renderer.Element.HandleScriptReceived(data);
        }

        [JavascriptInterface]
        [Export("initialiseApp")]
        public void InitialiseApp(string data)
        {
            if (Reference == null || !Reference.TryGetTarget(out HybridWebViewRenderer renderer)) return;
            if (renderer.Element == null) return;

            renderer.Element.HandleScriptReceived(data, JSInterfaceType.InitializeApp);
        }
    }
}