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

        protected async Task HandleFocus()
        {
            SearchText = "";
            EditMode = true;
            await Task.Delay(250);
            await JSRuntime.InvokeAsync<object>("Blazored.Typeahead.SetFocus", searchInput);
        }

        protected async Task HandleClear()
        {
            await ValueChanged.InvokeAsync(default(TItem));

            _searchText = "";
            EditMode = true;

            await Task.Delay(250);
            await JSRuntime.InvokeAsync<object>("Blazored.Typeahead.SetFocus", searchInput);
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

        public void Dispose()
        {
            _debounceTimer.Dispose();
        }

    }
}
