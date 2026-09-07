using System;
using System.ComponentModel.DataAnnotations;

namespace Project_IT.Models
{
    public enum FeedCategory
    {
        News = 0,
        Event = 1,
        Offer = 2
    }

    public class FeedItem
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Наслов")]
        public string Title { get; set; }

        [StringLength(500)]
        [Display(Name = "Поднаслов")]
        public string Subtitle { get; set; }

        [DataType(DataType.Html)]
        [Display(Name = "Опис")]
        public string Description { get; set; }

        [Display(Name = "Слика URL")]
        public string ImageUrl { get; set; }

        [Required]
        [Display(Name = "Категорија")]
        public FeedCategory Category { get; set; }

        [Display(Name = "Зчка / Баџ")]
        public string LabelBadge { get; set; }

        [Display(Name = "Текст за понуда")]
        public string OfferText { get; set; }

        [Display(Name = "Текст на копче")]
        public string ButtonText { get; set; }

        [Display(Name = "Линк / URL")]
        public string ActionUrl { get; set; }

        [Display(Name = "Важи до")]
        [DataType(DataType.Date)]
        public DateTime? ValidTo { get; set; }

        [Display(Name = "Истакнато")]
        public bool IsFeatured { get; set; }

        [Display(Name = "Објавено")]
        public bool IsPublished { get; set; } = true;

        [Display(Name = "Датум на креирање")]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
