/*
* Modals - Advanced UI
*/
$(function() {
    $('.modal').modal();
    $('#modal21').modal('open');
    $('#modal21').modal('close');

    jq('.col-1 ul ')

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

});