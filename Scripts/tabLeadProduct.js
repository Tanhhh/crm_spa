
var $tr_template_invoice = $('.productInvoiceList tr:first-child');
var idlead = $('#idleadproduct').data('id');
var isPartial = $('#isPartialdiv').data('id');
var arrPrice = [];
$(function () {
    data_selecteds = [];

    $("#Product").combojax({
        url: "/Product/GetListProductInventoryAndService",
        onNotFound: function (p) {  //khi tìm ko thấy dữ liệu thì sẽ hiển thị khung thêm mới dữ liệu
            //OpenPopup('/Customer/Create?IsPopup=true&Phone=' + p, 'Thêm mới', 500, 250);
        },
        onSelected: function (obj) {
            console.log(obj);
            if (obj) {

                debugger
                var len = $('.productInvoiceList tr').length;
                if (len > 1) {

                }
                    var tr_new = $tr_template_invoice.clone()[0].outerHTML;
                    tr_new = tr_new.replace(/\[0\]/g, "[" + len + "]");
                    tr_new = tr_new.replace(/\_0\__/g, "_" + len + "__");
                    var $tr_new = $(tr_new);
                    $tr_new.attr('role', len);
                    $tr_new.attr('id', len);
                    $tr_new.find('.pilstt').text(len);
                    $tr_new.find('.item_product_id').val(obj.Id);
                    $tr_new.find('.item_quantity').val(1);
                    $tr_new.find('.item_quantity_txt').text(1);
                    $tr_new.find('.item_product_name').text(obj.Name);
                    $tr_new.find('.item_product_origin').text(obj.Origin);
                    if (obj.Categories == "KHT") {
                        $tr_new.find('.item_is_TANG').attr("checked", "checked");
                        $tr_new.find('.item_price').text(0);
                        $tr_new.find('.item_total').text(0);
                        $tr_new.find('.hidden_price').val(0);
                        $tr_new.find('.hidden_total').text(0);
                    } else {
                        $tr_new.find('.item_is_TANG').text(obj.is_TANG);
                        //  total += parseFloat(removeComma($(elem).find('.item_total').text()));
                        //$(tbody + "_Amount").text(numeral(total).format('0,0'));
                        $tr_new.find('.item_price').text(numeral(obj.Price).format('0,0')).attr("readonly", "readonly");
                        $tr_new.find('.item_total').text(numeral(obj.Amount).format('0,0'));    
                        $tr_new.find('.hidden_price').val(obj.Price).attr("readonly", "readonly");
                        $tr_new.find('.hidden_total').text(obj.Amount);
                    }

                    $('.productInvoiceList').append($tr_new);
                    var $tr_after_append = $('.productInvoiceList tr[role="' + len + '"]');
                    $tr_after_append.removeAttr("hidden", "hidden");
                    $tr_after_append.addClass("alert-warning");
                    data_selecteds.push(obj);
                //}
                $.mask.definitions['~'] = '[+-]';

                calcAmountItem(len, ".productInvoiceList");
                calcTotalAmount(".productInvoiceList");
                LoadNumberInput(); 
                $('#saveLeadProduct').css('visibility', 'visible');
            }

        }       //chỗ xử lý khi chọn 1 dòng dữ liệu ngoài việc lưu vào textbox tìm kiếm
    , onShowImage: true                  //hiển thị hình ảnh
    , onSearchSaveSelectedItem: false    //lưu dòng dữ liệu đã chọn vào ô textbox tìm kiếm, mặc định lưu giá trị value trong hàm get data
    , onRemoveSelected: false  //những dòng đã chọn rồi thì sẽ ko hiển thị ở lần chọn tiếp theo
    });


    /*thay đổi note*/
    $('#listOrderDetail').on('change', '.item_note', function () {
        $(this).parent().parent().find('.item_note_txt').text($(this).val());
    });

    
     /*tính thành tiền và tổng cộng*/
    $('#listOrderDetail').on('change', '.item_quantity', function () {
        debugger;
        var $this = $(this);
        var id = $this.closest('tr').attr('id');
        $(this).val($(this).val().replace(/\-/g, ''));
        $(this).val($(this).val().replace(/[^0-9.,]/g, ''));
        $(this).parent().parent().find('.item_quantity_txt').text($(this).val().toString());
        var ralVal = numeral($(this).val());
        if (ralVal <= 0) {
            $(this).val(1);
        }
        //tính tổng cộng
        calcAmountItem(id, ".productInvoiceList");
        calcTotalAmount(".productInvoiceList");
    });
   
    $('#listOrderDetail').on('change', '.item_price', function () {
        var $this = $(this);
        var id = $this.closest('tr').attr('id');
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
        debugger;
        if ($(this).attr('id') === 'deleteajax') {
            var itemId = $(this).parent().parent().find('.item_id').val();
            $.ajax({
                url: '/AdviseCard/DeleteLeadQuotationDetail',
                type: 'POST',
                data: { Id: itemId },
                success: function (result) {
                    debugger;
                    if (result.success) {
                        console.log('Delete Row Thành công');
                    } else {
                        console.log('Delete Row Thất bại');
                    }
                }
            })
        } else {

        }
        $(this).closest('tr').remove();

        $('.productInvoiceList tr').each(function (index, tr) {

            if (arrPrice !== null);
            $(tr).attr('id', index);
            $(tr).find('.pilstt').text(index);
            $(tr).children().eq(6).attr('id', 'InvoiceList_' + index + '__Total');
            $(tr).find('.item_product_id').attr('name', 'InvoiceList[' + index + '].ProductId').attr('id', 'InvoiceList_' + index + '__ProductId');
            $(tr).find('.item_id').attr('name', 'InvoiceList[' + index + '].Id').attr('id', 'InvoiceList_' + index + '__Id');
            $(tr).find('.item_quantity').attr('name', 'InvoiceList[' + index + '].Quantity').attr('id', 'InvoiceList_' + index + '__Quantity');
            $(tr).find('.item_price').attr('name', 'InvoiceList[' + index + '].Price').attr('id', 'InvoiceList_' + index + '__Price');
            $(tr).find('.item_unit').attr('name', 'InvoiceList[' + index + '].Unit').attr('id', 'InvoiceList_' + index + '__Unit');
            $(tr).find('.item_note').attr('name', 'InvoiceList[' + index + '].Note').attr('id', 'InvoiceList_' + index + '__Note');
            $(tr).find('.item_origin').attr('name', 'InvoiceList[' + index + '].Origin').attr('id', 'InvoiceList_' + index + '__Origin');
            $(tr).find('.item_is_TANG').attr('name', 'InvoiceList[' + index + '].is_TANG').attr('id', 'InvoiceList_' + index + '__is_TANG');
            $(tr).find('.hidden_price').attr('name', 'hidden[' + index + '].Price').attr('id', 'hidden_' + index + '__Price');
            $(tr).find('td:last-child').attr('id', 'hidden_' + index + '__Total');
        });
        calcTotalAmount(".productInvoiceList");
        debugger;
        var ele = $('#listOrderDetail').find('.btn-delete-item');
        if (ele.length > 1) { }
        else {
            $('#saveLeadProduct').css('visibility', 'hidden');
            var flag = false;
            $('.productInvoiceList tr').each(function () {
                if ($(this).attr('role') !== '0') {
                    flag = true;
                }   
            });
            if (!flag) {
                $(".productInvoiceList_Amount").text(0);
                if ($('#saveLeadQuotationPP').length > 0) {
                    $('#saveLeadQuotationPP').css('visibility', 'hidden');
                }
                
            }
        }
        
    });
    
});
function calcAmountItem(id, tbody) {
    debugger;
    var $this = $(tbody + ' tr[id="' + id + '"]');
    var input_price = $this.find('.hidden_price');
    var _price = input_price.val() != '' ? removeComma(input_price.val()) : 0;
    console.log('_price:' + _price);
    var $qty = $this.find('.item_quantity');
    var qty = 1;
    if ($qty.val() == '') {
        $qty.val(1);
    } else {
        qty = parseInt(removeComma($qty.val())) < 0 ? parseInt(removeComma($qty.val())) * -1 : parseInt(removeComma($qty.val()));
    }
    var total = parseFloat(_price) * qty;
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
        } else {
            $('#Product').removeAttr('disabled');
            $('#test').val(selectedEventType);
        }
    });
               
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
            $('#InvoiceList_' + dda + '__is_TANG').val(1);
        }
        else {
            var a = $('.productInvoiceList tr').find('#hidden_' + dda + '__Price');
            var hidden_price = $('.productInvoiceList tr').find('#hidden_' + dda + '__Price').val();
            //var hidden_total = $('.productInvoiceList tr').find('#hidden_' + dda + '__Total').children("span").text()
            var hidden_total = removeComma(hidden_price) * removeComma($('#InvoiceList_' + dda + '__Quantity').val());
            //console.log('item price:' + item_price)
            console.log('hidden total:' + hidden_total)
            total = parseFloat($('.productInvoiceList tr').find('#InvoiceList_' + dda + '__Total').children("span").text(numeral(hidden_total).format('0,0')));
            total1 = parseFloat($('.productInvoiceList tr').find('#InvoiceList_' + dda + '__Price').val(numeral(hidden_price).format('0,0')));
        }
        
    });
    calcTotalAmount(".productInvoiceList"); 
});
var invoiceItems = [];
$('#saveLeadProduct').on('click', function () {
    debugger;
    $('.productInvoiceList tr').each(function () {

        if ($(this).attr('role') !== '0') {
            var item = {
                LeadId: idlead,
                ProductId: $(this).find('.item_product_id').val(),
                Quantity: $(this).find('.item_quantity').val(),
                Price: $(this).find('.item_price').text(),
                TotalAmount: $(this).find('.item_total').text(),
                Note: $(this).find('.item_note').val(),
                IsTang: $(this).find('.item_is_TANG').is(':checked'),
            };
            invoiceItems.push(item);
        }
    });
    debugger;
    if (isPartial == '') {
        var dataform = {
            datalist: invoiceItems
        };
    } else {
        var dataform = {
            datalist: invoiceItems,
            isPartial: isPartial
        };
    }
    
    $.ajax({
        url: '/AdviseCard/CreateLeadProduct',
        type: 'POST',
        data: JSON.stringify(dataform),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (result) {
            if (result.success) {
                DetailLeadProduct(idlead);
                console.log('Thành công');
            } else {
                console.log('Thất bại');
            }
           
        },
        error: function (xhr, status, error) {
            // Xử lý lỗi
        }
    });
});
//format money
function currencyFormatDE(num) {
    return (
        num.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1.')
    )
}
var editEnable = false;
var table = document.getElementById("productleadtable");
var rows = table.getElementsByTagName("tr");
for (i = 0; i < rows.length; i++) {
    let currentRow = table.rows[i];
    currentRow.onclick = function (e) {
        if ($(e.target).is('.checkbox_child')) {
            var ele = $('#listOrderDetail').find('.checkbox_child:checked')
            if (ele.length > 0) {
                    $('#editEnableLPShow').css('visibility', 'visible');
                    $('#editshowLP').css('visibility', 'visible');
            } else {
                $('#editEnableLPShow').css('visibility', 'hidden');
                $('#editshowLP').css('visibility', 'hidden');
            }
            if (editEnable) {
                var chkele = $('#listOrderDetail').find('.checkbox_child')
                debugger
                $.each(chkele, function () {
                    if ($(this).is(':checked')) {
                        $(this).parent().parent().find('.item_quantity').prop('hidden', false);
                        $(this).parent().parent().find('.item_note').prop('hidden', false);
                        $(this).parent().parent().find('.txt').prop('hidden', true);
                        $(this).parent().parent().find('.item_is_TANG').attr('disabled', false);
                    } else {
                        $(this).parent().parent().find('.item_quantity').prop('hidden', true);
                        $(this).parent().parent().find('.item_note').prop('hidden', true);
                        $(this).parent().parent().find('.txt').prop('hidden', false);
                        $(this).parent().parent().find('.item_is_TANG').attr('disabled', true);
                    }
                })
            }
        }
    }
}

