$(function() {
    $('.modal').modal();

    jq('a.get-purchases').click(function(){
        GetPurchases();
    });
});

String.prototype.toAccounting = function() {
    var str =  parseFloat(this).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');

    if (str.charAt(0) == '-'){
        return '(' + str.substring(1,40) + ')';
    }
    else {
        return str;
    }
};

function GetPurchases() {
    jq.ajax({
        dataType: "json",
        url: '/Purchases/GetPurchases',
        data: {
            "start":    jq("#purchase-start-date").val(),
            "stop":     jq("#purchase-stops-date").val(),
            "filter":   jq('#purchase-filter').val()
        },
        beforeSend: function() {
            $('body').removeClass('loaded');
        },
        success: function(results) {
            var total = 0.0;

            jq('#purchase-table tbody').empty();
            jq('#purchase-table tfoot').empty();

            jq.each(results, function(i, purc) {
                var row = "<tr>";
                row += "<td>" + purc.dateString + "</td>";
                row += "<td>" + purc.category + "</td>";
                row += "<td>" + purc.supplier.name + "</td>";
                row += "<td>" + purc.lpo + "</td>";
                row += "<td>" + purc.invoice + "</td>";
                row += "<td>" + purc.station.synonym + "</td>";
                row += "<td class='right-align'>" + purc.amount.toString().toAccounting() + "</td>";
                row += "<td><a class='material-icons tiny-box grey-text' data-idnt=" + purc.id + ">border_color</a><a class='material-icons tiny-box red-text' data-idnt=" + purc.id + ">delete_forever</a></td>";
                row += "</tr>";

                total += purc.amount;

                jq('#purchase-table tbody').append(row);
            });

            if (results.length == 0){
                jq('#purchase-table tbody').append("<tr><td colspan='8'>NO RECORDS FOUND</td></tr>");
            }

            var footr = "<tr>";
            footr += "<th colspan='6'>SUMMARY</td>";
            footr += "<th class='right-align'>" + total.toString().toAccounting() + "</td>";
            footr += "</tr>";

            jq('#purchase-table tfoot').append(footr);

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