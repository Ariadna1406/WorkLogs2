using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class CorRepository
    {
        private readonly AppDbContext context;

        public CorRepository(AppDbContext context)
        {
            this.context = context;
        }

        public IQueryable<Corrections> GetCorrection()
        {
            return context.Cors;
        }
        
        public int SaveCorrection(Corrections entity)
        {
            if (entity.Id == 0)
                context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Added;

            context.SaveChanges();

            return entity.Id;
        }
    }
}
