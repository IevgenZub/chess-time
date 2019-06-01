// ================ Declarations =================================================================
var board,
    game = new Chess(),
    statusEl = $('#status'),
    pgnEl = $('#pgn'),
    playersOnline = $('#playersOnline'),
    gamesOnline = $('#gamesOnline');

var connection = new signalR.HubConnectionBuilder().withUrl("/game-hub").build();


// ================ Client event handlers =======================================================

connection.start().then(function () {
    console.log("started");
    $('#btnStartWhite').click(function () {
        connection.invoke("StartGame", "white").catch(function (err) {
            return console.error(err.toString());
        });
    });
    $('#btnStartBlack').click(function () {
        connection.invoke("StartGame", "black").catch(function (err) {
            return console.error(err.toString());
        });
    });
}).catch(function (err) {
    return console.error(err.toString());
});

function joinGame(gameId) {
    connection.invoke("JoinGame", gameId).catch(function (err) {
        return console.error(err.toString());
    });
}


// ================ Client side methods to call from server with SignalR =======================

connection.on("GameCreated", function (game) {
    if (game.blackPlayerId === null) {
        game.blackPlayerId = "open";
    }

    if (game.whitePlayerId === null) {
        game.whitePlayerId = "open";
    }

    gamesOnline.append("<button id='" + game.id + "'>White: "
        + game.whitePlayerId + ', Black: ' + game.blackPlayerId + "</button>");

    $('#' + game.id).click(function (e) {
        joinGame(this.id);
    });
});


connection.on("PlayerJoined", function (playerName) {
    playersOnline.append('<p>' + playerName + '</p>');
});

connection.on("GameStarted", function (game) {
    var cfg = {
        draggable: true,
        position: 'start',
        onDragStart: onDragStart,
        onDrop: onDrop,
        orientation: currentUser === game.whitePlayerId ? "white" : "black",
        onSnapEnd: onSnapEnd
    };

    $('#' + game.id).text('White:' + game.whitePlayerId + ', Black: ' + game.blackPlayerId);

    board = ChessBoard('board', cfg);

    updateStatus();
});

connection.on("MoveMade", function (newMove) {
    var move = game.move({
        from: newMove.from,
        to: newMove.to,
        promotion: 'q'
    });

    board.move(move.from + "-" + move.to);

    updateStatus();
});


// do not pick up pieces if the game is over
// only pick up pieces for the side to move
var onDragStart = function (source, piece, position, orientation) {
    if (game.game_over() === true ||
        (game.turn() === 'w' && piece.search(/^b/) !== -1) ||
        (game.turn() === 'b' && piece.search(/^w/) !== -1)) {
        return false;
    }
};

var onDrop = function (source, target) {
    // see if the move is legal
    var move = game.move({
        from: source,
        to: target,
        promotion: 'q' // NOTE: always promote to a queen for example simplicity
    });

    // illegal move
    if (move === null) return 'snapback';

    connection.invoke("MakeMove", move).catch(function (err) {
        return console.error(err.toString());
    });

    updateStatus();
};

// update the board position after the piece snap
// for castling, en passant, pawn promotion
var onSnapEnd = function () {
    board.position(game.fen());
};

var updateStatus = function () {
    var status = '';

    var moveColor = 'White';
    if (game.turn() === 'b') {
        moveColor = 'Black';
    }

    // checkmate?
    if (game.in_checkmate() === true) {
        status = 'Game over, ' + moveColor + ' is in checkmate.';
    }

    // draw?
    else if (game.in_draw() === true) {
        status = 'Game over, drawn position';
    }

    // game still on
    else {
        status = moveColor + ' to move';

        // check?
        if (game.in_check() === true) {
            status += ', ' + moveColor + ' is in check';
        }
    }

    statusEl.html(status);
    pgnEl.html(game.pgn());
};
