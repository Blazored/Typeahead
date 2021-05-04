using Blazored.Typeahead.DynamicComponent;
using Microsoft.AspNetCore.Components;

namespace Blazored.Typeahead
{
    public partial class BlazoredTypeaheadModel<TItem, TValue>
    {
        [Parameter]
        public BlazoredTypeaheadConfigModel<TItem, TValue> ConfigModel { get; set; }

        private BlazoredTypeaheadConfigModel<TItem, TValue> _configModel;
        private BlazoredTypeaheadBuilder<TItem, TValue> _builder;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            // The builder will rerender on change, so only recreate the builder when the reference to the object is different
            if (_builder != null && ConfigModel.Equals(_configModel))
            {
                return;
            }

            _builder = new BlazoredTypeaheadBuilder<TItem, TValue>(ConfigModel, true);
            _configModel = ConfigModel;
        }
    }
}
