
$(document).ready(function () {
    $("#cancel").click(function () {
        alert("You have cancel the Purchase Orders.");
    });

    $("#target").blur(function () {
        var orderUnit = this.value;
        var idpod = this.name.split("_")[1];
        if (orderUnit == null) {
            orderUnit = 0;
        }
        window.location.href = "/StoreClerk/UpdateOrderUnit?orderUnit=" + orderUnit + "&idPOD=" + idpod;
        alert("you are changing the order units");
    });

    /*// Click on notification icon for show notification
    $("#noti-icon").click(function (e) {
        e.stopPropagation();
        
        $("#noti-content").show();
        var count = 0;
        count = parseInt($("#noti-count").html()) || 0;

        alert("icon is clicked count="+count);
        if (count >= 0) {
            $("#notiContent").empty();
            $("#notiContent").append($('<h4>Notifications</h4>'));
            $("#notiContent").append($('<li>Loading...</li>'));
            //need to get notificationchannels from server
            $.ajax({
                type: 'GET',
                url: '/Home/GetNotifications',
                success: function (response) {
                    $('#notiContent').empty();
                    $("#notiContent").append($('<h4>Notifications</h4>'));
                    if (response.length == 0) {
                        $('#notiContent').append($('<li>No data available</li>'));
                    }
                    $.each(response, function (index, value) {
                        $('#notiContent').append($('<li>From ' + value.From.Name + ': ' + value.Notification.Text + '</li>'));
                    });
                },
                error: function (error) {
                    console.log(error);
                }
            });
        }
        //set it to space
        $('#noti-count', this).html('&nbsp;');
    });

    // hide notifications
    $('html').click(function () {
        $('#noti-content').hide();
    });*/

    //initiate the connection
    $.connection.hub.start()
        .done(function () {
            //when the page is just opened
            //need to attach the onclick listener to submit button
            $("#submitaction").click(function () {
                alert("You have submitted the Purchase Orders.");
                //create entry in database

                var data = {
                    "role": "StockManager",
                    "message":"Hi Manager, your employee has submitted a purchase order for you to view."
                };
                $.ajax({
                    type: 'POST',
                    url: '/Home/CreateNotificationsToGroup',
                    
                    data: data,
                    success: function (response) {
                        console.log("success");
                    },
                    error: function (error) {
                        console.log(error);
                    }
                });
                //send a notification to storeManager
                $.connection.chatHub.server.sendNotificationByGroupByRole(parseInt($('#hdnSession1').val()), "StoreManager", "Hi Manager! You have a new Purchase Order Raised.");
            });
        })
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