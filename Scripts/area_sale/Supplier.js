function Check() {
    debugger
    var vnf_regex_phone = /((09|03|07|08|05)+([0-9]{8})\b)/g;
    var vnf_regex_mobile = /((09|03|07|08|05)+([0-9]{8})\b)/g;
    var regx = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
    var email = $('#Email').val();
    var phone = $('#Phone').val();
    var mobile = $('#Mobile').val();
    var nguonkhach = $('#tenNguonkhach').val();
   

    if (email !== '') {
        if (regx.test(email) == false) {
            alert('Email chưa đúng định dạng.\nExample@gmail.com');
            return false;
        }
    }
    if (mobile !== '') {
        if (vnf_regex_mobile.test(mobile) == false) {
            alert('Số di động của bạn không đúng định dạng!');
            return false;
        }
    }
    if (phone !== '') {
        if (vnf_regex_phone.test(phone) == false) {
            alert('Số điện thoại của bạn không đúng định dạng!');
            return false;
        }
    }
    if ($('#BranchId').val() == "") {
        alert('Chưa chọn chi nhánh!');
        return false;
    }
   else if (nguonkhach == "") {
        alert('Bạn chưa chọn nguồn khách!');
        return false;
    }
}
function FormSubmit() {
   
    
    var vnf_regex = /((09|03|07|08|05)+([0-9]{8})\b)/g;
    var regx = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
    var phone = $('#Phone').val();
    var email = $('#Email').val();
    if (email !== '') {
        if (regx.test(email) == false) {
            alert('Email chưa đúng định dạng.\nExample@gmail.com');
            return false;
        }
    }
    if (phone !== '') {
        if (vnf_regex.test(phone) == false) {
            alert('Số điện thoại của bạn không đúng định dạng!');
            return false;
        }
    }
    if ($('#FirstName').val() == "") {
        alert('Chưa nhập họ và tên lót!');
        return false;
    }
    if ($('#LastName').val() == "") {
        alert('Chưa nhập tên!');
        return false;
    }
    if ($('#ManagerStaffId').val() == "") {
        alert('Chưa chọn nhân viên quản lý!');
        return false;
    }
    //if ($('#Address').val() == "") {
    //    alert('Chưa nhập địa chỉ!');
    //    return false;
    //}
    //if ($('#Email').val() == "") {
    //    alert('Chưa nhập Email!');
    //    return false;
    //}
    //if ($('#BranchId').val() == "") {
    //    alert('Chưa chọn chi nhánh!');
    //    return false;
    //}

    //if ($('#tenNguonkhach').val() == "") {
    //    alert('Bạn chưa chọn nguồn khách!');
    //    return false;
    //}
    
    if ($("#FirstName").val() == "" || $("#LastName").val() == "" || $("#Gender").val() == "") {
        ShowTab(1);
        return true;
    }
    else
        if ($("#Phone").val() == "" || $("#Email").val() == "") {
            ShowTab(2);
            //alert("haha");
            return true;
        }

    //else
    //    if ($("#Sale_BranchId").val() == "")
    //    {
    //        ShowTab(3);
    //        return true;
    //    }

    return true;


}