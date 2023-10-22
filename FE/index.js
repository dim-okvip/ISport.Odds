"use strict";

const queryString = window.location.search;
const urlParams = new URLSearchParams(queryString);
const matchId = urlParams.get('matchId');
const companyId = '';

const UnableToJudge = '0';
const EarlyOdds = '1';
const InstantOdds = '2';
const InplayOdds = '3';

//const signalR_URL = "https://localhost:7067/oddsHub";
const signalR_URL = "http://localhost:5224/oddsHub";

var connection = new signalR.HubConnectionBuilder().withUrl(signalR_URL).build();

function checkOddsType(array) {
    var earlyOddsCount = 0;
        var instantOddsCount = 0;
        var inplayOddsCount = 0;
        for (let i = 0; i < array.length; i++) {
            const item = array[i];
            var oddsType = item[item.length - 1];
            switch (oddsType) {
                case EarlyOdds:
                    earlyOddsCount++;
                    break;
                case InstantOdds:
                    instantOddsCount++;
                    break;
                case InplayOdds:
                    inplayOddsCount++;
                break;
                default:
                    break;
            }
        }

        var oddsType = UnableToJudge;

        if(earlyOddsCount>instantOddsCount && earlyOddsCount>inplayOddsCount) oddsType = EarlyOdds;
        else if(inplayOddsCount>earlyOddsCount && inplayOddsCount>instantOddsCount) oddsType = InplayOdds;
        else oddsType = InstantOdds;

        return oddsType;
};

function logTime(message) {
    var currentdate = new Date(); 
    var datetime = `${message}: ` + currentdate.getDate() + "/"
                + (currentdate.getMonth() + 1)  + "/" 
                + currentdate.getFullYear() + " "  
                + currentdate.getHours() + ":"  
                + currentdate.getMinutes() + ":" 
                + currentdate.getSeconds();
    console.log(`${datetime}`);
}

