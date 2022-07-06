using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication5.Models;

namespace WebApplication5.Controllers
{
    
    public static class RespositoryService<T> where T : Project
    {
        public static Project Find(AppDbContext appDbContext, int? Id)
        {
            if (Id.HasValue)
            {
                var projectSet = appDbContext.ProjectSet.Where(x => x.Id == Id);
                if (projectSet.Count() > 0)
                {
                    return projectSet.First();
                }
                        
            }
            return null;
        }
    }
}
