# Blazored Typeahead
Typeahead control for Blazor applications.

[![Build Status](https://dev.azure.com/blazored/Typeahead/_apis/build/status/Blazored.Typeahead?branchName=master)](https://dev.azure.com/blazored/Typeahead/_build/latest?definitionId=10&branchName=master)

![Nuget](https://img.shields.io/nuget/v/blazored.typeahead.svg)

### Installing

You can install from NuGet using the following command:

`Install-Package Blazored.Typeahead`

Or via the Visual Studio package manger.

### Setup
If will need to include the following CSS and JS files in your `index.html` (Blazor WebAssembly) or `_Host.cshtml` (Blazor Server).

In the `head` tag add the following CSS.

```html
<link href="_content/Blazored.Typeahead/blazored-typeahead.css" rel="stylesheet" />
```

Then add the JS script at the bottom of the page using the following script tag.

```html
<script src="_content/Blazored.Typeahead/blazored-typeahead.js"></script>
```

## Usage
The component comes in two flavors, standalone and forms integrated.

### Forms Integrated
This version of the component is integrated with the forms and authentication system Blazor provides out the of box. 

I would suggest adding the following using statement to your main `_Imports.razor` to make referencing the component a bit easier.

```cs
@using Blazored.Typeahead.Forms
```

### Local Data Example
```cs
<EditForm Model="@MyFormModel" OnValidSubmit="@HandlValidSubmit">
    <BlazoredTypeaheadInput SearchMethod="@SearchFilms"
                            @bind-Value="@MyFormModel.SelectedFilm">
        <SelectedTemplate>
            @context.Title
        </SelectedTemplate>
        <ResultTemplate>
            @context.Title (@context.Year)
        </ResultTemplate>
    </BlazoredTypeaheadInput>
    <ValidationMessage For="@(() => MyFormModel.SelectedFilm)" />
</EditForm>

@code {

    [Parameter] protected IEnumerable<Film> Films { get; set; }

    private async Task<IEnumerable<Film>> SearchFilms(string searchText) 
    {
        return await Task.FromResult(Films.Where(x => x.Title.ToLower().Contains(searchText.ToLower())).ToList());
    }

}
```
In the example above, the component is setup with the minimum requirements. You must provide a method which has the following signature `Task<IEnumerable<T> MethodName(string searchText)`, to the `SearchMethod` parameter. The control will call this method with the current search text everytime the debounce timer expires (default: 300ms). You must also set a value for the `Value` parameter. This will be populated with the item selected from the search results. As this version of the control is integrated with Blazors built-in forms and validation, it must be wrapped in a `EditForm` component.

The component requires two templates to be provided...

- `SelectedTemplate`
- `ResultTemplates`

The `SelectedTemplate` is used to display the selected item and the `ResultTemplate` is used to display each result in the search list.


### Remote Data Example

```cs
@inject HttpClient httpClient

<EditForm Model="@MyFormModel" OnValidSubmit="@HandlValidSubmit">
    <BlazoredTypeahead SearchMethod="@SearchFilms"
                       @bind-Value="@SelectedFilm"
                       Debounce="500">
        <SelectedTemplate>
            @context.Title
        </SelectedTemplate>
        <ResultTemplate>
            @context.Title (@context.Year)
        </ResultTemplate>
        <NotFoundTemplate>
            Sorry, there weren't any search results.
        </NotFoundTemplate>
    </BlazoredTypeahead>
    <ValidationMessage For="@(() => MyFormModel.SelectedFilm)" />
</EditForm>

@code {

    [Parameter] protected IEnumerable<Film> Films { get; set; }

    private async Task<IEnumerable<Film>> SearchFilms(string searchText) 
    {
        var response = await httpClient.GetJsonAsync<IEnumerable<Film>>($"https://allfilms.com/api/films/?title={searchText}");
        return response;
    }

}
```
Because you provide the search method to the component, making a remote call is really straight-forward. In this example, the `Debounce` parameter has been upped to 500ms and the `NotFoundTemplate` has been specified.

### Standalone
I would suggest adding the following using statement to your main `_Imports.razor` to make referencing the component a bit easier.

```cs
@using Blazored.Typeahead
```

### Local Data Example
```cs
<BlazoredTypeahead SearchMethod="@SearchFilms"
                   @bind-Value="@SelectedFilm">
    <SelectedTemplate>
        @context.Title
    </SelectedTemplate>
    <ResultTemplate>
        @context.Title (@context.Year)
    </ResultTemplate>
</BlazoredTypeahead>

@code {

    [Parameter] protected IEnumerable<Film> Films { get; set; }

    private async Task<IEnumerable<Film>> SearchFilms(string searchText) 
    {
        return await Task.FromResult(Films.Where(x => x.Title.ToLower().Contains(searchText.ToLower())).ToList());
    }

}
```
In the example above, the component is setup with the minimum requirements. You must provide a method which has the following signature `Task<IEnumerable<T> MethodName(string searchText)`, to the `SearchMethod` parameter. The control will call this method with the current search text everytime the debounce timer expires (default: 300ms). You must also set a value for the `Value` parameter. This will be populated with the item selected from the search results.

The component requires two templates to be provided...

- `SelectedTemplate`
- `ResultTemplates`

The `SelectedTemplate` is used to display the selected item and the `ResultTemplate` is used to display each result in the search list.


### Remote Data Example

```cs
@inject HttpClient httpClient

<BlazoredTypeahead SearchMethod="@SearchFilms"
                   @bind-Value="@SelectedFilm"
                   Debounce="500">
    <SelectedTemplate>
        @context.Title
    </SelectedTemplate>
    <ResultTemplate>
        @context.Title (@context.Year)
    </ResultTemplate>
    <NotFoundTemplate>
        Sorry, there weren't any search results.
    </NotFoundTemplate>
</BlazoredTypeahead>

@code {

    [Parameter] protected IEnumerable<Film> Films { get; set; }

    private async Task<IEnumerable<Film>> SearchFilms(string searchText) 
    {
        var response = await httpClient.GetJsonAsync<IEnumerable<Film>>($"https://allfilms.com/api/films/?title={searchText}");
        return response;
    }

}
```
Because you provide the search method to the component, making a remote call is really straight-forward. In this example, the `Debounce` parameter has been upped to 500ms and the `NotFoundTemplate` has been specified.


### Full Options List
Below is a list of all the options available on the Typeahead.

**Templates**

- ResultTemplate (Required) - Allows the user to define a template for a result in the results list
- SelectedTemplate (Required) - Allows the user to define a template for a selected item
- NotFoundTemplate - Allows the user to define a template when no items are found
- FooterTemplate - Allows the user to define a template which is displayed at the end of the results list

**Parameters**

- Item (Required) - Used for binding local field to selected item on control
- Data (Required) - Method to call when performing a search
- Placeholder (Optional) - Allows user to specify a placeholder message
- MinimumLength (Optional - Default 1) - Minimum number of characters before starting a search
- Debounce (Optional - Default 300) - Time to wait after last keypress before starting a search

