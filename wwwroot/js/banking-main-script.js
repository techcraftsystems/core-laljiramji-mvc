jq(function() {
    jq(".collection").on("click", ".email-unread", function() {
        var action = jq(this).data('account');
        if (action != xCode){
            window.location.href = action;
        }
    });

    jq('ul .recon').click(function(){
        if(jq("#reconcile-table").data('loaded') == 0){
            jq("#reconcile-table").data('loaded', 1);
            GetStationsReconciles();
        }
    });

    jq('div.get-reconcile a').click(function(){
        GetStationsReconciles()
    });
});

function GetStationsReconciles(){
    jq.ajax({
        dataType: "json",
        url: '/Banking/GetStationsReconciles',
        data: {
            "accounts": xIdnt,
            "year":     jq('#reconcile-year :selected').val(),
            "mnth":     jq('#reconcile-mnth :selected').val()
        },
        beforeSend: function() {
            jq('body').removeClass('loaded');
        },
        success: function(results) {
            var cumm = 0.0;
            var revn = 0.0;
            var exps = 0.0;

            jq('#reconcile-table tbody').empty();
            jq('#reconcile-table tfoot').empty();

            jq.each(results, function(i, recon) {
                revn += recon.revenue;
                exps += recon.expense;
                cumm = recon.cummulative;

                var row = "<tr>";
                row += "<td>" + recon.date + "</td>";
                row += "<td>" + recon.quantity + "</td>";
                row += "<td>" + recon.price + "</td>";
                row += "<td>" + recon.description + "</td>";
                row += "<td class='center-align'>" + recon.customer + "</td>";
                row += "<td class='center-align'>" + recon.cheque + "</td>";
                row += "<td class='center-align'>" + recon.invoice + "</td>";
                row += "<td class='red-text bold-text right-align'>" + recon.revenue.toString().toAccounting() + "</td>";
                row += "<td class='red-text bold-text right-align'>" + recon.expense.toString().toAccounting() + "</td>";
                row += "<td class='blue-text bold-text right-align'>" + recon.cummulative.toString().toAccounting() + "</td>";
                row += "</tr>";

                jq('#reconcile-table tbody').append(row);
            })

            var footr = "<tr>";
            footr += "<td>&nbsp;</td>";
            footr += "<td>&nbsp;</td>";
            footr += "<td>&nbsp;</td>";
            footr += "<td class='red-text bold-text' colspan='4'>SUMMARY</td>";
            footr += "<td class='red-text bold-text right-align'>" + revn.toString().toAccounting() + "</td>";
            footr += "<td class='red-text bold-text right-align'>" + exps.toString().toAccounting() + "</td>";
            footr += "<td class='blue-text bold-text right-align'>" + cumm.toString().toAccounting() + "</td>";
            footr += "</tr>";

            jq('#reconcile-table tfoot').append(footr);

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