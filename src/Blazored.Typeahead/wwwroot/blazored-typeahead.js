window.blazoredTypeahead = {
    assemblyname: "Blazored.Typeahead",
    setFocus: (element) => {
        element.focus();
    },
    addFocusoutEventListener: (element) => {
        element.addEventListener("focusout", (event) => {
            if (element.contains(document.activeElement) || event.target.className.includes("blazored-typeahead")) // workaround relatedTarget bug
            {
                return; // don't react to this since it's a child.
            }
            DotNet.invokeMethodAsync(blazoredTypeahead.assemblyname, "OnFocusOut");
        }, true);
    },
    // No need to remove the event listeners later, the browser will clean this up automagically.
    addKeyDownEventListener: (element) => {
        element.addEventListener('keydown', (event) => {
            const key = event.key;

            if (key === "Enter") {
                event.preventDefault();
            }
        });
    }
}