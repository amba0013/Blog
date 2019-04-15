using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment1.Models
{
    public class BlogPostVM
    {
        public BlogPost BlogPost { get; set; }

        public List<Comment> Comments { get; set; }

        public List<Photo> Photos { get; set; }

    }
}
