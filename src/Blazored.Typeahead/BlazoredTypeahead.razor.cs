using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Blazored.Typeahead
{
    public class BlazoredTypeaheadBase<TItem> : ComponentBase, IDisposable
    {
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Parameter] public string Placeholder { get; set; }
        [Parameter] public TItem Value { get; set; }
        [Parameter] public EventCallback<TItem> ValueChanged { get; set; }
        [Parameter] public Func<string, Task<IEnumerable<TItem>>> SearchMethod { get; set; }
        [Parameter] public RenderFragment NotFoundTemplate { get; set; }
        [Parameter] public RenderFragment<TItem> ResultTemplate { get; set; }
        [Parameter] public RenderFragment<TItem> SelectedTemplate { get; set; }
        [Parameter] public RenderFragment FooterTemplate { get; set; }
        [Parameter] public int MinimumLength { get; set; } = 1;
        [Parameter] public int Debounce { get; set; } = 300;
        [Parameter] public int MaximumSuggestions { get; set; } = 25;

        protected bool Searching { get; set; } = false;
        protected bool ShouldShowMenu { get; private set; } = false;
        protected bool ShouldShowInput { get; private set; } = true;
        protected bool ShouldShowMask { get; private set; } = false;
        protected TItem[] SearchResults { get; set; } = new TItem[0];
        
        private Timer _debounceTimer;
        protected ElementReference searchInput;
        protected ElementReference mask;

        private string _searchText = string.Empty;
        protected string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;

                if (value.Length == 0)
                {
                    _debounceTimer.Stop();
                    SearchResults = new TItem[0];
                }
                else if (value.Length >= MinimumLength)
                {
                    _debounceTimer.Stop();
                    _debounceTimer.Start();
                }
            }
        }

        protected override void OnInitialized()
        {
            if (SearchMethod == null)
            {
                throw new InvalidOperationException($"{GetType()} requires a {nameof(SearchMethod)} parameter.");
            }

            if (SelectedTemplate == null)
            {
                throw new InvalidOperationException($"{GetType()} requires a {nameof(SelectedTemplate)} parameter.");
            }

            if (ResultTemplate == null)
            {
                throw new InvalidOperationException($"{GetType()} requires a {nameof(ResultTemplate)} parameter.");
            }

            _debounceTimer = new Timer();
            _debounceTimer.Interval = Debounce;
            _debounceTimer.AutoReset = false;
            _debounceTimer.Elapsed += Search;

            Initialze();
        }

        protected override void OnParametersSet()
        {
            Initialze();
        }

        private void Initialze()
        {
            ShouldShowMenu = false;
            if (Value == null)
            {
                ShouldShowMask = false;
                ShouldShowInput = true;
            }
            else
            {
                ShouldShowInput = false;
                ShouldShowMask = true;
            }
        }
        protected async Task HandleClear()
        {
            await ValueChanged.InvokeAsync(default);
            SearchText = "";
            await Task.Delay(250); // Possible race condition here.
            await JSRuntime.InvokeAsync<object>("blazoredTypeahead.setFocus", searchInput);
        }

        protected async Task ShowMaximumSuggestions()
        {
            ShouldShowMenu = !ShouldShowMenu;

            if (ShouldShowMenu)
            {
                SearchText = "";
                Searching = true;
                await InvokeAsync(StateHasChanged);

                SearchResults = (await SearchMethod?.Invoke(_searchText)).Take(MaximumSuggestions).ToArray();

                Searching = false;
                await InvokeAsync(StateHasChanged);
            }
        }

        protected async Task HandleKeyUpOnSuggestion(UIKeyboardEventArgs args, TItem item)
        {
            // Maybe on any key except Tab and Enter, continue the typing option.
            switch (args.Key)
            {
                case "Enter":
                    await SelectResult(item);
                    break;
                case "Escape":
                case "Backspace":
                case "Delete":
                    Initialze();
                    await Task.Delay(250);
                    await JSRuntime.InvokeAsync<object>("blazoredTypeahead.setFocus", searchInput);
                    break;
                default:
                    break;
            }
        }

        protected async Task HandleKeyUpOnShowMaximum(UIKeyboardEventArgs args)
        {
            if (args.Key == "Enter")
                await ShowMaximumSuggestions();
        }

        protected async Task HandleKeyUpOnMask(UIKeyboardEventArgs args)
        {
            switch (args.Key)
            {
                case "Enter":
                case "Escape":
                case "Backspace":
                case "Delete":
                    await HandleClear();
                    break;
                default:
                    break;
            }
        }

        protected string GetSelectedSuggestionClass(TItem item)
        {
            if (Value == null)
                return null;
            if (Value.Equals(item))
                return "blazored-typeahead__result-selected";
            return null;
        }

        protected async void Search(Object source, ElapsedEventArgs e)
        {
            Searching = true;
            await InvokeAsync(StateHasChanged);
            SearchResults = (await SearchMethod?.Invoke(_searchText)).Take(MaximumSuggestions).ToArray();

            Searching = false;
            ShouldShowMenu = true;
            await InvokeAsync(StateHasChanged);
        }

        protected async Task SelectResult(TItem item)
        {
            await ValueChanged.InvokeAsync(item);
            await Task.Delay(250);
            await JSRuntime.InvokeAsync<object>("blazoredTypeahead.setFocus", mask);
        }

        protected bool ShouldShowSuggestions()
        {
            return ShouldShowMenu &&
                   SearchResults.Any();
        }

        private bool HasValidSearch => !string.IsNullOrWhiteSpace(SearchText) && SearchText.Length >= MinimumLength;

        private bool IsSearchingOrDebouncing => Searching || _debounceTimer.Enabled;

        protected bool ShowNotFound()
        {
            return ShouldShowMenu &&
                   HasValidSearch &&
                   !IsSearchingOrDebouncing &&
                   !SearchResults.Any();
        }

        public void Dispose()
        {
            _debounceTimer.Dispose();
        }

    }
}
