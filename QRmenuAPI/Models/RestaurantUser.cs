using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace QRmenuAPI.Models
{
	public class RestaurantUser
	{
			
		public int RestaurantId { get; set; }

		[Column(TypeName = "nvarchar(450)")]
		public string UserApplicationUserId { get; set; } = "";

		[ForeignKey("RestaurantId")]
		public Restaurant? Restaurant { get; set; }

		[ForeignKey("UserApplicationUserId")]
		public ApplicationUser? ApplicationUser { get; set; }
	}
}

