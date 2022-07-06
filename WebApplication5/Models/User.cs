using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string AD_GUID { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        [NotMapped]
        public string PhoneNumber { get; set; }
        string fullName = string.Empty;

        [ForeignKey("DepartId")]
        public Department Department { get; set; }

        [NotMapped]
        int IdDepart { get; set; }

        [NotMapped]
        public bool NeedToImport{get; set;}

        public string FullName
        {
            get
            {
                return fullName;
            }
            set
            {
                fullName = value; // fullName = String.Format("{0} {1} {2}",  LastName, FirstName, MiddleName);
            }
        }

        string nameFromAD = string.Empty;
        public string NameFromAD
        {
            get
            {
                return nameFromAD;
            }
            set
            {
                nameFromAD = String.Format("{0} {1}. {2}", FirstName, MiddleName.First(), LastName );
            }
        }
    }
}
            
    

