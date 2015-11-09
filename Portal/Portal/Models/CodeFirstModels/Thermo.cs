using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal.Models.CodeFirstModels
{
    public class Device
    {
        public string Id { get; set; }

        [Required]
        [StringLength(256)]
        [MinLength(2)]
        public string Title { get; set; }

        public string UserDataId { get; set; }
        public virtual UserData UserData { get; set; }

        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }

        public int CurrentTemparature { get; set; }
    }

    public class Profile
    {
        public Profile()
        {
            Intervals=new HashSet<Interval>();
        }
        public int Id { get; set; }

        [Required]
        [StringLength(256)]
        [MinLength(2)]
        public string Title { get; set; }

        [Required]
        [StringLength(256)]
        [MinLength(2)]
        public string Description { get; set; }

        public string UserDataId { get; set; }
        public virtual UserData UserData { get; set; }

        public virtual ICollection<Interval> Intervals { get; set; }
    }

    public class Interval
    {
        public int Id { get; set; }

        public int Start { get; set; }

        public int End { get; set; }

        [Required]
        [StringLength(256)]
        [MinLength(2)]
        public string Title { get; set; }

        [Required]
        [StringLength(256)]
        [MinLength(2)]
        public string Description { get; set; }

        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
    }
}