$('#QuickEditLeadProductEnable').click(function () {
    debugger;
    $('#editLPDisableShow').prop('hidden', false);
    $('#QuickEditLeadProductEnable').prop('style', 'display:none;');
    editEnable = true;
    var chkele = $('#listOrderDetail').find('.checkbox_child')
    $.each(chkele, function () {
        if ($(this).is(':checked')) {
            $(this).parent().parent().find('.item_quantity').prop('hidden', false);
            $(this).parent().parent().find('.item_note').prop('hidden', false);
            $(this).parent().parent().find('.txt').prop('hidden', true);
            $(this).parent().parent().find('.item_is_TANG').attr('disabled', false);
        } else {
            $(this).parent().parent().find('.item_quantity').prop('hidden', true);
            $(this).parent().parent().find('.item_note').prop('hidden', true);
            $(this).parent().parent().find('.txt').prop('hidden', false);
            $(this).parent().parent().find('.item_is_TANG').attr('disabled', true);
        }
    });
});
$('#QuickEditLeadProductDisable').click(function () {
    $('#editLPDisableShow').prop('hidden', true);
    $('#QuickEditLeadProductEnable').prop('style', 'display:inline-block;');
    $('#editEnableLPShow').css('visibility', 'hidden');
    $('#editshowLP').css('visibility', 'hidden');
    editEnable = false;
    var chkele = $('#listOrderDetail').find('.checkbox_child:checked')
    $.each(chkele, function () {
        $(this).prop('checked', false);
        $(this).parent().parent().find('.item_quantity').prop('hidden', true);
        $(this).parent().parent().find('.item_note').prop('hidden', true);
        $(this).parent().parent().find('.txt').prop('hidden', false);
        $(this).parent().parent().find('.item_is_TANG').attr('disabled', true);
    });
});

