var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
var Blazored;
(function (Blazored) {
    var Typeahead;
    (function (Typeahead_1) {
        var Typeahead = /** @class */ (function () {
            function Typeahead() {
            }
            Typeahead.prototype.SetFocus = function (element) {
                element.focus();
            };
            return Typeahead;
        }());
        function Load() {
            var typeahead = {
                Typeahead: new Typeahead()
            };
            if (window['Blazored']) {
                window['Blazored'] = __assign({}, window['Blazored'], typeahead);
            }
            else {
                window['Blazored'] = __assign({}, typeahead);
            }
        }
        Typeahead_1.Load = Load;
    })(Typeahead = Blazored.Typeahead || (Blazored.Typeahead = {}));
})(Blazored || (Blazored = {}));
Blazored.Typeahead.Load();
//# sourceMappingURL=blazored.Typeahead.js.map