//SDK Facebook
window.fbAsyncInit = function () {
    FB.init({
        appId: '247660513180458',
        cookie: true,                     // Enable cookies to allow the server to access the session.
        xfbml: true,                     // Parse social plugins on this webpage.
        version: 'v7.0'           // Use this Graph API version for this call.
    });
    FB.AppEvents.logPageView();
    function checkLoginState() {
        debugger// Called when a person is finished with the Login Button.
        FB.getLoginStatus(function (response) {   // See the onlogin handler
            statusChangeCallback(response);
        });
    }
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
//----------------------------------------------------------------Kết thúc SDK

//----------------------------------------------------------------BIẾN TOÀN CỤC

//lấy token page
var token = localStorage.getItem("accessPage");
var idpage = localStorage.getItem("idPage");
var idpost = localStorage.getItem("idPost");
var iduser;
var maKH = localStorage.getItem("MaKH");
console.log(token);
console.log(idpage);

//----------------------------------------------------------------Hàm chuyển Tabs
function openTab(evt, tabName) {
    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("messages-box");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    document.getElementById(tabName).style.display = "block";
    evt.currentTarget.className += " active";
}
//----------------------------------------------------------------LẤY DANH SÁCH CÁC ĐOẠN HỘI THOẠI
function getUser() {
    localStorage.removeItem("MaKH");
    $("div").remove('.list-group.mess');
    $('div.px-4.py-5.chat-box.bg-white').remove();
    $('div.bg-light').remove();
    $('div._673w._6ynl').remove();
    document.getElementById("TTKH").style.display = "none";
    document.getElementById("nomsg-viewpost").style.display = "none";
    document.getElementById("nomsg-view").style.display = "block";
    //Truy vấn đến Facebook lấy dữ liệu về hội thoại
    url = 'https://graph.facebook.com/' + idpage + '?fields=conversations{unread_count,senders,snippet,messages.limit(300){attachments{image_data,video_data},message,sticker,from,created_time}}&access_token=' + token;
    $.ajax({
        url: url,
        method: 'GET',
        dataType: 'JSON',
        success: function (response) {
            for (var i = 0; i < response.conversations.data.length; i++) {
                var mes = response.conversations.data[i].messages;
                spl = mes.data[0].created_time.split('T');
                date = spl[0].split('-');
                time = spl[1].split(':');
                splt = mes.data[0].from.name.split(' ');
                name = splt[1];
                //HIỂN THỊ TIN NHẮN
                $('.messages-box.mess').append('<div class="list-group mess rounded-0" style="cursor:pointer" id=' + response.conversations.data[i].senders.data[0].id + ' onclick="showDetail(this.id);" ><a class="list-group-item list-group-item-action rounded-0" style="display: flex;"><div class="avt ' + i + '" style="height: 40px; width: 40px;"></div><div class="media" style="width: 100%;"><div class="media-body ml-4" style="line-height: 1.5; margin-left: 1rem !important;"><div class="d-flex align-items-center justify-content-between mb-1"><h6 class="mb-0 username">' + response.conversations.data[i].senders.data[0].name + '</h6><small class="small datetime">' + time[0] + ':' + time[1] + ' | ' + date[2] + '-' + date[1] + '</small></div><p class="font-italic mb-0 text-small lastMess ' + i + '" id="' + response.conversations.data[i].senders.data[0].id + '"style="font-weight:100;">' + response.conversations.data[i].snippet + '</p></div></div></a></div>');
                getAvt(response.conversations.data[i].senders.data[0].id, i);
                if (response.conversations.data[i].unread_count > 0) {
                    $('p.font-italic.mb-0.text-small.' + i).css('font-weight', '700');
                }
            }
        },
        error: function (jqXHR) {
            alert("Lấy người dùng bị lỗi");
            console.log(response);
        }
    });
}
//----------------------------------------------------------------Kết thúc lấy dữ liệu

$(window).on('load', function () {
    getUser();
})

//----------------------------------------------------------------Hiển thị chi tiết hội thoại
function showDetail(clicked_id) {
    document.getElementById("nomsg-view").style.display = "none";
    $('div.px-4.py-5.chat-box.bg-white').remove();
    $('div.bg-light').remove();
    $('div._673w._6ynl').remove();
    document.getElementById("TTKH").style.display = "block";
    document.getElementById("TTKH").style.animation = "slideIn 0.35s";
    iduser = clicked_id;
    localStorage.setItem("idUser", iduser);
    //Kiểm tra khách hang đã thêm vào DB
    checkFBID();
    //Thêm title
    $('.col-8.px-0').append('<div class="_673w _6ynl"><div class="_5743 _6y4y"><div aria-hidden="true" style="padding-bottom: 0.5px;"><div class="_6ynm _87u_"><div class="avttitle ' + iduser + '" style="height: 40px; width: 40px;"></div></div></div><div class="_6ynn"><h5 class="_17w2 _6ybr" id="js_5" style="margin-left: 10%; margin-top: 5%;"><span class="_3oh-"></span></h5><div class="user-type"><select id="role" style="border: none; width: 130%;"><option selected="selected">Chưa phân công</option></select></div></div></div><i class="fa fa-shopping-cart" id="hideKH" onclick="clickme(); " aria-hidden="true"></i></div>');
    getAvtTitle(iduser);
    //Thêm conversation
    $('.col-8.px-0').append('<div class="px-4 py-5 chat-box bg-white"></div>');
    //Thêm khung reply
    $('.col-8.px-0').append('<div class="bg-light reply"><div class="input-group"><input type="text" id ="message" onkeypress="return enterMess(event);" placeholder="Type a message" autocomplete="off" autocorrect="off" autocapitalize="off" spellcheck="false" aria-describedby="button-addon2" class="form-control rounded-0 border-0 py-4 bg-light"><div class="input-group-append" ><button id="button-addon2" type="submit" onclick="sendMess();" class="btn btn-link"> <i class="fa fa-paper-plane" id ="' + iduser + '"></i></button></div></div></div>');
    url = 'https://graph.facebook.com/' + idpage + '?fields=roles,conversations{senders,messages.limit(300){attachments{image_data,video_data},message,sticker,from,created_time}}&access_token=' + token;
    $.ajax({
        url: url,
        method: 'GET',
        dataType: 'JSON',
        success: function (response) {
            var spl, date, time;
            for (var k = 0; k < response.roles.data.length; k++) {
                $('#role').append('<option>' + response.roles.data[k].name + '</option>')
            }
            for (var i = 0; i < response.conversations.data.length; i++) {
                //So sánh response.conversations.data[i].senders.data[0].name=x?
                if (response.conversations.data[i].senders.data[0].id == iduser) {
                    $('._3oh-').append(response.conversations.data[i].senders.data[0].name);
                    $("#Ten-KH").val(response.conversations.data[i].senders.data[0].name);
                    //$("#Ma-macdinh").val(response.conversations.data[i].senders.data[0].id)
                    var mes = response.conversations.data[i].messages;
                    for (var j = response.conversations.data[i].messages.data.length - 1; j >= 0; j--) {
                        spl = mes.data[j].created_time.split('T');
                        date = spl[0].split('-');
                        time = spl[1].split(':');
                        //Hiển thị: response.conversations.data[i].messages.data[i].message
                        if (mes.data[j].from.id == iduser) {
                            if (mes.data[j].attachments) {
                                if (mes.data[j].attachments.data[0].image_data) {
                                    $('.px-4.py-5.chat-box.bg-white').append('<!-- Sender Message--><div class="media mb-3"><div class="media-body ml-3"><div class="rounded py-2 px-3 mb-2" style="max-width: 40%;width: fit-content;"><p class="text-small mb-0 text-muted sendermess"><image style=float:none;width:200px !important; height:150px !important; src=' + mes.data[j].attachments.data[0].image_data.url + '></p></div><p class="small text-muted">' + time[0] + ':' + time[1] + ' | ' + date[2] + '-' + date[1] + '-' + date[0] + '</p></div></div>');
                                }
                                if (mes.data[j].attachments.data[0].video_data) {
                                    $('.px-4.py-5.chat-box.bg-white').append('<!-- Sender Message--><div class="media mb-3"><div class="media-body ml-3"><div class="rounded py-2 px-3 mb-2" style="max-width: 40%;width: fit-content;"><p class="text-small mb-0 text-muted sendermess"><image style=float:none;width:200px !important; height:150px !important; src=' + mes.data[j].attachments.data[0].video_data.url + '></p></div><p class="small text-muted">' + time[0] + ':' + time[1] + ' | ' + date[2] + '-' + date[1] + '-' + date[0] + '</p></div></div>');
                                }
                            }
                            if (mes.data[j].sticker) {
                                $('.px-4.py-5.chat-box.bg-white').append('<!-- Sender Message--><div class="media mb-3"><div class="media-body ml-3"><div class="rounded py-2 px-3 mb-2" style="max-width: 40%;width: fit-content;"><p class="text-small mb-0 text-muted sendermess"><image style=float:none;width:150px !important; height:150px !important; src=' + mes.data[j].sticker + '></p></div><p class="small text-muted">' + time[0] + ':' + time[1] + ' | ' + date[2] + '-' + date[1] + '-' + date[0] + '</p></div></div>');
                            }
                            if (mes.data[j].message) {
                                $('.px-4.py-5.chat-box.bg-white').append('<!-- Sender Message--><div class="media mb-3"><div class="media-body ml-3"><div class="bg-light rounded py-2 px-3 mb-2" style="max-width: 40%;width: fit-content;"><p class="text-small mb-0 text-muted sendermess" style="word-wrap:break-word;">' + mes.data[j].message + '</p></div><p class="small text-muted">' + time[0] + ':' + time[1] + ' | ' + date[2] + '-' + date[1] + '-' + date[0] + '</p></div></div>');
                            }
                        }
                        else {
                            if (mes.data[j].attachments) {
                                if (mes.data[j].attachments.data[0].image_data) {
                                    $('.px-4.py-5.chat-box.bg-white').append('<!-- Reciever Message--><div class="media ml-auto mb-3"><div class="media-body"><div class="rounded py-2 px-3 mb-2" style="max-width: 40%;width: fit-content;height: fit-content;margin-left: auto;"><p class="text-small mb-0 text-white sendermess"><image style=float:none;width:200px !important; height:150px !important; src=' + mes.data[j].attachments.data[0].image_data.url + '></p></div><p class="small text-muted" style="float:right;">' + time[0] + ':' + time[1] + ' | ' + date[2] + '-' + date[1] + '-' + date[0] + '</p></div></div>');
                                }
                                if (mes.data[j].attachments.data[0].video_data) {
                                    $('.px-4.py-5.chat-box.bg-white').append('<!-- Reciever Message--><div class="media ml-auto mb-3"><div class="media-body"><div class="rounded py-2 px-3 mb-2" style="max-width: 40%;width: fit-content;height: fit-content;margin-left: auto;"><p class="text-small mb-0 text-white sendermess"><video style=float:none;width:250px !important; height:200px !important; src=' + mes.data[j].attachments.data[0].video_data.url + '></p></div><p class="small text-muted" style="float:right;">' + time[0] + ':' + time[1] + ' | ' + date[2] + '-' + date[1] + '-' + date[0] + '</p></div></div>');
                                }
                            }
                            if (mes.data[j].message) {
                                $('.px-4.py-5.chat-box.bg-white').append('<!-- Reciever Message--><div class="media ml-auto mb-3"><div class="media-body"><div class="bg-primary rounded py-2 px-3 mb-2" style="max-width: 40%;width: fit-content;height: fit-content;margin-left: auto;"><p class="text-small mb-0 text-white sendermess" style="word-wrap:break-word;white-space: pre-wrap;">' + mes.data[j].message + '</p></div><p class="small text-muted" style="float:right;">' + time[0] + ':' + time[1] + ' | ' + date[2] + '-' + date[1] + '-' + date[0] + '</p></div></div>');
                            }
                            if (mes.data[j].sticker) {
                                $('.px-4.py-5.chat-box.bg-white').append('<!-- Reciever Message--><div class="media ml-auto mb-3"><div class="media-body"><div class="rounded py-2 px-3 mb-2" style="max-width: 40%;width: fit-content;height: fit-content;margin-left: auto;"><p class="text-small mb-0 text-white sendermess"><image style=float:none;width:150px !important; height:150px !important; src=' + mes.data[j].sticker + '></p></div><p class="small text-muted" style="float:right;">' + time[0] + ':' + time[1] + ' | ' + date[2] + '-' + date[1] + '-' + date[0] + '</p></div></div>');
                            }
                        }
                    }
                }
            }
            $('.px-4.py-5.chat-box.bg-white').scrollTop($('.px-4.py-5.chat-box.bg-white')[0].scrollHeight);
        },
        error: function (jqXHR) {
            alert("LỖI RỒI BẠN ƠIIII");
        }

    });
}

//GỬI TIN NHẮN
//$(document).ready(function () {
//    $("#message").keypress(function (event) {
//        if (event.keyCode == 13) {
//            sendMess();
//        }
//    });
//});

//----------------------------------------------------------------Gửi tin nhắn bằng enter
function enterMess(event) {
    if (event.keyCode == 13) {
        var x = document.getElementById("message").value;
        id = localStorage.getItem("idUser");
        debugger
        //var message = CKEDITOR.instances.msg.getData();
        FB.api(
            "/" + idpage + "/messages", "POST",
            {
                "recipient": {
                    "id": id
                },
                "message": {
                    "text": x
                },
                "messaging_type": "update",
                "access_token": token
            },
            function (response) {
                if (response && !response.error) {
                    document.getElementById('message').value = '';
                    addChat();
                }
                else {
                    console.log(response);
                }
            }
        );
    }
}
//$("#button-addon2").click(function () {
//    sendMess();
//});

//----------------------------------------------------------------Gửi tin nhắn
function sendMess() {
    var x = document.getElementById("message").value;
    id = localStorage.getItem("idUser");
    debugger
    //var message = CKEDITOR.instances.msg.getData();
    FB.api(
        "/" + idpage + "/messages", "POST",
        {
            "recipient": {
                "id": id
            },
            "message": {
                "text": x
            },
            "messaging_type": "update",
            "access_token": token
        },
        function (response) {
            if (response && !response.error) {
                document.getElementById('message').value = '';
                addChat();
            }
            else {
                console.log(response);
            }
        }
    );
}

//----------------------------------------------------------------Thêm ngày chat với khách hàng
function addChat() {
    var FacebookId = localStorage.getItem("idUser");
    //Luu ngay chat vao DB
    $.ajax({
        type: "POST",
        url: "/Home/Create_Chat",
        data: JSON.stringify({ FacebookId: FacebookId }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (r) {
            console.log(r);
            if (r.Id > 0) {
                console.log(r);
            }
            else {
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            console.log(xhr);
            console.log(ajaxOptions);
            console.log(thrownError);

            //HideLoading();
            //$("#danger-alert").fadeTo(2000, 500).slideUp(500, function () {
            //    $("#danger-alert").slideUp(500);
            //});
            //alert("Thêm không được");
        }
    });
}

//---------------------------------------------------------------- CẬP NHẬT TIN NHẮN
function reload() {
    //var idUser = localStorage.getItem("idUser");
    var listgroup = document.getElementsByClassName("list-group");
    var username = document.getElementsByClassName("username");
    var lastmess = document.getElementsByClassName("lastMess");
    var datetime = document.getElementsByClassName("datetime");
    var sender = document.getElementsByClassName("sendermess");
    url = 'https://graph.facebook.com/' + idpage + '?fields=conversations{unread_count,senders,snippet,updated_time,messages.limit(300){attachments{image_data,video_data},message,sticker,from,created_time}}&access_token=' + token;
    if (username.length != 0) {
        $.ajax({
            url: url,
            method: 'GET',
            dataType: 'JSON',
            success: function (response) {
                //if ((response.conversations.data.length != username.length) && (username.length > 0)) {
                //    var count = parseFloat(response.conversations.data.length) - parseFloat(username.length);
                //    if (count > 0) {
                //        for (var i = count - 1; i >= 0; i--) {
                //            $('.messages-box.mess').append('<div class="list-group mess rounded-0"  id=' + response.conversations.data[i].senders.data[0].id + ' onclick="showDetail(this.id);" ><a class="list-group-item list-group-item-action rounded-0" style="display: flex;"><div class="avt ' + i + '" style="height: 40px; width: 40px;"><img class="_87v3 img userimg" alt="" src="" height="40" width="40"></div><div class="media" style="width: 100%;"><div class="media-body ml-4" style="line-height: 1.5; margin-left: 1rem !important;"><div class="d-flex align-items-center justify-content-between mb-1"><h6 class="mb-0 username">' + response.conversations.data[i].senders.data[0].name + '</h6><small class="small datetime">' + splDateTime(response.conversations.data[i].updated_time) + '</small></div><p class="font-italic mb-0 text-small lastMess ' + i + '" id="' + response.conversations.data[i].senders.data[0].id + '"style="font-weight:100;">' + response.conversations.data[i].snippet + '</p></div></div></a></div>');
                //        }
                //    }
                //}
                if ((response.conversations.data.length != username.length) && (username.length > 0)) {
                    getUser();
                }

                for (var i = 0; i < response.conversations.data.length; i++) {
                    var mes = response.conversations.data[i].messages;
                    //SO SÁNH class USERNAME
                    if (username.length != 0) {
                        if (response.conversations.data[i].senders.data[0].name != username[i].innerHTML) {
                            username[i].innerHTML = response.conversations.data[i].senders.data[0].name;
                            listgroup[i].id = response.conversations.data[i].senders.data[0].id;
                            reloadAvt(response.conversations.data[i].senders.data[0].id, i);
                            lastmess[i].innerHTML = response.conversations.data[i].snippet;
                            datetime[i].innerHTML = splDateTime(response.conversations.data[i].updated_time);
                        }
                        if (response.conversations.data[i].snippet != lastmess[i].innerHTML) {
                            lastmess[i].innerHTML = response.conversations.data[i].snippet;
                            datetime[i].innerHTML = splDateTime(response.conversations.data[i].updated_time);
                        }
                    }
                    if (response.conversations.data[i].senders.data[0].id == iduser) {
                        if ((response.conversations.data[i].unread_count > 0) && (mes.data[0].from.id == iduser)) {
                            $('p.font-italic.mb-0.text-small.' + i).css('font-weight', '700');
                        }
                        else {
                            $('p.font-italic.mb-0.text-small.' + i).css('font-weight', '100');
                        }
                        if ((mes.data.length != sender.length) && (sender.length > 0)) {
                            var count = parseFloat(mes.data.length) - parseFloat(sender.length);
                            if (count > 0) {
                                for (var k = count - 1; k >= 0; k--) {
                                    if (mes.data[k].from.id == iduser) {
                                        if (mes.data[k].attachments) {
                                            if (mes.data[k].attachments.data[0].image_data) {
                                                $('.px-4.py-5.chat-box.bg-white').append('<!-- Sender Message--><div class="media mb-3"><div class="media-body ml-3"><div class="rounded py-2 px-3 mb-2" style="max-width: 40%;width: fit-content;"><p class="text-small mb-0 text-muted sendermess"><image style=float:none;width:200px !important; height:150px !important; src=' + mes.data[k].attachments.data[0].image_data.url + '></p></div><p class="small text-muted">' + splDateTime(mes.data[k].created_time) + '</p></div></div>');
                                            }
                                            if (mes.data[k].attachments.data[0].video_data) {
                                                $('.px-4.py-5.chat-box.bg-white').append('<!-- Sender Message--><div class="media mb-3"><div class="media-body ml-3"><div class="rounded py-2 px-3 mb-2" style="max-width: 40%;width: fit-content;"><p class="text-small mb-0 text-muted sendermess"><image style=float:none;width:200px !important; height:150px !important; src=' + mes.data[k].attachments.data[0].video_data.url + '></p></div><p class="small text-muted">' + splDateTime(mes.data[k].created_time) + '</p></div></div>');
                                            }
                                        }
                                        if (mes.data[k].sticker) {
                                            $('.px-4.py-5.chat-box.bg-white').append('<!-- Sender Message--><div class="media mb-3"><div class="media-body ml-3"><div class="rounded py-2 px-3 mb-2" style="max-width: 40%;width: fit-content;"><p class="text-small mb-0 text-muted sendermess"><image style=float:none;width:150px !important; height:150px !important; src=' + mes.data[k].sticker + '></p></div><p class="small text-muted">' + splDateTime(mes.data[k].created_time) + '</p></div></div>');
                                        }
                                        if (mes.data[k].message) {
                                            $('.px-4.py-5.chat-box.bg-white').append('<!-- Sender Message--><div class="media mb-3 test"><div class="media-body ml-3"><div class="bg-light rounded py-2 px-3 mb-2" style="max-width: 40%;width: fit-content;"><p class="text-small mb-0 text-muted sendermess" style="word-wrap:break-word;">' + mes.data[k].message + '</p></div><p class="small text-muted">' + splDateTime(mes.data[k].created_time) + '</p></div></div>');
                                        }

                                    }
                                    else {
                                        if (mes.data[k].attachments) {
                                            if (mes.data[k].attachments.data[0].image_data) {
                                                $('.px-4.py-5.chat-box.bg-white').append('<!-- reciever message--><div class="media ml-auto mb-3"><div class="media-body"><div class="rounded py-2 px-3 mb-2" style="max-width: 40%;width: fit-content;height: fit-content;margin-left: auto;"><p class="text-small mb-0 text-white sendermess"><image style=float:none;width:200px !important; height:150px !important; src=' + mes.data[k].attachments.data[0].image_data.url + '></p></div><p class="small text-muted" style="float:right;">' + splDatetime(mes.data[k].created_time) + '</p></div></div>');
                                            }
                                            if (mes.data[k].attachments.data[0].video_data) {
                                                $('.px-4.py-5.chat-box.bg-white').append('<!-- reciever message--><div class="media ml-auto mb-3"><div class="media-body"><div class="rounded py-2 px-3 mb-2" style="max-width: 40%;width: fit-content;height: fit-content;margin-left: auto;"><p class="text-small mb-0 text-white sendermess"><video style=float:none;width:250px !important; height:200px !important; src=' + mes.data[k].attachments.data[0].video_data.url + '></p></div><p class="small text-muted" style="float:right;">' + splDatetime(mes.data[k].created_time) + '</p></div></div>');
                                            }
                                        }
                                        if (mes.data[k].message) {
                                            $('.px-4.py-5.chat-box.bg-white').append('<!-- reciever message--><div class="media ml-auto mb-3"><div class="media-body"><div class="bg-primary rounded py-2 px-3 mb-2" style="max-width: 40%;width: fit-content;height: fit-content;margin-left: auto;"><p class="text-small mb-0 text-white sendermess" style="word-wrap:break-word;white-space: pre-wrap;">' + mes.data[k].message + '</p></div><p class="small text-muted" style="float:right;">' + splDateTime(mes.data[k].created_time) + '</p></div></div>');
                                        }
                                        if (mes.data[k].sticker) {
                                            $('.px-4.py-5.chat-box.bg-white').append('<!-- reciever message--><div class="media ml-auto mb-3"><div class="media-body"><div class="rounded py-2 px-3 mb-2" style="max-width: 40%;width: fit-content;height: fit-content;margin-left: auto;"><p class="text-small mb-0 text-white sendermess"><image style=float:none;width:150px !important; height:150px !important; src=' + mes.data[k].sticker + '></p></div><p class="small text-muted" style="float:right;">' + splDatetime(mes.data[k].created_time) + '</p></div></div>');
                                        }
                                    }
                                    $('.px-4.py-5.chat-box.bg-white').scrollTop($('.px-4.py-5.chat-box.bg-white')[0].scrollHeight);

                                }
                            }
                        }
                    }
                    //if (mes.data[0].from.id != iduser) {
                    //    $('p#' + iduser).style.fontWeight = "0";
                    //}
                }
            },
            error: function (jqXHR) {
                //alert("RELOAD BỊ LỖI");
                //window.location.href = '/Home/FacebookLogin';
            }

        });
    }
}
//----------------------------------------------------------------KÍCH HOẠT HÀM RELOAD
var reload = setInterval(reload, 3000);

//---------------------------------------------------------Thông báo tin nhắn mới từ fanpage
function pushNoti() {
    // alert("vo");
    var item = 0;
    $.ajax({
        url: '/Home/GetFanpage',
        method: 'POST',
        dataType: 'JSON',
        success: function (response) {
            
            for (var i = 0 ; i < response.length; i++) {
                var url = 'https://graph.facebook.com/' + response[i].id + '?fields=conversations{unread_count,senders,snippet,updated_time,messages.limit(300){attachments{image_data,video_data},message,sticker,from,created_time}}&access_token=' + response[i].access_token;
                $.ajax({
                    url: url,
                    method: 'GET',
                    dataType: 'JSON',
                    success: function (response2) {
                        if (response2.conversations != undefined) {
                            for (var i = 0; i < response2.conversations.data.length; i++) {

                                if ((response2.conversations.data[i].unread_count > 0)) {
                                    /// console.log(response2.conversations.data[i]);
                                    item = item + 1;
                                }
                            }
                        }
                        $('.button__badge').removeAttr('style');
                        //alert(item);
                        $('.button__badge').html(item);
                    }
                });
            }
           
        }
    });
   
}

setInterval(pushNoti,5000);


//----------------------------------------------------------------Cắt chuổi ngày giờ
function splDateTime(datetime) {
    var spl, date, time;
    spl = datetime.split('T');
    date = spl[0].split('-');
    time = spl[1].split(':');
    datee = time[0] + ':' + time[1] + ' | ' + date[2] + '-' + date[1] + '-' + date[0];
    return datee;
}

//----------------------------------------------------------------Lấy avatar khách hàng
function getAvt(idUser, stt) {
    FB.api(
        '/' + idUser, "GET",
        {
            "access_token": token
        },
        function (response) {
            if (response && !response.error) {
                /* handle the result */
                $(".avt." + stt).append('<img class="_87v3 img userimg" alt="" src="' + response.profile_pic + '" height="40" width="40">');
            }
        }
    );
}

//----------------------------------------------------------------Reload avatar khách hàng
function reloadAvt(idUser, i) {
    FB.api(
    '/' + idUser, "GET",
    {
        "access_token": token
    },
    function (response) {
        if (response && !response.error) {
            img = document.getElementsByClassName("userimg");
            img[i].src = response.profile_pic;
            /* handle the result */

        }
    }
);
}

//----------------------------------------------------------------Lấy avatar title
function getAvtTitle(idUser) {
    FB.api(
        '/' + idUser, "GET",
        {
            "access_token": token
        },
        function (response) {
            if (response && !response.error) {
                /* handle the result */
                $(".avttitle." + idUser).append('<img class="_87v3 img" alt="" src="' + response.profile_pic + '" height="40" width="40">');
            }
            else {
                console.log(response);
                //alert("Lấy thất bại");
            }
        }
    );
}


//----------------------------------------------------------------CHECK TỒN TẠI FACEBOOKID
function checkFBID() {
    var FacebookId = localStorage.getItem("idUser");
    var hoten = document.getElementsByClassName("titleHoten");
    var email = document.getElementsByClassName("emailsdt");
    $.ajax({
        type: "POST",
        url: "/Customer/check_FBID",
        data: JSON.stringify({ FacebookId: FacebookId }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (r) {
            console.log(r);
            if (r.Id > 0) {
                document.getElementById("notadd").style.display = "none";
                document.getElementById("added").style.display = "block";
                hoten[0].innerHTML = 'Họ tên: ' + r.CompanyName + ' - ' + r.Code + r.PlayId;
                //email[0].innerHTML = 'Email: ' + r.Email;
                email[0].innerHTML = 'SĐT: ' + r.Mobile;
                localStorage.setItem("MaKH", r.Id);
            }
            else {
                document.getElementById("notadd").style.display = "block";
                document.getElementById("added").style.display = "none";
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            //HideLoading();
            //$("#danger-alert").fadeTo(2000, 500).slideUp(500, function () {
            //    $("#danger-alert").slideUp(500);
            //});
            //alert("Thêm không được");
        }
    });
}

//-----------------------------------------------------------------THÊM KHÁCH HÀNG VÀO DB
$('#save-add').click(function () {
    addCustomer();
    $('#modal1').modal('hide');
    $('#saleModal').modal('show');
});
$('#save').click(function () {
    addCustomer();
    $('#modal1').modal('hide');
});
function addCustomer() {
    var customer = {};
    customer.FirstName = document.getElementById("Ten-KH").value;
    customer.Mobile = document.getElementById("SDT").value;
    customer.Email = document.getElementById("email").value;
    customer.Gender = document.getElementById("Gender").value;
    customer.Birthday = document.getElementById("Ngaysinh").value;
    customer.FacebookId = localStorage.getItem("idUser");
    customer.Address = document.getElementById("diachi").value;
    customer.CityId = document.getElementById("CityId").value;
    customer.DistrictId = document.getElementById("DistrictId").value;
    customer.WardId = document.getElementById("WardId").value;
    $.ajax({
        type: "POST",
        url: "/Customer/Create_POS",
        data: JSON.stringify(customer),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (r) {
            if (r > 0) {
                checkFBID();
            }
            else {
                alert("Thêm không thành công");
            }

        },
        error: function (xhr, ajaxOptions, thrownError) {
            //HideLoading();
            //$("#danger-alert").fadeTo(2000, 500).slideUp(500, function () {
            //    $("#danger-alert").slideUp(500);
            //});
        }
    });

}
//----------------------------------------------------------------Lấy thông tin thành thị
$(function () {
    var url = '/api/BackOfficeServiceAPI/FetchLocation';
    var districts = $('#DistrictId'); // cache it
    var ward = $('#WardId');

    $("#CityId").change(function () {
        var id = $(this).val(); // Use $(this) so you don't traverse the DOM again
        $.getJSON(url, { parentId: id }, function (response) {
            districts.empty(); // remove any existing options
            ward.empty();
            $(document.createElement('option'))
                    .attr('value', '')
                    .text('- Rỗng -')
                    .appendTo(ward);
            $(response).each(function () {
                $(document.createElement('option'))
                    .attr('value', this.Id)
                    .text(this.Name.toLowerCase().replace('huyện', '').replace('quận', ''))
                    .appendTo(districts);
            });

            districts.trigger("chosen:updated");
        });
    });

    districts.change(function () {
        var id = $(this).val(); // Use $(this) so you don't traverse the DOM again
        $.getJSON(url, { parentId: id }, function (response) {
            ward.empty(); // remove any existing options
            $(response).each(function () {
                $(document.createElement('option'))
                    .attr('value', this.Id)
                    .text(this.Name.toLowerCase())
                    .appendTo(ward);
            });
            ward.trigger("chosen:updated");
        });
    });
});
//Kết thúc lấy thông tin thành thị
//----------------------------------------------------------------PHẦN BÌNH LUẬN

//----------------------------------------------------------------Lấy các bài đăng
function getPageContent() {
   
    localStorage.setItem("idUser", " ");
    localStorage.removeItem("MaKH");
    $("div").remove('.list-group.mess');
    $('div').remove('.list-group.comm');
    $('div.px-4.py-5.chat-box.bg-white').remove();
    $('div.bg-light').remove();
    $('div._673w._6ynl').remove();
    $("div").remove('.page-empty.comm');
    document.getElementById("nomsg-view").style.display = "none";
    document.getElementById("nomsg-viewpost").style.display = "block";
    document.getElementById("TTKH").style.display = "none";
    document.getElementById("notadd").style.display = "block";
    document.getElementById("added").style.display = "none";
    var url = 'https://graph.facebook.com/' + idpage + '/feed?access_token=' + token;
    $.ajax({
        url: url,
        method: 'GET',
        dataType: 'JSON',
        success: function (response) {
            for (var i = 0; i < response.data.length; i++) {
                var content = response.data[i];
                spl = content.created_time.split('T');
                date = spl[0].split('-');
                time = spl[1].split(':');
                if (content.story != null) {
                    $('.messages-box.comm').append('<div class="list-group comm rounded-0" id="' + content.id + '" onclick="showComment(this.id)"><a class="list-group-item list-group-item-action rounded-0" style="display: flex;"><div style="height: 40px; width: 40px;"></div><div class="media" style="width: 100%;"><div class="media-body ml-4" style="line-height: 1.5; margin-left: 1rem !important;"><div class="d-flex align-items-center justify-content-between mb-1"><h5 class="mb-0">' + content.story + '</h5><small class="small">' + time[0] + ':' + time[1] + ' | ' + date[2] + '-' + date[1] + '</small></div><p class="font-italic mb-0 text-small ' + i + '"></p></div></div></a></div>');

                } else {
                    $('.messages-box.comm').append('<div class="list-group comm rounded-0" id="' + content.id + '" onclick="showComment(this.id)"><a class="list-group-item list-group-item-action rounded-0" style="display: flex;"><div style="height: 40px; width: 40px;"></div><div class="media" style="width: 100%;"><div class="media-body ml-4" style="line-height: 1.5; margin-left: 1rem !important;"><div class="d-flex align-items-center justify-content-between mb-1"><h5 class="mb-0">' + "Bài đăng" + '</h5><small class="small">' + time[0] + ':' + time[1] + ' | ' + date[2] + '-' + date[1] + '</small></div><p class="font-italic mb-0 text-small ' + i + '"></p></div></div></a></div>');

                }
                  $('.font-italic.mb-0.text-small.' + i).append(content.message);
            }
        },
        error: function (jqXHR) {
            alert("LỖI RỒI BẠN ƠIIII");
        }
    });
}

//HIỂN THỊ CHI TIẾT BÀI ĐĂNG
function showComment(clicked_id) {
    $('div.px-4.py-5.chat-box.bg-white').remove();
    $('div.bg-light').remove();
    $('div._673w._6ynl').remove();
    document.getElementById("nomsg-viewpost").style.display = "none";
    document.getElementById("TTKH").style.display = "block";
    document.getElementById("notadd").style.display = "none";
    document.getElementById("TTKH").style.animation = "slideIn 0.35s";
    var x = clicked_id;
    localStorage.setItem("idPost", x);
    //Title
    $('.col-8.px-0').append('<div class="_673w _6ynl"><div class="_5743 _6y4y"><div aria-hidden="true" style="padding-bottom: 0.5px;"><div class="_6ynm _87u_"><div style="height: 40px; width: 40px;"><img class="_87v3 img" alt="" src="https://scontent.fvca1-2.fna.fbcdn.net/v/t1.0-1/cp0/p50x50/106243546_109497047483353_1218262410537704277_o.png?_nc_cat=104&_nc_sid=dbb9e7&_nc_ohc=Eh6V_d3gNtUAX-z05AO&_nc_ht=scontent.fvca1-2.fna&oh=42551e023b7cf2f4b4c7c38bca27bd86&oe=5F3704C5" height="40" width="40"></div></div></div><div class="_6ynn"><h5 class="_17w2 _6ybr" id="js_5" style="margin-left: 10%; margin-top: 5%;"><span class="_3oh-"></span></h5><div class="user-type"></div></div></div><button type="button" class="btn btn-info btn-lg publish" data-toggle="modal" data-target="#postModal">Đăng</button><i class="fa fa-shopping-cart" id="hideKH" onclick="clickme(); " aria-hidden="true"></i></div>');
    //Show comment
    $('.col-8.px-0').append('<div class="px-4 py-5 chat-box bg-white"></div>');
    //Bình luận
    $('.col-8.px-0').append('<div class="bg-light replycmt"><div class="input-group"><input type="text" id ="cmts" placeholder="Viết bình luận tại đây..." onkeypress ="return enterCmt(event);" class="form-control rounded-0 border-0 py-4 bg-light"><div class="input-group-append" ><button type="submit" onclick="sendCmt();" class="btn btn-link"> <i class="fa fa-paper-plane" id ="send" ></i></button></div></div>');
    url = 'https://graph.facebook.com/' + idpage + '?fields=feed{message,full_picture,created_time,permalink_url,comments{created_time,message,from,comments{from,message,created_time,likes{name}},likes{name}}}&access_token=' + token;
    $.ajax({
        url: url,
        method: 'GET',
        dataType: 'JSON',
        success: function (response) {
            var splp, splcmt, spltcmt1, date, time, cmt;
            console.log(response);
            for (var i = 0; i < response.feed.data.length; i++) {
                cmt = response.feed.data[i];
                splp = cmt.created_time.split('T');
                date = splp[0].split('-');
                time = splp[1].split(':');
                //Hiển thị bài đăng
                if (cmt.id == x) {
                    $('.px-4.py-5.chat-box.bg-white').append('<!-- Comment--><div class="post mb-3" style="border-bottom: 1px solid;"><div class="post-body"><div class="rounded py-2 px-3 mb-2" style="width:100%"><p class="text-post mb-0 ' + i + '" style="word-wrap:break-word;font-size:30px;">' + cmt.message + '</p><p class="post-action">Đăng : ' + time[0] + ':' + time[1] + ' | ' + date[2] + '-' + date[1] + '</p><p class="post-action link"><a href="' + cmt.permalink_url + '" target="_blank">Xem bài viết gốc</p></div></div></div>');
                    if (cmt.full_picture) {
                        $('.text-post.mb-0.' + i).after('<p><img class="post-img" src="' + cmt.full_picture + '"></p>');
                    }
                    //Nếu tồn tại bình luận thì hiển thị bình luận
                    if (cmt.comments) {
                        for (var j = cmt.comments.data.length - 1 ; j >= 0; j--) {
                            var cmt1 = cmt.comments.data[j];
                            if (cmt1.from.id == idpage) {
                                $('.post.mb-3').after('<div class="comments father w-100 mb-3 ml-3 ' + j + '"><!-- Comment--><div class="comments-body father"><div class="rounded py-2 px-3 mb-2 ml-2 father" id="' + cmt.comments.data[j].id + '" style="width:100%"><p class="text-post mb-0 " style="word-wrap:break-word;font-size:15px;">' + cmt.comments.data[j].message + '</p><p class="post-action like father" id="' + cmt.comments.data[j].id + '" onclick="like(this.id);">Thích</p><p class="post-action update ml-1" id="' + cmt.comments.data[j].id + '" onclick = "tofocus(this.id);">Chỉnh sửa</p><p class="post-action delete ml-1" id="' + cmt.comments.data[j].id + '" onclick="deletePost(this.id);">Xóa</p><p class="post-action cmtreply ml-1" id="' + cmt.comments.data[j].id + '" onclick = "focusReply(this.id);">Trả lời</p><p class="post-action time ml-3">' + splDateTime(cmt.comments.data[j].created_time) + '</p></div></div></div>');
                                $('#' + cmt.comments.data[j].id).before('<img src="https://scontent.fvca1-2.fna.fbcdn.net/v/t1.0-1/cp0/p50x50/106243546_109497047483353_1218262410537704277_o.png?_nc_cat=104&_nc_sid=dbb9e7&_nc_ohc=Eh6V_d3gNtUAX-z05AO&_nc_ht=scontent.fvca1-2.fna&oh=42551e023b7cf2f4b4c7c38bca27bd86&oe=5F3704C5" alt="user" width="50" height="50" class="rounded-circle">');
                            }
                            else {
                                $('.post.mb-3').after('<div class="comments father w-100 mb-3 ml-3 ' + j + '"><!-- Comment--><div class="comments-body father"><div class="rounded py-2 px-3 mb-2 ml-2 father" id="' + cmt.comments.data[j].id + '" style="width:100%"><p class="text-post mb-0 " style="word-wrap:break-word;font-size:15px;">' + cmt.comments.data[j].message + '</p><p class="post-action like father" id="' + cmt.comments.data[j].id + '" onclick="like(this.id);">Thích</p><p class="post-action delete ml-1" id="' + cmt.comments.data[j].id + '" onclick="deletePost(this.id);">Xóa</p><p class="post-action cmtreply ml-1" id="' + cmt.comments.data[j].id + '" onclick = "focusReply(this.id);">Trả lời</p><p class="post-action time ml-3">' + splDateTime(cmt.comments.data[j].created_time) + '</p></div></div></div>');
                                $('#' + cmt.comments.data[j].id).before('<img src="https://scontent.fvca1-2.fna.fbcdn.net/v/t1.0-1/cp0/p50x50/106243546_109497047483353_1218262410537704277_o.png?_nc_cat=104&_nc_sid=dbb9e7&_nc_ohc=Eh6V_d3gNtUAX-z05AO&_nc_ht=scontent.fvca1-2.fna&oh=42551e023b7cf2f4b4c7c38bca27bd86&oe=5F3704C5" alt="user" width="50" height="50" class="rounded-circle">');
                            }
                            //Nếu tôn tại bình luận con
                            if (cmt1.comments) {
                                for (var k = cmt1.comments.data.length - 1; k >= 0; k--) {
                                    if (cmt1.comments.data[k].from.id == idpage) {
                                        $('.comments.father.w-100.' + j).append('<div class="comments children w-100 ' + cmt.comments.data[j].id + '"><!-- Comment--><div class="comments-body children"><div class="rounded py-2 px-3 children" id="' + cmt1.comments.data[k].id + '" style="width:100%"><p class="text-post mb-0 " style="word-wrap:break-word;font-size:15px;">' + cmt1.comments.data[k].message + '</p><p class="post-action like children" id="' + cmt1.comments.data[k].id + '" onclick="like(this.id);">Thích</p><p class="post-action update ml-1" id="' + cmt1.comments.data[k].id + '" onclick = "tofocus(this.id);">Chỉnh sửa</p><p class="post-action delete ml-1" id="' + cmt1.comments.data[k].id + '" onclick="deletePost(this.id);">Xóa</p><p class="post-action time ml-3">' + splDateTime(cmt1.comments.data[k].created_time) + '</p></div></div></div>');
                                        $('#' + cmt1.comments.data[k].id).before('<img src="https://scontent.fvca1-2.fna.fbcdn.net/v/t1.0-1/cp0/p50x50/106243546_109497047483353_1218262410537704277_o.png?_nc_cat=104&_nc_sid=dbb9e7&_nc_ohc=Eh6V_d3gNtUAX-z05AO&_nc_ht=scontent.fvca1-2.fna&oh=42551e023b7cf2f4b4c7c38bca27bd86&oe=5F3704C5" alt="user" width="50" height="50" class="rounded-circle">');
                                    }
                                    else {
                                        $('.comments.father.w-100.' + j).append('<div class="comments children w-100 ' + cmt.comments.data[j].id + '"><!-- Comment--><div class="comments-body children"><div class="rounded py-2 px-3 children" id="' + cmt1.comments.data[k].id + '" style="width:100%"><p class="text-post mb-0 " style="word-wrap:break-word;font-size:15px;">' + cmt1.comments.data[k].message + '</p><p class="post-action like children" id="' + cmt1.comments.data[k].id + '" onclick="like(this.id);">Thích</p><p class="post-action delete ml-1" id="' + cmt1.comments.data[k].id + '" onclick="deletePost(this.id);">Xóa</p><p class="post-action time ml-3">' + splDateTime(cmt1.comments.data[k].created_time) + '</p></div></div></div>');
                                        $('#' + cmt1.comments.data[k].id).before('<img src="https://scontent.fvca1-2.fna.fbcdn.net/v/t1.0-1/cp0/p50x50/106243546_109497047483353_1218262410537704277_o.png?_nc_cat=104&_nc_sid=dbb9e7&_nc_ohc=Eh6V_d3gNtUAX-z05AO&_nc_ht=scontent.fvca1-2.fna&oh=42551e023b7cf2f4b4c7c38bca27bd86&oe=5F3704C5" alt="user" width="50" height="50" class="rounded-circle">');
                                    }
                                }
                            }
                        }
                    }
                }
            }
        },
        error: function (jqXHR) {
            alert("SHOW COMMENTS BỊ LỖI");
        }

    });
}
//----------------------------------------------------------------COMMENTS
function enterCmt(event) {
    if (event.keyCode == 13) {
        var x = document.getElementById("cmts").value;
        id = localStorage.getItem("idPost");
        debugger
        //var message = CKEDITOR.instances.msg.getData();
        FB.api(
            "/" + id + "/comments", "POST",
            {
                "message": x,
                "access_token": token
            },
            function (response) {
                if (response && !response.error) {
                    //reloadTest();
                    //reloadUser();
                    document.getElementById('cmts').value = '';
                }
                else {
                    console.log(response);
                }
            }
        );
    }
}
function sendCmt() {
    var x = document.getElementById("cmts").value;
    debugger
    id = localStorage.getItem("idPost");
    FB.api(
        "/" + id + "/comments", "POST",
        {
            "message": x,
            "access_token": token
        },
        function (response) {
            if (response && !response.error) {
                //reloadTest();
                //reloadUser();
                document.getElementById('cmts').value = '';
            }
            else {
                console.log(response);
            }
        }
    );
}
function tofocus(id) {
    $('.replycmt').remove();
    $('.chat-box.bg-white').after('<div class="bg-light replycmt"><div class="input-group"><input type="text" id ="cmts" placeholder="Viết bình luận tại đây..." onkeypress ="return enterreplyCmt(event);" class="form-control rounded-0 border-0 py-4 bg-light"><div class="input-group-append" ><button type="submit" onclick="updateCmt();" class="btn btn-link"> <i class="fa fa-paper-plane" id ="send" ></i></button></div></div>');
    $('#cmts').focus();
    localStorage.setItem("idPost", id);
    console.log(id);
}
function focusReply(id) {
    $('#cmts').focus();
    localStorage.setItem("idPost", id);
}
function enterreplyCmt(event) {
    if (event.keyCode == 13) {
        var x = document.getElementById("cmts").value;
        id = localStorage.getItem("idPost");
        debugger
        //var message = CKEDITOR.instances.msg.getData();
        FB.api(
            "/" + id, "POST",
            {
                "message": x,
                "access_token": token
            },
            function (response) {
                if (response && !response.error) {
                    $('.replycmt').remove();
                    $('.chat-box.bg-white').after('<div class="bg-light replycmt"><div class="input-group"><input type="text" id ="cmts" placeholder="Viết bình luận tại đây..." onkeypress ="return enterCmt(event);" class="form-control rounded-0 border-0 py-4 bg-light"><div class="input-group-append" ><button type="submit" onclick="sendCmt();" class="btn btn-link"> <i class="fa fa-paper-plane" id ="send" ></i></button></div></div>');
                }
                else {
                    console.log(response);
                }
            }
        );
    }
}
//----------------------------------------------------------------CHỈNH SỬA COMMENT
function updateCmt() {
    var x = document.getElementById("cmts").value;
    debugger
    id = localStorage.getItem("idPost");
    FB.api(
        "/" + id, "POST",
        {
            "message": x,
            "access_token": token
        },
        function (response) {
            if (response && !response.error) {
                $('.replycmt').remove();
                $('.chat-box.bg-white').after('<div class="bg-light replycmt"><div class="input-group"><input type="text" id ="cmts" placeholder="Viết bình luận tại đây..." onkeypress ="return enterCmt(event);" class="form-control rounded-0 border-0 py-4 bg-light"><div class="input-group-append" ><button type="submit" onclick="sendCmt();" class="btn btn-link"> <i class="fa fa-paper-plane" id ="send" ></i></button></div></div>');
            }
            else {
                console.log(response);
            }
        }
    );
}

//----------------------------------------------------------------XÓA COMMENT
function deletePost(id) {
    debugger
    FB.api(
    "/" + id,
    "DELETE",
    {
        "access_token": token
    },
    function (response) {
        if (response && !response.error) {
            /* handle the result */
            alert("Đã xóa bài đăng");
        }
        else {
            console.log(response);
        }
    }
);
}

//----------------------------------------------------------------ĐĂNG BÀI
function publishPost() {
    $('#postModal').hide('modal');
    $('.modal-backdrop').remove()
    var x = document.getElementById("post").value;
    debugger
    id = localStorage.getItem("idPage");
    FB.api(
        "/" + id + "/feed", "POST",
        {
            "message": x,
            "access_token": token
        },
        function (response) {
            if (response && !response.error) {
                document.getElementById('post').value = '';
                alert("Đã đăng");
                console.log(response);
            }
            else {
                console.log(response);
            }
        }
    );
}



function logoutFB() {
    FB.init({
        appId: '247660513180458',
        cookie: true,                     // Enable cookies to allow the server to access the session.
        xfbml: true,                     // Parse social plugins on this webpage.
        version: 'v7.0'           // Use this Graph API version for this call.
    });
    FB.getLoginStatus(handleSessionResponse);
    function handleSessionResponse(response) {
        //if we dont have a session (which means the user has been logged out, redirect the user)
        if (!response.session) {
            window.location = "/Home/FacebookLogin";
            return;
        }

        //if we do have a non-null response.session, call FB.logout(),
        //the JS method will log the user out of Facebook and remove any authorization cookies
        FB.logout(handleSessionResponse);
    }
}

function reloadCMT() {
    var listgroup = document.getElementsByClassName("list-group comm");
    var cmtfather = document.getElementsByClassName("comments father");
    if (listgroup.length != 0) {
        url = 'https://graph.facebook.com/' + idpage + '?fields=feed{message,full_picture,created_time,permalink_url,comments{created_time,message,from,comments{from,message,created_time}}}&access_token=' + token;
        $.ajax({
            url: url,
            method: 'GET',
            dataType: 'JSON',
            success: function (response) {
                if (response.feed.data.length != listgroup.length) {
                    getPageContent();
                }
                for (var i = 0; i < response.feed.data.length; i++) {
                    var post = response.feed.data[i];
                    if (post.id == idpost) {
                        if ((cmtfather.length != post.comments.data.length) && (cmtfather.length > 0)) {
                            var count = post.comments.data.length - cmtfather.length;
                            if (count > 0) {
                                for (var j = count - 1; j >= 0; j--) {
                                    var cmt1 = post.comments.data[j];
                                    if (cmt1.from.id == idpage) {
                                        $('.post.mb-3').after('<div class="comments father w-100 mb-3 ml-3 ' + j + '"><!-- Comment--><div class="comments-body father"><div class="rounded py-2 px-3 mb-2 ml-2 father" id= "' + post.comments.data[j].id + '" style="width:100%"><p class="text-post mb-0 " style="word-wrap:break-word;font-size:15px;">' + post.comments.data[j].message + '</p><p class="post-action like father">Thích</p><p class="post-action update ml-1" id="' + post.comments.data[j].id + '" onclick = "tofocus(this.id);">Chỉnh sửa</p><p class="post-action delete ml-1" id="' + post.comments.data[j].id + '" onclick="deletePost(this.id);">Xóa</p><p class="post-action time ml-3">' + splDateTime(post.comments.data[j].created_time) + '</p></div></div></div>');
                                        $('#' + post.comments.data[j].id).before('<img src="https://scontent.fvca1-2.fna.fbcdn.net/v/t1.0-1/cp0/p50x50/106243546_109497047483353_1218262410537704277_o.png?_nc_cat=104&_nc_sid=dbb9e7&_nc_ohc=Eh6V_d3gNtUAX-z05AO&_nc_ht=scontent.fvca1-2.fna&oh=42551e023b7cf2f4b4c7c38bca27bd86&oe=5F3704C5" alt="user" width="50" height="50" class="rounded-circle">');
                                    }
                                    else {
                                        $('.post.mb-3').after('<div class="comments father w-100 mb-3 ml-3 ' + j + '"><!-- Comment--><div class="comments-body father"><div class="rounded py-2 px-3 mb-2 ml-2 father" id="' + post.comments.data[j].id + '" style="width:100%"><p class="text-post mb-0 " style="word-wrap:break-word;font-size:15px;">' + post.comments.data[j].message + '</p><p class="post-action like father">Thích</p><p class="post-action delete ml-1" id="' + post.comments.data[j].id + '" onclick="deletePost(this.id);">Xóa</p><p class="post-action time ml-3">' + splDateTime(post.comments.data[j].created_time) + '</p></div></div></div>');
                                        $('#' + post.comments.data[j].id).before('<img src="https://scontent.fvca1-2.fna.fbcdn.net/v/t1.0-1/cp0/p50x50/106243546_109497047483353_1218262410537704277_o.png?_nc_cat=104&_nc_sid=dbb9e7&_nc_ohc=Eh6V_d3gNtUAX-z05AO&_nc_ht=scontent.fvca1-2.fna&oh=42551e023b7cf2f4b4c7c38bca27bd86&oe=5F3704C5" alt="user" width="50" height="50" class="rounded-circle">');
                                    }
                                }
                            }
                        }
                        //for (var m = 0; m < post.comments.data.length; m++) {
                        //    //var cmtchildren = document.getElementsByClassName('comments children w-100 123499999416391_191020722664318');
                        //    var cmt1 = post.comments.data[m];
                        //    if (cmt1.comments) {
                        //        var cmtchildren = document.getElementsByClassName('comments children '+ post.comments.data[m].id);
                        //        if ((cmtchildren.length != cmt1.comments.data.length) && (cmtchildren.length > 0)) {
                        //            var count = cmt1.comments.data.length - cmtchildren.length;
                        //            if (count > 0) {
                        //                for (var k = count - 1; k >= 0; k--) {
                        //                    if (cmt1.comments.data[k].from.id == idpage) {
                        //                        $('.comments.father.w-100.' + m).append('<div class="comments children w-100"><!-- Comment--><div class="comments-body children"><div class="rounded py-2 px-3 children" id="' + cmt1.comments.data[m].id + '" style="width:100%"><p class="text-post mb-0 " style="word-wrap:break-word;font-size:15px;">' + cmt1.comments.data[k].message + '</p><p class="post-action like children">Thích</p><p class="post-action update ml-1" id="' + cmt1.comments.data[k].id + '" onclick = "tofocus(this.id);">Chỉnh sửa</p><p class="post-action delete ml-1" id="' + cmt1.comments.data[k].id + '" onclick="deletePost(this.id);">Xóa</p><p class="post-action time ml-3">' + splDateTime(cmt1.comments.data[k].created_time) + '</p></div></div></div>');
                        //                        $('#' + cmt1.comments.data[k].id).before('<img src="https://scontent.fvca1-2.fna.fbcdn.net/v/t1.0-1/cp0/p50x50/106243546_109497047483353_1218262410537704277_o.png?_nc_cat=104&_nc_sid=dbb9e7&_nc_ohc=Eh6V_d3gNtUAX-z05AO&_nc_ht=scontent.fvca1-2.fna&oh=42551e023b7cf2f4b4c7c38bca27bd86&oe=5F3704C5" alt="user" width="50" height="50" class="rounded-circle">');
                        //                    }
                        //                    else {
                        //                        $('.comments.father.w-100.' + m).append('<div class="comments children w-100"><!-- Comment--><div class="comments-body children"><div class="rounded py-2 px-3 children" id="' + cmt1.comments.data[m].id + '" style="width:100%"><p class="text-post mb-0 " style="word-wrap:break-word;font-size:15px;">' + cmt1.comments.data[k].message + '</p><p class="post-action like children">Thích</p><p class="post-action delete ml-1" id="' + cmt1.comments.data[k].id + '" onclick="deletePost(this.id);">Xóa</p><p class="post-action time ml-3">' + splDateTime(cmt1.comments.data[k].created_time) + '</p></div></div></div>');
                        //                        $('#' + cmt1.comments.data[k].id).before('<img src="https://scontent.fvca1-2.fna.fbcdn.net/v/t1.0-1/cp0/p50x50/106243546_109497047483353_1218262410537704277_o.png?_nc_cat=104&_nc_sid=dbb9e7&_nc_ohc=Eh6V_d3gNtUAX-z05AO&_nc_ht=scontent.fvca1-2.fna&oh=42551e023b7cf2f4b4c7c38bca27bd86&oe=5F3704C5" alt="user" width="50" height="50" class="rounded-circle">');
                        //                    }
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                    }
                }
            },
            error: function (response) {
                alert("lỗi reload cmt");
            }

        });
    }
}
//var loadCMT = setInterval(reloadCMT, 3000);

$(document).ready(function () {
    $('#modalBtn2').click(function () {
        $('#saleModal').show('modal');
        $('.modal-backdrop').remove();

    })
});

//----------------------------------------------------------------Tính tiền theo giảm giá
$(document).ready(function () {
    $('input[type="checkbox"]').click(function () {
        var tienhang = document.getElementById('PriceDiscount1');
        var giamgia = $('#Discount').val();
        var thanhtien;
        var giamtheo;
        var check = document.getElementById('toggle1');
        if (giamgia > 100 && check.checked == true) {
            giamgia = 100;
            $('#Discount').val(100);
        }
        if ($(this).is(":checked")) {
            giamtheo = '%';
        }
        else if ($(this).is(":not(:checked)")) {
            giamtheo = 'vnd';
        }
        if (giamgia == "") {
            giamgia = 0;
        }
        if (giamtheo == '%') {
            thanhtien = parseInt(tienhang.innerHTML) - (parseInt(giamgia) * parseInt(tienhang.innerHTML)) / 100;
        }
        if (giamtheo == 'vnd') {
            thanhtien = parseInt(tienhang.innerHTML) - parseInt(giamgia);
        }
        $("#ThanhToan").val(thanhtien.toLocaleString('vi', { style: 'currency', currency: 'VND' }));
        $("#ThanhToan1").val(thanhtien);
    })

});
$('#Discount').change(function () {
    var tienhang = document.getElementById('PriceDiscount1');
    var giamgia = $('#Discount').val();
    var thanhtien;
    var giamtheo;
    var check = document.getElementById('toggle1');
    if (giamgia > 100 && check.checked == true) {
        giamgia = 100;
        $('#Discount').val(100);
    }
    if (check.checked == true) {
        giamtheo = '%';
    }
    else {
        giamtheo = 'vnd';
    }
    if (giamgia == "") {
        giamgia = 0;
    }
    if (giamtheo == '%') {
        thanhtien = parseInt(tienhang.innerHTML) - (parseInt(giamgia) * parseInt(tienhang.innerHTML)) / 100;
    }
    if (giamtheo == 'vnd') {
        thanhtien = parseInt(tienhang.innerHTML) - parseInt(giamgia);
    }
    $("#ThanhToan").val(thanhtien.toLocaleString('vi', { style: 'currency', currency: 'VND' }));
    $("#ThanhToan1").val(thanhtien);

});
$('#totalPrice tr td').on("DOMSubtreeModified", function () {
    debugger
    var tienhang = document.getElementById('PriceDiscount');
    var tienhang1 = document.getElementById('PriceDiscount1');
    var giamgia = $('#Discount').val();
    var thanhtien;
    var giamtheo;
    var check = document.getElementById('toggle1');
    if (check.checked == true) {
        giamtheo = '%';
    }
    else {
        giamtheo = 'vnd';
    }
    if (giamgia == "") {
        giamgia = 0;
    }
    if (giamtheo == '%') {
        thanhtien = parseInt(tienhang1.innerHTML) - (parseInt(giamgia) * parseInt(tienhang1.innerHTML)) / 100;
    }
    if (giamtheo == 'vnd') {
        thanhtien = parseInt(tienhang1.innerHTML) - parseInt(giamgia);
    }
    $("#ThanhToan").val(thanhtien.toLocaleString('vi', { style: 'currency', currency: 'VND' }));
    $("#ThanhToan1").val(thanhtien);
    $("#TienHang").val(tienhang.innerText.toLocaleString('vi', { style: 'currency', currency: 'VND' }));
});
//----------------------------------------------------------------Kết thúc tính tiền

//----------------------------------------------------------------TẠO ĐƠN HÀNG
$('.btn.Sale').click(function () {
    Sale();
    $('#discountModal').modal('hide');
    $('#saleModal').modal('hide');
    $(".detailList").find("tr").remove();
    $('#TongSoLuong').html("");
    $('#TongThanhTien').html("");
    $('#PriceDiscount').html("");
    $('#ProductItemCount').html("");
    $('#Discount').val("");
    $('#ThanhToan1').val("");
    $('#ThanhToan').val("");

});
function Sale() {
    debugger
    var customers = new Array();
    $("#listOrderDetail1 TBODY TR").each(function () {
        debugger
        var row = $(this);
        var customer = {};
        customer.ProductId = $(this).closest('tr').find("td:eq(1) input").val();
        customer.Quantity = $(this).closest('tr').find("td:eq(2) input:nth-child(2)").val();
        customer.Price = removeComma($(this).closest('tr').find("td:eq(3) input").val());
        var discountitem = removeComma($(this).closest('tr').find("td:eq(5) input:nth-child(1)").val());
        var check = $(this).closest('tr').find("td:eq(5) input");
        var checked = ToggleDiscount(check[1].id);
        if (checked == 1) {
            customer.IrregularDiscount = discountitem; //Giam theo %
        }
        else {
            var tmpdiscountchild = $(this).closest('tr').find("td:eq(5) input:nth-child(1)").val();
            customer.IrregularDiscountAmount = removeComma(tmpdiscountchild); //Giam theo tien mat
        }




        customers.push(customer);
    });
    if (customers.length == 0) {
        //$("#danger-alert").fadeTo(2000, 500).slideUp(500, function () {
        //    $("#danger-alert").slideUp(500);
        //});
        keydown = false;
        return;
    }
    var customerId = 0;
    var IsDisAll;
    if (localStorage.getItem("MaKH") != null) {
        customerId = parseInt(localStorage.getItem("MaKH"));
    }
    var countfoBrand = $('#CountForBrand').val();
    var DiscountAll = $('#Discount').val();
    var Hinhthuctt = "Tien mat";
    var MoneyPaytab = 0;
    var TransferPaytab = 0;
    var ATMPaytab = 0;
    var NotPrint = false;
    var IS_ONLINE = true;
    var NoteHDtab = $('#noteHD').val();
    var check = document.getElementById('toggle1');
    if (check.checked == true) {
        IsDisAll = "1";
    }
    else {
        IsDisAll = "0";
    }
    ShowLoading();
    $.ajax({
        type: "POST",
        url: "/ProductInvoice/CreatePOSInDathang",
        data: JSON.stringify({ listsp: customers, customerId: customerId, DiscountAll: DiscountAll, Hinhthuctt: Hinhthuctt, MoneyPaytab: MoneyPaytab, TransferPaytab: TransferPaytab, ATMPaytab: ATMPaytab, NotPrint: NotPrint, NoteHDtab: NoteHDtab, IsDisAll: IsDisAll, IS_ONLINE: IS_ONLINE, CountForBrand : countfoBrand }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (r) {
            if (r != 0) {
                alert('Đã thêm một đơn hàng mới');
                keydown = false;
                HideLoading();
            }

        },
        error: function (xhr, ajaxOptions, thrownError) {
            debugger
            alert('Loi');
            HideLoading();


        }
    });
}
//----------------------------------------------------------------Like
function like(idpost) {
    FB.api(
    "/" + idpost + "/likes",
    "POST",
    {
        "access_token": token

    },
    function (response) {
        if (response && !response.error) {
            console.log(response);
        }
        else {
            console.log(response)
        }
    }
);
}

//----------------------------------------------------------------Ẩn tab TTKH
var click = 0;
function clickme() {
    click += 1;
    if (click % 2 != 0) {
        $(".container.col-8.px-4.py-5").removeClass("col-8");
        $(".container.px-4.py-5").addClass("col-12");
        $(".empty").addClass("col-8");
        document.getElementById("nomsg-viewpost").style.left = "33%";
        document.getElementById("nomsg-view").style.left = "33%";
        document.getElementById("TTKH").style.display = "none";
    }
    else {
        $(".container.px-4.py-5").removeClass("col-12");
        $(".container.px-4.py-5").addClass("col-8");
        $(".empty").removeClass("col-8");
        document.getElementById("TTKH").style.display = "block";
        document.getElementById("nomsg-viewpost").style.left = "22%";
        document.getElementById("nomsg-view").style.left = "22%";
        document.getElementById("TTKH").style.animation = "slideIn 0.35s";
    }
}
//----------------------------------------------------------------Gửi tin nhắn trả lời riêng
function privateReply() {
    FB.api(
    '/123499999416391_195622925537431/private_replies',
    "POST",
    {
        "message": 'test',
        'access_token': token
    }
    ,
    function (response) {
        if (response && !response.error) {
            /* handle the result */
            alert("send ok");
        }
    }
);
}
