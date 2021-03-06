﻿//Shutong
$(document).ready(function () {
    $("#Unread").click(function () {
        $("#Unread").addClass("btn-primary");
        $("#Read").removeClass("btn-primary");
        $("#All").removeClass("btn-primary");
        $("#UnreadNC").show();
        $("#ReadNC").hide();
        $("#AllNC").hide();

        if ($("#UnreadNC").children().length == 0) {
            $("#UnreadNC").append("<p>There is no Unread Notifications.</p>")
        }
    });

    $("#Read").click(function () {
        $("#Read").addClass("btn-primary");
        $("#Unread").removeClass("btn-primary");
        $("#All").removeClass("btn-primary");
        $("#UnreadNC").hide();
        $("#ReadNC").show();
        $("#AllNC").hide();

        if ($("#ReadNC").children().length == 0) {
            $("#ReadNC").append("<p>There is no Read Notifications.</p>")
        }
    });

    $("#All").click(function () {
        $("#All").addClass("btn-primary");
        $("#Unread").removeClass("btn-primary");
        $("#Read").removeClass("btn-primary");
        $("#UnreadNC").hide();
        $("#ReadNC").hide();
        $("#AllNC").show();

        if ($("#AllNC").children().length == 0) {
            $("#AllNC").append("<p>There is no Notifications.</p>")
        }
    });

    $("button[name='markasread']").click(function () {

        //alert("maked as read");
        var idNC = $(this).attr("id");
        $.ajax({
            type: 'POST',
            url: '/Home/MarkNotificationChannelAsRead',
            data: { "idNC": idNC },
            success: function (response) {
                console.log(response);
            },
            error: function (error) {
                console.log(error);
            }
        });
        //alert("maked as read");
        //count minus 1
        var count = parseInt($('#noti-count').html());
        count--;
        $('#noti-count').html(count);

        $(this).attr("class", "btn-danger");
        $(this).empty();
        $(this).append("Read");
        $(this).off();

    });

    $("button[name='markasunread']").click(function () {
        //alert("maked as read");
        var idNC = $(this).attr("id");
        $.ajax({
            type: 'POST',
            url: '/Home/MarkNotificationChannelAsUnread',
            data: { "idNC": idNC },
            success: function (response) {
                console.log(response);
            },
            error: function (error) {
                console.log(error);
            }
        });
        //alert("maked as unread");

        //count plus 1
        var count = parseInt($('#noti-count').html());
        count++;
        $('#noti-count').html(count);
        $(this).attr("class", "btn-danger");
        $(this).empty();
        $(this).append("Unread");
        $(this).off();

    });

    // initially hide All and Read notifcations
    $("#Unread").addClass("btn-primary");
    $("#ReadNC").hide();
    $("#AllNC").hide();
    $(function () {
        if ($("#UnreadNC").children().length == 0) {
            $("#UnreadNC").append("<p>There is no Unread Notifications.</p>")
        }
    });

    //copy paste this to all pages.
    $.connection.hub.start()
        .done()
        .fail();

    //to change the count in real time when someone send a notification to the person
    $.connection.chatHub.client.receiveNotification = function (IdReceiver) {
        //alert("There is a notification");
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
            //alert("There are " + response + "unread notifications " + "for" + $("#hdnSession1").val());
            $('#noti-count').html(response);
        },
        error: function (error) {
            console.log(error);
        }
    });

});
