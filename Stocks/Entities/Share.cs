namespace Stocks.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Share
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        //[Required]
        //[StringLength(50)]
        public string Code { get; set; }

        //[Required]
        //[StringLength(50)]
        public string Name { get; set; }

        //[Column(TypeName = "numeric")]
        public decimal Price { get; set; }
    }
}
