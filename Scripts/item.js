if (document.readyState == 'loading') {
    document.addEventListener('DOMContentLoaded', ready)
} else {
    ready()
}

function ready() {
    var removeCartItemButtons = document.getElementsByClassName('btn-danger')
    for (var i = 0; i < removeCartItemButtons.length; i++) {
        var button = removeCartItemButtons[i]
        button.addEventListener('click', removeCartItem)
    }
    purchaseClicked
    var quantityInputs = document.getElementsByClassName('cart-quantity-input')
    for (var i = 0; i < quantityInputs.length; i++) {
        var input = quantityInputs[i]
        input.addEventListener('change', quantityChanged)
    }

    // var addToCartButtons = document.getElementsByClassName('shop-item-button')
    //   for (var i = 0; i < addToCartButtons.length; i++) {
    //      var button = addToCartButtons[i]
    //       button.addEventListener('click', addToCartClicked)
    //   }



 

    document.getElementsByClassName('btn-purchase')[0].addEventListener('click', function () {

  //      createReq();
        createReqitems();
       /*var cartItems = document.getElementsByClassName('cart-items')[0]
        var cartItemNames = cartItems.getElementsByClassName('cart-item-title')
        var quantityInputs = cartItems.getElementsByClassName('cart-quantity-input')

      

        for (var i = 0; i < cartItemNames.length; i++) {
            var f = {};
            f.url = '/Employee/OnJSON';
            f.type = "POST";
            f.dataType = "json";
            f.data = JSON.stringify({ itemName: cartItemNames[i].innerText, quantity: quantityInputs[i].value });
            f.contentType = "application/json";
            f.success = function (response) {
                alert("success");
            };
            f.error = function (response) {
                alert("failed");
            };
            $.ajax(f);
        }  */
       
    });

}

function createReqitems() {

    var cartItems = document.getElementsByClassName('cart-items')[0]
    var cartItemNames = cartItems.getElementsByClassName('cart-item-title')
    var quantityInputs = cartItems.getElementsByClassName('cart-quantity-input')


    if (cartItemNames.length == 0) {
        alert("There is no items in the requisition.");
    }

    else {

        var f = {};
        f.url = '/Employee/reqId/';
        f.type = "POST";
        f.dataType = "json";
        f.data = JSON.stringify({ username: "Sam Worthington" });
        f.contentType = "application/json";
        f.success = function (response) {



            for (var i = 0; i < cartItemNames.length; i++) {
                var g = {};
                g.url = '/Employee/OnJSON';
                g.type = "POST";
                g.dataType = "json";
                g.data = JSON.stringify({ itemName: cartItemNames[i].innerText, quantity: quantityInputs[i].value });
                g.contentType = "application/json";
                g.success = function (response) {
                    // alert("Req Items write success");
                };
                g.error = function (response) {
                    // alert("Req Items write failed");
                };
                $.ajax(g);
            }

            alert("Requisition has been sucessfully created!");

            while (cartItems.hasChildNodes()) {
                cartItems.removeChild(cartItems.firstChild)
            }

        };
        f.error = function (response) {
            alert("Requisition creation failed.Please try again!");
        };
        $.ajax(f);
    }

}



function createReq() {
    var f = {};
    f.url = '/Employee/reqId/';
    f.type = "POST";
    f.dataType = "json";
    f.data = JSON.stringify({ username: "Sam Worthington"});
    f.contentType = "application/json";
    f.success = function (response) {
        alert("success");
    };
    f.error = function (response) {
        alert("failed");
    };
    $.ajax(f);
}

function purchaseClicked() {


    var cartItems = document.getElementsByClassName('cart-items')[0];
    console.log(cartItems.length);
    alert('Requisition has been sucessfully created!')
    var cartItems = document.getElementsByClassName('cart-items')[0]
    while (cartItems.hasChildNodes()) {
        cartItems.removeChild(cartItems.firstChild)
    }

  //  alert('Request has been successfully submiited.')
  //  var cartItems = document.getElementsByClassName('cart-items')[0]
  //  var cartItemNames = cartItems.getElementsByClassName('cart-item-title')
  //  var quantityInputs = cartItems.getElementsByClassName('cart-quantity-input')

  //  for (var i = 0; i < cartItemNames.length; i++) {
  //      console.log(cartItemNames[i].innerText)
  //      var item = { itemName: cartItemNames[i].innerText };
  //      console.log(item);
  //      console.log(quantityInputs[i].value);
  //      var quan = { quantity: quantityInputs[i].value };
  ////      send(item);
  //      send(quan);

  //      var f = {};
  //      f.url = '@Url.Action("OnJSON","Employee")';
  //      f.type = "POST";
  //      f.dataType = "json";
  //      f.data = JSON.stringify({ itemName:cartItemNames[i].innerText, quantity: quantityInputs[i].value });
  //      f.contentType = "application/json";
  //      f.success = function (response) {
  //          alert("success");
  //      };
  //      f.error = function (response) {
  //          alert("failed");
  //      };
  //      $.ajax(f);
        
        
  //  }

      //  while (cartItems.hasChildNodes()) {
   //     cartItems.removeChild(cartItems.firstChild)
 //   }
  //  updateCartTotal()
}

