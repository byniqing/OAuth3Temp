using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationClient.Models
{
    public class GrantClientViewModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Resources { get; set; }
    }

    public class GrantClientInputModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        [Required]
        public string CallType { get; set; }
    }

}
