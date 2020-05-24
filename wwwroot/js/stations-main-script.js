jq(function() {
    jq(".collection").on("click", ".email-unread", function() {
        var action = jq(this).data('station');
        if (action != xCode){
            window.location.href = action;
        }
    });

    //::First Time clicks
    jq('ul .recon').click(function(){
        if(jq("#reconcile-table").data('loaded') == 0){
            GetStationsReconciles();
            jq("#reconcile-table").data('loaded', 1);
        }
    });

    jq('ul .ledgr').click(function(){
        if(jq("#ledger-table").data('loaded') == 0){
            GetLedgerEntries();
            jq("#ledger-table").data('loaded', 1);
        }
    });

    jq('ul .expns').click(function(){
        if(jq("#expense-table").data('loaded') == 0){
            GetExpenditure();
            jq("#expense-table").data('loaded', 1);
        }
    });

    jq('ul .purch').click(function(){
        if(jq("#purchase-table").data('loaded') == 0){
            GetPurchasesOthers();
            jq("#purchase-table").data('loaded', 1);
        }
    });

    jq('ul .other').click(function(){
        if(jq("#other-table").data('loaded') == 0){
            GetPurchasesOthers();
            jq("#other-table").data('loaded', 1);
        }
    });

    jq('ul .pymts').click(function(){
        if(jq("#payment-table").data('loaded') == 0){
            GetCustomerPayments();
            jq("#payment-table").data('loaded', 1);
        }
    });

    //::Link Clicks
    jq('.get-reconcile a').click(function(){
        GetStationsReconciles();
    });

    jq('.get-ledger a').click(function(){
        GetLedgerEntries();
    });

    jq('.get-expense a').click(function(){
        GetExpenditure();
    });

    jq('.get-purchases a').click(function(){
        GetPurchasesLedger();
    });

    jq('.get-others a').click(function(){
        GetPurchasesOthers();
    });

    jq('.get-payments a').click(function(){
        GetCustomerPayments();
    });

    //Scroll Dates
    jq('button.fc-button-date').click(function(){
        window.location.href = "/core/stations/" + xCode + "?dt=" + jq(this).data('date');
    });
});

