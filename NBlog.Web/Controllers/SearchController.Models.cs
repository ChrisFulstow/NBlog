using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NBlog.Web.Application;

namespace NBlog.Web.Controllers
{
    public partial class SearchController
    {
        public class IndexModel : LayoutModel
        {
            public string QueryText { get; set; }
            public IEnumerable<SearchResultModel> Results { get; set; }
        }

        public class SearchResultModel
        {
            public string Slug { get; set; }
            public string Title { get; set; }
            public string Date { get; set; }
        }
    }
}