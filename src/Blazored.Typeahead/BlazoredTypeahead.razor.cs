using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Timers;

namespace Blazored.Typeahead
{
    public partial class BlazoredTypeahead<TItem, TValue> : ComponentBase, IDisposable
    {
        private EditContext _editContext;
        private FieldIdentifier _fieldIdentifier;
        private Timer _debounceTimer;
        private string _searchText = string.Empty;
        private bool _eventsHookedUp = false;
        private ElementReference _searchInput;
        private ElementReference _mask;

        [Inject] private IJSRuntime JSRuntime { get; set; }

        [CascadingParameter] private EditContext CascadedEditContext { get; set; }

        [Parameter] public TValue Value { get; set; }
        [Parameter] public EventCallback<TValue> ValueChanged { get; set; }
        [Parameter] public Expression<Func<TValue>> ValueExpression { get; set; }

        [Parameter] public IList<TValue> Values { get; set; }
        [Parameter] public EventCallback<IList<TValue>> ValuesChanged { get; set; }
        [Parameter] public Expression<Func<IList<TValue>>> ValuesExpression { get; set; }

        [Parameter] public Func<string, Task<IEnumerable<TItem>>> SearchMethod { get; set; }
        [Parameter] public Func<TItem, TValue> ConvertMethod { get; set; }

        [Parameter] public RenderFragment NotFoundTemplate { get; set; }
        [Parameter] public RenderFragment HelpTemplate { get; set; }
        [Parameter] public RenderFragment<TItem> ResultTemplate { get; set; }
        [Parameter] public RenderFragment<TValue> SelectedTemplate { get; set; }
        [Parameter] public RenderFragment HeaderTemplate { get; set; }
        [Parameter] public RenderFragment FooterTemplate { get; set; }

        [Parameter(CaptureUnmatchedValues = true)] public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; }
        [Parameter] public int MinimumLength { get; set; } = 1;
        [Parameter] public int Debounce { get; set; } = 300;
        [Parameter] public int MaximumSuggestions { get; set; } = 10;
        [Parameter] public bool Disabled { get; set; } = false;
        [Parameter] public bool EnableDropDown { get; set; } = false;
        [Parameter] public bool ShowDropDownOnFocus { get; set; } = false;

        [Parameter] public bool StopPropagation { get; set; } = false;
        [Parameter] public bool PreventDefault { get; set; } = false;

        private bool IsSearching { get; set; } = false;
        private bool IsShowingSuggestions { get; set; } = false;
        private bool IsShowingMask { get; set; } = false;
        private TItem[] Suggestions { get; set; } = new TItem[0];
        private int SelectedIndex { get; set; }
        private bool ShowHelpTemplate { get; set; } = false;
        private string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;

