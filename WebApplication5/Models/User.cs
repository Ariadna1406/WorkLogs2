using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
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
                
        public Role Role { get; set; }
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
        public static User GetUser(AppDbContext context, HttpContext httpContext)
        {
            var winUs = GetLogin(httpContext.User.Identity.Name);
            var userSet = context.Users.Where(x => x.Login == winUs);
            return userSet.Count() > 0 ? userSet.First() : new User() { FullName = winUs };
        }

        static string GetLogin(string login)
        {
            if (login.Contains("\\"))
            {
                return login.Split("\\").Last();
            }
            return login;
        }

        public bool isHeadOfDepartment()
        {            
           var listOfHOD = GetListOfHOD();
           var listOfHODeq= listOfHOD.Where(x => x.Equals(this.FullName));
           if (listOfHODeq.Count() > 0) return true; else return false;
        }
        static List<string> GetListOfHOD()
        {
            var query = @"Select rs.rsrc_name 
                          From ROLES r
                          JOIN RSRC rs ON rs.role_id=r.role_id  
                          Where r.parent_role_id is NULL";
            List<string> hodList = new List<string>();
            string connectionString = @"Data Source=primadb;Initial Catalog=primavera;User ID=privuser;Password=P@ssw0rd";
            string sqlExpression = query;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        hodList.Add(reader.GetValue(0).ToString());                      

                    }
                }

                reader.Close();
            }
            return hodList;
        }
     
    }
}
            
    

