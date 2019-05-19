using Microsoft.AspNetCore.Components;
using System;
using System.Linq.Expressions;

namespace Blazored.Typeahead
{
    public class BlazoredTypeaheadBase : ComponentBase
    {
        [Parameter] protected string Placeholder { get; set; }

    }
}
