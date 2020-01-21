
var logindetails = "";

app.controller('LoginController', function ($scope, $http) {
    $scope.UserName = "";
    $scope.Password = "";
    $scope.UserAuthentication = function () {
      
        $scope.UserName = document.getElementById('txtusername').value;
        $scope.Password = document.getElementById('userPassword').value;

        if ($scope.UserName == "") {
            
            $("#alrtLoginEmail").fadeIn(1000);
            $("#alrtLoginEmail").fadeOut(4000);
        }
        else if ($scope.Password == "")
        {
            $("#alrtLoginPass").fadeIn(1000);
            $("#alrtLoginPass").fadeOut(4000);


        }

        else {

            var request = $http({
                method: "post",
                url: "/Login/SignIn",
                headers: {
                    'Content-Type': "application/json"
                },
                data: "{UserName_:'" + $scope.UserName + "',Password_:'" + $scope.Password + "'}",
            })
             .success(function (httpData) {
                 $scope.PrintUserDtl(httpData, 'Y');
             })
             .error(function (http, status, fnc, httpObj) {
             });
            return request;

        }

    }
    $scope.PrintUserDtl = function (httpData, isErr_) {
        logindetails = httpData;
      //  var hotelString = window.location.search.substring(1)
        //if (httpData != null && httpData.split('|').length > 1 && httpData.split('|')[1] != null && hotelString != null && hotelString.split('&').length > 0)
        //    Identify(hotelString.split('&')[0].split('=')[1], httpData.split('|')[1], '', hotelString.split('&')[0].split('=')[1]);
        //else
        //    Identify('', '', '', '');
        if (httpData != "" && httpData.split('|')[0].indexOf("not valid user") <= 0) {

            //document.getElementById('welcome-det-User').innerHTML = httpData.split('|')[1];
            if (document.getElementById('welcome-det-User')!= null) {
                document.getElementById('welcome-det-User').innerHTML = httpData.split('|')[1];
                if (document.getElementById("txtEmailId") != null) {
                    document.getElementById('txtEmailId').value = httpData.split('|')[1];
                    //document.getElementById('txtEmailId').readOnly = true;
                }
            }
            if (document.getElementById('divSignInPnl') != null) {
                document.getElementById('divSignInPnl').style.display = 'none';
            } 
            if (document.getElementById('spnLogoutPnl') != null) {
                document.getElementById('spnLogoutPnl').style.display = 'block';
            }
               
            if (document.getElementById('spnMyAcc') != null) {
                document.getElementById('spnMyAcc').style.display = 'none';
                }
            //if (document.getElementById('hid') != null) {
            //    document.getElementById('hid').style.display = 'none';
            //}
            if (document.getElementById('divLogin') != null) {
                document.getElementById('divLogin').style.display = 'none';
            }
            if (document.getElementById('spnLgnWelcome') != null) {
                document.getElementById('spnLgnWelcome').style.display = 'inline-block';
            }

            
        }
        else {
            if (document.getElementById('welcome-det-User') != null) {
                document.getElementById('welcome-det-User').innerHTML = "";
            }
            if (document.getElementById('divSignInPnl') != null) {
                document.getElementById('divSignInPnl').style.display = 'block';
            }
            if (document.getElementById('spnLogoutPnl') != null) {
                document.getElementById('spnLogoutPnl').style.display = 'none';
            }
            if (document.getElementById('hid') != null) {
                document.getElementById('hid').style.display = 'none';
            }
            if (isErr_ == "Y" && document.getElementById('divWrngPass') != null) {
                document.getElementById('divWrngPass').style.display = 'block';                
            }
            if (isErr_ == "Y")
            $("#alrtLogin").fadeIn(100);
            $("#alrtLogin").fadeOut(8000);
        }
    }

    $scope.CheckAuthentication = function () {
       
        var request = $http({
            method: "post",
            url: "/Login/CheckSignIn",
            headers: {
                'Content-Type': "application/json"
            }
        })
         .success(function (httpData) {
           //  alert(httpData)
             $scope.PrintUserDtl(httpData, 'N');
         })
         .error(function (http, status, fnc, httpObj) {
         });
        return request;
    }



    $scope.LogOut = function () {
        if (confirm("Do you want to logout !")) {
           
            var request = $http({
                method: "post",
                url: "/Login/LogOut",
                headers: {
                    'Content-Type': "application/json"
                }
            })
             .success(function (httpData) {
                 alert("You have Sucessfully Logged out !");
                 if (document.getElementById('welcome-det-User') != null) {
                     document.getElementById('welcome-det-User').innerHTML = "";
                 }
                 if (document.getElementById('divSignInPnl') != null) {
                     document.getElementById('divSignInPnl').style.display = 'inline-block';
                 }
                 if (document.getElementById('spnLogoutPnl') != null) {
                     document.getElementById('spnLogoutPnl').style.display = 'none';
                 }
                 //if (document.getElementById('hid') != null) {
                 //    document.getElementById('hid').style.display = 'none';
                 //}

                 if (document.getElementById('spnMyAcc') != null) {
                     document.getElementById('spnMyAcc').style.display = 'inline-block';
                 }

                 if (document.getElementById('spnLgnWelcome') != null) {
                     document.getElementById('spnLgnWelcome').style.display = 'none';
                 }
                 

                 // window.location.href = "/Index/Index";
             })
             .error(function (http, status, fnc, httpObj) {
             });
            return request;
        }
    }

    $scope.PhoneEmail = "";

    $scope.CreateAccount = function () {
        var validityUser = true;
        var input = $("#txtEmail").val();
        if (input != null && input != '') {
            if ((isValidEmail(input) ? true : (isValidPhone(input)))) {

                document.getElementById("regNumber").innerText = input;
                
                if (isValidEmail(input)) {

                    $scope.number = input;
                    localStorage.setItem('added-items', input);
                    //$("#RegWait").show();
                    $("#dvOtp").slideDown();
                    $("#dvRegister").slideUp();
                    $scope.PhoneEmail = "Email";
                }
               
                else {
                    $scope.number = input;
                    localStorage.setItem('added-items', input);
                    $("#dvOtp").slideDown();
                    $("#dvRegister").slideUp();
                    $scope.PhoneEmail = "mobile number";
                }
                               

                    $.ajax({
                        type: "Post",
                        contentType: "application/json; charset=utf-8",
                        dataType: "text",
                        url: "/login/RegisterNewUser/",
                        data: "{'emailph':'" + input + "'}",
                        success: function (response) {
                            // debugger;
                            if (response != null) {
                                $("#RegWait").hide();
                                $("#txtEmail").val('');
                                // $("#mailsent").fadeIn(500);
                                //setTimeout(function () {
                                //    $("#hid1").hide(500);
                                //    $("#mailsent").hide();
                                //    $("#txtEmail").val('');
                                //}, 3000);

                            }
                            //else if (response != null && response != 'OK') {
                            //    var resp = [];
                            //    resp = response.split('|');
                            //    if (resp[0].indexOf("Exist") > -1 && isValidPhone(input)== true && resp[4] != "False")
                            //    {
                            //        $("#RegWait").hide();
                            //        $("#AlertRegister").fadeIn(500);
                            //        $("#AlertRegister").fadeOut(8000);
                            //    }
                            //    else if (isValidPhone(input) == true && resp[4] == "False")
                            //    {
                            //        $("#RegWait").hide();
                            //        $(".se-m1").slideDown();
                            //        $(".se-m").slideUp();
                            //        $("#txtEmail").val('');
                            //    }
                            //    else if(resp[0].indexOf("Exist")>-1 && isValidEmail(input)== true && resp[5] != "False")
                            //    {
                            //        $("#RegWait").hide();
                            //        $("#AlertRegister").fadeIn(500);
                            //        $("#AlertRegister").fadeOut(8000);
                            //    }
                            //    else if (isValidEmail(input) == true && resp[5] == "False")
                            //    {
                            //        $("#RegWait").hide();
                            //        $(".se-m1").slideDown();
                            //        $(".se-m").slideUp();
                            //        $("#txtEmail").val('');
                            //    }
                            //    else
                            //    {
                            //        $("#RegWait").hide();
                            //        $("#AlertRegister").fadeIn(500);
                            //        $("#AlertRegister").fadeOut(8000);
                            //    }

                            // }
                        },
                        beforeSend: function (XMLHttpRequest) {
                        },
                        error: function (xmlHttpRequest, status, err) {
                        }
                    });
                
            }
            else {
                $("#RegValid").fadeIn(1000);
                $("#RegValid").fadeOut(5000);
                document.getElementById('txtEmail').focus();
            }

        }
        else {
            $("#RegWait").hide();
            $("#EmailAlert").fadeIn(1000);
            $("#EmailAlert").fadeOut(10000);
            $("#AlertRegister").hide();
        }

    }
    



    $scope.enterotp = "";
    $scope.pass = "";
    $scope.repass = "";
    $scope.OTP = function () {

        var otp = $scope.enterotp;
        var Pass = $scope.pass;
        var Confirmpass = $scope.repass;
        var Number = localStorage.getItem('added-items');
        if ($scope.enterotp != "") {
            if ($scope.pass != "" && $scope.repass != "") {
                if ($scope.enterotp != null && $scope.pass != null && $scope.repass != null) {
                    if ($scope.enterotp.length == "6") {
                        if (Pass == Confirmpass) {
                            $.ajax({
                                type: "POST",
                                contentType: "application/json;charset=utf-8",
                                dataType: "text",
                                url: "/MyBooking/OTPConfirm/",
                                data: "{'Input':'" + Number + "','OTP':'" + otp + "','Password':'" + Pass + "','Confirmpassword':'" + Confirmpass + "'}",
                                success: function (response) {

                                    var resp = [];
                                    resp = response.split('|');
                                    if (response != null && response.indexOf("|") > -1) {
                                        $('#otpmsg').fadeIn(500);
                                        $('#otpmsg').fadeOut(4000);
                                        $('#otptxt').val('');
                                        $('#pass').val('');
                                        $('#confirmpass').val('');
                                        if (resp[4] == "True" || resp[5] == "True") {
                                            setTimeout(function () {
                                                $scope.AutoLogin(Number, Pass);
                                                $('.signup').hide();
                                            }, 4000);

                                        }

                                        localStorage.clear();

                                    }
                                    else if (response.indexOf("expired") > -1) {
                                        $('#otperror').fadeIn(500);
                                        $('#otperror').fadeOut(10000);
                                    }
                                    else if (response.indexOf("valid") > -1) {
                                        $('#otperror1').fadeIn(500);
                                        $('#otperror1').fadeOut(10000);
                                    }

                                },
                                beforeSend: function (XMLHttpRequest) {
                                },
                                error: function (xmlHttpRequest, status, err) {
                                }

                            });

                        }
                        else {
                            $("#errorpass").fadeIn(1000);
                            $("#errorpass").fadeOut(10000);
                        }

                    }
                    else {
                        $("#errorotp").fadeIn(1000);
                        $("#errorotp").fadeOut(10000);
                    }
                }
            }
            else {
                $("#passerror").fadeIn(1000);
                $("#passerror").fadeOut(10000);
            }
        }
        else {
            $("#errorotp").fadeIn(1000);
            $("#errorotp").fadeOut(10000);
        }

    }




    $scope.ResendOTP = function () {
        // debugger;
        var mobile = localStorage.getItem('added-items');
        $.ajax({
            type: "POST",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            url: "/MyBooking/ResendOTP/",
            data: "{'Mobile':'" + mobile + "'}",
            success: function (response) {
                //complete response
            },
            beforeSend: function (XMLHttpRequest) {
            },
            error: function (xmlHttpRequest, status, err) {
            }

        });

    }









    function isValidEmail(email) {
        // debugger;
        var expr = new RegExp(/^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/);
        return $.trim(email).match(expr) ? true : false;
    }
    function isValidPhone(phone) {
        var phoneno = /^\d{10}$/;
        return $.trim(phone).match(phoneno) ? true : false;
    }











});



