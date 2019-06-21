
jq(function() {
    jq('.modal').modal();

    var route = '';

    jq('div.multiple-trigger').click(function(){
        route = jq(this).data('trigger');
    });

    jq('div.redirect-trigger').click(function(){
        window.location.href = jq(this).data('route');
    });

    jq('#modal11 .modal-footer .modal-post').click(function() {
        var month = jq('#modal11 .modal-mnth :selected').val();
        var year =  jq('#modal11 .modal-year').val();

        month = eval(month)+1;

        window.location.href = route + month + "/" + year; 
    });

});