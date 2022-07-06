using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class Licence
    {
        public Licence(string hostName)
        {
            HostName = hostName;
            AddUserDate = DateTime.Now;
            Status = false;
        }

        public Licence() { }

        public int Id { get; set; }
        public string HostName { get; set; }
        public DateTime AddUserDate { get; set; }
                
        public bool Status { get; set; }
        public DateTime LicApplyDate { get; set; }

    }
}    

    