connection.on("ReceiveInitializationOddsByMatch", function (message) {
    var preMatchAndInPlayOddsMain = message.preMatchAndInPlayOddsMain.data;
    if (preMatchAndInPlayOddsMain.europeOdds.length > 0 && preMatchAndInPlayOddsMain.handicap.length > 0 && preMatchAndInPlayOddsMain.overUnder.length > 0) {
        var oddsType = checkOddsType(preMatchAndInPlayOddsMain.handicap);
        
        var euroOdds = preMatchAndInPlayOddsMain.europeOdds[0].split(',');
        var handicap = preMatchAndInPlayOddsMain.handicap[0].split(',');
        var overUnder = preMatchAndInPlayOddsMain.overUnder[0].split(',');
    
        document.getElementById('europeOdds-initialHome').innerHTML = euroOdds[2];
        document.getElementById('europeOdds-initialDraw').innerHTML = euroOdds[3];
        document.getElementById('europeOdds-initialAway').innerHTML = euroOdds[4];
    
        document.getElementById('handicap-initialHandicapHome').innerHTML = `${-parseFloat(handicap[2])}`;
        document.getElementById('handicap-initialHome').innerHTML = handicap[3];
        document.getElementById('handicap-initialHandicapAway').innerHTML = handicap[2];
        document.getElementById('handicap-initialAway').innerHTML = handicap[4];
    
        document.getElementById('overUnder-initialHandicap').innerHTML = overUnder[2];
        document.getElementById('overUnder-initialOver').innerHTML = overUnder[3];
        document.getElementById('overUnder-initialUnder').innerHTML = overUnder[4];
        
        // var totalCornersPreMatch = message.totalCornersPreMatch.data;
        // if (totalCornersPreMatch.length > 0) {
        //     document.getElementById('totalCorners-initialTotalCorners').innerHTML = totalCornersPreMatch[0].odds.totalCorners;
        //     document.getElementById('totalCorners-initialOver').innerHTML = totalCornersPreMatch[0].odds.over;
        //     document.getElementById('totalCorners-initialUnder').innerHTML = totalCornersPreMatch[0].odds.under;    
        // }
    
        switch (oddsType) {
            case EarlyOdds:
            case InstantOdds:
                document.getElementById('inPlay').style.display = 'none';
                document.getElementById('beforeMatch').style.display = '';
    
                document.getElementById('europeOdds-beforeMatchHome').innerHTML = euroOdds[5];
                document.getElementById('europeOdds-beforeMatchDraw').innerHTML = euroOdds[6];
                document.getElementById('europeOdds-beforeMatchAway').innerHTML = euroOdds[7];
    
                document.getElementById('handicap-beforeMatchHandicapHome').innerHTML = `${-parseFloat(handicap[5])}`;
                document.getElementById('handicap-beforeMatchHome').innerHTML = handicap[6];
                document.getElementById('handicap-beforeMatchHandicapAway').innerHTML = handicap[5];
                document.getElementById('handicap-beforeMatchAway').innerHTML = handicap[7];
    
                document.getElementById('overUnder-beforeMatchHandicap').innerHTML = overUnder[5];
                document.getElementById('overUnder-beforeMatchOver').innerHTML = overUnder[6];
                document.getElementById('overUnder-beforeMatchUnder').innerHTML = overUnder[7];
                
                // var totalCornersInPlay = message.totalCornersInPlay.data;
                // if (totalCornersInPlay.length > 0) {
                //     document.getElementById('totalCorners-beforeMatchTotalCorners').innerHTML = totalCornersInPlay[0].odds.totalCorners;
                //     document.getElementById('totalCorners-beforeMatchOver').innerHTML = totalCornersInPlay[0].odds.over;
                //     document.getElementById('totalCorners-beforeMatchUnder').innerHTML = totalCornersInPlay[0].odds.under;
                // }
                break;
            case InplayOdds:
                document.getElementById('beforeMatch').style.display = 'none';
                document.getElementById('inPlay').style.display = '';
    
                document.getElementById('europeOdds-inPlayHome').innerHTML = euroOdds[5];
                document.getElementById('europeOdds-inPlayDraw').innerHTML = euroOdds[6];
                document.getElementById('europeOdds-inPlayAway').innerHTML = euroOdds[7];
    
                document.getElementById('handicap-inPlayHandicapHome').innerHTML = `${-parseFloat(handicap[5])}`;
                document.getElementById('handicap-inPlayHome').innerHTML = handicap[6];
                document.getElementById('handicap-inPlayHandicapAway').innerHTML = handicap[5];
                document.getElementById('handicap-inPlayAway').innerHTML = handicap[7];
    
                document.getElementById('overUnder-inPlayHandicap').innerHTML = overUnder[5];
                document.getElementById('overUnder-inPlayOver').innerHTML = overUnder[6];
                document.getElementById('overUnder-inPlayUnder').innerHTML = overUnder[7];
    
                // var totalCornersInPlay = message.totalCornersInPlay.data;
                // if (totalCornersInPlay.length > 0) {
                //     document.getElementById('totalCorners-inPlayTotalCorners').innerHTML = totalCornersInPlay[0].odds.totalCorners;
                //     document.getElementById('totalCorners-inPlayOver').innerHTML = totalCornersInPlay[0].odds.over;
                //     document.getElementById('totalCorners-inPlayUnder').innerHTML = totalCornersInPlay[0].odds.under;    
                // }
                break;
            default:
                break;
        }
    }else{
        alert('Không có dữ liệu');
    }
});

