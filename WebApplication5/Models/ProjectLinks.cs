using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class ProjectLinks
    {
        public int Id { get; set; }
        public Project Project { get; set; }
        public string ProjectPart { get; set; }
        public string RDPortalLink { get; set; }
        public string PDPortalLink { get; set; }
        public string ZPIPortalLink { get; set; }

        public ProjectLinks()
        {

        }
    }

   
}
