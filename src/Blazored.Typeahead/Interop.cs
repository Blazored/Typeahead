using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Blazored.Typeahead
{
    public static class Interop
    {
        internal static EventHandler OnFocusOutEvent { get; set; }

        internal static ValueTask<object> Focus(IJSRuntime jsRuntime, ElementReference element)
        {
            return jsRuntime.InvokeAsync<object>("blazoredTypeahead.setFocus", element);
        }

        internal static ValueTask<object> AddFocusOutEventListener(IJSRuntime jsRuntime, ElementReference element)
        {
            return jsRuntime.InvokeAsync<object>("blazoredTypeahead.addFocusoutEventListener", element);
        }

        internal static ValueTask<object> AddKeyDownEventListener(IJSRuntime jsRuntime, ElementReference element)
        {
            return jsRuntime.InvokeAsync<object>("blazoredTypeahead.addKeyDownEventListener", element);
        }

        [JSInvokable]
        public static Task OnFocusOut()
        {
            OnFocusOutEvent?.Invoke(null, new EventArgs());
            return default;
        }
    }
}
