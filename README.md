# Blazored Typeahead
Typeahead control for Blazor applications.

[![Build Status](https://dev.azure.com/blazored/Typeahead/_apis/build/status/Blazored.Typeahead?branchName=master)](https://dev.azure.com/blazored/Typeahead/_build/latest?definitionId=10&branchName=master)

![Nuget](https://img.shields.io/nuget/v/blazored.typeahead.svg)

## Important Notice For Server-side Blazor Apps
There is currently an issue with [Server-side Blazor apps](https://devblogs.microsoft.com/aspnet/aspnet-core-3-preview-2/#sharing-component-libraries) (not Client-side Blazor). They are unable to import static assets from component libraries such as this one. 

You can still use this package, however, you will need to manually add the CSS file to your Server-side Blazor projects `wwwroot` folder. Then you will need to reference it in your main `_Layout.cshtml`.

Alternatively, there is a great package by [Mister Magoo](https://github.com/SQL-MisterMagoo/BlazorEmbedLibrary) which offers a solution to this problem without having to manually copy files.

### Installing

You can install from Nuget using the following command:

`Install-Package Blazored.Typeahead`

Or via the Visual Studio package manger.

## Usage
I would suggest you add the following using statement to your main `_Imports.razor` to make referencing the component a bit easier.

```
@using Blazored.Typeahead
```

Once you've done this you can add the control to a component like so.

```
<BlazoredTypeahead></BlazoredTypeahead>
```

### Local Data Example

```
<BlazoredTypeahead Data="@SearchFilms"
                   bind-Item="@SelectedFilm">
    <SelectedTemplate>
        @context.Title
    </SelectedTemplate>
    <ResultTemplate>
        @context.Title (@context.Year)
    </ResultTemplate>
</BlazoredTypeahead>

@functions {

    [Parameter] protected List<Film> Films { get; set; }

    private async Task<List<Film>> SearchFilms(string searchText) 
    {
        return await Task.FromResult(Films.Where(x => x.Title.ToLower().Contains(searchText.ToLower())).ToList());
    }

}
```
In the example above, the component is setup with the minimum requirements. You must provide a method which has the following signature `Task<List<T> MethodName(string searchText)`, to the `Data` parameter. The control will call this method with the current search text everytime the debounce timer expires (default: 300ms). You must also set a value for the `Item` parameter. This will be populated with the item selected from the search results.

The component requires two templates to be provided...

- `SelectedTemplate`
- `ResultTemplates`

The `SelectedTemplate` is used to display the selected item and the `ResultTemplate` is used to display each result in the search list.


### Remote Data Example

```
@inject HttpClient httpClient

<BlazoredTypeahead Data="@SearchFilms"
                   bind-Item="@SelectedFilm"
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

@functions {

    [Parameter] protected List<Film> Films { get; set; }

    private async Task<List<Film>> SearchFilms(string searchText) 
    {
        var response = await httpClient.GetJsonAsync<List<Film>>($"https://allfilms.com/api/films/?title={searchText}");
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

**Parameters**

- Item (Required) - Used for binding local field to selected item on control
- Data (Required) - Method to call when performing a search
- Placeholder (Optional) - Allows user to specify a placeholder message
- MinimumLength (Optional - Default 1) - Minimum number of characters before starting a search
- Debounce (Optional - Default 300) - Time to wait after last keypress before starting a search

