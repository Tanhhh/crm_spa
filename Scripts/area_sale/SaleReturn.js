var $tr_template_invoice = $('.productInvoiceList tr:first-child');
$(function () {

    // tính thành tiền và tổng cộng
    $('#listReturnDetail').on('change', '.item_quantity', function () {
        debugger
        var $this = $(this);
        var id = $this.closest('tr').attr('role');

        $(this).val($(this).val().replace(/\-/g, ''));
        $(this).val($(this).val().replace(/[^0-9.,]/g, ''));
        var ralVal = numeral($(this).val());
        if (ralVal <= 0) {
            $(this).val(1);
        }

        var _OriginQuantity = $("#DetailList_" + id + "__OriginQuantity").val();
        if (parseInt($(this).val()) > parseInt(_OriginQuantity)) {
            $(this).val(_OriginQuantity);
        }
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

        brand = $this.closest('tr').find('.item_CountForBrand').val();
        orginalQuantity = $this.closest('tr').find('.item_OriginQuantity').val();
        var isNguyenGia = false;
        console.log(brand);
        if (brand === 'DICHVU' && parseFloat(orginalQuantity) !== parseFloat($this.val())) {
            isNguyenGia = true;
        }

        //tính tổng cộng
        calcAmountItem(id, ".detailList", isNguyenGia);
        calcTotalAmount(".detailList");
    });

    $('#listReturnDetail').on('change', '.item_price', function () {
        $(this).val($(this).val().replace(/\-/g, ''));
        $(this).val($(this).val().replace(/[^0-9.,]/g, ''));
        var ralVal = numeral($(this).val());
        if (ralVal <= 0) {
            $(this).val(1);
        }
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
        debugger
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
function calcAmountItem(id, tbody, isNguyenGia) {

    var $this = $(tbody + ' tr[role="' + id + '"]');

    console.log('calculate')
    // nguyên giá khi trả một phần cho dịch vụ
    if (isNguyenGia) {
        var inputQuantity = parseFloat($this.find('.item_quantity').val());
        console.log('inputQuantity:'+inputQuantity);
        var OriginalQuantity = parseFloat($this.find('.item_OriginQuantity').val());
        console.log('OriginalQuantity:' + OriginalQuantity);
        Originalprice = parseFloat(removeComma($this.find('.item_OriginalPrice').val()));
        console.log('Originalprice:' + Originalprice)
        // giá của sp đã sử dụng là giá gốc
        UsedItem_Amount = (OriginalQuantity - inputQuantity) * Originalprice;
        console.log('UsedItem_Amount:' + UsedItem_Amount)
        // tiền trả cho khách =  tổng tiển (gồm KM nếu có) - giá gốc của sp đã sử dụng
        var total = parseFloat(removeComma($this.find('.item_OriginalAmount').val())) - UsedItem_Amount;
        $this.find('.item_total').text(numeral(total).format('0,0'));
        return;
    }

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
    debugger
    var total = 0;
    var total1 = 0;

    var selector = tbody + ' tr';
    var count_rows = $('.detailList').find('tr').length;
    $(selector).each(function (index, elem) {
        debugger
        if (count_rows == 1) {
            $(tbody + "_Amount").text(numeral(total).format('0,0'));
        }
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