$(function() {
    String.prototype.toAccounting = function() {
        var str = parseFloat(this).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
        if (str == '0.00') {
            return '&mdash; &nbsp;';
        }
        else if (str.charAt(0) == '-') {
            return '(' + str.substring(1, 40) + ')';
        }
        else {
            return str;
        }
    };

    jq('.get-ledger a').click(function() {
        GetStocksPurchasesLedgers();
    });
});


function GetStocksPurchasesLedgers(){
    jq.ajax({
        dataType: "json",
        url: '/Purchases/GetStocksPurchasesLedgers',
        data: {
            "code":     jq('#Code :selected').val(),
            "start":    jq('#start').val(),
            "stop":     jq('#stops').val(),
            "filter":   jq('#filter').val()
        },
        beforeSend: function() {
            jq('body').removeClass('loaded');
        },
        success: function(results) {
            jq('#ledger-table tbody').empty();
            jq('#ledger-table tfoot').empty();

            var cumm = 0.0;
            var excl = 0.0;
            var vats = 0.0;
            var zero = 0.0;

            jq.each(results, function(i, item) {
                cumm += item.total;
                excl += item.excl;
                vats += item.vats;
                zero += item.zero;

                var row = "<tr data-idnt='" + item.id + "'>";
                row += "<td>" + item.date + "</td>";
                row += "<td>" + item.ltrs.toString().toAccounting() + "</td>";
                row += "<td>" + item.price.toString().toAccounting() + "</td>";
                row += "<td>" + item.category + "</td>";
                row += "<td>" + item.description + "</td>";
                row += "<td>" + item.invoice + "</td>";
                row += "<td>" + item.supplier.name + "</td>";
                row += "<td class='right-text'>" + item.total.toString().toAccounting() + "</td>";
                row += "<td class='right-text bold-text'>" + cumm.toString().toAccounting() + "</td>";
                row += "<td class='right-text'>" + item.excl.toString().toAccounting() + "</td>";
                row += "<td class='right-text'>" + item.vats.toString().toAccounting() + "</td>";
                row += "<td class='right-text'>" + item.zero.toString().toAccounting() + "</td>";
                row += "</tr>";

                jq('#ledger-table tbody').append(row);
            })

            if (results.length == 0) {
                jq('#ledger-table tbody').append("<tr><td colspan=12>No Records Found</td></tr>");
            }
            else {
                var footr = "<tr><th>&nbsp;</th><th colspan=6 class='bold-text white-text'>LEDGER SUMMARY</th>";
                footr += "<th class='right-text'>" + cumm.toString().toAccounting() + "</th>";
                footr += "<th class='right-text'>" + cumm.toString().toAccounting() + "</th>";
                footr += "<th class='right-text'>" + excl.toString().toAccounting() + "</th>";
                footr += "<th class='right-text'>" + vats.toString().toAccounting() + "</th>";
                footr += "<th class='right-text'>" + zero.toString().toAccounting() + "</th>";
                footr += "</tr>";

                jq('#ledger-table tfoot').append(footr);
            }
        },
        error: function(xhr, ajaxOptions, thrownError) {
            console.log(xhr.status);
            console.log(thrownError);
        },
        complete: function() {
            $('body').addClass('loaded');
        }
    });
}