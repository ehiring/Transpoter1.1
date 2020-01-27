
var app = angular.module('myApp2', []);
app.controller('CarlistCntl2', ['$scope', '$http', '$sce', '$compile', function ($scope, $http, $sce, $compile) {


    $scope.CheckSessionExp = function (value) {
        if (value == "SEXP") {
            location.href = "/SessionExpired";
        }
    }


    $scope.outtimeChange = function () {

        $scope.getOutResultlatest = [];
        $scope.tariffId = "";
        $.ajax({
            type: "GET",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            url: "../Scripts/Js/Transfer/time.js",
            async: false,
            success: function (response) {
                $scope.CheckSessionExp(response);
                var transDate = $('#txtDate').datepicker("getDate");
                var currDate = new Date();
                if (transDate.toLocaleDateString() != currDate.toLocaleDateString()) {
                    $scope.getOutResultlatest = response;
                }
                else {
                    var arr = [];
                    response.forEach(function (obj) {
                        var time = obj.Time.split(' ')[0];
                        var hr = time.split(':')[0];
                        var currhr = currDate.getHours();
                        if (parseInt(hr) > parseInt(currhr + 4)) {
                            arr.push(obj);
                        }
                    });
                    $scope.getOutResultlatest = arr;
                }
            },
            error: function (response) {
            }
        });
    }

    $("#txtDate").datepicker({
        minDate: -0,
        numberOfMonths: 1,
        showButtonPanel: true,
        dateFormat: 'dd/mm/yy',
        selectDefaultDate: true,
        onSelect: function (date) {
            $("#txtReturnDate").val("");
            $("#txtReturnDate").datepicker("option", "minDate", date);
            $scope.outtimeChange();
            $scope.$apply();
        }
    }).datepicker("setDate", new Date());


    $("#txtReturnDate").datepicker({
        minDate: -0,
        showButtonPanel: true,
        //maxDate: new Date(),
        numberOfMonths: 1,
        selectDefaultDate: true,
        dateFormat: 'dd/mm/yy'
    }).datepicker("setDate", new Date());


    $scope.outtimeChange();

    $scope.validate = function () {
        debugger;
        var result = true;

        if ($('#hdnTravelType').val() == "1") {
            if ($("#ddlSource").val() == "") {
                alert("Select Source City");
                result = false;
                return false;
            }

            if ($("#ddlDestination").val() == "") {
                alert("Select Destination City");
                result = false;
                return false;
            }


            if ($("#hdnTripType").val() == "1" || $("#hdnTripType").val() == "3") {
                if ($("#txtReturnDate").val() == "") {
                    alert("Select Return Date");
                    result = false;
                    return false;
                }
            }
        }

        if ($('#hdnTravelType').val() == "3") {

            if ($("#inpfromTransfer").val() == "") {
                alert("Select Source Address");
                result = false;
                return false;
            }

            if ($("#inpToTransfer").val() == "") {
                alert("Select Destination Address");
                result = false;
                return false;
            }

        }

        if ($('#hdnTravelType').val() == "2") {
            if ($("#inplocalTransfer").val() == "") {
                alert("Select city");
                result = false;
                return false;
            }
        }
        if ($("#txtDate").val() == "") {
            alert("Select Travel Date");
            result = false;
            //  return false;
        }

        if ($("#txtTime").val() == "") {
            alert("Select Pick-Up");
            result = false;
            return false;
        }
        if (result == true) $("#formTransfer").submit();
        //  return true;    
    }


    $scope.GetList = function (from, to) {
        document.getElementById("ddlSource").value = from;
        document.getElementById("ddlDestination").value = to;
        var todayDate = new Date();
        todayDate.setDate(todayDate.getDate() + 3);
        $('#txtDate').datepicker('setDate', todayDate);
        $scope.outtimeChange();
        $scope.$apply();
        document.getElementById("txtTime").value = "10:00 hrs";
        document.getElementById("formTransfer").submit();
    }

    $scope.redirectToListing = function (txtDate, ddlSource, ddlDestination, txtTime) {
        debugger;
        document.getElementById('ddlSource').value = ddlSource;
        document.getElementById('ddlDestination').value = ddlDestination;
        // document.getElementById('txtDate').value = txtDate;
        $('#txtDate').datepicker('setDate', txtDate);
        $scope.outtimeChange();
        $scope.$apply();
        document.getElementById('txtTime').value = txtTime;

        $("#formTransfer").submit();

        // window.location.href = "/Transfer/GetTransferSearch?TripType=1&txtfromcity=" + ddlSource + "&txtdesticity=" + ddlDestination + "&txtDate=" + txtDate;

    }
}]);





function SetTextBoxValue(li, control, CId) {
    debugger;
    document.getElementById('hdnLCountry').value = CId;
    alert("working bro")
    $("#hdnLCountry").val(CId);

}