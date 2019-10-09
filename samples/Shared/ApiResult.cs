using System;
using System.Collections.Generic;
using System.Text;

namespace Sample.Shared
{
    public class ApiResult<T>
    {
        public T Data { get; set; }
        public int Count { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
    }
}