function GetStationsReconciles(){
    jq.ajax({
        dataType: "json",
        url: '/Stations/GetStationsReconciles',
        data: {
            "stid": xIdnt,
            "year": jq('#reconcile-year :selected').val(),
            "mnth": jq('#reconcile-mnth :selected').val()
        },
        beforeSend: function() {
            jq('body').removeClass('loaded');
        },
        success: function(results) {
            var cumm = 0.0;
            var amts = 0.0;
            var paid = 0.0;
            var uprn = 0.0;
            var debt = 0.0;
            var disc = 0.0;
            var tnsp = 0.0;

            jq('#reconcile-table tbody').empty();
            jq('#reconcile-table tfoot').empty();

            jq.each(results, function(i, recon) {
                cumm += recon.balance;
                amts += recon.amount;
                paid += recon.payment;
                uprn += recon.uprna;
                debt += recon.debt;
                disc += recon.discount;
                tnsp += recon.transport;

                var row = "<tr>";
                row += "<td>" + recon.date + "</td>";
                row += "<td>" + recon.amount.toString().toAccounting() + "</td>";
                row += "<td>" + recon.payment.toString().toAccounting() + "</td>";
                row += "<td>" + recon.uprna.toString().toAccounting() + "</td>";
                row += "<td>" + recon.debt.toString().toAccounting() + "</td>";
                row += "<td>" + recon.discount.toString().toAccounting() + "</td>";
                row += "<td>" + recon.transport.toString().toAccounting() + "</td>";
                row += "<td class='red-text bold-text'>" + recon.balance.toString().toAccounting() + "</td>";
                row += "<td class='blue-text bold-text'>" + cumm.toString().toAccounting() + "</td>";
                row += "</tr>";

                jq('#reconcile-table tbody').append(row);
            })

            var footr = "<tr>";
            footr += "<td></td>";
            footr += "<td class='bold-text'>" + amts.toString().toAccounting() + "</td>";
            footr += "<td class='bold-text'>" + paid.toString().toAccounting() + "</td>";
            footr += "<td class='bold-text'>" + uprn.toString().toAccounting() + "</td>";
            footr += "<td class='bold-text'>" + debt.toString().toAccounting() + "</td>";
            footr += "<td class='bold-text'>" + disc.toString().toAccounting() + "</td>";
            footr += "<td class='bold-text'>" + tnsp.toString().toAccounting() + "</td>";
            footr += "<td class='red-text bold-text'>" + cumm.toString().toAccounting() + "</td>";
            footr += "<td class='blue-text bold-text'>" + cumm.toString().toAccounting() + "</td>";
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

function GetLedgerEntries(){
    jq.ajax({
        dataType: "json",
        url: '/Stations/GetLedgerEntries',
        data: {
            "stid":     xIdnt,
            "start":    jq('#ledgerStartDate').val(),
            "stop":     jq('#ledgerStopsDate').val(),
            "filter":   jq('#ledgerFilter').val()
        },
        beforeSend: function() {
            jq('body').removeClass('loaded');
        },
        success: function(results) {
            jq('#ledger-table tbody').empty();
            jq('#ledger-table tfoot').empty();

            var cumm = 0.0;

            jq.each(results, function(i, entry) {
                cumm += entry.amount;

                var name = entry.name;
                if (entry.action == 0) {
                    name = "<a href='/customers/" + xCode + "/" + entry.account + "'>" + entry.name + "</a>";
                }

                var row = "<tr data-idnt='" + entry.id + "' data-action='" + entry.action + "' data-account='" + entry.account + "'>";
                row += "<td>" + entry.date + "</td>";
                row += "<td>" + entry.quantity.toString().toAccounting() + "</td>";
                row += "<td>" + entry.description + "</td>";
                row += "<td>" + entry.lpo + "</td>";
                row += "<td>" + entry.invoice + "</td>";
                row += "<td>" + name + "</td>";
                row += "<td class='right'>" + entry.price.toString().toAccounting() + "</td>";
                row += "<td class='bold-text'>" + entry.amount.toString().toAccounting() + "</td>";
                row += "<td><i class='material-icons blue-text left'>code</i><i class='material-icons red-text right'>delete_forever</i></td>";
                row += "</tr>";

                jq('#ledger-table tbody').append(row);
            })

            var footr = "<tr>";
            footr += "<td class='bold-text' colspan='7'>&nbsp;SUMMARY</td>";
            footr += "<td class='bold-text right'>" + cumm.toString().toAccounting() + "</td>";
            footr += "<td>&nbsp;</td>";
            footr += "</tr>";

            jq('#ledger-table tfoot').append(footr);
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

function GetExpenditure(){
    jq.ajax({
        dataType: "json",
        url: '/Stations/GetExpenditure',
        data: {
            "stid":     xIdnt,
            "start":    jq('#expenseStartDate').val(),
            "stop":     jq('#expenseStopsDate').val(),
            "filter":   jq('#expenseFilter').val()
        },
        beforeSend: function() {
            jq('body').removeClass('loaded');
        },
        success: function(results) {
            jq('#expense-table tbody').empty();
            jq('#expense-table tfoot').empty();

            var cumm = 0.0;

            jq.each(results, function(i, expense) {
                cumm += expense.amount;

                var row = "<tr data-idnt='" + expense.id + "' data-account='" + expense.account.id + "'>";
                row += "<td>" + expense.date + "</td>";
                row += "<td>" + expense.location + "</td>";
                row += "<td>" + expense.account.name + "</td>";
                row += "<td>" + expense.description + "</td>";
                row += "<td class='bold-text'>" + expense.amount.toString().toAccounting() + "</td>";
                row += "<td><i class='material-icons blue-text left'>code</i><i class='material-icons red-text right'>delete_forever</i></td>";
                row += "</tr>";

                jq('#expense-table tbody').append(row);
            })

            var footr = "<tr>";
            footr += "<td class='bold-text' colspan='4'>&nbsp;EXPENDITURE SUMMARY</td>";
            footr += "<td class='bold-text right'>" + cumm.toString().toAccounting() + "</td>";
            footr += "<td>&nbsp;</td>";
            footr += "</tr>";

            jq('#expense-table tfoot').append(footr);
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

function GetPurchasesOthers(){
    jq.ajax({
        dataType: "json",
        url: '/Stations/GetPurchasesOthers',
        data: {
            "stid":     xIdnt,
            "start":    jq('#otherStartDate').val(),
            "stop":     jq('#otherStopsDate').val(),
            "filter":   jq('#otherFilter').val()
        },
        beforeSend: function() {
            jq('body').removeClass('loaded');
        },
        success: function(results) {
            jq('#other-table tbody').empty();
            jq('#other-table tfoot').empty();

            var taxx = 0.0;
            var summ = 0.0;

            jq.each(results, function(i, ledger) {
                summ += ledger.total;
                taxx += ledger.taxable;

                var row = "<tr data-idnt='" + ledger.id + "'>";
                row += "<td>" + ledger.date + "</td>";
                row += "<td>" + ledger.type + "</td>";
                row += "<td>" + ledger.supplier.name + "</td>";
                row += "<td>" + ledger.invoice + "</td>";
                row += "<td>" + ledger.lpo + "</td>";
                row += "<td>N/A</td>";
                row += "<td class='right-text'>" + ledger.total.toString().toAccounting() + "</td>";
                row += "<td class='right-text'>" + ledger.taxable.toString().toAccounting() + "</td>";
                row += "<td><a class='material-icons tiny-box grey-text link-fuel' data-idnt='" + ledger.id + "' style='margin-top:-5px'>border_color</a></td>";
                row += "</tr>";

                jq('#other-table tbody').append(row);
            });

            if (results.length == 0){
                jq('#other-table tbody').append("<tr><td colspan='9'>NO RECORDS FOUND</td></tr>");
            }

            var footr = "<tr>";
            footr += "<td class='bold-text' colspan='6'>&nbsp;PURCHASES SUMMARY</td>";
            footr += "<td class='bold-text right-text'>" + summ.toString().toAccounting() + "</td>";
            footr += "<td class='bold-text right-text'>" + taxx.toString().toAccounting() + "</td>";
            footr += "</tr>";

            jq('#other-table tfoot').append(footr);
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

function GetPurchasesLedger(){
    jq.ajax({
        dataType: "json",
        url: '/Stations/GetPurchasesLedger',
        data: {
            "stid":     xIdnt,
            "start":    jq('#purchaseStartDate').val(),
            "stop":     jq('#purchaseStopsDate').val(),
            "filter":   jq('#purchaseFilter').val()
        },
        beforeSend: function() {
            jq('body').removeClass('loaded');
        },
        success: function(results) {
            jq('#purchase-table tbody').empty();
            jq('#purchase-table tfoot').empty();

            var ltrs = 0.0;
            var delv = 0.0;
            var purc = 0.0;
            var cumm = 0.0;

            var array_delv = [0.0, 0.0, 0.0, 0.0, 0.0];
            var array_purc = [0.0, 0.0, 0.0, 0.0, 0.0];

            jq.each(results, function(i, ledger) {
                ltrs += ledger.quantity;
                delv += ledger.delivery;
                purc += ledger.purchase;
                cumm += ledger.total;

                if (ledger.type == 'DELV') {
                    array_delv[ledger.fuel.id] += ledger.delivery;
                }
                else {
                    array_purc[ledger.fuel.id] += ledger.purchase;
                }

                var supp = '';

                if (ledger.supplier.id != 0) {
                    supp = "<a href='/core/suppliers/" + ledger.supplier.id + "'>" + ledger.supplier.name + "</a>";
                }
                else {
                    supp = ledger.supplier.name;
                }

                var row = "<tr data-idnt='" + ledger.id + "'>";
                row += "<td>" + ledger.date + "</td>";
                row += "<td>" + ledger.type + "</td>";
                row += "<td><a href='/core/fuel/" + ledger.fuel.id + "'>" + ledger.fuel.name + "</td>";
                row += "<td>" + supp + "</td>";
                row += "<td>" + ledger.invoice + "</td>";
                row += "<td>" + ledger.delivery.toString().toAccounting() + "</td>";
                row += "<td>" + ledger.purchase.toString().toAccounting() + "</td>";
                row += "<td>" + ledger.price.toString().toAccounting() + "</td>";
                row += "<td class='bold-text right'>" + ledger.total.toString().toAccounting() + "</td>";
                row += "</tr>";

                jq('#purchase-table tbody').append(row);
            })

            var footr = "<tr>";
            footr += "<td class='bold-text' colspan='5'>&nbsp;PURCHASES SUMMARY</td>";
            footr += "<td class='bold-text'>" + delv.toString().toAccounting() + "</td>";
            footr += "<td class='bold-text'>" + purc.toString().toAccounting() + "</td>";
            footr += "<td>&nbsp;</td>";
            footr += "<td class='bold-text right'>" + cumm.toString().toAccounting() + "</td>";
            footr += "</tr>";

            jq('#purchase-table tfoot').append(footr);

            jq('#d1').text(array_purc[1].toString().toAccounting());
            jq('#s1').text(array_purc[2].toString().toAccounting());
            jq('#v1').text(array_purc[3].toString().toAccounting());
            jq('#k1').text(array_purc[4].toString().toAccounting());
            jq('#t1').text(purc.toString().toAccounting());

            jq('#d2').text(array_delv[1].toString().toAccounting());
            jq('#s2').text(array_delv[2].toString().toAccounting());
            jq('#v2').text(array_delv[3].toString().toAccounting());
            jq('#k2').text(array_delv[4].toString().toAccounting());
            jq('#t2').text(delv.toString().toAccounting());

            jq('#d3').text((array_delv[1]-array_purc[1]).toString().toAccounting());
            jq('#s3').text((array_delv[2]-array_purc[2]).toString().toAccounting());
            jq('#v3').text((array_delv[3]-array_purc[3]).toString().toAccounting());
            jq('#k3').text((array_delv[4]-array_purc[4]).toString().toAccounting());
            jq('#t3').text((delv-purc).toString().toAccounting());

            console.log(array_delv);
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
            "date1":  jq("#paymentStartDate").val(),
            "date2":  jq("#paymentStopsDate").val(),
            "filter": jq('#paymentFilter').val(),
            "stations": xIdnt,
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
                row += "<td><a class='blue-text' href='/core/customers/" + pt.station.code + "/" + pt.customer.id + "'>" + pt.customer.name + "</a></td>";
                row += "<td>" + pt.receipt + "</td>";
                row += "<td>" + pt.cheque + "</td>";
                row += "<td class='bold-text' style='text-align:right;padding-right:15px;'>" + pt.amount.toString().toAccounting() + "</td>";
                row += "<td>" + pt.notes + "</td>";
                row += "<td><a class='material-icons tiny-box grey-text right'>border_color</a></td>";
                row += "</tr>";

                total += pt.amount;

                jq('#payment-table tbody').append(row);
            })

            var footr = "<tr>";
            footr += "<td>&nbsp;</td>";
            footr += "<td class='bold-text' colspan='5'>PAYMENT SUMMARY</td>";
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

function DateValidated(date){
    if (date.length != 10){
        return false;
    }

    var dateRegex = /^(?:(?:31(\/|-|\.)(?:0?[13578]|1[02]))\1|(?:(?:29|30)(\/|-|\.)(?:0?[13-9]|1[0-2])\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(\/|-|\.)0?2\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(\/|-|\.)(?:(?:0?[1-9])|(?:1[0-2]))\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$/;
    return dateRegex.test(date);
}