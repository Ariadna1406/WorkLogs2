using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class AccessGroup
    {
        public string Name { get; set; }
        public AppDbContext Context { get; set; }
        public List<User> UserList { get; set; }

        public AccessGroup(string name, AppDbContext context)
        {
            Name = name;
            Context = context;
                  
        }


    }
}
