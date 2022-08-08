"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/drinkHub").build();

connection.on("DrinkPriceUpdated", function (drink) {
    document.getElementById(`${drink.id}`).innerHTML = `${drink.symbol} : ${drink.activePrice}`;
    (drink.priceLastIncreased
        ? document.getElementById(`div_${drink.id}`).querySelector('i').className = 'fa-solid fa-arrow-up price-increase' 
        : document.getElementById(`div_${drink.id}`).querySelector('i').className = 'fa-solid fa-arrow-down price-decrease');
});

connection.on("DrinkUpdated", function (drink) {
    document.getElementById(`${id}`).innerHTML = `${drink.symbol} : ${drink.activePrice}`;
});

connection.on("ActiveStatusToggled", function (active, id) {
    (active
        ? document.getElementById(`div_${id}`).style.display = "flex"
        : document.getElementById(`div_${id}`).style.display = "none")
});

connection.start().then(function () {
    console.log("Hit!");
}).catch(function (err) {
    return console.error(err.toString());
});