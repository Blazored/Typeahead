window.blazoredTypeahead = {
    assemblyname: "Blazored.Typeahead",
    setFocus: (element) => {
        if (element) element.focus();
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