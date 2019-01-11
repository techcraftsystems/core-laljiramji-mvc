jq(function() {
    jq('ul .invs').click(function(){
        if(jq("#invoice-table").data('loaded') == 0){
            GetLedgerEntries();
            jq("#invoice-table").data('loaded', 1);
        }
    });

    jq('ul li.pymt').click(function(){
        if(jq("#payment-table").data('loaded') == 0){
            GetCustomerPayments();
            jq("#payment-table").data('loaded', 1);
        }
    });

    jq('.get-invoices a').click(function(){
        GetLedgerEntries();
    });

    jq('.get-payments a').click(function(){
        GetCustomerPayments();
    });
});

function GetLedgerEntries(){
    jq.ajax({
        dataType: "json",
        url: '/Customers/GetLedgerEntries',
        data: {
            "custid":   xCust,
            "stid":     xStid,
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

                var row = "<tr data-idnt='" + entry.id + "' data-action='" + entry.action + "' data-account='" + entry.account + "'>";
                row += "<td>" + entry.date + "</td>";
                row += "<td>" + entry.quantity.toString().toAccounting() + "</td>";
                row += "<td>" + entry.description + "</td>";
                row += "<td>" + entry.lpo + "</td>";
                row += "<td>" + entry.invoice + "</td>";
                row += "<td>" + entry.price.toString().toAccounting() + "</td>";
                row += "<td class='bold-text'>" + entry.amount.toString().toAccounting() + "</td>";
                row += "<td><i class='material-icons blue-text left'>code</i><i class='material-icons red-text right'>delete_forever</i></td>";
                row += "</tr>";

                jq('#invoice-table tbody').append(row);
            })

            var footr = "<tr>";
            footr += "<td class='bold-text' colspan='6'>&nbsp;SUMMARY</td>";
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

function GetCustomerPayments() {
    jq.ajax({
        dataType: "json",
        url: '/Stations/GetCustomerPayments',
        data: {
            "date1":        jq("#paymentStartDate").val(),
            "date2":        jq("#paymentStopsDate").val(),
            "stations":     xStid,
            "customers":    xCust,
            "filter":       jq('#paymentFilter').val()
        },
        beforeSend: function() {
            $('body').removeClass('loaded');
        },
        success: function(results) {
            var total = 0.0;

            jq('#payment-table tbody').empty();
            jq('#payment-table tfoot').empty();

            jq.each(results, function(i, pt) {
                var row = "<tr>";
                row += "<td>" + pt.date + "</td>";
                row += "<td>" + pt.receipt + "</td>";
                row += "<td>" + pt.notes + "</td>";
                row += "<td>" + pt.cheque + "</td>";
                row += "<td class='bold-text' style='text-align:right;padding-right:15px;'>" + pt.amount.toString().toAccounting() + "</td>";
                row += "<td><a class='material-icons tiny-box grey-text right'>border_color</a></td>";
                row += "</tr>";

                total += pt.amount;

                jq('#payment-table tbody').append(row);
            })

            var footr = "<tr>";
            footr += "<td class='bold-text' colspan='4'>PAYMENT SUMMARY</td>";
            footr += "<td class='bold-text' style='text-align:right;padding-right:15px;'>" + total.toString().toAccounting() + "</td>";
            footr += "</tr>";

            jq('#payment-table tfoot').append(footr);

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