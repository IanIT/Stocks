using System;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR.Hubs;
using TableDependency.EventArgs;
using TableDependency.Enums;
using System.Configuration;
using TableDependency.SqlClient;
using Microsoft.AspNet.SignalR;
using System.Linq;
using Stocks.Entities;

namespace Stocks {
  public class StockTicker : IDisposable {
    private readonly DBContext db = new DBContext();
    // Singleton instance
    private readonly static Lazy<StockTicker> _instance = new Lazy<StockTicker>(
            () => new StockTicker(GlobalHost.ConnectionManager.GetHubContext<StockTickerHub>().Clients));


    private static SqlTableDependency<Stock> tableDependency;
    private static SqlTableDependency<Share> tableDependencyShare;

    private StockTicker(IHubConnectionContext<dynamic> clients) {
      try {

        Clients = clients;

        var conString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

        tableDependency = new SqlTableDependency<Stock>(conString, "Stocks");
        tableDependency.OnChanged += SqlTableDependency_Changed;
        tableDependency.OnError += SqlTableDependency_OnError;
        tableDependency.Start();

        tableDependencyShare = new SqlTableDependency<Share>(conString, "Shares");
        tableDependencyShare.OnChanged += SqlTableDependency_SharesChanged;
        tableDependencyShare.OnError += SqlTableDependency_OnError;
        tableDependencyShare.Start();

      }
      catch(Exception ex) {
        tableDependency.Stop();
        tableDependency.Dispose();

        tableDependencyShare.Stop();
        tableDependencyShare.Dispose();
        var mes = ex.Message;
        throw;
      }
    }

    public static StockTicker Instance {
      get {
        return _instance.Value;
      }
    }

    private IHubConnectionContext<dynamic> Clients {
      get;
      set;
    }

    public IEnumerable<Stock> GetAllStocks() {
      var res = db.Stocks.ToList();
      return res;
    }

    public IEnumerable<Share> GetAllShares() {
      var res = db.Shares.ToList();
      return res;
    }

    static void SqlTableDependency_OnError(object sender, ErrorEventArgs e) {
      throw e.Error;
    }

    void SqlTableDependency_Changed(object sender, RecordChangedEventArgs<Stock> e) {
      if(e.ChangeType != ChangeType.None) {
        BroadcastStockPrice(e.Entity, e.ChangeType);
      }
    }

    private void BroadcastStockPrice(Stock stock, ChangeType changeType) {

      switch(changeType) {
        case ChangeType.Insert:
          Clients.All.insertStockPrice(stock);
          break;
        case ChangeType.Update:
          Clients.All.updateStockPrice(stock);
          break;
        case ChangeType.Delete:
          Clients.All.deleteStockPrice(stock);
          break;
      }
    }


    void SqlTableDependency_SharesChanged(object sender, RecordChangedEventArgs<Share> e) {
      if(e.ChangeType != ChangeType.None) {
        BroadcastShare(e.Entity, e.ChangeType);
      }
    }

    private void BroadcastShare(Share share, ChangeType changeType) {

      switch(changeType) {
        case ChangeType.Insert:
          Clients.All.insertShare(share);
          break;
        case ChangeType.Update:
          Clients.All.updateShare(share);
          break;
        case ChangeType.Delete:
          Clients.All.deleteShare(share);
          break;
      }
    }

    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing) {
      if(!disposedValue) {
        if(disposing) {
          tableDependency.Stop();
        }

        disposedValue = true;
      }
      db.Dispose();
    }

    ~StockTicker() {
      Dispose(false);
    }

    // This code added to correctly implement the disposable pattern.
    public void Dispose() {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    #endregion
  }
}