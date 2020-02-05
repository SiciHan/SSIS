
$(document).ready(function () {
    $('div[id^="details-"]').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget); // Button that triggered the modal
        var modal = $(this);
        //modal.find('.modal-title').text('New message to ' + recipient)
        //modal.find('.modal-body input').val(recipient)
    });

    $('div[id^="change-"]').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget) // Button that triggered the modal
        //var target = button.data('target') // Extract info from data-* attributes
        // If necessary, you could initiate an AJAX request here (and then do the updating in a callback).
        // Update the modal's content. We'll use jQuery here, but you could use a data binding library or other methods instead.
        var modal = $(this)
        //modal.find('.modal-title').text('New message to ' + recipient)
        //modal.find('.modal-body input').val(recipient)
    });

    $('div[id^="schedule-"]').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget) // Button that triggered the modal
        //var target = button.data('target') // Extract info from data-* attributes
        // If necessary, you could initiate an AJAX request here (and then do the updating in a callback).
        // Update the modal's content. We'll use jQuery here, but you could use a data binding library or other methods instead.
        var modal = $(this)
        //modal.find('.modal-title').text('New message to ' + recipient)
        //modal.find('.modal-body input').val(recipient)
    });

    $("#updateRejectedPO").click(function () {
        window.location.href = "/StoreClerk/UpdatePO?id=" + $(this).attr("name")
    });

    $("#Schedule").click(function () {

        var idPO = $(this).attr("name")
        var x = document.getElementById("deliveredDate_" + idPO).value;
        window.location.href = "/StoreClerk/Schedule?idPO=" + idPO + "&deliverDate=" + x;
    });

      //copy paste this to all pages.
    $.connection.hub.start()
        .done()
        .fail();
  
    $.connection.chatHub.client.receiveNotification = function (IdReceiver) {
        //if the notification is sent to me
        if (parseInt($("#hdnSession1").val()) == IdReceiver) {
            var count = 0;
            count = parseInt($('#noti-count').html()) || 0;
            count++;
            alert("There is a notification");
            $('#noti-count').html(count);
        }
    };

});
