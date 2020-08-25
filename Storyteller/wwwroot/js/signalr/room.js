var thisRoom;
var secondsToAction;
var nextAction;

$(function () {
    let updateRunning = false;
    // Declare a proxy to reference the hub.
    var connection = new signalR.HubConnectionBuilder().withUrl("/storyhub").build();
    // Create a function that the hub can call to broadcast messages.

    function updateWriters(room) {
        $("#user-list").html("");
        room.PresentWriters.forEach((element) => {
            $("#user-list").append('<li style="color: ' + element.Color.HexCode + ';">' + element.Name + "</li>");
        });
        room.AbsentWriters.forEach((element) => {
            $("#user-list").append('<li style="text-decoration: line-through; color: ' + element.Color.HexCode + ';"">' + element.Name + "</li>");
        });
    }

    connection.on("userLeft", function (users) {
        console.log("User left.");
        updateWriters(users);
    });

    connection.on("gameStarted", function (room) {
        console.log("Game started.");
        processUpdate(room);
        startUpdating();
        setupWriting();
    });

    connection.on("userJoined", function (users) {
        console.log("User joined.");
        updateWriters(users);
    });

    connection.on("voteCast", function (userId, fragmentId) {
        console.log("Vote cast:");
        console.log(userId + " voted for " + fragmentId);
    });

    connection.on("newFragment", function (fragment) {
        console.log("New fragment:");
        console.log(fragment);
    });

    connection.on("startVoting", function (room) {
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
    });

    connection.on("startWriting", function (room) {
        processUpdate(room);
        $.ajax({
            url: rootPath + "/Story/StoryText/" + roomCode,
        })
            .done(function (story) {
                $("#story-body").html(story);
                setupWriting();
            });
    });

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

    connection.on("update", function (room) {
        processUpdate(room);
    });

    connection.on("welcome", function (room) {
        processUpdate(room);
        if (room.Started) {
            if (room.nextAction == 0) {
                setupWriting();
            }
            startUpdating();
        }
    });

    function leaveRoom() {
        connection.invoke("leaveRoom");
        document.location.href = rootPath + "/Story/LeaveRoom/" + roomCode;
    }

    function joinRoom() {
        connection.invoke("joinRoom", writerId, roomCode).catch(function (err) {
            console.log("there was an exception :C");
            console.log(writerId);
            console.log(roomCode);
            return console.error(err.toString());
        });
    }

    function setupFragmentSubmission() {
        $('#submit-fragment').click(function () {
            connection.invoke("submitFragment", $('#nextLine').val());
            $("#fragment-area").html("");
        });
    }

    function setupVoteSubmission() {
        $('#cast-vote').click(function () {
            connection.invoke("castVote", ($('#vote-area').find('input[name="vote"]:checked').val()));
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
    connection.start().then(function () {
        console.log("started - id: " + writerId + ", code: " + roomCode);
        start();
    }).catch(function (err) {
        return console.error(err.toString());
    });

    function start() {
        joinRoom();

        $('#leave-room').click(function () {
            leaveRoom();
        });

        $('#start-game').click(function () {
            connection.invoke("startGame").catch(function (err) {
                return console.error(err.toString());
            });

            $('#start-game').remove();
        });

        setupFragmentSubmission();
    }
});