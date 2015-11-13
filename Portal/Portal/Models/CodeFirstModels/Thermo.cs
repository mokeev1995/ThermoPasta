using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Portal.Models.CodeFirstModels
{
    public class Device
    {
        public Device()
        {
            Temperatures=new HashSet<Temperature>();
        }

        public string Id { get; set; }

        [Required]
        [StringLength(256)]
        [MinLength(2)]
        public string Title { get; set; }

        public string UserDataId { get; set; }
        public virtual UserData UserData { get; set; }

        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }

        public int Period { get; set; }

        public virtual ICollection<Temperature> Temperatures { get; set; }
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
        public string Description { get; set; }

        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
    }

    public class Temperature
    {
        public int Id { get; set; }

        [Required]
        public string DeviceId { get; set; }
        public virtual Device Device { get; set; }
               
        [Required]
        public int Value { get; set; }

        public DateTime Time { get; set; }
    }

    public class CheckCode
    {
        public string Id { get; set; }

        [Required]
        [StringLength(4)]
        [MinLength(4)]
        public string Code { get; set; }

        [Required]
        public DateTime Time { get; set; }
    }
}