                if (value.Length == 0)
                {
                    _debounceTimer.Stop();
                    SelectedIndex = -1;
                }
                else
                {
                    _debounceTimer.Stop();
                    _debounceTimer.Start();
                }
            }
        }

        private string FieldCssClasses => _editContext?.FieldCssClass(_fieldIdentifier) ?? "";
        private bool IsMultiselect => ValuesExpression != null;

        protected override void OnInitialized()
        {
            if (SearchMethod == null)
            {
                throw new InvalidOperationException($"{GetType()} requires a {nameof(SearchMethod)} parameter.");
            }

            if (ConvertMethod == null)
            {
                if (typeof(TItem) != typeof(TValue))
                {
                    throw new InvalidOperationException($"{GetType()} requires a {nameof(ConvertMethod)} parameter.");
                }

                ConvertMethod = item => item is TValue value ? value : default;
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

            _editContext = CascadedEditContext;
            _fieldIdentifier = IsMultiselect ? FieldIdentifier.Create(ValuesExpression) : FieldIdentifier.Create(ValueExpression);

            Initialize();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if ((firstRender && !Disabled) || (!_eventsHookedUp && !Disabled))
            {
                await Interop.AddKeyDownEventListener(JSRuntime, _searchInput);
                _eventsHookedUp = true;
            }
        }

        protected override void OnParametersSet()
        {
            Initialize();
        }

        private void Initialize()
        {
            SearchText = "";
            IsShowingSuggestions = false;
            IsShowingMask = Value != null;
        }

        private async Task RemoveValue(TValue item)
        {
            var valueList = Values ?? new List<TValue>();
            if (valueList.Contains(item))
            {
                valueList.Remove(item);
            }

            await ValuesChanged.InvokeAsync(valueList);
            _editContext?.NotifyFieldChanged(_fieldIdentifier);
        }

        private async Task HandleClear()
        {
            SearchText = "";
            IsShowingMask = false;

            if (IsMultiselect)
            {
                await ValuesChanged.InvokeAsync(new List<TValue>());
            }
            else
            {
                await ValueChanged.InvokeAsync(default);
            }

            _editContext?.NotifyFieldChanged(_fieldIdentifier);

            await Task.Delay(250); // Possible race condition here.
            await Interop.Focus(JSRuntime, _searchInput);
        }

        private async Task HandleClickOnMask()
        {
            SearchText = "";
            IsShowingMask = false;

            await Task.Delay(250); // Possible race condition here.
            await Interop.Focus(JSRuntime, _searchInput);
        }

        private async Task HandleKeyUpOnShowDropDown(KeyboardEventArgs args)
        {
            if (args.Key == "ArrowDown")
            {
                MoveSelection(1);
            }
            else if (args.Key == "ArrowUp")
            {
                MoveSelection(-1);
            }
            else if (args.Key == "Escape")
            {
                Initialize();
            }
            else if (args.Key == "Enter" && Suggestions.Length == 1)
            {
                await SelectTheFirstAndOnlySuggestion();
            }
            else if (args.Key == "Enter" && SelectedIndex >= 0 && SelectedIndex < Suggestions.Length)
            {
                await SelectResult(Suggestions[SelectedIndex]);
            }
            else if (args.Key == "Enter")
            {
                await ShowMaximumSuggestions();
            }
        }

        private async Task HandleKeyUpOnMask(KeyboardEventArgs args)
        {
            switch (args.Key)
            {
                case "Tab":
                    break; // Don't do anything on tab.
                case "Enter":
                case "Backspace":
                case "Delete":
                case "Escape":
                    await HandleClear();
                    break;
                default:
                    break;
            }

            // You can only start searching if it's not a special key (Tab, Enter, Escape, ...)
            if (args.Key.Length == 1)
            {
                IsShowingMask = false;
                await Task.Delay(250); // Possible race condition here.
                await Interop.Focus(JSRuntime, _searchInput);
                SearchText = args.Key;
            }
        }

        private async Task HandleKeyup(KeyboardEventArgs args)
        {
            if ((args.Key == "ArrowDown" || args.Key == "Enter") && EnableDropDown && !IsShowingSuggestions)
            {
                await ShowMaximumSuggestions();
            }

            if (args.Key == "ArrowDown")
            {
                MoveSelection(1);
            }
            else if (args.Key == "ArrowUp")
            {
                MoveSelection(-1);
            }
            else if (args.Key == "Escape")
            {
                Initialize();
            }
            else if (args.Key == "Enter" && Suggestions.Count() == 1)
            {
                await SelectTheFirstAndOnlySuggestion();
            }
            else if (args.Key == "Enter" && SelectedIndex >= 0 && SelectedIndex < Suggestions.Count())
            {
                await SelectResult(Suggestions[SelectedIndex]);
            }
            else if (IsMultiselect && args.Key == "Backspace" && (Values?.Any() ?? false) && SearchText.Length == 0)
            {   
                await RemoveValue(Values.Last());
            }
        }

        private async Task HandleInputFocus()
        {
            if (ShowDropDownOnFocus)
            {
                await ShowMaximumSuggestions();
            }
        }

        private bool _resettingControl = false;
        private async Task ResetControl()
        {
            if (!_resettingControl)
            {
                _resettingControl = true;
                await Task.Delay(200);
                Initialize();
                _resettingControl = false;
            }
        }

        private async Task ShowMaximumSuggestions()
        {
            if (_resettingControl)
            {
                while (_resettingControl)
                {
                    await Task.Delay(150);
                }
            }

            IsShowingSuggestions = !IsShowingSuggestions;

            if (IsShowingSuggestions)
            {
                SearchText = "";
                IsSearching = true;
                await InvokeAsync(StateHasChanged);

                Suggestions = (await SearchMethod?.Invoke(_searchText)).Take(MaximumSuggestions).ToArray();

                IsSearching = false;
                await InvokeAsync(StateHasChanged);
            }
        }

        private string GetSelectedSuggestionClass(TItem item, int index)
        {
            const string resultClass = "blazored-typeahead__active-item";
            TValue value = ConvertMethod(item);

            if (Equals(value, Value) || (Values?.Contains(value) ?? false))
            {
                if (index == SelectedIndex)
                {
                    return "blazored-typeahead__selected-item-highlighted";
                }

                return "blazored-typeahead__selected-item";
            }

            if (index == SelectedIndex)
            {
                return resultClass;
            }

            return Equals(value, Value) ? resultClass : string.Empty;
        }

        private async void Search(Object source, ElapsedEventArgs e)
        {
            if (_searchText.Length < MinimumLength)
            {
                ShowHelpTemplate = true;
                await InvokeAsync(StateHasChanged);
                return;
            }

            ShowHelpTemplate = false;
            IsSearching = true;
            await InvokeAsync(StateHasChanged);
            Suggestions = (await SearchMethod?.Invoke(_searchText)).Take(MaximumSuggestions).ToArray();

            IsSearching = false;
            IsShowingSuggestions = true;
            SelectedIndex = 0;
            await InvokeAsync(StateHasChanged);
        }

        private async Task SelectResult(TItem item)
        {
            var value = ConvertMethod(item);

            if (IsMultiselect)
            {
                var valueList = Values ?? new List<TValue>();

                if (valueList.Contains(value))
                    valueList.Remove(value);
                else
                    valueList.Add(value);

                await ValuesChanged.InvokeAsync(valueList);
            }
            else
            {
                Value = value;
                await ValueChanged.InvokeAsync(value);
            }

            _editContext?.NotifyFieldChanged(_fieldIdentifier);

            if (IsMultiselect)
            {
                await Interop.Focus(JSRuntime, _searchInput);
            }
            else
            {
                await Task.Delay(250);
                await Interop.Focus(JSRuntime, _mask);
            }
        }

        private bool ShouldShowHelpTemplate()
        {
            return SearchText.Length > 0 &&
                ShowHelpTemplate &&
                HelpTemplate != null;
        }

        private bool ShouldShowSuggestions()
        {
            return IsShowingSuggestions &&
                   Suggestions.Any() && !IsSearchingOrDebouncing;
        }

        private void MoveSelection(int count)
        {
            var index = SelectedIndex + count;

            if (index >= Suggestions.Length)
            {
                index = 0;
            }

            if (index < 0)
            {
                index = Suggestions.Length - 1;
            }

            SelectedIndex = index;
        }

        private Task SelectTheFirstAndOnlySuggestion()
        {
            SelectedIndex = 0;
            return SelectResult(Suggestions[SelectedIndex]);
        }

        private bool IsSearchingOrDebouncing => IsSearching || _debounceTimer.Enabled;
        private bool ShowNotFound()
        {
            return IsShowingSuggestions &&
                   !IsSearchingOrDebouncing &&
                   !Suggestions.Any();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _debounceTimer.Dispose();
            }
        }
    }
}
