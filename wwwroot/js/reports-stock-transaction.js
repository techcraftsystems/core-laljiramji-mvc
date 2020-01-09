  
String.prototype.toAccounting = function() {
    var str = parseFloat(this).toFixed(0).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
    if (str.charAt(0) == '-') {
        return '(' + str.substring(1, 40) + ')';
    } else {
        return str;
    }
};

$(function() {
    jq('#Station_Code, #Start, #Ended').change(function(){
        GetProductsByStation();
    });

    jq('#Product_Id').change(function(){
        GetProductsTransactions();
    });
});

function GetProductsByStation(){
    jq.ajax({
        dataType: "json",
        url: '/Reports/GetProductsByStation',
        data: {
            "code": jq('#Station_Code').val()
        },
        beforeSend: function() {
            jq('body').removeClass('loaded');
        },
        success: function(results) {
            jq('#Product_Id').find('option').remove();
            jq('#Product_Id').find('optgroup').remove();

            var opts = "";
            var grps = "";

            jq.each(results, function(i, prd) {
                if (grps !== prd.group.name){
                    if (opts != ""){
                        opts += "</optgroup>";
                    };

                    grps = prd.group.name;
                    opts += "<optgroup label='" + grps + "'>";
                }

                opts += "<option value='" + prd.value + "'>" + prd.text + "</option>";                
            });

            opts += opts == "" ? "" : "</optgroup>";

            jq('#Product_Id').append(opts);
            jq('#Product_Id').material_select();

            GetProductsTransactions();
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

function GetProductsTransactions(){
    
    jq.ajax({
        dataType: "json",
        url: '/Reports/GetProductsTransactions',
        data: {
            "code": jq('#Station_Code').val(),
            "item": jq('#Product_Id').val(),
            "from": jq('#Start').val(),
            "ends": jq('#Ended').val()
        },
        beforeSend: function() {
            jq('body').removeClass('loaded');
        },
        success: function(results) {
            jq('#ledger-table tbody').empty();
            jq('#ledger-table tfoot').empty();

            var running = 0.0;

            jq.each(results, function(i, itm) {
                running += itm.in - itm.out;
                var row = "<tr>";
                row += "<td>" + eval(i+1) + "</td>";
                row += "<td>" + itm.date + "</td>";
                row += "<td>" + itm.product.name + "</td>";
                row += "<td>" + itm.product.category + "</td>";
                row += "<td>" + itm.description + "</td>";
                row += "<td class='center-text'>" + itm.reference + "</td>";
                row += "<td class='center-text'>" + itm.in.toString().toAccounting() + "</td>";
                row += "<td class='center-text'>" + itm.out.toString().toAccounting() + "</td>";
                row += "<td class='center-text'>" + running.toString().toAccounting() + "</td>";

                jq('#ledger-table tbody').append(row);
            });

            if (results.length == 0){
                jq('#ledger-table tbody').append("<tr><td colspan='9' class='center-text'>NO RECORDS FOUNDS</td></tr>");
            }

            var ftr = "<tr><th>@nbsp;</th>";
            ftr += "<th colspan='7'>CLOSING BALANCE</th>";
            ftr += "<th class='center-text'>" + running.toString().toAccounting() + "</th>";
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