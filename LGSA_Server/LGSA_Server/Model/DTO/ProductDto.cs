using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LGSA_Server.Model.DTO
{
    public class ProductDto
    {
        public int Id { get; set; }
        public int ProductOwner { get; set; }
        public string Name { get; set; }
        public double? Rating { get; set; }
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }
        [Range(0, int.MaxValue)]
        public int SoldCopies { get; set; }
        public int? GenreId { get; set; }
        public int? ProductTypeId { get; set; }
        public int? ConditionId { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdateWho { get; set; }

    }
}