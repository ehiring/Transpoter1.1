var allPasswordInp = [];

$(document).ready(function () {

    $("input[type=password]").each(function (idx, ele) {
        allPasswordInp.push(ele)
    })

    routie({
        '': function () {
            showProgress()
            $("#passwordDiv").addClass("scale-out")
            $(".formTitle").html("Sign In")
            /* Show Login Form */
            $("#formContainer").removeClass("goLeft").addClass("goRight")

        },

        'hasPassword': function () {
            showProgress()
            $("#passwordDiv").addClass("hide")
            $("#enterOTPDiv").addClass("scale-out")
            $("#ForgetPswdDiv").addClass("scale-out")
            $(".passwordOrOTP").html("Get New Password").attr("data-PassOTP", "psw").attr("href", "#enterPWD")
            //  $(".passwordOrOTP").html("Get OTP").attr("data-PassOTP","OTP").attr("href", "#enterOTP")
            $(".loginNextBtn").addClass("hide")
            $(".loginBtn").removeClass("hide")
        },

        'enterOTP': function () {
            showProgress()

            setTimeout(function () {
                M.toast({ html: 'OTP Sent!', classes: 'rounded' })
            }, 600)

            $("#passwordDiv").addClass("scale-out")
            $("#enterOTPDiv").addClass("pulse").removeClass("scale-out")
            $(".passwordOrOTP").html("Enter Password").attr("data-PassOTP", "Password").attr("href", "#hasPassword")
            $(".loginNextBtn").addClass("hide")
            $(".loginBtn").removeClass("hide")

        },

        'enterPWD': function () {
            showProgress()

            setTimeout(function () {
                M.toast({ html: 'PW Sent!', classes: 'rounded' })
            }, 600)

            $("#ForgetPswdDiv").addClass("scale-out").removeClass("scale-out")
            $("#enterOTPDiv").addClass("pulse")
            $(".passwordOrOTP").html("Enter OTP").attr("data-PassOTP", "Password").attr("href", "#hasPassword").addClass("hide")
            $(".loginNextBtn").addClass("hide")
            $(".loginBtn").removeClass("hide")

        },
        'createAccountNow': function () {
            $(".formTitle").html("Sign Up")
            showProgress()
            /* Show Signup Form */
            $("#formContainer").removeClass("goRight").addClass("goLeft")
        },

        'loginNow': function login() {
            location.replace("bhadaa.html")

        }
    });

});

// function createAccountNow() {
//   $(".formTitle").html("Sign Up")
//   showProgress()
//   /* Show Signup Form */
//   $("#formContainer").removeClass("goRight").addClass("goLeft")
// }

function showProgress() {
    $("#progress-bar").removeClass("hidden")
    setTimeout(function () {
        $("#progress-bar").addClass("hidden")
    }, 500)
}

function showPassword() {
    var iconText = $(".showPassword:eq(0) i").text();
    var input_type = (iconText == "visibility") ? "text" : "password";
    if (input_type == "text") {
        $(".showPassword i").text("visibility_off");
    } else {
        $(".showPassword i").text("visibility");
    }

    $.each(allPasswordInp, function (idx, ele) {

        $(ele).attr("type", input_type);
    })
}

