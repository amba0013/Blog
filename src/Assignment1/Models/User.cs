using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment1.Models
{
    public class User
    {
        public int UserId {
            get;
            set;
        }
        [required]
        [ForeignKey("RoleId")]
        public int RoleId
        { 
            get;
            set;
        }
        [required]
        public string FirstName {
            get;
            set;
        }
        [required]
        public string LastName
        {
            get;
            set;
        }
        [required]
        public string EmailAddress
        {
            get;
            set;
        }
        [required]
        public string Password
        {
            get;
            set;
        }
        [required]
        public DateTime DateOfBirth {
            get;
            set;
        }

        [required]
        public String City {
            get;
            set;
        }

        [required]
        public String Address {
            get;
            set;
        }


        [required]
        public String PostalCode
        {
            get;
            set;
        }


        [required]
        public String Country {
            get;
            set;
        }
       
    }
}
