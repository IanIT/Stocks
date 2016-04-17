using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Collections.Generic;
using Stocks.Entities;

namespace Stocks {
  [HubName("stockTicker")]
  public class StockTickerHub : Hub {
    private readonly StockTicker _stockTicker;

    public StockTickerHub() :
        this(StockTicker.Instance) {

    }

    public StockTickerHub(StockTicker stockTicker) {
      _stockTicker = stockTicker;
    }

    public IEnumerable<Stock> GetAllStocks() => _stockTicker.GetAllStocks();

    public IEnumerable<Share> GetAllShares() => _stockTicker.GetAllShares();

  }
}