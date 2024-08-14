function changeStatus(id, status, note,moretime) {
    ShowLoading();
    //alertPopup("Lỗi", "Bạn đang click vào hoàn thành", "warning");
    if ((status == "inprogress") || (status == "complete")) {
        if (confirm('Bạn có chắc muốn thực hiện thao tác đã chọn ?')) {
            $.post("/SchedulingHistory/ChangeStatus?id=" + id + "&status=" + status + "&note=" + note + "&moretime=" + moretime)
    .done(function (data) {
        if (data != null) {
            debugger
            $("#info_customer_" + data.BedId).data('id', data.Id).data('room-id', data.BedId).data('customerName', data.CustomerName).data('nameNV', data.NameNV);
            $("#status_complete_" + data.BedId).data('id', data.Id).data('room-id', data.BedId);
            $("#status_remove_" + data.BedId).data('id', data.Id).data('room-id', data.BedId);
            $("#status_plustime_" + data.BedId).data('id', data.Id).data('room-id', data.BedId);
            $("#countdown_" + data.BedId).data('a', data.Name_Bed);
            if (data.Status == "inprogress") {
                $("#countdown_" + data.BedId).countdown(data.strEndDate, data.Name_Bed, data.Name_Room, function (event) {
                    $(this).text(event.strftime('%H:%M:%S')
                );
                }).on('finish.countdown', function () {
                    changeStatus(data.Id, "expired", "");
                });

                // $("#status_room_" + data.RoomId).removeClass("label-info").addClass(data.ColorStatus).text(data.strStatus);
                $("#scheduling_" + data.Id).closest('div').remove();
                $("#CustomerName_" + data.RoomId).text(data.CustomerName);
                $('img[name="img_' + data.RoomId + '"]').attr("src", data.CustomerImagePath);
                $("#aImg_" + data.RoomId).attr("href", data.CustomerImagePath);
                if (note == "") {
                    var total = parseInt($("#total_da_xep_lich").text()) - 1;
                    $("#total_da_xep_lich").text(total);
                }
                $("#status_room_" + data.RoomId).removeClass("label-danger").removeClass("label-success").removeClass("label-info").removeClass("label-warning").addClass(data.ColorStatus).text(data.strStatus);
                $("#countdown_" + data.BedId).removeClass("label-danger").removeClass("label-success").removeClass("label-info").removeClass("label-warning").addClass(data.ColorStatus);

            }
            if (data.Status == "complete") {
                $("#countdown_" + data.BedId).countdown(data.strEndDate, data.Name_Bed, data.Name_Room, function (event) {
                    $(this).text(event.strftime('%H:%M:%S')
                    );
                }).on('finish.countdown', function () {
                    changeStatus(data.Id, "expired", "");
                });

                // $("#status_room_" + data.RoomId).removeClass("label-info").addClass(data.ColorStatus).text(data.strStatus);
                $("#scheduling_" + data.Id).closest('div').remove();
                $("#CustomerName_" + data.RoomId).text(data.CustomerName);
                $('img[name="img_' + data.RoomId + '"]').attr("src", data.CustomerImagePath);
                $("#aImg_" + data.RoomId).attr("href", data.CustomerImagePath);
                if (note == "" && data.Status == "inprogress") {
                    var total = parseInt($("#total_da_xep_lich").text()) - 1;
                    $("#total_da_xep_lich").text(total);
                }
                $("#status_room_" + data.RoomId).removeClass("label-danger").removeClass("label-success").removeClass("label-info").removeClass("label-warning").addClass(data.ColorStatus).text(data.strStatus);
                $("#countdown_" + data.BedId).removeClass("label-danger").removeClass("label-success").removeClass("label-info").removeClass("label-warning").addClass(data.ColorStatus);
                window.location.reload(true);
            }
           

        }
        else {
            alertPopup("Lỗi", "Dữ liệu tìm không thấy!", "warning");
        }
        HideLoading();
    });
            return false;
        }
        else {
            HideLoading();
            return false;
        }
    }
    else {
        HideLoading();
    }

}


function DeleteById(id) {
    ShowLoading();
    //alertPopup("Lỗi", "Bạn đang click vào hoàn thành", "warning");
    if (confirm('Bạn có chắc muốn thực hiện xóa khách chăm sóc đã chọn không ?')) {
        $.post("/SchedulingHistory/DeleteById?id=" + id)
.done(function (data) {
    if (data != null) {
        location.reload(true);
    }
    else {
        alertPopup("Lỗi", "Dữ liệu tìm không thấy!", "warning");
    }
    HideLoading();
});
        return false;
    }
    else {
        return false;
    }


}


