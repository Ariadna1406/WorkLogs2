using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class MailService
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<User> Users { get; set; }

        public Status MailStatus { get; set; }
        public enum Status { Confirmed, Declined }

        static SmtpClient smtp = new SmtpClient("mail.oilpro.ru", 25);
        static MailAddress from = new MailAddress("web-service@oilpro.ru", "ПОРТАЛ ТРУДОЗАТРАТ");
        static string signature = Environment.NewLine + "<p>__________________________ </p><p>С уважением, </p> <p>Портал трудозатрат <b>report.oilpro.ru</b>.</p>";
        public MailService() { }

        public static bool SendMessage(string toEmail, string subject, string messageText, List<string> errorMes)
        {
            MailAddress to = new MailAddress(toEmail);
            // создаем объект сообщения
            MailMessage m = new MailMessage(from, to);
            // тема письма
            m.Subject = subject;
            // текст письма
            m.Body = messageText + Environment.NewLine;
            m.Body += signature;
            // письмо представляет код html
            m.IsBodyHtml = true;
            // адрес smtp-сервера и порт, с которого будем отправлять письмо

            // логин и пароль            
            smtp.EnableSsl = true;
            try
            {
                smtp.Send(m);
                return true;
            }
            catch (Exception ex)
            {
                errorMes.Add(ex.ToString());
                return false;
            }

        }

        
        
        public static void Notification(List<TaskCompRequest> tcrAr, User userForAction, Status status, List<string> errorMes, TaskCompRequest[] commentList)
        {
            var tcrGroupedByUser = tcrAr.GroupBy(x => x.User);
            foreach (var tcrGr in tcrGroupedByUser)
            {
                NotificationToUser(tcrGr, userForAction, status, errorMes, commentList);
            }
        }

        public static void NotificationToUser(IGrouping<User, TaskCompRequest> tcrGr, User userForAction, Status status, List<string> errorMes, TaskCompRequest[] commentList)
        {
            string message = string.Empty;
            string subject = string.Empty;
            if (status == Status.Declined)
            {
                subject = "Заявка на работы отклонена";
                message = $"<p>Добрый день!</p> <p>Пользователем {userForAction.FullName} были отклонены следующие работы/комплекты: </p>";
            }
            else if(status == Status.Confirmed)
            {
                subject = "Заявка на работы принята";
                message = $"<p>Добрый день!</p> <p>Пользователем {userForAction.FullName} по Вашей заявке были созданы следующие работы/комплекты: </p>";
            }
         
            foreach (var tcrKV in tcrGr)
            {                                      
                message += $"<p>Работа - {tcrKV.TaskCompName} ; Проект - {tcrKV.ProjectNumber}; ";
                var dc = tcrKV.DenyComment;
                    if (!string.IsNullOrEmpty(dc)) {
                        message += "Комментарий - " + dc;
                    }
                
                message += "</p>";


            }
            if (tcrGr.Key != null) {
                var sendingRes = SendMessage(tcrGr.Key.Email, subject, message, errorMes);
            }

        }

        public static void NotifyKspUsers(TaskCompRequest tcr, User userCreatedRequest, User[] userKspAr, List<string> errorMes)
        {
            string subject = "Заявка на работы создана";
            string message = $"<p>Добрый день!</p> <p>Пользователем {userCreatedRequest.FullName} по была создана след. заявка на работы/комплекты: </p>";
            message += $"<p>Работа - {tcr.TaskCompName} ; Проект - {tcr.ProjectNumber}</p>";
            foreach (var userKsp in userKspAr)
            {
                SendMessage(userKsp.Email, subject, message, errorMes);
            }
        }

        public static void SendMessagesToAbusers(IOrderedEnumerable<TotalWorkLog> totalWlOrdered, User curUser, string monthStr)
        {
            List<string> errorMes = new List<string>();
            var totalWlAbusers = totalWlOrdered.Where(x => x.TotalSendedWorkLogs + 16 < x.TotalWorkLogsShouldBe);
            foreach (var totalWlAbuser in totalWlAbusers)
            {
                var abuser = totalWlAbuser.User;
                var subject = $"Отчёт по трудозатратам за {monthStr}";
                var message = $"<p>Добрый день, {abuser.FullName}! Требуется отчитаться по трудозатратам за {monthStr}. </p> " +
                    $"<p>Кол-во поданных трудозатрат: <b style=\"color: red;\">{totalWlAbuser.TotalSendedWorkLogs}</b></p> " +
                    $"<p>Нормативное кол-во трудозатрат: <b> {totalWlAbuser.TotalWorkLogsShouldBe} </b></p>" +
                    $"<p><br> Данное письмо отправлено по просьбе <b>{curUser.FullName}</b> </p>";
                //SendMessage(totalWlAbuser.User.Email, subject, message, errorMes); 
                SendMessage("nesterovig@oilpro.ru", subject, message, errorMes);
            }
        }

    }
}
            
    

