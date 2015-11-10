using System.ComponentModel.DataAnnotations;

namespace Portal.Models.ViewModels
{
    public class DeviceView
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Profile { get; set; }

        [Required]
        [Display(Name = "Current temparature")]
        public int CurrentTemparature { get; set; }
        public string Status { get; set; }
    }
}