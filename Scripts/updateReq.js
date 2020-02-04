var previous = null;
var current = null;
function populateTable() {

    $.get("UpdatereqId",
        //      $.getJSON("UpdatereqId",
        function (json) {
            current = JSON.stringify(json);
            console.log(current);
            //Append each row to html table
            for (var i = 0; i < json.length; i++) {

                current = $('<tr/>');
                current.append("<td>" + json[i].IdReqItem + "</td>");
                current.append("<td>" + json[i].IdRequisition + "</td>");
                current.append("<td>" + json[i].IdItem + "</td>");
                current.append("<td>" + json[i].Unit + "</td>");
                $('table').append(current);
            }

        });
}


function getJson() {



    //var results = document.getElementById("results");
    //var hr = new XMLHttpRequest();
    //hr.open("GET", "/Employee/UpdatereqId", true);
    //hr.setRequestHeader("Content-type", "application/json", true);
    //hr.onreadystatechange = function () {
    //    if (hr.readyState == 4 && hr.status == 200) {
    //        var data = JSON.parse(hr.responseText);
    //        results.innerHTML = "";
    //        for (var obj in data) {
    //            results.innerHTML += data[obj].IdReqItem;
    //        }
    //    }
    //}

    //hr.send(null);
    //results.innerHTML = "requesting...";


}


function sendJson(selectedId) {
    //   $("select.req").change(function () {
    //  var selectedId = $(this).children("option:selected").val();
    var current = $('<tbody class="tItems">')
    var g = {};
    g.url = '/Employee/UpdatereqId';
    g.type = "POST";
    g.dataType = "json";
    g.data = JSON.stringify({ reqID: selectedId });
    g.contentType = "application/json";
    g.success = function (response) {
        console.log(response);
        console.log(response.status);
        console.log(response.descrip);


        if (response.status != "Approved" && response.status != "Cancelled") {

            for (var i = 0; i < response.Items.length; i++) {


                current = $('<tr/>');
                current.append("<td>" + '<span class="req-item-id">' + response.Items[i].IdReqItem + "</td>");
                current.append("<td>" + response.Items[i].IdItem + "</td>");
                current.append("<td>" + '<span class="cart-item-title">' + response.descrip[i] + '</span>' + "</td>");
                current.append("<td>" + '<input class="cart-quantity-input" type="number" value=' + response.Items[i].Unit + '><button class="btn btn-danger" type="button" onclick="removeCartItem()">REMOVE</button>' + "</td>");

                $('table').append(current);



                //  console.log(response[i].IdReqItem );
            }

            document.getElementById("delete").style.visibility = 'visible';
            document.getElementById("submit").style.visibility = 'visible';
            document.getElementById("add").style.visibility = 'visible';


            //     var btn = document.createElement("BUTTON");
            //     btn.innerHTML = "Add Item"
            //     document.body.appendChild(btn);
        }

        else {
            for (var i = 0; i < response.Items.length; i++) {


                current = $('<tr/>');
                current.append("<td>" + response.Items[i].IdReqItem + "</td>");
                current.append("<td>" + response.Items[i].IdItem + "</td>");
                current.append("<td>" + response.descrip[i] + "</td>");
                current.append("<td>" + response.Items[i].Unit + "</td>");
                $('table').append(current);


                //  console.log(response[i].IdReqItem );
            }

            document.getElementById("delete").style.visibility = 'hidden';
            document.getElementById("submit").style.visibility = 'hidden';
            document.getElementById("add").style.visibility = 'hidden';


        }

        document.getElementsByClassName('remark')[0].innerText = response.Req.HeadRemark;
        document.getElementsByClassName('status')[0].innerText = response.status;
        console.log("Req Id write success");
    };
    g.error = function (response) {
        console.log("Req Id write failed");
    };
    $.ajax(g);
    console.log(g);
}




function clearTableData() {
    $("select.req").change(function () {
        console.log("It change!")
        $('#table tbody').empty();
        var selectedId = $(this).children("option:selected").val();
        console.log(selectedId);


    });
}
function newPopup(url) {
    popupWindow = window.open(
        url, 'popUpWindow',
        'height=600,width=800,left=50,top=10,resizable=yes,scrollbars=yes,toolbar=yes,menubar=no,location=no,directories=no,status=yes')
}

