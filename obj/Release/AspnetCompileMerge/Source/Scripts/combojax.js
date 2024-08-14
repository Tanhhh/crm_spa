/***
* Combojax
*/
function ignoreSpaces(string) {
    var temp = "";
    string = '' + string;
    splitstring = string.split(" ");
    for (i = 0; i < splitstring.length; i++)
        temp += splitstring[i];
    return temp;
}
window.pageCombojaxs = window.pageCombojaxs || {};
$.fn.extend({
    combojax: function (params) {
        var aObj = [];
        $(this).each(function () {
            if (!$(this).data("combojax")) {
                var combojax = new ComboJax($(this), params);
                var name = $(this).attr("id");
                if (name.length > 0)
                    window.pageCombojaxs[name] = combojax;

                aObj.push(combojax);
                $(this).data("combojax", combojax);
            } else {
                aObj.push($(this).data("combojax"));
            }
        });
        if (aObj.length == 1)
            return aObj[0];
        return aObj;
    }
});

ComboJax = (function ($) {
    function comboJax(input, options) {        
        this.input = input;
        this.input.attr("data-id", "");
        this.id = 'combojax_' + input.attr("id");
        input.parent().append('<div class="combojax"></div>');
        input.parent().find('div').attr('id', this.id);
        this.container = $("#" + this.id);
        input.appendTo(this.container);
        this.container.append('<ul></ul>');
        this.ul = $("#" + this.id + ' ul');
        this.data = [];
        this.data_selected = [];
        options = options || {};
        this.options = $.extend({}, this.defaults(), options);
        this.init();
    }

    /**** init ************************/
    comboJax.prototype.init = function () {
        var $this = this;
        $this.ulShowHide = false;
      
        $.getJSON($this.options.url, {}, function (response) {
            $(response).each(function (index, obj) {
                //hiển thị hình ảnh
                if ($this.options.onShowImage == true) {
                    var image = " <div class='itemdiv commentdiv'>";
                             image += "<div class='user'>";
                                  image += "<div class='ace-thumbnails'>";
                                  image += "<a href='" + obj.Image + "' title='" + obj.Name + "' data-rel='colorbox' class='cboxElement'>";
                                          image += "<img id='myImg' src='" + obj.Image + "' style=''>";
                                  image += "</a>";
                                  image += "</div>";
                            image+="</div>";
                            image += "<div class='body'>";
                            image += "<div class='name'><span>" + obj.Text + "</span>";
                            if (obj.Note != undefined) {
                                image += "<i class='note red left-right' style='font-size:11px'>" + obj.Note + "</i>";
                            }
                            image += "</div>";
                            image+="</div>";
                    image += "</div>";
                    obj.i
                    var item = "<li class='item' data-id='" + obj.Id + "'>" + image + "</li>";
                    $this.ul.append(item);
                }
                else {
                    var item = "<li class='item' data-id='" + obj.Id + "'><div><span>" + obj.Text + "</span>";
                    if (obj.Note != undefined) {
                        item += "<i class='red pull-right' style='font-size:11px'>" + obj.Note + "</i>";
                    }
                    item += "</div></li>";
                    $this.ul.append(item);
                }
                $this.data.push(obj);
            });
            combojaxSelectItem($this.input.val().trim());
        });

        //------- input ------------------------------------
        $this.input.bind('click', function (event) {
            //console.log("combojax click");
            event.stopPropagation();
           
        });
      
        $this.input.bind('focus', function (event) {
            console.log("combojax focus");
            $(".combojax ul").hide();
            search($this.input.val().trim());
            $this.input.select();
        });

        $this.input.bind('blur', function (event) {
            console.log("combojax blur");
            combojaxSelectItem($this.input.val().trim());
            $this.input.val("");
        });

        var timeout = null;
        $this.input.keydown(function (e) {
            if (e.which == 37 || e.which == 39) {
                e.preventDefault();
            }
            else {
                if (e.which == 9) {
                    combojaxSelectItem($this.input.val());
                }
                else {
                    if (e.which == 40) {
                        var selectedItem = $this.ul.find(".item.selected");
                        if (!selectedItem.is(':last-child')) {
                            selectedItem.removeClass("selected").next().addClass("selected");
                            var name = selectedItem.next().find("div>span").text().trim();
                            //var loCode = selectedItem.next().find("div>i.loCode") 
                            var id = selectedItem.next().data("id");
                            //combojaxSelectItem(name);
                            $this.input.val(name);
                            $this.ul.scrollTop($this.ul.scrollTop() + selectedItem.next().position().top);
                        }
                    }
                    else if (e.which == 38) {
                        var selectedItem = $this.ul.find(".item.selected");
                        if (!selectedItem.is(':first-child')) {
                            selectedItem.removeClass("selected").prev().addClass("selected");
                            var name = selectedItem.prev().find("div>span").text().trim();
                            var id = selectedItem.prev().data("id");
                            //combojaxSelectItem(name);
                            $this.input.val(name);
                            $this.ul.scrollTop($this.ul.scrollTop() + selectedItem.prev().position().top);
                        }
                    }
                    else if (e.which == 13) {
                        var selectedItem = $this.ul.find(".item.selected");
                        var id = selectedItem.data("id");
                        var name = selectedItem.find("div>span").text().trim();

                        //Gọi callback khi không tìm thấy kết quả
                        if (name == '' && $this.options.onNotFound != undefined)
                        {
                            $this.options.onNotFound($this.input.val().trim());
                        }

                        combojaxSelectItem(name);

                        showList(!$this.ulShowHide);
                    }
                    else {
                        if ($this.ulShowHide == false) {
                            showList(true);
                        }
                    }
                }
            }
        });
      
      
        $this.input.keyup(function (e) {
            if (e.which == 13 || e.which == 37 || e.which == 38 || e.which == 39 || e.which == 40)
                return;

            var searchText = $(this).val().trim();
            //if (searchText == '') {
            //    $this.input.data("id", "");
            //    $this.input.trigger("change");
            //}

            //if you already have a timout, clear it
            if (timeout) { clearTimeout(timeout); }

            //start new time, to perform ajax stuff in 500ms
            timeout = setTimeout(function () {
                search(searchText);
            }, 0);
        });
      
        //Sự kiện khi bấm ra ngoài thì ẩn khung tìm kiếm
        $(window).click(function (event) {
            $this.ul.hide();
        });

        //--------- function -------------------------------
        var combojaxSelectItem = function combojaxSelectItem(name, loCode) {
            console.log("combojaxSelectItem: " + name, loCode);

            var found = false;
            var obj = null;
            for (var i in $this.data) {
                if ($this.data[i].Text.trim() == name.trim() && $this.data[i].LoCode == loCode) {
                    found = true;
                    obj = $this.data[i];
                    if ($this.options.onSearchSaveSelectedItem == true) {
                        $this.input.val(obj.Text);
                        $this.input.data("id", obj.Id);
                        $this.input.data("value", obj.Value);
                    }

                    break;
                }
                if ($this.data[i].Text.trim() == name.trim() && $this.data[i].LoCode != loCode) {
                    found = true;
                    obj = $this.data[i];
                    if ($this.options.onSearchSaveSelectedItem == true) {
                        $this.input.val(obj.Text);
                        $this.input.data("id", obj.Id);
                        $this.input.data("value", obj.Value);
                    }

                    $this.input.val('');
                    $this.input.data("id", "");
                    $this.input.data("value", "");

                    break;
                }
            }
            if ($this.options.onSearchSaveSelectedItem == true) {
                if (!found) {
                    $this.input.val('');
                    $this.input.data("id", "");
                    $this.input.data("value", "");
                }
            }
            else {
                $this.input.val('');
                $this.input.data("id", "");
                $this.input.data("value", "");
            }
            $this.input.trigger("change");
            if ($this.options.onSelected != undefined) {
                $this.options.onSelected(obj);
                if ($this.options.onRemoveSelected == true) {
                    if (obj != null) {
                        $this.data_selected.push(obj);
                    }
                }
            }
        };

        function showList(show) {
            if (show) {
                $this.ul.show();
                $this.ulShowHide = true;
                var value = ignoreSpaces($('#test').val());//lay nhan hang
                if ((value == "all") || (value == "undefined")) {
                    $this.ul.show();
                    $this.ulShowHide = true;
                } 
                else {
                    $this.ul.show();
                    $this.ulShowHide = true;
                    $this.ul.find('li').hide();
                    //$this.ul.find('#' + $('#test').val()).show();
                    $('.item').filter('#' + value).show();
                }
            }
            else {
                $this.ul.hide();
                $this.ulShowHide = false;
            }
        };
        function ReloadPreviewImage() {
            var $overflow = '';
            var colorbox_params = {
                rel: 'colorbox',
                reposition: true,
                scalePhotos: true,
                scrolling: false,
                previous: '<i class="ace-icon fa fa-arrow-left"></i>',
                next: '<i class="ace-icon fa fa-arrow-right"></i>',
                close: '&times;',
                current: '{current} of {total}',
                maxWidth: '100%',
                maxHeight: '100%',
                onOpen: function () {
                    $overflow = document.body.style.overflow;
                    document.body.style.overflow = 'hidden';
                },
                onClosed: function () {
                    document.body.style.overflow = $overflow;
                },
                onComplete: function () {
                    $.colorbox.resize();
                }
            };

            $('.ace-thumbnails [data-rel="colorbox"]').colorbox(colorbox_params);
        };
        function search(searchText) {
            $this.ul.html("");
            // Toán tử số học (Arithmetic operators), Toán tử quan hệ (Relational operator), Toán tử logic (Logical operator), Toán tử điều kiện (Condition operator), Các toán tử tăng, giảm (Increment and decrement operator), Toán tử gán (Assignment operator)
            var relational_operator = '>,>=,<,<=,=';
            var isSearchNumber = true;

            //lấy từ đầu tiên (tính bằng khoảng trắng) trong nội dung tìm kiếm, xem có các toán tử so sánh hay không để tìm kiếm cho giá trị số
            var first_word = searchText.split(' ')[0].trim().replace(/\d/g, '');
            if (relational_operator.indexOf(first_word) != -1) {
                isSearchNumber = true;

                // nếu tìm kiếm cho số thì bỏ đi các toán tử so sánh, chỉ giữ lại số
                searchText = searchText.replace(/\D/g, '');
            }

            searchText = convertVNtoEN(searchText);
            var result = [];

            //if (searchText != '') {
            var numberOfItem = 0;
            for (var i in $this.data) {
                var status_data_Selected = false;//mặc định là không trùng text đã chọn
                var obj = $this.data[i];
                //
                if ($this.options.onRemoveSelected == true) {
                    if ($this.data_selected.length > 0) {
                        for (var ii in $this.data_selected) {
                            if ($this.data_selected[ii].Text == obj.Text) {
                                status_data_Selected = true;
                            }
                        }
                    }
                }
                //
                if (status_data_Selected == false) {//nếu ko có trong array selected thì cho hiển thị để chọn
                    //  if (numberOfItem <= 10) {
                    if (convertVNtoEN(obj.Text).indexOf(searchText) != -1) {
                        result.push(obj);
                        numberOfItem++;
                    }
                    //    }
                }
            }
            //}

            if (result.length > 0) {
                for (i = 0; i < result.length; i++) {
                    //hiển thị hình ảnh
                    if ($this.options.onShowImage == true) {
                        var image = " <div class='itemdiv commentdiv'>";
                        image += "<div class='user'>";
                        image += "<div class='ace-thumbnails'>";
                        image += "<a href='" + result[i].Image + "' title='" + result[i].Name + "' data-rel='colorbox' class='cboxElement'>";
                        image += "<img id='myImg' src='" + result[i].Image + "' style=''>";
                        image += "</a>";
                        image += "</div>";
                        image += "</div>";
                        image += "<div class='body'>";
                        image += "<div class='name'><span>" + result[i].Text + "</span>";
                        image += "<div class='tfoot'>";
                        if (result[i].Note != undefined) {
                            image += "<i class='red pull-right' style='font-size:11px'>" + result[i].Note +"</i>";
                        }
                        image += "<i class='blue pull-right' style='font-size:11px'>"+ result[i].Categories +"</i>";
                        if (result[i].Type == "service") {
                            image += "<i class='blue pull-left' style='font-size:11px'>" + (result[i].Origin == null ? "" : "Nhãn hàng: " + result[i].Origin) + "</i>";
                        } else {
                            image += "<i  style='color:DarkOrange' class='pull-left' style='font-size:11px'>" + (result[i].Origin == null ? "" : "Nhãn hàng: " + result[i].Origin) + "</i>";
                        }
                        image += "<i hidden  style='color:DarkOrange' class='pull-left' style='font-size:11px'>" + (result[i].LoCode == null ? "" : "Nhãn hàng: " + result[i].LoCode) + "</i>";
                        image += "</div>";
                        image += "</div>";
                        image += "</div>";
                        image += "</div>";
                        var item = "<li class='item' id='" + ignoreSpaces(result[i].Origin) +"' style='height:50px' data-id='" + result[i].Id + "'>" + image + "</li>";
                        $this.ul.append(item);
                        ReloadPreviewImage();
                    }
                    else {
                        var item = "<li class='item' data-id='" + result[i].Id + "'><div><span>" + result[i].Text + "</span>";
                        if (result[i].Note != undefined) {
                            item += "<i class='red pull-right' style='font-size:11px'>" + result[i].Note + "</i>";
                        }
                        item+="</div></li>";
                        $this.ul.append(item);
                    }
                }

                showList(true);

                $this.ul.find(".item:first").addClass("selected");

                $this.ul.find('li').bind('click', function (event) {
                    console.log("combojax click in ul");
                    var text = $(this).find("div>span").text();
                    var loCode = $(this).find(".loCode").text();
                    combojaxSelectItem(text.trim(), loCode.trim());
                    showList(false);
                });
            }
        };
       
    };

    comboJax.prototype.defaults = function () {
 
        return {
            url: ''
        };
    };

    comboJax.prototype.addItem = function (obj) {
        console.log("comboJax.prototype.addItem", obj);
        var $this = this;
        $this.data.push(obj);
        $this.input.val(obj.Text);
        $this.input.focus();
    };

    return comboJax;
})(window.jQuery);