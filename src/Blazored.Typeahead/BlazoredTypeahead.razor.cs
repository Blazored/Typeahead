using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

namespace Blazored.Typeahead
{
    public class BlazoredTypeaheadBase<TItem> : ComponentBase, IDisposable
    {
        [Parameter] protected string Placeholder { get; set; }
        [Parameter] protected TItem Item { get; set; }
        [Parameter] protected EventCallback<TItem> ItemChanged { get; set; }
        [Parameter] protected Func<string, Task<List<TItem>>> Data { get; set; }
        [Parameter] protected RenderFragment NotFoundTemplate { get; set; }
        [Parameter] protected RenderFragment<TItem> ResultTemplate { get; set; }
        [Parameter] protected RenderFragment<TItem> SelectedTemplate { get; set; }
        [Parameter] protected int MinimumLength { get; set; } = 1;
        [Parameter] protected int Debounce { get; set; } = 300;

        protected bool Searching { get; set; } = false;
        protected bool EditMode { get; set; } = true;
        protected List<TItem> SearchResults { get; set; } = new List<TItem>();

        private Timer _debounceTimer;

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
                else if (value.Length > MinimumLength)
                {
                    _debounceTimer.Stop();
                    _debounceTimer.Start();
                }
            }
        }

        protected override void OnInit()
        {
            if (Data == null)
            {
                throw new InvalidOperationException("Must provide a data source");
            }

            _debounceTimer = new Timer();
            _debounceTimer.Interval = Debounce;
            _debounceTimer.AutoReset = false;
            _debounceTimer.Elapsed += Search;

            if (Item != null)
            {
                EditMode = false;
            }
        }

        protected void HandleFocus()
        {
            SearchText = "";
            EditMode = true;
        }

        protected async void Search(Object source, ElapsedEventArgs e)
        {
            Searching = true;
            await Invoke(StateHasChanged);

            SearchResults = await Data?.Invoke(_searchText);

            Searching = false;
            await Invoke(StateHasChanged);
        }

        protected async Task SelectResult(TItem item)
        {
            Item = item;
            await ItemChanged.InvokeAsync(item);

            EditMode = false;
        }

        public void Dispose()
        {
            _debounceTimer.Dispose();
        }
    }
}
