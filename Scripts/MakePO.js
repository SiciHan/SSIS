$(document).ready(function () {

    $("#findItems").click(function () {
        var searchStr = document.getElementById("searchStr").value;
        window.location.href = window.location.href.split('?')[0] + "?searchStr=" + searchStr;
    });

    $("#addToCart").submit(function () { alert("you have added something to the cart"); });
    $("#addToCart2").submit(function () { alert("you have added something to the cart"); });

    $("#select-all1").click(function () {

        var countSearchResult = parseInt($(this).attr("name"));
        var checkboxes = document.querySelectorAll('input[type="checkbox"]');
        for (var i = 0; i < countSearchResult+1; i++) {
            if (checkboxes[i] !=this)
                checkboxes[i].checked = this.checked;
        }
    });

    $("#select-all2").click(function () {
        var countSearchResult = parseInt($("#select-all1").attr("name"))||0;
        var checkboxes = document.querySelectorAll('input[type="checkbox"]');
        for (var i = countSearchResult+1; i < checkboxes.length; i++) {
            if (checkboxes[i] != this)
                checkboxes[i].checked = this.checked;
        }
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


