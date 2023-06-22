using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.Models
{
	public class ApplicationUser : IdentityUser
	{
		[Required]
		public String? Name { get; set; }

		public String? StreetAddress { get; set; }
		public String? City { get; set; }
		public String? State { get; set; }
		public String? PostalCode { get; set; }

		public int? CompanyId { get; set; }

		[ForeignKey("CompanyId")]
		[ValidateNever]
		public Company? Company { get; set; }

		[NotMapped]
		public String Role { get; set; }
	}
}
