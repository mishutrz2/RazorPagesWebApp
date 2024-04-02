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

    connection.invoke("JoinSession", sessionId)
        .catch(err => console.error(err));

}).catch(function (err) {
    return console.error(err.toString());
});

// Event handler for when a user joins the session
connection.on("UserJoined", (connectionId) => {
    console.log("User joined:", connectionId);
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