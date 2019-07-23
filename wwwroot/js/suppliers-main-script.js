String.prototype.toAccounting = function() {
    var str =  parseFloat(this).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
    if (str.charAt(0) == '-'){
        return '(' + str.substring(1,40) + ')';
    }
    else {
        return str;
    }
};

jq(function() {
    jq('ul .invs').click(function(){
        if(jq("#invoice-table").data('loaded') == 0){
            GetSupplierExpenses();
            jq("#invoice-table").data('loaded', 1);
        }
    });

    jq('ul li.pymt').click(function(){
        if(jq("#payment-table").data('loaded') == 0){
            //GetCustomerPayments();
            jq("#payment-table").data('loaded', 1);
        }
    });

    jq('.get-invoices a').click(function(){
        GetSupplierExpenses();
    });

    jq('.get-payments a').click(function(){
        //GetCustomerPayments();
    });
});

function GetSupplierExpenses(){
    jq.ajax({
        dataType: "json",
        url: '/Suppliers/GetSupplierExpenses',
        data: {
            "supp":     xSupp,
            "start":    jq('#invoiceStartDate').val(),
            "stop":     jq('#invoiceStopsDate').val(),
            "filter":   jq('#invoiceFilter').val()
        },
        beforeSend: function() {
            jq('body').removeClass('loaded');
        },
        success: function(results) {
            jq('#invoice-table tbody').empty();
            jq('#invoice-table tfoot').empty();

            var cumm = 0.0;

            jq.each(results, function(i, entry) {
                cumm += entry.amount;

                var row = "<tr data-idnt='" + entry.id + "'>";
                row += "<td>" + entry.dateString + "</td>";
                row += "<td>" + entry.invoice + "</td>";
                row += "<td>" + entry.category + "</td>";
                row += "<td>" + entry.description + "</td>";
                row += "<td>" + entry.station.code.toUpperCase() + "</td>";
                row += "<td class='bold-text'>" + entry.amount.toString().toAccounting() + "</td>";
                row += "<td><i class='material-icons blue-text left pointer' style='font-size:1em;'>border_color</i><i class='material-icons red-text right pointer' style='font-size:1em;'>delete_forever</i></td>";
                row += "</tr>";

                jq('#invoice-table tbody').append(row);
            })

            var footr = "<tr>";
            footr += "<td class='bold-text' colspan='5'>&nbsp;SUMMARY</td>";
            footr += "<td class='bold-text right'>" + cumm.toString().toAccounting() + "</td>";
            footr += "<td>&nbsp;</td>";
            footr += "</tr>";

            jq('#invoice-table tfoot').append(footr);
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