String.prototype.toAccounting = function() {
    return parseFloat(this).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
};

jq(function() {
    jq('.modal').modal();

    jq('.get-ledger a').click(function() {
        GetProductsTransfers();
    });
});

function GetProductsTransfers() {
    jq.ajax({
        dataType: "json",
        url: '/Sales/GetProductsTransfers',
        data: {
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

            var gits = 0;
            var kaag = 0;
            var kiru = 0;
            var nkub = 0;
            var totl = 0;

            jq.each(results, function(i, ldg) {
                var row = "<tr data-idnt='" + ldg.id + "'>";
                row += "<td>" + ldg.dateString + "</td>";
                row += "<td><a class='blue-text' href='/products/kinoru/" + ldg.product.id + "'>" + ldg.product.name + "</td>";
                row += "<td>" + ldg.product.category + "</td>";
                row += "<td class='center-text'>" + ldg.gitimbine + "</td>";
                row += "<td class='center-text'>" + ldg.kaaga + "</td>";
                row += "<td class='center-text'>" + ldg.kirunga + "</td>";
                row += "<td class='center-text'>" + ldg.nkubu + "</td>";
                row += "<td class='center-text bold-text'>" + ldg.total + "</td>";
                row += "<td><a class='material-icons tiny-box grey-text right pointer edit-transfer' style='margin-top: -5px' data-idnt=" + ldg.id + ">border_color</a></td>";
                row += "</tr>";

                gits += ldg.gitimbine;
                kaag += ldg.kaaga;
                kiru += ldg.kirunga;
                nkub += ldg.nkubu;
                totl += ldg.total;

                jq('#ledger-table tbody').append(row);
            })

            if (results.length == 0) {
                jq('#ledger-table tbody').append("<tr><td colspan='9'>NO RECORDS FOUND</td></tr>");
            }
            else {
                var footr = "<tr><th colspan='3' class='bold-text white-text'>LEDGER SUMMARY</th>";
                footr += "<th class='center-text'>" + gits + "</th>";
                footr += "<th class='center-text'>" + kaag + "</th>";
                footr += "<th class='center-text'>" + kiru + "</th>";
                footr += "<th class='center-text'>" + nkub + "</th>";
                footr += "<th class='center-text'>" + totl + "</th>";
                footr += "<th></th>";
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