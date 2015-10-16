using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TestAPI.Models
{
    public class Journey
    {
        [Key]
        public int JourneyId { get; set; }

        public string Name { get; set; }

        public DateTime CreatedDate { get; } = DateTime.Now;

        public int UserId { get; set;  }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}