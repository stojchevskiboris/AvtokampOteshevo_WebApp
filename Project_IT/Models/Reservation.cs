using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project_IT.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "E-mail адреса")]
        [Required (ErrorMessage = "Полето е задолжително")]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$",
                            ErrorMessage = "Внесете валидна e-mail адреса")]
        public string Email { get; set; }

        [Display(Name = "Име")]
        [Required(ErrorMessage = "Полето е задолжително")]
        public string Ime { get; set; }

        [Display(Name = "Презиме")]
        [Required(ErrorMessage = "Полето е задолжително")]
        public string Prezime { get; set; }
        //0(2|7)(\d{4,4}|[0123456789]\d{3,3})\d{3,3}
        [RegularExpression(@"^0(2|7)(\d{4,4}|[012345678]\d{3,3})\d{3,3}$",
                           ErrorMessage = "Внесете валиден телефонски број<br/> 07XYYYYYY")]
        [Display(Name = "Телефон")]
        [Required(ErrorMessage = "Полето е задолжително")]
        public string Telefon { get; set; }

        [Required(ErrorMessage = "Полето е задолжително")]
        [Display(Name = "Ноќевања")]
        public int Denovi { get; set; }

        [Range(1,4, ErrorMessage = "Во еден бунгалов може да <br>престојуваат макс. 4 лица")]
        [Display(Name = "Лица")]
        [Required(ErrorMessage = "Полето е задолжително")]
        public int Lica { get; set; }

        [Display(Name = "Дата на пристигнување")]
        [Required(ErrorMessage = "Полето е задолжително")]
        public DateTime DataNaPristignuvanje { get; set; }

        [Display(Name = "Дата на заминување")]
        [Required(ErrorMessage = "Полето е задолжително")]
        public DateTime DataNaZaminuvanje { get; set; }

        [Display(Name = "Резервирано на")]
        public DateTime VremeRezervacija { get; set; }

        [Display(Name = "Цена")]
        public int Cena { get; set; }

        [Display (Name = "Забелешка")]
        [DataType(DataType.MultilineText)]
        public string Info { get; set; }

    }
}