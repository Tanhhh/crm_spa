var $tr_template = $('.detailList tr:first-child');
$(function () {

    $("#Material").combojax({
        url: "/Material/GetListMaterial",
        onNotFound: function (p) {  //khi tìm ko thấy dữ liệu thì sẽ hiển thị khung thêm mới dữ liệu
            //OpenPopup('/Customer/Create?IsPopup=true&Phone=' + p, 'Thêm mới', 500, 250);
        },
        onSelected: function (obj) {
            //debugger
            //console.log("test" + obj);
            if (obj) {
               // $("#listOrderDetail tbody tr").each(function () {
                //    debugger
                    //var row = $(this);
                    //var w = $(window);
                //    //$(this).removeClass('selected_grey').addClass("text_data");

                //    var ProductCode1 = $(this).closest('tr').find("td:eq(1) input:nth-child(2)").val();

                //    //alert($('#product_barcode3').val());
                //    if (obj.Id == ProductCode1) {
                //        //alert(ProductId);
                //        $(this).closest('tr').find("td:eq(3) input:nth-child(2)").val(parseInt($(this).closest('tr').find("td:eq(3) input:nth-child(2)").val()) + 1);
                //        var body = $("html,#listOrderDetail");
                //        //body.stop().animate({ scrollTop: row.offset().top - (w.height() / 1.6) }, 500, 'swing', function () {
                //        // row.addClass('scrollCode');
                //        //});
                //        formdata = {};
                //        return;
                //    } else {
                //        //$(this).removeClass("scrollCode");
                //    }
                //    //alert(ProductId);

                //});
                //$('#combojax_Material ul li.item').click(function () {
               //alert(obj.Id);
                var length = $('.detailList tr').length;
                var arr = [];

                if (length > 1) {
                    for (var i = 1; i < length; i++) {
                        var checkActive = $('.detailList').find("#DetailList_" + i + "__MaterialId").val();
                        if (checkActive != 0) {
                            debugger;
                            if (obj.Id == checkActive) {
                                var soLuong = $('.detailList').find("#DetailList_" + i + "__Quantity").val() != '' ? removeComma($('.detailList').find("#DetailList_" + i + "__Quantity").val()) : 0;
                                var tang = i + 1;
                                $('.detailList').find("#DetailList_" + i + "__Quantity").val(parseInt(soLuong) + 1);
                                var sl_new = $('.detailList').find("#DetailList_" + i + "__Quantity").val();
                                $(".detailList tr:nth-child(" + tang + ")").find(".item_total").text(parseInt(sl_new) * obj.Price);
                                calcAmountItem(i);
                            }
                            var id = $(".detailList tr").find("#DetailList_" + i + "__MaterialId").val();
                            arr.push(parseInt(id));
                        }
                    }
                    //console.log(typeof (obj.Id));
                    //console.log(typeof (arr[1]));
                    if (obj.Id != undefined && !arr.includes(obj.Id)) {
                        var len = $('.detailList tr').length;
                        var tr_new = $tr_template.clone()[0].outerHTML;
                        tr_new = tr_new.replace(/\[0\]/g, "[" + len + "]");
                        tr_new = tr_new.replace(/\_0\__/g, "_" + len + "__");
                        var $tr_new = $(tr_new);
                        $tr_new.attr('role', len);
                        $tr_new.find('td:first-child').text(len);
                        $tr_new.find('.item_material_id').val(obj.Id);
                        $tr_new.find('.item_id').val(0);
                        $tr_new.find('.item_quantity').val(1);
                        $tr_new.find('.item_price').val(obj.Price);
                        $tr_new.find('.item_unit').val(obj.Unit);
                        $tr_new.find('.item_total').text("");
                        $tr_new.find('.item_material_name').text(obj.Name);
                        $tr_new.find('.item_locode').val("");
                        $tr_new.find('.item_expiry_date').val("");

                        $('.detailList').append($tr_new);
                        var $tr_after_append = $('tr[role="' + len + '"]');
                        $tr_after_append.removeAttr("hidden", "hidden");
                        $.mask.definitions['~'] = '[+-]';
                        $('.input-mask-date').mask('99/99/9999');


                        if ($('#demo').text() == 'VT') {
                            $('.detail_locode').hide();
                        }
                        else {
                            $('.detail_locode').show();
                        }
                    }
                }
                else if (length == 1) {
                    var len = $('.detailList tr').length;
                    var tr_new = $tr_template.clone()[0].outerHTML;
                    tr_new = tr_new.replace(/\[0\]/g, "[" + len + "]");
                    tr_new = tr_new.replace(/\_0\__/g, "_" + len + "__");
                    var $tr_new = $(tr_new);
                    $tr_new.attr('role', len);
                    $tr_new.find('td:first-child').text(len);
                    $tr_new.find('.item_material_id').val(obj.Id);
                    $tr_new.find('.item_id').val(0);
                    $tr_new.find('.item_quantity').val(1);
                    $tr_new.find('.item_price').val(obj.Price);
                    $tr_new.find('.item_unit').val(obj.Unit);
                    $tr_new.find('.item_total').text("");
                    $tr_new.find('.item_material_name').text(obj.Name);
                    $tr_new.find('.item_locode').val("");
                    $tr_new.find('.item_expiry_date').val("");

                    $('.detailList').append($tr_new);
                    var $tr_after_append = $('tr[role="' + len + '"]');
                    $tr_after_append.removeAttr("hidden", "hidden");
                    $.mask.definitions['~'] = '[+-]';
                    $('.input-mask-date').mask('99/99/9999');


                    if ($('#demo').text() == 'VT') {
                        $('.detail_locode').hide();
                    }
                    else {
                        $('.detail_locode').show();
                    }
                }

                //});


                //tính tổng cộng
                calcAmountItem(len);
                calcTotalAmount();
                LoadNumberInput();
                $('#ProductItemCount').val($('#listOrderDetail .detailList tr').length - 1);
            }
        }
        , onShowImage: true
        , onSearchSaveSelectedItem: false
        , onRemoveSelected: false
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
        //tính tổng cộng
        calcAmountItem(id);
        calcTotalAmount();
    });

    $('#listOrderDetail').on('change', '.item_price', function () {
        $(this).val($(this).val().replace(/\-/g, ''));
        $(this).val($(this).val().replace(/[^0-9.,]/g, ''));
        var ralVal = numeral($(this).val());
        if (ralVal <= 0) {
            $(this).val(1);
        }
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
        $('#ProductItemCount').val($('#listOrderDetail .detailList tr').length - 1);
    });
});

function calcAmountItem(id) {
    debugger
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
    // console.log(_price);
    var total = parseFloat(_price) * qty;
    $this.find('.item_total').text(numeral(total).format('0,0'));
    // LoadNumberInput();
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
