"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

// Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

// Get user avatar
var img_url = 'https://upload.wikimedia.org/wikipedia/commons/f/fb/Small_loadingspinner.svg';
fetch('https://source.unsplash.com/random/100x100?face')
    .then(data => { img_url = data.url })

// Function for scrolling to the bottom of the chat
function scrollToBottom() {
    const chatParentContainer = document.getElementById('messagesListParent');
    chatParentContainer.scrollTop = chatParentContainer.scrollHeight;
}

connection.on("ReceiveMessage", function (user, message, received_img_url) {
    const chat = document.getElementById('messagesList');

    const messageElement = document.createElement('div');
    messageElement.classList.add('flex', 'items-end', 'space-x-4');
    messageElement.innerHTML = `
        <div class="flex-shrink-0 h-14 w-14">
          <img class="h-14 w-14 rounded-full" src="${received_img_url}" alt="Avatar">
        </div>
        <div class="ml-4">
          <div class="text-lg text-gray-400">${user}</div>
          <div class="mt-1 text-base text-gray-800">${message}</div>
        </div>
      `;
    chat.appendChild(messageElement);

    // this is for automatically scrolling to the bottom of the chat
    scrollToBottom();
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
    if (!started) {
        connection.invoke("JoinSession", sessionId, user)
            .catch(err => console.error(err));
        started = true;
    }
}).catch(function (err) {
    return console.error(err.toString());
});


document.getElementById('sendButton').addEventListener('click', function (event) {
    const messageInput = document.getElementById('messageInput');
    const chat = document.getElementById('messagesList');
    const messageText = messageInput.value.trim();

    if (messageText) {
        connection.invoke("SendMessage", sessionId, user, messageText, img_url).catch(function (err) {
            return console.error(err.toString());
        });
        event.preventDefault();

        const messageElement = document.createElement('div');
        messageElement.classList.add('flex', 'items-end', 'space-x-4');
        messageElement.innerHTML = `
                        <div class="flex-shrink-0 h-14 w-14">
                          <img class="h-14 w-14 rounded-full" src="${img_url}" alt="Avatar">
                        </div>
                        <div class="ml-4">
                          <div class="text-lg text-gray-400">you</div>
                          <div class="mt-1 text-base text-gray-800">${messageText}</div>
                        </div>
                      `;
        chat.appendChild(messageElement);
        messageInput.value = '';

        // this is for automatically scrolling to the bottom of the chat
        scrollToBottom();
    }
});

connection.on("UnlockTopList", () => {
    unlockedTopList = true;
});

//////////////////////////////////////////////////////////////////////////////////////////////

connection.on("UpdateTopListAndTeams", (captainId, chosenPlayerName) => {

    // add item to team lists
    //var parrentTeamListId = 'team' + captainId + 'Avatar';
    var teamListId = 'captain' + captainId + 'List';
    const playerAddedLiElement = document.createElement('li');
    playerAddedLiElement.innerHTML = `${chosenPlayerName}`;

    var playerAlreadyExists = false;
    // Check if the player already exists in the list
    var listItems = document.getElementById(teamListId).getElementsByTagName('li');
    for (var i = 0; i < listItems.length; i++) {
        if (listItems[i].textContent.trim() === chosenPlayerName.trim()) {
            playerAlreadyExists = true;
            break;
        }
    }

    if (!playerAlreadyExists) {
        document.getElementById(teamListId).append(playerAddedLiElement);
    }

    // remove item from common list
    Array.from(document.getElementById('topList').getElementsByTagName('li')).forEach(function (listItem) {
        if (listItem.innerText.trim() === chosenPlayerName.trim()) {
            listItem.remove();
        }
    });

    currentTurn++;

    document.getElementById('team1Avatar').classList.remove('makeItGreen');
    document.getElementById('team2Avatar').classList.remove('makeItGreen');
    document.getElementById('team3Avatar').classList.remove('makeItGreen');
    if (currentTurnSchedule[currentTurn] === currentUser) {
        document.getElementById(parrentTeamListId).classList.add('makeItGreen');
    }
});




document.getElementById('topList').addEventListener('click', function (event) {
    if (event.target.tagName === 'LI' && unlockedTopList && currentTurnSchedule[currentTurn] === currentUser) {
        var chosenPlayer = event.target.textContent.trim();
        connection.invoke("ChoosePlayer", sessionId, user, currentUser, chosenPlayer).catch(function (err) {
            return console.error(err.toString());
        });
    }
});






/*const captain1List = document.getElementById('captain1List');
const captain2List = document.getElementById('captain2List');
const captain3List = document.getElementById('captain3List');

let currentCaptain = 1;
let i = 1;

document.getElementById('topList').addEventListener('click', function (event) {
    if (event.target.tagName === 'LI') {
        const element = event.target;

        const orderOfChosing = "123321123321";

        switch (currentCaptain) {
            case 1:
                captain1List.appendChild(element.cloneNode(true));
                var orderIntValue = parseInt(orderOfChosing.charAt(i), 10);
                i++;
                currentCaptain = orderIntValue;
                break;
            case 2:
                captain2List.appendChild(element.cloneNode(true));
                var orderIntValue = parseInt(orderOfChosing.charAt(i), 10);
                i++;
                currentCaptain = orderIntValue;
                break;
            case 3:
                captain3List.appendChild(element.cloneNode(true));
                var orderIntValue = parseInt(orderOfChosing.charAt(i), 10);
                i++;
                currentCaptain = orderIntValue;
                break;
        }
        element.parentNode.removeChild(element);
    }
});*/