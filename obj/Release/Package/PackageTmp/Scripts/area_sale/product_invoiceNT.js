/*********** khai báo biến ***************/
var city = $('#ShipCityId');
var districts = $('#ShipDistrictId');
var ward = $('#ShipWardId');
var promotion = {};
var arrPrice=[];

/*********** function ***************/
//hàm gọi lại từ form tạo mới khách hàng
function ClosePopupAndDoSomethings(optionSelect) {
    ClosePopup(false);
    $("#CustomerId").val($(optionSelect).val()).triggerHandler('change');
    $("#CustomerId_DisplayText").val($(optionSelect).text()).triggerHandler('change');
}
/*********** page load ***************/
$(function () {
    LoadNumberInput();
    //VAT thay đổi
    $('#TaxFee').change(function () {
        if ($(this).val() != '') {
            if ($(this).val() > 100) {
                $(this).val(100);
            }

            var total = removeComma($("#TotalAmount").val());
            var vat = parseInt($(this).val());

          //  var DiscountAmount = parseFloat(removeComma($("#DiscountAmount").val()));

            var TongTienSauVAT = parseFloat(total) + (vat * (parseFloat(total) / 100));
            //console.log(total, vat, TongTienSauVAT);
            $("#TongTienSauVAT").val(numeral(TongTienSauVAT).format('0,0'));

            $('#mask-TongTienSauVAT').val(numeral(TongTienSauVAT).format('0,0'));
        }
    });


    $('#TaxFee').focus(function () {
        $(this).select();
    });

    //Load quận huyện => xã phường
    var url = '/api/BackOfficeServiceAPI/FetchLocation';

    city.change(function () {
        districts.attr("disabled", "disabled");
        ward.attr("disabled", "disabled");

        var id = $(this).val(); // Use $(this) so you don't traverse the DOM again
        $.getJSON(url, { parentId: id }, function (response) {
            districts.empty();
            $(document.createElement('option'))
                    .attr('value', '')
                    .text('- Quận/Huyện -')
                    .appendTo(districts);

            ward.empty();
            $(document.createElement('option'))
                    .attr('value', '')
                    .text('- Xã/Phường -')
                    .appendTo(ward);

            $(response).each(function () {
                $(document.createElement('option'))
                    .attr('value', this.Id)
                    .text(capitalizeFirstAllWords(this.Name.toLowerCase()))
                    //.text(capitalizeFirstAllWords(this.Name.toLowerCase().replace('huyện', '').replace('quận', '')))
                    .appendTo(districts);
            });

            districts.removeAttr("disabled");
            ward.removeAttr("disabled");

            //var $option = $('#ContactId').find('option:selected');
            //districts.val($option.data('district'));

            districts.trigger("chosen:updated");
            districts.trigger('change');
        });
    });

    districts.change(function () {
        ward.attr("disabled", "disabled");

        var id = $(this).val(); // Use $(this) so you don't traverse the DOM again
        $.getJSON(url, { parentId: id }, function (response) {
            ward.empty();
            $(document.createElement('option'))
                    .attr('value', '')
                    .text('- Xã/Phường -')
                    .appendTo(ward);

            $(response).each(function () {
                $(document.createElement('option'))
                    .attr('value', this.Id)
                    .text(capitalizeFirstAllWords(this.Name.toLowerCase()))
                    .appendTo(ward);
            });

            ward.removeAttr("disabled");

            //var $option = $('#ContactId').find('option:selected');
            //ward.val($option.data('ward'));
            ward.trigger("chosen:updated");
        });
    });

    $('#AmountRemain').val('0');

    $('#ReceiptViewModel_Amount').focus(function () {
        $(this).select();
    });

    $('#ReceiptViewModel_Amount').blur(function () {
        //$("#TongTienSauVAT").val(numeral(Math.round(TongTienSauVAT)).format('0,0.00'));
        //$('#mask-TongTienSauVAT').val(numeral(Math.round(TongTienSauVAT)).format('0,0.00'));
        var totalAmount = parseFloat(removeComma($("#TongTienSauVAT").val()));
        var amount = parseFloat(removeComma($('#ReceiptViewModel_Amount').val()));
        if (amount < totalAmount) {
            $('.NextPaymentDate-container').show();
            $('#AmountRemain').val(numeral(totalAmount - amount).format('0,0'));
        }
        else
            $('.NextPaymentDate-container').hide();
    });

    $("#btnShowPayment").click(function () {
        $('#ReceiptViewModel_Amount').val($("#TongTienSauVAT").val());
        $('#mask-ReceiptViewModel_Amount').val($("#TongTienSauVAT").val());
        $('#mask-ReceiptViewModel_Amount').focus();
    });
});

