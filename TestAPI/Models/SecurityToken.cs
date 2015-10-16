using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TestAPI.Models
{
    public class SecurityToken
    {
        [Key]
        public int SecurityTokenId { get; set; }

        [Required]
        public string Token { get; set; }
    }
}