
/*********** function ***************/

//function tinh_total_minute() {
//    var total = 0;
//    var total1 = 0;
//    var selector = '.detailList tr';
//    $(selector).each(function (index, elem) {
//        if ($(elem).find('.is_actived').is(':checked')) { // la số thì mới tính
//            if ($(elem).find('.total_mitute').val() != '') { // la số thì mới tính
//                total += parseFloat($(elem).find('.total_mitute').val());
//                $("#TotalMinute").val(total);
//                $('#mask-TotalMinute').val(total);
//            }
//        }
//    });

$(function () {
    $("#ProductName").combojax({
        url: "/CommissionCus/GetListJsonAll",
        onNotFound: function (p) {  //khi tìm ko thấy dữ liệu thì sẽ hiển thị khung thêm mới dữ liệu
            //OpenPopup('/Customer/Create?IsPopup=true&Phone=' + p, 'Thêm mới', 500, 250);
        },
        onSelected: function (obj) {
            console.log(obj);
            if (obj) {
                var Index = $(".detailList").find("tr").length + ($("tbody.detailListDonate>tr").length / 2);
                var ProductId = obj.Id;
                var ProductName = obj.Name;
                var ProductCode = obj.Code;
                var Price = obj.Price;
               
                if ($("#discount").hasClass("active")) {
                    var OrderNo = $(".detailList").find("tr").length;
                    var formdata = {
                        OrderNo: OrderNo,
                        Index: Index,
                        ProductCode: ProductCode,
                        ProductName: ProductName,
                        ProductId: ProductId,
                        Price: Price
                    };
                    //Thêm dòng mới
                    ClickEventHandler(true, "/CommissionCus/LoadProduct", ".detailList", formdata, function () {
                        LoadNumberInput();
                        $(".detail_item_check").change(function () {
                            var index = $(this).data("index");
                            if ($(this).prop("checked")) {
                                $("#DetailList_" + index + "__IsMoney").val(true);
                                $("#DetailList_" + index + "__CommissionValueText").val("VNĐ");
                            }
                            else {
                                $("#DetailList_" + index + "__IsMoney").val(false);
                                $("#DetailList_" + index + "__CommissionValueText").val("%");
                            }
                        });

                    });
                }
                else {
                    var OrderNo = ($("tbody.detailListDonate>tr").length / 2);
                    var formdata = {
                        OrderNo: OrderNo,
                        Index: Index,
                        ProductCode: ProductCode,
                        ProductName: ProductName,
                        ProductId: ProductId,
                        Price: Price
                    };
                    ClickEventHandler(true, "/CommissionCus/LoadProductDonate", ".detailListDonate", formdata, function () {
                        LoadNumberInput();
                    });
                }
            }
        }       //chỗ xử lý khi chọn 1 dòng dữ liệu ngoài việc lưu vào textbox tìm kiếm
        ,onShowImage: true                  //hiển thị hình ảnh
        ,onSearchSaveSelectedItem: false    //lưu dòng dữ liệu đã chọn vào ô textbox tìm kiếm, mặc định lưu giá trị value trong hàm get data
        ,onRemoveSelected: false  //những dòng đã chọn rồi thì sẽ ko hiển thị ở lần chọn tiếp theo
    });
    LoadNumberInput();
});
$(window).keydown(function (e) {
    if (e.which == 115) {   // khi nhấn F4 trên bàn phím hiển thị dữ liệu dropdownlist
        e.preventDefault();
        $("#ProductName").focus();
    }
});
function ShowDetailBtn(targetId) {
    $('td.cell-has-table').toggleClass('open');
    var tableDetail = $("#table-detail-" + targetId);
    if (tableDetail.hasClass("open")) {
        tableDetail.removeClass("open");
        $(".show-details-btn[data-target='" + targetId + "']").find("i").removeClass("fa-angle-double-up").addClass("fa-angle-double-down");
    }
    else {
        tableDetail.addClass("open");
        $(".show-details-btn[data-target='" + targetId + "']").find("i").removeClass("fa-angle-double-down").addClass("fa-angle-double-up");
    }
};
function AddRow(type) {
    var Index = $(".InvoicedetailList").find("tr").length + $(".InvoicedetailListDif").find("tr").length + ($("tbody.InvoicedetailListDonate>tr").length / 2);
    if (type == "discount") {
        var OrderNo = $(".InvoicedetailList").find("tr").length;
        var formdata = {
            OrderNo: OrderNo,
            Index: Index
        };
        //Thêm dòng mới
        ClickEventHandler(true, "/CommissionCus/LoadInvoice", ".InvoicedetailList", formdata, function () {
            LoadNumberInput();
            $(".invoice_detail_item_check").change(function () {
                var index = $(this).data("index");
                if ($(this).prop("checked")) {
                    $("#InvoiceDetailList_" + index + "__IsMoney").val(true);
                    $("#InvoiceDetailList_" + index + "__CommissionValueText").val("VNĐ");
                }
                else {
                    $("#InvoiceDetailList_" + index + "__IsMoney").val(false);
                    $("#InvoiceDetailList_" + index + "__CommissionValueText").val("%");
                }
            });

        });
    }
    if (type == "discountdif") {
        var OrderNo = $(".InvoicedetailListDif").find("tr").length;
        var formdata = {
            OrderNo: OrderNo,
            Index: Index
        };
        //Thêm dòng mới
        ClickEventHandler(true, "/CommissionCus/LoadInvoiceDif", ".InvoicedetailListDif", formdata, function () {
            LoadNumberInput();
            $(".invoice_detail_item_check").change(function () {
                var index = $(this).data("index");
                if ($(this).prop("checked")) {
                    $("#InvoicedetailList_" + index + "__IsMoney").val(true);
                    $("#InvoicedetailList_" + index + "__CommissionValueText").val("VNĐ");
                }
                else {
                    $("#InvoicedetailList_" + index + "__IsMoney").val(false);
                    $("#InvoicedetailList_" + index + "__CommissionValueText").val("%");
                }
            });

        });
    }
    else {
        var OrderNo = ($("tbody.InvoicedetailListDonate>tr").length / 2);
        var formdata = {
            OrderNo: OrderNo,
            Index: Index
        };
        ClickEventHandler(true, "/CommissionCus/LoadInvoiceDonate", ".InvoicedetailListDonate", formdata, function () {
            LoadNumberInput();
        });
    }
};

function ShowDetailBtnInvoice(targetId) {
    $('td.cell-has-table').toggleClass('open');
    var tableDetail = $("#table-invoice-detail-" + targetId);
    if (tableDetail.hasClass("open")) {
        tableDetail.removeClass("open");
        $(".show-invoice-details-btn[data-target='" + targetId + "']").find("i").removeClass("fa-angle-double-up").addClass("fa-angle-double-down");
    }
    else {
        tableDetail.addClass("open");
        $(".show-invoice-details-btn[data-target='" + targetId + "']").find("i").removeClass("fa-angle-double-down").addClass("fa-angle-double-up");
    }
};