var $tr_template_invoice = $('.productInvoiceList tr:first-child');
$(function () {
    $('#Total_Discount').val(0);
    data_selecteds = [];
    $("#Product").combojax({
        url: "/Product/GetListProductInventoryAndService",
        onNotFound: function (p) {  //khi tìm ko thấy dữ liệu thì sẽ hiển thị khung thêm mới dữ liệu
            //OpenPopup('/Customer/Create?IsPopup=true&Phone=' + p, 'Thêm mới', 500, 250);
        },
        onSelected: function (obj) {
            console.log(obj);
            if (obj) {
                //var status_data_Selected = false;//mặc định là không trùng text đã chọn
                //if (data_selecteds.length > 0) {
                //    for (var ii in data_selecteds) {
                //        if (data_selecteds[ii].Id == obj.Id && data_selecteds[ii].LoCode == obj.LoCode && data_selecteds[ii].ExpiryDate == obj.ExpiryDate)
                //        {
                //            status_data_Selected = true;
                //        }
                //    }
                //}
               
                //if (status_data_Selected)
                //{
                    //var aa = $('.productInvoiceList input').find('[data-product_id="' + obj.Id + '"]').find('[data-lo_code="' + obj.LoCode + '"]').find('[data-expiry_date="' + obj.ExpiryDate.replace("/", "") + '"]');
                    //var quantity = parseInt(removeComma(aa.val()))+1;
                    //aa.val(numeral(quantity).format('0,0'));
                //}
                //else
                //{
                debugger
                var len = $('.productInvoiceList tr').length;
                if (len > 1) {

                }
                    var tr_new = $tr_template_invoice.clone()[0].outerHTML;
                    tr_new = tr_new.replace(/\[0\]/g, "[" + len + "]");
                    tr_new = tr_new.replace(/\_0\__/g, "_" + len + "__");
                    var $tr_new = $(tr_new);
                    $tr_new.attr('role', len);
                   // $tr_new.attr('id', len);
                    $tr_new.find('td:first-child').text(len);
                    $tr_new.find('.item_product_id').val(obj.Id);
                    $tr_new.find('.item_id').val(0);
                    $tr_new.find('.item_quantity').val(1).data('quantity_inventory', obj.QuantityTotalInventory).data('product_type', obj.Type);
                    //$tr_new.find('.item_price').val(obj.Price).attr("readonly", "readonly");
                    $tr_new.find('.item_unit').val(obj.Unit);
                    //$tr_new.find('.item_total').text(obj.Amount);
                    $tr_new.find('.item_product_name').text(obj.Name);
                    $tr_new.find('.item_product_origin').text(obj.Origin);
                    $tr_new.find('.item_note').text(obj.Note);
                    if (obj.Categories == "KHT") {
                        $tr_new.find('.item_is_TANG').attr("checked", "checked");
                        $tr_new.find('.item_price').val(0);
                        $tr_new.find('.item_total').text(0);
                        $tr_new.find('.hidden_price').val(0);
                        $tr_new.find('.hidden_total').text(0);
                    } else {
                        $tr_new.find('.item_is_TANG').text(obj.is_TANG);
                        $tr_new.find('.item_price').val(obj.Price).attr("readonly", "readonly");
                        $tr_new.find('.item_total').text(obj.Amount);    
                        $tr_new.find('.hidden_price').val(obj.Price).attr("readonly", "readonly");
                        $tr_new.find('.hidden_total').text(obj.Amount); 

                    }
                    //$tr_new.find('.item_is_TANG').text(obj.is_TANG);
                    $tr_new.find('.item_locode').val(obj.LoCode).attr("readonly", "readonly");
                    $tr_new.find('.item_expiry_date').val(obj.ExpiryDate);
                    if (obj.Type == "product") {
                        $tr_new.find('.item_expiry_date').attr("readonly", "readonly");
                    }
                    if (obj.Type == "service") {
                        $tr_new.find('.item_locode').val(obj.LoCode).attr("hidden", "hidden");
                    }
                    $('.productInvoiceList').append($tr_new);
                    var $tr_after_append = $('.productInvoiceList tr[role="' + len + '"]');
                    $tr_after_append.removeAttr("hidden", "hidden");
                    if (obj.Type == "product")
                    {
                        $tr_after_append.addClass("alert-warning");
                    }
                    data_selecteds.push(obj);
                //}
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
    
    //$(".item_is_TANG").change(function () {
    //    var is_checked = $(this).is(":checked");
    //    if (is_checked) {
    //        $(".item_price").text(0);
    //        $(".item_total").text(0);
    //    } else {
    //        return;
    //    }
    //});
    
    // tính thành tiền và tổng cộng
    $('#listOrderDetail').on('change', '.item_quantity', function () {
        var $this = $(this);
        var id = $this.closest('tr').attr('role');
        $(this).val($(this).val().replace(/\-/g, ''));
        $(this).val($(this).val().replace(/[^0-9.,]/g, ''));
        var ralVal = numeral($(this).val());
        if (ralVal <= 0) {
            $(this).val(1);
        }
        var _OriginQuantity = $("#InvoiceList_" + id + "__OriginQuantity").val();
        
        //
        if (parseInt($(this).val()) > parseInt(_OriginQuantity)) {
            $(this).val(_OriginQuantity);
        }
        var product_type = $(this).data("product_type");
        if (product_type == "product") {
            var _quantity_inventory = $(this).data("quantity_inventory");

            var _quantity = parseInt(removeComma($(this).val()));
            if (_quantity > _quantity_inventory) {
                $("#InvoiceList_" + id + "__Quantity").val(numeral(_quantity_inventory).format('0,0'));
            }
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

        $('.productInvoiceList tr').each(function (index, tr) {

            if (arrPrice !== null);
            // arrPrice.splice(index,1);
            //console.log('arrpice[index]:' +arrPrice[index])
            //console.log('index:' + index);
            $(tr).attr('role', index);
            $(tr).find('td:first-child').text(index);
            $(tr).children().eq(6).attr('id', 'InvoiceList_' + index + '__Total');
            $(tr).find('.item_product_id').attr('name', 'InvoiceList[' + index + '].ProductId').attr('id', 'InvoiceList_' + index + '__ProductId');
            $(tr).find('.item_id').attr('name', 'InvoiceList[' + index + '].Id').attr('id', 'InvoiceList_' + index + '__Id');
            $(tr).find('.item_quantity').attr('name', 'InvoiceList[' + index + '].Quantity').attr('id', 'InvoiceList_' + index + '__Quantity');
            $(tr).find('.item_price').attr('name', 'InvoiceList[' + index + '].Price').attr('id', 'InvoiceList_' + index + '__Price');
            $(tr).find('.item_locode').attr('name', 'InvoiceList[' + index + '].LoCode').attr('id', 'InvoiceList_' + index + '__LoCode');
            $(tr).find('.item_expiry_date').attr('name', 'InvoiceList[' + index + '].ExpiryDate').attr('id', 'InvoiceList_' + index + '__ExpiryDate');
            $(tr).find('.item_unit').attr('name', 'InvoiceList[' + index + '].Unit').attr('id', 'InvoiceList_' + index + '__Unit');
            $(tr).find('.item_note').attr('name', 'InvoiceList[' + index + '].Note').attr('id', 'InvoiceList_' + index + '__Note');
            $(tr).find('.item_origin').attr('name', 'InvoiceList[' + index + '].Origin').attr('id', 'InvoiceList_' + index + '__Origin');
            $(tr).find('.item_is_TANG').attr('name', 'InvoiceList[' + index + '].is_TANG').attr('id', 'InvoiceList_' + index + '__is_TANG');
            $(tr).find('.hidden_price').attr('name', 'hidden[' + index + '].Price').attr('id', 'hidden_' + index + '__Price');
            $(tr).find('td:last-child').attr('id', 'hidden_' + index + '__Total');
        });
        calcTotalAmount(".productInvoiceList");
    });
    
});
function calcAmountItem(id, tbody) {
    var $this = $(tbody + ' tr[role="' + id + '"]');
    var input_price = $this.find('.item_price');
    var _price = input_price.val() != '' ? removeComma(input_price.val()) : 0;
    var totalDiscount = parseFloat($('#Total_Discount').val());
    
    var $qty = $this.find('.item_quantity');
    var qty = 1;
    if ($qty.val() == '') {
        $qty.val(1);
    } else {
        qty = parseInt(removeComma($qty.val())) < 0 ? parseInt(removeComma($qty.val())) * -1 : parseInt(removeComma($qty.val()));
    }
    var total = parseFloat(_price) * qty * ((100 - totalDiscount)/100);
    $this.find('.item_total').text(numeral(total).format('0,0'));
    $this.find('.hidden_total').text(numeral(total).format('0,0'));

    console.log('total when calculate:' + total);
};

function calcTotalAmount(tbody) {
    var total = 0;
    var total1 = 0;

    var selector = tbody + ' tr';
    $(selector).each(function (index, elem) {
        if ($(elem).find('.item_total').text() != '') { // la số thì mới tính
            total += parseFloat(removeComma($(elem).find('.item_total').text()));
            $(tbody + "_Amount").text(numeral(total).format('0,0'));
            console.log('total_amount_after_change_input:' + total)
        }

        if ($(elem).find('.item_quantity').val() != '') { // la số thì mới tính
            total1 += parseInt(removeComma($(elem).find('.item_quantity').val()));
            $(tbody + "_SL").text(numeral(total1).format('0,0'));
        }

        if (index == $(selector).length - 1) {
            $('#mask-TotalAmount').val(numeral(total).format('0,0'));
            $('#TotalAmount').val(numeral(total).format('0,0'));
        }
        var vat = parseFloat(removeComma($('#TaxFee').val()));
        var totalVAT = total + total * vat / 100;
        $('#mask-TongTienSauVAT').val(numeral(totalVAT).format('0,0'));
        $('#TongTienSauVAT').val(numeral(totalVAT).format('0,0'));
    });
};
$(window).keydown(function (e) {
    if (e.which == 115) {   // khi nhấn F4 trên bàn phím hiển thị dữ liệu dropdownlist
        e.preventDefault();
        $("#Product").focus();
    }
});
$(function () {
    //$('#item_origin')[0].selectedIndex = 0;
    $('#item_origin').change(function () {
        var selectedEventType = this.options[this.selectedIndex].value;
        if (selectedEventType == "all") {
            $('#Product').removeAttr('disabled');
            $('#test').val(selectedEventType);
            var index = $(this)[0].selectedIndex;
            $('#CountForBrand')[0].selectedIndex = index;
        } else {
            $('#Product').removeAttr('disabled');
            var index = $(this)[0].selectedIndex;
            $('#CountForBrand')[0].selectedIndex = index;
            $('#test').val(selectedEventType);
        }
    });
                    //if ($('#test').val()=="all") {
                //    $this.ul.show();
                //    $this.ulShowHide = true;
                //} 
                //else {
                //    $this.ul.show();
                //    $this.ulShowHide = true;
                //    $this.ul.find('li').hide();
                //    //$this.ul.find('#' + $('#test').val()).show();
                //    $('.item').filter('#' + $('#test').val()).show();
                //}
});

//Checkbox event
$('#listOrderDetail').on('click', '.item_is_TANG', function () {

    $('.productInvoiceList tr').filter(':has(:checkbox)').each(function (elem) {
        debugger
        var dda = parseFloat(removeComma(this.textContent));
        console.log('id_dda -1:' + (dda))

        var ischecked = $('#InvoiceList_' + dda + '__is_TANG').is(":checked");
        console.log('ischecked:' + ischecked);

        var total;
        var total1;
        if (ischecked) {
            total = parseFloat($('.productInvoiceList tr').find('#InvoiceList_' + dda + '__Total').children("span").text(0));
            total1 = parseFloat($('.productInvoiceList tr').find('#InvoiceList_' + dda + '__Price').val(0));
            $('#InvoiceList_' + dda + '__is_TANG').val(1);
        }
        else {
            var a = $('.productInvoiceList tr').find('#hidden_' + dda + '__Price');
            var hidden_price = $('.productInvoiceList tr').find('#hidden_' + dda + '__Price').val();
            //var hidden_total = $('.productInvoiceList tr').find('#hidden_' + dda + '__Total').children("span").text()
            var hidden_total = removeComma(hidden_price) * removeComma($('#InvoiceList_' + dda + '__Quantity').val());
            //console.log('item price:' + item_price)
            console.log('hidden total:' + hidden_total)
            total = parseFloat($('.productInvoiceList tr').find('#InvoiceList_' + dda + '__Total').children("span").text(hidden_total));
            total1 = parseFloat($('.productInvoiceList tr').find('#InvoiceList_' + dda + '__Price').val(hidden_price));
        }

    });
    calcTotalAmount(".productInvoiceList");
});


