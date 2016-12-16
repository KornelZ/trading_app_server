using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LGSA_Server.Model.DTO
{
    public class BuyOfferDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int BuyerId { get; set; }
        public decimal? Price { get; set; }
        [Required, Range(1, int.MaxValue)]
        public int Amount { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int ProductId { get; set; }
        public ProductDto Product { get; set; }
    }
}