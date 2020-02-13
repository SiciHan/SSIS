//Shutong
$.connection.hub.start()
    .done(function () {
        alert("it worked");
        $.connection.chatHub.server.announce("Connected");

        $.connection.chatHub.server.getNotifications()
            .done(function (data) {
                if (data.length!=0) {
                    $("#msg").append(message);
                }
                else {
                    alert("No Notification");
                }
                
            })
            .fail(function () { });
    }
    )
    .fail(function () {
        alert("Error")
    }
        );
$.connection.chatHub.client.announce = function (message) {
    $("#msg").append(message);
}
