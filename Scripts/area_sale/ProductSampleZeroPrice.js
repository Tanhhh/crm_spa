
$(function () {
    $("#ProductName").combojax({
        url: "/ProductSample/GetListProductZeroPrice",
        onNotFound: function (p) {  //khi tìm ko thấy dữ liệu thì sẽ hiển thị khung thêm mới dữ liệu
            //OpenPopup('/Customer/Create?IsPopup=true&Phone=' + p, 'Thêm mới', 500, 250);
        },
        onSelected: function (obj) {
            console.log(obj);
            if (obj) {
                $("#ProductId").val(obj.Id);
            }
        }       //chỗ xử lý khi chọn 1 dòng dữ liệu ngoài việc lưu vào textbox tìm kiếm
        , onShowImage: false                  //hiển thị hình ảnh
        , onSearchSaveSelectedItem: true    //lưu dòng dữ liệu đã chọn vào ô textbox tìm kiếm, mặc định lưu giá trị value trong hàm get data
        , onRemoveSelected: false  //những dòng đã chọn rồi thì sẽ ko hiển thị ở lần chọn tiếp theo
    });
    LoadNumberInput();
});
$(window).keydown(function (e) {
    if (e.which == 115) {   // khi nhấn F4 trên bàn phím hiển thị dữ liệu dropdownlist
        e.preventDefault();
        $("#Customer").focus();
    }
});
