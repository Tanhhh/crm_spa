
$(function () {
    $("#Product").combojax({
        url: "/InquiryCard/GetListJsonAll",
        onNotFound: function (p) {  //khi tìm ko thấy dữ liệu thì sẽ hiển thị khung thêm mới dữ liệu
            //OpenPopup('/Customer/Create?IsPopup=true&Phone=' + p, 'Thêm mới', 500, 250);
        },
        onSelected: function (obj) {
            console.log(obj);
            if (obj) {
                var ProductId = obj.Id;
                var ProductName = obj.Name;
                var ProductCode = obj.Code;
                var Price = obj.Price;
                var formdata = {
                    ProductId: ProductId
                };
                $("#TotalMinute").val(obj.TimeForService);
                $('#mask-TotalMinute').val(obj.TimeForService);
                $("#ProductId").val(ProductId);
                $("#ProductName").val(ProductName);
                $(".detailList").html("");
                //Thêm dòng mới
                ClickEventHandler(true, "/InquiryCard/LoadDetail", ".detailList", formdata, function () { });
            }
        }
        , onShowImage: true
        , onSearchSaveSelectedItem: false
        , onRemoveSelected: true
    });
});

$(window).keydown(function (e) {
    if (e.which == 115) {
        e.preventDefault();
        $("#ProductName").focus();
    }
});
function tinh_total_minute() {
    var total = 0;
    var total1 = 0;
    var selector = '.detailList tr';
    $(selector).each(function (index, elem) {
        if ($(elem).find('.is_actived').is(':checked')) { // la số thì mới tính
            if ($(elem).find('.total_mitute').val() != '') { // la số thì mới tính
                total += parseFloat($(elem).find('.total_mitute').val());
                $("#TotalMinute").val(total);
                $('#mask-TotalMinute').val(total);
            }
        }
    });
}
/*********** page load ***************/
$(function () {

    //Khi thay đổi giá
    $('#listOrderDetail').on('change', '.is_actived', function () {
        var $this = $(this);
        if (!$this.is(':checked')) {
            $this.val(false);
        }
        else {
            $this.val(true);
        }
        tinh_total_minute();
    });

});

function SetTextForId(ele, strName) {
    var a = $(ele).closest('div .control-group');
    a.find('label').html(strName);
}
$(document).ready(function () {
    _val = $("#TargetModule").val();
    if (_val == "AdviseCard") {
        SetTextForId($('#TargetId'), "Mã phiếu tư vấn");
        $(".product-search-box").show();
    }
    else {
        SetTextForId($('#TargetId'), "Mã phiếu Membership");
        $(".product-search-box").hide();
    }

});
$(".group_choice").change(function () {
    ShowLoading();
    if ($(this).is(":checked")) {
        var _val = $(this).val();
        $("#TargetModule").val(_val);
        var _str = "";
        if (_val == "AdviseCard") {
            _str = "OpenPopup('/" + _val + "/Index?IsPopup=true&module_list=&jsCallback=selectItem_TargetId', 'Tìm kiếm dữ liệu', 0, 0)";
        } else {
            _str = "OpenPopup('/" + _val + "/MembershipIndex?IsPopup=True', 'Chi tiết phiếu MBS', 0, 0)";
        }

        $(".targetid").attr("onclick", _str);
        ResetData();
        if (_val == "AdviseCard") {
            SetTextForId($('#TargetId'), "Mã phiếu tư vấn");
            $(".product-search-box").show();
        }
        else {
            SetTextForId($('#TargetId'), "Mã phiếu Membership");
            $(".product-search-box").hide();
        }
    }
    HideLoading();
});
function ResetData() {
    $("#CustomerId").val("");
    $("#CustomerName").val("");
    $("#ProductName").val("");
    $("#ProductId").val("");
    $("#Type").val("");
    $("#TotalMinute").val("");
    $("#TargetId").val("");
    $("#TargetId_DisplayText").val("");
    $(".detailList").html("");
};