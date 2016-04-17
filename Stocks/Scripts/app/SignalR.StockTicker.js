
// Crockford's supplant method (poor man's templating)
if (!String.prototype.supplant) {
  String.prototype.supplant = function (o) {
    return this.replace(/{([^{}]*)}/g,
        function (a, b) {
          var r = o[b];
          return typeof r === 'string' || typeof r === 'number' ? r : a;
        }
    );
  };
}

$(function () {
  var ticker = $.connection.stockTicker; // the generated client-side hub proxy
  var $stockTable = $('#stockTable');
  var $stockTableBody = $stockTable.find('tbody');
  var $shareTable = $('#shareTable');
  var $shareTableBody = $shareTable.find('tbody');
  var rowTemplate = '<tr data-id="{Id}"><td>{Id}</td><td>{Code}</td><td>{Name}</td><td>{Price}</td></tr>';

  function formatPrice(item) {
    return $.extend(item, {
      Price: item.Price.toFixed(2)
    });
  }

  function init() {
    return ticker.server.getAllStocks().done(function (stocks) {
      $stockTableBody.empty();

      $.each(stocks, function () {
        var stock = formatPrice(this);
        $stockTableBody.append(rowTemplate.supplant(stock));
      });
    });
  }

  function initShares() {
    return ticker.server.getAllShares().done(function (shares) {
      $shareTableBody.empty();

      $.each(shares, function () {
        var share = formatPrice(this);
        $shareTableBody.append(rowTemplate.supplant(share));
      });
    });
  }

  // Add client-side hub methods that the server will call
  $.extend(ticker.client, {
    updateStockPrice: function (stock) {
      var displayStock = formatPrice(stock);
      $row = $(rowTemplate.supplant(displayStock)),
      $stockTableBody.find('tr[data-id=' + stock.Id + ']').replaceWith($row);
    },

    insertStockPrice: function (stock) {
      var displayStock = formatPrice(stock);
      if ($stockTableBody.find('tr').length == 0) {
        $stockTableBody.append(rowTemplate.supplant(stock));
      } else {
        if (stock.Id - 2 >= 0) {
          $stockTableBody.find('tr').eq(stock.Id - 2).after(rowTemplate.supplant(stock));
        } else {
          $stockTableBody.find('tr').eq(0).before(rowTemplate.supplant(stock));
        }
      }
    },

    deleteStockPrice: function (stock) {
      var displayStock = formatPrice(stock);
      $stockTableBody.find('tr[data-id=' + stock.Id + ']').remove();
    }
  });


  $.extend(ticker.client, {
    updateShare: function (share) {
      var displayShare = formatPrice(share);
      $row = $(rowTemplate.supplant(displayShare)),
      $shareTableBody.find('tr[data-id=' + share.Id + ']').replaceWith($row);
    },

    insertShare: function (share) {
      var displayShare = formatPrice(share);
      if ($shareTableBody.find('tr').length == 0) {
        $shareTableBody.append(rowTemplate.supplant(share));
      } else {
        if (share.Id - 2 >= 0) {
          $shareTableBody.find('tr').eq(share.Id - 2).after(rowTemplate.supplant(share));
        } else {
          $shareTableBody.find('tr').eq(0).before(rowTemplate.supplant(share));
        }
      }
    },

    deleteShare: function (share) {
      var displayShare = formatPrice(share);
      $shareTableBody.find('tr[data-id=' + share.Id + ']').remove();
    }
  });

  // Start the connection
  $.connection.hub.start().then(function () {
    init();
    initShares();
  });
});