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

The control has two modes, local and remote. In local mode, you must provide the data and which property to search on. In remote mode, you provide a remote URL to query such as an API endpoint.

### Local Mode Example
```
<BlazoredTypeahead Data="@People"
                   SearchOn="@(x => x.Firstname)"
                   bind-Item="@SelectedPerson">
    <SelectedTemplate>
        @context.Firstname
    </SelectedTemplate>
    <ResultTemplate>
        @context.Firstname @context.Lastname
    </ResultTemplate>
</BlazoredTypeahead>
```

The code above is the minimum required to use the control in local mode. `Data` is the list of people the control will search on. The `SearchOn` parameter is used to tell the control which field to search. Finally, `bind-Item` is used to bind the selected item to a local field.

The example also shows the two required templates, `SelectedTemplate` and `ResultTemplate`. The `SelectedTemplate` is used when an item is selected from the results list. The `ResultTemplate` is used for each result in the results list when searching.

### Remote Mode Example
```
<BlazoredTypeahead Remote="https://somesite.com/api/persons/?name={query}"
                   TItem="Person"
                   bind-Item="@SelectedPerson">
    <SelectedTemplate>
        @context.Firstname @context.Lastname
    </SelectedTemplate>
    <ResultTemplate>
        @context.Firstname @context.Lastname
    </ResultTemplate>
</BlazoredTypeahead>
```

The code above is the minimum required to use the control in remote mode. `Remote` is the URL you want the control to query. When providing this URL you must include the `{query}` placeholder. The control will replace this with the search text before making a request. You must also provide the type of the result object you're expecting from the API using the `TItem` property. Again, `bind-Item` is used to bind the selected item to a local field.

The example again shows the two required templates, `SelectedTemplate` and `ResultTemplate`.

### Full Options List
Below is a list of all the options available on the Typeahead.

**Templates**

- ResultTemplate (Required) - Allows the user to define a template for a result in the results list
- SelectedTemplate (Required) - Allows the user to define a template for a selected item
- NotFoundTemplate - Allows the user to define a template when no items are found

**Parameters**

- Item (Required) - Used for binding local field to selected item on control
- Data (Local Mode Only) - Collection to use for searching
- SearchOn (Local Mode Only) - Property used for searching
- Remote (Remote Mode Only) - URL of remoate API used for searching
- Placeholder (Optional) - Allows user to specify a placeholder message
- MinimumLength (Optional - Default 1) - Minimum number of characters before starting a search
- Debounce (Optional - Default 300) - Time to wait after last keypress before starting a search

