var Blazored;
(function (Blazored) {
    var Typeahead;
    (function (Typeahead_1) {
        class Typeahead {
            SetFocus(element) {
                element.focus();
            }
        }
        function Load() {
            const typeahead = {
                Typeahead: new Typeahead()
            };
            if (window['Blazored']) {
                window['Blazored'] = Object.assign({}, window['Blazored'], typeahead);
            }
            else {
                window['Blazored'] = Object.assign({}, typeahead);
            }
        }
        Typeahead_1.Load = Load;
    })(Typeahead = Blazored.Typeahead || (Blazored.Typeahead = {}));
})(Blazored || (Blazored = {}));
Blazored.Typeahead.Load();
//# sourceMappingURL=blazored-typeahead.js.map