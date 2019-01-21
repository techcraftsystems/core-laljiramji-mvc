$(function() {
    $('.modal').modal();

    jq('#truck-fuel-modal .modal-footer .modal-post').click(function() {
        if (jq('#TrucksExpenses_DateString').val().trim() == ''){
            Materialize.toast('<span>Specify a valid Date</span><a class="btn-flat yellow-text right" href="#!">Close<a>', 3000);
            return;
        }

        if (jq('#TrucksExpenses_Invoice').val().trim() == ''){
            Materialize.toast('<span>Specify Invoice Number</span><a class="btn-flat yellow-text right" href="#!">Close<a>', 3000);
            return;
        }

        if (jq('#TrucksExpenses_Quantity').val().trim() == '' || jq('#TrucksExpenses_Quantity').val() == 0){
            Materialize.toast('<span>Specify the Quantity Fueled</span><a class="btn-flat yellow-text right" href="#!">Close<a>', 3000);
            return;
        }

        jq('#truck-fuel-form').submit();
    });

    jq('#fuel-monthly-modal .modal-footer .modal-post').click(function() {
        window.location.href = "/reports/trucks/fuel/" + jq("#fuel-monthly-modal .modal-year").val(); 
    });

    jq('#fuel-vat-modal .modal-footer .modal-post').click(function() {
        var mnths = 0;
        mnths = parseInt(jq("#fuel-vat-modal .modal-months :selected").val())+1;
        
        window.location.href = "/reports/trucks/fuel-vat/" + mnths + "/" + jq("#fuel-vat-modal .modal-year").val(); 
    });

    jq('#truck-fuel-table tbody input.qnty').change(function(){
        var row = jq(this).closest('tr');
        var qty = jq(this).val();
        var prc = row.find('input.price').val();
        var amt = qty * prc;

        row.find('input.amts').val(amt.toString().toAccounting());
    });

    jq('.get-expense a').click(function(){
        GetExpensesCore();
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
                row += "<td><a class='material-icons tiny-box grey-text'>border_color</a></td>";
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