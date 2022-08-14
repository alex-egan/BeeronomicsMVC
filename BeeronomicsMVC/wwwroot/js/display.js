"use strict";
var charts = [];

$(document).ready(function () {
    $(".drink-display").each(function () {
        let id = this.querySelector("span").id;

        $.ajax({
            url: `${getChartDataUrl}/${id}`,
            context: document.body,
            data: id
        }).done(function (purchaseHistories) {
            if (purchaseHistories.length > 0) {
                let data = [];
                for (let x = 0; x < purchaseHistories.length; x++) {
                    data.push(purchaseHistories[x].activePrice.toFixed(2));
                }

                //Charts are not showing the line itself, but they are displaying fine.
                const ctx = this.querySelector('canvas').getContext('2d');
                const chart = new Chart(ctx, {
                    type: 'line',
                    data: {
                        datasets: [{
                            label: 'Last 10',
                            data: data,
                            fill: true,
                            backgroundColor: 'black',
                            tension: 0.5
                        }]
                    },
                    options: {
                        scales: {
                            y: {
                                beginAtZero: true
                            }
                        }
                    }
                });

                charts.push(chart);
            }
        });
    });
});

var connection = new signalR.HubConnectionBuilder().withUrl("/drinkHub").build();

connection.on("DrinkPriceUpdated", function (drink) {
    document.getElementById(`${drink.id}`).innerHTML = `${drink.symbol} : ${drink.activePrice.toFixed(2)}`;
    (drink.priceLastIncreased
        ? document.getElementById(`div_${drink.id}`).querySelector('i').className = 'fa-solid fa-arrow-up price-increase' 
        : document.getElementById(`div_${drink.id}`).querySelector('i').className = 'fa-solid fa-arrow-down price-decrease');
});

connection.on("DrinkUpdated", function (drink) {
    document.getElementById(`${drink.id}`).innerHTML = `${drink.symbol} : ${drink.activePrice.toFixed(2)}`;
});

connection.on("ActiveStatusToggled", function (active, id) {
    (active
        ? document.getElementById(`div_${id}`).style.display = "flex"
        : document.getElementById(`div_${id}`).style.display = "none")
});

connection.on("CrashActionInitiated", function (drinks) {
    for (var x = 0; x < drinks.length; x++) {
        document.getElementById(`${drinks[x].id}`).innerHTML = `${drinks[x].symbol} : ${drinks[x].activePrice.toFixed(2)}`;
        (drinks[x].priceLastIncreased
            ? document.getElementById(`div_${drinks[x].id}`).querySelector('i').className = 'fa-solid fa-arrow-up price-increase'
            : document.getElementById(`div_${drinks[x].id}`).querySelector('i').className = 'fa-solid fa-arrow-down price-decrease');
    }
});

connection.start().then(function () {
    console.log("Hit!");
}).catch(function (err) {
    return console.error(err.toString());
});