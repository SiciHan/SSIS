if (document.readyState == 'loading') {
    document.addEventListener('DOMContentLoaded', ready)
} else {
    ready()
}

function ready() {


    document.getElementsByClassName('btn-search')[0].addEventListener('click', function () {



        var startDate = document.getElementsByClassName('start-date')[0].value;
        var endDate = document.getElementsByClassName('end-date')[0].value;
        var e = document.getElementById("mySelect");
        var strSelected = e.options[e.selectedIndex].value;

        //     console.log(strSelected);
        //     console.log(startDate);
        //     console.log(endDate);
        $('#table1 tbody').empty();
        searchReqHistory();


    });

}


function searchReqHistory() {

    var current = $('<tbody class="tItems">')
    var startDate = document.getElementsByClassName('start-date')[0].value;
    var endDate = document.getElementsByClassName('end-date')[0].value;
    var e = document.getElementById("mySelect");
    var status = e.options[e.selectedIndex].value;

    console.log(startDate);
    console.log(endDate);
    console.log(status);


    var f = {};
    f.url = '/Employee/searchReqHistory/';
    f.type = "POST";
    f.dataType = "json";
    f.data = JSON.stringify({ startDate: startDate, endDate: endDate, status: status });
    f.contentType = "application/json";
    f.success = function (response) {
        //Req
        console.log(response);
        console.log(response.Req.length);

        if (response.Req.length == 0) {
            alert("There is no requisiton items within chosen criteria.Please choose again!")
        }

        for (var i = 0; i < response.Req.length; i++) {


            var json = parseJsonDate(response.Req[i].RaiseDate);
            let date = JSON.stringify(json);
            date = date.slice(1, 11);
            if (response.Req[i].HeadRemark == null) {
                response.Req[i].HeadRemark = "-";
            }

            switch (response.Req[i].IdStatusCurrent) {
                case 1:
                    response.Req[i].IdStatusCurrent = "Incomplete"
                    break; 
                case 2:
                    response.Req[i].IdStatusCurrent = "Pending"
                    break; 
                case 3:
                    response.Req[i].IdStatusCurrent = "Approved"
                    break;
                case 4:
                    response.Req[i].IdStatusCurrent = "Rejected"
                    break;
                case 5:
                    response.Req[i].IdStatusCurrent = "Cancelled"
                    break;
                case 6:
                    response.Req[i].IdStatusCurrent = "Delivered"
                    break;
                case 7:
                    response.Req[i].IdStatusCurrent = "Disbursed"
                    break;
                case 8:
                    response.Req[i].IdStatusCurrent = "Preparing"
                    break;
                case 9:
                    response.Req[i].IdStatusCurrent = "Prepared"
                    break;
                case  10:
                    response.Req[i].IdStatusCurrent = "Scheduled"
                    break;
                case 11:
                    response.Req[i].IdStatusCurrent = "Received"
                    break;


            }
          

            current = $('<tr/>');
            current.append("<td>" + '<a href ="https://localhost:44304/Employee/Update">' + response.Req[i].IdRequisition + "</td>");
            current.append("<td>" + response.Req[i].IdStatusCurrent + "</td>");
            current.append("<td>" + '<span class="cart-item-title" value=' + i + '>' + date + '</span>' + "</td>");
            current.append("<td>" + response.Req[i].HeadRemark + "</td>");

            $('table').append(current);




            //  console.log(response[i].IdReqItem );
        }
        console.log("History Read success");


    };
    f.error = function (response) {
        console.log("History Read failed");
    };
    $.ajax(f);


}




function parseJsonDate(jsonDateString) {
    return new Date(parseInt(jsonDateString.replace('/Date(', '')));
}