function send(item) {
    var ajax = new XMLHttpRequest();
    ajax.onreadystatechange = function () {
        if (ajax.readyState == 4) {
            document.open();
            document.write(ajax.response);
            document.close();
        }
    }

    ajax.open("POST", "/Employee/OnJSON", true);
    ajax.setRequestHeader("Content-type", "application/json; charset=UTF-8");
    //alert(JSON.stringify(search));
    ajax.send(JSON.stringify(item));
}




function removeCartItem(event) {
    var buttonClicked = event.target
    buttonClicked.parentElement.parentElement.remove()
   // updateCartTotal()
}

function quantityChanged(event) {
    var input = event.target
    if (isNaN(input.value) || input.value <= 0) {
        input.value = 1
    }
 //   updateCartTotal()
}

function addToCartClicked(event) {

    var inputs = document.getElementsByTagName("input");
    var titles = document.getElementsByClassName('shop-item-title');
    var prices = document.getElementsByClassName('shop-item-price');
    var quantity = document.getElementsByClassName('shop-item-quantity');


    var str = ' ';
    var pos;

    for (var i = 0; i < inputs.length; i++) {
        if (inputs[i].checked == true) {
           

            var title = titles[i-3].innerText;
            console.log(title);

            var price = prices[i-3].innerText;
            console.log(price);

            var quan = quantity[i - 3].innerText;
            addItemToCart(title, price,quan);
        }
 
    }
    uncheck();
  

 //   var button = event.target
  //  var shopItem = button.parentElement.parentElement
  //  var title = shopItem.getElementsByClassName('shop-item-title')[0].innerText
 //   var price = shopItem.getElementsByClassName('shop-item-price')[0].innerText


  //  addItemToCart(title, price)
 //   updateCartTotal()
}

function uncheck() {
    var uncheck = document.getElementsByTagName('input');
    for (var i = 0; i < uncheck.length; i++) {
        if (uncheck[i].type == 'checkbox') {
            uncheck[i].checked = false;
        }
    }
}

function addToCartRecentClicked(event) {

    var inputs = document.getElementsByTagName("input");
    var titles = document.getElementsByClassName('shop-item-title');
    var prices = document.getElementsByClassName('shop-item-price');
    var quantity = document.getElementsByClassName('shop-item-quantity');

    var str = ' ';
    var pos;

    for (var i = 0; i < inputs.length; i++) {
        if (inputs[i].checked == true) {


            var title = titles[i-2].innerText;
            console.log(title);

            var price = prices[i-2].innerText;
            console.log(price);

            var quan = quantity[i-2].innerText;
            addItemToCart(title, price, quan);

            
        }

    }
    uncheck();


    //   var button = event.target
    //  var shopItem = button.parentElement.parentElement
    //  var title = shopItem.getElementsByClassName('shop-item-title')[0].innerText
    //   var price = shopItem.getElementsByClassName('shop-item-price')[0].innerText


    //  addItemToCart(title, price)
    //   updateCartTotal()
}

function uncheck() {
    var uncheck = document.getElementsByTagName('input');
    for (var i = 0; i < uncheck.length; i++) {
        if (uncheck[i].type == 'checkbox') {
            uncheck[i].checked = false;
        }
    }
}



function getValue() {

    var checks = document.getElementsByClassName("checkmark");
    var des = document.getElementsByClassName("shop-item-title");
    var inputs = document.getElementsByTagName("input");
    
    var str;

    for (var i = 0; i < inputs.length; i++) {
        if (inputs[i].checked == true) {
            str = inputs[i].value;
        }
       
    }

  

    document.getElementsByClassName('shop-item-button')[0].addEventListener('click', addToCartClicked)
   // document.getElementsByClassName('btn-purchase')[0].addEventListener('click', purchaseClicked)

}

function addItemToCart(title, price, quan) {
    var cartRow = document.createElement('div')
    cartRow.classList.add('cart-row')
    var cartItems = document.getElementsByClassName('cart-items')[0]
    var cartItemNames = cartItems.getElementsByClassName('cart-item-title')

    for (var i = 0; i < cartItemNames.length; i++) {
        if (cartItemNames[i].innerText == title) {

            alert('This item is already added to the requisition form.')
            return
        }
    }

    var cartRowContents = ` 
        <span class="cart-item-title">${title}</span>
        <span class="cart-title cart-column">${price}</span>      
        <div class="cart-quantity cart-column">
            <input class="cart-quantity-input" type="number" max="${quan}" value="1">
            <button class="btn btn-primary btn-danger" type="button">REMOVE</button>
        </div>`
    cartRow.innerHTML = cartRowContents
    cartItems.append(cartRow)
    cartRow.getElementsByClassName('btn-danger')[0].addEventListener('click', removeCartItem)
    cartRow.getElementsByClassName('cart-quantity-input')[0].addEventListener('change', quantityChanged)
}

//function updateCartTotal() {
//    var cartItemContainer = document.getElementsByClassName('cart-items')[0]
//    var cartRows = cartItemContainer.getElementsByClassName('cart-row')
//    var total = 0
//    for (var i = 0; i < cartRows.length; i++) {
//        var cartRow = cartRows[i]
//        var priceElement = cartRow.getElementsByClassName('cart-price')[0]
//        var quantityElement = cartRow.getElementsByClassName('cart-quantity-input')[0]
//        var price = parseFloat(priceElement.innerText.replace('$', ''))
//        var quantity = quantityElement.value
//        total = total + (price * quantity)
//    }
//    total = Math.round(total * 100) / 100
//    document.getElementsByClassName('cart-total-price')[0].innerText = '$' + total
//}