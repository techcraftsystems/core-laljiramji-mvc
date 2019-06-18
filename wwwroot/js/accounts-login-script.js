jq(function() {
    jq('.modal').modal();
    jq('#Username').blur(function() {
        var un = jq(this).val();
        var dv = jq(this).closest('div').find('div');
        if (un.length == 0) {
            dv.html("Enter a username");
            jq(this).addClass('error');
        }
        else if (un.length < 4) {
            dv.html("Enter at least 4 characters");
            jq(this).addClass('error');
        }
        else {
            dv.html("");
            jq(this).removeClass('error');
        }
    });

    //Password Blurred
    jq('#Password').blur(function() {
        var pw = jq(this).val();
        var dv = jq(this).closest('div').find('div');
        if (pw.length == 0) {
            dv.html("Enter a password");
            jq(this).addClass('error');
        }
        else if (pw.length < 4) {
            dv.html("Enter at least 4 characters");
            jq(this).addClass('error');
        }
        else {
            dv.html("");
            jq(this).removeClass('error');
        }
    });

    //Validate Form before Submit
    jq('form a').on('click', function(e) {
        jq('#Username').blur();
        jq('#Password').blur();

        var un = jq('#User_Username');
        var pw = jq('#User_Password');

        if (un.hasClass('error') || pw.hasClass('error')) {
            return;
        }

        jq("form").submit();
    });

    //Post Change Password
    jq('a.modal-submit').click(function(){
        if (jq('#TypePass').val() != jq('#ConfirmPass').val()){
            Materialize.toast('<span>New Password do not Match</span><a class="btn-flat yellow-text" href="#!">Try Again<a>', 3000);
            return;
        }

        if (jq('#OldPass').val() == jq('#TypePass').val()){
            Materialize.toast('<span>New Password cannot be the same as the old password</span><a class="btn-flat yellow-text" href="#!">Try Again<a>', 3000);
            return;
        }

        var pass = jq('#TypePass').val();

        var strength = 1;
        var arr = [/.{5,}/, /[a-z]+/, /[0-9]+/, /[A-Z]+/];
        jQuery.map(arr, function(regexp) {
          if(pass.match(regexp))
             strength++;
        });

        if (strength < 4){
            Materialize.toast('<span>Specified password is weak and can be easily bypassed</span><a class="btn-flat yellow-text" href="#!">Try Again<a>', 3000);
            return;
        }

        jq('#Password').val(jq('#TypePass').val());
        jq('#User_Password').val(jq('#OldPass').val());

        jq("form").submit();

    });

    if (messages !== ""){
        setTimeout(
          function()
          {
            Materialize.toast('<span>' + messages + '</span><a class="btn-flat yellow-text" href="#!">Try Again<a>', 3000);
          },
        1000);
    }

    if (tochange == 1) {
        setTimeout(
          function() {
            jq('#changepw').modal('open')
          },
        500);
    }
});