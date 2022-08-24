"use strict";
var charts = {};

$(document).ready(function () {
    $(".drink-display").each(function () {
        let id = this.querySelector("span").id;

        $.ajax({
            url: `${getChartDataUrl}/${id}`,
            context: document.body,
            data: id
        }).done(function (purchaseHistories) {
            if (purchaseHistories.length > 0) {
                let vals = [];
                for (let x = purchaseHistories.length - 1; x >= 0; x--) {
                    vals.push(purchaseHistories[x].activePrice.toFixed(2));
                }

                var chart = new Chart(document.getElementById(`chart_${id}`).getContext('2d'), {
                    type: 'line',
                    data: {
                        labels: ["", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""],
                        datasets: [{
                            data: vals,
                            label: "Active Price",
                            borderColor: "#3e95cd",
                            fill: false,
                            tension: 0.5,
                            pointRadius: 0
                        }]
                    },
                    options: {
                        title: {
                            display: true,
                            text: 'Active Price over the last minute.'
                        },
                        scales: {
                            x: {
                                grid: {
                                    display: false
                                }
                            },
                            y: {
                                grid: {
                                    display: false
                                },
                                display: true,
                                ticks: {
                                    beginAtZero: false,
                                    steps: 10,
                                    stepValue: 5,
                                    min: 2,
                                    max: 8
                                }
                            }
                        },
                    }
                });

                charts[chart.canvas.id] = chart;
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

    let chart = charts[`chart_${drink.id}`];
    chart.data.datasets[0].data.shift();
    chart.data.datasets[0].data.push(drink.activePrice);
    chart.update();
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

        let chart = charts[`chart_${drinks[x].id}`];
        chart.data.datasets[0].data.shift();
        chart.data.datasets[0].data.push(drinks[x].activePrice);
        chart.update();
    }
});

connection.start().then(function () {
    console.log("Hit!");
}).catch(function (err) {
    return console.error(err.toString());
});