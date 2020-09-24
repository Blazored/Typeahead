"use strict";

window.blazoredTypeahead = {
    assemblyname: "Blazored.Typeahead",
    setFocus: function (element) {
        if (element && element.focus) element.focus();
    },
    // No need to remove the event listeners later, the browser will clean this up automagically.
    addKeyDownEventListener: function (element) {
        if (element) {
            element.addEventListener('keydown', function (event) {
                var key = event.key;

                if (key === "Enter") {
                    event.preventDefault();
                }
            });
        }
    }
};
