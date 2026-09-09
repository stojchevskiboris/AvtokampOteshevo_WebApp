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
        public string FirstName { get; set; }

        [Display(Name = "Презиме")]
        public string LastName { get; set; }

        [Display(Name = "Име и презиме")]
        public string FullName { get; set; }

        [Display(Name = "Телефон")]
        public string Phone { get; set; }

        [Display(Name = "Ноќевања")]
        public int Days { get; set; }

        [Display(Name = "Лица")]
        public int Guests { get; set; }

        [Display(Name = "Дата на пристигнување")]
        public DateTime? CheckInDate { get; set; }

        [Display(Name = "Дата на заминување")]
        public DateTime? CheckOutDate { get; set; }

        [Display(Name = "Цена")]
        public int Price { get; set; }

        [Display(Name = "Тип на сместување")]
        public string AccommodationType { get; set; }

        [Display(Name = "Статус")]
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
