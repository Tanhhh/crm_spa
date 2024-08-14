window.fbAsyncInit = function () {
    FB.init({
        appId: '247660513180458',
        cookie: true,                     // Enable cookies to allow the server to access the session.
        xfbml: true,                     // Parse social plugins on this webpage.
        version: 'v7.0'           // Use this Graph API version for this call.
    });
    FB.AppEvents.logPageView();
    FB.getLoginStatus(function (response) {
        statusChangeCallback(response);
    });
};
(function (d, s, id) {
    var js, fjs = d.getElementsByTagName(s)[0];
    if (d.getElementById(id)) { return; }
    js = d.createElement(s); js.id = id;
    js.src = "https://connect.facebook.net/en_US/sdk.js";
    fjs.parentNode.insertBefore(js, fjs);
}(document, 'script', 'facebook-jssdk'));
function statusChangeCallback(response) {
    debugger// Called with the results from FB.getLoginStatus().
    console.log('statusChangeCallback');
    console.log(response);                   // The current login status of the person.
    if (response.status === 'connected') {   // Logged into your webpage and Facebook.
        Gettoken(response.authResponse.accessToken);
        console.log("Login thanh cong");
    }
    else {                                 // Not logged into your webpage or we are unable to tell.
        console.log("Login khong thanh cong");
    }
}



function testAPI() {                      // Testing Graph API after login.  See statusChangeCallback() for when this call is made.
    console.log('Welcome!  Fetching your information.... ');
    FB.api('/me', function (response) {
        console.log('Successful login for: ' + response.name);
        document.getElementById('status').innerHTML =
            'Thanks for logging in, ' + response.name + '!';
    });
}

//Đăng nhập Facebook
function LoginFB() {
    debugger
    FB.login(function (response) {
        if (response.authResponse) {
            var access_token = FB.getAuthResponse()['accessToken'];
            console.log('Access Token = ' + access_token);
            testAPI();
            //begin buoc 1: lay tat ca cac fanpage
            var url = "https://graph.facebook.com/me/accounts?access_token="+access_token;
            $.ajax({
                url: url,
                method: 'GET',
                //data: { token: access_token },
                dataType: 'JSON',
                success: function (response) {
                    //$('#myModal').modal('show');
                    console.log(response);
                   
                    //alert("lấy thông tin page thành công " + response.data.length);                   
                    for (var i = 0; i < response.data.length ; i++) {
                        console.log(response.data[i].name);
                        console.log(response.data[i].access_token);

                        localStorage.setItem("accessPage", response.data[i].access_token);
                        localStorage.setItem("idPage", response.data[i].id);
                    } 
                    var array = [];
                    $.each(response.data, function (i, item) {

                        //array.push({
                        //    ID: $(item).data('id'),
                        //    Access: $(item).data('access_token'),
                        //    Name: $(item).data('name')
                        //});
                        array.push(item);
                    });

                 
                    $.ajax({
                        method: "POST",
                        url: "/Home/GetPageSession",
                        data: { model: JSON.stringify(array)},
                        dataType: "json",
                        success: function (rs) {
                            if (rs.status == true) {
                                localStorage.getItem("accessPage");
                               var id= localStorage.getItem("idPage");
                                window.location.href = '/Home/ManageFB?id='+id+'';
                            }
                        }
                    })
                    //window.location.href = '/Home/ManageFB';
                },
                error: function (jqxhr) {
                    alert("lỗi rồi bạn ơiiii");
                }
            });
            //end buoc 1: lay tat ca cac fanpage
        } else {
            console.log('User cancelled login or did not fully authorize.');
        }
    }
    );
}

