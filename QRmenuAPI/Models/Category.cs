using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;

namespace QRmenuAPI.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = "";


        public byte StateId { get; set; }

        public int RestaurantId { get; set; }

        [ForeignKey("RestaurantId")]
        public Restaurant? Restaurant { get; set; }

        [ForeignKey("StateId")]
        public State? State { get; set; }

        public virtual List<Food>? Foods { get; set; }
    }
}
