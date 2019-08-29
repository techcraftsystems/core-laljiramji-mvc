jq(function() {
    jq('.modal').modal();
    jq('#suppliers-table').DataTable({
        "displayLength": 25
    });
	
    jq('#add-supplier-modal a.modal-post').click(function(){
        var err_count = 0;

        if (jq('#add-supplier-modal #Supplier_Name').val().trim().length < 3){
            Materialize.toast('<span>Invalid Supplier Name. Length must be greater than 3 character</span><a class="btn-flat yellow-text pointer">FIX IT</a>', 3000)
            return false;
        }
		
        if (jq('#add-supplier-modal #Supplier_Pin').val().trim().length < 10){
            Materialize.toast('<span>Invalid supplier PIN Number</span><a class="btn-flat yellow-text pointer">FIX IT</a>', 3000)
            return false;
        }

        if (err_count > 0){
            return false;
        }

        jq('#add-supplier-form').submit();
		
		Materialize.toast('<span>Updated Successfully</span>', 5000)
		return true;
    });
});