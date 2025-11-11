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
var RewardDetailFilter = /** @class */ (function (_super) {
    __extends(RewardDetailFilter, _super);
    function RewardDetailFilter() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.dateFormat = "MM/DD/YYYY";
        return _this;
    }
    RewardDetailFilter.prototype.renderCalendarPicker = function (options) {
        var _this = this;
        var dateRangePicker = $(options.selectorId);
        this.fromDate = dateRangePicker.data("fromdate");
        this.toDate = dateRangePicker.data("todate");
        dateRangePicker
            .dateRangePicker({
            format: this.dateFormat,
            separator: " - ",
            startDate: dateRangePicker.data("mindate"),
            endDate: dateRangePicker.data("maxdate"),
            showShortcuts: true,
            shortcuts: null,
            customShortcuts: this.getCalenderDefinedShortcuts(dateRangePicker),
            duration: 0,
            singleMonth: true,
            monthSelect: true,
            yearSelect: true
        }).bind("datepicker-change", function (_, obj) {
            _this.fromDate = moment(obj.date1).format(_this.dateFormat);
            _this.toDate = moment(obj.date2).format(_this.dateFormat);
        })
            .data("dateRangePicker").setDateRange(this.fromDate, this.toDate, false);
    };
    RewardDetailFilter.prototype.getCalenderDefinedShortcuts = function (dateRangePicker) {
        var today = moment(dateRangePicker.data("currentdate"), this.dateFormat);
        var todayValue = today.toDate();
        var thisWeekStartOfDay = moment(today).startOf("isoWeek");
        var lastMonthStartOfDay = moment(today).subtract(1, "M").startOf("M");
        return [
            {
                name: dateRangePicker.data("label-today"),
                dates: function () {
                    return [todayValue, todayValue];
                }
            },
            {
                name: dateRangePicker.data("label-yesterday"),
                dates: function () {
                    var yesterday = moment(today).subtract(1, "d");
                    var yesterdayValue = yesterday.toDate();
                    return [yesterdayValue, yesterdayValue];
                }
            },
            {
                name: dateRangePicker.data("label-thisweek"),
                dates: function () {
                    return [thisWeekStartOfDay.toDate(), todayValue];
                }
            },
            {
                name: dateRangePicker.data("label-lastweek"),
                dates: function () {
                    var lastWeekStartOfDay = moment(thisWeekStartOfDay).subtract(1, "w");
                    return [lastWeekStartOfDay.toDate(), moment(thisWeekStartOfDay).subtract(1, "d").toDate()];
                }
            },
            {
                name: dateRangePicker.data("label-thismonth"),
                dates: function () {
                    var thisMonthStartOfDay = moment(today).startOf("M");
                    return [thisMonthStartOfDay.toDate(), todayValue];
                }
            },
            {
                name: dateRangePicker.data("label-lastmonth"),
                dates: function () {
                    var lastMonthEndOfDay = moment(lastMonthStartOfDay).endOf("M").toDate();
                    return [lastMonthStartOfDay.toDate(), lastMonthEndOfDay];
                }
            },
            {
                name: dateRangePicker.data("label-sincelastmonth"),
                dates: function () {
                    return [lastMonthStartOfDay.toDate(), todayValue];
                }
            }
        ];
    };
    return RewardDetailFilter;
}(BaseReportFilters));
//# sourceMappingURL=RewardDetailFilter.js.map
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
var RewardDetailGrid = /** @class */ (function (_super) {
    __extends(RewardDetailGrid, _super);
    function RewardDetailGrid(options) {
        var _this = _super.call(this, options) || this;
        _this.gridOptions = {
            customizeHeader: true,
            columns: RewardDetailGrid.DefaultColumns,
            groupHeaderRows: [{ Columns: RewardDetailGrid.GroupHeaderColumns }],
            //numberOfFrozenColumn: 2,
            autoCalculateGridExpandHeight: false,
            fullHeight: !options.isMobile.phone,
            gridHeight: options.isMobile.phone ? $(window).height() - 70 : 700,
            showTitle: true,
            titleInfo: {
                customButtons: "<span id=" + options.copyBtnId + " title=\"Copy\" class=\"icon-copy\"></span><span id=" + options.excelBtnId + " title=\"Export Excel\" class=\"icon-excel\"></span>"
            },
            gridExpandHeight: 260,
            showPager: true,
            customPager: true,
            pagerOption: {
                itemsPerPageArray: options.pageSizeOptions,
                currentPage: 1,
                totalItem: 10,
                itemsPerPage: options.pageSizeOptions[0],
                pagerSelectCallBack: function (pageIndex, pageSize) {
                }
            },
            bottomBorderWidth: 0,
            showDistinctRow: false,
            allowTruncate: true,
            bodyRows: [],
            showFooter: true,
            footerRows: [],
            showHighlightRow: false
        };
        _this.initSelectColumnsDropdown(RewardDetailGrid.DefaultColumns, [0, 1, 2]);
        _this.initCopyBtn(options.copyBtnId);
        return _this;
    }
    RewardDetailGrid.sortDateTime = function (col1Value, col2Value, sortDirection) {
        var parsedCol1Value = moment(col1Value);
        var parsedCol2Value = moment(col2Value);
        if (sortDirection === 'asc') {
            return parsedCol1Value > parsedCol2Value ? 1 : -1;
        }
        else {
            return parsedCol1Value < parsedCol2Value ? 1 : -1;
        }
    };
    RewardDetailGrid.customUsernameCell = function (currentCell) {
        var _a = currentCell.Value, username = _a.username, isDirectMember = _a.isDirectMember;
        return isDirectMember ? username + "<span class='icon-directmember' title=\"Direct Member\"></span>" : username;
    };
    RewardDetailGrid.prototype.buildBodyRows = function (reportData) {
        var bodyData = [];
        function StatusColor(status) {
            return status ? "green" : "red";
        }
        if (reportData) {
            for (var _i = 0, reportData_1 = reportData; _i < reportData_1.length; _i++) {
                var element = reportData_1[_i];
                var CreatedDate = element.CreatedDate, ProgramName = element.ProgramName, MemberUsername = element.MemberUsername, MemberAmount = element.MemberAmount, MemberCurrency = element.MemberCurrency, MemberPaymentStatus = element.MemberPaymentStatus, MemberPaymentStatusMapped = element.MemberPaymentStatusMapped, MemberPaymentDateString = element.MemberPaymentDateString, MemberFeedBackMapped = element.MemberFeedBackMapped, AgentUsername = element.AgentUsername, AgentPaymentStatus = element.AgentPaymentStatus, AgentPaymentStatusMapped = element.AgentPaymentStatusMapped, AgentPaymentDateString = element.AgentPaymentDateString, MasterUsername = element.MasterUsername, MasterPaymentStatus = element.MasterPaymentStatus, MasterPaymentStatusMapped = element.MasterPaymentStatusMapped, MasterPaymentDateString = element.MasterPaymentDateString, SuperUsername = element.SuperUsername, SuperPaymentStatus = element.SuperPaymentStatus, SuperPaymentStatusMapped = element.SuperPaymentStatusMapped, SuperPaymentDateString = element.SuperPaymentDateString, Recommend = element.Recommend;
                bodyData.push({
                    Columns: [
                        { Name: "CreatedDate", Value: this.getDateTime(CreatedDate), CellAlign: "center" },
                        { Name: "ProgramName", Value: ProgramName },
                        { Name: "MemberUsername", Value: { username: MemberUsername, isDirectMember: Recommend == -3 || Recommend == -4 } },
                        { Name: "MemberAmount", Value: MemberAmount },
                        { Name: "MemberCurrency", Value: MemberCurrency, CellAlign: "center" },
                        { Name: "MemberPaymentStatus", Value: MemberPaymentStatusMapped, CellAlign: "center", CssClass: StatusColor(MemberPaymentStatus) },
                        { Name: "MemberPaymentDate", Value: MemberPaymentDateString, CellAlign: "center" },
                        { Name: "MemberFeedBack", Value: MemberFeedBackMapped, CellAlign: "center", CssClass: MemberFeedBackMapped === "Not Received" ? "red" : "" },
                        { Name: "AgentUsername", Value: AgentUsername },
                        { Name: "AgentPaymentStatus", Value: AgentPaymentStatusMapped, CellAlign: "center", CssClass: StatusColor(AgentPaymentStatus) },
                        { Name: "AgentPaymentDate", Value: AgentPaymentDateString, CellAlign: "center" },
                        { Name: "MasterUsername", Value: MasterUsername },
                        { Name: "MasterPaymentStatus", Value: MasterPaymentStatusMapped, CellAlign: "center", CssClass: StatusColor(MasterPaymentStatus) },
                        { Name: "MasterPaymentDate", Value: MasterPaymentDateString, CellAlign: "center" },
                        { Name: "SuperUsername", Value: SuperUsername },
                        { Name: "SuperPaymentStatus", Value: SuperPaymentStatusMapped, CellAlign: "center", CssClass: StatusColor(SuperPaymentStatus) },
                        { Name: "SuperPaymentDate", Value: SuperPaymentDateString, CellAlign: "center" }
                    ]
                });
            }
        }
        this.gridBodyRowsCache = bodyData;
        return bodyData;
    };
    RewardDetailGrid.prototype.buildFooterRows = function (reportData) {
        return undefined;
    };
    RewardDetailGrid.prototype.getDefaultColumns = function () {
        return RewardDetailGrid.DefaultColumns;
    };
    RewardDetailGrid.prototype.getGridOptions = function () {
        return this.gridOptions;
    };
    RewardDetailGrid.prototype.getGroupHeaderColumns = function () {
        return RewardDetailGrid.GroupHeaderColumns;
    };
    RewardDetailGrid.prototype.getDateTime = function (date) {
        if (!date)
            return "";
        return moment(date).format("MM/DD/YYYY hh:mm:ss A");
    };
    RewardDetailGrid.DefaultColumns = [
        { HeaderText: "Created Date", Name: "CreatedDate", Width: 170, Sort: true, HeaderAlign: "center", ColumnName2: "Created Date", CustomSortFunction: RewardDetailGrid.sortDateTime },
        { HeaderText: "Program", Name: "ProgramName", Width: 250, Sort: true, HeaderAlign: "center", AllowTruncate: true, ColumnName2: "Program" },
        { HeaderText: "Username", Name: "MemberUsername", Width: 180, Sort: true, HeaderAlign: "center", AllowTruncate: true, ColumnName2: "Member Username", CustomCellRender: RewardDetailGrid.customUsernameCell },
        { HeaderText: "Amount", Name: "MemberAmount", Width: 150, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Member Amount" },
        { HeaderText: "Reward Currency", Name: "MemberCurrency", Width: 140, Sort: true, HeaderAlign: "center", ColumnName2: "Member Reward Currency" },
        { HeaderText: "Payment Status", Name: "MemberPaymentStatus", Width: 120, Sort: true, HeaderAlign: "center", ColumnName2: "Member Payment Status" },
        { HeaderText: "Payment Date", Name: "MemberPaymentDate", Width: 110, Sort: true, HeaderAlign: "center", ColumnName2: "Member Payment Date" },
        { HeaderText: "Feedback", Name: "MemberFeedBack", Width: 110, Sort: true, HeaderAlign: "center", ColumnName2: "Member Feedback" },
        { HeaderText: "Username", Name: "AgentUsername", Width: 180, Sort: true, HeaderAlign: "center", AllowTruncate: true, ColumnName2: "Agent Username" },
        { HeaderText: "Payment Status", Name: "AgentPaymentStatus", Width: 120, Sort: true, HeaderAlign: "center", ColumnName2: "Agent Payment Status" },
        { HeaderText: "Payment Date", Name: "AgentPaymentDate", Width: 110, Sort: true, HeaderAlign: "center", ColumnName2: "Agent Payment Date" },
        { HeaderText: "Username", Name: "MasterUsername", Width: 180, Sort: true, HeaderAlign: "center", AllowTruncate: true, ColumnName2: "Master Username" },
        { HeaderText: "Payment Status", Name: "MasterPaymentStatus", Width: 120, Sort: true, HeaderAlign: "center", ColumnName2: "Master Payment Status" },
        { HeaderText: "Payment Date", Name: "MasterPaymentDate", Width: 110, Sort: true, HeaderAlign: "center", ColumnName2: "Master Payment Date" },
        { HeaderText: "Username", Name: "SuperUsername", Width: 180, Sort: true, HeaderAlign: "center", AllowTruncate: true, ColumnName2: "Super Username" },
        { HeaderText: "Payment Status", Name: "SuperPaymentStatus", Width: 120, Sort: true, HeaderAlign: "center", ColumnName2: "Super Payment Status" },
        { HeaderText: "Payment Date", Name: "SuperPaymentDate", Width: 110, Sort: true, HeaderAlign: "center", ColumnName2: "Super Payment Date" },
    ];
    RewardDetailGrid.GroupHeaderColumns = [
        { HeaderText: "Created Date", Name: "CreatedDate", MergeRowNumber: 2 },
        { HeaderText: "Program", Name: "ProgramName", MergeRowNumber: 2 },
        { HeaderText: "Member", Name: "MemberUsername", MergeColNumber: 6 },
        { HeaderText: "Agent", Name: "AgentUsername", MergeColNumber: 3 },
        { HeaderText: "Master", Name: "MasterUsername", MergeColNumber: 3 },
        { HeaderText: "Super", Name: "SuperUsername", MergeColNumber: 3 },
    ];
    return RewardDetailGrid;
}(BaseReportGrid));
//# sourceMappingURL=RewardDetailGrid.js.map
var RewardDetail = /** @class */ (function () {
    function RewardDetail() {
        this.typeReport = "reward-detail";
        this.cachePageSize = 10;
        this.cachePage = 1;
        this.gridComponentConfig = {
            reportId: "reward-detail",
            isMobile: isMobile,
            gridId: "reward-detail-jqgrid",
            gridContainerId: "reward-detail-mainGrid",
            selectColumnsId: "reward-detail-ddl-columns",
            selectColumnsContainerId: "reward-detail-ddl-columns-container",
            msgNoInfoId: "reward-detail-msgNoInfo",
            excelBtnId: "reward-detail-excel",
            copyBtnId: "reward-detail-copy",
            pageSizeOptions: [100, 200, 500, 1000]
        };
        this.filterComponents = new RewardDetailFilter($);
        this.gridComponent = new RewardDetailGrid(this.gridComponentConfig);
    }
    RewardDetail.prototype.init = function () {
        this.initDateRangePicker();
        this.initProgramDropdown(JSON.parse($("#inp-programs").val()));
        this.initPaymentStatusDropdown();
        this.initUsernameInput();
        this.initSubmitBtn();
    };
    RewardDetail.prototype.initDateRangePicker = function () {
        this.filterComponents.renderCalendarPicker({
            selectorId: "#" + this.typeReport + "-date-range-picker"
        });
    };
    RewardDetail.prototype.initProgramDropdown = function (programs) {
        var maxLength = 0;
        var mappedDataDropDown = programs.map(function (e) {
            if (e.ProgramName.length > maxLength) {
                maxLength = e.ProgramName.length;
            }
            return {
                id: e.ProgramId,
                value: e.ProgramId,
                text: e.ProgramName,
                checked: true,
                enable: true,
            };
        });
        maxLength *= 11;
        this.filterComponents.registerDropdownComponent({
            dropdownId: this.typeReport + "-ddl-programs",
            dropdownName: "programs",
            dropdownData: mappedDataDropDown,
            minWidth: maxLength > 200 ? maxLength : 200,
            dropdownType: "fanexMultiSelect"
        });
    };
    RewardDetail.prototype.initPaymentStatusDropdown = function () {
        this.filterComponents.registerDropdownComponent({
            dropdownId: this.typeReport + "-ddl-payment-status",
            dropdownName: "payment-status",
            dropdownType: "combobox"
        });
    };
    RewardDetail.prototype.initUsernameInput = function () {
        var usernameInput = {
            inputName: "username",
            inputId: this.typeReport + "-username-filter",
        };
        this.filterComponents.registerInputComponent(usernameInput);
        $("#" + usernameInput.inputId).keyup(function (e) {
            $(e.target).val().length > 1 && Fanex.ExtAcc.InitSuggestion("" + usernameInput.inputId);
        });
    };
    RewardDetail.prototype.submitSuccess = function (result) {
        if (result.errorCode) {
            Fanex.ExtAcc.UnBlockUI();
            Fanex.ExtAcc.Form.Alert("Error", result.errorMessage.formatErrorMessage(), 350, 150, "alertWLD");
            return;
        }
        if (result.Items.length) {
            this.gridComponent.hideNoInfo(true);
            this.renderGrid(result);
            this.initExportExcelBtn();
        }
        else {
            this.gridComponent.hideNoInfo(false);
        }
        Fanex.ExtAcc.UnBlockUI();
    };
    RewardDetail.prototype.clickSubmit = function (page, pageSize) {
        var _this = this;
        this.cachePage = page || this.cachePage;
        this.cachePageSize = pageSize || this.cachePageSize;
        Fanex.ExtAcc.BlockUI();
        var isValidProgram = this.validateProgramDDL();
        if (this.filterComponents.isUnValidInputs() || !isValidProgram) {
            Fanex.ExtAcc.UnBlockUI();
            this.gridComponent.hideNoInfo(false);
        }
        else {
            $.ajax({
                url: resolveClientUrl("RewardManagement/GetReport" + this.getQueryString(page, pageSize)),
                type: "post",
                dataType: "json",
                cache: false,
                success: function (result) { return _this.submitSuccess(result); }
            });
        }
    };
    RewardDetail.prototype.validateProgramDDL = function () {
        var params = this.filterComponents.getAllFilterValues();
        var dropdownParams = params.dropdownParams;
        return !!dropdownParams.filter(function (e) { return e.name === "programs"; })[0].currentValue.length;
    };
    RewardDetail.prototype.initSubmitBtn = function () {
        var _this = this;
        $("#" + this.typeReport + "-submit-btn").click(function () { return _this.clickSubmit(1, _this.gridComponentConfig.pageSizeOptions[0]); });
    };
    RewardDetail.prototype.initExportExcelBtn = function () {
        var _this = this;
        $("#" + this.gridComponentConfig.excelBtnId).off().click(function () {
            var url = resolveClientUrl("RewardManagement/ExportRewardDetail" + _this.getQueryString());
            window.top.exportExcel(url, document);
        });
    };
    RewardDetail.prototype.getQueryString = function (page, pageSize) {
        var params = this.filterComponents.getAllFilterValues();
        var inputParams = params.inputParams, dropdownParams = params.dropdownParams, calendarParams = params.calendarParams;
        return [
            '?FromDate=', calendarParams.fromDate,
            '&ToDate=', calendarParams.toDate,
            '&Page=', page,
            '&PageSize=', pageSize,
            '&ProgramIds=', dropdownParams[0].currentValue,
            '&MemberPaymentStatus=', dropdownParams[1].currentValue - 1,
            '&Username=', inputParams[0].currentValue,
        ].join('');
    };
    RewardDetail.prototype.renderGrid = function (data) {
        var _this = this;
        try {
            var gridOptions = this.gridComponent.getGridOptions();
            gridOptions.titleInfo.text = this.reportTitle();
            gridOptions.bodyRows = this.gridComponent.buildBodyRows(data.Items);
            gridOptions.footerRows = this.gridComponent.buildFooterRows(data.RowTotal);
            gridOptions.pagerOption.pagerSelectCallBack = function (pageIndex, pageSize) {
                _this.cachePage = pageIndex + 1;
                _this.cachePageSize = pageSize;
                _this.clickSubmit(_this.cachePage, _this.cachePageSize);
            };
            gridOptions.pagerOption.currentPage = this.cachePage - 1;
            gridOptions.pagerOption.itemsPerPage = this.cachePageSize;
            gridOptions.pagerOption.totalItem = data.TotalItems;
            setPagerOptionOnMobile(gridOptions);
            this.gridComponent.setGridOptions(gridOptions);
            this.gridComponent.fanexGrid(gridOptions);
            workaroundForGridOnPhone();
        }
        catch (e) {
            console.log(e);
        }
    };
    RewardDetail.prototype.reportTitle = function () {
        var _a = this.filterComponents.getCalendarValues(), fromDate = _a.fromDate, toDate = _a.toDate;
        var formatedFromDate = moment(fromDate, 'MM/DD/YYYY').format('MMM DD, YYYY');
        var timeInfo = fromDate === toDate
            ? "on " + formatedFromDate
            : "from " + formatedFromDate + " to " + moment(toDate, 'MM/DD/YYYY').format('MMM DD, YYYY');
        return "Reward Detail Report " + timeInfo;
    };
    return RewardDetail;
}());
$(function () {
    new RewardDetail().init();
});
//# sourceMappingURL=RewardDetail.js.map