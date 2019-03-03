jq(function() {
    jq('div.purchases').click(function(){
        var perc = jq(this).data('rate');
        GetVatDownloadPurchaseEntries(perc);
    });

    function GetVatDownloadPurchaseEntries(perc){
        jq.ajax({
            dataType: "json",
            url: '/Reports/GetVatDownloadPurchaseEntries',
            data: {
                "rate": perc,
                "year": year,
                "mnth": mnth
            },
            beforeSend: function() {
                jq('body').removeClass('loaded');
            },
            success: function(results) {
                jq('#purchase-table tbody').empty();

                jq.each(results, function(i, entry) {
                    var row = "<tr>";
                    row += "<td>LOCAL</td>";
                    row += "<td>" + entry.supplier.pin + "</td>";
                    row += "<td>" + entry.supplier.name + "</td>";
                    row += "<td>" + entry.date + "</td>";
                    row += "<td>" + entry.invoice + "</td>";
                    row += "<td>" + entry.description + "</td>";
                    row += "<td></td>";
                    row += "<td>" + entry.amount + "</td>";
                    row += "</tr>";

                    jq('#purchase-table tbody').append(row);
                })
            },
            error: function(xhr, ajaxOptions, thrownError) {
                console.log(xhr.status);
                console.log(thrownError);
            },
            complete: function() {
                jq('body').addClass('loaded');
                var doc = perc + '%-Purchase.csv';
                ExportTableToCSV(jq('#purchase-table'), doc);
                Materialize.toast('<span>' + doc + ' will start downloading shortly</span><a class="btn-flat yellow-text" href="#!">Close<a>', 3000);
            }
        });
    }

    function ExportTableToCSV($table, filename) {
        var $rows = $table.find('tr:has(td)'),
          tmpColDelim = String.fromCharCode(11), // vertical tab character
          tmpRowDelim = String.fromCharCode(0), // null character

          colDelim = '","',
          rowDelim = '"\r\n"',

          // Grab text from table into CSV formatted string
          csv = '"' + $rows.map(function(i, row) {
            var $row = $(row),
              $cols = $row.find('td');

            return $cols.map(function(j, col) {
              var $col = $(col),
                text = $col.text();

              return text.replace(/"/g, '""'); // escape double quotes

            }).get().join(tmpColDelim);

          }).get().join(tmpRowDelim)
          .split(tmpRowDelim).join(rowDelim)
          .split(tmpColDelim).join(colDelim) + '"';

        console.log(csv);
        // Deliberate 'false', see comment below
        if (false && window.navigator.msSaveBlob) {
          var blob = new Blob([decodeURIComponent(csv)], {
            type: 'text/csv;charset=utf8'
          });

          window.navigator.msSaveBlob(blob, filename);

        } else if (window.Blob && window.URL) {
          var blob = new Blob([csv], {
            type: 'text/csv;charset=utf-8'
          });
          var csvUrl = URL.createObjectURL(blob);

          $(this)
            .attr({
              'download': filename,
              'href': csvUrl
            });
        } else {
          var csvData = 'data:application/csv;charset=utf-8,' + encodeURIComponent(csv);

          $(this)
            .attr({
              'download': filename,
              'href': csvData,
              'target': '_blank'
            });
        }
    }
});