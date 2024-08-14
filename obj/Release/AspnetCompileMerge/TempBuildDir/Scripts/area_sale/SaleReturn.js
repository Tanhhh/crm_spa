var $tr_template_invoice = $('.productInvoiceList tr:first-child');
$(function () {
    $("#Product").combojax({
        url: "/Product/GetListProductInventoryAndService",
        onNotFound: function (p) {  //khi tìm ko thấy dữ liệu thì sẽ hiển thị khung thêm mới dữ liệu
            //OpenPopup('/Customer/Create?IsPopup=true&Phone=' + p, 'Thêm mới', 500, 250);
        },
        onSelected: function (obj) {
            console.log(obj);
            if (obj) {
                var len = $('.productInvoiceList tr').length;
                var tr_new = $tr_template_invoice.clone()[0].outerHTML;
                tr_new = tr_new.replace(/\[0\]/g, "[" + len + "]");
                tr_new = tr_new.replace(/\_0\__/g, "_" + len + "__");
                var $tr_new = $(tr_new);
                $tr_new.attr('role', len);
                $tr_new.find('td:first-child').text(len);
                $tr_new.find('.item_product_id').val(obj.Id);
                $tr_new.find('.item_id').val(0);
                $tr_new.find('.item_quantity').val(1);
                $tr_new.find('.item_price').val(obj.Price);
                $tr_new.find('.item_unit').val(obj.Unit);
                $tr_new.find('.item_total').text(obj.Amount);
                $tr_new.find('.item_product_name').text(obj.Name);
                $tr_new.find('.item_locode').val(obj.LoCode);
                $tr_new.find('.item_expiry_date').val(obj.ExpiryDate);

                $('.productInvoiceList').append($tr_new);
                var $tr_after_append = $('.productInvoiceList tr[role="' + len + '"]');
                $tr_after_append.removeAttr("hidden", "hidden");
                $.mask.definitions['~'] = '[+-]';
                $('.input-mask-date').mask('99/99/9999');
                calcAmountItem(len, ".productInvoiceList");
                calcTotalAmount(".productInvoiceList");
                LoadNumberInput();
            }
        }       //chỗ xử lý khi chọn 1 dòng dữ liệu ngoài việc lưu vào textbox tìm kiếm
    , onShowImage: true                  //hiển thị hình ảnh
    , onSearchSaveSelectedItem: false    //lưu dòng dữ liệu đã chọn vào ô textbox tìm kiếm, mặc định lưu giá trị value trong hàm get data
    , onRemoveSelected: false  //những dòng đã chọn rồi thì sẽ ko hiển thị ở lần chọn tiếp theo
    });
    // tính thành tiền và tổng cộng
    $('#listOrderDetail').on('change', '.item_quantity', function () {
        $(this).val($(this).val().replace(/\-/g, ''));
        $(this).val($(this).val().replace(/[^0-9.,]/g, ''));
        var ralVal = numeral($(this).val());
        if (ralVal <= 0) {
            $(this).val(1);
        }
        var $this = $(this);
        var id = $this.closest('tr').attr('role');
        //
        var _material_id = $("#InvoiceList_" + id + "__ProductId").val();
        var _LoCode = $("#InvoiceList_" + id + "__LoCode").val();
        var _ExpiryDate = $("#InvoiceList_" + id + "__ExpiryDate").val();
        var _quantity_inventory = $(this).data("quantity-inventory");
        var selector = '.productInvoiceList tr';
        var quantity_used = 0;
        $(selector).each(function (index, elem) {
            if (index != id) {
                var material_id = $("#InvoiceList_" + index + "__ProductId").val();
                var LoCode = $("#InvoiceList_" + index + "__LoCode").val();
                var ExpiryDate = $("#InvoiceList_" + index + "__ExpiryDate").val();
                var Quantity = $("#InvoiceList_" + index + "__Quantity").val();
                if (material_id == _material_id && LoCode == _LoCode && ExpiryDate == _ExpiryDate) { // la số thì mới tính
                    quantity_used += parseInt(removeComma(Quantity));
                }
            }
        });
        var inventory_qty = parseInt(_quantity_inventory) - parseInt(quantity_used);
        var _quantity = parseInt(removeComma($(this).val()));
        $("#status").text("");
        if (_quantity > inventory_qty) {
            $("#InvoiceList_" + id + "__Quantity").val(inventory_qty);
            $("#status").text("Tổng số lượng xuất ra không được lớn hơn số lượng tồn kho hiện tại!!");
        }
        //tính tổng cộng
        calcAmountItem(id, ".productInvoiceList");
        calcTotalAmount(".productInvoiceList");
    });

    $('#listOrderDetail').on('change', '.item_price', function () {
        var $this = $(this);
        var id = $this.closest('tr').attr('role');
        calcAmountItem(id, ".productInvoiceList");
        calcTotalAmount(".productInvoiceList");
    });

    $('#listOrderDetail').on('keypress', '.item_price, .item_quantity', function (e) {
        if (e.which == 13) {
            e.preventDefault();
        }
    });

    // xóa sản phẩm
    $('#listOrderDetail').on('click', '.btn-delete-item', function () {
        $(this).closest('tr').remove();
        calcTotalAmount(".productInvoiceList");
        $('.productInvoiceList tr').each(function (index, tr) {
            $(tr).attr('role', index);
            $(tr).find('td:first-child').text(index);
            $(tr).find('.item_product_id').attr('name', 'InvoiceList[' + index + '].ProductId').attr('id', 'InvoiceList_' + index + '__ProductId');
            $(tr).find('.item_id').attr('name', 'InvoiceList[' + index + '].Id').attr('id', 'InvoiceList_' + index + '__Id');
            $(tr).find('.item_quantity').attr('name', 'InvoiceList[' + index + '].Quantity').attr('id', 'InvoiceList_' + index + '__Quantity');
            $(tr).find('.item_price').attr('name', 'InvoiceList[' + index + '].Price').attr('id', 'InvoiceList_' + index + '__Price');
            $(tr).find('.item_locode').attr('name', 'InvoiceList[' + index + '].LoCode').attr('id', 'InvoiceList_' + index + '__LoCode');
            $(tr).find('.item_expiry_date').attr('name', 'InvoiceList[' + index + '].ExpiryDate').attr('id', 'InvoiceList_' + index + '__ExpiryDate');
            $(tr).find('.item_unit').attr('name', 'InvoiceList[' + index + '].Unit').attr('id', 'InvoiceList_' + index + '__Unit');
        });
    });

    // tính thành tiền và tổng cộng
    $('#listReturnDetail').on('change', '.item_quantity', function () {
        $(this).val($(this).val().replace(/\-/g, ''));
        $(this).val($(this).val().replace(/[^0-9.,]/g, ''));
        var ralVal = numeral($(this).val());
        if (ralVal <= 0) {
            $(this).val(1);
        }
        var $this = $(this);
        var id = $this.closest('tr').attr('role');
        //
        var _material_id = $("#DetailList_" + id + "__ProductId").val();
        var _LoCode = $("#DetailList_" + id + "__LoCode").val();
        var _ExpiryDate = $("#DetailList_" + id + "__ExpiryDate").val();
        var _quantity_inventory = $(this).data("quantity-inventory");
        var selector = '.detailList tr';
        var quantity_used = 0;
        $(selector).each(function (index, elem) {
            if (index != id) {
                var material_id = $("#DetailList_" + index + "__ProductId").val();
                var LoCode = $("#DetailList_" + index + "__LoCode").val();
                var ExpiryDate = $("#DetailList_" + index + "__ExpiryDate").val();
                var Quantity = $("#DetailList_" + index + "__Quantity").val();
                if (material_id == _material_id && LoCode == _LoCode && ExpiryDate == _ExpiryDate) { // la số thì mới tính
                    quantity_used += parseInt(removeComma(Quantity));
                }
            }
        });
        var inventory_qty = parseInt(_quantity_inventory) - parseInt(quantity_used);
        var _quantity = parseInt(removeComma($(this).val()));
        $("#status").text("");
        if (_quantity > inventory_qty) {
            $("#DetailList_" + id + "__Quantity").val(inventory_qty);
            $("#status").text("Tổng số lượng xuất ra không được lớn hơn số lượng tồn kho hiện tại!!");
        }
        //tính tổng cộng
        calcAmountItem(id, ".detailList");
        calcTotalAmount(".detailList");
    });

    $('#listReturnDetail').on('change', '.item_price', function () {
        var $this = $(this);
        var id = $this.closest('tr').attr('role');
        calcAmountItem(id, ".detailList");
        calcTotalAmount(".detailList");
    });

    $('#listReturnDetail').on('keypress', '.item_price, .item_quantity', function (e) {
        if (e.which == 13) {
            e.preventDefault();
        }
    });

    // xóa sản phẩm
    $('#listReturnDetail').on('click', '.btn-delete-item', function () {
        $(this).closest('tr').remove();
        calcTotalAmount(".detailList");
        $('.detailList tr').each(function (index, tr) {
            $(tr).attr('role', index);
            $(tr).find('td:first-child').text(index);
            $(tr).find('.item_product_id').attr('name', 'DetailList[' + index + '].ProductId').attr('id', 'DetailList_' + index + '__ProductId');
            $(tr).find('.item_id').attr('name', 'DetailList[' + index + '].Id').attr('id', 'DetailList_' + index + '__Id');
            $(tr).find('.item_quantity').attr('name', 'DetailList[' + index + '].Quantity').attr('id', 'DetailList_' + index + '__Quantity');
            $(tr).find('.item_price').attr('name', 'DetailList[' + index + '].Price').attr('id', 'DetailList_' + index + '__Price');
            $(tr).find('.item_locode').attr('name', 'DetailList[' + index + '].LoCode').attr('id', 'DetailList_' + index + '__LoCode');
            $(tr).find('.item_expiry_date').attr('name', 'DetailList[' + index + '].ExpiryDate').attr('id', 'DetailList_' + index + '__ExpiryDate');
            $(tr).find('.item_unit').attr('name', 'DetailList[' + index + '].Unit').attr('id', 'DetailList_' + index + '__Unit');
        });
    });
});
function calcAmountItem(id,tbody) {
    var $this = $(tbody + ' tr[role="' + id + '"]');
    var input_price = $this.find('.item_price');
    var _price = input_price.val() != '' ? removeComma(input_price.val()) : 0;

    var $qty = $this.find('.item_quantity');
    var qty = 1;
    if ($qty.val() == '') {
        $qty.val(1);
    } else {
        qty = parseInt(removeComma($qty.val())) < 0 ? parseInt(removeComma($qty.val())) * -1 : parseInt(removeComma($qty.val()));
    }
    var total = parseFloat(_price) * qty;
    $this.find('.item_total').text(numeral(total).format('0,0'));
};

