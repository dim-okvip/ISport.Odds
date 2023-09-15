"use strict";

// const queryString = window.location.search;
// const urlParams = new URLSearchParams(queryString);
// const matchId = urlParams.get('matchId');
const matchId = '121512326';
const companyId = '';

var connection = new signalR.HubConnectionBuilder().withUrl("https://localhost:7067/oddsHub").build();

connection.on("ReceiveMessage", function (message) {
    var initialOddsEuro1 = `${message.preMatchAndInPlayOddsMain.data.europeOdds[0].split(',')[2]}`;
    document.getElementById('initialOddsEuro1').innerHTML = initialOddsEuro1;
});

connection.start().then(function () {
    triggerHub();
    setInterval(triggerHub, 5_000);
    // alert(connection.connectionId);
}).catch(function (err) {
    return console.error(err.toString());
});

function triggerHub() {
    connection.invoke("SendMessage", matchId, companyId).catch(function (err) {
        return console.error(err.toString());
    });
}