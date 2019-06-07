/*
* Modals - Advanced UI
*/
$(function() {
    $('.modal').modal();
    $('#modal21').modal('open');
    $('#modal21').modal('close');

    jq('div.multiple-trigger').click(function(){
        route = jq(this).data('trigger');
    });

    jq('div.redirect-trigger').click(function(){
        window.location.href = jq(this).data('route');
    });

    jq('#modal21 .modal-footer .modal-post').click(function() {
        var station =   jq('#modal21 .modal-station :selected').val();
        var year =      jq('#modal21 .modal-year').val();
        var type =      jq('#modal21 .modal-types :selected').val();

        window.location.href = "/reports/customers/summary/" + station + "/" + year + "/" + type; 
    });

    jq('#modal22 .modal-footer .modal-post').click(function() {
        var station =   jq('#modal22 .modal-station :selected').val();
        var year =      jq('#modal22 .modal-year').val();

        window.location.href = "/reports/customers/balances/" + station + "/" + year; 
    });

    jq('#modal31 .modal-footer .modal-post').click(function() {
        var month =     jq('#modal31 .modal-months :selected').val();
        var year =      jq('#modal32 .modal-year').val();

        month = eval(month)+1;

        window.location.href = route + "/" + month + "/" + year; 
    });

    jq('#modal32 .modal-footer .modal-post').click(function() {
        var station =   jq('#modal32 .modal-station :selected').val();
        var month =     jq('#modal32 .modal-months :selected').val();
        var year =      jq('#modal32 .modal-year').val();

        month = eval(month)+1;

        window.location.href = route + station + "/" + month + "/" + year; 
    });

    jq('#modal61 .modal-footer .modal-post').click(function() {
        var station =   jq('#modal61 .modal-station :selected').val();
        var month =     jq('#modal61 .modal-months :selected').val();
        var year =      jq('#modal61 .modal-year').val();
        var type =      jq('#modal61 .modal-type :selected').val();
        var catg =      jq('#modal61 .modal-catg :selected').val();

        month = eval(month)+1;

        window.location.href = route + type + "/" + station + "/" + catg + "/" + month + "/" + year; 
    });

});