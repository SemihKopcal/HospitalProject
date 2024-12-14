using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HospitalApp.Models // Namespace added here
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string Department { get; set; }
       
    }
}
