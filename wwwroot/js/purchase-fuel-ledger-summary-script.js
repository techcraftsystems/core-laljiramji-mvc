jq(function() {
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
        GetFuelPurchasesLedgers();
    });

    function GetFuelPurchasesLedgers() {
        var opening = 0.0;

        jq.ajax({
            dataType: "json",
            url: '/Purchases/GetFuelPurchasesLedgersSummary',
            data: {
                "start": jq('#start').val(),
                "stop": jq('#stops').val(),
                "filter": jq('#filter').val()
            },
            beforeSend: function() {
                jq('body').removeClass('loaded');
            },
            success: function(results) {
                jq('#ledger-table tbody').empty();
                jq('#ledger-table tfoot').empty();

                var dx = 0.0;
                var ux = 0.0;
                var vp = 0.0;
                var ik = 0.0;
                var total = 0.0;
                var excl = 0.0;
                var zero = 0.0;
                var vats = 0.0;

                jq.each(results, function(i, item) {
                    dx += item.dx;
                    ux += item.ux;
                    vp += item.vp;
                    ik += item.ik;
                    excl += item.excl;
                    zero += item.zero;
                    vats += item.vats;
                    total += item.total;

                    var row = "<tr>";
                    row += "<td>" + item.name + "</td>";
                    row += "<td class='right-text'>" + item.dx.toString().toAccounting() + "</td>";
                    row += "<td class='right-text'>" + item.ux.toString().toAccounting() + "</td>";
                    row += "<td class='right-text'>" + item.vp.toString().toAccounting() + "</td>";
                    row += "<td class='right-text'>" + item.ik.toString().toAccounting() + "</td>";
                    row += "<td class='right-text bold-text'>" + item.total.toString().toAccounting() + "</td>";
                    row += "<td class='right-text'>" + item.excl.toString().toAccounting() + "</td>";
                    row += "<td class='right-text'>" + item.vats.toString().toAccounting() + "</td>";
                    row += "<td class='right-text'>" + item.zero.toString().toAccounting() + "</td>";
                    row += "<td class='right-text'>" + item.total.toString().toAccounting() + "</td>";
                    row += "</tr>";

                    jq('#ledger-table tbody').append(row);
                })

                if (results.length == 0) {
                    jq('#ledger-table tbody').append("<tr><td colspan=12>No Records Found</td></tr>");
                }
                else {
                    var footr = "<tr><td>&nbsp;</td>";
                    footr += "<td class='right-text white-text bold-text'>" + dx.toString().toAccounting() + "</td>";
                    footr += "<td class='right-text white-text bold-text'>" + ux.toString().toAccounting() + "</td>";
                    footr += "<td class='right-text white-text bold-text'>" + vp.toString().toAccounting() + "</td>";
                    footr += "<td class='right-text white-text bold-text'>" + ik.toString().toAccounting() + "</td>";
                    footr += "<td class='right-text white-text bold-text'>" + total.toString().toAccounting() + "</td>";
                    footr += "<td class='right-text white-text bold-text'>" + excl.toString().toAccounting() + "</td>";
                    footr += "<td class='right-text white-text bold-text'>" + vats.toString().toAccounting() + "</td>";
                    footr += "<td class='right-text white-text bold-text'>" + excl.toString().toAccounting() + "</td>";
                    footr += "<td class='right-text white-text bold-text'>" + total.toString().toAccounting() + "</td>";
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

    GetFuelPurchasesLedgers();
});