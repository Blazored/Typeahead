using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Blazored.Typeahead.Forms
{
    public class BlazoredTypeaheadInputBase<TItem> : InputBase<TItem>, IDisposable
    {
        [Inject] IJSRuntime JSRuntime { get; set; }

        [Parameter] public string Placeholder { get; set; }
        [Parameter] public Func<string, Task<List<TItem>>> SearchMethod { get; set; }
        [Parameter] public RenderFragment NotFoundTemplate { get; set; }
        [Parameter] public RenderFragment<TItem> ResultTemplate { get; set; }
        [Parameter] public RenderFragment<TItem> SelectedTemplate { get; set; }
        [Parameter] public RenderFragment FooterTemplate { get; set; }
        [Parameter] public int MinimumLength { get; set; } = 1;
        [Parameter] public int Debounce { get; set; } = 300;

        protected bool Searching { get; set; } = false;
        protected bool EditMode { get; set; } = true;
        protected List<TItem> SearchResults { get; set; } = new List<TItem>();
        protected TItem FocussedSuggestion { get; private set; }
        
        private Timer _debounceTimer;
        protected ElementReference searchInput;

        private string _searchText;
        protected string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;

                if (value.Length == 0)
                {
                    _debounceTimer.Stop();
                    SearchResults.Clear();
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

            if (Value != null)
            {
                EditMode = false;
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
            var indexOfCurrentSuggestion = SearchResults.IndexOf(FocussedSuggestion);
            var indexOfNextSuggestion = indexOfCurrentSuggestion + 1;

            if (indexOfNextSuggestion > SearchResults.Count - 1)
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
            var indexOfCurrentSuggestion = SearchResults.IndexOf(FocussedSuggestion);
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
            FocussedSuggestion = SearchResults[SearchResults.Count - 1];
        }

        protected string GetFocussedSuggestionClass(TItem item)
        {
            if (FocussedSuggestion == null)
                return null;
            if (FocussedSuggestion.Equals(item))
                return "blazored-typeahead__result_focussed";
            return null;
        }

        protected async Task HandleFocus()
        {
            SearchText = "";
            EditMode = true;
            await Task.Delay(250);
            await JSRuntime.InvokeAsync<object>("blazoredTypeahead.setFocus", searchInput);
        }

        /// <summary>
        /// Handles the Enter Key Event when the item is selected.
        /// </summary>
        /// <param name="args">Arguments of the KeyUpEvent"/></param>
        /// <param name="item">Selected Item</param>
        /// <returns></returns>
        protected async Task HandleEnterKeySelect(UIKeyboardEventArgs args, TItem item)
        {
            if (args.Key == "Enter")
                await SelectResult(item);
        }

        protected async Task HandleClear()
        {
            await ValueChanged.InvokeAsync(default(TItem));
            EditContext.NotifyFieldChanged(FieldIdentifier);

            _searchText = "";
            EditMode = true;

            await Task.Delay(250);
            await JSRuntime.InvokeAsync<object>("blazoredTypeahead.setFocus", searchInput);
        }

        protected async void Search(Object source, ElapsedEventArgs e)
        {
            Searching = true;
            await InvokeAsync(StateHasChanged);

            SearchResults = await SearchMethod?.Invoke(_searchText);

            Searching = false;
            await InvokeAsync(StateHasChanged);
        }

        protected async Task SelectResult(TItem item)
        {
            await ValueChanged.InvokeAsync(item);
            EditContext.NotifyFieldChanged(FieldIdentifier);

            EditMode = false;
        }

        protected bool ShowSuggestions()
        {
            return EditMode &&
                   !string.IsNullOrWhiteSpace(SearchText) &&
                   SearchText.Length >= MinimumLength &&
                   SearchResults.Any();
        }

        private bool HasValidSearch => !string.IsNullOrWhiteSpace(SearchText) && SearchText.Length >= MinimumLength;

        private bool IsSearchingOrDebouncing => Searching || _debounceTimer.Enabled;

        protected bool ShowNotFound()
        {
            return EditMode &&
                   HasValidSearch &&
                   !IsSearchingOrDebouncing &&
                   !SearchResults.Any();
        }

        protected override bool TryParseValueFromString(string value, out TItem result, out string validationErrorMessage)
        {
            result = (TItem)(object)value;
            validationErrorMessage = null;

            return true;
        }

        public void Dispose()
        {
            _debounceTimer.Dispose();
        }

    }
}
