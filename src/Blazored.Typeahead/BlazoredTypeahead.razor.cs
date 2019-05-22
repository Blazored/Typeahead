using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Blazored.Typeahead
{
    public class BlazoredTypeaheadBase<TItem> : ComponentBase, IDisposable
    {
        [Parameter] protected string Placeholder { get; set; }
        [Parameter] protected TItem Item { get; set; }
        [Parameter] protected EventCallback<TItem> ItemChanged { get; set; }
        [Parameter] protected List<TItem> Data { get; set; }
        [Parameter] protected string Remote { get; set; }
        [Parameter] protected RenderFragment NotFoundTemplate { get; set; }
        [Parameter] protected RenderFragment<TItem> ResultTemplate { get; set; }
        [Parameter] protected RenderFragment<TItem> SelectedTemplate { get; set; }
        [Parameter] protected int MinimumLength { get; set; } = 1;
        [Parameter] protected Func<TItem, string> SearchOn { get; set; }
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
                    Searching = false;
                    SearchResults.Clear();
                }
                else if (value.Length > MinimumLength)
                {
                    Console.WriteLine($"Resetting Debounce Timer");
                    _debounceTimer.Stop();
                    Console.WriteLine($"Starting Debounce Timer");
                    _debounceTimer.Start();
                    Searching = true;
                    Console.WriteLine($"Started Debounce Timer");
                }
            }
        }

        protected override void OnInit()
        {
            if (Data != null && Remote != null)
            {
                throw new InvalidOperationException("Cannot use local and remote data sources");
            }

            _debounceTimer = new Timer();
            _debounceTimer.Interval = Debounce;
            _debounceTimer.AutoReset = false;
            _debounceTimer.Elapsed += Search;
        }

        protected void HandleFocus()
        {
            EditMode = true;
            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                Searching = true;
                Console.WriteLine("Existing Search Continuing");
            }
        }

        protected void HandleKeypress(UIKeyboardEventArgs args)
        {
            if (args.Key == "Escape")
            {
                Searching = false;
            }
        }

        protected void Search(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine($"Debounce timer elapsed, beginning search for {_searchText}");
            if (Data != null)
            {
                SearchData(_searchText);
                StateHasChanged();
            }
            else
            {
                // Search Remote
            }
        }

        protected async Task SelectResult(TItem item)
        {
            Console.WriteLine($"Selecting Item: {item}");
            Item = item;
            await ItemChanged.InvokeAsync(item);
            Searching = false;
            EditMode = false;
        }

        private void SearchData(string query)
        {
            SearchResults = Data.Where(x => SearchOn(x).ToString().ToLower().Contains(query)).ToList();
        }

        public void Dispose()
        {
            _debounceTimer.Dispose();
        }
    }
}