function GoToMybookingSec() {
    window.location.href = "https://mybookings.easemytrip.com"

}












//var app = angular.module('LoginApp', []);

//app.controller('LoginControl', function ($scope, $http, $window, $location) {
    
//    $scope.username = null;
//    $scope.passwrd = null;


//    $scope.login= function()
//    {
//       var user= document.getElementById("txtusername").value;
//       var pass = document.getElementById("Password1").value;


//        var dataObj = {
//            UserName_: user,
//            Password_: pass
            
//            //model.Dlng = droplng;
//        };
//        var res = $http.post('../Login/SignIn', dataObj);
//        res.success(function (data) {
//            $scope.message = data;
//            debugger;
//           // alert(data);
//            response(data);

//        });
//        res.error(function (data, status, headers, config) {
//            alert("failure message: " + JSON.stringify({ data: data }));
//        });
        
//    }


//    function response(data)
//    {
//        if (data=="1")
//            location.href = '../Booking/Book';
//        else if (data == "2")
//            location.href = '../Booking/Book';
//                  //  alert("invalid uer id or passsword")


//    }


//});

$(document).ready(function () {

    $("#signInHome").click(function () {
        document.getElementById("divLogin").style.display = "block";
    });

    $("#RegInHome").click(function () {
        document.getElementById("divSignup").style.display = "block";
    });

    $("#closeRegtab").click(function () {
        $("#dvOtp").slideUp();
        $("#dvRegister").slideDown();
      //  document.getElementById("dvOtp").style.display = "none";


    });

});