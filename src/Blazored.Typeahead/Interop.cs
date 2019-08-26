using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Blazored.Typeahead
{
    public static class Interop
    {
        public static EventHandler OnFocusOutEvent { get; set; }
        public static EventHandler OnEscapeEvent { get; set; }

        public static async Task Focus(IJSRuntime jsRuntime, ElementReference element)
        {
            await jsRuntime.InvokeAsync<object>("blazoredTypeahead.setFocus", element);
        }
        public static async Task AddFocusOutEventListener(IJSRuntime jsRuntime, ElementReference element)
        {
            await jsRuntime.InvokeAsync<object>("blazoredTypeahead.addFocusoutEventListener", element);
        }
        public static async Task AddEscapeEventListener(IJSRuntime jsRuntime, ElementReference element)
        {
            await jsRuntime.InvokeAsync<object>("blazoredTypeahead.addEscapeEventListener", element);
        }

        [JSInvokable]
        public static Task OnEscape()
        {
            OnEscapeEvent?.Invoke(null, new EventArgs());
            return default;
        }
        [JSInvokable]
        public static Task OnFocusOut()
        {
            OnFocusOutEvent?.Invoke(null, new EventArgs());
            return default;
        }
    }
}
