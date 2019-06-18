jq(function() {
    jq('.modal').modal();

    if (message != '') {
        Materialize.toast('<span>' + message + '</span><a class="btn-flat yellow-text" href="#!">Try Again<a>', 3000)
    }

    jq('#queue-table').DataTable({
        "displayLength": 25,
    });

    jq('#queue-table_filter').on('click', 'i', function() {
        window.location.href = "/accounts/users/add";
    });

    jq('#queue-table td').on('click', 'i.blue-text', function() {
        window.location.href = "/accounts/users/edit?u=" + jq(this).data('idnt');
    });

    jq('#queue-table td').on('click', 'i.black-text', function() {
        uuid = jq(this).data('idnt');
        name = jq(this).closest('tr ').find('td:nth-child(2)').text();

        jq('#user-name').val(name);
        jq('#reset-pw-modal').modal('open');
    });

    jq('<i class="material-icons medium blue-text right">person_add</i>').insertBefore(jq("#queue-table_filter label"));

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
            Materialize.toast('<span>Specified password is weak and can be easily bypassed</span><a class="btn-flat yellow-text" href="#!">Try Again<a>', 3000);
            return;
        }

        jq("form").submit();
    });

    jq('#reset-pw-modal a.modal-post').click(function(){
        ResetPassword();
        setTimeout(
          function() {
            window.location.href = "/accounts/users";
          },
        1000);
    });

    jq('a.reset-password').click(function(){
        ResetPassword();
    });

    jq('a.disable-account').click(function(){
        if (usern == login){
            Materialize.toast('<span>You can not perform this action on your own account</span><a class="btn-flat yellow-text" href="#!">Try Again<a>', 3000);
            return;
        }

        var txt = jq(this).text();
        var opt = 0;
        var msg = "";

        if (txt == 'Disable Account') {
            opt = 0;
            msg = "disabled";
            jq(this).text('Enable Account');
        }
        else{
            opt = 1;
            msg = "enabled";
            jq(this).text('Disable Account')
        } 

        EnableAccount(opt, msg);
    });
});

function ResetPassword(){
    jq.ajax({
        dataType: "text",
        url: '/Account/ResetPassword',
        data: {
            "uuid": uuid
        },
        success: function(results) {
            Materialize.toast('<span>Successfully reset password for ' + name + '</span><a class="btn-flat yellow-text" href="#!">Close<a>', 3000);
        },
        error: function(xhr, ajaxOptions, thrownError) {
            Materialize.toast('<span>' + xhr.status + '</span><a class="btn-flat yellow-text" href="#!">Close<a>', 3000);
            Materialize.toast('<span>' + thrownError + '</span><a class="btn-flat yellow-text" href="#!">Close<a>', 3000);
        }
    });
}

function EnableAccount(opts, msgs){
    jq.ajax({
        dataType: "text",
        url: '/Account/EnableAccount',
        data: {
            "uuid": uuid,
            "opts": opts,
        },
        success: function(results) {
            Materialize.toast('<span>Successfully '+ msgs + ' account for ' + name + '</span><a class="btn-flat yellow-text" href="#!">Close<a>', 3000);
        },
        error: function(xhr, ajaxOptions, thrownError) {
            Materialize.toast('<span>' + xhr.status + '</span><a class="btn-flat yellow-text" href="#!">Close<a>', 3000);
            Materialize.toast('<span>' + thrownError + '</span><a class="btn-flat yellow-text" href="#!">Close<a>', 3000);
        }
    });
}
