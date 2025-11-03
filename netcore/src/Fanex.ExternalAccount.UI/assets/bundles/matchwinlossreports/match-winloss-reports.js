// --------------------------------------------------------------------------
// <copyright file="fanex.MultiSelect.js">
// Copyright (c). All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
'use strict';
(function ($) {

    //// Method expose for user use
    var fanexMultiSelectSearchableMethods =
        {
            createfanexMultiSelect: function (opts) {
                opts = $.extend({}, $.fn.fanexMultiSelectSearchable.defaults, opts);
                return this.each(function () {
                    return new fanex.MultiSelectSearchable(this, opts);
                });
            },
            getItems: function (checked) {
                var $me = $(this);
                if ($me.parent().length === 0) {
                    $me = $('#' + $me.attr('id'));
                }
                var checkedText = checked ? ':checked' : ':not(:checked)',
                    items = $me.next().find('input[data-is-group=false]:not(:disabled)' + checkedText),
                    itemLength = items.length,
                    values = [];
                for (var i = 0; i < itemLength; i++) {
                    values.push(items[i].value);
                }
                return values;
            },
            getSelectedTexts: function (checked) {
                var $me = $(this);
                if ($me.parent().length === 0) {
                    $me = $('#' + $me.attr('id'));
                }
                var checkedText = checked ? ':checked' : ':not(:checked)',
                    items = $me.next().find('input[data-is-group=false]' + checkedText),
                    itemLength = items.length,
                    values = [];
                for (var i = 0; i < itemLength; i++) {
                    values.push(items[i].title);
                }
                return values;
            },
            setCheckAll: function (check) {
                var $me = $(this);
                if ($me.parent().length === 0) {
                    $me = $('#' + $me.attr('id'));
                }
                var $headerCheckbox = $me.next().find("#" + $me.attr('id') + "HeaderCheckBox");
                $headerCheckbox.prop('checked', check).trigger('change');
            },
            setEnable: function (enable) {
                var $me = $(this);
                if ($me.parent().length === 0) {
                    $me = $('#' + $me.attr('id'));
                }
                if (enable) {
                    $me.removeClass("fanex-multiselect-state-disabled");
                } else {
                    $me.addClass("fanex-multiselect-state-disabled");
                }
            }
        };

    //// Plugin
    $.fn.fanexMultiSelectSearchable = function (options) {
        if (fanexMultiSelectSearchableMethods[options]) {
            return fanexMultiSelectSearchableMethods[options].apply(this, Array.prototype.slice.call(arguments, 1));
        }
        else if (typeof options === 'object' || !options) {
            return fanexMultiSelectSearchableMethods.createfanexMultiSelect.apply(this, arguments);
        }
        else {
            throw 'Method does not supported!';
        }
    };

    //// Default Options
    $.fn.fanexMultiSelectSearchable.defaults =
        {
            showHeader: true,
            showFooter: false,
            showTooltip: true,
            showFilter: false,
            maxHeight: 400,
            minWidth: 200,
            checkAllText: 'Check All',
            noItemText: 'No item!',
            noItemFoundText: 'No item found!',
            footerButtonText: 'Update Result Table',
            noneSelectedText: 'Select options',
            selectedText: '# selected',
            tooltipNoItem: 'There is no selected item.',
            position: 'bottom', /* top|middle|bottom */
            fadeSpeed: 100,
            disabled: false,
            state: 'closed',
            multiple: true,
            isCheckAll: false,
            filterPaddingRight: 13,
            filterPlaceHolder: 'Enter keywords',
            isRetrieveDataAfterClose: false,
            listData: [],
            useCustomCheckAllText: false,
            customCheckAllText: 'All Checked',
            showParentSelectedText: false,
            toggleWidth: 120,
            onLeave: function () { },
            onCheck: function () { },
            onOpen: function () { },
            onCheckAll: function () { },
            onFooterButtonClick: function () { }
        };

    var fanex = fanex || {};

    fanex.MultiSelectSearchable = function (multiSelect, options) {
        var me = this;
        me.options = options;
        me.id = multiSelect.id;
        me.$multiSelect = $(multiSelect);
        me.$original = $(multiSelect);

        me.initialize();

        return me.$multiSelect;
    };

    fanex.MultiSelectSearchable.prototype =
        {
            getjQueryInstance: function () {
                return this.$multiSelect;
            },
            initialize: function () {
                var me = this,
                    html = [],
                    options = me.options,
                    id = me.id;
                me.browser = fanex.MultiSelectSearchable.helper.detectBrowser();
                me.isOpenning = false;
                me.checkShowGroup();
                me.bodyId = 'fanex-multiselect-options-' + id;
                html.push(me.renderInterface(id, options.disabled, options.noneSelectedText));
                html.push('<div class="fanex-multiselect-options fanex-multiselect-treeview fanex-multiselect-shadow" >');
                if (options.listData.length > 0) {
                    html.push(me.renderHeader(id, options.checkAllText));
                    html.push(me.renderBody());
                }
                else {
                    html.push(me.renderHeaderNoItem(id, options.noItemText));
                }
                if (options.showFooter) {
                    html.push(me.renderFooter());
                }
                html.push('</div>');

                //add global variable
                me.$multiSelect = me.$multiSelect.after(html.join('')).next('a.fanex-multiselect');
                me.$options = me.$multiSelect.next('div.fanex-multiselect-options');
                me.$options.attr('id', me.bodyId);
                me.$options = $('#' + me.bodyId);
                me.$header = me.$options.find('div.custom-ui-widget-header');
                me.$body = me.$options.find('ul.first-group');
                me.$footer = me.$options.find('div.fanex-multiselect-footer');
                me.$labels = me.$options.find('label').not('.fanex-multiselect-group-mark');
                me.$allLabel = me.$options.find('label');
                me.$listItem = me.$options.find('ul.fanex-multiselect-ul-group');
                me.$listCollapse = me.$options.find('span.collapseClass');
                me.$listGroupCheckBox = me.$options.find('label.fanex-multiselect-group-mark input');
                me.$divHover = me.$options.find('div.fanex-multiselect-div');
                me.$mainParentGroup = me.$options.find('ul.first-group > li > div.fanex-multiselect-div');
                me.$collapseButton = me.$options.find('#' + id + "CollapseButton");
                me.$noDataText = me.$options.find('#no-data-text');

                me.$allCheckBoxInputs = me.$options.find('label')
                    .not('.fanex-multiselect-state-disabled')
                    .find('input');
                me.$allInputsExceptHeaderCheckBox = me.$labels.find('input').not("#" + id + "HeaderCheckBox");
                me.$multiSelectinput = me.$multiSelect.find('input');
                me.$headerCheckbox = me.$options.find("#" + id + "HeaderCheckBox");
                me.$checkedInput = me.$labels.find('input:checked');
                if (me.options.showFilter) {
                    me.$filterInput = me.$options.find('#fanex-multiselect-filter-input');
                    me.$filterDeleteButton = me.$options.find('#filter-delete-button');
                }

                me.addPropertyValue();
                me.registerEvents();

                if (options.isCheckAll === true) {
                    me.$headerCheckbox.prop('checked', true);
                }
                // open by default?
                if (options.state === 'open') {
                    me.$options.trigger('open', [false]);
                }

                // update the number of selected elements when the page initially loads, and use that as the defaultValue.  
                // necessary for form resets when options are pre-selected.

                me.$multiSelect.find('input')[0].defaultValue = me.updateSelected();
                me.updateCheckall();
                me.$options.hide();
                me.$original.remove();
            },
            renderInterface: function (id, disable, nonSelectedText) {
                var me = this,
                    $multiSelect = me.$multiSelect;
                if ($multiSelect.prev().is('div#' + me.bodyId)) {
                    $multiSelect.prev().remove();
                }
                if ($multiSelect.next().is('div#' + me.bodyId)) {
                    $multiSelect.next().remove();
                }

                var html = [];
                html.push('<a id="'
                    + id + '" class="fanex-multiselect fanex-multiselect-widget fanex-multiselect-state-default '
                    + (disable ? ' fanex-multiselect-state-disabled' : '') + '">');
                html.push('<input readonly="readonly" type="text" class="fanex-multiselect-state-default" value="'
                    + nonSelectedText
                    + '" /><span class="fanex-multiselect-icon fanex-multiselect-icon-triangle-1-s"></span></a>');
                return html.join('');
            },
            checkShowGroup: function () {
                var me = this;
                var options = this.options;
                for (var i = 0; i < options.listData.length; i++) {
                    if (options.listData[i].items && options.listData[i].items.length > 0) {
                        me.isShowGroup = true;
                        break;
                    }
                }
            },
            renderHeader: function (id, checkAllText) {
                var me = this,
                    options = me.options,
                    html = [];
                if (options.showFilter) {
                    html.push('<div class="fanex-multiselect-filter">');
                    html.push('<lable for="fanex-multiselect-filter-input">');
                    html.push('<input type="textbox" id="fanex-multiselect-filter-input" placeholder="'
                        + options.filterPlaceHolder + '"/></label>');
                    html.push('<span class="delete-button" id="filter-delete-button" title="Clear"></span>');
                    html.push('</div>');

                    html.push('<div id="no-data-text" class="no-data-text" style="display:none;">' + options.noItemFoundText + '</div>');
                }

                html.push('<div class="custom-ui-widget-header" ' + (options.showHeader ? '' : 'style="display:none;"') + '>');
                html.push('<ul class="fanex-multiselect-helper-reset" style="overflow-y:hidden;">');
                html.push('<li style="display:inline;" class="show-item">');
                html.push('<label class="fanex-multiselect-check-all" for="' + id + "HeaderCheckBox" + '">');
                html.push('<input type="checkbox" id="' + id + "HeaderCheckBox" + '">' + checkAllText + '</label></li>');
                if (me.isShowGroup) {
                    html.push('<li class="fanex-multiselect-icon-dropdown fanex-multiselect-icon-collapse" title="Collapse/Expand" id="'
                        + id + "CollapseButton" + '" >&nbsp;</li>');
                }

                html.push('</ul>');
                html.push('</div>');
                return html.join('');
            },
            renderHeaderNoItem: function (id, noItemText) {
                var html = [];
                html.push('<div class="fanex-multiselect-widget-header fanex-multiselect-helper-clearfix \
                      fanex-multiselect-corner-all fanex-multiselect-header custom-ui-widget-header" style="min-width:' + this.options.minWidth + 'px;">');
                html.push('<ul class="fanex-multiselect-helper-reset fanex-multiselect-checkboxes-head" style="overflow-y:hidden;">');
                html.push('<li style="display:inline;">');
                html.push('<label class="fanex-multiselect-corner-all">' + noItemText + '</label>');
                html.push('</ul>');
                html.push('</div>');
                return html.join('');
            },
            renderBody: function () {
                var me = this,
                    options = me.options,
                    html = [],
                    groupIdAttr = "fanex-multiselect-option-group-0";
                me.numGroup = 0;
                html.push('<ul class="first-group rtUL '
                    + (me.isShowGroup ? "rtLines" : "")
                    + ' fanex-multiselect-ul-group" childgroupid="' + groupIdAttr + '">');
                for (var i = 0; i < options.listData.length; i++) {
                    var tempGroup = options.listData[i];
                    me.numGroup++;

                    html.push(me.renderRow({
                        data: tempGroup,
                        index: i,
                        hasChild: tempGroup.items && tempGroup.items.length > 0,
                        isLastChild: i === options.listData.length - 1,
                        parentGroupId: groupIdAttr,
                        groupId: "0-" + i
                    }));
                }
                html.push('</ul>');
                return html.join('');
            },
            //renderRow: function (tempData, index, hasChild, isLastChild, currentParentGroupId, currentGroupId) {
            renderRow: function (row) {
                row.data.id = row.data.id || '';
                var me = this,
                    options = me.options,
                    tempHtml = [],
                    title = row.data.text || '',
                    value = row.data.value || '',
                    inputId = 'fanex-multiselect-' + me.id + '-' + row.data.id + '-option-' + row.groupId,
                    isDisabled = !row.data.enable,
                    labelClasses = row.data.cssClass || '',
                    liClasses = 'rtLI show-item ',
                    divClasses = 'fanex-multiselect-div ',
                    groupIdAttr = 'fanex-multiselect-option-group-' + row.groupId;


                row.hasChild = row.hasChild || false;

                if (value) {
                    liClasses += (isDisabled ? ' fanex-multiselect-disabled ' : '')
                        + (row.isLastChild ? ' rtLast' : '')
                        + (row.index === 0 && row.hasChild ? ' rtFirst' : '');

                    //if (!me.isShowGroup && options.columnNumber > 0) {
                    //    var liStyle = 'float: left; width:' + (100 / options.columnNumber).toFixed(0) + '%;';
                    //    tempHtml.push('<li class="' + liClasses + '" style="' + liStyle + '">');
                    //}
                    //else {
                    tempHtml.push('<li class="' + liClasses + '" data-content-text="' + title.toLowerCase() + '">');
                    //}

                    if (me.isShowGroup) {
                        divClasses += row.isLastChild ? ' rtBot' : (row.index === 0 ? ' rtMid rtSelected' : ' rtMid');
                        tempHtml.push('<div class="' + divClasses + '">');
                        if (row.data.items && row.data.items.length > 0) {
                            tempHtml.push('<span class="collapseClass rtMinus" data-group-show="' + (row.data.show !== false) + '"></span>');
                        }
                    }
                    else {
                        if (!row.hasChild && me.isShowGroup) {
                            divClasses += !me.isShowGroup ? "" : (row.isLastChild ? ' rtBot' : ' rtMid');
                            tempHtml.push('<div class="' + divClasses + '">');
                        }
                        else {
                            tempHtml.push('<div class="' + divClasses + '">');
                        }
                    }

                    labelClasses += isDisabled ? ' fanex-multiselect-state-disabled' : '';
                    labelClasses += row.hasChild && me.isShowGroup ? ' fanex-multiselect-group-mark' : '';
                    tempHtml.push('<label for="' + inputId + '" class="' + labelClasses + '" style="' + row.data.cssClass + '">');
                    tempHtml.push('<input id="' + inputId
                        + '" type="' + (options.multiple ? 'checkbox' : 'radio')
                        + '" name="' + value
                        + '" value="' + value
                        + '" class="show-item'
                        + '" title="' + title
                        + '" childgroupid="' + row.parentGroupId
                        + '" data-is-group="' + row.hasChild
                        + '"');
                    tempHtml.push(options.isCheckAll ? ' checked="checked"' : '');
                    tempHtml.push(row.data.checked ? ' checked="checked"' : '');
                    tempHtml.push(isDisabled ? ' disabled="disabled"' : '');
                    tempHtml.push(row.data.id !== '' ? ' data-item-id="' + row.data.id + '"' : '');
                    tempHtml.push(' />' + title + '</label></div>');

                    if (row.hasChild) {
                        tempHtml.push('<ul class="fanex-multiselect-ul-group custom-multi-select-rtUL" childgroupid="' + groupIdAttr + '">');
                        var currentChildLength = row.data.items.length;
                        for (var j = 0; j < currentChildLength; j++) {
                            var currentChildItem = row.data.items[j],
                                currentIsLastChild = j === row.data.items.length - 1,
                                currentRow = {
                                    data: currentChildItem,
                                    index: j,
                                    hasChild: currentChildItem.items && currentChildItem.items.length > 0,
                                    isLastChild: currentIsLastChild,
                                    parentGroupId: groupIdAttr,
                                    groupId: row.groupId + '-' + j
                                };

                            tempHtml.push(me.renderRow(currentRow));
                        }
                        tempHtml.push("</ul>");
                    }

                    tempHtml.push('</li>');
                }
                return tempHtml.join('');
            },
            renderFooter: function () {
                var options = this.options,
                    html = [];
                html.push('<div class="fanex-multiselect-footer custom-ui-widget-footer">');
                html.push('<a class="fanex-multiselect-update-result" href="">' + options.footerButtonText + '</a>');
                html.push('</div>');
                return html.join('');
            },
            addPropertyValue: function () {
                var me = this,
                    iconWidth = me.$multiSelect.find('span.fanex-multiselect-icon').width(),
                    inputWidth = me.options.toggleWidth - Math.max(iconWidth, 0) + 3;
                // set widths
                me.$multiSelect.css('width', me.options.minWidth + 'px')
                    .find('input').css('width', inputWidth + 'px');
                if (me.options.showFilter) {
                    me.$filterInput.css('width', me.options.minWidth + 1 + 'px');
                }
            },
            updateCheckall: function () {
                var me = this;
                if (me.numGroup === me.$options.find('input[childgroupid="fanex-multiselect-option-group-0"]').filter(':checked').length) {
                    me.$headerCheckbox.prop('checked', true);
                    me.$listGroupCheckBox.prop('checked', true);
                }
            },
            updateSelected: function () {
                var me = this,
                    options = me.options,
                    $inputs = me.$allInputsExceptHeaderCheckBox.not(":disabled"),
                    $checked = $inputs.filter(':checked'),
                    value = '',
                    numChecked = $checked.length;
                if (numChecked === 0) {
                    value = options.noneSelectedText;
                } else {
                    var allChecked = $inputs.length === numChecked;
                    allChecked && options.onCheckAll();

                    if (options.useCustomCheckAllText && allChecked) {
                        if (typeof options.customCheckAllText === 'function') {
                            value = options.customCheckAllText();
                        } else {
                            value = options.customCheckAllText + " (" + numChecked + ")";
                        }
                    }
                    else {
                        if (options.showParentSelectedText) {
                            $checked = me.$mainParentGroup.find('input:checked');
                            numChecked = $checked.length;
                        }

                        if (numChecked > 0) {
                            var arrayItemChecked = [];
                            for (var i = 0; i < numChecked; i++) {
                                arrayItemChecked.push($checked[i].title);
                            }
                            value = arrayItemChecked.join(', ');
                            if (value.length * 8 > options.minWidth) {
                                value = options.selectedText.replace('#', numChecked);
                            }
                        }
                        else {
                            value = options.noneSelectedText;
                        }
                    }
                }

                // Update tooltip
                if (options.showTooltip) {
                    var tooltipValue = $checked.map(function () { return this.title; }).get().join(', ');
                    if (tooltipValue === '') {
                        tooltipValue = options.tooltipNoItem;
                    }
                    me.$multiSelect.attr("original-title", tooltipValue);
                    me.$multiSelect.attr("title", tooltipValue);
                }

                me.$multiSelect.find('input').val(value);
                return value;
            },
            updateParentSelected: function (currentGroup) {
                var me = this,
                    parentGroup = currentGroup.parent().parent();
                if (parentGroup.prop("tagName").toLowerCase() === 'ul' && parentGroup.length > 0) {
                    var currentChildren = parentGroup.find("li.show-item > div > label > input.show-item"),
                        currentChildSelected = currentChildren.filter(':checked').length;
                    if (currentChildren.length === currentChildSelected) {
                        parentGroup.prev().find('input').prop('checked', true);
                    }
                    me.updateParentSelected(parentGroup);
                }
            },
            removeParentSelected: function (currentGroup) {
                var me = this;
                if (currentGroup.hasClass('fanex-multiselect-ul-group') && currentGroup.length > 0) {
                    if (currentGroup.prev().find('input').prop('checked')) {
                        currentGroup.prev().find('input').prop('checked', false);
                        me.$headerCheckbox.prop('checked', false);
                    }
                    me.removeParentSelected(currentGroup.parent().parent());
                }
            },
            registerEvents: function () {
                var me = this,
                    options = me.options;
                // build header links
                me.$headerCheckbox.change(function () {
                    me.$allCheckBoxInputs.filter('.show-item').prop('checked', this.checked);
                });
                //build footer link
                if (options.showFooter) {
                    me.$footer.find('a').click(function (e) {
                        options.onFooterButtonClick.call(this);
                        me.$options.fadeOut(options.fadeSpeed);
                        e.preventDefault();
                    });
                }

                //if (options.showTooltip) {
                //    //try {
                //    //    me.$multiSelect.tipsy(
                //    //        {
                //    //            html: true,
                //    //            opacity: 0.9,
                //    //            gravity: 'sw'
                //    //        }
                //    //    );

                //    //}
                //    //catch (e) { };
                //}

                if (options.showFilter) {
                    me.$filterInput
                        .off('keyup.multipleselect')
                        .on('keyup.multipleselect', function () {
                            var $this = $(this);
                            var filterVal = $this.val().toLowerCase();
                            if (filterVal !== '') {
                                me.$listItem
                                    .find('li').removeAttr('data-dont-hide-me')
                                    .removeClass('show-item')
                                    .find('div').removeClass('high-light-item')
                                    .find('input').removeClass('show-item');

                                var dontHideElements = me.$listItem.find('li[data-content-text*="' + filterVal + '"]')
                                    .removeClass('hide-item')
                                    .addClass('show-item')
                                    .attr('data-dont-hide-me', 'true');

                                dontHideElements.find('input').addClass('show-item');
                                if (dontHideElements.length > 0) {
                                    me.$noDataText.hide();
                                    me.$header.show();
                                    dontHideElements.each(function () {
                                        var dontHideElement = $(this);
                                        dontHideElement.children().first().addClass('high-light-item');

                                        var prevParent = dontHideElement.parent().prev();
                                        var grandParent = dontHideElement.parent().parent();

                                        while (grandParent.hasClass('rtLI')) {
                                            grandParent.removeClass('hide-item')
                                                .addClass('show-item').attr('data-dont-hide-me', 'true');
                                            prevParent.find('input').addClass('show-item');
                                            prevParent = grandParent.parent().prev();
                                            grandParent = grandParent.parent().parent();

                                        }

                                        dontHideElement
                                            .find('li.rtLI').removeClass('hide-item').addClass('show-item').attr('data-dont-hide-me', 'true')
                                            .find('input').addClass('show-item');
                                    });
                                }
                                else {
                                    me.$header.hide();
                                    me.$noDataText.show();
                                }

                                me.$listItem.find('li[data-dont-hide-me!="true"]').addClass('hide-item');
                                //updateParentSelected
                            }
                            else {
                                me.$noDataText.hide();
                                me.$header.show();
                                me.$listItem.find('li').removeAttr('data-dont-hide-me')
                                    .removeClass('hide-item')
                                    .addClass('show-item')
                                    .find('div').removeClass('high-light-item')
                                    .find('input').addClass('show-item');
                            }

                            me.$listItem.find('input.show-item:checked').each(function () {
                                var inputShowItem = $(this);
                                var grantParent = inputShowItem.parent().parent();
                                if (grantParent.next().find('li').length === 0) {
                                    //me.updateParentSelected(grantParent.parent().parent());
                                    me.updateParentSelected(grantParent);
                                }
                            });
                            me.$listItem.find('input.show-item:not(:checked)').each(function () {
                                me.removeParentSelected($(this).parent().parent().parent().parent());
                            });
                        });

                    me.$filterDeleteButton
                        .off('click.multipleselect')
                        .on('click.multipleselect', function () {
                            if (me.$filterInput.val() !== '') {
                                me.$filterInput.val('');
                                me.$filterInput.trigger('keyup.multipleselect');
                            }
                        });
                }
                //add handler event collapse
                me.$listCollapse.each(function () {
                    var $this = $(this);
                    $this.click(function () {
                        if ($this.hasClass('rtMinus')) {
                            $this.removeClass('rtMinus').addClass('rtPlus');
                        }
                        else {
                            $this.removeClass('rtPlus').addClass('rtMinus');
                        }
                        $this.parent().next().toggle();
                    });

                    var currentStatus = $this.attr('data-group-show');
                    if (currentStatus === 'false') {
                        $this.removeClass('rtMinus').addClass('rtPlus');
                        $this.parent().next().hide();
                    }
                });
                // the select box events
                me.$multiSelect.bind({
                    click: function () {
                        me.$options.trigger('toggle');
                    },
                    keypress: function () {
                        //                    switch (e.keyCode) {
                        //                        case 27: // esc
                        //                        case 38: // up
                        //                            me.$options.trigger('close');
                        //                            break;
                        //                        case 40: // down
                        //                        case 0: // space
                        //                            me.$options.trigger('toggle');
                        //                            break;
                        //                    }
                    },
                    mouseenter: function () {
                        if (!me.$multiSelect.hasClass('fanex-multiselect-state-disabled')) {
                            $(this).addClass('fanex-multiselect-state-hover');
                        }

                    },
                    mouseleave: function () {
                        $(this).removeClass('fanex-multiselect-state-hover');
                    },
                    focus: function () {
                        if (!me.$multiSelect.hasClass('fanex-multiselect-state-disabled')) {
                            $(this).addClass('fanex-multiselect-state-focus');
                        }
                    },
                    blur: function () {
                        $(this).removeClass('fanex-multiselect-state-focus');
                    }
                });
                // bind custom events to the options div
                me.$options.bind({
                    'close': function (e, others, isRetrieveData, targetId) {
                        if (me.$options.css('display') !== 'none') {
                            others = others || false;

                            // hides all other options but the one clicked
                            if (others) {
                                $('div.fanex-multiselect-options')
                                    .filter(':visible')
                                    .fadeOut(options.fadeSpeed)
                                    .prev('a.fanex-multiselect')
                                    .removeClass('fanex-multiselect-state-active')
                                    .trigger('mouseout');

                                // hides the clicked options
                            } else {
                                me.$multiSelect.removeClass('fanex-multiselect-state-active').trigger('mouseout');
                                me.$options.fadeOut(options.fadeSpeed);
                            }
                            if (options.isRetrieveDataAfterClose) {
                                isRetrieveData = isRetrieveData || false;
                                if (isRetrieveData) {
                                    me.$allLabel.find('input[ischeck="true"]').prop('checked', true);
                                    if (me.isOpenning) {
                                        me.$allLabel.find('input[ischeck!="true"]').prop('checked', false);
                                    }
                                }
                                me.$allLabel.find('input').removeAttr('isCheck');
                                me.isOpenning = false;

                            }
                            me.updateSelected();
                            //$.fn.multiSelect.defaults.onLeave();
                            if (targetId &&
                                (targetId !== me.$multiSelect.attr("id") || targetId === 'html') && me.isOpenning) {
                                options.onLeave.call(me.$multiSelect);
                                me.isOpenning = false;
                            }
                        }
                    },
                    'open': function (e, closeOthers) {

                        // bail if this widget is disabled
                        if (me.$multiSelect.hasClass('fanex-multiselect-state-disabled')) {
                            return;
                        }

                        //currentTriggerEvents.push(e.)

                        // use position() if inside ui-widget-content, because offset() won't cut it.
                        var offset = me.$multiSelect.position(),
                            $container = me.$options.find('ul:last'),
                            top, width;

                        // calling select is active
                        me.$multiSelect.addClass('fanex-multiselect-state-active');

                        // hide all other options
                        if (closeOthers || typeof closeOthers === 'undefined') {
                            //$options.trigger('close', [true, true]);
                            $('div.fanex-multiselect-options').trigger('close', [true, true, 'html']);
                        }

                        // calculate positioning
                        if (options.position === 'middle') {
                            top = (offset.top + (me.$multiSelect.height() / 2) - (me.$options.outerHeight() / 2));
                        } else if (options.position === 'top') {
                            top = (offset.top - me.$options.outerHeight()) - 7;
                        } else {
                            top = (offset.top + me.$multiSelect.outerHeight());
                        }
                        top += 3; // padding extra 3px to seperate options with multi select div 

                        // show the options div + position it
                        me.$options.css({
                            position: 'absolute',
                            top: top + 'px',
                            left: offset.left + 'px',
                            width: me.options.minWidth + 'px'
                        }).show();

                        // set the scroll of the checkbox container
                        $container.scrollTop(0);

                        // set the height of the checkbox container

                        if (options.maxHeight) {
                            if (options.maxHeight <= me.$body.height()) {
                                me.$body.css('max-height', options.maxHeight);
                            }
                        }
                        if (options.isRetrieveDataAfterClose) {
                            me.$allLabel.find('input').filter(':checked').attr("ischeck", "true");

                        }
                        me.isOpenning = true;
                        options.onOpen.call();
                    },
                    'toggle': function () {
                        var isHide = $(this).is(':hidden');
                        if (isHide) {
                            me.$options.trigger('open');
                        }
                        else {
                            me.$options.trigger('close', [true, true]);
                        }
                    }
                });
                // checkbox click event handle
                me.$listItem.each(function () {
                    var $this = $(this);

                    //handler children checkbox event
                    var childrenNodes = $this.find('li.show-item input[childgroupid="' + $this.attr("childgroupid") + '"]');
                    childrenNodes.change(function () {
                        var parent = $this,
                            current = $(this);
                        if (!this.checked) {
                            me.removeParentSelected(parent);
                        }
                        else if (this.checked) {
                            var currentGroup = parent,
                                currentChildSelected = childrenNodes.filter('.show-item:not(:disabled)').filter(':checked').length,
                                currentChildLength = childrenNodes.filter('.show-item:not(:disabled)').length;
                            if (currentChildLength === currentChildSelected) {
                                currentGroup.prev().find('input').prop('checked', true);
                                me.updateParentSelected(current.parent().parent().parent().parent());
                            }

                            me.updateCheckall();
                        }
                        if (current.attr('data-is-group') === 'false') {
                            options.onCheck.call(this);
                        }
                        me.updateSelected();
                    });

                    //handle group checkbox click event
                    $this.prev().children().not('span.collapseClass').find('input').change(function () {
                        if (this.checked) {
                            $(this).parent().parent().next().find('li.show-item input.show-item:not(:disabled)').prop('checked', true);
                            me.updateParentSelected($this);
                            me.updateCheckall();
                        }
                        else {
                            $(this).parent().parent().next().find('li.show-item input.show-item:not(:disabled)').prop('checked', false);
                            me.$headerCheckbox.prop('checked', false);
                        }
                        options.onCheck.call(this);
                        me.updateSelected();
                    });
                });
                //handle collapse button
                me.$collapseButton.click(function () {
                    var $this = $(this);
                    if ($this.hasClass("fanex-multiselect-icon-collapse")) {
                        $this.removeClass("fanex-multiselect-icon-collapse").addClass("fanex-multiselect-icon-expand");
                        me.$options.find('ul.fanex-multiselect-ul-group:not(.first-group):visible').hide();
                        me.$options.find('span.collapseClass').removeClass('rtMinus').addClass('rtPlus');
                    }
                    else {
                        me.$options.find('ul.fanex-multiselect-ul-group:not(.first-group):hidden').show();
                        me.$options.find('span.collapseClass').removeClass('rtPlus').addClass('rtMinus');
                        $this.removeClass("fanex-multiselect-icon-expand").addClass("fanex-multiselect-icon-collapse");
                    }
                });

                //// Register windows delegate event for closing milti select
                $(document).delegate('html', 'click', function (e) {
                    var $target = $(e.target);
                    if (!$target.closest('div.fanex-multiselect-options').length && !$target.parent().hasClass('fanex-multiselect')) {
                        if (!$target.hasClass('exeption-multiselect-click')) {
                            $('div.fanex-multiselect-options').trigger('close', [true, true, ($target.attr("id") || 'html')]);
                        }
                    }
                });
            }
        };

    //// multiSelect Helper
    fanex.MultiSelectSearchable.helper =
        {
            detectBrowser: function () {
                //// multiSelect Scroller
                var matchedBrowser, browserDectect;

                // Use of jQuery.browser is frowned upon.
                // More details: http://api.jquery.com/jQuery.browser
                // jQuery.uaMatch maintained for back-compat
                function ubMatch(ua) {
                    ua = ua.toLowerCase();

                    var match = /(chrome)[ \/]([\w.]+)/.exec(ua) ||
                        /(webkit)[ \/]([\w.]+)/.exec(ua) ||
                        /(opera)(?:.*version|)[ \/]([\w.]+)/.exec(ua) ||
                        /(msie) ([\w.]+)/.exec(ua) ||
                        ua.indexOf("compatible") < 0 && /(mozilla)(?:.*? rv:([\w.]+)|)/.exec(ua) ||
                        [];

                    return {
                        browserDectect: match[1] || "",
                        version: match[2] || "0"
                    };
                }

                matchedBrowser = ubMatch(navigator.userAgent);
                browserDectect = {};

                if (matchedBrowser.browserDectect) {
                    browserDectect[matchedBrowser.browserDectect] = true;
                    browserDectect.version = matchedBrowser.version;
                }

                // Chrome is Webkit, but Webkit is also Safari.
                if (browserDectect.chrome) {
                    browserDectect.webkit = true;
                } else if (browserDectect.webkit) {
                    browserDectect.safari = true;
                }

                return browserDectect;
            }
        };

    fanex.MultiSelectSearchable.list = fanex.MultiSelectSearchable.list || {};

    fanex.MultiSelectSearchable.constant = fanex.MultiSelectSearchable.constant || {};


})(jQuery);

