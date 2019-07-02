String.prototype.toAccounting = function() {
    return parseFloat(this).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
};

$(function() {
    $('.modal').modal();

    jq('a.fuel-modal').click(function(){
        jq('#TrucksExpenses_Id').val(0);
    });

    jq('a.fuel-modal').click(function(){
        jq("tr.trcks").each(function(i, row) {
            if (i == 0){
                jq(this).removeClass('hide');
            }
            else{
                if (!jq(this).hasClass('hide')){
                    jq(this).addClass('hide');
                }
            }

            jq(this).find('td:eq(0)').text(eval(i+1) + '.');
            jq(this).find('td:eq(3) input').val('');
            jq(this).find('td:eq(4) input').val('0');
            jq(this).find('td:eq(6) input').val('0.00');
            jq(this).find('td:eq(7) input').val('0.00');
            jq(this).find('td:eq(8) input').val('0');
        });

        jq('#Notes').val("N/A");
        jq('#truck-fuel-modal').modal('open');
    });

    jq('#expense-table').on('click', 'a.link-fuel', function(){
        if (isadmin == 'false'){
            Materialize.toast('<span>You do not have permission to perform this task</span><a class="btn-flat red-text pointer">Access Denied</a>', 3000);
            return false;
        }

        jq("tr.trcks").each(function(i, row) {
            if (i == 0){
                jq(this).removeClass('hide');
            }
            else{
                if (!jq(this).hasClass('hide')){
                    jq(this).addClass('hide');
                }

                jq(this).find('td:eq(3) input').val('');
                jq(this).find('td:eq(4) input').val('0');
                jq(this).find('td:eq(6) input').val('0.00');
                jq(this).find('td:eq(7) input').val('0.00');
                jq(this).find('td:eq(8) input').val('0');
            }

            jq(this).find('td:eq(0)').text(eval(i+1) + '.');
        });

        var row = jq('#truck-fuel-table tbody tr.trcks:eq(0)');

        jq.ajax({
            dataType: "json",
            url: '/Expense/GetTrucksFuelExpense',
            data: {
                "idnt": jq(this).data('idnt')
            },
            success: function(exps) {
                jq('#DateX').val(exps.dateString);
                jq('#Notes').val(exps.description);

                row.find('td:eq(1) input').val(exps.truck.registration);
                row.find('td:eq(1) select').val(exps.truck.id);
                row.find('td:eq(2) input').val(exps.supplier.name);
                row.find('td:eq(2) select').val(exps.supplier.id);
                row.find('td:eq(3) input').val(exps.invoice);
                row.find('td:eq(4) input').val(exps.quantity);
                row.find('td:eq(5) input').val(exps.price);
                row.find('td:eq(5) input').change();
                row.find('td:eq(7) input').val(exps.vatAmount);
                row.find('td:eq(8) input').val(exps.id);
            },
            error: function(xhr, ajaxOptions, thrownError) {
                console.log(xhr.status);
                console.log(thrownError);
            },
            complete: function() {
                $('#truck-fuel-modal').modal('open');
            }
        }); 
    });

    jq('a.btn-truck-fuel-row').click(function(){
        jq("tr.trcks").each(function() {
            if (jq(this).hasClass('hide')){
                jq(this).removeClass('hide');
                return false;
            }
        });
    });

    jq('a.remove-truck-fuel-row').click(function(){
        jq(this).closest('tr').remove();

        jq("tr.trcks").each(function(i, row) {
            jq(this).find('td:eq(0)').text(eval(i+1) + '.');
        });
    });

    jq('#truck-fuel-modal .modal-footer .modal-post').click(function() {
        var err_count = 0;

        if (jq('#truck-fuel-table tr.trcks').length == 0){
            Materialize.toast('<span>No fuel purchase data rows were found</span><a class="btn-flat yellow-text" href="#!">Correct that</a>', 3000)
            return false;
        }

        jq("#truck-fuel-table tr.trcks").each(function(i, row) {
            if (jq(this).hasClass('hide'))
                return;

            if (jq(this).find('td:eq(3) input').val().trim() == '') {
                Materialize.toast('<span>Invoice number in row ' + eval(i+1) + ' cannot be blank</span><a class="btn-flat yellow-text" href="#!">Correct that</a>', 3000)
                err_count++;
                return false;
            }

            if (!eval(jq(this).find('td:eq(6) input.amts').val()) > 0) {
                Materialize.toast('<span>Invalid quantity/amount in row ' + eval(i+1) + '</span><a class="btn-flat yellow-text" href="#!">Correct that</a>', 3000)
                err_count++;
                return false;
            }

            if (!eval(jq(this).find('td:eq(7) input.vats').val()) > 0) {
                Materialize.toast('<span>Invalid purchase VAT in row ' + eval(i+1) + '</span><a class="btn-flat yellow-text" href="#!">Correct that</a>', 3000)
                err_count++;
                return false;
            }
        });

        if (err_count > 0){
            return false;
        }

        jq('#truck-fuel-form').submit();
    });

    jq('#station-expense-modal .modal-footer .modal-post').click(function() {
        if (jq('#StationExpense_DateString').val().trim() == ''){
            Materialize.toast('<span>Specify a valid Date</span><a class="btn-flat yellow-text right" href="#!">Close<a>', 3000);
            return;
        }

        if (jq('#StationExpense_Invoice').val().trim() == ''){
            Materialize.toast('<span>Specify expense Invoice Number</span><a class="btn-flat yellow-text right" href="#!">Close<a>', 3000);
            return;
        }

        if (jq('#StationExpense_Description').val().trim() == ''){
            Materialize.toast('<span>Specify the expenditure Description</span><a class="btn-flat yellow-text right" href="#!">Close<a>', 3000);
            return;
        }

        if (jq('#StationExpense_Amount').val().trim() == '' || jq('#StationExpense_Amount').val() == 0){
            Materialize.toast('<span>Specify the expense Amount</span><a class="btn-flat yellow-text right" href="#!">Close<a>', 3000);
            return;
        }

        jq('#station-expense-form').submit();
    });


    jq('#fuel-monthly-modal .modal-footer .modal-post').click(function() {
        window.location.href = "/reports/trucks/fuel/" + jq("#fuel-monthly-modal .modal-year").val(); 
    });

    jq('#fuel-vat-modal .modal-footer .modal-post').click(function() {
        var mnths = 0;
        mnths = parseInt(jq("#fuel-vat-modal .modal-months :selected").val())+1;
        
        window.location.href = "/reports/trucks/fuel-vat/" + mnths + "/" + jq("#fuel-vat-modal .modal-year").val(); 
    });

    jq('#truck-fuel-table tbody input.qnty, #truck-fuel-table tbody input.price').change(function(){
        var row = jq(this).closest('tr');
        var qty = row.find('input.qnty').val();
        var prc = row.find('input.price').val();
        var amt = qty * prc;

        row.find('input.amts').val(amt.toString().toAccounting());
    });

    jq('.get-expense a').click(function(){
        GetExpensesCore();
    });

});