$('#QuickEditLeadProduct').click(function () {
    debugger;
    var ele = $('#listOrderDetail').find('.checkbox_child:checked')
    let outvoiceItems = [];
    $.each(ele, function () {
        var itemi = {
            LeadId: $(this).parent().parent().find('.item_id').val(),
            ProductId: $(this).parent().parent().find('.item_product_id').val(),
            Quantity: $(this).parent().parent().find('.item_quantity').val(),
            Price: $(this).parent().parent().find('.item_price').text(),
            TotalAmount: $(this).parent().parent().find('.item_total').text(),
            Note: $(this).parent().parent().find('.item_note').val(),
            IsTang: $(this).parent().parent().find('.item_is_TANG').is(':checked'),
        };
        outvoiceItems.push(itemi);
    })
    if (isPartial == '') {
        var dataform = {
            datalist: outvoiceItems,
            leadId: idlead
        };
    } else {
        var dataform = {
            datalist: outvoiceItems,
            leadId: idlead,
            isPartial: isPartial
        };
    }
    $.ajax({
        url: "/AdviseCard/EditLeadProduct",
        method: "post",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(dataform),
        dataType: "json",
        success: function (result) {
            if (result.success) {
                DetailLeadProduct(idlead);
                console.log('Thành công');
            } else {
                console.log('Thất bại');
            }
        }
    })
});
$('#editshowLP').click(function () {
    debugger
    Swal.fire({
        title: 'Bạn có chắc chắn muốn xóa những Sản phẩm này không?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Xóa',
        cancelButtonText: 'Không'
    }).then((result) => {
        if (result.isConfirmed) {
            var ele = $('#listOrderDetail').find('.checkbox_child:checked')
            let arr = []
            $.each(ele, function (idx, item) {
                var productIdInput = $(this).closest('td').find('.item_id');
                arr.push(productIdInput.val());
            })
            if (isPartial == '') {
                var dataform = {
                    datalist: arr
                };
            } else {
                var dataform = {
                    datalist: arr,
                    isPartial: isPartial
                };
            }
            $.ajax({
                url: "/AdviseCard/DeleteLeadProduct",
                method: "post",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(dataform),
                dataType: "json",
                success: function (result) {
                    if (result.success) {
                        DetailLeadProduct(idlead);
                        console.log('Thành công');
                    } else {
                        console.log('Thất bại');
                    }
                }
            })
        } else {

        }
    })
})


