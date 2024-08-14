var $tr_template = $('.detailList tr:first-child');
$(function () {
    var warehouse = $("#WarehouseSourceId").val();
    if(warehouse!="")
    {
        $("#Material").combojax({
            url: "/InventoryMaterial/GetListMaterialJsonByWarehouseId?warehouseId=" + warehouse,
            onNotFound: function (p) {  //khi tìm ko thấy dữ liệu thì sẽ hiển thị khung thêm mới dữ liệu
                //OpenPopup('/Customer/Create?IsPopup=true&Phone=' + p, 'Thêm mới', 500, 250);
            },
            onSelected: function (obj) {
                console.log(obj);
                if (obj) {
                    var len = $('.detailList tr').length;
                    var tr_new = $tr_template.clone()[0].outerHTML;
                    tr_new = tr_new.replace(/\[0\]/g, "[" + len + "]");
                    tr_new = tr_new.replace(/\_0\__/g, "_" + len + "__");
                    var $tr_new = $(tr_new);
                    $tr_new.attr('role', len);
                    $tr_new.find('td:first-child').text(len);
                    $tr_new.find('.item_material_id').val(obj.Value);
                    $tr_new.find('.item_id').val(0);
                    $tr_new.find('.item_quantity').val(1);
                    $tr_new.find('.item_price').val(obj.Price);
                    $tr_new.find('.item_unit').val(obj.Unit);
                    $tr_new.find('.item_total').text("");
                    $tr_new.find('.item_material_name').text(obj.Name);
                    $tr_new.find('.item_locode').val(obj.LoCode);
                    $tr_new.find('.item_expiry_date').val(obj.ExpiryDate);

                    $('.detailList').append($tr_new);
                    var $tr_after_append = $('tr[role="' + len + '"]');
                    $tr_after_append.removeAttr("hidden", "hidden");
                    $.mask.definitions['~'] = '[+-]';
                    $('.input-mask-date').mask('99/99/9999');
                    //tính tổng cộng
                    calcAmountItem(len);
                    calcTotalAmount();
                    LoadNumberInput();
                }
            }
          , onShowImage: true
          , onSearchSaveSelectedItem: false
          , onRemoveSelected: false
        });
    }
  

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
        var _material_id = $("#DetailList_" + id + "__MaterialId").val();
        var _LoCode = $("#DetailList_" + id + "__LoCode").val();
        var _ExpiryDate = $("#DetailList_" + id + "__ExpiryDate").val();
        var _quantity_inventory = $(this).data("quantity-inventory");
        var selector = '.detailList tr';
        var quantity_used = 0;
        $(selector).each(function (index, elem) {
            if (index != id) {
                var material_id = $("#DetailList_" + index + "__MaterialId").val();
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
        calcAmountItem(id);
        calcTotalAmount();
    });

    $('#listOrderDetail').on('change', '.item_price', function () {
        var $this = $(this);
        var id = $this.closest('tr').attr('role');
        calcAmountItem(id);
        calcTotalAmount();
    });

    $('#listOrderDetail').on('keypress', '.item_price, .item_quantity', function (e) {
        if (e.which == 13) {
            e.preventDefault();
        }
    });

    // xóa sản phẩm
    $('#listOrderDetail').on('click', '.btn-delete-item', function () {
        $(this).closest('tr').remove();
        calcTotalAmount();
        $('.detailList tr').each(function (index, tr) {
            $(tr).attr('role', index);
            $(tr).find('td:first-child').text(index);
            $(tr).find('.item_material_id').attr('name', 'DetailList[' + index + '].MaterialId').attr('id', 'DetailList_' + index + '__MaterialId');
            $(tr).find('.item_id').attr('name', 'DetailList[' + index + '].Id').attr('id', 'DetailList_' + index + '__Id');
            $(tr).find('.item_quantity').attr('name', 'DetailList[' + index + '].Quantity').attr('id', 'DetailList_' + index + '__Quantity');
            $(tr).find('.item_price').attr('name', 'DetailList[' + index + '].Price').attr('id', 'DetailList_' + index + '__Price');
            $(tr).find('.item_locode').attr('name', 'DetailList[' + index + '].LoCode').attr('id', 'DetailList_' + index + '__LoCode');
            $(tr).find('.item_expiry_date').attr('name', 'DetailList[' + index + '].ExpiryDate').attr('id', 'DetailList_' + index + '__ExpiryDate');
            $(tr).find('.item_unit').attr('name', 'DetailList[' + index + '].Unit').attr('id', 'DetailList_' + index + '__Unit');
        });
    });
});
function calcAmountItem(id) {
    var $this = $('tr[role="' + id + '"]');
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

function calcTotalAmount() {
    var total = 0;
    var total1 = 0;

    var selector = '.detailList tr';
    $(selector).each(function (index, elem) {
        if ($(elem).find('.item_total').text() != '') { // la số thì mới tính
            total += parseFloat(removeComma($(elem).find('.item_total').text()));
            $("#TongThanhTien").text(numeral(total).format('0,0'));
        }

        if ($(elem).find('.item_quantity').val() != '') { // la số thì mới tính
            total1 += parseInt($(elem).find('.item_quantity').val().replace(/\./g, ''));
            $("#TongSoLuong").text(currencyFormat(total1));
        }

        if (index == $(selector).length - 1) {
            $('#mask-TotalAmount').val(numeral(total).format('0,0'));
            $('#TotalAmount').val(numeral(total).format('0,0'));
        }
    });
};
$(window).keydown(function (e) {
    if (e.which == 115) {   // khi nhấn F4 trên bàn phím hiển thị dữ liệu dropdownlist
        e.preventDefault();
        $("#Material").focus();
    }
});