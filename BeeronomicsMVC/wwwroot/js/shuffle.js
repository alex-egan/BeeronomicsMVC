let drinks = [];

//$(document).ready(function () {
//    for (let x = 0; x < $('.flexitem').length; x++) {
//        drinks.push()
//    }
//});

var connection = new signalR.HubConnectionBuilder().withUrl("/drinkHub").build();

connection.on("DrinkPriceUpdated", function (drink) {
    let divDrink = document.getElementById(`div_${drink.id}`);
    let currentIndex = $('li').index(divDrink);
    let newIndex = 0;

    $('.active-price').each(function () {
        if (parseFloat(drink.activePrice) >= parseFloat(this.innerHTML)
            && $('li').index(this.parentElement) != currentIndex) {
            newIndex = $('li').index(this.parentElement);
            return false;
        }
        else {
            newIndex++;
        }
    });

    if (newIndex == $('li').length) {
        let li = $('li')[newIndex - 1];
        //$(divDrink).addClass('not-visible');
        //let newDivDrink = $(divDrink).clone();
        //$(li).after(newDivDrink);
        //$(divDrink).remove();
        //$(newDivDrink).removeClass('not-visible');

        let newDivDrink = $(divDrink).clone();
        $(newDivDrink).addClass('ui-effects-placeholder');
        $(newDivDrink).hide();
        $(li).after(newDivDrink);

        $(divDrink).toggle('slide', { direction: 'left' }, 500, function () {
            $(newDivDrink).toggle('slide', { direction: 'left' }, 500);
            $(divDrink).remove();
            document.getElementById(`div_${drink.id}`).getElementsByClassName('active-price')[0].innerHTML = `${drink.activePrice.toFixed(2)}`;
            (drink.priceLastIncreased
                ? document.getElementById(`div_${drink.id}`).querySelector('i').className = 'fa-solid fa-arrow-up price-increase'
                : document.getElementById(`div_${drink.id}`).querySelector('i').className = 'fa-solid fa-arrow-down price-decrease');
        });

        //$(divDrink).toggle('slide', { direction: 'left' }, 250).promise().pipe(function () {
        //    $(newDivDrink).toggle('slide', { direction: 'left' }, 250);
        //    $(divDrink).remove();
        //    document.getElementById(`div_${drink.id}`).getElementsByClassName('active-price')[0].innerHTML = `${drink.activePrice.toFixed(2)}`;
        //    (drink.priceLastIncreased
        //        ? document.getElementById(`div_${drink.id}`).querySelector('i').className = 'fa-solid fa-arrow-up price-increase'
        //        : document.getElementById(`div_${drink.id}`).querySelector('i').className = 'fa-solid fa-arrow-down price-decrease');
        //});

    }
    else {
        //$(divDrink).css('visibility', 'hidden');
        //let newDivDrink = $(divDrink).clone();
        //$(li).after(newDivDrink);
        //$(divDrink).remove();
        //$(newDivDrink).css('visibility', 'visible');

        let li = $('li')[newIndex];
        let newDivDrink = $(divDrink).clone();
        $(newDivDrink).addClass('ui-effects-placeholder');
        $(newDivDrink).hide();
        $(li).before(newDivDrink);

        $(divDrink).toggle('slide', { direction: 'left' }, 500, function () {
            $(newDivDrink).toggle('slide', { direction: 'left' }, 500);
            $(divDrink).remove();
            document.getElementById(`div_${drink.id}`).getElementsByClassName('active-price')[0].innerHTML = `${drink.activePrice.toFixed(2)}`;
            (drink.priceLastIncreased
                ? document.getElementById(`div_${drink.id}`).querySelector('i').className = 'fa-solid fa-arrow-up price-increase'
                : document.getElementById(`div_${drink.id}`).querySelector('i').className = 'fa-solid fa-arrow-down price-decrease');
        });

        //$(divDrink).toggle('slide', { direction: 'left' }, 250).promise().pipe(function () {
        //    $(newDivDrink).toggle('slide', { direction: 'left' }, 250);
        //    $(divDrink).remove();
        //    document.getElementById(`div_${drink.id}`).getElementsByClassName('active-price')[0].innerHTML = `${drink.activePrice.toFixed(2)}`;
        //    (drink.priceLastIncreased
        //        ? document.getElementById(`div_${drink.id}`).querySelector('i').className = 'fa-solid fa-arrow-up price-increase'
        //        : document.getElementById(`div_${drink.id}`).querySelector('i').className = 'fa-solid fa-arrow-down price-decrease');
        //});
    }


    //$(divDrink).slideToggle(100, function () {
        //    $(li).after(divDrink);
        //    $(divDrink).slideToggle(500);
        //});

    //$(divDrink).slideToggle(100, function () {
        //    $(li).after(divDrink);
        //    $(divDrink).slideToggle(500);
        //});
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
        document.getElementById(`div_${drinks[x].id}`).getElementsByClassName('active-price')[0].innerHTML = `${drinks[x].activePrice.toFixed(2)}`;
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