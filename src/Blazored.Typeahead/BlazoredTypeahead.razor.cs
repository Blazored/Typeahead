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

        [Parameter] protected string Placeholder { get; set; }
        [Parameter] protected TItem Value { get; set; }
        [Parameter] protected EventCallback<TItem> ValueChanged { get; set; }
        [Parameter] protected Func<string, Task<List<TItem>>> SearchMethod { get; set; }
        [Parameter] protected RenderFragment NotFoundTemplate { get; set; }
        [Parameter] protected RenderFragment<TItem> ResultTemplate { get; set; }
        [Parameter] protected RenderFragment<TItem> SelectedTemplate { get; set; }
        [Parameter] protected int MinimumLength { get; set; } = 1;
        [Parameter] protected int Debounce { get; set; } = 300;

        protected bool Searching { get; set; } = false;
        protected bool EditMode { get; set; } = true;
        protected List<TItem> SearchResults { get; set; } = new List<TItem>();

        private Timer _debounceTimer;
        protected ElementRef searchInput;

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

        protected override void OnInit()
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
            await Invoke(StateHasChanged);

            SearchResults = await SearchMethod?.Invoke(_searchText);

            Searching = false;
            await Invoke(StateHasChanged);
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

        protected bool ShowNotFound()
        {
            return EditMode && 
                   !string.IsNullOrWhiteSpace(SearchText) && 
                   SearchText.Length >= MinimumLength &&
                   !SearchResults.Any();
        }

        public void Dispose()
        {
            _debounceTimer.Dispose();
        }

    }
}
