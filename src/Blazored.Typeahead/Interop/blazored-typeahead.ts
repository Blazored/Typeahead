namespace Blazored.Typeahead {
    class Typeahead {
        public SetFocus(element: HTMLElement): void {
            element.focus();
        }
    }

    export function Load(): void {
        const typeahead: any = {
            Typeahead: new Typeahead()
        };

        if (window['Blazored']) {
            window['Blazored'] = {
                ...window['Blazored'],
                ...typeahead
            }
        } else {
            window['Blazored'] = {
                ...typeahead
            }
        }
    }
}

Blazored.Typeahead.Load();
