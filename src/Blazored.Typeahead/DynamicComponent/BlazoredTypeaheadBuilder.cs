using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using static Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers;

namespace Blazored.Typeahead.DynamicComponent
{
    public class BlazoredTypeaheadBuilder<TItem, TValue>
    {
        protected IDictionary<string, object> attributes = new Dictionary<string, object>(20);
        protected IDictionary<string, object> additionalAttributes = new Dictionary<string, object>();
        protected Action stateHasChanged;

        protected Func<IList<TValue>> GetValues { get; set; }
        protected Action<IList<TValue>> SetValues { get; set; }

        protected Func<TValue> GetValue { get; set; }
        protected Action<TValue> SetValue { get; set; }

        public TValue Value
        {
            get => GetValue();
            set
            {
                SetValue(value);
                stateHasChanged();
            }
        }

        public IList<TValue> Values
        {
            get => GetValues();
            set
            {
                SetValues(value);
                stateHasChanged();
            }
        }

        public bool IsMultiSelect { get; protected set; }

        public void ChangeToSingleSelect(Func<TValue> getValue, Action<TValue> setValue)
        {
            GetValue = getValue;
            SetValue = setValue;
            IsMultiSelect = false;
            stateHasChanged?.Invoke();
        }

        public void ChangeToMultiSelect(Func<IList<TValue>> getValues, Action<IList<TValue>> setValues)
        {
            GetValues = getValues;
            SetValues = setValues;
            IsMultiSelect = true;
            stateHasChanged?.Invoke();
        }

        public Func<string, Task<IEnumerable<TItem>>> SearchMethod
        {
            get => GetAttribute<Func<string, Task<IEnumerable<TItem>>>>(nameof(BlazoredTypeahead<object, object>.SearchMethod));
            set => SetAttribute(nameof(BlazoredTypeahead<object, object>.SearchMethod), value);
        }

        public Func<TItem, TValue> ConvertMethod
        {
            get => GetAttribute<Func<TItem, TValue>>(nameof(BlazoredTypeahead<object, object>.ConvertMethod));
            set => SetAttribute(nameof(BlazoredTypeahead<object, object>.ConvertMethod), value);
        }

        public Func<string, Task<TItem>> AddItemOnEmptyResultMethod
        {
            get => GetAttribute<Func<string, Task<TItem>>>(nameof(BlazoredTypeahead<object, object>.AddItemOnEmptyResultMethod));
            set => SetAttribute(nameof(BlazoredTypeahead<object, object>.AddItemOnEmptyResultMethod), value);
        }

        public RenderFragment<TValue> SelectedTemplate
        {
            get => GetAttribute<RenderFragment<TValue>>(nameof(BlazoredTypeahead<object, object>.SelectedTemplate));
            set => SetAttribute(nameof(BlazoredTypeahead<object, object>.SelectedTemplate), value);
        }

        public RenderFragment<TItem> ResultTemplate
        {
            get => GetAttribute<RenderFragment<TItem>>(nameof(BlazoredTypeahead<object, object>.ResultTemplate));
            set => SetAttribute(nameof(BlazoredTypeahead<object, object>.ResultTemplate), value);
        }

        public RenderFragment HelpTemplate
        {
            get => GetAttribute<RenderFragment>(nameof(BlazoredTypeahead<object, object>.HelpTemplate));
            set => SetAttribute(nameof(BlazoredTypeahead<object, object>.HelpTemplate), value);
        }

        public RenderFragment<string> NotFoundTemplate
        {
            get => GetAttribute<RenderFragment<string>>(nameof(BlazoredTypeahead<object, object>.NotFoundTemplate));
            set => SetAttribute(nameof(BlazoredTypeahead<object, object>.NotFoundTemplate), value);
        }

        public RenderFragment HeaderTemplate
        {
            get => GetAttribute<RenderFragment>(nameof(BlazoredTypeahead<object, object>.HeaderTemplate));
            set => SetAttribute(nameof(BlazoredTypeahead<object, object>.HeaderTemplate), value);
        }

        public RenderFragment FooterTemplate
        {
            get => GetAttribute<RenderFragment>(nameof(BlazoredTypeahead<object, object>.FooterTemplate));
            set => SetAttribute(nameof(BlazoredTypeahead<object, object>.FooterTemplate), value);
        }

        public int? MinimumLength
        {
            get => GetAttribute<int?>(nameof(BlazoredTypeahead<object, object>.MinimumLength));
            set => SetAttribute(nameof(BlazoredTypeahead<object, object>.MinimumLength), value);
        }

