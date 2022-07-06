using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class KindOfAct
    {

        public int Id { get; set; }
        public string Name { get; set; }

        public KindOfAct(string name)
        {
            Name = name;
        }

      
    }

    
}
