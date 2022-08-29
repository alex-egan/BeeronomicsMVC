var colors = [];
var chart = null;

$(document).ready(function () {
    $.ajax({
        url: `${getBigChartDataUrl}`
    }).done(function (purchaseHistories) {
        if (Object.keys(purchaseHistories).length > 0) {

            let datasets = [];

            for (let x = 0; x < Object.keys(purchaseHistories).length; x++) {
                let colorNo1 = getRandomInt(256);
                let colorNo2 = getRandomInt(256);
                let colorNo3 = getRandomInt(256);

                let randomColor = `rgba(${colorNo1}, ${colorNo2}, ${colorNo3}, 1)`;
                let drinkName = Object.keys(purchaseHistories)[x];

                let dataset = {
                    label: drinkName,
                    data: purchaseHistories[drinkName],
                    tension: 0.1,
                    fill: false,
                    borderColor: randomColor,
                    pointRadius: 0,
                };

                datasets.push(dataset);
            }

            chart = new Chart(document.getElementById(`chart`).getContext('2d'), {
                type: 'line',
                data: {
                    labels: ["", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""],
                    datasets: datasets
                },
                options: {
                    plugins: {
                        legend: {
                            labels: {
                                color: "white"
                            }
                        },
                    },
                    scales: {
                        x: {
                            grid: {
                                display: false
                            },
                            ticks: {
                                color: "white",
                                beginAtZero: false,
                                steps: 10,
                                stepValue: 5,
                                min: 2,
                                max: 8
                            }
                        },
                        y: {
                            grid: {
                                display: false
                            },
                            display: true,
                            ticks: {
                                color: "white",
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
        }
    });
});

function getRandomInt(max) {
    return Math.floor(Math.random() * max);
}

var connection = new signalR.HubConnectionBuilder().withUrl("/drinkHub").build();

connection.on("DrinkPriceUpdated", function (drink) {
    let datasetID = parseInt(drink.id) - 1;
    chart.data.datasets[datasetID].data.shift();
    chart.data.datasets[datasetID].data.push(drink.activePrice);
    chart.update();
});

connection.on("CrashActionInitiated", function (drinks) {
    for (var x = 0; x < drinks.length; x++) {
        chart.data.datasets[x].data.shift();
        chart.data.datasets[x].data.push(drinks[x].activePrice);
        chart.update();
    }
});

connection.start().then(function () {
    console.log("Hit!");
}).catch(function (err) {
    return console.error(err.toString());
});