function LoadData() {
    ShowLoading();
    $.post("/SchedulingHistory/LoadData")
.done(function (data) {
    if (data != null) {
          //alertPopup("Thông báo", "Load thành công", "success");
        for (var i = 0; i < data.length; i++) {
            var customerName = data[i].CustomerName;
            var nameNV = data[i].NameNV;
            var NameBed = data[i].Name_Bed;
            var NameRoom = data[i].Name_Room;
            var id = data[i].Id;
            var BedId = data[i].BedId;
            //alertPopup("Lỗi", "Dữ liệu tìm không thấy!" + id, "warning");
            $("#info_customer_" + data[i].BedId).data('id', id).data('room-id', BedId).data('customerName', customerName).data('nameNV', nameNV);
            $("#status_complete_" + data[i].BedId).data('id', id).data('room-id', BedId);
            $("#status_remove_" + data[i].BedId).data('id', id).data('room-id', BedId);
            $("#status_plustime_" + data[i].BedId).data('id', data[i].Id).data('room-id', data[i].BedId).data('status',data[i].Status);
            if (data[i].Status == "inprogress") {

                $("#countdown_" + data[i].BedId).countdown(data[i].strEndDate, NameBed, NameRoom, function (event) {
                    $(this).text(event.strftime('%H:%M:%S')
                );
                }).on('finish.countdown', function () {
                    changeStatus(id, "expired", "");
                    });

              
                $("#countdown_" + data[i].BedId).addClass(data[i].ColorStatus);
                $("#status_room_" + data[i].RoomId).removeClass("label-info").addClass(data[i].ColorStatus).text(data[i].strStatus);
                $("#scheduling_" + data[i].Id).closest('div').remove();
                $("#CustomerName_" + data[i].RoomId).text(data[i].CustomerName);
                $('img[name="img_' + data[i].RoomId + '"]').attr("src", data[i].CustomerImagePath);
                $("#aImg_" + data[i].RoomId).attr("href", data[i].CustomerImagePath);
            }
            if (data[i].Status == "complete") {
                $("#status_room_" + data[i].RoomId).removeClass("label-danger").removeClass("label-success").removeClass("label-info").removeClass("label-warning").addClass(data[i].ColorStatus).text(data[i].strStatus);
                $("#countdown_" + data[i].BedId).removeClass(data[i].ColorStatus);
            }
        }

    }
    else {
        alertPopup("Lỗi", "Dữ liệu tìm không thấy!", "warning");
    }

});
    HideLoading();
}
$(function () {
    LoadData();
    $('.info_customer').click(function () {
        ShowLoading();
        var id = $(this).data("id");
        if (id == undefined || id == null || id == "") {
            alertPopup("Lỗi", "Dữ liệu tìm không thấy!", "warning");
        }
        else {
            OpenPopup('/SchedulingHistory/Detail/?Id=' + id + '&IsPopup=true', 'Chi tiết xếp lịch', 0, 0);
        }
        HideLoading();
    });

    $('.info_customer').mouseenter(function () {
        //ShowLoading();
        var id = $(this).data("id");
        var namecustomer = $(this).data("customerName");
        var nhanvien = $(this).data("nameNV");
       
        if (namecustomer != undefined) {
            alertPopupinfo("Khách hàng:" + namecustomer, "Nhân viên:" + nhanvien);
        }
        HideLoading();
    });
    //$('.info_customer').mouseleave(function () {
      
    //   // ClosealertPopup();
    //   //HideLoading();
       
    //});
    $('.status_complete').click(function () {
        ShowLoading();

        var id = $(this).data("id");
        //alertPopup("Lỗi 123", "lỗi vào hàm click" + id, "warning");

        if (id == undefined || id == null || id == "") {
            //alertPopup("Lỗi", "Dữ liệu tìm không thấy!", "warning");
        }
        else {
            changeStatus(id, "complete", "");
        }
        HideLoading();
    });
    $('.status_remove').click(function () {

        ShowLoading();
        var room_id = $(this).data("room-id");
        if (room_id == undefined || room_id == null || room_id == "") {
            alertPopup("Lỗi", "Dữ liệu tìm không thấy!", "warning");
        }
        else {

            var id = $(this).data("id");
            DeleteById(id);
        }
        HideLoading();
    });
    $('.status_plustime').click(function () {
        ShowLoading();
        debugger
        var status = $(this).data("status");
        var id = $(this).data("id");
        var idbed = $(this).data("room-id");
        if (id == undefined || id == null || id == "") {
            alertPopup("Lỗi", "Dữ liệu tìm không thấy!", "warning");
        }
        else {
            if (status == "inprogress" && $("#countdown_" + idbed).attr('class') == "pull-right label-danger") {
                alertPopup("Lỗi", "chỉ thêm giờ được 1 lần!", "warning");
                HideLoading();
                return false;
            }
            var person = prompt("Nhập thời gian thêm(phút) :", "");
            if (person == null || person == "") {
                alert('Không có dữ liệu')
            } else {
                changeStatus(id, "inprogress", "plustime", person);
            }
        }
        HideLoading();
    });

});
var span = document.getElementById('span');

function time() {
    var d = new Date();
    var s = d.getSeconds();
    var m = d.getMinutes();
    var h = d.getHours();
    span.textContent = h + ":" + m + ":" + s;
}

setInterval(time, 1000);