using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ATLManager.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ATLManagerUser class
public class ATLManagerUser : IdentityUser
{
    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string FirstName { get; set; }
    
    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string LastName { get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(15)")]
    public string CC { get; set; } = string.Empty;

    public ATLManagerUserRole Role { get; set; }

    public ATLManagerUserStatus Status { get; set; }
}

