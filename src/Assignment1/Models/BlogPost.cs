using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

//make these fields required once u done

namespace Assignment1.Models
{
    public class BlogPost
    {
        public int BlogPostId {
            get;
            set;
        }


        [ForeignKey("UserId")]
        public int UserId { //need to check this one. This should be a foreign key attribute
            get;
            set;
        }
        [required]
        public string Title {
            get;
            set;
        }

        [required]
        public String ShortDescription {
            get;
            set;
        }
        [required]
        public string Content {
            get;
            set;
        }
        [required]
        public DateTime Posted {
            get;
            set;
        }

        public bool IsAvailable {
            get;
            set;
        }


        
    }
}
