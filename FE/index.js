"use strict";

// const queryString = window.location.search;
// const urlParams = new URLSearchParams(queryString);
// const matchId = urlParams.get('matchId');
const matchId = '447986420';
const companyId = '';

var connection = new signalR.HubConnectionBuilder().withUrl("https://localhost:7067/oddsHub").build();

connection.on("ReceiveMessage", function (message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    li.textContent = `${message.data.handicap[0].split(',')[2]}`;
});

connection.start().then(function () {
    connection.invoke("SendMessage", matchId, companyId).catch(function (err) {
        return console.error(err.toString());
    });
    setInterval(() => {
        connection.invoke("SendMessage", matchId, companyId).catch(function (err) {
                    return console.error(err.toString());
                });
    }, 5_000);
    // alert(connection.connectionId);
}).catch(function (err) {
    return console.error(err.toString());
});