using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SafeMobileBrowser.Controls
{
    public partial class HybridWebView
    {
        internal static event EventHandler<string> CallbackAdded;

        /// <summary>
        /// A bindable property for the EnableGlobalCallbacks property.
        /// </summary>
        public static readonly BindableProperty EnableGlobalCallbacksProperty = BindableProperty.Create(
            nameof(EnableGlobalCallbacks),
            typeof(bool),
            typeof(HybridWebView),
            true);

        internal readonly static Dictionary<string, Action<string>> GlobalRegisteredCallbacks = new Dictionary<string, Action<string>>();

        /// <summary>
        /// Adds a callback to every FormsWebView available in the application.
        /// </summary>
        /// <param name="functionName">The function to call</param>
        /// <param name="action">The returning action</param>
        public static void AddGlobalCallback(string functionName, Action<string> action)
        {
            if (string.IsNullOrWhiteSpace(functionName)) return;

            if (GlobalRegisteredCallbacks.ContainsKey(functionName))
                GlobalRegisteredCallbacks.Remove(functionName);

            GlobalRegisteredCallbacks.Add(functionName, action);
            CallbackAdded?.Invoke(null, functionName);
        }

        /// <summary>
        /// Removes a callback by the function name.
        /// Note: this does not remove it from the DOM, rather it removes the action, resulting in your view never getting the response.
        /// </summary>
        /// <param name="functionName"></param>
        public static void RemoveGlobalCallback(string functionName)
        {
            if (GlobalRegisteredCallbacks.ContainsKey(functionName))
                GlobalRegisteredCallbacks.Remove(functionName);
        }

        /// <summary>
        /// Removes a callback by the function name.
        /// Note: this does not remove it from the DOM, rather it removes the action, resulting in your view never getting the response.
        /// </summary>
        /// <param name="functionName"></param>
        public static void RemoveAllGlobalCallbacks()
        {
            GlobalRegisteredCallbacks.Clear();
        }

        internal static string InjectedFunction
        {
            get
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.Android:
                        return "function csharp(data){safe.invokeAction(data);}";

                    case Device.iOS:
                    case "macOS":
                        return "function csharp(data){window.webkit.messageHandlers.invokeAction.postMessage(data);}";

                    default:
                        return "function csharp(data){window.external.notify(data);}";
                }
            }
        }

        internal static string GenerateFunctionScript(string name)
        {
            return $"function {name}(str){{csharp(\"{{'action':'{name}','data':'\"+window.btoa(str)+\"'}}\");}}";
        }
    }
}
