var thisRoom;
var secondsToAction;
var nextActionTime;
var nextAction;

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

    story.client.startVoting = function (room) {
        let fragments = room.FrameFragments;

        $("#vote-area").empty();

        fragments.forEach((element) => {
            $("#vote-area").append("<div><input type=\"radio\" id=\"radio-" + element.Identifier + "\" name=\"vote\" value=\"" + element.Identifier + "\">" + element.Text + "</input></div>");
        });

        $("#vote-area").append('<input type="button" id="cast-vote" value="Vote!" />');
        setupVoteSubmission();
    };

    story.client.startWriting = function (room) {
        $.ajax({
            url: rootPath + "/Story/StoryText/" + roomCode,
                })
        .done(function (story) {
            $("#story-body").html(story);
            $("#vote-area").empty();
        });
    }

    story.client.update = function (room) {
        processUpdate(room);
    }

    story.client.welcome = function (room) {
        processUpdate(room);
        update();
    }

    function leaveRoom() {
        story.server.leaveRoom();
        document.location.href = rootPath + "/Story/LeaveRoom/" + roomCode;
    }

    function joinRoom() {
        story.server.joinRoom(writerId, roomCode);
    }

    function setupFragmentSubmission() {
        $('#submit-fragment').click(function () {
            story.server.submitFragment($('#nextLine').val());
            $("#fragment-area").html("");
        });
    }

    function setupVoteSubmission() {
        $('#cast-vote').click(function () {
            story.server.castVote($('#vote-area').find('input[name="vote"]:checked').val());
            $("#vote-area").html("");
        });
    }

    function processUpdate(room) {
        console.log(room);
        thisRoom = room;
        nextActionTime = new Date(thisRoom.NextActionTime);
    }

    function update() {
        timeLeft = nextActionTime - new Date();
        secondsToAction = timeLeft / 1000;
        $('#countdown-timer').attr('data-value', secondsToAction);
        $('#seconds-to-vote').html(Math.round(secondsToAction));
        updateTimer();
        setTimeout(function () {
            update();
        }, 20);
    }

    // Start the connection.
    $.connection.hub.start().done(function () {
        joinRoom();

        $('#leave-room').click(function () {
            leaveRoom();
        });

        setupFragmentSubmission();

        $('#sendmessage').click(function () {
            // Call the Send method on the hub.
            story.server.send($('#displayname').val(), $('#message').val(), $("#room-code").val());
            // Clear text box and reset focus for next comment.
            $('#message').val('').focus();
        });
    });
});