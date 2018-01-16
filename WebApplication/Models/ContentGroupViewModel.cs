using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class ContentGroupViewModel
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public ICollection<ContentElement> ContentElements { get; set; }

        public ContentGroupViewModel()
        {
            this.ContentElements = new List<ContentElement>();
        }
    }
}