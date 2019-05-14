using Newtonsoft.Json;
using SafeMobileBrowser.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;


[assembly: InternalsVisibleTo("SafeMobileBrowser.Android")]
[assembly: InternalsVisibleTo("SafeMobileBrowser.iOS")]
namespace SafeMobileBrowser.Controls
{
    public partial class HybridWebView : View, IDisposable
    {
        /// <summary>
        /// A delegate which takes valid javascript and returns the response from it, if the response is a string.
        /// </summary>
        /// <param name="js">The valid JS to inject</param>
        /// <returns>Any string response from the DOM or string.Empty</returns>
        public delegate Task<string> JavascriptInjectionRequestDelegate(string js);

        internal event JavascriptInjectionRequestDelegate OnJavascriptInjectionRequest;

        internal readonly Dictionary<string, Action<string>> LocalRegisteredCallbacks = new Dictionary<string, Action<string>>();

        /// <summary>
        /// Opt in and out of global callbacks
        /// </summary>
        public bool EnableGlobalCallbacks
        {
            get => (bool)GetValue(EnableGlobalCallbacksProperty);
            set => SetValue(EnableGlobalCallbacksProperty, value);
        }

        /// <summary>
        /// Inject some javascript, returning a string result if the resulting Javascript resolves to a string on the DOM.
        /// For example 'document.body.style.backgroundColor = \"red\";' will return 'red'.
        /// </summary>
        /// <param name="js">The javascript to inject</param>
        /// <returns>A valid string response or string.Empty</returns>
        public async Task<string> InjectJavascriptAsync(string js)
        {
            if (string.IsNullOrWhiteSpace(js)) return string.Empty;

            if (OnJavascriptInjectionRequest != null)
                return await OnJavascriptInjectionRequest.Invoke(js);

            return string.Empty;
        }

        /// <summary>
        /// Adds a callback to the DOM, this callback when passed a string by the Javascript, will fire an action with that string as the parameter.
        /// </summary>
        /// <param name="functionName">The name of the function</param>
        /// <param name="action">The action to call back to</param>
        public void AddLocalCallback(string functionName, Action<string> action)
        {
            if (string.IsNullOrWhiteSpace(functionName)) return;

            if (LocalRegisteredCallbacks.ContainsKey(functionName))
                LocalRegisteredCallbacks.Remove(functionName);

            LocalRegisteredCallbacks.Add(functionName, action);
            CallbackAdded?.Invoke(this, functionName);
        }

        /// <summary>
        /// Removes a callback by the function name.
        /// Note: this does not remove it from the DOM, rather it removes the action, resulting in your view never getting the response.
        /// </summary>
        /// <param name="functionName"></param>
        public void RemoveLocalCallback(string functionName)
        {
            if (LocalRegisteredCallbacks.ContainsKey(functionName))
                LocalRegisteredCallbacks.Remove(functionName);
        }

        /// <summary>
        /// Removes all local callbacks from the DOM.
        /// Note: this does not remove it from the DOM, rather it removes the action, resulting in your view never getting the response.
        /// </summary>
        public void RemoveAllLocalCallbacks()
        {
            LocalRegisteredCallbacks.Clear();
        }

        private Action<string> action;

        public static readonly BindableProperty UriProperty = BindableProperty.Create(
            nameof(Uri),
            typeof(string),
            typeof(HybridWebView),
            default(string));

        public static readonly BindableProperty IsLoadingProperty = BindableProperty.Create(
            nameof(IsLoading),
            typeof(bool),
            typeof(HybridWebView),
            default(bool));

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        public string Uri
        {
            get { return (string)GetValue(UriProperty); }
            set { SetValue(UriProperty, value); }
        }

        public void RegisterAction(Action<string> callback)
        {
            action = callback;
        }

        public void Cleanup()
        {
            action = null;
        }

        public void InvokeAction(string data)
        {
            if (action == null || data == null)
            {
                return;
            }
            action.Invoke(data);
        }

        public void Dispose()
        {
            LocalRegisteredCallbacks.Clear();
        }

        internal void HandleScriptReceived(string data, [Optional] JSInterfaceType interfaceType)
        {
            if (string.IsNullOrWhiteSpace(data)) return;

            ActionEvent action;
            if (interfaceType == JSInterfaceType.InitializeApp)
            {
                byte[] toEncodeAsBytes = Encoding.ASCII.GetBytes(data);
                action = new ActionEvent { Action = "initialiseApp", Data = Convert.ToBase64String(toEncodeAsBytes) };
            }
            else
            {
                action = JsonConvert.DeserializeObject<ActionEvent>(data);
            }

            // Decode
            byte[] dBytes = Convert.FromBase64String(action.Data);
            action.Data = Encoding.UTF8.GetString(dBytes, 0, dBytes.Length);

            // Local takes priority
            if (LocalRegisteredCallbacks.ContainsKey(action.Action))
                LocalRegisteredCallbacks[action.Action]?.Invoke(action.Data);

            // Global is checked if local fails
            else if (GlobalRegisteredCallbacks.ContainsKey(action.Action))
                GlobalRegisteredCallbacks[action.Action]?.Invoke(action.Data);
        }

    }
}
