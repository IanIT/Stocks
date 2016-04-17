namespace Stocks.Entities {
  using System;
  using System.Data.Entity;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Linq;

  public partial class DBContext : DbContext {
    public DBContext()        : base("name=DBContext") {
    }

    public virtual DbSet<Share> Shares { get; set; }
    public virtual DbSet<Stock> Stocks { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder) {
    }
  }
}
