$(function () {
    // Declare a proxy to reference the hub.
    var story = $.connection.storyHub;
    // Create a function that the hub can call to broadcast messages.
    story.client.broadcastMessage = function (name, message) {
        // Html encode display name and message.
        var encodedName = $('<div />').text(name).html();
        var encodedMsg = $('<div />').text(message).html();
        // Add the message to the page.
        $('#discussion').append('<li><strong>' + encodedName
            + '</strong>:&nbsp;&nbsp;' + encodedMsg + '</li>');
    };

    story.client.userLeft = function (user) {
        console.log("User left:");
        console.log(user);
    };

    story.client.userJoined = function (user) {
        console.log("User joined:");
        console.log(user);
    };

    story.client.voteCast = function (userId, fragmentId) {
        console.log("Vote cast:");
        console.log(userId + " voted for " + fragmentId);
    };

    story.client.newFragment = function (fragment) {
        console.log("New fragment:");
        console.log(fragment);
    };

    function leaveRoom() {
        story.server.leaveRoom();
        document.location.href = rootPath + "/Story/LeaveRoom/" + roomCode;
    }

    function joinRoom() {
        story.server.joinRoom(writerId, roomCode);
    }

    // Start the connection.
    $.connection.hub.start().done(function () {
        joinRoom();

        $('#leave-room').click(function () {
            leaveRoom();
        });

        $('#sendmessage').click(function () {
            // Call the Send method on the hub.
            story.server.send($('#displayname').val(), $('#message').val(), $("#room-code").val());
            // Clear text box and reset focus for next comment.
            $('#message').val('').focus();
        });
    });
});