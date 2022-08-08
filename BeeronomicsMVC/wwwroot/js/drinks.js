﻿"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/drinkHub").build();

connection.on("DrinkPriceUpdated", function (drink) {
    document.getElementById(`drink_${drink.id}`).querySelector('td.active-price').innerHTML = drink.activePrice;
});


connection.on("DrinkUpdated", function (drink) {
    document.getElementById(`drink_${drink.id}`).querySelector('td.photo img').src = drink.photo;
    document.getElementById(`drink_${drink.id}`).querySelector('td.symbol').innerHTML = drink.symbol;
    document.getElementById(`drink_${drink.id}`).querySelector('td.name').innerHTML = drink.name;
    document.getElementById(`drink_${drink.id}`).querySelector('td.active-price').innerHTML = drink.activePrice;
    document.getElementById(`drink_${drink.id}`).querySelector('td.active').innerHTML = (active ? "Yes" : "No");
});

connection.on("ActiveStatusToggled", function (active, id) {
    document.getElementById(`drink_${id}`).querySelector('td.active').innerHTML = (active ? "Yes" : "No");
    document.getElementById(`drink_${id}`).querySelector('td.buttons .btn-deactivate').innerHTML = (active ? '<i class="fa-solid fa-ban"></i>' : '<i class="fa-solid fa-plus"></i>');
});

connection.start().then(function () {
    console.log("Hit!");
}).catch(function (err) {
    return console.error(err.toString());
});

function purchaseDrink(id) {
    connection.invoke("UpdateDrinkPrice", id).catch(function (err) {
        return console.error(err.toString());
    });

    event.preventDefault();
}

function editDrink(id) {
    location.replace(`Drinks/Edit/${id}`);
}

function toggleActiveStatus(id) {
    connection.invoke("ToggleActiveStatus", id).catch(function (err) {
        return console.error(err.toString());
    })

    event.preventDefault();
}