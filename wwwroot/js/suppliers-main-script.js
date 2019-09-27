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
    jq('.modal').modal();

    jq('ul .invs').click(function(){
        if(jq("#invoice-table").data('loaded') == 0){
            GetSupplierExpenses();
            jq("#invoice-table").data('loaded', 1);
        }
    });

    jq('ul li.pymt').click(function(){
        if(jq("#payment-table").data('loaded') == 0){
            GetSuppliersPayment();
            jq("#payment-table").data('loaded', 1);
        }
    });

    jq('ul li.note').click(function(){
        if(jq("#credits-table").data('loaded') == 0){
            GetSuppliersCredits();
            jq("#credits-table").data('loaded', 1);
        }
    });

    jq('div.get-invoices a').click(function(){
        GetSupplierExpenses();
    });

    jq('div.get-payments a').click(function(){
        GetSuppliersPayment();
    });

    jq('div.get-credits a').click(function(){
        GetSuppliersCredits();
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
            jq(this).find('td:eq(4) input').val('');
            jq(this).find('td:eq(5) input').val('0.00');
            jq(this).find('td:eq(6) input').val('N/A');
            jq(this).find('td:eq(7) input.idnt-data').val(0);
        });

        jq('#payment-modal').modal('open');
    });
	
    jq('a.make-withholding').click(function(){
        jq("#withholding-modal-table tr.whts").each(function(i, row) {
            if (i == 0){
                jq(this).removeClass('hide');
            }
            else{
                if (!jq(this).hasClass('hide')){
                    jq(this).addClass('hide');
                }
            }

            jq(this).find('td:eq(1) input').val('KRAWHTWON0');
            jq(this).find('td:eq(2) input').val('');
            jq(this).find('td:eq(3) input').val('');
            jq(this).find('td:eq(5) input').val('0.00');
            jq(this).find('td:eq(6) input').val('N/A');
            jq(this).find('td:eq(7) input.idnt-data').val(0);
        });

        jq('#withholding-modal').modal('open');
    });

    jq('a.make-invoice').click(function(){
        jq("#invoice-modal-table tr.invs").each(function(i, row) {
            if (i == 0){
                jq(this).removeClass('hide');
            }
            else{
                if (!jq(this).hasClass('hide')){
                    jq(this).addClass('hide');
                }
            }

            jq(this).find('td:eq(1) input').val('');
            jq(this).find('td:eq(4) input').val('N/A');
            jq(this).find('td:eq(5) input').val('0.00');
            jq(this).find('td:eq(6) input').val('0.00');
            jq(this).find('td:eq(7) input').val('0.00');
            jq(this).find('td:eq(8) input.idnt-data').val(0);
        });

        jq('#invoice-modal').modal('open');
    });

    jq('a.credit-note').click(function(){
        jq("#credits-modal-table tr.notes").each(function(i, row) {
            if (i == 0){
                jq(this).removeClass('hide');
            }
            else{
                if (!jq(this).hasClass('hide')){
                    jq(this).addClass('hide');
                }
            }

            jq(this).find('td:eq(1) input').val('');
            jq(this).find('td:eq(4) input').val('0.00');
            jq(this).find('td:eq(5) input').val('N/A');
            jq(this).find('td:eq(6) input.idnt-data').val(0);
        });

        jq('#credits-modal').modal('open');
    });
	
    jq('a.add-rows').click(function(){
		var tbody = jq(this).parent().find('table tbody tr');
		
        tbody.each(function() {
            if (jq(this).hasClass('hide')){
                jq(this).removeClass('hide');
                return false;
            }
        });
    });

    jq('a.remove-row').click(function() {
		var table = jq(this).closest('table');
        jq(this).closest('tr').remove();
        
		table.find('tbody tr').each(function(i, row) {
            jq(this).find('td:eq(0)').text(eval(i+1) + '.');
        });
    });

    jq('#payment-modal a.modal-post').click(function(){
        var err_count = 0;

        jq("#payment-modal-table tbody tr.itms").each(function(i, $row) {
            if (!jq(this).hasClass('hide')){
                if (jq(this).find('td:eq(2) input').val().trim() == '') {
                    Materialize.toast('<span>Cheque number in row ' + eval(i+1) + ' cannot be blank</span><a class="btn-flat yellow-text pointer">FIX IT</a>', 3000)
                    err_count++;
                    return false;
                }

                if (!eval(jq(this).find('td:eq(5) input').val()) > 0) {
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
	
    jq('#withholding-modal a.modal-post').click(function(){
        var err_count = 0;

        jq("#withholding-modal-table tbody tr.whts").each(function(i, $row) {
            if (!jq(this).hasClass('hide')){
                if (jq(this).find('td:eq(3) input').val().trim() == '') {
                    Materialize.toast('<span>Cheque number in row ' + eval(i+1) + ' cannot be blank</span><a class="btn-flat yellow-text pointer">FIX IT</a>', 3000)
                    err_count++;
                    return false;
                }

                if (!eval(jq(this).find('td:eq(5) input').val()) > 0) {
                    Materialize.toast('<span>Invalid withholding amount in row ' + eval(i+1) + '</span><a class="btn-flat yellow-text" href="#!">Correct that</a>', 3000)
                    err_count++;
                    return false;
                }
            }
        });

        if (err_count > 0){
            return false;
        }

        jq('#withholding-form').submit();
    });

    jq('#invoice-modal a.modal-post').click(function(){
        var err_count = 0;

        jq("#invoice-modal-table tbody tr.invs").each(function(i, $row) {
            if (!jq(this).hasClass('hide')){
                if (jq(this).find('td:eq(1) input').val().trim() == '') {
                    Materialize.toast('<span>Invoice number in row ' + eval(i+1) + ' cannot be blank</span><a class="btn-flat yellow-text pointer">FIX IT</a>', 3000)
                    err_count++;
                    return false;
                }

                if (!eval(jq(this).find('td:eq(5) input').val()) > 0) {
                    Materialize.toast('<span>Invalid invoice amount in row ' + eval(i+1) + '</span><a class="btn-flat yellow-text" href="#!">Correct that</a>', 3000)
                    err_count++;
                    return false;
                }
            }

        });

        if (err_count > 0){
            return false;
        }

        jq('#invoice-form').submit();
    });

    jq('#credits-modal a.modal-post').click(function(){
        var err_count = 0;

        jq("#credits-modal-table tbody tr.notes").each(function(i, $row) {
            if (!jq(this).hasClass('hide')){
                if (jq(this).find('td:eq(1) input').val().trim() == '') {
                    Materialize.toast('<span>Receipt number in row ' + eval(i+1) + ' cannot be blank</span><a class="btn-flat yellow-text pointer">FIX IT</a>', 3000)
                    err_count++;
                    return false;
                }

                if (!eval(jq(this).find('td:eq(4) input').val()) > 0) {
                    Materialize.toast('<span>Invalid credits amount in row ' + eval(i+1) + '</span><a class="btn-flat yellow-text" href="#!">Correct that</a>', 3000)
                    err_count++;
                    return false;
                }
            }

        });

        if (err_count > 0){
            return false;
        }

        jq('#credits-form').submit();
    });
	
    jq('#edit-modal a.modal-post').click(function(){
        var err_count = 0;

        if (jq('#edit-modal #Supplier_Name').val().trim().length < 3){
            Materialize.toast('<span>Invalid Supplier Name. Length must be greater than 3 character</span><a class="btn-flat yellow-text pointer">FIX IT</a>', 3000)
            return false;
        }
		
        if (jq('#edit-modal #Supplier_Pin').val().trim().length < 10){
            Materialize.toast('<span>Invalid supplier PIN Number</span><a class="btn-flat yellow-text pointer">FIX IT</a>', 3000)
            return false;
        }

        if (err_count > 0){
            return false;
        }

        jq('#edit-form').submit();		
		Materialize.toast('<span>Updated Successfully</span>', 3000);
    });

    jq('#statement-modal a.modal-post').click(function(){
        window.location.href = "/core/suppliers/statement?uuid=" + xUuid + "&from=" + jq('#statement-modal input.modal-start').val() + "&to=" + jq('#statement-modal input.modal-stop').val(); 
    });

    jq('#payment-table tbody').on('click', 'i.edit-payment', function(){
        if (isadmin == 'false'){
            Materialize.toast('<span>You do not have permission to perform this task</span><a class="btn-flat red-text pointer">Access Denied</a>', 3000);
            return false;
        }
		
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
            jq(this).find('td:eq(4) input').val('');
            jq(this).find('td:eq(5) input').val('0.00');
            jq(this).find('td:eq(6) input').val('N/A');
            jq(this).find('td:eq(7) input.idnt-data').val(0);
        });		

        var row = jq('#payment-modal-table tbody tr:eq(0)');

        jq.ajax({
            dataType: "json",
            url: '/Suppliers/GetSuppliersPayment',
            data: {
                "idnt": jq(this).data('idnt')
            },
            success: function(pymt) {
                jq('#Date').val(pymt.dateString);

                row.find('td:eq(1) input').val(pymt.receipt);
                row.find('td:eq(2) input').val(pymt.cheque);
                row.find('td:eq(3) select').val(pymt.bank.id);
                row.find('td:eq(3) input').val(pymt.bank.name);
                row.find('td:eq(4) input').val(pymt.invoices);
                row.find('td:eq(5) input').val(pymt.amount);
                row.find('td:eq(6) input').val(pymt.description);
                row.find('td:eq(7) input.idnt-data').val(pymt.id);
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
	
    jq('#payment-table tbody').on('click', 'i.edit-withholding', function(){
        if (isadmin == 'false'){
            Materialize.toast('<span>You do not have permission to perform this task</span><a class="btn-flat red-text pointer">Access Denied</a>', 3000);
            return false;
        }

        jq("#withholding-modal-table tr.whts").each(function(i, row) {
            if (i == 0){
                jq(this).removeClass('hide');
            }
            else{
                if (!jq(this).hasClass('hide')){
                    jq(this).addClass('hide');
                }

	            jq(this).find('td:eq(1) input').val('KRAWHTWON0');
	            jq(this).find('td:eq(2) input').val('');
	            jq(this).find('td:eq(3) input').val('');
	            jq(this).find('td:eq(5) input').val('0.00');
	            jq(this).find('td:eq(6) input').val('N/A');
	            jq(this).find('td:eq(7) input.idnt-data').val(0);
            }
        });

        var row = jq('#withholding-modal-table tbody tr:eq(0)');

        jq.ajax({
            dataType: "json",
            url: '/Suppliers/GetSuppliersWithholding',
            data: {
                "idnt": jq(this).data('idnt')
            },
            success: function(pymt) {
                jq('#Date').val(pymt.dateString);

                row.find('td:eq(1) input').val(pymt.receipt);
                row.find('td:eq(2) input').val(pymt.invoice);
                row.find('td:eq(3) input').val(pymt.cheque);
                row.find('td:eq(4) select').val(pymt.bank.id);
                row.find('td:eq(4) input').val(pymt.bank.name);
                row.find('td:eq(5) input').val(pymt.amount);
                row.find('td:eq(6) input').val(pymt.description);
                row.find('td:eq(7) input.idnt-data').val(pymt.id);
            },
            error: function(xhr, ajaxOptions, thrownError) {
                console.log(xhr.status);
                console.log(thrownError);
            },
            complete: function() {
                jq('#withholding-modal').modal('open');
            }
        });
    });

    jq('#invoice-table tbody').on('click', 'i.edit-invoice', function(){
        if (isadmin == 'false'){
            Materialize.toast('<span>You do not have permission to perform this task</span><a class="btn-flat red-text pointer">Access Denied</a>', 3000);
            return false;
        }

        jq("tr.invs").each(function(i, row) {
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

        var row = jq('#invoice-modal-table tbody tr:eq(0)');

        jq.ajax({
            dataType: "json",
            url: '/Suppliers/GetSuppliersInvoice',
            data: {
                "idnt": jq(this).data('idnt')
            },
            success: function(invs) {
                jq('#Date').val(invs.dateString);

                row.find('td:eq(1) input').val(invs.invoice);
                row.find('td:eq(2) select').val(invs.category.id);
                row.find('td:eq(2) input').val(invs.category.name);
                row.find('td:eq(3) select').val(invs.station.id);
                row.find('td:eq(3) input').val(invs.station.name);
                row.find('td:eq(4) input').val(invs.description);
                row.find('td:eq(5) input').val(invs.amount);
                row.find('td:eq(6) input').val(invs.vatAmount);
                row.find('td:eq(7) input').val(invs.zerorated);
                row.find('td:eq(8) input.idnt-data').val(invs.id);
            },
            error: function(xhr, ajaxOptions, thrownError) {
                console.log(xhr.status);
                console.log(thrownError);
            },
            complete: function() {
                jq('#invoice-modal').modal('open');
            }
        });
    });

    jq('#credits-table tbody').on('click', 'i.edit-credits', function(){
        if (isadmin == 'false'){
            Materialize.toast('<span>You do not have permission to perform this task</span><a class="btn-flat red-text pointer">Access Denied</a>', 3000);
            return false;
        }

        jq("tr.notes").each(function(i, row) {
            if (i == 0){
                jq(this).removeClass('hide');
            }
            else{
                if (!jq(this).hasClass('hide')){
                    jq(this).addClass('hide');
                }

                jq(this).find('td:eq(1) input').val('');
                jq(this).find('td:eq(4) input').val('0.00');
                jq(this).find('td:eq(5) input').val('N/A');
                jq(this).find('td:eq(6) input.idnt-data').val(0);
            }
        });

        var row = jq('#credits-modal-table tbody tr:eq(0)');

        jq.ajax({
            dataType: "json",
            url: '/Suppliers/GetSuppliersCredit',
            data: {
                "idnt": jq(this).data('idnt')
            },
            success: function(notes) {
                jq('#Date').val(notes.dateString);

                row.find('td:eq(1) input').val(notes.receipt);
                row.find('td:eq(2) select').val(notes.type.id);
                row.find('td:eq(2) input').val(notes.type.name);
                row.find('td:eq(3) select').val(notes.station.id);
                row.find('td:eq(3) input').val(notes.station.name);
                row.find('td:eq(4) input').val(notes.amount);
                row.find('td:eq(5) input').val(notes.description);
                row.find('td:eq(6) input.idnt-data').val(notes.id);
            },
            error: function(xhr, ajaxOptions, thrownError) {
                console.log(xhr.status);
                console.log(thrownError);
            },
            complete: function() {
                jq('#credits-modal').modal('open');
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
            url: '/Suppliers/GetSuppliersPayment',
            data: {
                "idnt": line
            },
            success: function(delv) {
                jq('#delete-modal-field').html('Confirm deleting receipt <code class="language-css"> ' + delv.receipt + '</code> cheque <code class="language-css">' + delv.cheque + '</code> dated <code class="language-css">' + delv.dateString + '</code> for <code class="language-css">Kes ' + delv.amount.toString().toAccounting() + '</code>?');
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
	
    jq('#payment-table tbody').on('click', 'i.delete-withholding', function(){
        if (isadmin == 'false'){
            Materialize.toast('<span>You do not have permission to perform this task</span><a class="btn-flat red-text pointer">Access Denied</a>', 3000);
            return false;
        }

        line = jq(this).data('idnt');

        jq.ajax({
            dataType: "json",
            url: '/Suppliers/GetSuppliersWithholding',
            data: {
                "idnt": line
            },
            success: function(delv) {
                jq('#delete-withholding-modal p.modal-field').html('Confirm deleting WHT <code class="language-css"> ' + delv.receipt + '</code> dated <code class="language-css">' + delv.dateString + '</code> for <code class="language-css">Kes ' + delv.amount.toString().toAccounting() + '</code>?');
            },
            error: function(xhr, ajaxOptions, thrownError) {
                console.log(xhr.status);
                console.log(thrownError);
            },
            complete: function() {
                jq('#delete-withholding-modal').modal('open');
            }
        });
    });
	

    jq('#invoice-table tbody').on('click', 'i.delete-invoice', function(){
        if (isadmin == 'false'){
            Materialize.toast('<span>You do not have permission to perform this task</span><a class="btn-flat red-text pointer">Access Denied</a>', 3000);
            return false;
        }

        line = jq(this).data('idnt');

        jq.ajax({
            dataType: "json",
            url: '/Suppliers/GetSuppliersInvoice',
            data: {
                "idnt": line
            },
            success: function(invs) {
                jq('#delete-invoice-modal .delete-modal-field').html('Confirm deleting invoice <code class="language-css"> ' + invs.invoice + '</code> dated <code class="language-css">' + invs.dateString + '</code> for <code class="language-css">Kes ' + invs.amount.toString().toAccounting() + '</code>?');
            },
            error: function(xhr, ajaxOptions, thrownError) {
                console.log(xhr.status);
                console.log(thrownError);
            },
            complete: function() {
                jq('#delete-invoice-modal').modal('open');
            }
        });
    });

    jq('#credits-table tbody').on('click', 'i.delete-credits', function(){
        if (isadmin == 'false'){
            Materialize.toast('<span>You do not have permission to perform this task</span><a class="btn-flat red-text pointer">Access Denied</a>', 3000);
            return false;
        }

        line = jq(this).data('idnt');

        jq.ajax({
            dataType: "json",
            url: '/Suppliers/GetSuppliersCredit',
            data: {
                "idnt": line
            },
            success: function(notes) {
                jq('#delete-credits-modal .delete-modal-field').html('Confirm deleting ' + notes.type.name + ' <code class="language-css"> ' + notes.receipt + '</code> dated <code class="language-css">' + notes.dateString + '</code> for <code class="language-css">Kes ' + notes.amount.toString().toAccounting() + '</code>?');
            },
            error: function(xhr, ajaxOptions, thrownError) {
                console.log(xhr.status);
                console.log(thrownError);
            },
            complete: function() {
                jq('#delete-credits-modal').modal('open');
            }
        });
    });

    jq('#delete-payment-modal a.modal-post').click(function(){
        jq('#delete-payment-modal').modal('close');
        jq.ajax({
            type: "post",
            url: '/Suppliers/DeleteSuppliersPayment',
            data: {
                "idnt": line,
                "supp": xSupp
            },
            success: function(delv) {
                Materialize.toast('<span>Successfully deleted supplier payment</span><a class="btn-flat yellow-text pointer">Task Complete</a>', 3000);
                setTimeout(function(){
					GetSuppliersPayment();
					UpdateSupplierBalance();
                }, 3000);
            },
            error: function(xhr, ajaxOptions, thrownError) {
                console.log(xhr.status);
                console.log(thrownError);
                Materialize.toast('<span>Error ' + xhr.status + '. ' + thrownError + '</span><a class="btn-flat red-text pointer">Delete Failed</a>', 3000);
            }
        });
    });
	
    jq('#delete-withholding-modal a.modal-post').click(function(){
        jq('#delete-withholding-modal').modal('close');
        jq.ajax({
            type: "post",
            url: '/Suppliers/DeleteSuppliersWithholding',
            data: {
                "idnt": line,
                "supp": xSupp
            },
            success: function(delv) {
                Materialize.toast('<span>Successfully deleted witholding payment</span><a class="btn-flat yellow-text pointer">Task Complete</a>', 3000);
                setTimeout(function(){
					GetSuppliersPayment();
					UpdateSupplierBalance();
                }, 3000);
            },
            error: function(xhr, ajaxOptions, thrownError) {
                console.log(xhr.status);
                console.log(thrownError);
                Materialize.toast('<span>Error ' + xhr.status + '. ' + thrownError + '</span><a class="btn-flat red-text pointer">Delete Failed</a>', 3000);
            }
        });
    });

    jq('#delete-invoice-modal a.modal-post').click(function(){
        jq('#delete-invoice-modal').modal('close');
        jq.ajax({
            type: "post",
            url: '/Suppliers/DeleteSuppliersInvoice',
            data: {
                "idnt": line,
                "supp": xSupp
            },
            success: function(delv) {
                Materialize.toast('<span>Successfully deleted supplier payment</span><a class="btn-flat yellow-text pointer">Task Complete</a>', 3000);
                setTimeout(function(){
					GetSuppliersPayment();
					UpdateSupplierBalance();
                }, 3000);
            },
            error: function(xhr, ajaxOptions, thrownError) {
                console.log(xhr.status);
                console.log(thrownError);
                Materialize.toast('<span>Error ' + xhr.status + '. ' + thrownError + '</span><a class="btn-flat red-text pointer">Delete Failed</a>', 3000);
            }
        });
    });
	
    jq('#delete-credits-modal a.modal-post').click(function(){
        jq('#delete-credits-modal').modal('close');
        jq.ajax({
            type: "post",
            url: '/Suppliers/DeleteSuppliersCredit',
            data: {
                "idnt": line,
                "supp": xSupp
            },
            success: function(delv) {
                Materialize.toast('<span>Successfully deleted supplier credits</span><a class="btn-flat yellow-text pointer">Task Complete</a>', 3000);

                setTimeout(function(){
					GetSuppliersPayment();
					UpdateSupplierBalance();
                }, 3000);
            },
            error: function(xhr, ajaxOptions, thrownError) {
                console.log(xhr.status);
                console.log(thrownError);
                Materialize.toast('<span>Error ' + xhr.status + '. ' + thrownError + '</span><a class="btn-flat red-text pointer">Delete Failed</a>', 3000);
            }
        });
    });
	
	jq('a.payment-ledger').click(function(){
		url = jq(this).data('url');
		jq('#reports-modal').modal('open');
	});
	
	jq('#reports-modal a.modal-post').click(function(){
		var month = eval(jq('#reports-modal select.modal-month').val()) + 1;
		window.location.href = url + month + '/' + jq('#reports-modal input.modal-year').val();
	});
});

function UpdateSupplierBalance() {
    jq.ajax({
        dataType: "json",
        url: '/Suppliers/GetSupplierBalance',
        data: {
            "idnt": xSupp
        },
        success: function(balance) {
			jq('span.bals').html('Ksh ' + balance.toString().toAccounting());
		}
    });
}

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
                row += "<td><i class='material-icons blue-text edit-invoice left pointer' style='font-size:1em;' data-idnt='" + entry.id + "'>border_color</i><i class='material-icons red-text delete-invoice right pointer' style='font-size:1.2em;' data-idnt='" + entry.id + "'>delete_forever</i></td>";
                row += "</tr>";

                jq('#invoice-table tbody').append(row);
            })

            if(results.length == 0){
                jq('#invoice-table tbody').append("<tr><td colspan='6'>NO INVOICES FOUND</td></tr>");
            }

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

function GetSuppliersPayment(){
    jq.ajax({
        dataType: "json",
        url: '/Suppliers/GetSuppliersPayments',
        data: {
            "supp":     xSupp,
            "start":    jq('#paymentStartDate').val(),
            "stop":     jq('#paymentStopsDate').val(),
            "filter":   jq('#paymentFilter').val()
        },
        beforeSend: function() {
            jq('body').removeClass('loaded');
        },
        success: function(results) {
            jq('#payment-table tbody').empty();
            jq('#payment-table tfoot').empty();

            var cumm = 0.0;

            jq.each(results, function(i, entry) {
                cumm += entry.amount;

                var row = "<tr data-idnt='" + entry.id + "'>";
                row += "<td>" + entry.dateString + "</td>";
                row += "<td>" + entry.receipt + "</td>";
                row += "<td>" + entry.cheque + "</td>";
                row += "<td>" + entry.bank.name + "</td>";
                row += "<td>" + entry.invoices + "</td>";
                row += "<td>" + (entry.type == 1 ? "WITHHOLDING TAX" : entry.description) + "</td>";
                row += "<td class='right-text bold-text'>" + entry.amount.toString().toAccounting() + "</td>";
                row += "<td><i class='material-icons blue-text " + (entry.type == 0 ? "edit-payment" : "edit-withholding") + " left pointer' style='font-size:1em;' data-idnt='" + entry.id + "'>border_color</i><i class='material-icons red-text " + (entry.type == 0 ? "delete-payment" : "delete-withholding") + " right pointer' style='font-size:1.2em;' data-idnt='" + entry.id + "'>delete_forever</i></td>";
                row += "</tr>";

                jq('#payment-table tbody').append(row);
            })

            if(results.length == 0){
                jq('#payment-table tbody').append("<tr><td colspan='8'>NO PAYMENTS FOUND</td></tr>");
            }

            var footr = "<tr>";
            footr += "<td class='bold-text' colspan='6'>&nbsp;SUMMARY</td>";
            footr += "<td class='bold-text right'>" + cumm.toString().toAccounting() + "</td>";
            footr += "<td>&nbsp;</td>";
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

function GetSuppliersCredits(){
    jq.ajax({
        dataType: "json",
        url: '/Suppliers/GetSuppliersCredits',
        data: {
            "supp":     xSupp,
            "start":    jq('#creditsStartDate').val(),
            "stop":     jq('#creditsStopsDate').val(),
            "filter":   jq('#creditsFilter').val()
        },
        beforeSend: function() {
            jq('body').removeClass('loaded');
        },
        success: function(results) {
            jq('#credits-table tbody').empty();
            jq('#credits-table tfoot').empty();

            var cumm = 0.0;

            jq.each(results, function(i, credits) {
                cumm += credits.amount;

                var row = "<tr data-idnt='" + credits.id + "'>";
                row += "<td>" + credits.dateString + "</td>";
                row += "<td>" + credits.type.name.toUpperCase() + "</td>";
                row += "<td>" + credits.receipt + "</td>";
                row += "<td>" + credits.station.name + "</td>";
                row += "<td>" + credits.description + "</td>";
                row += "<td class='bold-text right-text'>" + credits.amount.toString().toAccounting() + "</td>";
                row += "<td><i class='material-icons blue-text edit-credits left pointer' style='font-size:1em;' data-idnt='" + credits.id + "'>border_color</i><i class='material-icons red-text delete-credits right pointer' style='font-size:1.2em;' data-idnt='" + credits.id + "'>delete_forever</i></td>";
                row += "</tr>";

                jq('#credits-table tbody').append(row);
            })

            if(results.length == 0){
                jq('#credits-table tbody').append("<tr><td colspan='7'>NO CREDITS FOUND</td></tr>");
            }

            var footr = "<tr>";
            footr += "<td class='bold-text' colspan='5'>&nbsp;SUMMARY</td>";
            footr += "<td class='bold-text right-text'>" + cumm.toString().toAccounting() + "</td>";
            footr += "<td>&nbsp;</td>";
            footr += "</tr>";

            jq('#credits-table tfoot').append(footr);
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