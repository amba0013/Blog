using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment1.Models
{
    public class Photo
    {
        public int PhotoId
        {
            get;
            set;
        }

        [required]
        [ForeignKey("BlogPostId")]
        public int BlogPostId
        {
            get;
            set;
        }
        [required]
        public String FileName {
            get;
            set;
        }

        [required]
        public String Url {
            get;
            set;
        }
    }
}