var BaseReportFilters = /** @class */ (function () {
    function BaseReportFilters($) {
        this.inputIds = [];
        this.dropdowns = [];
        this.$ = $;
    }
    BaseReportFilters.prototype.registerInputComponent = function (inputInfo) {
        if (!this.inputIds.filter(function (id) { return id.inputId === inputInfo.inputId; }).length) {
            this.inputIds.push(inputInfo);
            return true;
        }
        return false;
    };
    BaseReportFilters.prototype.registerDropdownComponent = function (dropdownInfo) {
        var isExist = this.dropdowns.filter(function (id) { return id.dropdownId === dropdownInfo.dropdownId; }).length;
        switch (dropdownInfo.dropdownType) {
            case "fanexMultiSelect":
                this.renderFanexMultiSelectDDL(dropdownInfo);
                !isExist && this.dropdowns.push(dropdownInfo);
                return true;
            case "fanexMultiSelectSearchable":
                this.renderFanexMultiSelectSearchableDDL(dropdownInfo);
                !isExist && this.dropdowns.push(dropdownInfo);
                return true;
            case "combobox":
                this.renderComboboxDDL(dropdownInfo);
                !isExist && this.dropdowns.push(dropdownInfo);
                return true;
            case "select2":
                this.renderSelect2DDL(dropdownInfo);
                !isExist && this.dropdowns.push(dropdownInfo);
                return true;
            default:
                throw new Error("Unknown dropdown type");
        }
    };
    BaseReportFilters.prototype.getAllFilterValues = function () {
        var self = this;
        var inputParams = this.inputIds.map(function (elem) {
            return {
                name: elem.inputName,
                currentValue: self.$("#" + elem.inputId).val(),
            };
        });
        var getSelectedValueDropdown = function (dropdownInfo) {
            switch (dropdownInfo.dropdownType) {
                case "fanexMultiSelect":
                    return self.$("#" + dropdownInfo.dropdownId).fanexMultiSelect("getItems", true).toString();
                case "fanexMultiSelectSearchable":
                    return self.$("#" + dropdownInfo.dropdownId).fanexMultiSelectSearchable("getItems", true).toString();
                case "combobox":
                case "select2":
                    return self.$("#" + dropdownInfo.dropdownId).find(":selected").val();
                default:
                    throw new Error("Unknown dropdown type");
            }
        };
        var getSelectedTextDropdown = function (dropdownInfo) {
            switch (dropdownInfo.dropdownType) {
                case "fanexMultiSelect":
                    return self.$("#" + dropdownInfo.dropdownId).fanexMultiSelect("getSelectedTexts", true).toString();
                case "fanexMultiSelectSearchable":
                    return self.$("#" + dropdownInfo.dropdownId).fanexMultiSelectSearchable("getSelectedTexts", true).toString();
                case "combobox":
                case "select2":
                    return self.$("#" + dropdownInfo.dropdownId).find(":selected").text();
                default:
                    throw new Error("Unknown dropdown type");
            }
        };
        var dropdownParams = this.dropdowns.map(function (elem) {
            return {
                name: elem.dropdownName,
                currentValue: getSelectedValueDropdown(elem),
                currentText: getSelectedTextDropdown(elem),
            };
        });
        return {
            inputParams: inputParams,
            dropdownParams: dropdownParams,
            calendarParams: this.getCalendarValues(),
        };
    };
    BaseReportFilters.prototype.isUnValidInputs = function () {
        var inputParams = this.getAllFilterValues().inputParams;
        return inputParams.some(function (e) { return e.currentValue.includes("script"); });
    };
    BaseReportFilters.prototype.getCalendarValues = function () {
        return { fromDate: this.fromDate, toDate: this.toDate };
    };
    BaseReportFilters.prototype.renderCalendarPicker = function (options) {
        throw new Error("Method not implemented.");
    };
    BaseReportFilters.prototype.renderSingleDatePicker = function (options) {
        throw new Error("Method not implemented.");
    };
    BaseReportFilters.prototype.renderFanexMultiSelectDDL = function (options) {
        if (!options.dropdownData || !options.dropdownId)
            return;
        var dropdownConfig = options.dropdownConfig;
        var _a = dropdownConfig || { checkAllText: "Select All", noneSelectedText: "None Selected" }, checkAllText = _a.checkAllText, noneSelectedText = _a.noneSelectedText;
        return this.$("#" + options.dropdownId).fanexMultiSelect({
            showFilter: true,
            checkAllText: checkAllText,
            noneSelectedText: noneSelectedText,
            useCustomCheckAllText: false,
            customCheckAllText: "All",
            isCheckAll: true,
            listData: options.dropdownData,
            minWidth: options.minWidth,
            toggleWidth: options.toggleWidth,
            onLeave: function () {
                var leagueIDs = $("#" + options.dropdownId).fanexMultiSelect("getItems", true).toString();
                if (options.onClosed) {
                    options.onClosed(leagueIDs);
                }
            },
        });
    };
    BaseReportFilters.prototype.renderFanexMultiSelectSearchableDDL = function (options) {
        if (!options.dropdownData || !options.dropdownId)
            return;
        var dropdownConfig = options.dropdownConfig;
        var _a = dropdownConfig || { checkAllText: "Select All", noneSelectedText: "None Selected" }, checkAllText = _a.checkAllText, noneSelectedText = _a.noneSelectedText;
        return this.$("#" + options.dropdownId).fanexMultiSelectSearchable({
            showFilter: true,
            checkAllText: checkAllText,
            noneSelectedText: noneSelectedText,
            useCustomCheckAllText: false,
            customCheckAllText: "All",
            isCheckAll: true,
            listData: options.dropdownData,
            minWidth: options.minWidth,
            toggleWidth: options.toggleWidth,
            onLeave: function () {
                var leagueIDs = $("#" + options.dropdownId).fanexMultiSelectSearchable("getItems", true).toString();
                if (options.onClosed) {
                    options.onClosed(leagueIDs);
                }
            },
        });
    };
    BaseReportFilters.prototype.renderComboboxDDL = function (options) {
        var _this = this;
        if (!options.dropdownId)
            return;
        var $dropdownList = this.$("#" + options.dropdownId).combobox();
        $dropdownList.next("input.ui-autocomplete-input")
            .val($dropdownList.find("option:selected").text())
            .attr("readonly", "true")
            .bind("click", function (event) { return _this.$(event.currentTarget).autocomplete("search", ""); });
    };
    BaseReportFilters.prototype.renderSelect2DDL = function (options) {
        var _this = this;
        if (!options.dropdownId)
            return;
        this.$("#" + options.dropdownId).empty().select2({
            placeholder: "Select",
            data: options.dropdownData,
            dropdownAutoWidth: true,
            templateSelection: function (data) {
                return data.text; // Customize how selected items are shown
            },
            sorter: function (data) {
                return data; // Keep original order, no sorting
            }
        }).bind("select2:select", function () {
            if (options.onClosed) {
                options.onClosed(_this.$("#" + options.dropdownId).val());
            }
        });
    };
    return BaseReportFilters;
}());
//# sourceMappingURL=ReportFilters.js.map
var defaultOptions = {
    reportId: "",
    gridId: "",
    gridContainerId: "",
    selectColumnsId: "",
    selectColumnsContainerId: "",
    msgNoInfoId: "",
    isMobile: { phone: false },
    excelBtnId: "",
    pageSizeOptions: [10, 20, 50]
};
var BaseReportGrid = /** @class */ (function () {
    function BaseReportGrid(options) {
        var _this = this;
        this.options = defaultOptions;
        this.gridBodyRowsCache = [];
        this.gridFooterRowsCache = [];
        this.options = options;
        Fanex.ExtAcc.OnResize = function () {
            try {
                if (_this.gridOptions) {
                    $("#" + _this.options.gridId).fanexGrid("resizeGrid");
                }
            }
            catch (err) {
                console.log(err);
            }
        };
    }
    BaseReportGrid.prototype.initSelectColumnsDropdown = function (columns, ignoreColumns) {
        var _this = this;
        var options = [];
        var numberOfColumns = columns.length;
        for (var i = 0; i < numberOfColumns; i++) {
            if (ignoreColumns.includes(i))
                continue;
            options.push({
                id: i.toString(),
                value: i.toString(),
                text: columns[i].ColumnName2,
                checked: true,
                enable: true
            });
        }
        $("#" + this.options.selectColumnsId).fanexMultiSelectSearchable({
            showFilter: true,
            checkAllText: "Select All",
            noneSelectedText: "None Selected",
            useCustomCheckAllText: true,
            customCheckAllText: "All",
            isCheckAll: true,
            listData: options,
            onLeave: function () { return _this.showHideColumns(); },
            toggleWidth: this.options.isMobile.phone ? 243 : 193,
            minWidth: this.options.isMobile.phone ? 250 : 200
        });
    };
    BaseReportGrid.prototype.showHideColumns = function () {
        var unSelectedColumns = $("#" + this.options.selectColumnsId).fanexMultiSelectSearchable("getItems", false);
        $("#" + this.options.gridId).fanexGrid("hideColumns", this.gridOptions, unSelectedColumns.map(function (colIndex) { return Number(colIndex); }));
    };
    BaseReportGrid.prototype.buildBodyRows = function (reportData) {
        return undefined;
    };
    BaseReportGrid.prototype.buildFooterRows = function (reportData) {
        return undefined;
    };
    BaseReportGrid.prototype.getDefaultColumns = function () {
        return undefined;
    };
    BaseReportGrid.prototype.getGridOptions = function () {
        return this.gridOptions;
    };
    BaseReportGrid.prototype.setGridOptions = function (gridOptions) {
        this.gridOptions = gridOptions;
    };
    BaseReportGrid.prototype.getGroupHeaderColumns = function () {
        return undefined;
    };
    BaseReportGrid.prototype.hideNoInfo = function (isHide) {
        var msgNoInfo = $("#" + this.options.msgNoInfoId);
        var gridContainer = $("#" + this.options.gridContainerId);
        var selectColumnsContainer = $("#" + this.options.selectColumnsContainerId);
        if (isHide) {
            msgNoInfo.hide();
            gridContainer.show();
            selectColumnsContainer.show();
        }
        else {
            msgNoInfo.show();
            gridContainer.hide();
            selectColumnsContainer.hide();
        }
    };
    BaseReportGrid.prototype.getDateTime = function (date) {
        if (!date)
            return "";
        return moment(date).format("MM/DD/YYYY hh:mm A");
    };
    BaseReportGrid.prototype.getDate = function (date) {
        if (!date)
            return "";
        return moment(date).format("MM/DD/YYYY");
    };
    BaseReportGrid.prototype.fanexGrid = function (gridOptions) {
        $("#" + this.options.gridId).fanexGrid(gridOptions);
        if (this.options.copyBtnId) {
            this.initCopyBtn(this.options.copyBtnId);
        }
    };
    BaseReportGrid.prototype.initCopyBtn = function (copyBtnId) {
        var defaultColumns = this.getDefaultColumns();
        var groupHeaderColumns = this.getGroupHeaderColumns();
        var _self = this;
        function getHidedColumnsInGroup(groupLength, startIndex) {
            if (groupLength === 0 || startIndex >= defaultColumns.length || startIndex < 0)
                return [];
            var hideColumns = $("#" + _self.options.selectColumnsId).fanexMultiSelect("getItems", false);
            var hideColumnsName = hideColumns.map(function (colIndex) { return defaultColumns[colIndex].Name; });
            var count = 0;
            var hidedColumnsInGroup = [];
            while (count < groupLength) {
                var columnName = defaultColumns[startIndex + count].Name;
                if (hideColumnsName.includes(columnName)) {
                    hidedColumnsInGroup.push(columnName);
                }
                count++;
            }
            return hidedColumnsInGroup;
        }
        function formatValue(value, options) {
            var currentValue = value;
            if (options.FormatNumber && !isNaN(currentValue) && currentValue !== '') {
                currentValue = numeral(currentValue).format(options.FormatNumber);
            }
            if (options.FormatNegative) {
                if (value < 0) {
                    currentValue = '<span style="color:red">' + options.FormatNegative.replace('$', currentValue) + '</span>';
                }
            }
            return currentValue;
        }
        function createHeader(hideColumnsName) {
            var firstHeaderRow = ['<tr>'];
            var _loop_1 = function (col) {
                if (col.MergeColNumber) {
                    var startIndex = defaultColumns.findIndex(function (e) { return e.Name === col.Name; });
                    var hidedColumnsInGroup = getHidedColumnsInGroup(col.MergeColNumber, startIndex);
                    if (col.MergeColNumber === hidedColumnsInGroup.length)
                        return "continue";
                    var colSpan = " colspan=" + (col.MergeColNumber - hidedColumnsInGroup.length);
                    firstHeaderRow.push("<th " + colSpan + ">" + col.HeaderText + "</th>");
                }
                else if (col.MergeRowNumber) {
                    if (hideColumnsName.includes(col.Name))
                        return "continue";
                    var rowSpan = " rowspan=" + col.MergeRowNumber;
                    firstHeaderRow.push("<th " + rowSpan + ">" + col.HeaderText + "</th>");
                }
            };
            for (var _i = 0, groupHeaderColumns_1 = groupHeaderColumns; _i < groupHeaderColumns_1.length; _i++) {
                var col = groupHeaderColumns_1[_i];
                _loop_1(col);
            }
            firstHeaderRow.push('</tr>');
            var secondHeaderRow = ['<tr>'];
            var filteredColumns = defaultColumns.filter(function (e) { return !hideColumnsName.includes(e.Name); });
            var _loop_2 = function (col) {
                var groupHeaderInfo = groupHeaderColumns.find(function (e) { return e.Name === col.Name; });
                if (groupHeaderInfo && groupHeaderInfo.MergeRowNumber)
                    return "continue";
                secondHeaderRow.push("<th>" + col.HeaderText + "</th>");
            };
            for (var _a = 0, filteredColumns_1 = filteredColumns; _a < filteredColumns_1.length; _a++) {
                var col = filteredColumns_1[_a];
                _loop_2(col);
            }
            secondHeaderRow.push('</tr>');
            return firstHeaderRow.join("") + secondHeaderRow.join("");
        }
        function createBody(hideColumnsName) {
            var bodyRows = [];
            var _loop_3 = function (row) {
                var tds = [];
                var cols = row.Columns.filter(function (col) { return !hideColumnsName.includes(col.Name); });
                cols.forEach(function (col) {
                    var definedCol = defaultColumns.find(function (e) { return e.Name === col.Name; });
                    tds.push("<td>" + formatValue(col.Value, definedCol) + "</td>");
                });
                bodyRows.push("<tr>" + tds.join("") + "</tr>");
            };
            for (var _i = 0, _a = _self.gridBodyRowsCache; _i < _a.length; _i++) {
                var row = _a[_i];
                _loop_3(row);
            }
            return bodyRows.join("");
        }
        function createFooter(hideColumnsName) {
            var footerRows = [];
            var _loop_4 = function (row) {
                var tds = [];
                var cols = row.Columns.filter(function (col) { return !hideColumnsName.includes(col.Name); });
                cols.forEach(function (col) {
                    var startIndex = defaultColumns.findIndex(function (e) { return e.Name === col.Name; });
                    var hidedColumnsInGroup = getHidedColumnsInGroup(col.MergeColNumber, startIndex);
                    var colSpan = col.MergeColNumber ? " colspan=\"" + (col.MergeColNumber - hidedColumnsInGroup.length) + "\"" : '';
                    tds.push("<td " + colSpan + ">" + formatValue(col.Value, defaultColumns[startIndex]) + "</td>");
                });
                footerRows.push("<tr>" + tds.join("") + "</tr>");
            };
            for (var _i = 0, _a = _self.gridFooterRowsCache; _i < _a.length; _i++) {
                var row = _a[_i];
                _loop_4(row);
            }
            return footerRows.join("");
        }
        function createTable() {
            var hideColumns = $("#" + _self.options.selectColumnsId).fanexMultiSelect("getItems", false);
            var hideColumnsName = hideColumns.map(function (colIndex) { return defaultColumns[colIndex].Name; });
            var theadContent = "<thead>" + createHeader(hideColumnsName) + "</thead>";
            var tbodyContent = _self.gridBodyRowsCache.length > 0 ? "<tbody>" + createBody(hideColumnsName) + "</tbody>" : '';
            var tFooterContent = _self.gridFooterRowsCache.length > 0 ? "<tfoot>" + createFooter(hideColumnsName) + "</tfoot>" : '';
            return "<table>" + theadContent + tbodyContent + tFooterContent + "</table>";
        }
        function createHiddenTextArea(text) {
            var textArea = document.createElement("textarea");
            textArea.value = text;
            textArea.style.position = "absolute";
            textArea.style.left = "-9999px";
            document.body.appendChild(textArea);
            return textArea;
        }
        var copyBtn = $("#" + copyBtnId).tooltip({ content: "Copy" }).off("click").on("click", function () {
            var tableHtml = createTable();
            var copyToClipboard = function (text) {
                if (window.navigator["clipboard"] && window.isSecureContext) {
                    return window.navigator["clipboard"].writeText(text);
                }
                else {
                    var textArea = createHiddenTextArea(text);
                    textArea.select();
                    var successful = document.execCommand('copy');
                    textArea.remove();
                    return successful ? Promise.resolve() : Promise.reject();
                }
            };
            copyToClipboard(tableHtml).then(function () {
                copyBtn.tooltip({ content: "Copied" }).tooltip("open");
                setTimeout(function () { return copyBtn.tooltip({ content: "Copy" }).tooltip("close"); }, 2000);
            }).catch(function (err) { return console.error("Copy failed:", err); });
        });
    };
    return BaseReportGrid;
}());
//# sourceMappingURL=ReportGrid.js.map
//# sourceMappingURL=Types.js.map
var StateManager = /** @class */ (function () {
    function StateManager(initialValue) {
        this.subscribers = [];
        this.value = initialValue;
    }
    StateManager.prototype.isSubscribed = function (callback) {
        return this.subscribers.some(function (item) { return item.prototype.name === callback.prototype.name; });
    };
    StateManager.prototype.setValue = function (newValue) {
        if (JSON.stringify(this.value) !== JSON.stringify(newValue)) {
            this.value = newValue;
            this.notifySubscribers();
        }
    };
    StateManager.prototype.getValue = function () {
        return this.value;
    };
    StateManager.prototype.subscribe = function (callback) {
        if (!this.isSubscribed(callback)) {
            this.subscribers.push(callback);
        }
    };
    StateManager.prototype.unsubscribe = function (callback) {
        this.subscribers = this.subscribers.filter(function (subscriber) { return subscriber !== callback; });
    };
    StateManager.prototype.notifySubscribers = function () {
        var _this = this;
        this.subscribers.forEach(function (subscriber) { return subscriber(_this.value); });
    };
    return StateManager;
}());
//# sourceMappingURL=StateManager.js.map
var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var MatchWinLossFilter = /** @class */ (function (_super) {
    __extends(MatchWinLossFilter, _super);
    function MatchWinLossFilter() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.dateFormat = "MM/DD/YYYY";
        return _this;
    }
    MatchWinLossFilter.prototype.renderCalendarPicker = function (options) {
        var _this = this;
        var dateRangePicker = $(options.selectorId);
        var today = moment(dateRangePicker.data("currentdate"));
        var todayValue = today.toDate();
        var yesterday = moment(today).subtract(1, "d");
        this.fromDate = dateRangePicker.data("fromdate");
        this.toDate = dateRangePicker.data("todate");
        dateRangePicker
            .dateRangePicker({
            format: this.dateFormat,
            separator: " - ",
            language: options.language,
            startDate: dateRangePicker.data("mindate"),
            endDate: dateRangePicker.data("maxdate"),
            showShortcuts: true,
            shortcuts: null,
            customShortcuts: this.getCalenderDefinedShortcuts(dateRangePicker),
            duration: 0,
            singleMonth: true,
            monthSelect: true,
            yearSelect: true,
            maxDays: 7
        }).bind("datepicker-change", function (_, obj) {
            _this.fromDate = moment(obj.date1).format(_this.dateFormat);
            _this.toDate = moment(obj.date2).format(_this.dateFormat);
        })
            .bind("datepicker-closed", function () { return options.onClosed(_this.getCalendarValues()); })
            .data("dateRangePicker").setDateRange(this.fromDate, this.toDate, false);
    };
    MatchWinLossFilter.prototype.renderSingleDatePicker = function (options) {
        var _this = this;
        var dateRangePicker = $(options.selectorId);
        this.fromDate = dateRangePicker.data("fromdate");
        this.toDate = dateRangePicker.data("todate");
        dateRangePicker
            .dateRangePicker({
            format: "MM/DD/YYYY",
            separator: " - ",
            language: options.language,
            startDate: dateRangePicker.data("mindate"),
            endDate: dateRangePicker.data("maxdate"),
            showShortcuts: false,
            duration: 0,
            monthSelect: true,
            yearSelect: false,
            singleDate: true,
            singleMonth: true,
            showTopbar: false
        })
            .bind("datepicker-change", function (_, obj) {
            var startDate = moment(obj.date1, "MM/dd/yyyy");
            var endDate = moment(obj.date2, "MM/dd/yyyy");
            //validate if needed
            _this.fromDate = startDate.format("MM/DD/YYYY");
            _this.toDate = startDate.format("MM/DD/YYYY");
        })
            .bind("datepicker-closed", function () { return options.onClosed(_this.getCalendarValues()); })
            .data("dateRangePicker").setDateRange(this.fromDate, this.toDate, false);
        return this.getCalendarValues();
    };
    MatchWinLossFilter.prototype.getCalenderDefinedShortcuts = function (dateRangePicker) {
        var minDate = moment(dateRangePicker.data("mindate"));
        var today = moment(dateRangePicker.data("currentdate"));
        var todayValue = today.toDate();
        var yesterday = moment(today).subtract(1, "d");
        var thisWeekStartOfDay = moment(today).startOf("isoWeek");
        return [
            {
                name: dateRangePicker.data("label-today"), dates: function () {
                    return [todayValue, todayValue];
                }
            },
            {
                name: dateRangePicker.data("label-yesterday"),
                dates: function () {
                    var yesterdayValue = yesterday.toDate();
                    return [yesterdayValue, yesterdayValue];
                }
            },
            {
                name: dateRangePicker.data("label-thisweek"),
                dates: function () {
                    return [
                        thisWeekStartOfDay.diff(minDate) >= 0 ? thisWeekStartOfDay.toDate() : minDate.toDate(),
                        todayValue
                    ];
                }
            },
            {
                name: dateRangePicker.data("label-lastweek"),
                dates: function () {
                    var lastWeekStartOfDay = moment(thisWeekStartOfDay).subtract(1, "w");
                    return [
                        lastWeekStartOfDay.diff(minDate) >= 0 ? lastWeekStartOfDay.toDate() : minDate.toDate(),
                        moment(thisWeekStartOfDay).subtract(1, "d").toDate()
                    ];
                }
            }
        ];
    };
    return MatchWinLossFilter;
}(BaseReportFilters));
//# sourceMappingURL=MatchWinLossFilter.js.map
var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
function roundMoney(cell, formatNumber) {
    if (formatNumber === void 0) { formatNumber = ""; }
    if (cell.Value == "empty")
        return "";
    if (isNaN(cell.Value) || !formatNumber)
        return cell.Value;
    var absNum = Math.abs(cell.Value);
    var color = cell.Value < 0 ? 'red' : '';
    var fullNumber = numeral(cell.Value).format(formatNumber);
    if (absNum >= 1000000000) {
        return "<span style=\"color:" + color + "\" title=\"" + fullNumber + "\" >" + numeral(cell.Value / 1000000000).format(formatNumber) + "B</span>";
    }
    else if (absNum >= 1000000) {
        return "<span style=\"color:" + color + "\" title=\"" + fullNumber + "\">" + numeral(cell.Value / 1000000).format(formatNumber) + "M</span>";
    }
    return "<span style=\"color:" + color + "\">" + numeral(cell.Value).format(formatNumber) + "</span>";
}
var MatchWinLossGrid = /** @class */ (function (_super) {
    __extends(MatchWinLossGrid, _super);
    function MatchWinLossGrid(options) {
        return _super.call(this, options) || this;
    }
    MatchWinLossGrid.prototype.buildEventName = function (homeTeam, awayTeam, isNeutral) {
        var neutralSuffix = isNeutral ? '(N)' : '';
        if (!homeTeam && !awayTeam)
            return '';
        if (homeTeam && awayTeam)
            return "" + homeTeam + neutralSuffix + "\n" + awayTeam + neutralSuffix;
        if (homeTeam)
            return "" + homeTeam + neutralSuffix + "\n__";
        return "__\n" + awayTeam + neutralSuffix;
    };
    MatchWinLossGrid.prototype.buildCommonColumns = function (element) {
        if (!element)
            return [];
        return [
            { Name: "StakeAvg", Value: element["StakeAvg"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
            { Name: "BetCount", Value: element["BetCount"], CustomCellRender: function (cell) { return roundMoney(cell); } },
            { Name: "Turnover", Value: element["Turnover"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
            { Name: "NetTurnover", Value: element["NetTurnover"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
            { Name: "GrossComm", Value: element["GrossComm"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
            { Name: "MemberWinLoss", Value: element["MemberWinLoss"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
            { Name: "MemberComm", Value: element["MemberComm"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
            { Name: "MemberTotal", Value: element["MemberTotal"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
            { Name: "MemberMargin", Value: element["MemberMargin"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
            { Name: "AgentWinLoss", Value: element["AgentWinLoss"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
            { Name: "AgentComm", Value: element["AgentComm"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
            { Name: "AgentTotal", Value: element["AgentTotal"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
            { Name: "MasterWinLoss", Value: element["MasterWinLoss"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
            { Name: "MasterComm", Value: element["MasterComm"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
            { Name: "MasterTotal", Value: element["MasterTotal"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
            { Name: "SuperWinLoss", Value: element["SuperWinLoss"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
            { Name: "SuperComm", Value: element["SuperComm"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
            { Name: "SuperTotal", Value: element["SuperTotal"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
            { Name: "CompanyTotal", Value: element["CompanyTotal"], CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } }
        ];
    };
    return MatchWinLossGrid;
}(BaseReportGrid));
var SummaryGrid = /** @class */ (function (_super) {
    __extends(SummaryGrid, _super);
    function SummaryGrid(options) {
        var _this = _super.call(this, options) || this;
        _this.gridOptions = {
            customizeHeader: true,
            groupHeaderRows: [{ Columns: SummaryGrid.GroupHeaderColumns }],
            columns: SummaryGrid.DefaultColumns,
            numberOfFrozenColumn: 2,
            autoCalculateGridExpandHeight: true,
            fullHeight: !options.isMobile.phone,
            gridHeight: options.isMobile.phone ? $(window).height() - 70 : 0,
            showTitle: false,
            // titleInfo: {
            //     text: "",
            //     customButtons: `<span>Columns<div id=${options.selectColumnsId}></div> </span><span id=${options.excelBtnId} title="Export Excel" class="icon-excel"></span>`
            // },
            gridExpandHeight: 400,
            numOfPaddingForGridExpandHeight: 40,
            headerColumnHeight: 30,
            bodyRowHeight: 26,
            showPager: true,
            customPager: true,
            pagerOption: {
                itemsPerPageArray: options.pageSizeOptions,
                currentPage: 1,
                totalItem: 2000,
                itemsPerPage: options.pageSizeOptions[0],
                pagerSelectCallBack: function (pageIndex, pageSize) {
                }
            },
            marginRight: 0,
            // padding: 10,
            bottomBorderWidth: 0,
            showDistinctRow: false,
            allowTruncate: true,
            bodyRows: [],
            showFooter: true,
            footerRows: [],
            sortOption: {
                serverSortFunction: function (colname) {
                }
            },
            showHighlightRow: false,
        };
        return _this;
    }
    SummaryGrid.prototype.buildBodyRows = function (reportData) {
        var bodyData = [];
        var Events = reportData.Events, GroupLeagues = reportData.GroupLeagues;
        for (var _i = 0, GroupLeagues_1 = GroupLeagues; _i < GroupLeagues_1.length; _i++) {
            var leagueGroup = GroupLeagues_1[_i];
            bodyData.push({
                Columns: [
                    { Name: "KickOffTime", Value: leagueGroup.LeagueName.trim(), CssClass: 'break-space', MergeColNumber: 2, CellAlign: "left" },
                    { Name: "StakeAvg", Value: "empty" },
                    { Name: "BetCount", Value: "empty" },
                    { Name: "Turnover", Value: "empty" },
                    { Name: "NetTurnover", Value: "empty" },
                    { Name: "GrossComm", Value: "empty" },
                    { Name: "MemberWinLoss", Value: "empty" },
                    { Name: "MemberComm", Value: "empty" },
                    { Name: "MemberTotal", Value: "empty" },
                    { Name: "MemberMargin", Value: "empty" },
                    { Name: "AgentWinLoss", Value: "empty" },
                    { Name: "AgentComm", Value: "empty" },
                    { Name: "AgentTotal", Value: "empty" },
                    { Name: "MasterWinLoss", Value: "empty" },
                    { Name: "MasterComm", Value: "empty" },
                    { Name: "MasterTotal", Value: "empty" },
                    { Name: "SuperWinLoss", Value: "empty" },
                    { Name: "SuperComm", Value: "empty" },
                    { Name: "SuperTotal", Value: "empty" },
                    { Name: "CompanyTotal", Value: "empty" }
                ],
                CssClass: "row-league-group"
            });
            for (var _a = 0, Events_1 = Events; _a < Events_1.length; _a++) {
                var element = Events_1[_a];
                if (element.LeagueId != leagueGroup.LeagueId)
                    continue;
                var HomeName = element.HomeName, AwayName = element.AwayName, IsNeutral = element.IsNeutral, KickOffTimeStr = element.KickOffTimeStr;
                bodyData.push({
                    Columns: [
                        {
                            Name: "KickOffTime",
                            Value: moment(KickOffTimeStr).format('MM/DD/YYYY [\n] hh:mm A'),
                            CssClass: 'break-space'
                        },
                        {
                            Name: "Event",
                            Value: this.buildEventName(HomeName.trim(), AwayName.trim(), IsNeutral),
                            CssClass: 'break-space'
                        }
                    ].concat(this.buildCommonColumns(element))
                });
            }
        }
        return bodyData;
    };
    SummaryGrid.prototype.buildFooterRows = function (reportData) {
        var _this = this;
        var Total = reportData.Total, SystemParlayOutright = reportData.SystemParlayOutright, GrandTotal = reportData.GrandTotal, Events = reportData.Events;
        var totalText = reportData.currency ? "Total in " + reportData.currency : "Total";
        var grandTotalText = reportData.currency ? "Grand Total in " + reportData.currency : "Grand Total";
        var footerColumns = [];
        var mergeColNumber = 2;
        if (Events) {
            if (Events.length > 0 && JSON.stringify(Total) !== JSON.stringify(GrandTotal)) {
                footerColumns.push({
                    Columns: [
                        { Name: "KickOffTime", Value: totalText, MergeColNumber: mergeColNumber, CellAlign: "right" }
                    ].concat(this.buildCommonColumns(Total)),
                    RowHeight: 21,
                    CssClass: "row-total"
                });
            }
        }
        if (SystemParlayOutright.length > 0) {
            SystemParlayOutright.forEach(function (e) {
                footerColumns.push({
                    Columns: [
                        { Name: "KickOffTime", Value: e["BetTypeName"], MergeColNumber: mergeColNumber, CellAlign: "right" }
                    ].concat(_this.buildCommonColumns(e)),
                    RowHeight: 21,
                    CssClass: 'system-parlay'
                });
            });
        }
        if (GrandTotal) {
            footerColumns.push({
                Columns: [
                    { Name: "KickOffTime", Value: grandTotalText, MergeColNumber: mergeColNumber, CellAlign: "right" }
                ].concat(this.buildCommonColumns(GrandTotal)),
                RowHeight: 21,
                CellAlign: "right",
                CssClass: 'row-grand-total'
            });
        }
        return footerColumns;
    };
    SummaryGrid.prototype.getDefaultColumns = function () {
        return SummaryGrid.DefaultColumns.filter(function (e) { return e.Name != "BetType"; });
    };
    ;
    SummaryGrid.DefaultColumns = [
        { HeaderText: "KO Time \n (GMT+8)", Name: "KickOffTime", Width: 80, Sort: false, HeaderAlign: "center", ColumnName2: "KO Time (GMT+8)", CellAlign: "center" },
        { HeaderText: "Event", Name: "Event", Width: 380, Sort: false, HeaderAlign: "center", ColumnName2: "Event", },
        { HeaderText: "Avg.Stake", Name: "StakeAvg", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", ColumnName2: "Avg.Stake", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Bet Count", Name: "BetCount", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", ColumnName2: "Bet Count", CustomCellRender: function (cell) { return roundMoney(cell); } },
        { HeaderText: "Turnover", Name: "Turnover", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Turnover", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Net Turnover", Name: "NetTurnover", Width: 120, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Net Turnover", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "GrossComm", Name: "GrossComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "GrossComm", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Win/Loss", Name: "MemberWinLoss", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Member-Win Loss", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Comm", Name: "MemberComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Member-Comm", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Total", Name: "MemberTotal", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Member-Total", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        {
            HeaderText: "Margin(%)&nbsp;<span title='Member Margin is calculated by dividing the sum of Member Win/Loss and Commission by the Net Turnover.' class=\"bet-type-detail-icon\"></span>",
            Name: "MemberMargin", Width: 120, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Member-Margin", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); }
        },
        { HeaderText: "Win/Loss", Name: "AgentWinLoss", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Agent-Win Loss", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Comm", Name: "AgentComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Agent-Comm", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Total", Name: "AgentTotal", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Agent-Total", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Win/Loss", Name: "MasterWinLoss", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Master-Win Loss", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Comm", Name: "MasterComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Master-Comm", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Total", Name: "MasterTotal", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Master-Total", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Win/Loss", Name: "SuperWinLoss", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Super-Win Loss", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Comm", Name: "SuperComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Super-Comm", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Total", Name: "SuperTotal", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Super-Total", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Company", Name: "CompanyTotal", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Company", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } }
    ];
    SummaryGrid.GroupHeaderColumns = [
        { HeaderText: "KO Time \n (GMT+8)", Name: "KickOffTime", MergeRowNumber: 2 },
        { HeaderText: "Event", Name: "Event", MergeRowNumber: 2 },
        { HeaderText: "AvgStake", Name: "StakeAvg", MergeRowNumber: 2 },
        { HeaderText: "Bet Count", Name: "BetCount", MergeRowNumber: 2 },
        { HeaderText: "Turnover", Name: "Turnover", MergeRowNumber: 2 },
        {
            HeaderText: "Net Turnover&nbsp;<span id=\"netTurnover\" title='Net Turnover is Turnover which excludes the draw tickets and halves of the turnover of the half won, half lost tickets.' class=\"bet-type-detail-icon\"></span>",
            Name: "NetTurnover",
            MergeRowNumber: 2
        },
        { HeaderText: "GrossComm", Name: "GrossComm", MergeRowNumber: 2 },
        { HeaderText: "Member", Name: "MemberWinLoss", MergeColNumber: 4 },
        { HeaderText: "Agent", Name: "AgentWinLoss", MergeColNumber: 3 },
        { HeaderText: "Master", Name: "MasterWinLoss", MergeColNumber: 3 },
        { HeaderText: "Super", Name: "SuperWinLoss", MergeColNumber: 3 },
        { HeaderText: "Company", Name: "CompanyTotal", MergeRowNumber: 2 }
    ];
    return SummaryGrid;
}(MatchWinLossGrid));
var DetailGrid = /** @class */ (function (_super) {
    __extends(DetailGrid, _super);
    function DetailGrid(options) {
        var _this = _super.call(this, options) || this;
        _this.gridOptions = {
            customizeHeader: true,
            groupHeaderRows: [{ Columns: DetailGrid.GroupHeaderColumns }],
            columns: DetailGrid.DefaultColumns,
            numberOfFrozenColumn: 3,
            autoCalculateGridExpandHeight: true,
            fullHeight: !options.isMobile.phone,
            gridHeight: options.isMobile.phone ? $(window).height() - 70 : 0,
            showTitle: false,
            // titleInfo: {
            //     text: "",
            //     customButtons: `<span id=${options.excelBtnId} title="Export Excel" class="icon-excel"></span>`
            // },
            gridExpandHeight: 400,
            numOfPaddingForGridExpandHeight: 40,
            headerColumnHeight: 30,
            bodyRowHeight: 26,
            showPager: true,
            customPager: true,
            pagerOption: {
                itemsPerPageArray: options.pageSizeOptions,
                currentPage: 1,
                totalItem: 2000,
                itemsPerPage: options.pageSizeOptions[0],
                pagerSelectCallBack: function (pageIndex, pageSize) {
                }
            },
            marginRight: 0,
            // padding: 10,
            bottomBorderWidth: 0,
            showDistinctRow: false,
            allowTruncate: true,
            bodyRows: [],
            showFooter: true,
            footerRows: [],
            sortOption: {
                serverSortFunction: function (colname) {
                }
            },
            showHighlightRow: false,
            numberOfColumnHaveMergeRow: 2,
            enableHiddenColumns: true
        };
        return _this;
    }
    DetailGrid.prototype.buildBodyRows = function (reportData) {
        var rows = [];
        var currentMergeRowNumber = 0;
        var Events = reportData.Events, GroupLeagues = reportData.GroupLeagues;
        var subTotalText = reportData.currency ? "Sub Total in " + reportData.currency : "Sub Total";
        for (var _i = 0, GroupLeagues_2 = GroupLeagues; _i < GroupLeagues_2.length; _i++) {
            var leagueGroup = GroupLeagues_2[_i];
            rows.push({
                Columns: [
                    { Name: "KickOffTime", Value: leagueGroup.LeagueName.trim(), CssClass: 'break-space', MergeColNumber: 3, CellAlign: "left" },
                    { Name: "StakeAvg", Value: "empty" },
                    { Name: "BetCount", Value: "empty" },
                    { Name: "Turnover", Value: "empty" },
                    { Name: "NetTurnover", Value: "empty" },
                    { Name: "GrossComm", Value: "empty" },
                    { Name: "MemberWinLoss", Value: "empty" },
                    { Name: "MemberComm", Value: "empty" },
                    { Name: "MemberTotal", Value: "empty" },
                    { Name: "MemberMargin", Value: "empty" },
                    { Name: "AgentWinLoss", Value: "empty" },
                    { Name: "AgentComm", Value: "empty" },
                    { Name: "AgentTotal", Value: "empty" },
                    { Name: "MasterWinLoss", Value: "empty" },
                    { Name: "MasterComm", Value: "empty" },
                    { Name: "MasterTotal", Value: "empty" },
                    { Name: "SuperWinLoss", Value: "empty" },
                    { Name: "SuperComm", Value: "empty" },
                    { Name: "SuperTotal", Value: "empty" },
                    { Name: "CompanyTotal", Value: "empty" }
                ],
                CssClass: "row-league-group"
            });
            var _loop_1 = function (element) {
                if (element.LeagueId != leagueGroup.LeagueId)
                    return "continue";
                var currentColumns = [];
                var MatchId = element.MatchId, HomeName = element.HomeName, AwayName = element.AwayName, IsNeutral = element.IsNeutral, KickOffTimeStr = element.KickOffTimeStr, BetTypeName = element.BetTypeName, BetTypeId = element.BetTypeId;
                if (currentMergeRowNumber == 0) {
                    currentMergeRowNumber = Events.filter(function (e) { return e.MatchId == MatchId; }).length;
                    currentColumns.push({
                        Name: "KickOffTime",
                        Value: moment(KickOffTimeStr).format('MM/DD/YYYY hh:mm A'),
                        MergeRowNumber: currentMergeRowNumber,
                        CssClass: 'break-space'
                    }, {
                        Name: "Event",
                        Value: this_1.buildEventName(HomeName.trim(), AwayName.trim(), IsNeutral),
                        MergeRowNumber: currentMergeRowNumber,
                        CssClass: 'break-space'
                    });
                }
                var isBetType0 = BetTypeId == 0;
                rows.push({
                    Columns: currentColumns.concat([
                        {
                            Name: "BetType",
                            Value: isBetType0 ? subTotalText : BetTypeName
                        }
                    ].concat(this_1.buildCommonColumns(element))),
                    CssClass: isBetType0 ? 'sub-total' : ''
                });
                currentMergeRowNumber--;
            };
            var this_1 = this;
            for (var _a = 0, Events_2 = Events; _a < Events_2.length; _a++) {
                var element = Events_2[_a];
                _loop_1(element);
            }
        }
        return rows;
    };
    DetailGrid.prototype.getDefaultColumns = function () {
        return DetailGrid.DefaultColumns;
    };
    DetailGrid.prototype.buildFooterRows = function (reportData) {
        var _this = this;
        var Total = reportData.Total, SystemParlayOutright = reportData.SystemParlayOutright, GrandTotal = reportData.GrandTotal, Events = reportData.Events;
        var totalText = reportData.currency ? "Total in " + reportData.currency : "Total";
        var grandTotalText = reportData.currency ? "Grand Total in " + reportData.currency : "Grand Total";
        var footerColumns = [];
        var mergeColNumber = 3;
        if (Events) {
            if (Events.length > 0 && JSON.stringify(Total) !== JSON.stringify(GrandTotal)) {
                footerColumns.push({
                    Columns: [
                        { Name: "KickOffTime", Value: totalText, MergeColNumber: mergeColNumber, CellAlign: "right" }
                    ].concat(this.buildCommonColumns(Total)),
                    RowHeight: 21,
                    CssClass: "row-total"
                });
            }
        }
        if (SystemParlayOutright.length > 0) {
            SystemParlayOutright.forEach(function (e) {
                footerColumns.push({
                    Columns: [
                        {
                            Name: "KickOffTime",
                            Value: e["BetTypeName"],
                            MergeColNumber: mergeColNumber,
                            CellAlign: "right"
                        }
                    ].concat(_this.buildCommonColumns(e)),
                    RowHeight: 21,
                    CssClass: 'system-parlay'
                });
            });
        }
        if (GrandTotal) {
            footerColumns.push({
                Columns: [
                    { Name: "KickOffTime", Value: grandTotalText, MergeColNumber: mergeColNumber, CellAlign: "right" }
                ].concat(this.buildCommonColumns(GrandTotal)),
                RowHeight: 21,
                CellAlign: "right",
                CssClass: "row-grand-total"
            });
        }
        return footerColumns;
    };
    DetailGrid.DefaultColumns = [
        { HeaderText: "KO Time\n(GMT+8)", Name: "KickOffTime", Width: 80, Sort: false, HeaderAlign: "center", ColumnName2: "KO Time (GMT+8)", CellAlign: "center", },
        { HeaderText: "Event", Name: "Event", Width: 380, Sort: false, HeaderAlign: "center", ColumnName2: "Event", },
        { HeaderText: "BetType", Name: "BetType", Width: 200, Sort: false, HeaderAlign: "center", ColumnName2: "BetType", },
        { HeaderText: "Avg.Stake", Name: "StakeAvg", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", ColumnName2: "Avg.Stake", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Bet Count", Name: "BetCount", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", ColumnName2: "Bet Count", CustomCellRender: function (cell) { return roundMoney(cell); } },
        { HeaderText: "Turnover", Name: "Turnover", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Turnover", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Net Turnover", Name: "NetTurnover", Width: 120, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Net Turnover", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "GrossComm", Name: "GrossComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "GrossComm", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Win/Loss", Name: "MemberWinLoss", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Member-Win Loss", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Comm", Name: "MemberComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Member-Comm", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Total", Name: "MemberTotal", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Member-Total", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        {
            HeaderText: "Margin(%)&nbsp;<span title='Member Margin is calculated by dividing the sum of Member Win/Loss and Commission by the Net Turnover.' class=\"bet-type-detail-icon\"></span>",
            Name: "MemberMargin", Width: 120, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Member-Margin", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); }
        },
        { HeaderText: "Win/Loss", Name: "AgentWinLoss", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Agent-Win Loss", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Comm", Name: "AgentComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Agent-Comm", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Total", Name: "AgentTotal", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Agent-Total", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Win/Loss", Name: "MasterWinLoss", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Master-Win Loss", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Comm", Name: "MasterComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Master-Comm", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Total", Name: "MasterTotal", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Master-Total", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Win/Loss", Name: "SuperWinLoss", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Super-Win Loss", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Comm", Name: "SuperComm", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Super-Comm", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Total", Name: "SuperTotal", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Super-Total", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } },
        { HeaderText: "Company", Name: "CompanyTotal", Width: 100, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Company", CustomCellRender: function (cell) { return roundMoney(cell, "0,0.00"); } }
    ];
    DetailGrid.GroupHeaderColumns = [
        { HeaderText: "KO Time\n(GMT+8)", Name: "KickOffTime", MergeRowNumber: 2 },
        { HeaderText: "Event", Name: "Event", MergeRowNumber: 2 },
        { HeaderText: "Bet Type", Name: "BetType", MergeRowNumber: 2 },
        { HeaderText: "AvgStake", Name: "StakeAvg", MergeRowNumber: 2 },
        { HeaderText: "Bet Count", Name: "BetCount", MergeRowNumber: 2 },
        { HeaderText: "Turnover", Name: "Turnover", MergeRowNumber: 2 },
        {
            HeaderText: "Net Turnover&nbsp;<span id=\"netTurnover\" title='Net Turnover is Turnover which excludes the draw tickets and halves of the turnover of the half won, half lost tickets.' class=\"bet-type-detail-icon\"></span>",
            Name: "NetTurnover",
            MergeRowNumber: 2
        },
        { HeaderText: "GrossComm", Name: "GrossComm", MergeRowNumber: 2 },
        { HeaderText: "Member", Name: "MemberWinLoss", MergeColNumber: 4 },
        { HeaderText: "Agent", Name: "AgentWinLoss", MergeColNumber: 3 },
        { HeaderText: "Master", Name: "MasterWinLoss", MergeColNumber: 3 },
        { HeaderText: "Super", Name: "SuperWinLoss", MergeColNumber: 3 },
        { HeaderText: "Company", Name: "CompanyTotal", MergeRowNumber: 2 }
    ];
    return DetailGrid;
}(MatchWinLossGrid));
//# sourceMappingURL=MatchWinLossGrid.js.map
var __assign = (this && this.__assign) || Object.assign || function(t) {
    for (var s, i = 1, n = arguments.length; i < n; i++) {
        s = arguments[i];
        for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
            t[p] = s[p];
    }
    return t;
};
var MatchWinLossReportDetail = /** @class */ (function () {
    function MatchWinLossReportDetail() {
        this.gridComponentConfig = {
            reportId: "detail",
            isMobile: isMobile,
            gridId: "detail-jqgrid",
            gridContainerId: "detail-mainGrid",
            selectColumnsId: "detail-ddl-columns",
            selectColumnsContainerId: "detail-ddl-columns-container",
            msgNoInfoId: "detail-msgNoInfo",
            excelBtnId: "detail-excel",
            pageSizeOptions: [100, 150, 200]
        };
        this.typeReport = "detail";
        this.cachePageSize = this.gridComponentConfig.pageSizeOptions[0];
        this.cachePage = 1;
        this.sortBy = "";
        this.sortType = 0;
        this.baseCurrency = "";
        this.byPassValidateLeague = false;
        this.byPassValidateEvent = false;
        this.paramsState = new StateManager({
            "FromDate": null,
            "ToDate": null,
            "Sport": null,
            "Leagues": null,
            "Matches": null,
            "BetTypes": null
        });
        this.filterComponents = new MatchWinLossFilter($);
        this.gridComponent = new DetailGrid(this.gridComponentConfig);
        this.isMY = $("#inp-isMY").val().toLowerCase() === "true";
        this.sportState = new StateManager("");
        this.datesState = new StateManager({ fromDate: null, toDate: null });
        this.leagueIdsState = new StateManager("");
        this.currencyState = new StateManager("All");
    }
    MatchWinLossReportDetail.prototype.init = function () {
        var _this = this;
        Fanex.ExtAcc.BlockUI();
        this.initDateRangePicker();
        this.initSportsDropdown(JSON.parse($("#inp-sports").val()));
        this.initLeaguesDropdown(JSON.parse($("#inp-leagues").val()));
        this.initEventsDropdown(JSON.parse($("#inp-events").val()));
        this.initBetTypesDropdown(JSON.parse($("#inp-bettypes").val()));
        if (this.isMY) {
            this.initCurrencyDropDown(JSON.parse($("#inp-currencies").val()));
            this.initBaseCurrencyDropDown([
                { id: "MYR", value: "MYR", text: "MYR", selected: true },
                { id: "CNY", value: "CNY", text: "CNY", selected: false },
                { id: "THB", value: "THB", text: "THB", selected: false },
                { id: "IDR", value: "IDR", text: "IDR", selected: false }
            ]);
        }
        $("#" + this.typeReport + "-submit-btn").click(function () { return _this.handleClickSubmit(); });
        this.gridComponent.initSelectColumnsDropdown(DetailGrid.DefaultColumns, [0, 1, 2]);
        this.initExportExcelBtn();
        Fanex.ExtAcc.UnBlockUI();
    };
    MatchWinLossReportDetail.prototype.initDateRangePicker = function () {
        var _self = this;
        this.filterComponents.renderSingleDatePicker({
            selectorId: "#" + this.typeReport + "-date-range-picker",
            onClosed: function (value) {
                _self.datesState.setValue(value);
            }
        });
        this.initCalendarState();
    };
    MatchWinLossReportDetail.prototype.initCalendarState = function () {
        var _self = this;
        var initDate = this.filterComponents.getCalendarValues();
        _self.datesState.setValue(initDate);
        var updateLeaguesEvents = function (_a) {
            var fromDate = _a.fromDate, toDate = _a.toDate;
            Fanex.ExtAcc.BlockUI();
            _self.getLeaguesAndEvents(_self.sportState.getValue(), fromDate, toDate, function (_a) {
                var leagues = _a.leagues, events = _a.events;
                _self.initLeaguesDropdown(leagues, true);
                _self.initEventsDropdown(events);
            });
            Fanex.ExtAcc.UnBlockUI();
        };
        _self.datesState.subscribe(updateLeaguesEvents);
    };
    MatchWinLossReportDetail.prototype.initSportsDropdown = function (sports) {
        if (!sports)
            return;
        var _self = this;
        var items = [];
        sports.forEach(function (e, i) {
            items.push({
                id: e.SportType,
                text: e.SportName,
                selected: i === 0
            });
        });
        this.initSportState(sports[0].SportType);
        this.filterComponents.registerDropdownComponent({
            dropdownId: this.typeReport + "-ddl-sport",
            dropdownName: "ddl-sport",
            dropdownType: "select2",
            dropdownData: items,
            dropdownConfig: {
                noneSelectedText: "Select sport",
            },
            onClosed: function (sportID) {
                _self.sportState.setValue(sportID);
            }
        });
    };
    MatchWinLossReportDetail.prototype.initSportState = function (sportType) {
        var _self = this;
        this.sportState.setValue(sportType);
        var updateLeaguesEvents = function (sportID) {
            Fanex.ExtAcc.BlockUI();
            var _a = _self.datesState.getValue(), fromDate = _a.fromDate, toDate = _a.toDate;
            _self.getLeaguesAndEvents(sportID, fromDate, toDate, function (_a) {
                var leagues = _a.leagues, events = _a.events;
                _self.initLeaguesDropdown(leagues, true);
                _self.initEventsDropdown(events);
            });
            Fanex.ExtAcc.UnBlockUI();
        };
        _self.sportState.subscribe(updateLeaguesEvents);
    };
    MatchWinLossReportDetail.prototype.initLeaguesDropdown = function (leagues, isSkipNotify) {
        if (isSkipNotify === void 0) { isSkipNotify = false; }
        this.byPassValidateLeague = leagues.length === 0;
        var items = [];
        var maxLength = 0;
        leagues.forEach(function (e, i) {
            if ((e.LeagueName || "").length > maxLength) {
                maxLength = e.LeagueName.length;
            }
            items.push({
                id: e.LeagueId,
                value: e.LeagueId,
                text: e.LeagueName,
                checked: false,
                enable: true,
                items: []
            });
        });
        var _self = this;
        !isSkipNotify && _self.initLeagueIdsState(leagues);
        this.filterComponents.registerDropdownComponent({
            dropdownId: this.typeReport + "-ddl-leagues",
            dropdownName: "ddl-leagues",
            dropdownType: "fanexMultiSelectSearchable",
            dropdownData: items,
            dropdownConfig: {
                noneSelectedText: items.length ? "Select league" : "There is no League",
            },
            allowRerender: true,
            toggleWidth: isMobile.phone ? 243 : 193,
            minWidth: isMobile.phone ? 300 : 500,
            onClosed: function (leagueIDs) {
                _self.leagueIdsState.setValue(leagueIDs);
            }
        });
    };
    MatchWinLossReportDetail.prototype.initLeagueIdsState = function (leagues) {
        var _self = this;
        var leagueIDs = leagues.map(function (e) { return e.LeagueId; }).join(",");
        this.leagueIdsState.setValue(leagueIDs);
        var updateEvents = function (leagueIDs) {
            Fanex.ExtAcc.BlockUI();
            _self.getEvents(_self.sportState.getValue(), leagueIDs, _self.datesState.getValue().fromDate);
            Fanex.ExtAcc.UnBlockUI();
        };
        this.leagueIdsState.subscribe(updateEvents);
    };
    MatchWinLossReportDetail.prototype.initEventsDropdown = function (events) {
        this.byPassValidateEvent = events.length === 0;
        var items = [];
        var maxLength = 0;
        events.forEach(function (e, i) {
            var textValue = e.HomeName + "-vs-" + e.AwayName;
            if ((e.textValue || "").length > maxLength) {
                maxLength = e.textValue.length;
            }
            items.push({
                id: e.MatchId,
                value: e.MatchId,
                text: textValue,
                checked: false,
                enable: true,
                items: []
            });
        });
        this.filterComponents.registerDropdownComponent({
            dropdownId: this.typeReport + "-ddl-events",
            dropdownName: "ddl-events",
            dropdownConfig: {
                noneSelectedText: items.length ? "Select event" : "There is no event"
            },
            dropdownType: "fanexMultiSelectSearchable",
            dropdownData: items,
            toggleWidth: isMobile.phone ? 243 : 193,
            minWidth: isMobile.phone ? 300 : 500,
        });
    };
    MatchWinLossReportDetail.prototype.initBetTypesDropdown = function (betTypes) {
        var items = [];
        betTypes.forEach(function (e, i) {
            items.push({
                id: e.BetTypeId.toString(),
                value: e.BetTypeId.toString(),
                text: e.BetTypeName,
                checked: false,
                enable: true,
                items: []
            });
        });
        var _self = this;
        this.filterComponents.registerDropdownComponent({
            dropdownId: this.typeReport + "-ddl-bet-types",
            dropdownName: "ddl-bet-types",
            dropdownConfig: {
                noneSelectedText: "Select bet type"
            },
            dropdownType: "fanexMultiSelectSearchable",
            dropdownData: items,
            toggleWidth: isMobile.phone ? 243 : 193,
            minWidth: isMobile.phone ? 250 : 300
        });
    };
    MatchWinLossReportDetail.prototype.initCurrencyDropDown = function (currencies) {
        if (!currencies)
            return;
        var _self = this;
        var items = [
            {
                id: "All",
                value: "All",
                text: "All",
                selected: false
            }
        ];
        currencies.forEach(function (e, i) {
            items.push({
                id: e.CurrencyName,
                value: e.CurrencyName,
                text: e.CurrencyName,
                selected: false
            });
        });
        this.initCurrencyState();
        this.filterComponents.registerDropdownComponent({
            dropdownId: this.typeReport + "-ddl-currencies",
            dropdownName: "ddl-currencies",
            dropdownType: "select2",
            dropdownData: items,
            onClosed: function (currencyName) {
                _self.currencyState.setValue(currencyName);
            }
        });
    };
    MatchWinLossReportDetail.prototype.initCurrencyState = function () {
        var _self = this;
        var updateBaseCurrency = function (currencyID) {
            Fanex.ExtAcc.BlockUI();
            var optionBaseCurrency = [];
            if (currencyID === "All") {
                optionBaseCurrency = [
                    { id: "MYR", value: "MYR", text: "MYR", selected: true },
                    { id: "CNY", value: "CNY", text: "CNY", selected: false },
                    { id: "THB", value: "THB", text: "THB", selected: false },
                    { id: "IDR", value: "IDR", text: "IDR", selected: false }
                ];
            }
            else {
                optionBaseCurrency = [
                    { id: "Customer Currency", value: "Customer Currency", text: "Customer Currency", selected: true },
                    { id: "MYR", value: "MYR", text: "MYR", selected: false },
                    { id: "CNY", value: "CNY", text: "CNY", selected: false },
                    { id: "THB", value: "THB", text: "THB", selected: false },
                    { id: "IDR", value: "IDR", text: "IDR", selected: false },
                ];
            }
            _self.initBaseCurrencyDropDown(optionBaseCurrency);
            Fanex.ExtAcc.UnBlockUI();
        };
        this.currencyState.subscribe(updateBaseCurrency);
    };
    MatchWinLossReportDetail.prototype.initBaseCurrencyDropDown = function (baseCurrencies) {
        if (!baseCurrencies)
            return;
        this.filterComponents.registerDropdownComponent({
            dropdownId: this.typeReport + "-ddl-base-currencies",
            dropdownName: "ddl-base-currencies",
            dropdownType: "select2",
            dropdownData: baseCurrencies
        });
    };
    MatchWinLossReportDetail.prototype.handleClickSubmit = function () {
        var _this = this;
        Fanex.ExtAcc.BlockUI();
        var isValid = this.validateParams();
        if (!isValid) {
            Fanex.ExtAcc.UnBlockUI();
            this.gridComponent.hideNoInfo(false);
        }
        else {
            var paramsData = this.getParams();
            paramsData.Page = 1;
            paramsData.PageSize = this.cachePageSize;
            paramsData.SortBy = "";
            paramsData.SortType = 0;
            this.paramsState.setValue(paramsData);
            $.ajax({
                url: resolveClientUrl("MatchWinLossReports/GetDetailReport"),
                type: "post",
                dataType: "json",
                cache: false,
                data: paramsData,
                success: function (result) { return _this.submitSuccess(result); }
            });
        }
    };
    MatchWinLossReportDetail.prototype.handleClickPage = function (_a) {
        var _this = this;
        var page = _a.page, pageSize = _a.pageSize;
        Fanex.ExtAcc.BlockUI();
        var paramsData = this.paramsState.getValue();
        paramsData.Page = page;
        paramsData.PageSize = pageSize;
        $.ajax({
            url: resolveClientUrl("MatchWinLossReports/GetDetailReport"),
            type: "post",
            dataType: "json",
            cache: false,
            data: paramsData,
            success: function (result) { return _this.submitSuccess(result); }
        });
    };
    MatchWinLossReportDetail.prototype.handleClickSort = function (_a) {
        var _this = this;
        var sortBy = _a.sortBy, sortType = _a.sortType;
        Fanex.ExtAcc.BlockUI();
        var paramsData = this.paramsState.getValue();
        paramsData.SortBy = sortBy;
        paramsData.SortType = sortType;
        $.ajax({
            url: resolveClientUrl("MatchWinLossReports/GetDetailReport"),
            type: "post",
            dataType: "json",
            cache: false,
            data: paramsData,
            success: function (result) { return _this.submitSuccess(result); }
        });
    };
    MatchWinLossReportDetail.prototype.getParams = function () {
        var params = this.filterComponents.getAllFilterValues();
        var dropdownParams = params.dropdownParams, calendarParams = params.calendarParams;
        var obj = {
            "FromDate": calendarParams.fromDate,
            "ToDate": calendarParams.toDate,
            "Sport": dropdownParams[0].currentValue,
            "Leagues": dropdownParams[1].currentValue,
            "Matches": dropdownParams[2].currentValue,
            "BetTypes": dropdownParams[3].currentValue
        };
        if (this.isMY) {
            var isCustomerCurrency = dropdownParams[5].currentValue === "Customer Currency";
            obj.Currencies = dropdownParams[4].currentValue === "All" ? "" : dropdownParams[4].currentValue;
            obj.BaseCurrency = isCustomerCurrency ? "" : dropdownParams[5].currentValue;
            this.baseCurrency = isCustomerCurrency ? obj["Currencies"] : obj["BaseCurrency"];
        }
        return obj;
    };
    MatchWinLossReportDetail.prototype.submitSuccess = function (result) {
        if (result.errorCode) {
            Fanex.ExtAcc.UnBlockUI();
            Fanex.ExtAcc.Form.Alert("Error", result.errorMessage.formatErrorMessage(), 350, 150, "alertWLD");
            return;
        }
        try {
            if (result.Events.length || result.SystemParlayOutright.length) {
                this.gridComponent.hideNoInfo(true);
                this.renderGrid(result);
            }
            else {
                this.gridComponent.hideNoInfo(false);
            }
            Fanex.ExtAcc.UnBlockUI();
        }
        catch (e) {
            this.gridComponent.hideNoInfo(false);
            Fanex.ExtAcc.UnBlockUI();
        }
    };
    MatchWinLossReportDetail.prototype.initExportExcelBtn = function () {
        var isValid = this.validateParams();
        if (isValid) {
            $("#" + this.gridComponentConfig.excelBtnId).off().click(function () {
                var url = resolveClientUrl("MatchWinLossReports/ExportDetailReport");
                window.top.exportExcel(url, document);
            });
        }
    };
    MatchWinLossReportDetail.prototype.reportTitle = function () {
        var params = this.paramsState.getValue();
        var fromDate = moment(params.FromDate, 'MM/DD/YYYY');
        var toDate = moment(params.ToDate, 'MM/DD/YYYY');
        var formattedFromDate = fromDate.format('MMM DD, YYYY');
        var formattedToDate = toDate.format('MMM DD, YYYY');
        var dateTitle = fromDate.isSame(toDate)
            ? "on " + formattedFromDate
            : "from " + formattedFromDate + " to " + formattedToDate;
        var title = "Match Win Loss " + dateTitle;
        if (this.isMY) {
            var currency = params.Currencies;
            var baseCurrency = params.BaseCurrency;
            if (currency) {
                title += ", Currency: " + currency;
            }
            if (baseCurrency) {
                title += ", Base Currency: " + baseCurrency;
            }
        }
        $("#" + this.typeReport + "-grid-title").text(title);
    };
    MatchWinLossReportDetail.prototype.renderGrid = function (data) {
        var _this = this;
        try {
            var gridOptions = this.gridComponent.getGridOptions();
            this.reportTitle();
            var totalGrandTotalSystemParlayOutright = this.gridComponent.buildFooterRows(__assign({}, data, { currency: this.isMY ? this.baseCurrency : "" }));
            if (data.Events.length > 0) {
                gridOptions.bodyRows = this.gridComponent.buildBodyRows(__assign({}, data, { currency: this.isMY ? this.baseCurrency : "" }));
                gridOptions.footerRows = totalGrandTotalSystemParlayOutright;
            }
            else {
                gridOptions.bodyRows = totalGrandTotalSystemParlayOutright;
                gridOptions.footerRows = [];
            }
            gridOptions.pagerOption.pagerSelectCallBack = function (pageIndex, pageSize) {
                _this.cachePage = pageIndex + 1;
                _this.cachePageSize = pageSize;
                _this.handleClickPage({
                    page: _this.cachePage,
                    pageSize: _this.cachePageSize,
                });
            };
            gridOptions.pagerOption.currentPage = this.cachePage - 1;
            gridOptions.pagerOption.itemsPerPage = this.cachePageSize;
            gridOptions.pagerOption.totalItem = data.TotalItems;
            gridOptions.sortOption.serverSortFunction = function (colname, currentDirection) {
                _this.sortType = currentDirection === 'desc' ? 1 : 0;
                _this.sortBy = colname;
                _this.handleClickSort({
                    sortBy: _this.sortBy,
                    sortType: _this.sortType
                });
            };
            setPagerOptionOnMobile(gridOptions);
            this.gridComponent.setGridOptions(gridOptions);
            this.gridComponent.fanexGrid(gridOptions);
            workaroundForGridOnPhone();
        }
        catch (e) {
            console.log(e);
        }
    };
    MatchWinLossReportDetail.prototype.validateParams = function () {
        var params = this.filterComponents.getAllFilterValues();
        var dropdownParams = params.dropdownParams;
        var isValidLeague = dropdownParams.filter(function (e) { return e.name === "ddl-leagues"; })[0].currentValue.length > 0;
        if (!isValidLeague) {
            return this.byPassValidateLeague;
        }
        var isValidEvents = dropdownParams.filter(function (e) { return e.name === "ddl-events"; })[0].currentValue.length > 0;
        if (!isValidEvents) {
            return this.byPassValidateEvent;
        }
        var isValidBetType = dropdownParams.filter(function (e) { return e.name === "ddl-bet-types"; })[0].currentValue.length > 0;
        if (!isValidBetType) {
            return false;
        }
        return true;
    };
    MatchWinLossReportDetail.prototype.getLeaguesAndEvents = function (sportID, fromDate, toDate, callback) {
        Fanex.ExtAcc.BlockUI();
        var params = {
            sportId: sportID,
            fromDate: fromDate,
            toDate: toDate
        };
        $.ajax({
            url: resolveClientUrl("MatchWinLossReports/GetLeaguesAndEvents"),
            type: "post",
            dataType: "json",
            cache: false,
            data: params,
            success: function (result) {
                callback(result);
            }
        });
    };
    MatchWinLossReportDetail.prototype.getEvents = function (sportID, leagueIDs, fromDate) {
        var _this = this;
        var params = {
            sportId: sportID,
            leagueIds: leagueIDs,
            fromDate: fromDate
        };
        $.ajax({
            url: resolveClientUrl("MatchWinLossReports/GetEvents"),
            type: "post",
            dataType: "json",
            cache: false,
            data: params,
            success: function (result) {
                _this.initEventsDropdown(result);
            }
        });
    };
    return MatchWinLossReportDetail;
}());
$(document).ready(function () {
    new MatchWinLossReportDetail().init();
});
//# sourceMappingURL=MatchWinLossReportDetailNew.js.map
var __assign = (this && this.__assign) || Object.assign || function(t) {
    for (var s, i = 1, n = arguments.length; i < n; i++) {
        s = arguments[i];
        for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
            t[p] = s[p];
    }
    return t;
};
var MatchWinLossReportSummary = /** @class */ (function () {
    function MatchWinLossReportSummary() {
        this.typeReport = "summary";
        this.gridComponentConfig = {
            reportId: "summary",
            isMobile: isMobile,
            gridId: "summary-jqgrid",
            gridContainerId: "summary-mainGrid",
            selectColumnsId: "summary-ddl-columns",
            selectColumnsContainerId: "summary-ddl-columns-container",
            msgNoInfoId: "summary-msgNoInfo",
            excelBtnId: "summary-excel",
            pageSizeOptions: [500, 1000, 1500],
        };
        this.cachePageSize = this.gridComponentConfig.pageSizeOptions[0];
        this.cachePage = 1;
        this.sortBy = "";
        this.sortType = 0;
        this.baseCurrency = "";
        this.byPassLeagueValidate = false;
        this.paramsState = new StateManager({
            "FromDate": null,
            "ToDate": null,
            "Sport": null,
            "Leagues": null
        });
        this.datesState = new StateManager({ fromDate: null, toDate: null });
        this.sportState = new StateManager("");
        this.currencyState = new StateManager("All");
        this.filterComponents = new MatchWinLossFilter($);
        this.gridComponent = new SummaryGrid(this.gridComponentConfig);
        this.isMY = $("#inp-isMY").val().toLowerCase() === "true";
    }
    MatchWinLossReportSummary.prototype.init = function () {
        var _this = this;
        Fanex.ExtAcc.BlockUI();
        this.initDateRangePicker();
        this.initSportsDropdown(JSON.parse($("#inp-sports").val()));
        this.initLeaguesDropdown(JSON.parse($("#inp-leagues").val()));
        if (this.isMY) {
            this.initCurrencyDropDown(JSON.parse($("#inp-currencies").val()));
            this.initBaseCurrencyDropDown([
                { id: "MYR", value: "MYR", text: "MYR", selected: true },
                { id: "CNY", value: "CNY", text: "CNY", selected: false },
                { id: "THB", value: "THB", text: "THB", selected: false },
                { id: "IDR", value: "IDR", text: "IDR", selected: false }
            ]);
        }
        $("#" + this.typeReport + "-submit-btn").click(function () { return _this.handleClickSubmit(); });
        this.gridComponent.initSelectColumnsDropdown(SummaryGrid.DefaultColumns, [0, 1]);
        this.initExportExcelBtn();
        Fanex.ExtAcc.UnBlockUI();
    };
    MatchWinLossReportSummary.prototype.handleClickSubmit = function () {
        var _this = this;
        Fanex.ExtAcc.BlockUI();
        var isValid = this.validateParams();
        if (!isValid) {
            Fanex.ExtAcc.UnBlockUI();
            this.gridComponent.hideNoInfo(false);
        }
        else {
            var paramsData = this.getParams();
            paramsData.Page = 1;
            paramsData.PageSize = this.cachePageSize;
            paramsData.SortBy = "";
            paramsData.SortType = 0;
            this.paramsState.setValue(paramsData);
            $.ajax({
                url: resolveClientUrl("MatchWinLossReports/GetSummaryReport"),
                type: "post",
                dataType: "json",
                cache: false,
                data: paramsData,
                success: function (result) { return _this.submitSuccess(result); },
            });
        }
    };
    MatchWinLossReportSummary.prototype.handleClickSort = function (_a) {
        var _this = this;
        var sortBy = _a.sortBy, sortType = _a.sortType;
        Fanex.ExtAcc.BlockUI();
        var params = this.paramsState.getValue();
        params.SortBy = sortBy;
        params.SortType = sortType;
        this.paramsState.setValue(params);
        $.ajax({
            url: resolveClientUrl("MatchWinLossReports/GetSummaryReport"),
            type: "post",
            dataType: "json",
            cache: false,
            data: params,
            success: function (result) { return _this.submitSuccess(result); },
        });
    };
    MatchWinLossReportSummary.prototype.handleClickPage = function (_a) {
        var _this = this;
        var page = _a.page, pageSize = _a.pageSize;
        Fanex.ExtAcc.BlockUI();
        var params = this.paramsState.getValue();
        params.Page = page;
        params.PageSize = pageSize;
        this.paramsState.setValue(params);
        $.ajax({
            url: resolveClientUrl("MatchWinLossReports/GetSummaryReport"),
            type: "post",
            dataType: "json",
            cache: false,
            data: params,
            success: function (result) { return _this.submitSuccess(result); },
        });
    };
    MatchWinLossReportSummary.prototype.getLeagues = function (sportID, fromDate, toDate, callback) {
        var params = {
            sportId: sportID,
            fromDate: fromDate,
            toDate: toDate
        };
        $.ajax({
            url: resolveClientUrl("MatchWinLossReports/GetLeagues"),
            type: "post",
            dataType: "json",
            cache: false,
            data: params,
            success: function (leagues) {
                callback(leagues);
            }
        });
    };
    MatchWinLossReportSummary.prototype.submitSuccess = function (result) {
        if (result.errorCode) {
            Fanex.ExtAcc.UnBlockUI();
            Fanex.ExtAcc.Form.Alert("Error", result.errorMessage.formatErrorMessage(), 350, 150, "alertWLD");
            return;
        }
        try {
            if (result.Events.length || result.SystemParlayOutright.length) {
                this.gridComponent.hideNoInfo(true);
                this.renderGrid(result);
            }
            else {
                this.gridComponent.hideNoInfo(false);
            }
            Fanex.ExtAcc.UnBlockUI();
        }
        catch (e) {
            this.gridComponent.hideNoInfo(false);
            Fanex.ExtAcc.UnBlockUI();
        }
    };
    MatchWinLossReportSummary.prototype.initExportExcelBtn = function () {
        var isValid = this.validateParams();
        var _self = this;
        if (isValid) {
            $("#" + this.gridComponentConfig.excelBtnId).off().click(function () {
                var url = resolveClientUrl("MatchWinLossReports/ExportSummaryReport");
                window.top.exportExcel(url, document);
            });
        }
    };
    MatchWinLossReportSummary.prototype.renderGrid = function (data) {
        var _this = this;
        try {
            var gridOptions = this.gridComponent.getGridOptions();
            this.reportTitle();
            var totalGrandTotalSystemParlayOutright = this.gridComponent.buildFooterRows(__assign({}, data, { currency: this.isMY ? this.baseCurrency : "" }));
            if (data.Events.length > 0) {
                gridOptions.bodyRows = this.gridComponent.buildBodyRows(data);
                gridOptions.footerRows = totalGrandTotalSystemParlayOutright;
            }
            else {
                gridOptions.bodyRows = totalGrandTotalSystemParlayOutright;
                gridOptions.footerRows = [];
            }
            gridOptions.pagerOption.pagerSelectCallBack = function (pageIndex, pageSize) {
                _this.cachePage = pageIndex + 1;
                _this.cachePageSize = pageSize;
                _this.handleClickPage({
                    page: _this.cachePage,
                    pageSize: _this.cachePageSize,
                });
            };
            gridOptions.pagerOption.currentPage = this.cachePage - 1;
            gridOptions.pagerOption.itemsPerPage = this.cachePageSize;
            gridOptions.pagerOption.totalItem = data.TotalItems;
            gridOptions.sortOption.serverSortFunction = function (colname, currentDirection) {
                _this.sortType = currentDirection === 'desc' ? 1 : 0;
                _this.sortBy = colname;
                _this.handleClickSort({
                    sortBy: _this.sortBy,
                    sortType: _this.sortType
                });
            };
            setPagerOptionOnMobile(gridOptions);
            this.gridComponent.setGridOptions(gridOptions);
            this.gridComponent.fanexGrid(gridOptions);
            workaroundForGridOnPhone();
        }
        catch (e) {
            console.log(e);
        }
    };
    MatchWinLossReportSummary.prototype.reportTitle = function () {
        var params = this.paramsState.getValue();
        var fromDate = moment(params.FromDate, 'MM/DD/YYYY');
        var toDate = moment(params.ToDate, 'MM/DD/YYYY');
        var formattedFromDate = fromDate.format('MMM DD, YYYY');
        var formattedToDate = toDate.format('MMM DD, YYYY');
        var dateTitle = fromDate.isSame(toDate)
            ? "on " + formattedFromDate
            : "from " + formattedFromDate + " to " + formattedToDate;
        var title = "Match Win Loss " + dateTitle;
        if (this.isMY) {
            var currency = params.Currencies;
            var baseCurrency = params.BaseCurrency;
            if (currency) {
                title += ", Currency: " + currency;
            }
            if (baseCurrency) {
                title += ", Base Currency: " + baseCurrency;
            }
        }
        $("#" + this.typeReport + "-grid-title").text(title);
    };
    MatchWinLossReportSummary.prototype.initDateRangePicker = function () {
        var _self = this;
        this.filterComponents.renderCalendarPicker({
            selectorId: "#" + this.typeReport + "-date-range-picker",
            onClosed: function (value) {
                _self.datesState.setValue(value);
            }
        });
        this.initCalendarState();
    };
    MatchWinLossReportSummary.prototype.initCalendarState = function () {
        var _self = this;
        var initDate = this.filterComponents.getCalendarValues();
        _self.datesState.setValue(initDate);
        var updateLeagues = function (dateRange) {
            Fanex.ExtAcc.BlockUI();
            var fromDate = dateRange.fromDate, toDate = dateRange.toDate;
            _self.getLeagues(_self.sportState.getValue(), fromDate, toDate, function (leagues) {
                _self.initLeaguesDropdown(leagues);
            });
        };
        _self.datesState.subscribe(updateLeagues);
    };
    MatchWinLossReportSummary.prototype.validateParams = function () {
        var params = this.filterComponents.getAllFilterValues();
        var dropdownParams = params.dropdownParams;
        var isValidLeague = dropdownParams[1].currentValue.length > 0;
        if (!isValidLeague) {
            return this.byPassLeagueValidate;
        }
        return true;
    };
    MatchWinLossReportSummary.prototype.getParams = function () {
        var params = this.filterComponents.getAllFilterValues();
        var dropdownParams = params.dropdownParams, calendarParams = params.calendarParams;
        var obj = {
            "FromDate": calendarParams.fromDate,
            "ToDate": calendarParams.toDate,
            "Sport": dropdownParams[0].currentValue,
            "Leagues": dropdownParams[1].currentValue,
        };
        if (this.isMY) {
            var isCustomerCurrency = dropdownParams[3].currentValue === "Customer Currency";
            obj.Currencies = dropdownParams[2].currentValue === "All" ? "" : dropdownParams[2].currentValue;
            obj.BaseCurrency = isCustomerCurrency ? "" : dropdownParams[3].currentValue;
            this.baseCurrency = isCustomerCurrency ? obj["Currencies"] : obj["BaseCurrency"];
        }
        return obj;
    };
    MatchWinLossReportSummary.prototype.initSportsDropdown = function (sports) {
        if (!sports)
            return;
        var _self = this;
        var items = [];
        sports.forEach(function (e, i) {
            items.push({
                id: e.SportType,
                text: e.SportName,
                selected: i === 0,
            });
        });
        this.initSportState(sports[0].SportType);
        this.filterComponents.registerDropdownComponent({
            dropdownId: this.typeReport + "-ddl-sport",
            dropdownName: "ddl-sport",
            dropdownType: "select2",
            dropdownData: items,
            dropdownConfig: {
                noneSelectedText: "Select sport",
            },
            onClosed: function (sportID) {
                _self.sportState.setValue(sportID);
            }
        });
    };
    MatchWinLossReportSummary.prototype.initSportState = function (sportType) {
        var _self = this;
        _self.sportState.setValue(sportType);
        var updateLeagues = function (sportID) {
            Fanex.ExtAcc.BlockUI();
            var _a = _self.datesState.getValue(), fromDate = _a.fromDate, toDate = _a.toDate;
            _self.getLeagues(sportID, fromDate, toDate, function (leagues) {
                _self.initLeaguesDropdown(leagues);
            });
        };
        _self.sportState.subscribe(updateLeagues);
    };
    MatchWinLossReportSummary.prototype.initLeaguesDropdown = function (leagues) {
        this.byPassLeagueValidate = leagues.length === 0;
        var items = [];
        var maxLength = 0;
        leagues.forEach(function (e, i) {
            if ((e.LeagueName || "").length > maxLength) {
                maxLength = e.LeagueName.length;
            }
            items.push({
                id: e.LeagueId,
                value: e.LeagueId,
                text: e.LeagueName,
                checked: false,
                enable: true,
                items: []
            });
        });
        this.filterComponents.registerDropdownComponent({
            dropdownId: this.typeReport + "-ddl-leagues",
            dropdownName: "ddl-leagues",
            dropdownType: "fanexMultiSelectSearchable",
            dropdownData: items,
            dropdownConfig: {
                noneSelectedText: items.length ? "Select league" : "There is no League",
            },
            toggleWidth: isMobile.phone ? 243 : 193,
            minWidth: isMobile.phone ? 300 : 500,
        });
        Fanex.ExtAcc.UnBlockUI();
    };
    MatchWinLossReportSummary.prototype.initCurrencyDropDown = function (currencies) {
        if (!currencies)
            return;
        var _self = this;
        var items = [
            {
                id: "All",
                value: "All",
                text: "All",
                selected: false
            }
        ];
        currencies.forEach(function (e, i) {
            items.push({
                id: e.CurrencyName,
                value: e.CurrencyName,
                text: e.CurrencyName,
                selected: false
            });
        });
        this.initCurrencyState();
        this.filterComponents.registerDropdownComponent({
            dropdownId: this.typeReport + "-ddl-currencies",
            dropdownName: "ddl-currencies",
            dropdownType: "select2",
            dropdownData: items,
            onClosed: function (currencyName) {
                _self.currencyState.setValue(currencyName);
            }
        });
    };
    MatchWinLossReportSummary.prototype.initCurrencyState = function () {
        var _self = this;
        var updateBaseCurrency = function (currencyName) {
            Fanex.ExtAcc.BlockUI();
            var optionBaseCurrency = [];
            if (currencyName === "All") {
                optionBaseCurrency = [
                    { id: "MYR", value: "MYR", text: "MYR", selected: true },
                    { id: "CNY", value: "CNY", text: "CNY", selected: false },
                    { id: "THB", value: "THB", text: "THB", selected: false },
                    { id: "IDR", value: "IDR", text: "IDR", selected: false }
                ];
            }
            else {
                optionBaseCurrency = [
                    { id: "Customer Currency", value: "Customer Currency", text: "Customer Currency", selected: true },
                    { id: "MYR", value: "MYR", text: "MYR", selected: false },
                    { id: "CNY", value: "CNY", text: "CNY", selected: false },
                    { id: "THB", value: "THB", text: "THB", selected: false },
                    { id: "IDR", value: "IDR", text: "IDR", selected: false },
                ];
            }
            _self.initBaseCurrencyDropDown(optionBaseCurrency);
            Fanex.ExtAcc.UnBlockUI();
        };
        this.currencyState.subscribe(updateBaseCurrency);
    };
    MatchWinLossReportSummary.prototype.initBaseCurrencyDropDown = function (baseCurrencies) {
        if (!baseCurrencies)
            return;
        this.filterComponents.registerDropdownComponent({
            dropdownId: this.typeReport + "-ddl-base-currencies",
            dropdownName: "ddl-base-currencies",
            dropdownType: "select2",
            dropdownData: baseCurrencies
        });
    };
    return MatchWinLossReportSummary;
}());
$(document).ready(function () {
    new MatchWinLossReportSummary().init();
});
//# sourceMappingURL=MatchWinLossReportSummary.js.map