using System;
using System.Collections.Generic;

namespace Project_IT.Models
{
    public class FileItemViewModel
    {
        public string Name { get; set; }
        public string VirtualPath { get; set; }
        public string RelativePath { get; set; }
        public bool IsDirectory { get; set; }
        public long SizeBytes { get; set; }
        public string FormattedSize { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string Extension { get; set; }
        public bool IsImage { get; set; }
        public int? ImageWidth { get; set; }
        public int? ImageHeight { get; set; }
        public string DimensionString => (ImageWidth.HasValue && ImageHeight.HasValue) ? $"{ImageWidth.Value} x {ImageHeight.Value} px" : null;
    }

    public class BreadcrumbItem
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }

    public class FileManagerViewModel
    {
        public string CurrentPath { get; set; }
        public string SearchQuery { get; set; }
        public string ViewType { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public List<FileItemViewModel> Items { get; set; } = new List<FileItemViewModel>();
        public List<BreadcrumbItem> Breadcrumbs { get; set; } = new List<BreadcrumbItem>();
    }
}
