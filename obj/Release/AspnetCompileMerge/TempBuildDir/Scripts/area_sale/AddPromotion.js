
/*********** page load ***************/
$(function () {
    LoadNumberInput();
   
    //VAT thay đổi sdsd
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

    calcTotalAmount(".productInvoiceList");

    $('#TaxFee').focus(function () {
        $(this).select();
    });

    $(".commission_check_save").change(function () {
       

        if ($(this).is(':checked')) {
            $(this).val("True");
        } else {
            $(this).val("False");
        }
    });
    $("#Discount").change(function () {
        var total = parseFloat(removeComma($('#TotalAmount').val()));
        var ck= parseFloat(removeComma($(this).val()));
        if(ck>100)
        {
            ck = 100;
            $(this).val(ck);
        }
        var discount_amount = ck * total / 100;
        $('#DiscountAmount').val(numeral(discount_amount).format('0,0'));
    });
    $("#DiscountAmount").change(function () {
        var total = parseFloat(removeComma($('#TotalAmount').val()));
        var ck=parseFloat(removeComma($(this).val()));
        if (ck > total)
        {
            ck = total;
            $(this).val(ck);
        }
        var discount_amount = ck * 100 / total;
        $('#Discount').val(numeral(discount_amount).format('0,0'));
    });
});

var $tr_template_invoice = $('.productInvoiceList tr:first-child');
$(function () {
    // tính thành tiền và tổng cộng
    $('#listOrderDetail').on('change', '.item_discount', function () {
        $(this).val($(this).val().replace(/\-/g, ''));
        $(this).val($(this).val().replace(/[^0-9.,]/g, ''));
        var $this = $(this);
        var id = $this.closest('tr').attr('role');
        //
        var discount = $(this).val();
        if (discount != "") {
            var _discount = parseFloat(removeComma(discount));
            if (_discount > 100)
            {
                $this.val(100);
                _discount = 100;
            }
            var total = $this.closest('tr').find(".item_total1").text();
            var discount_amount = parseFloat(removeComma(total)) * _discount / 100;
            $("#InvoiceList_" + id + "__DiscountAmount").val(numeral(discount_amount).format('0,0'));
        }
        //tính tổng cộng
        calcAmountItem(id, ".productInvoiceList");
        calcTotalAmount(".productInvoiceList");
    });

    $('#listOrderDetail').on('change', '.item_discount_amount', function () {
        $(this).val($(this).val().replace(/\-/g, ''));
        $(this).val($(this).val().replace(/[^0-9.,]/g, ''));
        var $this = $(this);
        var id = $this.closest('tr').attr('role');
        //
        var discount_amount = $(this).val();
        if (discount_amount != "") {
            var _discount_amount = parseFloat(removeComma(discount_amount));
            var total =  parseFloat(removeComma($this.closest('tr').find(".item_total1").text()));
            if (_discount_amount > total) {
                $this.val(total);
                _discount_amount = total;
            }
            var discount = _discount_amount * 100 /total;
            $("#InvoiceList_" + id + "__Discount").val(numeral(discount).format('0,0'));
        }
        //tính tổng cộng
        calcAmountItem(id, ".productInvoiceList");
        calcTotalAmount(".productInvoiceList");
    });
});
function calcAmountItem(id, tbody) {
    var $this = $(tbody + ' tr[role="' + id + '"]');
    var input_price = $this.find('.item_price');
    var _price = input_price.text() != '' ? removeComma(input_price.text()) : 0;
    var input_disount_amount = $this.find('.item_discount_amount');
    var _item_discount_amount = input_disount_amount.val() != '' ? removeComma(input_disount_amount.val()) : 0;

    var $qty = $this.find('.item_quantity');
    
    var qty = 1;
    //if ($qty.val() == '') {
    //    $qty.val(1);
    //} else {
        qty = parseInt(removeComma($qty.text())) < 0 ? parseInt(removeComma($qty.text())) * -1 : parseInt(removeComma($qty.text()));
    //}
    var total = parseFloat(_price) * qty;
    var total2 = total - parseFloat(_item_discount_amount);
   
    $this.find('.item_total1').text(numeral(total).format('0,0'));
    $this.find('.item_total2').text(numeral(total2).format('0,0'));
};

function calcTotalAmount(tbody) {
    var total = 0;
    var total1 = 0;
    var total2 = 0;
    var selector = tbody + ' tr';
    $(selector).each(function (index, elem) {
        if ($(elem).find('.item_total2').text() != '') { // la số thì mới tính
            total2 += parseFloat(removeComma($(elem).find('.item_total2').text()));
            $(tbody + "_Amount2").text(numeral(total2).format('0,0'));
        }
        if ($(elem).find('.item_total1').text() != '') { // la số thì mới tính
            total1 += parseFloat(removeComma($(elem).find('.item_total1').text()));
            $(tbody + "_Amount").text(numeral(total1).format('0,0'));
        }
        if ($(elem).find('.item_quantity').text() != '') { // la số thì mới tính
            total += parseInt(removeComma($(elem).find('.item_quantity').text()));
            $(tbody + "_SL").text(numeral(total).format('0,0'));
        }

        if (index == $(selector).length - 1) {
            $('#mask-TotalAmount').val(numeral(total2).format('0,0'));
            $('#TotalAmount').val(numeral(total2).format('0,0'));
        }
      
        var vat = parseFloat(removeComma($('#TaxFee').val()));
        var ck = parseFloat(removeComma($('#DiscountAmount').val()));
        var totalCK = total2 - ck;
        var totalVAT = totalCK + totalCK * vat / 100;
        $('#mask-TongTienSauVAT').val(numeral(totalVAT).format('0,0'));
        $('#TongTienSauVAT').val(numeral(totalVAT).format('0,0'));
    });
};

