using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Blazored.Typeahead.DynamicComponent
{
    [DataContract]
    public class BlazoredTypeaheadConfigModel<TItem, TValue> : INotifyPropertyChanged, INotifyCollectionChanged
    {
        private bool _isMultiSelect;
        private TValue _value;
        private IList<TValue> _values;

        private ObservableDictionary<string, object> _additionalAttributes;
        private IDictionary<string, object> _attributes;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

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

        [DataMember]
        public bool IsMultiSelect
        {
            get => _isMultiSelect;
            set
            {
                _isMultiSelect = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public TValue Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public IList<TValue> Values
        {
            get => _values ?? new List<TValue>();
            set
            {
                _values = value;
                OnPropertyChanged();
            }
        }

        public ObservableDictionary<string, object> AdditionalAttributes
        {
            get => _additionalAttributes;
            set
            {
                if (_additionalAttributes != null)
                {
                    _additionalAttributes.CollectionChanged -= AdditionalAttributesOnCollectionChanged;
                }

                _additionalAttributes = value;

                if (_additionalAttributes != null)
                {
                    _additionalAttributes.CollectionChanged += AdditionalAttributesOnCollectionChanged;
                }

                OnPropertyChanged();
            }
        }

        public IReadOnlyDictionary<string, object> Attibutes => (IReadOnlyDictionary<string, object>) _attributes;

        public BlazoredTypeaheadConfigModel()
        {
            _attributes = new Dictionary<string, object>(20);
            _additionalAttributes = new ObservableDictionary<string, object>();
            _additionalAttributes.CollectionChanged += AdditionalAttributesOnCollectionChanged;

            // The renderfragments need to always be added as attributes, otherwise changes in these properties will not be shown
            ResetRenderTemplates();
        }

        private void ResetRenderTemplates()
        {
            SelectedTemplate = null;
            ResultTemplate = null;
            HelpTemplate = null;
            NotFoundTemplate = null;
            HeaderTemplate = null;
            FooterTemplate = null;
        }

        private void AdditionalAttributesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual T GetAttribute<T>(string property)
        {
            if (_attributes.TryGetValue(property, out var value))
            {
                return (T)value;
            }

            return default;
        }

        protected virtual void SetAttribute(string property, object value)
        {
            _attributes[property] = value;
            OnPropertyChanged(property);
        }

        public virtual T GetAdditionalAttribute<T>(string property)
        {
            if (_additionalAttributes.TryGetValue(property, out var value))
            {
                return (T)value;
            }

            return default;
        }

        public virtual void SetAdditionalAttribute(string property, object value)
        {
            _additionalAttributes[property] = value;
            OnPropertyChanged(nameof(AdditionalAttributes));
        }

        public virtual void SetAdditionalAttributes(IDictionary<string, object> newAttributes)
        {
            foreach (var (property, value) in newAttributes)
            {
                _additionalAttributes[property] = value;
            }

            OnPropertyChanged(nameof(AdditionalAttributes));
        }
        
        #region Serialization

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            // Call the constructor as this will init the collections
            this.GetType().GetConstructor(Array.Empty<Type>()).Invoke(this, null);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            // Because Values is of type IList, it will be deserialized into an array
            // The code however does not support working with an array and will throw an exception when adding or removing items
            Values = Values.ToList();
        }

        /// <summary>
        /// This should ONLY be set by the deserializer and not manually
        /// </summary>
        [DataMember] private Dictionary<string, object> SerializableAttributes
        {
            get => GetSerializableAttributes();
            set => _attributes = value;
        }

        /// <summary>
        /// This should ONLY be set by the deserializer and not manually
        /// </summary>
        [DataMember] private Dictionary<string, object> SerializableAdditionalAttributes
        {
            get => GetSerializableAdditionalAttributes();
            set => _additionalAttributes = new ObservableDictionary<string, object>(value);
        }

        protected virtual Dictionary<string, object> GetSerializableAdditionalAttributes()
        {
            return GetSerialiableProperties(_additionalAttributes)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        protected virtual Dictionary<string, object> GetSerializableAttributes()
        {
            return GetSerialiableProperties(_attributes)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        protected virtual IEnumerable<KeyValuePair<string, object>> GetSerialiableProperties(IDictionary<string, object> serializableDictionary)
        {
            return serializableDictionary
                .Where(item =>
                {
                    if (item.Value == null)
                    {
                        return false;
                    }

                    return item.Value.GetType().IsPrimitive || item.Value is string;
                });
        }

        #endregion
    }
}