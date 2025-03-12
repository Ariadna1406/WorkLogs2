using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class ProjectShares
    {
        public User User { get; set; }

        public string TaskCompName { get; set; }
        public string ProjectNumber { get; set; }
        public double ProjectShare { get; set; }
        public double TotalProjectWLVal { get; set; }
        public bool? IsProduction { get; set; }
        public int HighLight { get; set; } // 1 - подсвечиваем при выгрузке в Excel
        public List<WorkLogs> WLForProjList { get; set; }

        public static List<ProjectShares> Calculate(AppDbContext context, out List<ProjectSharesTot> projectSharesTotList, int monthOfReport, int year)
        {
            List<ProjectShares> prjShareGlobList = new List<ProjectShares>();
            var wlSet = context.WorkLogs.Include(x=>x.User).ThenInclude(x=>x.Department).Where(x=>x.User.Department.IgnoreInReport!=true).Where(x => x.Proj_id.Trim() != "0" && x.Proj_id.Trim() != "0000" && x.Proj_id.Trim() != "Обучение").Where(x=>x.TaskComp_id != "undefined").Where(x => x.DateOfReport.Month == monthOfReport && x.DateOfReport.Year==year);//.Include(x => x.WorkTime).Include(x => x.User);            
            User.RefreshPublicDepart(context);
            var psGrByDepart = WorkLogs.GetWlProjectShareGroupedByDepart(wlSet, context);
            var whShouldBe = WorkLogs.GetTotalWTSHouldBeExactMonth(context, monthOfReport);
            //Добавление произведственников, которые отчитались
            var wlSetGroupedByUser = wlSet.GroupBy(x => x.User);
            if (wlSetGroupedByUser!=null && wlSetGroupedByUser.Count()>0) {
                foreach (var wlSetGr in wlSetGroupedByUser)
                {
                    List<ProjectShares> prjShareLocList = new List<ProjectShares>();
                    var wlSetGroupedByProj = wlSetGr.GroupBy(x => x.Proj_id); //Доли по проектам отчитавшихся пользователей
                    foreach (var wlSetGrProj in wlSetGroupedByProj)
                    {
                        if (wlSetGrProj.Count() > 0) {
                            var defWl = wlSetGrProj.First();
                            var totalWT = WorkLogs.GetTotalHoursWorkLogSet(wlSetGrProj);
                            var newPrjShare = new ProjectShares()
                            {
                                ProjectNumber = defWl.Proj_id,
                                TotalProjectWLVal = Math.Round(totalWT.TotalHours, 4),
                                User = wlSetGr.Key,
                                IsProduction = true
                                
                            };
                            prjShareLocList.Add(newPrjShare);

                        }
                    }
                    var totalHoursForAllPrj = prjShareLocList.Sum(x => x.TotalProjectWLVal); //Суммирование всех добавленых трудозатрат
                    if (totalHoursForAllPrj != 0)
                    {
                        foreach (var prjShare in prjShareLocList)
                        {
                            prjShare.ProjectShare = prjShare.TotalProjectWLVal / totalHoursForAllPrj;  //Math.Round(prjShare.TotalProjectWLVal / totalHoursForAllPrj, 4);
                        }
                    }
                    prjShareGlobList.AddRange(prjShareLocList);
                }
            }

            //Добавление производственников которые ни разу не отчитывались по трудозатратам
            
            var userProdactionSet = User.GetProductionUsers(context).ToList();
            var upWithWlSet = prjShareGlobList.Select(x => x.User).Distinct();            
            foreach (var upWithWl in upWithWlSet) // Убираем из 
            {                
                userProdactionSet.Remove(upWithWl);
            }

            foreach (var upWithoutWl in userProdactionSet)
            {
                
                var whShouldBeForUser = upWithoutWl.SubstractAbsencesAndCountRate(context, whShouldBe, monthOfReport, year);
                var psDepartSet = psGrByDepart.Where(x => x.Key == upWithoutWl.PublicDepart);
                if (psDepartSet.Count() > 0)
                {
                    var psDepart = psDepartSet.First();
                    foreach (var ps in psDepart.Value) {

                        prjShareGlobList.Add(new ProjectShares()
                        {
                            User = upWithoutWl,
                            ProjectShare = ps.ProjectShare,
                            ProjectNumber = ps.ProjectNumber,
                            IsProduction = true,
                            HighLight= upWithoutWl.isHeadOfDepartment(context) ? 0 : 1,
                            TotalProjectWLVal =ps.ProjectShare * whShouldBeForUser

                        });
                }
                }
            }

            //Добавления непроизводственников
            var newWlSet = context.WorkLogs.Where(x => x.DateOfReport.Month == monthOfReport);
            Dictionary<string, double> projectSharesTot = new Dictionary<string, double>();
            var wlSetGrByProj = wlSet.GroupBy(x => x.Proj_id.Trim());
            foreach (var wlGr in wlSetGrByProj)
            {
               var totWT = wlGr.Sum(x => x.WorkTime.TotalHours);
                projectSharesTot.Add(wlGr.Key, totWT);
            }
            projectSharesTotList = new List<ProjectSharesTot>();
            var totSum = projectSharesTot.Sum(x => x.Value);
            foreach (var psTot in projectSharesTot)
            {
                projectSharesTotList.Add(new ProjectSharesTot(psTot.Key, psTot.Value / totSum, psTot.Value));
            }
           
            var usersNPSet = User.GetNonProductionUsers(context).ToList();
            var userProdList = wlSetGroupedByUser.Select(x => x.Key);
            User.SubstractIfNonProdReport(userProdList, usersNPSet);
            foreach (var usersNP in usersNPSet)
            {
                var whShouldBeForUser = usersNP.SubstractAbsencesAndCountRate(context, whShouldBe, monthOfReport, year);
                foreach (var pst in projectSharesTotList)
                {
                    if (usersNP.Rate == null)
                        prjShareGlobList.Add(new ProjectShares()
                        {
                            ProjectNumber = pst.ProjectNumber,
                            ProjectShare = pst.ProjectShare,
                            TotalProjectWLVal = pst.ProjectShare * whShouldBeForUser,
                            User = usersNP,
                            IsProduction = false

                        });

                    else
                    {
                        prjShareGlobList.Add(new ProjectShares()
                        {
                            ProjectNumber = pst.ProjectNumber,
                            ProjectShare = pst.ProjectShare,
                            TotalProjectWLVal = pst.ProjectShare * whShouldBe*usersNP.Rate.Value,
                            User = usersNP,
                            IsProduction = false

                        });
                    }
                }
            }
            return prjShareGlobList;

        }

        public static List<ProjectShares> CalculateExcludeByUser(AppDbContext context, out List<ProjectSharesTot> projectSharesTotList,out User[] userAr, int monthOfReport, int year)
        {
            List<ProjectShares> prjShareGlobList = new List<ProjectShares>();
            var users = User.GetAllActiveUsersListForReportExcludeByUser(context, monthOfReport, year);          
            var wlSet = context.WorkLogs.Include(x => x.User).ThenInclude(x => x.Department).Where(x => x.User.IgnoreUserInReport!= true).Where(x => x.Proj_id.Trim() != "0" && x.Proj_id.Trim() != "0000" && x.Proj_id.Trim() != "Обучение").Where(x => x.TaskComp_id != "undefined").Where(x => x.DateOfReport.Month == monthOfReport && x.DateOfReport.Year==year);//.Include(x => x.WorkTime).Include(x => x.User);            
            User.RefreshPublicDepart(context);
            var psGrByDepart = WorkLogs.GetWlProjectShareGroupedByDepart(wlSet, context);
            var whShouldBe = WorkLogs.GetTotalWTSHouldBe(context, monthOfReport, year);//GetTotalWTSHouldBeExactMonth(context, monthOfReport);
            //Добавление произведственников, которые отчитались
            var wlSetGroupedByUser = wlSet.GroupBy(x => x.User);
            if (wlSetGroupedByUser != null && wlSetGroupedByUser.Count() > 0)
            {
                foreach (var wlSetGr in wlSetGroupedByUser)
                {
                    List<ProjectShares> prjShareLocList = new List<ProjectShares>();
                    var wlSetGroupedByProj = wlSetGr.GroupBy(x => x.Proj_id); //Доли по проектам отчитавшихся пользователей
                    foreach (var wlSetGrProj in wlSetGroupedByProj)
                    {
                        if (wlSetGrProj.Count() > 0)
                        {
                            var defWl = wlSetGrProj.First();
                            var totalWT = WorkLogs.GetTotalHoursWorkLogSet(wlSetGrProj);
                            var newPrjShare = new ProjectShares()
                            {
                                ProjectNumber = defWl.Proj_id,
                                TotalProjectWLVal = Math.Round(totalWT.TotalHours, 4),
                                User = wlSetGr.Key,
                                IsProduction = true

                            };
                            prjShareLocList.Add(newPrjShare);

                        }
                    }
                    var totalHoursForAllPrj = prjShareLocList.Sum(x => x.TotalProjectWLVal); //Суммирование всех добавленых трудозатрат
                    if (totalHoursForAllPrj != 0)
                    {
                        foreach (var prjShare in prjShareLocList)
                        {
                            prjShare.ProjectShare = prjShare.TotalProjectWLVal / totalHoursForAllPrj;  //Math.Round(prjShare.TotalProjectWLVal / totalHoursForAllPrj, 4);
                        }
                    }
                    prjShareGlobList.AddRange(prjShareLocList);
                }
            }

            //Добавление производственников которые ни разу не отчитывались по трудозатратам

            // В условии отсеивание пользователей, которые не работали на месяц отчёта.
            var userProdactionSet = User.GetProductionUsersExcludeByUser(context, monthOfReport, year).ToList();
            var upWithWlSet = prjShareGlobList.Select(x => x.User).Distinct();
            foreach (var upWithWl in upWithWlSet) // Убираем из 
            {
                userProdactionSet.Remove(upWithWl);
            }           

            // userProdactionSet набор пользователей без трудозатрат
            foreach (var upWithoutWl in userProdactionSet)
            {
                var whShouldBeForUser = upWithoutWl.SubstractAbsencesAndCountRate(context, whShouldBe, monthOfReport, year);
                var psDepartSet = psGrByDepart.Where(x => x.Key == upWithoutWl.PublicDepart);
                if (psDepartSet.Count() > 0)
                {
                    var psDepart = psDepartSet.First();
                    foreach (var ps in psDepart.Value)
                    {

                        prjShareGlobList.Add(new ProjectShares()
                        {
                            User = upWithoutWl,
                            ProjectShare = ps.ProjectShare,
                            ProjectNumber = ps.ProjectNumber,
                            IsProduction = true,
                            HighLight = upWithoutWl.isHeadOfDepartment(context) ? 0 : 1,
                            TotalProjectWLVal = ps.ProjectShare * whShouldBeForUser

                        });
                    }
                }
            }

            //Вычисление долей по пулу проектов
            var newWlSet = context.WorkLogs.Where(x => x.DateOfReport.Month == monthOfReport && x.DateOfReport.Year == year);
            Dictionary<string, double> projectSharesTot = new Dictionary<string, double>();
            var wlSetGrByProj = wlSet.GroupBy(x => x.Proj_id.Trim());
            foreach (var wlGr in wlSetGrByProj)
            {
                var totWT = wlGr.Sum(x => x.WorkTime.TotalHours);
                projectSharesTot.Add(wlGr.Key, totWT);
            }
            projectSharesTotList = new List<ProjectSharesTot>();
            var totSum = projectSharesTot.Sum(x => x.Value);
            foreach (var psTot in projectSharesTot)
            {
                projectSharesTotList.Add(new ProjectSharesTot(psTot.Key, psTot.Value / totSum, psTot.Value));
            }

            //Добавления непроизводственников
            WebApplication5.Models.User.SubstructUsersFromCol(users, userProdactionSet); //Вычитаем пользователей-производственников без трудозатрат 
            WebApplication5.Models.User.SubstructUsersFromCol(users, upWithWlSet); //Вычитаем пользователей-производственников с трудозатрат
            var usersNPSet = users; //остаются непроизводственники

            var userProdList = wlSetGroupedByUser.Select(x => x.Key);
            User.SubstractIfNonProdReport(userProdList, usersNPSet);
            foreach (var usersNP in usersNPSet)
            {
                
                var whShouldBeForUser = usersNP.SubstractAbsencesAndCountRate(context, whShouldBe, monthOfReport, year);
                foreach (var pst in projectSharesTotList)
                {
                    if (usersNP.Rate == null)
                        prjShareGlobList.Add(new ProjectShares()
                        {
                            ProjectNumber = pst.ProjectNumber,
                            ProjectShare = pst.ProjectShare,
                            TotalProjectWLVal = pst.ProjectShare * whShouldBeForUser,
                            User = usersNP,
                            IsProduction = false

                        });

                    else
                    {
                        prjShareGlobList.Add(new ProjectShares()
                        {
                            ProjectNumber = pst.ProjectNumber,
                            ProjectShare = pst.ProjectShare,
                            TotalProjectWLVal = pst.ProjectShare * whShouldBe * usersNP.Rate.Value,
                            User = usersNP,
                            IsProduction = false

                        });
                    }
                }
            }                        

            //Получаем всех пользователей, попавщих в отчёт
            //userProdactionSet.AddRange(upWithWlSet);
            //userProdactionSet.AddRange(usersNPSet);
            //var users = userProdactionSet.Distinct();
            //userAr = userProdactionSet.ToArray();

            userAr = User.GetAllActiveUsersForReportExcludeByUser(context, monthOfReport, year);

            return prjShareGlobList;

        }

        public static List<ProjectShares> CalculateExcludeByUserTaskComp(AppDbContext context, out List<ProjectSharesTaskCompTot> projectSharesTaskCompTotList, out User[] userAr, int monthOfReport, int year)
        {
            List<ProjectShares> prjShareGlobList = new List<ProjectShares>();
            var users = User.GetAllActiveUsersListForReportExcludeByUser(context, monthOfReport, year);
            var wlSet = context.WorkLogs.Include(x => x.User).ThenInclude(x => x.Department).Where(x => x.User.IgnoreUserInReport != true).Where(x => x.Proj_id.Trim() != "0" && x.Proj_id.Trim() != "0000" && x.Proj_id.Trim() != "Обучение").Where(x => x.TaskComp_id != "undefined").Where(x => x.DateOfReport.Month == monthOfReport && x.DateOfReport.Year==year);//.Include(x => x.WorkTime).Include(x => x.User);            
            User.RefreshPublicDepart(context);
            var taskCompSet = context.TaskComps.ToList();
            var psGrByDepart = WorkLogs.GetWlProjectShareGroupedByDepartTaskComp(wlSet, context, taskCompSet);
            var whShouldBe = WorkLogs.GetTotalWTSHouldBe(context, monthOfReport, year);//GetTotalWTSHouldBeExactMonth(context, monthOfReport);
            var wlSetGroupedByUser = wlSet.GroupBy(x => x.User);
            if (wlSetGroupedByUser != null && wlSetGroupedByUser.Count() > 0)
            {
                foreach (var wlSetGr in wlSetGroupedByUser)
                {
                    List<ProjectShares> prjShareLocList = new List<ProjectShares>();
                    var wlSetGroupedByProj = wlSetGr.GroupBy(x => x.TaskComp_id); //Доли по проектам отчитавшихся пользователей
                    foreach (var wlSetGrProj in wlSetGroupedByProj)
                    {
                        if (wlSetGrProj.Count() > 0)
                        {
                            var defWl = wlSetGrProj.First();
                            var totalWT = WorkLogs.GetTotalHoursWorkLogSet(wlSetGrProj);
                            var newPrjShare = new ProjectShares()
                            {
                                TaskCompName = taskCompSet.Single(x => x.Id.ToString() == defWl.TaskComp_id).TaskCompName,
                                ProjectNumber = defWl.Proj_id,
                                TotalProjectWLVal = Math.Round(totalWT.TotalHours, 4),
                                User = wlSetGr.Key,
                                IsProduction = true

                            };
                            prjShareLocList.Add(newPrjShare);

                        }
                    }
                    var totalHoursForAllPrj = prjShareLocList.Sum(x => x.TotalProjectWLVal); //Суммирование всех добавленых трудозатрат
                    if (totalHoursForAllPrj != 0)
                    {
                        foreach (var prjShare in prjShareLocList)
                        {
                            prjShare.ProjectShare = prjShare.TotalProjectWLVal / totalHoursForAllPrj;  //Math.Round(prjShare.TotalProjectWLVal / totalHoursForAllPrj, 4);
                        }
                    }
                    prjShareGlobList.AddRange(prjShareLocList);
                }
            }

          

            //Добавление производственников которые ни разу не отчитывались по трудозатратам

            // В условии отсеивание пользователей, которые не работали на месяц отчёта.
            var userProdactionSet = User.GetProductionUsersExcludeByUser(context, monthOfReport, year).ToList();
            var upWithWlSet = prjShareGlobList.Select(x => x.User).Distinct();
            foreach (var upWithWl in upWithWlSet) // Убираем из 
            {
                userProdactionSet.Remove(upWithWl);
            }

            // userProdactionSet набор пользователей без трудозатрат
            foreach (var upWithoutWl in userProdactionSet)
            {
                var whShouldBeForUser = upWithoutWl.SubstractAbsencesAndCountRate(context, whShouldBe, monthOfReport, year);
                var psDepartSet = psGrByDepart.Where(x => x.Key == upWithoutWl.PublicDepart);
                if (psDepartSet.Count() > 0)
                {
                    var psDepart = psDepartSet.First();
                    foreach (var ps in psDepart.Value)
                    {

                        prjShareGlobList.Add(new ProjectShares()
                        {
                            User = upWithoutWl,
                            ProjectShare = ps.ProjectShare,
                            ProjectNumber = ps.ProjectNumber,
                            TaskCompName = ps.TaskCompName,
                            IsProduction = true,
                            HighLight = upWithoutWl.isHeadOfDepartment(context) ? 0 : 1,
                            TotalProjectWLVal = ps.ProjectShare * whShouldBeForUser

                        });
                    }
                }
            }

            foreach (var prSh in prjShareGlobList)
            {
                int y = 0;
                
                    int x = 1;
                    string ffff = prSh.User.LastName;
                
            }

            //Вычисление долей по пулу проектов
            var newWlSet = context.WorkLogs.Where(x => x.DateOfReport.Month == monthOfReport && x.DateOfReport.Year == year);
            Dictionary<string, double> projectSharesTot = new Dictionary<string, double>();
            var wlSetGrByTaskComp = wlSet.GroupBy(x => x.TaskComp_id.Trim());
            foreach (var wlGr in wlSetGrByTaskComp)
            {
                var totWT = wlGr.Sum(x => x.WorkTime.TotalHours);                
                projectSharesTot.Add(wlGr.Key, totWT);
            }
            projectSharesTaskCompTotList = new List<ProjectSharesTaskCompTot>();
            var totSum = projectSharesTot.Sum(x => x.Value);
            foreach (var psTot in projectSharesTot)
            {
                var taskComp = taskCompSet.Single(x => x.Id.ToString() == psTot.Key);//ProjectNumber здесь TaskComp_Id просто строка для передачи                
                projectSharesTaskCompTotList.Add(new ProjectSharesTaskCompTot(taskComp.ProjectNumber, psTot.Value / totSum, psTot.Value, taskComp.TaskCompName));
            }

            //Получение активных пользователей
            userAr = User.GetAllActiveUsersForReportExcludeByUser(context, monthOfReport, year);
            return prjShareGlobList;

        }
    }
}
