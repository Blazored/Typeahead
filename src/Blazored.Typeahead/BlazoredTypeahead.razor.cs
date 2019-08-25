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
        protected TItem FocussedSuggestion { get; private set; }

        private Timer _debounceTimer;
        protected ElementReference searchInput;

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

            if (Value == null)
            {
                ShouldShowMask = false;
                ShouldShowInput = true;
            }
            else
            {
                ShouldShowInput = true;
                ShouldShowMask = false;
            }
        }

        protected void HandleClick()
        {
            SearchText = "";
            ShouldShowMenu = false;
            ShouldShowMask = false;
            ShouldShowInput = true;
        }

        protected async Task HandleMaskClick()
        {
            SearchText = "";
            ShouldShowInput = true;
            ShouldShowMenu = false;
            ShouldShowMask = false;
            await Task.Delay(250); // Possible race condition here.
            await JSRuntime.InvokeAsync<object>("blazoredTypeahead.setFocus", searchInput);
        }

        protected async Task HandleClear()
        {
            await ValueChanged.InvokeAsync(default);

            SearchText = "";
            ShouldShowMenu = false;
            ShouldShowInput = true;
            ShouldShowMask = false;
            FocussedSuggestion = default;
            await Task.Delay(250); // Possible race condition here.
            await JSRuntime.InvokeAsync<object>("blazoredTypeahead.setFocus", searchInput);
        }

        protected async Task ShowMaximumSuggestions()
        {
            ShouldShowMenu = !ShouldShowMenu;

            if (ShouldShowMenu)
            {
                _searchText = "";
                Searching = true;
                await InvokeAsync(StateHasChanged);

                SearchResults = (await SearchMethod?.Invoke(_searchText)).Take(MaximumSuggestions).ToArray();

                Searching = false;
                await InvokeAsync(StateHasChanged);
            }
        }

        protected async Task HandleKeyUpOnSuggestion(UIKeyboardEventArgs args, TItem item)
        {
            if (args.Key == "Tab")
                FocussedSuggestion = item;
            if (args.Key == "ArrowDown")
                FocusNextSuggestion();
            if (args.Key == "ArrowUp")
                FocusPreviousSuggestion();
            if (args.Key == "Enter")
                await SelectResult(FocussedSuggestion);
        }

        private void FocusNextSuggestion()
        {
            var indexOfCurrentSuggestion = Array.FindIndex(SearchResults, x => x.Equals(FocussedSuggestion));
            var indexOfNextSuggestion = indexOfCurrentSuggestion + 1;

            if (indexOfNextSuggestion > SearchResults.Length - 1)
            {
                FocusFirstSuggestion();
            }
            else
            {
                FocussedSuggestion = SearchResults[indexOfNextSuggestion];
            }
        }

        private void FocusPreviousSuggestion()
        {
            var indexOfCurrentSuggestion = Array.FindIndex(SearchResults, x => x.Equals(FocussedSuggestion));
            var indexOfPreviousSuggestion = indexOfCurrentSuggestion - 1;

            if (indexOfPreviousSuggestion < 0)
            {
                FocusLastSuggestion();
            }
            else
            {
                FocussedSuggestion = SearchResults[indexOfPreviousSuggestion];
            }
        }

        private void FocusFirstSuggestion()
        {
            FocussedSuggestion = SearchResults[0];
        }

        private void FocusLastSuggestion()
        {
            FocussedSuggestion = SearchResults[SearchResults.Length - 1];
        }

        protected string GetFocussedSuggestionClass(TItem item)
        {
            if (FocussedSuggestion == null)
                return null;
            if (FocussedSuggestion.Equals(item))
                return "blazored-typeahead__result-focussed";
            return null;
        }

        protected async void Search(Object source, ElapsedEventArgs e)
        {
            Searching = true;
            await InvokeAsync(StateHasChanged);
            SearchResults = (await SearchMethod?.Invoke(_searchText)).ToArray();

            Searching = false;
            ShouldShowMenu = true;
            await InvokeAsync(StateHasChanged);
        }

        protected async Task SelectResult(TItem item)
        {               
            await ValueChanged.InvokeAsync(item);

            ShouldShowMenu = false;
            ShouldShowInput = false;
            ShouldShowMask = true;
            FocussedSuggestion = item;
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
