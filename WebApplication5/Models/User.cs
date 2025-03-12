using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebApplication5.Models.StaticData;

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
        public string Position { get; set; }
        public string City { get; set; }
        public bool? IsProduction { get; set; }
        public string AdditionalDepartmentAcronym { get; set; }


        public bool? IsActive { get; set; }
        [NotMapped]
        AppDbContext AppDbContext { get; set; }
        public Role Role { get; set; }
        [NotMapped]
        public string PhoneNumber { get; set; }
        string fullName = string.Empty;

        [ForeignKey("DepartId")]
        public Department Department { get; set; }

        public string PublicDepart { get; set; }

        [NotMapped]
        int IdDepart { get; set; }

        [NotMapped]
        public bool NeedToImport { get; set; }

        public bool? IsHeadOfDepartment { get; set; }

        public bool? IgnoreUserInReport { get; set; }

        public bool IsGIP { get; set; }

        public double? Rate { get; set; }

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

        public bool? IngnoreInReportShares { get; set; }

        public bool IsGIPCheck(AppDbContext context)
        {
            if (IsGIP)
            {
                return true;
            }
            else
            {
                var departAcron = Department?.Acronym;
                if (departAcron == "БГИП" || departAcron == "ОГИП")
                {
                    return true;
                }
            }
            return false;
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
                nameFromAD = String.Format("{0} {1}. {2}", FirstName, MiddleName.First(), LastName);
            }
        }

        public DateTime? ImportDate { get; set; }
        public DateTime? BlockDate { get; set; }


        public void SetIsNotActive()
        {
            IsActive = false;
            BlockDate = DateTime.Now;
        }

        public List<string> GetOptionsOfGIPAcromym()
        {
            var returnList = new List<string>();
            returnList.Add(FullName); //Беликов Тимофей Петрович
            returnList.Add($"{LastName.First()}{FirstName.First()}{MiddleName.First()}"); //БТП            
            returnList.Add($"{LastName}{FirstName.First()}{MiddleName.First()}"); //БеликовТП
            return returnList;
        }

        public List<User> GetAllUsersFromMyDepart(AppDbContext context)
        {
            List<User> userList = new List<User>();
            var departAcr = GetDepartmentAcronym(context);
            List<User> userListFiltered = new List<User>();
            var userAr = context.Users.Include(x => x.Department).ToArray();
            userListFiltered = userAr.Where(x => x.GetDepartmentAcronym(context) == departAcr || x.AdditionalDepartmentAcronym==departAcr).Where(x => x.IsActive == true).ToList();                               
            userList.AddRange(userListFiltered);           

            return userList;
        }

        // Получение списка сотрудников того же отдела, что и текущий пользователь
        public static List<User> GetUsersFromCurrentDepartment(AppDbContext context, HttpContext httpContext)
        {
            var currentUser = GetUser(context, httpContext);
            var usersInDepartment = context.Users
                .Where(u => u.Department.Id == currentUser.Department.Id && u.IsActive == true)
                .ToList();
            return usersInDepartment;
        }

        // Получение Id текущего пользователя
        public static int GetCurrentUserId(AppDbContext context, HttpContext httpContext)
        {
            var currentUser = GetUser(context, httpContext);
            return currentUser != null ? currentUser.Id : 0;
        }

        public static User GetUser(AppDbContext context, HttpContext httpContext)
        {            
            var winUs = GetLogin(httpContext.User.Identity.Name); //nesterovig
            var userSet = context.Users.Include(x=>x.Department).Where(x => x.Login == winUs);
            return userSet.Count() > 0 ? userSet.First() : new User() { FullName = winUs };
        }

        public static User GetUserById(AppDbContext context, string Id)
        {
            var res = Int32.TryParse(Id, out int IdInt);
            if (res)
            {
                var userSet = context.Users.Where(x => x.Id == IdInt);
                    if (userSet.Count() > 0)
                {
                    return userSet.First();
                }
            }
            return null;
        }

        public static User GetUserByFullName(AppDbContext context, string fullName)
        {            
            var userSet = context.Users.Include(x => x.Department).Where(x => x.FullName == fullName);
            return userSet.Count() > 0 ? userSet.First() : null;
        }

        public static User[] GetUserKsp(AppDbContext context)
        {            
            var userAr= context.Users.Include(x => x.Role).Where(x => x.Role.Id == 3 || x.Id==497 || x.Id==1521 || x.Id== 1658).ToArray(); //497 - Мамедова, 1521 - Галеев Айнур, 1658 - Ибрагимов Ильфат

            return userAr;
        }

        static string GetLogin(string login)
        {
            if (!string.IsNullOrEmpty(login))
            {
                if (login.Contains("\\"))
                {
                    return login.Split("\\").Last();
                }
                return login;
            }
            else return "";
        }

        public bool isHeadOfDepartment(AppDbContext context)
        {          
           var listOfHOD = GetListOfHOD(context);
           var listOfHODeq= listOfHOD.Where(x => x.FullName.Equals(this.FullName));
            if (listOfHODeq.Count() > 0)
            {
                return true;
            }
            else 
            { 
                return false; 
            }
        }

        //public bool isHeadOfDepartment(AppDbContext context)
        //{
        //    var departSet = context.Departments.Include(x => x.HeadOfDepartment).Where(x => x.HeadOfDepartment != null);
        //    return true
        //}

        public bool IsAdmin(AppDbContext context)
        {
            var userSet = context.Users.Include(x => x.Role).Where(x => x.Id == Id);
            if (userSet.Count() > 0)
            {
                var user = userSet.First();
                if (user.Role != null && user.Role.Id == 2)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsKSP(AppDbContext context)
        {
            var userSet = context.Users.Include(x => x.Role).Where(x => x.Id == Id);
            if (userSet.Count() > 0)
            {
                var user = userSet.First();
                if (user.Role != null && (user.Role.Id == 3 || user.Role.Id == 2))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsBuh(AppDbContext context)
        {
            var userSet = context.Users.Include(x => x.Role).Where(x => x.Id == Id);
            if (userSet.Count() > 0)
            {
                var user = userSet.First();
                if (user.Role != null && (user.Role.Id == 4 || user.Role.Id == 2))
                {
                    return true;
                }
            }
            return false;
        }

        static List<User> GetListOfHOD(AppDbContext context)
        {
            
            List<User> userList = new List<User>();
            var departSetIncluded = context.Departments.Include(x=>x.HeadOfDepartment);
            var departSet= departSetIncluded.Where(x => x.HeadOfDepartment != null);
            foreach (var depart in departSet)
            {
                userList.Add(depart.HeadOfDepartment);
            }
            userList.AddRange(context.Users.Where(x => x.IsHeadOfDepartment != null && x.IsHeadOfDepartment == true));
            return userList;
            //var query = @"Select rs.rsrc_name 
            //              From ROLES r
            //              JOIN RSRC rs ON rs.role_id=r.role_id  
            //              Where r.parent_role_id is NULL";
            //List<string> hodList = new List<string>();
            //string connectionString = @"Data Source=primadb;Initial Catalog=primavera;User ID=privuser;Password=P@ssw0rd";
            //string sqlExpression = query;
            //using (SqlConnection connection = new SqlConnection(connectionString))
            //{
            //    connection.Open();
            //    SqlCommand command = new SqlCommand(sqlExpression, connection);
            //    SqlDataReader reader = command.ExecuteReader();

            //    if (reader.HasRows) // если есть данные
            //    {
            //        while (reader.Read()) // построчно считываем данные
            //        {
            //            hodList.Add(reader.GetValue(0).ToString());                      

            //        }
            //    }

            //    reader.Close();
            //}
            //return hodList;
        }

        static List<User> GetListOfHODFromDepart(IQueryable<Department> departSet)
        {
            List<User> userList = new List<User>();            
            foreach (var depart in departSet)
            {
                userList.Add(depart.HeadOfDepartment);
            }           
            return userList;
        }

            public static User GetUserByFullNameFromDb(AppDbContext context, string fullName)
        {
            var userSetFound = context.Users.Where(x => x.FullName == fullName);
            if (userSetFound.Count() > 0)
            {
                return userSetFound.First();
            }
            return null;
        }

        public void AddUserToTask(TaskComp taskComp)
        {
            var executerStr = taskComp.Executers;

            if (string.IsNullOrEmpty(executerStr))
            {
                taskComp.Executers = this.FullName;
            }
            else
            {
                if (!executerStr.Contains(FullName))
                {
                    taskComp.Executers = $"{executerStr};{this.FullName}";
                }
            }

        }


        public void DeleteUserFromTask(TaskComp taskComp)
        {
            var executerStr = taskComp.Executers;
            if (executerStr==FullName)
            {
                taskComp.Executers = string.Empty;
            }
            else
            {
                string strForReplace = $"{this.FullName}";
                taskComp.Executers = executerStr.Replace(strForReplace, "").Trim(';');
            }
        }

        public Department GetDepartment(AppDbContext context)
        {
            var userSet = context.Users.Where(x => x.Id == Id).Include(x => x.Department);
            return userSet.First().Department;
        }

        public string GetDepartmentAcronym(AppDbContext context)
        {
            if (Id == 324)
            {
                int a = 1;
            }
            var userSet = context.Users.Where(x => x.Id == Id).Include(x => x.Department);
            if (userSet.Count() > 0)
            {
                if (!string.IsNullOrEmpty(City) && City == "Москва")
                {
                    return DepartAcronym.MSK;
                }
                else if(!string.IsNullOrEmpty(City) && City == "Нижнекамск")
                {
                    return DepartAcronym.NK;
                }
                else if (!string.IsNullOrEmpty(City) && City == "Альметьевск")
                {
                    return DepartAcronym.ALM;
                }
                else if (!string.IsNullOrEmpty(City) && City == "Нижний Новгород")
                {
                    return DepartAcronym.NNIZ;
                }
                else if (!string.IsNullOrEmpty(City) && City == "Уфа")
                {
                    return DepartAcronym.UFA;
                }
                else if (!string.IsNullOrEmpty(City) && City == "Салават")
                {
                    return DepartAcronym.SLV;
                }
                else
                {
                    var depart = userSet.First().Department;
                    if (depart != null)
                    {
                        if (depart.Name != null)
                        {
                            if (depart.Name.Contains("Электротехнический"))
                            {
                                return DepartAcronym.ETO;
                            }

                            else
                            {
                                return depart.Acronym;
                            }
                        }
                    }
                }
            }
            return string.Empty;
            
        }



        public double SubstractAbsencesAndCountRate(AppDbContext context, double whShouldBeFull, int month, int year)
        {
           var absSet = Absence.GetTotalHoursOfAbsenceExactMonth(context, this, month, year);
            if (Rate == null)
            {
                var res = whShouldBeFull - absSet;
                if (res > 0)
                {
                    return res;
                }
            }
            else
            {
                //Учитывает кто работает на полставки.
                var res = whShouldBeFull*Rate.Value - absSet;
                if (res > 0)
                {
                    return res;
                }
            }
            
            return 0;
        }

        public static void SubstractIfNonProdReport(IQueryable<User> prodList, List<User> nonProdList)
        {            
            foreach (var prodUser in prodList)
            {
                nonProdList.Remove(prodUser);
            }
            //Ссылочный тип поэтому возвращается изменённый nonProdList
        }

        public static User[] GetAllActiveUsers(AppDbContext context)
        {
            var userAr = context.Users.Where(x => x.IsActive.HasValue && x.IsActive.Value).Include(x => x.Department).ToArray();
            return userAr;
        }

        public static User[] GetAllActiveUsersForReport(AppDbContext context)
        {
            var userAr = context.Users.Where(x => x.IsActive.HasValue && x.IsActive.Value).Include(x => x.Department).Where(x=>x.Department.IgnoreInReport!=true).ToArray();
            return userAr;
        }

        public static User[] GetAllActiveUsersForReportExcludeByUser(AppDbContext context, int month, int year)
        {
            var userAr = context.Users.Where(x => x.IsActive.HasValue && x.IsActive.Value).Where(x=>x.IgnoreUserInReport!=true)
                .Where(x => x.ImportDate == null || (x.ImportDate.HasValue && x.ImportDate.Value.Month <= month && x.ImportDate.Value.Year <= year)).ToArray();
            return userAr;
        }

        public static List<User> GetAllActiveUsersListForReportExcludeByUser(AppDbContext context, int month, int year)
        {
            var userList = context.Users.Where(x => x.IsActive.HasValue && x.IsActive.Value).Where(x => x.IgnoreUserInReport != true)
                .Where(x => x.ImportDate == null || (x.ImportDate.HasValue && x.ImportDate.Value.Month <= month && x.ImportDate.Value.Year <= year)).Include(x=>x.Department).ToList();
            
            return userList;
        }

        public static IQueryable<User> GetNonProductionUsers(AppDbContext context)
        {
            var userSet = context.Users.Where(x => x.IsActive.HasValue && x.IsActive.Value).Include(x => x.Department).Where(x=>x.Department!=null).Where(x=>x.Department.IgnoreInReport.HasValue==false || x.Department.IgnoreInReport.Value==false).Where(x => x.Department.Production == null || x.Department.Production == false);
            return userSet;
        }

        public static IQueryable<User> GetNonProductionUsersExcludeByUser(AppDbContext context)
        {
            var userSet = context.Users.Where(x => x.IgnoreUserInReport != true).Where(x => x.IsActive.HasValue && x.IsActive.Value).Include(x=>x.Department).Where(x => x.Department.Production == null || x.Department.Production == false);            
            return userSet;
        }

        public static IQueryable<User> GetProductionUsers(AppDbContext context)
        {

            var userSet = context.Users.Where(x => x.IsActive.HasValue && x.IsActive.Value).Include(x => x.Department).Where(x => x.Department.Production != null || x.Department.Production == true).Where(x => x.Rate == null || x.Rate != 0);
                        //var userSetWOHOD = userSet.Where(x => x.isHeadOfDepartment(context) == false);
            return userSet;
        }

        public static IQueryable<User> GetProductionUsersWOHOD(AppDbContext context)
        {
            var listOfHOD = GetListOfHOD(context);
            var userSet = context.Users.Where(x => x.IsActive.HasValue && x.IsActive.Value).Include(x => x.Department).Where(x => x.Department.Production != null || x.Department.Production == true).Where(x => x.Rate == null || x.Rate != 0).Where(x => !listOfHOD.Contains(x));           
            
            //var userSetWOHOD = userSet.Where(x => x.isHeadOfDepartment(context) == false);
            return userSet;
        }

        public static IQueryable<User> GetProductionUsersExcludeByUser(AppDbContext context, int month, int year)
        {
            var userSet = context.Users.Where(x=>x.IgnoreUserInReport!=true).Where(x => x.IsActive.HasValue && x.IsActive.Value)
                .Where(x => x.ImportDate == null || (x.ImportDate.HasValue && x.ImportDate.Value.Month <= month && x.ImportDate.Value.Year <= year))
                .Include(x => x.Department).Where(x => x.Department.Production != null || x.Department.Production == true).Where(x => x.Rate == null || x.Rate != 0);
            return userSet;
        }

        public static void RefreshPublicDepart(AppDbContext context)
        {
            foreach (var user in context.Users)
            {
                user.PublicDepart= user.GetDepartmentAcronym(context);
            }
            context.SaveChanges();
        }

        public static void SubstructUsersFromCol(List<User> userList, IEnumerable<User> userSubstrCol)
        {
            foreach (var userForSub in userSubstrCol)
            {
                userList.Remove(userForSub);
            }
        }

        public static void SubstructUsersFromCol(List<User> userList, List<User> userSubstrCol)
        {
            foreach (var userForSub in userSubstrCol)
            {
                userList.Remove(userForSub);
            }
        }
    }
}
            
    

