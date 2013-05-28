(function ($) {
    var methods = {
        createNexTreeview: function (opts) {
            opts = $.extend({}, $.fn.nexTreeview.defaults, opts);
            return this.each(function () {
                return new NexTreeview(this, opts);
            });
        },
        appendItem: function (item) {
        },
        removeItem: function (item) {
        },
        updateItems: function (opts) {
            opts = $.extend({}, $.fn.nexTreeview.defaults, opts);
            return this.each(function () {
                return new NexTreeview(this, opts);
            });
        },
        checkAllItems: function () {
            var $inputs = $(this).next('div.ui-nextreeview-options').find('label').not('.ui-nextreeview-state-disabled').find('input');
            $inputs.not(':disabled').attr('checked', 'checked');
            var numChecked = $(this).next('div.ui-nextreeview-options').find('label').not('.ui-nextreeview-state-disabled').not('.ui-nextreeview-group-mark').find('input').not("#" + $(this).attr('id') + "HeaderCheckBox").filter('[checked]').length;

            $(this).find('input').val(numChecked + " selected");
        },
        unCheckAllItems: function () {
            var $inputs = $(this).next('div.ui-nextreeview-options').find('label').not('.ui-nextreeview-state-disabled').find('input');
            $inputs.not(':disabled').removeAttr('checked');
            var numChecked = $(this).next('div.ui-nextreeview-options').find('label').not('.ui-nextreeview-state-disabled').find('input').filter('[checked]').length;

            $(this).find('input').val($.fn.nextreeview.defaults.noneSelectedText);

        },
        getUncheckItems: function () {
            var listChecked = $(this).next('div.ui-nextreeview-options').find('label:not(.ui-nextreeview-state-disabled,.ui-nextreeview-group-mark)').find('input:not(:checked)').not("#" + $(this).attr('id') + "HeaderCheckBox");
            var tempArray = [];
            for (var i = 0; i < listChecked.length; i++) {
                tempArray.push(listChecked[i].value);
            }
            return tempArray;
        },
        getCheckedItems: function () {
            var listChecked = $(this).next('div.ui-nextreeview-options').find('label:not(.ui-nextreeview-state-disabled,.ui-nextreeview-group-mark)').find('input:not(#' + $(this).attr('id') + "HeaderCheckBox)").filter('[checked]');
            var tempArray = [];
            for (var i = 0; i < listChecked.length; i++) {
                tempArray.push(listChecked[i].value);
            }
            return tempArray;
        },
        hideMe: function () {
            $(this).next('div.ui-nextreeview-options').trigger('close', true, false);
        },
        openMe: function () {
            $(this).next('div.ui-nextreeview-options').trigger('open');
        }
    };

    $.fn.nexTreeview = function (opts) {
        if (methods[opts]) {
            return methods[opts].apply(this, Array.prototype.slice.call(arguments, 1));
        }
        else if (typeof opts === 'object' || !opts) {
            // Default to "createnextreeview"
            return methods.createNexTreeview.apply(this, arguments);
        }
        else {
            //throw error
        }
    };

    // counter to dynamically generate label/option IDs if they don't exist
    var nextreeviewID = 0;
    // built up a nextreeview
    var NexTreeview = function (select, o) {
        //global variables
        var $select = $original = $(select),
            $options, $header, $labels, $footer, $listGroupCheckBox, $body, $listItem, $listCollapse, $divHover,
            $allLabel,
            html = [],
            optgroups = [],
            isDisabled = $select.is(':disabled'),
            id = select.id || 'ui-nextreeview-' + nextreeviewID++, // unique ID for the label & option tags
            listData = o.listData,
            isShowGroup = false,
            isOpenning = false;
        //render MultiSleclect Interface
        var RenderInterface = function () {
            if ($select.prev().is('div.ui-nextreeview-options')) {
                $select.prev().remove();
            }
            if ($select.next().is('div.ui-nextreeview-options')) {
                $select.next().remove();
            }
            //render new
            var tempHtml = [];
            html.push('<div id="' + id + '" class="ui-nextreeview ui-nextreeview-widget ui-nextreeview-state-default ' + (isDisabled || o.disabled ? ' ui-nextreeview-state-disabled' : '') + '"></div>');
            return tempHtml.join('');
        };
        //render Header 
        var checkShouldShowGroupOrNo = function () {
            for (var i = 0; i < o.listData.length; i++) {
                if (typeof o.listData[i].Items !== 'undefined' && o.listData[i].Items.length > 0) {
                    isShowGroup = true;
                    break;
                }
            }
        };
        var RenderHeader = function (id, checkAllText) {
            var tempHtml = [];
            tempHtml.push('<div class="ui-nextreeview-widget-header ui-nextreeview-helper-clearfix ui-nextreeview-corner-all ui-nextreeview-header ui-widget-header">');
            tempHtml.push('<ul class="ui-nextreeview-helper-reset ui-nextreeview-checkboxes-head" style="overflow-y:hidden;">');
            tempHtml.push('<li style="display:inline;">');
            tempHtml.push('<label class="ui-nextreeview-corner-all" for="' + id + "HeaderCheckBox" + '">');
            tempHtml.push('<input type="checkbox" id="' + id + "HeaderCheckBox" + '">' + checkAllText + '</label></li>');
            if (isShowGroup) {
                tempHtml.push('<li class="ui-nextreeview-icon ui-nextreeview-icon-collapse" title="Collapse/Expand" style="display:inline; float:right; cursor:pointer;" id="' + id + "CollapseButton" + '" >&nbsp;</li>');
            }
            //tempHtml.push('</li>');
            tempHtml.push('</ul>');
            tempHtml.push('</div>');
            return tempHtml.join('');
        };
        //render Body
        var InsertRow = function (tempData, index, isChild, isLastChildItem, isNoGroup, siblingGroup) {
            var tempHtml = [];
            var title = tempData.Text || "",
            value = tempData.Value || "",
            inputID = tempData.Id || "ui-nextreeview-" + id + "-option-" + index,
            isDisabled = !tempData.Enable,
            labelClasses = tempData.CssClass || "",
            liClasses = "",
            divClasses = "ui-nextreeview-div ui-nextreeview-corner-all",
            divHtml = "";
            if (value.length > 0) {
                if (isDisabled) {
                    labelClasses += ' ui-nextreeview-state-disabled';
                }
                if (typeof tempData.Items !== 'undefined' && (typeof isChild === 'undefined' || isChild === false) && siblingGroup) {
                    labelClasses += ' ui-nextreeview-group-mark';
                }

                liClasses += (isDisabled ? ' ui-nextreeview-disabled ' : '');
                liClasses += (isNoGroup === false ? ' rtLI ' : '');
                liClasses += (isLastChildItem === true ? " rtLast" : "");
                liClasses += (index === 0 && isNoGroup === false) ? " rtFirst" : "";

                tempHtml.push('<li class="' + liClasses + '">');

                if (siblingGroup === false) {
                    if (isNoGroup === true) {
                        tempHtml.push('<div class="' + divClasses + '" style="margin-left:10px;">');
                    }
                    else {
                        divClasses += (isLastChildItem === true) ? ' rtBot' : ' rtMid';
                        tempHtml.push('<div class="' + divClasses + '">');
                    }
                }
                else {
                    divClasses += (index === 0) ? " rtTop" : " rtMid rtSelected";
                    divClasses += (isLastChildItem === true ? " rtBot" : "");
                    tempHtml.push('<div class="' + divClasses + '">');
                    tempHtml.push('<span class="collapseClass rtMinus"></span>');
                }

                labelClasses += " ui-nextreeview-corner-all";
                tempHtml.push('<label for="' + inputID + '" class="' + labelClasses + '" style="' + tempData.Css + '">');
                tempHtml.push('<input id="' + inputID + '" type="' + (o.multiple ? 'checkbox' : 'radio') + '" name="' + value + '" value="' + value + '" title="' + title + '"');

                if (o.isCheckAll) {
                    tempHtml.push(' checked="checked"');
                }
                else if (tempData.Checked) {
                    tempHtml.push(' checked="checked"');
                }
                if (isDisabled) {
                    tempHtml.push(' disabled="disabled"');
                }
                tempHtml.push(' />' + title + '</label>');
                tempHtml.push('</div>');

                if (typeof tempData.Items !== 'undefined' && tempData.Items.length > 0) {
                    tempHtml.push('<ul class="ui-nextreeview-ul-group rtUL">');
                    for (var j = 0; j < tempData.Items.length; j++) {
                        tempHtml.push(InsertRow(tempData.Items[j], j, true, (j === tempData.Items.length - 1 ? true : false), false, false));
                    }
                    tempHtml.push("</ul>");
                }

                tempHtml.push('</li>');
            }
            return tempHtml.join('');
        };
        var RenderBody = function () {
            try {

                var tempHtml = [];
                tempHtml.push('<ul class="ui-nextreeview-checkboxes ui-nextreeview-helper-reset rtUL rtLines ui-nextreeview-ul-group">');
                for (var i = 0; i < listData.length; i++) {
                    var tempGroup = listData[i];
                    var isChild = false;
                    var isLastChildItem = (i === listData.length - 1 ? true : false);
                    var isNoGroup = (typeof tempGroup.Items === 'undefined' || (typeof tempGroup.Items !== 'undefined' && tempGroup.Items.length === 0)) ? true : false;
                    var siblingIsGroup = isShowGroup ? true : false;
                    tempHtml.push(InsertRow(tempGroup, i, isChild, isLastChildItem, isNoGroup, siblingIsGroup));
                }
                tempHtml.push('</ul>');

            } catch (e) {
                console.log(e.toString());
            }
            return tempHtml.join('');
        };
        //render Footer
        var RenderFooter = function () {
            var tempHtml = [];
            tempHtml.push('<div class="ui-nextreeview-footer">');
            tempHtml.push('<label style="font-weight:normal; color: #FFFFFF;">Total:  ' + ' ' + 'selected </label>');
            tempHtml.push('</div>');
            return tempHtml.join('');
        };
        //
        //add property value
        var AddPropertyValue = function () {
            // calculate widths
            var iconWidth = $select.find('span.ui-nextreeview-icon').outerWidth();
            var inputWidth = o.minWidth - iconWidth;

            // set widths
            $select.width(o.minWidth).find('input').width(inputWidth);

            var offset = $select.position(),
            $container = $options.find('ul:last');

            // calling select is active
            $select.addClass('ui-nextreeview-state-active');
            // show the options div + position it
            $options.css({
                //                position: '',
                //                top: offset.top + 'px',
                //                left: offset.left + 'px',
                width: $select.width() + 'px'
            }).show();

            // set the scroll of the checkbox container
            $container.scrollTop(0);

            // set the height of the checkbox container

            $body.css('height', o.maxHeight);


        };
        // binding event
        var checkCheckAll = function () {
            if ($labels.find('input').not("#" + id + "HeaderCheckBox").length === $labels.find('input[checked]').length) {
                $("#" + id + "HeaderCheckBox").attr('checked', 'checked');
                $listGroupCheckBox.attr('checked', 'checked');
            }
        };
        var UpdateSelected = function () {
            var $inputs = $labels.find('input').not("#" + id + "HeaderCheckBox"),
                    $checked = $inputs.filter('[checked]'),
                    value = '',
                    numChecked = $checked.length;

            if (numChecked === 0) {
                value = o.noneSelectedText;
            } else {
                if ($.isFunction(o.selectedText)) {
                    value = o.selectedText.call(this, numChecked, $inputs.length, $checked.get());
                } else {
                    value = o.selectedText.replace('#', numChecked).replace('#', $inputs.length);
                }
            }
            //$select.find('input').val(value);
            $footer.find('label').html(value);
            return value;
        };
        var BindingEvent = function () {
            // build header links
            if (o.showHeader) {
                $header.find('input').click(function (e) {
                    var $this = $(this);
                    if ($this.attr('checked') === 'checked') {
                        $options.find('label').not('.ui-nextreeview-state-disabled').find('input').attr('checked', 'checked');
                    }
                    else {
                        $options.find('label').not('.ui-nextreeview-state-disabled').find('input').removeAttr('checked');
                    }
                    UpdateSelected();
                });
            }
            //build footer link
            if (o.showFooter) {
                $footer.find('a').click(function (e) {

                    o.onUpdateResult.call(this);
                    e.preventDefault();
                });
            }
            //add handler event collapse
            $listCollapse.each(function () {
                $(this).click(function () {
                    if ($(this).hasClass('rtMinus')) {
                        //alert("aaa");
                        $(this).removeClass('rtMinus').addClass('rtPlus');
                    }
                    else {
                        $(this).removeClass('rtPlus').addClass('rtMinus');
                    }
                    $(this).parent().next().toggle('slow');
                });
            });
            // checkbox click event handle
            $listItem.each(function () {
                var $this = $(this);
                //handle group checkbox click event
                $this.prev().children().not('span.collapseClass').find('input').click(function () {
                    if ($(this).attr('checked') === 'checked') {
                        $(this).parent().parent().next().find('input:not(:disabled)').attr('checked', 'checked');
                        checkCheckAll();
                    }
                    else {
                        $(this).parent().parent().next().find('input:not(:disabled)').removeAttr('checked');
                        $("#" + id + "HeaderCheckBox").removeAttr('checked');
                    }
                    o.onCheck.call(this);
                    UpdateSelected();
                });
                //handler chilren checkbox event
                $this.find('input').click(function () {
                    if (typeof $(this).attr('checked') === 'undefined') {
                        $(this).removeAttr('checked');
                        if ($this.prev().find('input').attr('checked') === 'checked') {//true) {
                            $this.prev().find('input').removeAttr('checked');
                            $("#" + id + "HeaderCheckBox").removeAttr('checked');
                        }
                    }
                    if ($(this).attr('checked') === 'checked') {
                        $(this).attr('checked', 'checked');
                        if ($this.find('input:not(:disabled)').length === ($this.find('input[checked]').length)) {
                            $this.prev().find('input').attr('checked', 'checked');
                        }
                        checkCheckAll();
                    }
                    o.onCheck.call(this);
                    UpdateSelected();
                });
            });
            //handle collapse button
            $('#' + id + "CollapseButton").click(function () {
                if ($(this).hasClass("ui-nextreeview-icon-collapse")) {
                    $(this).removeClass("ui-nextreeview-icon-collapse").addClass("ui-nextreeview-icon-expand");
                    $options.find('ul.ui-nextreeview-ul-group:not(.ui-nextreeview-checkboxes):visible').hide('slow');
                    $options.find('span.collapseClass').removeClass('rtMinus').addClass('rtPlus'); ;
                    //$(this).text('Expande All');
                }
                else {
                    $options.find('ul.ui-nextreeview-ul-group:not(.ui-nextreeview-checkboxes):hidden').show('slow');
                    $options.find('span.collapseClass').removeClass('rtPlus').addClass('rtMinus');
                    $(this).removeClass("ui-nextreeview-icon-expand").addClass("ui-nextreeview-icon-collapse");
                }
            });
            //add mouse hover event
            $divHover.not('div.ui-nextreeview-widget-header label').bind({
                mouseenter: function () {
                    $divHover.removeClass('ui-nextreeview-state-hover');
                    $(this).addClass('ui-nextreeview-state-hover').find('input').focus();
                }
            });
        };
        //initial setup
        var InitialSetup = function () {
            if (o.isCheckAll === true) {
                $("#" + id + "HeaderCheckBox").attr('checked', 'checked');
            }
            //$("#" + id + "HeaderCheckBox").attr('checked', 'checked');
            UpdateSelected();
            $select.hide();
        };

        //Render All
        (function () {
            checkShouldShowGroupOrNo();
            html.push(RenderInterface());
            html.push('<div class="ui-nextreeview-options RadTreeView RadTreeView_Office2010Silver' + (o.shadow ? ' ui-nextreeview-shadow' : '') + ' ui-nextreeview-widget ui-nextreeview-widget-content" >'); //ui-nextreeview-corner-all" >');
            html.push(RenderHeader(id, o.checkAllText));
            html.push(RenderBody());
            html.push(RenderFooter());
            html.push('</div>');

            //add global variable
            $select = $select.after(html.join('')).next('div.ui-nextreeview');
            $options = $select.next('div.ui-nextreeview-options');
            $header = $options.find('div.ui-nextreeview-header');
            $body = $options.find('ul.ui-nextreeview-checkboxes');
            $footer = $options.find('div.ui-nextreeview-footer');
            $labels = $options.find('label').not('.ui-nextreeview-state-disabled').not('.ui-nextreeview-group-mark');
            $allLabel = $options.find('label');
            $listItem = $options.find('ul.ui-nextreeview-ul-group');
            $listCollapse = $options.find('span.collapseClass');
            $listGroupCheckBox = $options.find('label.ui-nextreeview-group-mark').find('input');
            $divHover = $options.find('div.ui-nextreeview-div');

            AddPropertyValue();
            BindingEvent();

            InitialSetup();
            // remove the original input element
            $original.remove();
        })();
        return $select;
    };
    // close each select when clicking on any other element/anywhere else on the page
    $(document).delegate('html', 'click', function (e, isInsideCall) {
        var $target = $(e.target);
        if (!$target.closest('div.ui-nextreeview-options').length && !$target.parent().hasClass('ui-nextreeview')) {
            $('div.ui-nextreeview-options').trigger('close', [true, true]);
        }
    });
    // default options
    $.fn.nexTreeview.defaults = {
        showHeader: true,
        showFooter: true,
        maxHeight: 175, /* max height of the checkbox container (scroll) in pixels */
        minWidth: 215, /* min width of the entire widget in pixels. setting to 'auto' will disable */
        checkAllText: 'Check All',
        unCheckAllText: 'Uncheck All',
        updateResult: 'Update Result Table',
        noneSelectedText: 'Total: <b>0</b> selected',
        selectedText: 'Total: <b>#</b> selected',
        selectedList: 0,
        position: 'bottom', /* top|middle|bottom */
        shadow: false,
        fadeSpeed: 100,
        disabled: false,
        state: 'open',
        multiple: true,
        isCheckAll: false,
        hasOptGroup: false,
        isRetrieveDataAfterClose: true,
        listData: [],
        buttons: [],
        onLeave: function () { },
        onCheck: function () { }, /* when an individual checkbox is clicked */
        onOpen: function () { }, /* when the select menu is opened */
        onCheckAll: function () { }, /* when the check all link is clicked */
        onUncheckAll: function () { }, /* when the uncheck all link is clicked */
        onOptgroupToggle: function () { }, /* when the optgroup heading is clicked */
        onUpdateResult: function () { }
    };

})(jQuery);
