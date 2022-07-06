using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }


        [ForeignKey("HeadOfDepartmentId")]
        public User HeadOfDepartment { get; set; }

        
    }
}
