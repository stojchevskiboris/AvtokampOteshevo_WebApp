using System.ComponentModel.DataAnnotations;

namespace Project_IT.Models.ViewModels
{
    public class GeneralAppPreferencesViewModel
    {
        [Display(Name = "Патека до галерија за 'За нас'")]
        public string AboutUsGalleryPath { get; set; }
    }
}
