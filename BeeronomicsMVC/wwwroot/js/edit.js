"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/drinkHub").build();

connection.start().then(function () {
    console.log("Hit!");
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("update").addEventListener("click", function (event) {
    let drink = Object.fromEntries(new FormData(document.querySelector('form')).entries());
    connection.invoke("UpdateDrink", drink).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

connection.on("ActiveStatusToggled", function (active) {
    document.getElementById(`active`).value = (active ? "Yes" : "No");
});