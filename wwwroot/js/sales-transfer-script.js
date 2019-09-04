String.prototype.toAccounting = function() {
    return parseFloat(this).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
};

jq(function() {
    jq('.modal').modal();

    jq('div.get-ledger a').click(function() {
        GetProductsTransfers();
    });
	
    jq('div.get-compare a').click(function() {
        GetProductsTransferCompare();
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

function GetProductsTransferCompare() {
    jq.ajax({
        dataType: "json",
        url: '/Sales/GetProductsTransferCompare',
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

            var gt_load = 0;
			var gt_delv = 0;
            var kg_load = 0;
            var kg_delv = 0;
            var kr_load = 0;
            var kr_delv = 0;
            var nk_load = 0;
            var nk_delv = 0;
            var tt_load = 0;
            var tt_delv = 0;

            jq.each(results, function(i, ldg) {
                var row = "<tr class='" + (ldg.error == 1 ? "red-text" : "") + "'>";
                row += "<td>" + eval(i+1) + "</td>";
                row += "<td><a class='" + (ldg.error == 1 ? "red-text" : "blue-text") + "' href='/products/kinoru/" + ldg.load.product.id + "'>" + ldg.load.product.name + "</td>";
                row += "<td class='center-text'>" + ldg.load.gitimbine + "</td>";
                row += "<td class='center-text'>" + ldg.load.kaaga + "</td>";
                row += "<td class='center-text'>" + ldg.load.kirunga + "</td>";
                row += "<td class='center-text'>" + ldg.load.nkubu + "</td>";
                row += "<td class='center-text bold-text'>" + ldg.load.total + "</td>";
                row += "<td class='center-text'>" + ldg.delv.gitimbine + "</td>";
                row += "<td class='center-text'>" + ldg.delv.kaaga + "</td>";
                row += "<td class='center-text'>" + ldg.delv.kirunga + "</td>";
                row += "<td class='center-text'>" + ldg.delv.nkubu + "</td>";
                row += "<td class='center-text bold-text'>" + ldg.delv.total + "</td>";
                row += "</tr>";

                gt_load += ldg.load.gitimbine;
                kg_load += ldg.load.kaaga;
                kr_load += ldg.load.kirunga;
                nk_load += ldg.load.nkubu;
                tt_load += ldg.load.total;
				
                gt_delv += ldg.delv.gitimbine;
                kg_delv += ldg.delv.kaaga;
                kr_delv += ldg.delv.kirunga;
                nk_delv += ldg.delv.nkubu;
                tt_delv += ldg.delv.total;

                jq('#ledger-table tbody').append(row);
            })

            if (results.length == 0) {
                jq('#ledger-table tbody').append("<tr><td colspan='9'>NO TRANSFERS FOUND</td></tr>");
            }
            else {
                var footr = "<tr><th colspan='2' class='bold-text white-text'>LEDGER SUMMARY</th>";
                footr += "<th class='center-text'>" + gt_load + "</th>";
                footr += "<th class='center-text'>" + kg_load + "</th>";
                footr += "<th class='center-text'>" + kr_load + "</th>";
                footr += "<th class='center-text'>" + nk_load + "</th>";
                footr += "<th class='center-text'>" + tt_load + "</th>";
                footr += "<th class='center-text'>" + gt_delv + "</th>";
                footr += "<th class='center-text'>" + kg_delv + "</th>";
                footr += "<th class='center-text'>" + kr_delv + "</th>";
                footr += "<th class='center-text'>" + nk_delv + "</th>";
                footr += "<th class='center-text'>" + tt_delv+ "</th>";
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