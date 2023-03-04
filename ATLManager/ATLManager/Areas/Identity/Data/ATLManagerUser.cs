using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ATLManager.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ATLManagerUser class
public class ATLManagerUser : IdentityUser
{
    [ProtectedPersonalData]
    [Column(TypeName = "nvarchar(100)")]
    [Required]
    public string FirstName { get; set; }
    
    [ProtectedPersonalData]
    [Column(TypeName = "nvarchar(100)")]
    [Required]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    [ProtectedPersonalData]
    override public string Email { get; set; }

    public ATLManagerUserRole Role { get; set; }

    public ATLManagerUserStatus Status { get; set; }
}

