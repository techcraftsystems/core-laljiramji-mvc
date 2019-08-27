/*
 * DataTables - Tables
 */


$(function() {
  var table = $('#customers-table').DataTable({
    "columnDefs": [{
      "visible": false,
      "targets": 2
    }],
    "order": [
      [2, 'asc']
    ],
    "displayLength": 50,
    "drawCallback": function(settings) {
      var api = this.api();
      var rows = api.rows({
        page: 'current'
      }).nodes();
      var last = null;

      api.column(2, {
        page: 'current'
      }).data().each(function(group, i) {
        if (last !== group) {
          $(rows).eq(i).before(
            '<tr class="group"><td colspan="6">' + group + '</td></tr>'
          );

          last = group;
        }
      });
    }
  });
});

// Datatable click on select issue fix
$(window).on('load', function() {
  $(".dropdown-content.select-dropdown li").on("click", function() {
    var that = this;
    setTimeout(function() {
      if ($(that).parent().parent().find('.select-dropdown').hasClass('active')) {
        // $(that).parent().removeClass('active');
        $(that).parent().parent().find('.select-dropdown').removeClass('active');
        $(that).parent().hide();
      }
    }, 100);
  });
});
