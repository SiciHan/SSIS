﻿//Shutong
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

    //initiate the connection
    $.connection.hub.start()
        .done(function () {
            //when the page is just opened
            //need to attach the onclick listener to submit button
            $("#submitaction").click(function () {
                
                //create entry in database

                var data = {
                    "role": "Supervisor",
                    "message":"Hi Supervisor, your employee has submitted a purchase order for you to view."
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

                $.ajax({
                    type: 'POST',
                    url: '/Home/SendEmailToGroup',

                    data: data,
                    success: function (response) {
                        console.log("success");
                    },
                    error: function (error) {
                        console.log(error);
                    }
                });

                alert("You have submitted the Purchase Orders.");
                //send a notification to storeManager
                $.connection.chatHub.server.sendNotificationByGroupByRole(parseInt($('#hdnSession1').val()), "StoreManager", "Hi Manager! You have a new Purchase Order Raised.");
            });
        })
        .fail();

    //copy paste this to all pages.
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