using System.ComponentModel.DataAnnotations.Schema;

namespace Stocks {
  [Table("Stocks")]

  public class Stock {
    public string Code { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
  }
}