connection.on("ReceiveOddsChangesByMatch", function (message) {
    var preMatchAndInPlayOddsMain = message;

    if (preMatchAndInPlayOddsMain.europeOdds.length > 0) {
        var oddsType = checkOddsType(preMatchAndInPlayOddsMain.europeOdds);
        var euroOdds = preMatchAndInPlayOddsMain.europeOdds[0].split(',');

        document.getElementById('europeOdds-initialHome').innerHTML = euroOdds[2];
        document.getElementById('europeOdds-initialDraw').innerHTML = euroOdds[3];
        document.getElementById('europeOdds-initialAway').innerHTML = euroOdds[4];

        switch (oddsType) {
            case EarlyOdds:
            case InstantOdds:
                document.getElementById('inPlay').style.display = 'none';
                document.getElementById('beforeMatch').style.display = '';
    
                document.getElementById('europeOdds-beforeMatchHome').innerHTML = euroOdds[5];
                document.getElementById('europeOdds-beforeMatchDraw').innerHTML = euroOdds[6];
                document.getElementById('europeOdds-beforeMatchAway').innerHTML = euroOdds[7];
                break;
            case InplayOdds:
                document.getElementById('beforeMatch').style.display = 'none';
                document.getElementById('inPlay').style.display = '';
    
                document.getElementById('europeOdds-inPlayHome').innerHTML = euroOdds[5];
                document.getElementById('europeOdds-inPlayDraw').innerHTML = euroOdds[6];
                document.getElementById('europeOdds-inPlayAway').innerHTML = euroOdds[7];
                break;
            default:
                break;
        }
    };

    if (preMatchAndInPlayOddsMain.handicap.length > 0) {
        var oddsType = checkOddsType(preMatchAndInPlayOddsMain.handicap);
        var handicap = preMatchAndInPlayOddsMain.handicap[0].split(',');

        document.getElementById('handicap-initialHandicapHome').innerHTML = `${-parseFloat(handicap[2])}`;
        document.getElementById('handicap-initialHome').innerHTML = handicap[3];
        document.getElementById('handicap-initialHandicapAway').innerHTML = handicap[2];
        document.getElementById('handicap-initialAway').innerHTML = handicap[4];

        switch (oddsType) {
            case EarlyOdds:
            case InstantOdds:
                document.getElementById('inPlay').style.display = 'none';
                document.getElementById('beforeMatch').style.display = '';
    
                document.getElementById('handicap-beforeMatchHandicapHome').innerHTML = `${-parseFloat(handicap[5])}`;
                document.getElementById('handicap-beforeMatchHome').innerHTML = handicap[6];
                document.getElementById('handicap-beforeMatchHandicapAway').innerHTML = handicap[5];
                document.getElementById('handicap-beforeMatchAway').innerHTML = handicap[7];
                break;
            case InplayOdds:
                document.getElementById('beforeMatch').style.display = 'none';
                document.getElementById('inPlay').style.display = '';
    
                document.getElementById('handicap-inPlayHandicapHome').innerHTML = `${-parseFloat(handicap[5])}`;
                document.getElementById('handicap-inPlayHome').innerHTML = handicap[6];
                document.getElementById('handicap-inPlayHandicapAway').innerHTML = handicap[5];
                document.getElementById('handicap-inPlayAway').innerHTML = handicap[7];
                break;
            default:
                break;
        }
    };

    if (preMatchAndInPlayOddsMain.overUnder.length > 0) {
        var oddsType = checkOddsType(preMatchAndInPlayOddsMain.overUnder);
        var overUnder = preMatchAndInPlayOddsMain.overUnder[0].split(',');

        document.getElementById('overUnder-initialHandicap').innerHTML = overUnder[2];
        document.getElementById('overUnder-initialOver').innerHTML = overUnder[3];
        document.getElementById('overUnder-initialUnder').innerHTML = overUnder[4];

        switch (oddsType) {
            case EarlyOdds:
            case InstantOdds:
                document.getElementById('inPlay').style.display = 'none';
                document.getElementById('beforeMatch').style.display = '';
    
                document.getElementById('overUnder-beforeMatchHandicap').innerHTML = overUnder[5];
                document.getElementById('overUnder-beforeMatchOver').innerHTML = overUnder[6];
                document.getElementById('overUnder-beforeMatchUnder').innerHTML = overUnder[7];
                
                break;
            case InplayOdds:
                document.getElementById('beforeMatch').style.display = 'none';
                document.getElementById('inPlay').style.display = '';
    
                document.getElementById('overUnder-inPlayHandicap').innerHTML = overUnder[5];
                document.getElementById('overUnder-inPlayOver').innerHTML = overUnder[6];
                document.getElementById('overUnder-inPlayUnder').innerHTML = overUnder[7];
                break;
            default:
                break;
        }
    };

    logTime("Last sync pre-match & in-play odds by match");
});

