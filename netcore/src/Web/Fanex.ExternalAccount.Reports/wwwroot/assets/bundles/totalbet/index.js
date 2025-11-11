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

// --------------------------------------------------------------------------
// <copyright file="fanex.MultiSelect.js">
// Copyright (c). All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
'use strict';
(function ($) {

    //// Method expose for user use
    var fanexSingleSelectMethods =
        {
            createfanexSingleSelect: function (opts) {
                opts = $.extend({}, $.fn.fanexSingleSelect.defaults, opts);
                return this.each(function () {
                    return new fanex.MultiSelect(this, opts);
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
    $.fn.fanexSingleSelect = function (options) {
        if (fanexSingleSelectMethods[options]) {
            return fanexSingleSelectMethods[options].apply(this, Array.prototype.slice.call(arguments, 1));
        }
        else if (typeof options === 'object' || !options) {
            return fanexSingleSelectMethods.createfanexSingleSelect.apply(this, arguments);
        }
        else {
            throw 'Method does not supported!';
        }
    };

    //// Default Options
    $.fn.fanexSingleSelect.defaults =
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
            onLeave: function () { },
            onCheck: function () { },
            onOpen: function () { },
            onCheckAll: function () { },
            onFooterButtonClick: function () { }
        };

    var fanex = fanex || {};

    fanex.MultiSelect = function (multiSelect, options) {
        var me = this;
        me.options = options;
        me.id = multiSelect.id;
        me.$multiSelect = $(multiSelect);
        me.$original = $(multiSelect);

        me.initialize();

        return me.$multiSelect;
    };

    fanex.MultiSelect.prototype =
        {
            getjQueryInstance: function () {
                return this.$multiSelect;
            },
            initialize: function () {
                var me = this,
                    html = [],
                    options = me.options,
                    id = me.id;
                me.browser = fanex.MultiSelect.helper.detectBrowser();
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
                        + '" name="radio-' + me.id
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
    fanex.MultiSelect.helper =
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

    fanex.MultiSelect.list = fanex.MultiSelect.list || {};

    fanex.MultiSelect.constant = fanex.MultiSelect.constant || {};


})(jQuery);

(function ($) {
    $.widget("ui.combobox", {
        _create: function () {
            var self = this;
            var select = this.element.hide();
            var wasOpen = true; 
            var input = $("<input>")
				.insertAfter(select)
				.autocomplete({
                    source: function (request, response) {
                        var matcher;

                        try {
                             matcher = new RegExp(request.term, "i");
                        } catch (e) {
                            matcher = new RegExp(request.term.replace(/[-\/\\^$*+?.()|[\]{}]/g, '\\$&'), "i");
                        }

				        response(select.children("option").map(function () {
				            var text = $(this).text();
				            if (this.value && (!request.term || matcher.test(text)))
				                return {
				                    id: this.value,
				                    label: text, //text.replace(new RegExp("(?![^&;]+;)(?!<[^<>]*)(" + $.ui.autocomplete.escapeRegex(request.term) + ")(?![^<>]*>)(?![^&;]+;)", "gi"), "<strong>$1</strong>"),
				                    value: text
				                };
				        }));
				    },
				    delay: 0,
				    select: function (event, ui) {
				        if (!ui.item) {
				            // remove invalid value, as it didn't match anything
				            $(this).val("");
				            return false;
				        }
				        select.val(ui.item.id);
				        self._trigger("selected", event, {
				            item: select.find("[value='" + ui.item.id + "']")
				        });
				    },
				    minLength: 0
				})
				.addClass("ui-widget ui-widget-content ui-corner-left");
            $("<button type='button'>▾</button>")
			.attr("tabIndex", -1)
			.attr("title", "Show All Items")
			.insertAfter(input)
			.button({
			    icons: {
			        primary: "ui-icon-triangle-1-s"
			    },
			    text: false
			}).removeClass("ui-corner-all")
			.addClass("ui-corner-right ui-button-icon")
			.mousedown(function () {
			    wasOpen = input.autocomplete("widget").is(":visible");
			})
            .mouseenter(function () {
                wasOpen = input.autocomplete("widget").is(":visible");
            })
            .click(function () {
                input.focus();
                // Close if already visible
                if (wasOpen) {
                    return;
                }
                // Pass empty string as value to search for, displaying all results
                input.autocomplete("search", "");
            });
        }
    });

})(jQuery);
var AutoRefresh = {
    refreshTime: 0,
    currentTime: 0,
    refreshAction: null,
    isInit: false,
    noRefresh: 'No refresh',
    init: function (refreshAction) {
        AutoRefresh.refreshAction = refreshAction;

        $(".div-down").click(function () { AutoRefresh.openOptions(); });
        $(".div-refresh").click(function () { AutoRefresh.doRefresh(); });
        $(".count-down-item").click(function () {
            var time = $(this).attr('time');
            AutoRefresh.saveOption(parseInt(time, 0));
        });

        var defaultTime = 15;
        AutoRefresh.saveOption(defaultTime);
        AutoRefresh.runCountDown();
        AutoRefresh.isInit = true;

        var $this = $(this);
        $(document).delegate('html', 'click', function (e) {
            var $target = $(e.target);

            if (!$target.closest('.div-count-down').length && !$(".panel-options").hasClass('hide')) {
                AutoRefresh.openOptions();
            }
        });
    },
    openOptions: function () {
        var isHide = $(".panel-options").hasClass('hide');
        if (isHide) {
            $(".panel-options").removeClass('hide');
        } else {
            $(".panel-options").addClass('hide');
        }
    },
    saveOption: function (time) {

        $(".panel-options").addClass('hide');
        $(".count-down-item").removeClass('menuitemcheck');
        $(".count-down-item[time='" + time + "']").addClass('menuitemcheck');

        var txt = time || AutoRefresh.noRefresh;
        $(".div-refresh").html(txt);

        AutoRefresh.refreshTime = time;
        AutoRefresh.currentTime = time;

        if (time === 0) {
            AutoRefresh.stop();
        } else {
            AutoRefresh.runCountDown();
        }
    },
    runCountDown: function () {
        $(".div-refresh").html(AutoRefresh.currentTime);

        AutoRefresh.currentTime--;

        if (window.top.countDownTimeout != null) {
            clearTimeout(window.top.countDownTimeout);
        }

        window.top.countDownTimeout = setTimeout(AutoRefresh.runCountDown, 1000);

        if (AutoRefresh.currentTime == -1) {
            AutoRefresh.doRefresh();
        }
    },
    doRefresh: function () {
        if (window.top.countDownTimeout != null) {
            clearTimeout(window.top.countDownTimeout);
        }

        $(".div-refresh").html("<span class='miniloading'>Loading</span>");

        if (AutoRefresh.refreshAction) {
            AutoRefresh.refreshAction(AutoRefresh.resetTime);
        } else {
            AutoRefresh.resetTime();
        }
    },
    resetTime: function () {
        if (AutoRefresh.refreshTime) {
            AutoRefresh.currentTime = AutoRefresh.refreshTime;
            AutoRefresh.runCountDown();
        } else {
            $(".div-refresh").html(AutoRefresh.noRefresh);
        }
    },
    stop: function () {
        if (window.top.countDownTimeout != null) {
            clearTimeout(window.top.countDownTimeout);
        }
    }
}; 
if (typeof Fanex == "undefined" || typeof Fanex.ExtAcc == "undefined") {
    alert("Fanex.ExtAcc.js module is required!");
}

Fanex.ExtAcc.TotalBet = {
    hideFilter: false,
    filters: null,
    leagueName: "Please select",
    viewBy: "Headline",
    Init: function () {
        var $this = this;
        $(".icon-filter").click(this.showHideFilter);
        this.initLeaguesDropdown();

        this.initCombobox("#ddl-view-by", function () { Fanex.ExtAcc.TotalBet.viewBy = $("#ddl-view-by option:selected").text(); });

        this.initEventsDropdown([]);

        $("#Submit").click(function (e) { $this.getReport(); });
        $(".btn-refresh").click(function (e) { $this.refresh(); });
    },
    showHideFilter: function () {
        if ($this.hideFilter) {
            $(".icon-filter").addClass("active");
            $(".filterbox").removeClass("hide");
        } else {
            $(".icon-filter").removeClass("active");
            $(".filterbox").addClass("hide");
        }

        $this.hideFilter = !$this.hideFilter;
    },
    initCombobox: function (query, onChangeEvent) {
        var _this = this;
        onChangeEvent = onChangeEvent || function () { };
        var $dropdownList = $(query).combobox({ selected: onChangeEvent });

        var inp = $dropdownList.next("input.ui-autocomplete-input");
        inp.val($dropdownList.find("option:selected").text())
            .attr("readonly", "true")
            .css("width", "120px")
            .bind("click", function (event) { return $(event.currentTarget).autocomplete("search", ""); });
    },
    getEvents: function () {
        var leagueIds = $("#ddl-leagues").fanexMultiSelectSearchable("getItems", true);

        Fanex.ExtAcc.BlockUI();

        $.ajax({
            context: this,
            url: resolveClientUrl("TotalBet/GetEvents"),
            type: "POST",
            data: { leagueIds: leagueIds.join(",") },
            dataType: "json",
            cache: false,
            success: function (result) {
                this.initEventsDropdown(result);
            },
            error: function () { },
            complete: function () { Fanex.ExtAcc.UnBlockUI(); }
        });
    },
    initLeaguesDropdown: function () {
        var leagues = JSON.parse($("#inp-leagues").val());
        var $ddl = $("#ddl-leagues");
        var items = [];
        leagues.forEach(function (e, i) {
            items.push({
                id: e.LeagueId,
                value: e.LeagueId,
                text: e.LeagueName,
                checked: false,
                enable: true,
                items: []
            })
        });

        var $this = this;
        $("#ddl-leagues").fanexMultiSelectSearchable({
            showFilter: true,
            toggleWidth: isMobile.phone ? 243 : 193,
            minWidth: isMobile.phone ? 300 : 500,
            checkAllText: "Select all",
            noneSelectedText: "Select Leagues",
            useCustomCheckAllText: false,
            customCheckAllText: "All Leagues",
            isCheckAll: false,
            listData: items,
            multiple: true,
            onCheck: function () { $this.getEvents(); }
        });
    },
    initEventsDropdown: function (events) {
        var $ddl = $("#ddl-events");
        var items = [];
        events.forEach(function (e, i) {
            items.push({
                id: e.MatchId,
                value: e.MatchId,
                text: e.MatchName,
                checked: true,
                enable: true,
                items: []
            })
        });

        $("#ddl-events").fanexMultiSelectSearchable({
            showFilter: true,
            toggleWidth: isMobile.phone ? 243 : 193,
            minWidth: isMobile.phone ? 300 : 500,
            checkAllText: "Select all",
            noneSelectedText: "Select Events",
            useCustomCheckAllText: true,
            customCheckAllText: "All Events",
            isCheckAll: true,
            listData: items,
            onCheck: function () { }
        });
    },
    getReport: function () {
        Fanex.ExtAcc.BlockUI();

        var viewMode = $("#ddl-view-by").val();
        var leagues = $("#ddl-leagues").fanexMultiSelectSearchable("getItems", true);
        var events = $("#ddl-events").fanexMultiSelectSearchable("getItems", true);
        var param = {
            viewMode: viewMode,
            leagues: leagues,
            events: events
        };

        if (!Fanex.ExtAcc.TotalBet.validateParams(param)) {
            Fanex.ExtAcc.UnBlockUI();
            return;
        }

        Fanex.ExtAcc.TotalBet.filters = param;

        if (AutoRefresh.isInit) {
            AutoRefresh.stop();
        }

        $.ajax({
            context: this,
            url: resolveClientUrl("TotalBet/GetData"),
            type: "POST",
            data: param,
            dataType: "json",
            cache: false,
            success: function (result) {
                Fanex.ExtAcc.TotalBet.renderReport(result);

                if (AutoRefresh.isInit) {
                    AutoRefresh.resetTime();
                } else {
                    AutoRefresh.init(Fanex.ExtAcc.TotalBet.refresh);
                }
            },
            error: function () { },
            complete: function () { Fanex.ExtAcc.UnBlockUI(); }
        });
    },
    refresh: function (callback) {
        if ($('.tbl-total-bet').length === 0 && window.top.countDownTimeout != null) {
            clearTimeout(window.top.countDownTimeout);
            return;
        }

        if (!Fanex.ExtAcc.TotalBet.filters) {
            return;
        }

        $.ajax({
            context: this,
            url: resolveClientUrl("TotalBet/GetData"),
            type: "POST",
            data: Fanex.ExtAcc.TotalBet.filters,
            dataType: "json",
            cache: false,
            success: function (result) {
                Fanex.ExtAcc.TotalBet.renderReport(result);

                if (callback) {
                    callback();
                }
            },
            error: function () { }
        });
    },
    renderReport: function (report) {
        var headers = [
            {
                Columns: [
                    { HeaderText: "Time", Name: "Time", MergeRowNumber: 2, HeaderAlign: "center" },
                    { HeaderText: "Event", Name: "Event", MergeRowNumber: 2 },
                    { HeaderText: "Full Time", Name: "FullTime_HDP", MergeColNumber: 2, ThClass: 'col-full-time' },
                    { HeaderText: "1st Half", Name: "_1stHalf_HDP", MergeColNumber: 2, ThClass: 'col-1st-half' }
                ]
            }
        ];

        var arrColData = [
            { HeaderText: "Time", Name: "Time", Width: 130, HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Event", Name: "Event", Width: 400 },
            { HeaderText: "Handicap", Name: "FullTime_HDP", Width: 130, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0", FormatNegative: "$" },
            { HeaderText: "Over/Under", Name: "FullTime_OU", Width: 130, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0", FormatNegative: "$" },
            { HeaderText: "Handicap", Name: "_1stHalf_HDP", Width: 130, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0", FormatNegative: "$" },
            { HeaderText: "Over/Under", Name: "_1stHalf_OU", Width: 130, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0", FormatNegative: "$" }];

        var bodyData = [];
        report.forEach(function (league, il) {
            bodyData.push({
                Columns: [
                    { Name: "Time", TdClass: 'row-league-title', CellAlign: "left", Value: league.LeagueName, MergeColNumber: 6 },
                ]
            });

            league.Events.forEach(function (v, i) {
                var isLive = v.LiveIndicator == 2;
                var rowClass = isLive ? 'row-live-match' : '';
                var expandButton = isLive && v.IsLatest && v.TotalScore > 0
                    ? ' <span class="tgg-score icon-plus" league-id="' + league.LeagueId + '" match-id="' + v.MatchId + '" collapsed="1"></span>'
                    : '';
                var time = isLive
                    ? '<span class="match-score">Live!</span> ' + v.LiveHomeScore + '-' + v.LiveAwayScore + expandButton
                    : v.ShowTime;
                var event = v.HomeName + (v.IsNeutral ? ' (N)' : '') + " -vs- " + v.AwayName;

                bodyData.push({
                    CssClass: isLive && !v.IsLatest ? 'hide' : '',
                    CustomAttr: 'league-id="' + league.LeagueId + '" match-id="' + v.MatchId + '" latest="' + (v.IsLatest ? 1 : 0) + '"' + (isLive ? ' live="1"' : ''),
                    Columns: [
                        { Name: "Time", TdClass: rowClass, CustomCellRender: function () { return time; } },
                        { Name: "Event", TdClass: rowClass, Value: event },
                        { Name: "FullTime_HDP", TdClass: rowClass, Value: v.Show_FullTime_HDP ? v.HDPFT : '' },
                        { Name: "FullTime_OU", TdClass: rowClass, Value: v.Show_FullTime_OU ? v.OUFT : '' },
                        { Name: "_1stHalf_HDP", TdClass: rowClass, Value: v.Show_1stHalf_HDP ? v.HDPHT : '' },
                        { Name: "_1stHalf_OU", TdClass: rowClass, Value: v.Show_1stHalf_OU ? v.OUHT : '' },
                    ]
                });
            });
        });


        var gridOption = {
            customizeHeader: true,
            groupHeaderRows: headers,
            columns: arrColData,
            numberOfFrozenColumn: 0,
            autoCalculateGridExpandHeight: true,
            fullHeight: !isMobile.phone,
            gridHeight: isMobile.phone ? $(window).height() - 70 : 0,
            showFooter: false,
            gridExpandHeight: 0,
            marginRight: 10,
            bottomBorderWidth: 0,
            showDistinctRow: false,
            allowTruncate: true,
            numOfPaddingForGridExpandHeight: 0,
            bodyRows: bodyData
        };

        $("#jqgrid").fanexGrid(gridOption);
        workaroundForGridOnPhone();

        $(".tgg-score").click(function () {
            var $this = $(this);
            var leagueId = $this.attr('league-id');
            var matchId = $this.attr('match-id');
            var isCollapse = $this.attr('collapsed');
            if (isCollapse == '1') {
                $this.attr('collapsed', '0');
                $("tr[league-id='" + leagueId + "'][match-id='" + matchId + "'][live='1'][latest='0']").removeClass('hide');
                $this.removeClass('icon-plus').addClass('icon-minus');
            } else {
                $this.attr('collapsed', '1');
                $("tr[league-id='" + leagueId + "'][match-id='" + matchId + "'][live='1'][latest='0']").addClass('hide');
                $this.removeClass('icon-minus').addClass('icon-plus');
            }

            $("#jqgrid").fanexGrid('resizeGrid');
        })
    },
    validateParams: function (param) {
        if (param.leagues == '') {
            Validators.show($("#ddl-leagues")[0], 'Required field!', undefined, undefined, undefined, undefined, undefined, 3000);

            return false;
        }

        if (param.events == '') {
            Validators.show($("#ddl-events")[0], 'Required field!', undefined, undefined, undefined, undefined, undefined, 3000);
            return false;
        }

        return true;
    }
};

$(document).ready(function () {
    if (window.top.countDownTimeout != null) {
        clearTimeout(window.top.countDownTimeout);
    }

    Fanex.ExtAcc.TotalBet.Init();
});