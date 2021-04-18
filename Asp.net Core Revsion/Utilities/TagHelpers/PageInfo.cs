using System;

namespace Asp.netCoreRevsion.Utilities.TagHelper
{
    public class PageInfo
    {
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }

        public int TotalPages
        {
            get { return (int)Math.Ceiling((double)TotalItems / ItemsPerPage); }
        }
    }
}
