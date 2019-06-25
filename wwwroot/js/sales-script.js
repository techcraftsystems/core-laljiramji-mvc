String.prototype.toAccounting = function() {
    return parseFloat(this).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
};

jq(function() {
    jq('.modal').modal();

    jq('.get-deliveries a').click(function() {
        GetDeliveries();
    });

    jq('a.add-rows').click(function(){
        jq("tr.itms").each(function() {
            if (jq(this).hasClass('hide')){
                jq(this).removeClass('hide');
                return false;
            }
        });
    });

    jq('#delivery-table tbody tr td a.blue-text').click(function(){
        jq('#expense-table tbody').empty();

        line = jq(this).data('line');
        code = jq.parseJSON(jq(this).closest('td').find('input.json-data').val());

        var count = 0;
        jq.each(code.PettyCash, function(key,value) {
            var row = "<tr class='exps'><td>" + eval(count+1) + ".</td>";
            row += "<td class='width-120px'><input class='autocomplete' type='text' value='" + value.account + "'></td>";
            row += "<td><input class='input-opts' type='text' value='" + value.description + "'></td>";
            row += "<td class='width-100px'><input type='number' class='right-text input-amts' value='" + value.amount + "'></td>";
            row += "<td><a class='red-text' style='font-size:0.7em'><i class='material-icons pointer'>delete_forever</i></a></td>";
            row += "</tr>";

            jq('#expense-table tbody').append(row);

            jq('#expense-table tbody tr:last td:eq(1) input').autocomplete({
                data: petty_accounts,
                limit: 10,
                minLength: 1
            });

            count++;
        });

        if (count == 0){
            ExpenseAddRow();
        }

        jq('#expense-modal').modal('open');
    });


    jq('#delivery-modal a.modal-post').click(function(){
        var err_count = 0;

        jq("#delivery-table tbody tr.itms").each(function(i, $row) {
            if (!jq(this).hasClass('hide')){
                if (jq(this).find('td:eq(1) input').val().trim() == '') {
                    Materialize.toast('<span>Delivery number in row ' + eval(i+1) + ' cannot be blank</span><a class="btn-flat yellow-text pointer">FIX IT</a>', 3000)
                    err_count++;
                    return false;
                }

                if (!eval(jq(this).find('td:eq(5) input').val()) > 0) {
                    Materialize.toast('<span>Invalid banking amount in row ' + eval(i+1) + '</span><a class="btn-flat yellow-text" href="#!">Correct that</a>', 3000)
                    err_count++;
                    return false;
                }
            }

        });

        if (err_count > 0){
            return false;
        }

        jq('#delivery-form').submit();
    });

    jq('input.delv-amts, input.delv-exps').on('change', function(){
        var amts = jq(this).closest('tr').find('input.delv-amts').val();
        var exps = jq(this).closest('tr').find('input.delv-exps').val();

        jq(this).closest('tr').find('input.delv-bank').val(eval(amts - exps));
    });

    jq('#expense-modal a.modal-post').click(function(){
        var err_count = 0;
        var json = '{"PettyCash":[';
        var amts = 0;

        if (jq('#expense-table tr.exps').length == 0){
            Materialize.toast('<span>No expenditure data rows found</span><a class="btn-flat yellow-text" href="#!">Correct that</a>', 3000)
            return false;
        }

        jq("#expense-table tbody tr.exps").each(function(i, $row) {
            if (jq(this).find('td:eq(1) input').val().trim() == '') {
                Materialize.toast('<span>Expense Account in row ' + eval(i+1) + ' cannot be blank</span><a class="btn-flat yellow-text" href="#!">Correct that</a>', 3000)
                err_count++;
                return false;
            }

            if (!eval(jq(this).find('td:eq(3) input').val()) > 0) {
                Materialize.toast('<span>Expense Amount in row ' + eval(i+1) + ' cannot be blank</span><a class="btn-flat yellow-text" href="#!">Correct that</a>', 3000)
                err_count++;
                return false;
            }

            json += '{"account":"' + jq(this).find('td:eq(1) input').val().trim() + '", "description":"' + jq(this).find('td:eq(2) input').val().trim() + '", "amount":"' + eval(jq(this).find('td:eq(3) input').val()) + '"},';
            amts += eval(jq(this).find('td:eq(3) input').val());
        });

        if (err_count > 0){
            return false;
        }

        json = json.slice(0,-1) + ']}';

        jq("#Entries_" + line + "__JSonExpense").val(json);
        jq("#Entries_" + line + "__Expense").val(amts);
        jq("#Entries_" + line + "__Expense").change();

        console.log(json);

        jq('#expense-modal').modal('close');
    });

    jq('#expense-modal a.add-expense').click(function(){
        ExpenseAddRow();
    });

    jq('#expense-modal tbody').on('click', 'a.red-text', function(){
        jq(this).closest('tr').remove();
        FormatRowNumbers();
    });

    jq('#expense-modal tbody').on('change', 'input.input-amts', function(){
        ExpenseCummulativeCount();
    }); 
});

