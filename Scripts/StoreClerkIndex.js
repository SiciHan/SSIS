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

    $("#markasread").click(function () {
        var idNC = $(this).attr("name");
        $.ajax({
            type: 'POST',
            url: '/Home/MarkNotificationChannelAsRead',
            data: {"idNC":idNC},
            success: function (response) {
                console.log("success");
            },
            error: function (error) {
                console.log(error);
            }
        });

        $(this).attr("disable", true);
    });
    $("#markasunread").click(function () {
        var idNC = $(this).attr("name");
        $.ajax({
            type: 'POST',
            url: '/Home/MarkNotificationChannelAsUnread',
            data: { "idNC": idNC },
            success: function (response) {
                console.log("success");
            },
            error: function (error) {
                console.log(error);
            }
        });

        $(this).attr("disable", true);
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

});
