using System.Collections.Generic;
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

    public class DeviceCreate
    {
        [Required]
        [StringLength(256)]
        [MinLength(2)]
        public string Title { get; set; }

        public int ProfileId { get; set; }

        [Required]
        [StringLength(4)]
        [MinLength(4)]
        public string Code { get; set; }
    }

        public class ProfileView
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsDefault { get; set; }
    }

    public class ProfileCreate
    {
        [Required]
        [StringLength(256)]
        [MinLength(2)]
        public string Title { get; set; }

        [Required]
        [StringLength(256)]
        [MinLength(2)]
        public string Description { get; set; }
    }

    public class IntervalView
    {
        public int Id { get; set; }

        public int Start { get; set; }

        public int End { get; set; }

        [Required]
        [StringLength(256)]
        [MinLength(2)]
        public string Description { get; set; }
    }

    public class IntervalCreate
    {
        [Required]
        [StringLength(256)]
        [MinLength(2)]
        public string Title { get; set; }

        public int ProfileId { get; set; }

        [Required]
        [StringLength(4)]
        [MinLength(4)]
        public string Code { get; set; }
    }
}