        public int? Debounce
        {
            get => GetAttribute<int?>(nameof(BlazoredTypeahead<object, object>.Debounce));
            set => SetAttribute(nameof(BlazoredTypeahead<object, object>.Debounce), value);
        }

        public int? MaximumSuggestions
        {
            get => GetAttribute<int?>(nameof(BlazoredTypeahead<object, object>.MaximumSuggestions));
            set => SetAttribute(nameof(BlazoredTypeahead<object, object>.MaximumSuggestions), value);
        }

        public bool? Disabled
        {
            get => GetAttribute<bool?>(nameof(BlazoredTypeahead<object, object>.Disabled));
            set => SetAttribute(nameof(BlazoredTypeahead<object, object>.Disabled), value);
        }

        public bool? EnableDropDown
        {
            get => GetAttribute<bool?>(nameof(BlazoredTypeahead<object, object>.EnableDropDown));
            set => SetAttribute(nameof(BlazoredTypeahead<object, object>.EnableDropDown), value);
        }

        public bool? ShowDropDownOnFocus
        {
            get => GetAttribute<bool?>(nameof(BlazoredTypeahead<object, object>.ShowDropDownOnFocus));
            set => SetAttribute(nameof(BlazoredTypeahead<object, object>.ShowDropDownOnFocus), value);
        }

        public bool? DisableClear
        {
            get => GetAttribute<bool?>(nameof(BlazoredTypeahead<object, object>.DisableClear));
            set => SetAttribute(nameof(BlazoredTypeahead<object, object>.DisableClear), value);
        }

        public string Placeholder
        {
            get => GetAttribute<string>("placeholder");
            set => SetAttribute("placeholder", value);
        }

        public bool? StopPropagation
        {
            get => GetAttribute<bool?>(nameof(BlazoredTypeahead<object, object>.StopPropagation));
            set => SetAttribute(nameof(BlazoredTypeahead<object, object>.StopPropagation), value);
        }

        public bool? PreventDefault
        {
            get => GetAttribute<bool?>(nameof(BlazoredTypeahead<object, object>.PreventDefault));
            set => SetAttribute(nameof(BlazoredTypeahead<object, object>.PreventDefault), value);
        }

        private BlazoredTypeaheadBuilder()
        {
            // The renderfragments need to always be added as attributes, otherwise changes in these properties will not be shown
            SelectedTemplate = null;
            ResultTemplate = null;
            HelpTemplate = null;
            NotFoundTemplate = null;
            HeaderTemplate = null;
            FooterTemplate = null;
        }

        public BlazoredTypeaheadBuilder(Func<TValue> getValue, Action<TValue> setValue)
            : this()
        {
            GetValue = getValue;
            SetValue = setValue;
            IsMultiSelect = false;
        }

        public BlazoredTypeaheadBuilder(Func<IList<TValue>> getValues, Action<IList<TValue>> setValues)
            : this()
        {
            GetValues = getValues;
            SetValues = setValues;
            IsMultiSelect = true;
        }

        public BlazoredTypeaheadBuilder(BlazoredTypeaheadConfigModel<TItem, TValue> configModel, bool renderOnModelChange = false)
        {
            if (configModel == null)
            {
                throw new ArgumentNullException(nameof(configModel));
            }

            if (renderOnModelChange)
            {
                configModel.PropertyChanged += ConfigModelOnPropertyChanged;
                configModel.CollectionChanged += ConfigModelOnCollectionChanged;
            }

            IsMultiSelect = configModel.IsMultiSelect;

            if (configModel.IsMultiSelect)
            {
                GetValues = () => configModel.Values;
                SetValues = values => configModel.Values = values;
            }
            else
            {
                GetValue = () => configModel.Value;
                SetValue = value => configModel.Value = value;
            }

            attributes = (IDictionary<string, object>) configModel.Attibutes;
            additionalAttributes = configModel.AdditionalAttributes;
        }

        private void ConfigModelOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var configModel = (BlazoredTypeaheadConfigModel<TItem, TValue>) sender;
            additionalAttributes = configModel.AdditionalAttributes;
            stateHasChanged?.Invoke();
        }

        private void ConfigModelOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var configModel = (BlazoredTypeaheadConfigModel<TItem, TValue>) sender;

