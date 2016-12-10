using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LGSA_Server.Model.DTO
{
    public class ProductDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int ProductOwner { get; set; }
        [Required]
        public string Name { get; set; }
        public double? Rating { get; set; }
        [Required, Range(0, int.MaxValue)]
        public int Stock { get; set; }
        [Required, Range(0, int.MaxValue)]
        public int SoldCopies { get; set; }
        public int? GenreId { get; set; }
        public int? ProductTypeId { get; set; }
        public int? ConditionId { get; set; }
        public ConditionDto Condition { get; set; }
        public GenreDto Genre { get; set; }
        public ProductTypeDto ProductType { get; set; }

    }
}