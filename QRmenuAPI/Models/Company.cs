﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QRmenuAPI.Models
{
	public class Company
	{
        public int Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 3)]
        [Column(TypeName = "nvarchar(200)")]
        public string Name { get; set; } = "";

        [Required]
        [Phone]
        [Column("varchar(30)")]
        public string Phone { get; set; } = "";

        [Required]
        [StringLength(100, MinimumLength = 5)]
        [Column(TypeName = "varchar(100)")]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [StringLength(11, MinimumLength = 10)]
        [Column(TypeName = "varchar(11)")]
        public string TaxNumber { get; set; } = "";

        [Required]
        [StringLength(5, MinimumLength = 5)]
        [Column(TypeName = "char(5)")]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; } = "";

        [Required]
        [StringLength(200, MinimumLength = 5)]
        [Column(TypeName = "nvarchar(200)")]
        public string AddressDetail { get; set; } = "";

        [StringLength(200)]
        [Column(TypeName = "varchar(200)")]
        public string? Web { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime RegisterDate { get; set; }

        [Required]
        public byte StateId { get; set; }
        [ForeignKey("StateId")]
        public  State? State { get; set; }

    }
}

