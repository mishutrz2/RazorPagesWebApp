"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

// Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;


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

connection.on("UserJoined", function (user) {
    console.log(user + " has joined");
    const userJoinedElement = document.createElement('li');
    userJoinedElement.innerHTML = `${user}`;
    
    var playerAlreadyOnTheList = false;
    // Check if the player already exists in the list
    var listItems = document.getElementById("connectedPlayersList").getElementsByTagName('li');
    for (var i = 0; i < listItems.length; i++) {
        if (listItems[i].textContent.trim() === user.trim()) {
            playerAlreadyOnTheList = true;
            break;
        }
    }

    if (!playerAlreadyOnTheList) {
        document.getElementById("connectedPlayersList").appendChild(userJoinedElement);
    }
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
        connection.invoke("JoinSession", sessionId, user)
            .catch(err => console.error(err));

        // Get user avatar
        fetch('https://source.unsplash.com/random/100x100?face')
            .then(data => { img_url = data.url })

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


connection.on("UpdateTopListAndTeams", (captainId, chosenPlayerName, nextUserOrder) => {

    // add item to team lists
    var teamListId = 'captain' + captainId + 'List';
    const playerAddedLiElement = document.createElement('li');
    playerAddedLiElement.innerHTML = `${chosenPlayerName}`;

    var nextUserOrderTeamlistId = 'team' + nextUserOrder + "Avatar";

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

    document.getElementById("team1Avatar").classList.remove("currentlyChoosingTeam");
    document.getElementById("team2Avatar").classList.remove("currentlyChoosingTeam");
    document.getElementById("team3Avatar").classList.remove("currentlyChoosingTeam");
    document.getElementById(nextUserOrderTeamlistId).classList.add("currentlyChoosingTeam");

    

    currentTurn++;
});




document.getElementById('topList').addEventListener('click', function (event) {
    if (event.target.tagName === 'LI' && unlockedTopList && currentTurnSchedule[currentTurn] === currentUser) {
        var chosenPlayer = event.target.textContent.trim();
        var nextOneChoosingOrder = currentTurnSchedule[currentTurn + 1];

        var confirmed = window.confirm(`Are you sure you want to choose ${chosenPlayer}?`);

        // Check if the user clicked OK
        if (confirmed) {
            // Perform the action
            // Add your logic here
            console.log("Action performed!");
            connection.invoke("ChoosePlayer", sessionId, user, currentUser, chosenPlayer, nextOneChoosingOrder).catch(function (err) {
                return console.error(err.toString());
            });
        } else {
            // User clicked Cancel or closed the dialog
            console.log("Action canceled!");
        }
    }
});


window.addEventListener("beforeunload", function (event) {
    connection.invoke("LeaveChat", sessionId, user).catch(function (err) {
        return console.error(err.toString());
    });
});


connection.on("RemoveFromConnectedList", function (user) {
    var list = document.getElementById("connectedPlayersList");
    var items = list.getElementsByTagName("li");
    for (var i = 0; i < items.length; i++) {
        if (items[i].textContent.trim() === user) {
            list.removeChild(items[i]);
            break; // Exit the loop after removing the first matching item
        }
    }
});