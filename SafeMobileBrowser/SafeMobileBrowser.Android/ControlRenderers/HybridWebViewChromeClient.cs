using System;
using Android.Webkit;

namespace SafeMobileBrowser.Droid.ControlRenderers
{
    public class HybridWebViewChromeClient : WebChromeClient
    {
        readonly WeakReference<HybridWebViewRenderer> Reference;

        public HybridWebViewChromeClient(HybridWebViewRenderer renderer)
        {
            Reference = new WeakReference<HybridWebViewRenderer>(renderer);
        }
    }
}