            switch (args.PropertyName)
            {
                // Multiselect is a special case as it has custom logic attached to it
                case nameof(configModel.IsMultiSelect):
                    IsMultiSelect = configModel.IsMultiSelect;

                    if (configModel.IsMultiSelect)
                    {
                        GetValues = () => configModel.Values;
                        SetValues = values => configModel.Values = values;
                    }
                    else
                    {
                        GetValue = () => configModel.Value;
                        SetValue = value => configModel.Value = value;
                    }

                    stateHasChanged?.Invoke();
                    break;
                default:
                    attributes = (IDictionary<string, object>) configModel.Attibutes;
                    stateHasChanged?.Invoke();
                    break;
            }
        }

        protected virtual T GetAttribute<T>(string property)
        {
            if (attributes.TryGetValue(property, out var value))
            {
                return (T) value;
            }

            return default;
        }

        protected virtual void SetAttribute(string property, object value)
        {
            attributes[property] = value;
            stateHasChanged?.Invoke();
        }

        protected virtual void SetAttributes(IDictionary<string, object> newAttributes)
        {
            foreach (var (property, value) in newAttributes)
            {
                attributes[property] = value;
            }

            stateHasChanged?.Invoke();
        }

        public virtual T GetAdditionalAttribute<T>(string property)
        {
            if (additionalAttributes.TryGetValue(property, out var value))
            {
                return (T)value;
            }

            return default;
        }

        public virtual void SetAdditionalAttribute(string property, object value)
        {
            additionalAttributes[property] = value;
            stateHasChanged?.Invoke();
        }

        public virtual void SetAdditionalAttributes(IDictionary<string, object> newAttributes)
        {
            foreach (var (property, value) in newAttributes)
            {
                additionalAttributes[property] = value;
            }

            stateHasChanged?.Invoke();
        }

        public virtual RenderFragment BuildTypeaheadComponent(Action stateHasChangedMethod) => __builder =>
        {
            if (stateHasChangedMethod == null)
            {
                throw new ArgumentNullException(nameof(stateHasChangedMethod));
            }

            var index = 0;
            stateHasChanged = stateHasChangedMethod;

            __builder.OpenComponent<BlazoredTypeahead<TItem, TValue>>(index++);

            BuildComponent(ref __builder, ref index);

            __builder.CloseComponent();
        };

        protected virtual void BuildComponent(ref RenderTreeBuilder __builder, ref int index)
        {
            Expression<Func<IList<TValue>>> valuesExpression = () => Values;
            Expression<Func<TValue>> valueExpression = () => Value;

            if (IsMultiSelect)
            {
                Debug.Assert(GetValues != null && SetValues != null);
                __builder.AddAttribute(index++, nameof(BlazoredTypeahead<object, object>.Values), TypeCheck(Values));
                __builder.AddAttribute(index++, "ValuesChanged", TypeCheck(EventCallback.Factory.Create(this, CreateInferredEventCallback(this, __value => Values = __value, Values))));
                __builder.AddAttribute(index++, "ValuesExpression", valuesExpression);

                // The typeahead component interally uses these fields to decide if the component is multi select or not, so we reset these fields
                __builder.AddAttribute(index++, nameof(BlazoredTypeahead<object, object>.Value), (object)null);
                __builder.AddAttribute(index++, "ValueChanged", default(EventCallback<TValue>));
                __builder.AddAttribute(index++, "ValueExpression", (object)null);
            }
            else
            {
                Debug.Assert(GetValue != null && SetValue != null);
                __builder.AddAttribute(index++, nameof(BlazoredTypeahead<object, object>.Value), TypeCheck(Value));
                __builder.AddAttribute(index++, "ValueChanged", TypeCheck(EventCallback.Factory.Create(this, CreateInferredEventCallback(this, __value => Value = __value, Value))));
                __builder.AddAttribute(index++, "ValueExpression", valueExpression);

                // The typeahead component interally uses these fields to decide if the component is multi select or not, so we reset these fields
                __builder.AddAttribute(index++, nameof(BlazoredTypeahead<object, object>.Values), (object)null);
                __builder.AddAttribute(index++, "ValuesChanged", default(EventCallback<IList<TValue>>));
                __builder.AddAttribute(index++, "ValuesExpression", (object)null);
            }

            __builder.AddMultipleAttributes(index++, attributes);

            __builder.AddMultipleAttributes(index++,
                additionalAttributes.Where(item => !attributes.ContainsKey(item.Key)));
        }
    }
}