function GetExpensesCore() {
    jq.ajax({
        dataType: "json",
        url: '/Expense/GetExpensesCore',
        data: {
            "start":        jq("#expenseStartDate").val(),
            "stop":         jq("#expenseStopsDate").val(),
            "filter":       jq('#expenseFilter').val()
        },
        beforeSend: function() {
            $('body').removeClass('loaded');
        },
        success: function(results) {
            var total = 0.0;

            jq('#expense-table tbody').empty();
            jq('#expense-table tfoot').empty();

            jq.each(results, function(i, expense) {
                var cls = expense.source == 1 ? "link-fuel" : "link-exps";
                var row = "<tr>";
                row += "<td>" + expense.date + "</td>";
                row += "<td>" + expense.quantity + "</td>";
                row += "<td>" + expense.account + "</td>";
                row += "<td>" + expense.details + "</td>";
                row += "<td>" + expense.delivery + "</td>";
                row += "<td>" + expense.invoice + "</td>";
                row += "<td>" + expense.description + "</td>";
                row += "<td class='right-align'>" + expense.price + "</td>";
                row += "<td class='right-align'>" + expense.amount.toString().toAccounting() + "</td>";
                row += "<td><a class='material-icons tiny-box grey-text " + cls + "'>border_color</a></td>";
                row += "</tr>";

                total += expense.amount;

                jq('#expense-table tbody').append(row);
            })

            var footr = "<tr>";
            footr += "<th colspan='8'>PAYMENT SUMMARY</td>";
            footr += "<th class='right-align'>" + total.toString().toAccounting() + "</td>";
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