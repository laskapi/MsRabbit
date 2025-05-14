"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/myHub").build();


connection.on("Received", function (message) {
/*    var li = document.createElement("li");
    document.getElementById("messageList").appendChild(li);
*/
    var li = document.getElementById("myMessage");

    li.textContent = `new message: ${message}`;
});

connection.start();

