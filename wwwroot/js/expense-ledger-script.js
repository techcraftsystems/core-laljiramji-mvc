$(function() {
    jq('a.btn-expense').click(function(){
        GetStationsExpenses();
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

function GetStationsExpenses() {
    var stations = jq('#summarySel').closest('div').find('select').val();

    jq.ajax({
        dataType: "json",
        url: '/Expense/GetStationsExpenses',
        data: {
            "start":        jq("#startDate").val(),
            "stop":         jq("#stopsDate").val(),
            "filter":       jq('#Filter').val(),
            "stations":     stations.toString(),
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
                row += "<td>" + expense.dateString + "</td>";
                row += "<td>" + expense.invoice + "</td>";
                row += "<td>" + expense.category.name + "</td>";
                row += "<td>" + expense.supplier.name + "</td>";
                row += "<td>" + expense.station.name + "</td>";
                row += "<td>" + expense.description + "</td>";
                row += "<td class='right-align'>" + expense.amount.toString().toAccounting() + "</td>";
                row += "<td><a class='material-icons tiny-box grey-text'>border_color</a></td>";
                row += "</tr>";

                total += expense.amount;

                jq('#expense-table tbody').append(row);
            })

            var footr = "<tr>";
            footr += "<th colspan='6' style='padding-left:20px'>PAYMENT SUMMARY</th>";
            footr += "<th class='right-align'>" + total.toString().toAccounting() + "</th>";
            footr += "<th>&nbsp;</td>";
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