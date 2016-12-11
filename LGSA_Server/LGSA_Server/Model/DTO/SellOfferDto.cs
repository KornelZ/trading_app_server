using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LGSA_Server.Model.DTO
{
    public class SellOfferDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int SellerId { get; set; }
        public decimal? Price { get; set; }
        [Required, Range(0, int.MaxValue)]
        public int Amount { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int ProductId { get; set; }
        public ProductDto Product { get; set; }
    }
}