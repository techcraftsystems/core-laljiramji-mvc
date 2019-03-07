jq(function() {
    jq('a.export').on('click', function(event){

        var table = jq(this).data('table');
        var title = jq(this).data('title');

        Materialize.toast('<span>' + title + ' will start downloading shortly</span><a class="btn-flat yellow-text" href="#!">Close<a>', 3000);

        var args = [$('#'+table), title];
        ExportTableToCSV.apply(this, args);
    });

    function ExportTableToCSV($table, filename) {
        var $rows = $table.find('tr:has(td)'),
          tmpColDelim = String.fromCharCode(11), // vertical tab character
          tmpRowDelim = String.fromCharCode(0), // null character

          // actual delimiter characters for CSV format
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

        // Deliberate 'false', see comment below
        if (false && window.navigator.msSaveBlob) {
          var blob = new Blob([decodeURIComponent(csv)], {
            type: 'text/csv;charset=utf8'
          });

          // Crashes in IE 10, IE 11 and Microsoft Edge
          // See MS Edge Issue #10396033
          // Hence, the deliberate 'false'
          // This is here just for completeness
          // Remove the 'false' at your own risk
          window.navigator.msSaveBlob(blob, filename);

        } else if (window.Blob && window.URL) {
          // HTML5 Blob        
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
          // Data URI
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