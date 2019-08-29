jq(function() {
	jq('.modal').modal();
	
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
	
    jq('a.make-payment').click(function(){
        jq("#payment-modal-table tr.itms").each(function(i, row) {
            if (i == 0){
                jq(this).removeClass('hide');
            }
            else{
                if (!jq(this).hasClass('hide')){
                    jq(this).addClass('hide');
                }
            }

            jq(this).find('td:eq(1) input').val('');
            jq(this).find('td:eq(2) input').val('');
            jq(this).find('td:eq(3) input').val('0.00');
            jq(this).find('td:eq(4) input').val('N/A');
            jq(this).find('td:eq(5) input.idnt-data').val(0);
        });

        jq('#payment-modal').modal('open');
    });
	
    jq('a.add-pymt-rows').click(function(){
        jq("#payment-modal-table tr.itms").each(function() {
            if (jq(this).hasClass('hide')){
                jq(this).removeClass('hide');
                return false;
            }
        });
    });
	
    jq('a.remove-pymt-row').click(function(){
        jq(this).closest('tr').remove();

        jq("#payment-modal-table tr.itms").each(function(i, row) {
            jq(this).find('td:eq(0)').text(eval(i+1) + '.');
        });
    });
	
    jq('#payment-modal a.modal-post').click(function(){
        var err_count = 0;

        jq("#payment-modal-table tbody tr.itms").each(function(i, $row) {
            if (!jq(this).hasClass('hide')){
                if (jq(this).find('td:eq(1) input').val().trim() == '') {
                    Materialize.toast('<span>Receipt number in row ' + eval(i+1) + ' cannot be blank</span><a class="btn-flat yellow-text pointer">FIX IT</a>', 3000)
                    err_count++;
                    return false;
                }

                if (!eval(jq(this).find('td:eq(3) input').val()) > 0) {
                    Materialize.toast('<span>Invalid payment amount in row ' + eval(i+1) + '</span><a class="btn-flat yellow-text" href="#!">Correct that</a>', 3000)
                    err_count++;
                    return false;
                }
            }

        });

        if (err_count > 0){
            return false;
        }

        jq('#payment-form').submit();
    });
	
    jq('#payment-table tbody').on('click', 'i.edit-payment', function(){
        if (isadmin == 'false'){
            Materialize.toast('<span>You do not have permission to perform this task</span><a class="btn-flat red-text pointer">Access Denied</a>', 3000);
            return false;
        }

        jq("tr.itms").each(function(i, row) {
            if (i == 0){
                jq(this).removeClass('hide');
            }
            else{
                if (!jq(this).hasClass('hide')){
                    jq(this).addClass('hide');
                }

                jq(this).find('td:eq(1) input').val('');
                jq(this).find('td:eq(3) input').val(0);
                jq(this).find('td:eq(4) input').val(0);
                jq(this).find('td:eq(5) input').val(0);
                jq(this).find('td:eq(7) input').val('N/A');
                jq(this).find('td:eq(8) input.json-data').val('"PettyCash":[]}');
                jq(this).find('td:eq(8) input.idnt-data').val(0);
            }
        });

        var row = jq('#payment-modal-table tbody tr:eq(0)');

        jq.ajax({
            dataType: "json",
            url: '/Customers/GetCustomersPayment',
            data: {
                "idnt": jq(this).data('idnt'),
				"code": xCode
            },
            success: function(pymt) {
                jq('#Date').val(pymt.date);

                row.find('td:eq(1) input').val(pymt.receipt);
                row.find('td:eq(2) input').val(pymt.cheque);
                row.find('td:eq(3) input').val(pymt.amount);
                row.find('td:eq(4) input').val(pymt.notes);
                row.find('td:eq(5) input.idnt-data').val(pymt.id);
            },
            error: function(xhr, ajaxOptions, thrownError) {
                console.log(xhr.status);
                console.log(thrownError);
            },
            complete: function() {
                jq('#payment-modal').modal('open');
            }
        });
    });
	
    jq('#payment-table tbody').on('click', 'i.delete-payment', function(){
        if (isadmin == 'false'){
            Materialize.toast('<span>You do not have permission to perform this task</span><a class="btn-flat red-text pointer">Access Denied</a>', 3000);
            return false;
        }

        line = jq(this).data('idnt');

        jq.ajax({
            dataType: "json",
            url: '/Customers/GetCustomersPayment',
            data: {
                "idnt": line,
				"code": xCode
            },
            success: function(delv) {
                jq('#delete-modal-field').html('Confirm deleting payment receipt <code class="language-css"> ' + delv.receipt + '</code> dated <code class="language-css">' + delv.date + '</code> for <code class="language-css">Kes ' + delv.amount.toString().toAccounting() + '</code>?');
            },
            error: function(xhr, ajaxOptions, thrownError) {
                console.log(xhr.status);
                console.log(thrownError);
            },
            complete: function() {
                jq('#delete-payment-modal').modal('open');
            }
        });
    });
	
    jq('#delete-payment-modal a.modal-post').click(function(){
        jq('#delete-payment-modal').modal('close');
        jq.ajax({
            type: "post",
            url: '/Customers/DeleteCustomersPayment',
            data: {
                "idnt": line,
				"code": xCode
            },
            success: function(delv) {
                Materialize.toast('<span>Successfully deleted customer payment</span><a class="btn-flat yellow-text pointer">Task Complete</a>', 3000);

                setTimeout(function(){
                    window.location.href = "/core/customers/" + xCode + "/" + xCust;
                }, 3000);
            },
            error: function(xhr, ajaxOptions, thrownError) {
                console.log(xhr.status);
                console.log(thrownError);
                Materialize.toast('<span>Error ' + xhr.status + '. ' + thrownError + '</span><a class="btn-flat red-text pointer">Delete Failed</a>', 3000);
            }
        });
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
                row += "<td>" + (pt.cheque == 0?' &mdash;':pt.cheque)  + "</td>";
                row += "<td class='bold-text' style='text-align:right;padding-right:15px;'>" + pt.amount.toString().toAccounting() + "</td>";
				row += "<td><i class='material-icons blue-text edit-payment left pointer' style='font-size:1em;' data-idnt='" + pt.id + "'>border_color</i><i class='material-icons red-text delete-payment right pointer' style='font-size:1.2em;' data-idnt='" + pt.id + "'>delete_forever</i></td>";
                row += "</tr>";

                total += pt.amount;

                jq('#payment-table tbody').append(row);
            });
			
			if (results.length == 0){
				jq('#payment-table tbody').append("<tr><td colspan='6'>NO PAYMENTS FOUND</td></tr>");
			}

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