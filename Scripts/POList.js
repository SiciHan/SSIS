
$(document).ready(function () {
    $('div[id^="details-"]').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget); // Button that triggered the modal
        var modal = $(this);
    });

    $('div[id^="change-"]').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget) // Button that triggered the modal
        var modal = $(this)
    });

    $('div[id^="schedule-"]').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget) // Button that triggered the modal
        var modal = $(this)
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
            alert("There is a notification for you.");
            $('#noti-count').html(count);
        }
    };
    $.ajax({
        type: 'GET',
        url: '/Home/GetUnreadNotificationCount?IdReceiver=' + $("#hdnSession1").val(),
        success: function (response) {
            $('#noti-count').html(response);
        },
        error: function (error) {
            console.log(error);
        }
    })

});
