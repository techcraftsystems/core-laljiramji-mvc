jq(function() {
    jq('.modal').modal();

    if (message != '') {
        Materialize.toast('<span>' + message + '</span><a class="btn-flat yellow-text" href="#!">Try Again<a>', 3000)
    }

    jq('#change-pw-modal a.modal-post').click(function(){
        if (jq('#User_Password').val().trim() == ''){
            Materialize.toast('<span>Specify the old password</span><a class="btn-flat yellow-text" href="#!">Try Again<a>', 3000);
            return;
        }

        if (jq('#Password').val() != jq('#Confirm').val()){
            Materialize.toast('<span>New Password do not Match</span><a class="btn-flat yellow-text" href="#!">Try Again<a>', 3000);
            return;
        }

        if (jq('#User_Password').val() == jq('#Password').val()){
            Materialize.toast('<span>New Password cannot be the same as the old password</span><a class="btn-flat yellow-text" href="#!">Try Again<a>', 3000);
            return;
        }

        var pass = jq('#Password').val();

        var strength = 1;
        var arr = [/.{5,}/, /[a-z]+/, /[0-9]+/, /[A-Z]+/];
        jQuery.map(arr, function(regexp) {
          if(pass.match(regexp))
             strength++;
        });

        if (strength < 4){
            Materialize.toast('<span>Specified password is weak and can be easily hacked</span><a class="btn-flat yellow-text" href="#!">Try Again<a>', 3000);
            return;
        }

        jq("form").submit();
    });
});
