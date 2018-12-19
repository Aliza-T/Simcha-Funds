$(() => {
    $(".deposit").on('click', function () {
        const button = $(this);
        const contributorId = button.data('contributor-id');
        $('#contributorId').val(contributorId);

        $(".deposit-modal").modal();
    });
    $("#contributor").on('click', function () {
        $(".contributor").modal();
    });
    $(".edit").on('click', function () {
        const button = $(this);
        const id = button.data('id');
        const firstName = button.data('firstname');
        const lastName = button.data('lastname');
        const cell = button.data('cell');
        const alwaysInclude = button.data('always_include');
        const date = button.data('date');

        $("#firstname").val(firstName);
        $("#lastName").val(lastName);
        $("#cell").val(cell);
        $("#alwaysInclude").attr('checked', alwaysInclude === "True");
        $("#date").val(date);
        $(".edit-modal").modal();
        $("#id").val(id);
    });
    $("#clear").on('click', function () {
        $("#search").val('');
        $("tr").show();
    });

    $("#search").keyup(function () {
        const val = $(this).val().toUpperCase();
        $("#table tr:gt(0)").each(function () {
            const row = $(this);
            const name = row.find("td:eq(1)").text().toUpperCase();
            if (name.includes(val)) {
                row.show();
            } else {
                row.hide();
            }
        });
    });

   
     
  

});
