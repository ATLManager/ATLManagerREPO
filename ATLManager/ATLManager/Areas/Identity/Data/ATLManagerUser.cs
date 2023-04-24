using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ATLManager.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ATLManagerUser class
public class ATLManagerUser : IdentityUser
{
    [Required]
    [ProtectedPersonalData]
    [Column(TypeName = "nvarchar(100)")]
    [DisplayName("Nome")]
    public string FirstName { get; set; }
    
    [Required]
    [ProtectedPersonalData]
    [Column(TypeName = "nvarchar(100)")]
    [DisplayName("Apelido")]
    public string LastName { get; set; }

	[Required]
	[EmailAddress]
	[ProtectedPersonalData]
	override public string Email { get; set; }

    [Required]
    [ProtectedPersonalData]
    [DisplayName("Foto de perfil")]
    public string Picture { get; set; } = "logo.png";
}

