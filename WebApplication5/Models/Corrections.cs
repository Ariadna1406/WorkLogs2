using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Models
{
    public class Corrections
    {
        public Corrections(int corNumber, DateTime corTerm, string corBodyText)
        {
            CorNumber = corNumber;
            CorTerm = corTerm;
            CorBodyText = corBodyText;
        }

        public Corrections() { }




        public int Id { get; set; }
        public int CorNumber { get; set; }
        public DateTime CorTerm { get; set; }
        [Required(ErrorMessage = "Требуется заполнить замечание")]
        public string CorBodyText { get; set; }
        public CorStatus Status { get; set; }
        
        public string ImageLink { get; set; }
        [Required(ErrorMessage = "Выберите исполнителя")]
        public User Executor { get; set; }
        public Project Project { get; set; }
        public Response Response { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Укажите, пож-та, скриншот")]
        public IFormFile ImageFile { get; set; }

        public DateTime RecieveDate { get; set; }

        public int ReopenTimes { get; set; }

        public Corrections OriginalCor { get; set; }

        public List<string> Statuses = new List<string> { "Новое", "Исправлено исполнителем", "Проверено BIM-координатором", "Снято", "Повторное" };
        public enum CorStatus { New, CorrectedByExecutor, CheckedByBim, Done, NewAgain }
    }

    
}