function UpdateReqitems(selectedId) {

    var cartItems = document.getElementsByClassName('tItems')[0]
    var cartItemNames = cartItems.getElementsByClassName('cart-item-title')
    var quantityInputs = cartItems.getElementsByClassName('cart-quantity-input')
    var cartRedIds = cartItems.getElementsByClassName('req-item-id');

    if (cartItemNames.length > 0) {
        //var f = {};
        //f.url = '/Employee/updateReq';
        //f.type = "POST";
        //f.dataType = "json";
        //f.data = JSON.stringify({ username: "Sam Worthington", selectedId: selectedId });
        //console.log(selectedId);
        //f.contentType = "application/json";
        //f.success = function (response) {
        for (var i = 0; i < cartItemNames.length; i++) {
            if (cartRedIds[i].innerText == "-") {

                var gg = {};
                gg.url = '/Employee/insertReq';
                gg.type = "POST";
                gg.dataType = "json";
                gg.data = JSON.stringify({ username: "Sam Worthington", selectedId: selectedId, itemName: cartItemNames[i].innerText, quantity: quantityInputs[i].value });

                gg.contentType = "application/json";
                gg.success = function (response) {
                    alert("Update New Req Items write success");
                };
                gg.error = function (response) {
                    alert("Update New Req Items write failed");
                };
                $.ajax(gg);

            }
        }
        for (var i = 0; i < cartItemNames.length; i++) {


            var g = {};
            g.url = '/Employee/updateReq';
            g.type = "POST";
            g.dataType = "json";
            g.data = JSON.stringify({ username: "Sam Worthington", selectedId: selectedId, itemName: cartItemNames[i].innerText, quantity: quantityInputs[i].value });
            console.log(cartItemNames[i].innerText);
            console.log(quantityInputs[i].value);
            g.contentType = "application/json";
            g.success = function (response) {
                alert("Update Old Req Items write success");
            };
            g.error = function (response) {
                alert("Update Old Req Items write failed");
            };
            $.ajax(g);
        }
    }
    else {
        alert("There is no stationery requests left in the form.\n Do you want to delete the reqest?");
    }

    //    alert("Red Id Write success");

    //    // while (cartItems.hasChildNodes()) {
    //    //   cartItems.removeChild(cartItems.firstChild)
    //    //}

    //};
    //f.error = function (response) {
    //    alert("Red Id Write failed");
    //};
    //$.ajax(f);


}

function DeleteReqitems(selectedId) {

    var cartItems = document.getElementsByClassName('tItems')[0]
    var cartItemNames = cartItems.getElementsByClassName('cart-item-title')
    var quantityInputs = cartItems.getElementsByClassName('cart-quantity-input')

    //var f = {};
    //f.url = '/Employee/updateReq';
    //f.type = "POST";
    //f.dataType = "json";
    //f.data = JSON.stringify({ username: "Sam Worthington", selectedId: selectedId });
    //console.log(selectedId);
    //f.contentType = "application/json";
    //f.success = function (response) {

    var g = {};
    g.url = '/Employee/deleteReq';
    g.type = "POST";
    g.dataType = "json";
    g.data = JSON.stringify({ username: "Sam Worthington", selectedId: selectedId });

    g.contentType = "application/json";
    g.success = function (response) {
        alert("Req Items delete success");
    };
    g.error = function (response) {
        alert("Req Items delete failed");
    };
    $.ajax(g);


    //    alert("Red Id Write success");

    //    // while (cartItems.hasChildNodes()) {
    //    //   cartItems.removeChild(cartItems.firstChild)
    //    //}

    //};
    //f.error = function (response) {
    //    alert("Red Id Write failed");
    //};
    //$.ajax(f);


}


function removeCartItem() {
    var buttonClicked = document.getElementsByClassName('btn-danger')[0];
    buttonClicked.parentElement.parentElement.remove()
    //updateCartTotal()
}




$(document).ready(function () {
    $("select.req").change(function () {
        var selectedId = $(this).children("option:selected").val();
        console.log(selectedId);
        sendJson(selectedId);
        clearTableData();
        //   populateTable();

        //  getJsonData();

        document.getElementsByClassName("delete-req-button")[0].addEventListener("click", function (event) {
            DeleteReqitems(selectedId);


            // event.target.setAttribute("href", "/Update?cmd=delete&reqID=" + selectedId);

        });


        document.getElementsByClassName('update-item-button')[0].addEventListener('click', function () {
            UpdateReqitems(selectedId);
            //   window.location.href = "https://localhost:44304/Employee/Update?cmd=update&reqID=" + selectedId;

        });

        document.getElementsByClassName('add-item-button')[0].addEventListener('click', function () {

            //   window.location.href = "https://localhost:44304/Employee/Update?cmd=update&reqID=" + selectedId;

        });






    });




});