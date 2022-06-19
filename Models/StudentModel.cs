using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace REST_wyklad.Models
{
    public class StudentModel
    {
        [Key]
        public int StudentID { get; set; }

        [Column(TypeName="nvarchar(50)")]
        public string StudentImie { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string StudentKierunek { get; set; }

        public int StudentNrIndeksu { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string ImageName { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }

        [NotMapped]
        public string ImageSrc { get; set; }
    }
}
