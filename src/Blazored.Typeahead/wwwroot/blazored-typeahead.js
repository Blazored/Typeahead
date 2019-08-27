window.blazoredTypeahead = {
    assemblyname: "Blazored.Typeahead",
    setFocus: (element) => {
        element.focus();
    },
    addFocusoutEventListener: (element) => {
        element.addEventListener("focusout", (event) => {
            if (element.contains(event.relatedTarget)
                || event.target.className == "blazored-typeahead__input-mask") // workaround relatedTarget bug
            {
                return; // don't react to this since it's a child.
            }
            DotNet.invokeMethodAsync(blazoredTypeahead.assemblyname, "OnFocusOut");
        })
    },
        // No need to remove the event listeners later, the browser will clean this up automagically.
    addEscapeEventListener: (element) => {
        element.addEventListener('keydown', (event) => {
            const key = event.key;
            if (key === "Escape") {
                DotNet.invokeMethodAsync(blazoredTypeahead.assemblyname, "OnEscape");
            }
        });
    }
}