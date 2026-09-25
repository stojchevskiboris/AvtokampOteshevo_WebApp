using System.ComponentModel.DataAnnotations;

namespace Project_IT.Models
{
    public class BookingPricesViewModel
    {
        [Display(Name = "Цена по ноќевање (Бунгалов 1)")]
        [Range(0, 100000)]
        public decimal PricePerNightBungalow1 { get; set; }

        [Display(Name = "Цена по ноќевање (Бунгалов 2)")]
        [Range(0, 100000)]
        public decimal PricePerNightBungalow2 { get; set; }

        [Display(Name = "Цена по ноќевање (Бунгалов 3)")]
        [Range(0, 100000)]
        public decimal PricePerNightBungalow3 { get; set; }

        [Display(Name = "Цена по ноќевање (Приколка)")]
        [Range(0, 100000)]
        public decimal PricePerNightTrailer { get; set; }

        [Display(Name = "Такса за плац")]
        [Range(0, 100000)]
        public decimal PitchFee { get; set; }
    }
}