function calcTotalAmount(tbody) {
    var total = 0;
    var total1 = 0;

    var selector = tbody + ' tr';
    $(selector).each(function (index, elem) {
        if ($(elem).find('.item_total').text() != '') { // la số thì mới tính
            total += parseFloat(removeComma($(elem).find('.item_total').text()));
            $(tbody + "_Amount").text(numeral(total).format('0,0'));
        }

        if ($(elem).find('.item_quantity').val() != '') { // la số thì mới tính
            total1 += parseInt(removeComma($(elem).find('.item_quantity').val()));
            $(tbody + "_SL").text(numeral(total1).format('0,0'));
        }

      
    });
    if (tbody == ".detailList") {
        $('#mask-TotalAmount').val(numeral(total).format('0,0'));
        $('#TotalAmount').val(numeral(total).format('0,0'));
    }
    if (tbody == ".productInvoiceList") {
        var vat = $('#InvoiceNew_TaxFee').val();
        var tt = total + total * parseInt(vat) / 100;
        $('#mask-InvoiceNew_TotalAmount').val(numeral(total).format('0,0'));
        $('#InvoiceNew_TotalAmount').val(numeral(total).format('0,0'));
        $('#mask-InvoiceNew_TongTienSauVAT').val(numeral(tt).format('0,0'));
        $('#InvoiceNew_TongTienSauVAT').val(numeral(tt).format('0,0'));
    }
    var aa = $("#InvoiceNew_TongTienSauVAT").val();
    var bb =  $("#TotalAmount").val();
    var _amount1 = aa==""?0:aa;
    var _amount2 = bb == "" ? 0 : bb;
    var amount = parseFloat(removeComma(_amount1)) - parseFloat(removeComma(_amount2));
    if (amount < 0) {
        $('#mask-AmountPayment').val(numeral(amount*(-1)).format('0,0'));
        $('#AmountPayment').val(numeral(amount * (-1)).format('0,0'));
        $('#mask-AmountReceipt').val(0);
        $('#AmountReceipt').val(0);
    }
    if (amount == 0) {
        $('#mask-AmountPayment').val(0);
        $('#AmountPayment').val(0);
        $('#mask-AmountReceipt').val(0);
        $('#AmountReceipt').val(0);
    }
    if (amount > 0) {
        $('#mask-AmountReceipt').val(numeral(amount).format('0,0'));
        $('#AmountReceipt').val(numeral(amount).format('0,0'));
        $('#mask-AmountPayment').val(0);
        $('#AmountPayment').val(0);
    }
  
};
$(window).keydown(function (e) {
    if (e.which == 115) {   // khi nhấn F4 trên bàn phím hiển thị dữ liệu dropdownlist
        e.preventDefault();
        $("#Product").focus();
    }
    else
    if(e.which == 113) {
        e.preventDefault();
        $("#ProductInvoice").focus();
    }
});
function selectItemCustomer(id, name, customername, customerid) {
    $("#ProductInvoiceOldId").val(id).trigger('change');
    $("#ProductInvoiceOldId_DisplayText").val(name).trigger('change');
    $("#ProductInvoiceOldId_DisplayText").focus().blur();
    $("#CustomerId").val(customerid).trigger('change');
    $("#CustomerId_DisplayText").val(customername).trigger('change');
    ClosePopup();
    var formdata = {
        id: id
    };
    $("#search_content_invoice").html("");
    //Thêm dòng mới
    ClickEventHandler(false, "/SalesReturns/SearchProductInvoice", "#search_content_invoice", formdata, function () { });
}