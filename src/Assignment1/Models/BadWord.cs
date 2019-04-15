using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment1.Models
{
    public class BadWord
    {
        public int BadWordId
        {
            get;
            set;
        }

        [required]
        public String Word {
            get;
            set;
        }
    }
}
