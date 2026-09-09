using System;
using System.ComponentModel.DataAnnotations;

namespace Project_IT.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "E-mail адреса")]
        [Required(ErrorMessage = "Полето е задолжително")]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$",
                            ErrorMessage = "Внесете валидна e-mail адреса")]
        public string Email { get; set; }

        [Display(Name = "Име")]
        public string Ime { get; set; }

        [Display(Name = "Презиме")]
        public string Prezime { get; set; }

        [Display(Name = "Име и презиме")]
        public string FullName { get; set; }

        [RegularExpression(@"^0(2|7)(\d{4,4}|[012345678]\d{3,3})\d{3,3}$",
                           ErrorMessage = "Внесете валиден телефонски број<br/> 07XYYYYYY")]
        [Display(Name = "Телефон")]
        [Required(ErrorMessage = "Полето е задолжително")]
        public string Telefon { get; set; }

        [Required(ErrorMessage = "Полето е задолжително")]
        [Display(Name = "Ноќевања")]
        public int Denovi { get; set; }

        [Range(1, 10, ErrorMessage = "Во еден бунгалов може да <br>престојуваат макс. 10 лица")]
        [Display(Name = "Лица")]
        [Required(ErrorMessage = "Полето е задолжително")]
        public int Lica { get; set; }

        [Display(Name = "Дата на пристигнување")]
        public DateTime DataNaPristignuvanje { get; set; }

        [Display(Name = "Дата на заминување")]
        public DateTime DataNaZaminuvanje { get; set; }

        [Display(Name = "Резервирано на")]
        public DateTime VremeRezervacija { get; set; }

        [Display(Name = "Цена")]
        public int Cena { get; set; }

        [Display(Name = "Тип на сместување")]
        public string Smestuvanje { get; set; }

        [Display(Name = "Статус")]
        [Required]
        public string Status { get; set; } = "New";

        [Display(Name = "Забелешка")]
        [DataType(DataType.MultilineText)]
        public string Info { get; set; }

        [Display(Name = "Платформа")]
        public string UserPlatform { get; set; }

        [Display(Name = "Оперативен систем")]
        public string UserOs { get; set; }

        [Display(Name = "User Agent")]
        public string UserAgent { get; set; }

        [Display(Name = "IP адреса")]
        public string IPAddress { get; set; }

        public string UserIp { get; set; }

        public string UserBrowser { get; set; }

        public string UserVersion { get; set; }

        public string UserCountry { get; set; }

        public string UserReferrer { get; set; }

        [Display(Name = "Креирано")]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [Display(Name = "Променето")]
        public DateTime? ModifiedOn { get; set; }
    }
}
