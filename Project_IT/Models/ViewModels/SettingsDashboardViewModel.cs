using System.Collections.Generic;

namespace Project_IT.Models.ViewModels
{
    public class SettingsModuleViewModel
    {
        public string Key { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string IconSvg { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public bool IsEnabled { get; set; }
        public string BadgeText { get; set; }
        public string BadgeClass { get; set; }
    }

    public class SettingsDashboardViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<SettingsModuleViewModel> Modules { get; set; }

        public SettingsDashboardViewModel()
        {
            Modules = new List<SettingsModuleViewModel>();
        }
    }
}