function FormatRowNumbers(){
    var total = 0;

    jq("tr.exps").each(function(i, $row) {
        jq(this).find('td:eq(0)').text(eval(i+1)+'.');
        total += eval(jq(this).find('td:eq(3) input').val());
    });

    jq('#expense-modal tfoot tr').find('th.right-align').text(total.toString().toAccounting());
}

function ExpenseCummulativeCount(){
    var total = 0;

    jq("tr.exps").each(function() {
        total += eval(jq(this).find('td:eq(3) input').val());
    });

    jq('#expense-modal tfoot tr').find('th.right-align').text(total.toString().toAccounting());
}

function ExpenseAddRow(jsondata){
    count = jq('#expense-table tr.exps').length;

    var row = "<tr class='exps'><td>" + eval(count+1) + ".</td>";
    row += "<td class='width-120px'><input class='autocomplete' type='text'></td>";
    row += "<td><input class='input-opts' type='text' value='N/A'></td>";
    row += "<td class='width-100px'><input type='number' class='right-text input-amts' value='0.00'></td>";
    row += "<td><a class='red-text' style='font-size:0.7em'><i class='material-icons pointer'>delete_forever</i></a></td>";
    row += "</tr>";

    jq('#expense-table tbody').append(row);

    jq('#expense-table tbody tr:last td:eq(1) input').autocomplete({
        data: petty_accounts,
        limit: 10,
        minLength: 1
    });
}

function GetDeliveries() {
    jq.ajax({
        dataType: "json",
        url: '/Sales/GetDeliveries',
        data: {
            "start": jq('#start').val(),
            "stop": jq('#stops').val(),
            "filter":   jq('#filter').val() + ' ' + jq('#Code :selected').val()
        },
        beforeSend: function() {
            jq('body').removeClass('loaded');
        },
        success: function(results) {
            jq('#ledger-table tbody').empty();
            jq('#ledger-table tfoot').empty();

            var amts = 0;
            var exps = 0;
            var disc = 0;
            var bank = 0;

            jq.each(results, function(i, delv) {
                var row = "<tr data-idnt='" + delv.id + "'>";
                row += "<td>" + delv.dateString + "</td>";
                row += "<td>" + delv.receipt + "</td>";
                row += "<td>" + delv.type.name + "</td>";
                row += "<td>" + delv.description + "</td>";
                row += "<td><a class='blue-text' href='/core/stations/" + delv.station.code + "'>" + delv.station.code.toUpperCase() + "</td>";
                row += "<td class='right-text'>" + delv.amount.toString().toAccounting() + "</td>";
                row += "<td class='right-text'>" + delv.expense.toString().toAccounting() + "</td>";
                row += "<td class='right-text'>" + delv.discount.toString().toAccounting() + "</td>";
                row += "<td class='right-text'>" + delv.banking.toString().toAccounting() + "</td>";
                row += "<td>&nbsp;</td>";
                row += "</tr>";

                amts += delv.amount;
                exps += delv.expense;
                disc += delv.discount;
                bank += delv.banking;

                jq('#ledger-table tbody').append(row);
            })

            if (results.length == 0) {
                jq('#ledger-table tbody').append("<tr><td colspan=10>NO RECORDS FOUND</td></tr>");
            }
            else {
                var footr = "<tr><th colspan=5 class='bold-text white-text'>&nbsp; &nbsp; SUMMARY</th>";
                footr += "<th class='right-text'>" + amts.toString().toAccounting() + "</th>";
                footr += "<th class='right-text'>" + exps.toString().toAccounting() + "</th>";
                footr += "<th class='right-text'>" + disc.toString().toAccounting() + "</th>";
                footr += "<th class='right-text'>" + bank.toString().toAccounting() + "</th>";
                footr += "<th>&nbsp;</th>";
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