connection.on("ReceiveInitializationOddsAllMatches", function (message) {
    var count = 0;
    var table = $('#tableAllOdds').find('tbody');
    let startTime = new Date();
    for (var k in message) {
        var item = message[k];
        for (let i = 0; i < item.handicap.length/2; i++) {
            const handicap = item.handicap[i].split(",");
            table.append(`
            <tr>
             <td>${handicap[0]}</td>
             <td>${handicap[5]}</td>
            </tr>
             `);
        };
        count++;
        // var tbody = document.getElementById('tableAllOdds').getElementsByTagName('tbody')[0];
        // for (let i = 0; i < item.handicap.length; i++) {
        //     const line = item.handicap[i];
            
        // }
        // var newRow = tbody.insertRow();
        // var newCell1 = newRow.insertCell();
        // var newCell2 = newRow.insertCell();
        // var newText = document.createTextNode('new row');
        // newCell1.appendChild(newText);
        // newCell2.appendChild(newText);

        // console.log(item);
    };
    let endTime = new Date();
    let timeElapsed = endTime - startTime;
    console.log("Total time elapsed to loop " + count + " items: " + timeElapsed + " milliseconds.");
});

connection.on("ReceiveOddsChangesAllMatches", function (message) {
    logTime("Last sync pre-match & in-play odds all matches");
});

connection.on("ReceiveCornerPreMatchChanges", function (message) {
    if (message.length > 0) {
        document.getElementById('totalCorners-initialTotalCorners').innerHTML = message[0].odds.totalCorners;
        document.getElementById('totalCorners-initialOver').innerHTML = message[0].odds.over;
        document.getElementById('totalCorners-initialUnder').innerHTML = message[0].odds.under;    
        
        logTime("Last sync pre-match corners");
    };
});

connection.on("ReceiveCornerInPlayChanges", function (message) {
    if (message.length > 0) {
        var check = document.getElementById('beforeMatch').style.display;
        if (check === '') {
            document.getElementById('totalCorners-beforeMatchTotalCorners').innerHTML = message[0].odds.totalCorners;
            document.getElementById('totalCorners-beforeMatchOver').innerHTML = message[0].odds.over;
            document.getElementById('totalCorners-beforeMatchUnder').innerHTML = message[0].odds.under;
        }else{
            document.getElementById('totalCorners-inPlayTotalCorners').innerHTML = message[0].odds.totalCorners;
            document.getElementById('totalCorners-inPlayOver').innerHTML = message[0].odds.over;
            document.getElementById('totalCorners-inPlayUnder').innerHTML = message[0].odds.under;  
        };
        logTime("Last sync in-play corners");
    };
});

connection.start().then(function () {
    if (typeof matchId !== 'undefined' && matchId !== null && matchId !== '') {
        document.getElementById('allOdds').style.display = 'none';
        document.getElementById('oddsByMatch').style.display = '';

        connection.invoke("SendInitializationOddsByMatch", matchId, companyId)
                   .then(function () {})
                   .catch(function (err) {
                        return console.error(err.toString());
                    });
    }else{
        document.getElementById('allOdds').style.display = '';
        document.getElementById('oddsByMatch').style.display = 'none';

        connection.invoke("SendInitializationOddsAllMatches")
                   .then(function () {})
                   .catch(function (err) {
                        return console.error(err.toString());
                    });
    }
    // alert(connection.connectionId);
}).catch(function (err) {
    return console.error(err.toString());
});