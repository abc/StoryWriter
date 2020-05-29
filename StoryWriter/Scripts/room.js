var thisRoom;
var secondsToAction;
var nextAction;

$(function () {
    let updateRunning = false;
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

    function updateWriters(room) {
        $("#user-list").html("");
        room.PresentWriters.forEach((element) => {
            $("#user-list").append('<li style="color: ' + element.Color.HexCode + ';">' + element.Name + "</li>");
        });
        room.AbsentWriters.forEach((element) => {
            $("#user-list").append('<li style="text-decoration: line-through; color: ' + element.Color.HexCode + ';"">' + element.Name + "</li>");
        });
    }

    story.client.userLeft = function (users) {
        console.log("User left.");
        updateWriters(users);
    };

    story.client.gameStarted = function (room) {
        console.log("Game started.");
        processUpdate(room);
        startUpdating();
        setupWriting();
    }

    story.client.userJoined = function (users) {
        console.log("User joined.");
        updateWriters(users);
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
        processUpdate(room);
        let fragments = room.FrameFragments;
        
        $("#vote-area").empty();

        fragments.forEach((element) => {
            if (element.Author.Identifier != writerId) {
                $("#vote-area").append("<div><input type=\"radio\" id=\"radio-" + element.Identifier + "\" name=\"vote\" value=\"" + element.Identifier + "\">" + element.Text + "</input></div>");
            }
        });

        $("#vote-area").append('<input type="button" id="cast-vote" value="Vote!" />');
        setupVoteSubmission();
    };

    story.client.startWriting = function (room) {
        processUpdate(room);
        $.ajax({
            url: rootPath + "/Story/StoryText/" + roomCode,
                })
            .done(function (story) {
            $("#story-body").html(story);
            setupWriting();
        });
    }

    function startUpdating() {
        if (!updateRunning) {
            update();
        }
    }

    function setupWriting() {
        $("#vote-area").empty();
        $("#fragment-area").html('<label for="nextLine">Next line of the story:</label>');
        $("#fragment-area").append('<input class="form-control" size="120" maxlength="120" name="nextLine" id="nextLine" type="text" placeholder="And then, suddenly..." />');
        $("#fragment-area").append('<input class="btn" type="button" value="Submit" id="submit-fragment" />');
        setupFragmentSubmission();
    }

    story.client.update = function (room) {
        processUpdate(room);
    }

    story.client.welcome = function (room) {
        processUpdate(room);
        if (room.Started) {
            if (room.nextAction == 0) {
                setupWriting();
            }
            startUpdating();
        }
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
        nextActionTime = new Date();
        nextActionTime = nextActionTime.getTime() + room.MillisecondsToAction;
        console.log("Update");
        console.log(nextActionTime);

        if (room.Started) {
            $('#timer-area').removeClass('d-none');
        }
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

        $('#start-game').click(function () {
            story.server.startGame();
            $('#start-game').remove();
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