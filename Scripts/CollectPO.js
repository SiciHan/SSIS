$(document).ready(function () {

    $("#confirmReceiveOrder").submit(function () {
        alert("You have confirmed that you received all items in this purchase order.");
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


