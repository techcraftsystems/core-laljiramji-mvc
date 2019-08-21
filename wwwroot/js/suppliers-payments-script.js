jq(function() {
	jq('.modal').modal();
	
	jq('a.payment-ledger').click(function(){
		url = jq(this).data('url');
		jq('#reports-modal').modal('open');
	});
	
	jq('#reports-modal a.modal-post').click(function(){
		var month = eval(jq('#reports-modal select.modal-month').val()) + 1;
		window.location.href = url + month + '/' + jq('#reports-modal input.modal-year').val();
	});
});