using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class UserSubs
    {
        [Key]
        public int Id { get; set; }
        public User SubstiteUser { get; set; }
        public User ReplacedUser { get; set; }

        public static List<User> GetUserSubs(AppDbContext context, User user)
        {
            var usersForReplace = context.UsersSubs.Where(x => x.SubstiteUser == user).Select(x => x.ReplacedUser);
            var subordinates = GetSubordinates(context, user).OrderBy(x=>x.FullName).ToList();
            subordinates.AddRange(usersForReplace);
            return subordinates;
        }

        static List<User> GetSubordinates(AppDbContext context, User user)
        {
            var IsHOD = user.isHeadOfDepartment(context);
            if (IsHOD)
            {
                var subordUsers = user.GetAllUsersFromMyDepart(context);
                return subordUsers;
            }
            return new List<User>();
        }

        public static UserSubs GetInst(AppDbContext context, User curUser, string userSubsSelected)
        {
            var userSubsSelSet = context.Users.Where(x => x.FullName == userSubsSelected);
            var userSubsSel = userSubsSelSet.Count() > 0 ? userSubsSelSet.First() : null;
            if (userSubsSel != null)
            {
                var subsInstSet = context.UsersSubs.Include(x => x.SubstiteUser).Include(x => x.ReplacedUser).Where(x => x.SubstiteUser == curUser && x.ReplacedUser == userSubsSel);
                if (subsInstSet.Count() > 0) return subsInstSet.First();
            }
            return null;
        }

    }
}
