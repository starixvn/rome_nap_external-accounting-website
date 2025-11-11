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

var NexDatePickerHelper = /** @class */ (function () {
    function NexDatePickerHelper() {
    }
    NexDatePickerHelper.prototype.GetShortDateString = function (date) {
        // month is 0 based (0 - 11)
        var strDate = this.GetDateStr(date.getMonth() + 1) + "/";
        strDate += this.GetDateStr(date.getDate()) + "/";
        strDate += this.GetDateStr(date.getFullYear());
        return strDate;
    };
    NexDatePickerHelper.prototype.GetDateStr = function (date) {
        if (!date) {
            return "";
        }
        if (date < 10) {
            return "0" + date.toString();
        }
        return date.toString();
    };
    NexDatePickerHelper.prototype.GetBaseDateOfWeek = function (date) {
        // Base date from Monday
        var day = date.getDay(); // Sun:0 -> Sat:6
        return day === 0 // Sun:0: default of system
            ? date.getDate() - 6 // Sunday<-Mon: 6
            : date.getDate() - day + 1; // other days<-Mon: date.getDay() - 1
    };
    NexDatePickerHelper.prototype.GetDateRangeOfCurrentWeek = function (today) {
        var date = new Date(today);
        var lastDate = new Date(today);
        var baseDate = this.GetBaseDateOfWeek(date);
        var startDate = new Date(date.setDate(baseDate));
        return [startDate, lastDate];
    };
    NexDatePickerHelper.prototype.GetDateRangeOfLastWeek = function (today) {
        var baseDateOfCurrentWeek = this.GetBaseDateOfWeek(new Date(today)); // Monday
        var baseDateOfLastWeek = baseDateOfCurrentWeek - 7; // Seek 7 days
        var lastWeek = new Date(today);
        lastWeek.setDate(baseDateOfLastWeek);
        var startDate = this.GetShortDateString(lastWeek);
        lastWeek.setDate(lastWeek.getDate() + 6); // Seek 6 days: Monday->Sunday
        var endDate = this.GetShortDateString(lastWeek);
        return [new Date(startDate), new Date(endDate)];
    };
    return NexDatePickerHelper;
}());
var DateTimeControls = /** @class */ (function () {
    function DateTimeControls($) {
        this.$ = $;
    }
    DateTimeControls.prototype.Init = function () {
        var _this = this;
        this.$("#DateList").combobox({
            selected: function (event) { return _this.DateRangeSelected(event.target.selectedIndex); }
        });
        this.$("#dtpFromDate").datepicker({
            onClose: function () { _this.$("#userGroup input").val(_this.$("#DateList option:last").val()); },
            showOn: "both",
            buttonImageOnly: true,
            buttonImage: resolveClientUrl("Content/Images/calendar.png"),
            buttonText: "",
            beforeShow: this.SetRange,
            dateFormat: "M dd, yy"
        });
        this.$("#dtpFromDate").val(this.$.datepicker.formatDate("M dd, yy", new Date($("#MaxDate").val())));
        this.$("#dtpToDate").datepicker({
            onClose: function () { _this.$("#userGroup input").val(_this.$("#DateList option:last").val()); },
            showOn: "both",
            buttonImageOnly: true,
            buttonImage: resolveClientUrl("Content/Images/calendar.png"),
            buttonText: "",
            beforeShow: this.SetRange,
            dateFormat: "M dd, yy"
        });
        this.$("#dtpToDate").val(this.$.datepicker.formatDate("M dd, yy", new Date(this.$("#MaxDate").val())));
        this.$(".ui-datepicker").draggable();
        var optionSelected = this.$("#DateList option:selected");
        var optionSelectedIndex = optionSelected.index();
        this.$("#userGroup input").val(optionSelected.val());
        optionSelectedIndex > 0 && this.DateRangeSelected(optionSelectedIndex);
        this.InitMonthlyCalendar();
    };
    DateTimeControls.prototype.GetFromDateToDate = function () {
        return this.$(".from-to-date").css('display') !== 'none' ?
            {
                fromdate: this.$("#dtpFromDate").val(),
                todate: this.$("#dtpToDate").val()
            } :
            {
                fromdate: this.$("#monPickr1").monPickr("getFromDate", "MM/dd/yyyy"),
                todate: this.$("#monPickr1").monPickr("getToDate", "MM/dd/yyyy")
            };
    };
    DateTimeControls.prototype.InitMonthlyCalendar = function () {
        this.$("#monPickr1").monPickr({
            today: this.$("#MaxDate").val(),
            minDate: "2010/1/1",
            showDateRange: true,
            fromDayId: "fromDay1",
            toDayId: "toDay1"
        });
        var today = new Date(this.$("#MaxDate").val());
        this.$("#monPickr1").monPickr("setMonth", today);
        this.$("#monPickr1").monPickr("setToDay", today.getDate());
        this.$("#monPickr1").monPickr("setFromDay", 1);
    };
    DateTimeControls.prototype.GetDayOfYear = function (date) {
        var day = date.getDate();
        var month = date.getMonth() + 1;
        var year = date.getFullYear();
        var totalDays = day + (2 * month) + Math.round(3 * (month + 1) / 5) + year + Math.round(year / 4) - (year / 100) + Math.round(year / 400) + 2;
        var dayNo = (totalDays % 7);
        if (dayNo === 0) {
            dayNo = 7;
        }
        return dayNo;
    };
    DateTimeControls.prototype.SetRange = function () {
        return {
            minDate: new Date($("#MinDate").val()),
            maxDate: new Date($("#MaxDate").val())
        };
    };
    DateTimeControls.prototype.DateRangeSelected = function (itemIndex) {
        var dateRange;
        var nexDatePickerHelper = new NexDatePickerHelper();
        this.$("#DateList").prop("selectedIndex", itemIndex);
        var fdate = new Date(this.$("#MaxDate").val());
        var tdate = new Date(this.$("#MaxDate").val());
        if (itemIndex === 1) {
            fdate = new Date(tdate.getFullYear(), tdate.getMonth(), tdate.getDate() - 1);
            tdate = fdate;
        }
        else if (itemIndex === 2) {
            dateRange = nexDatePickerHelper.GetDateRangeOfCurrentWeek(tdate);
            fdate = dateRange[0];
        }
        else if (itemIndex === 3) {
            dateRange = nexDatePickerHelper.GetDateRangeOfLastWeek(tdate);
            fdate = dateRange[0];
            tdate = dateRange[1];
        }
        else if (itemIndex === 4) {
            fdate = new Date(tdate.getFullYear(), tdate.getMonth(), 1);
            tdate = new Date(tdate.getFullYear(), tdate.getMonth(), tdate.getDate());
        }
        else if (itemIndex === 5) {
            fdate = new Date(tdate.getFullYear(), tdate.getMonth() - 1, 1);
            tdate = new Date(tdate.getFullYear(), tdate.getMonth(), 0);
        }
        else if (itemIndex === 6) {
            fdate = new Date(tdate.getFullYear(), tdate.getMonth() - 1, 1);
        }
        else if (itemIndex === 7) {
            return;
        }
        this.$("#dtpFromDate").val($.datepicker.formatDate("M dd, yy", fdate));
        this.$("#dtpToDate").val($.datepicker.formatDate("M dd, yy", tdate));
    };
    return DateTimeControls;
}());
//# sourceMappingURL=DateTimeControls.js.map
//The jQuery UI Month Picker Version 3.0.4
//https://kidsysco.github.io/jquery-ui-month-picker/
(function ($, window, document, Date) {
    'use strict';

    var _setupErr = 'MonthPicker Error: ';
    // This test must be run before any rererence is made to jQuery.
    // In case the user didn't load jQuery or jQuery UI the plugin
    // will fail before it get's to this test + there is no reason
    // to perform this test for every MonthPicker instance being created.
    if (!$ || !$.ui || !$.ui.button || !$.ui.datepicker) {
        alert(_setupErr + 'The jQuery UI button and datepicker plug-ins must be loaded.');
        return;
    }

    // Creates an alias to jQuery UI's .button() that dosen't
    // conflict with Bootstrap.js button (#35)
    $.widget.bridge('jqueryUIButton', $.ui.button);

    var _speeds = $.fx.speeds;
    var _eventsNs = '.MonthPicker';
    var _textfieldClass = 'month-year-input';
    var _clearHint = 'month-picker-clear-hint';
    var _iconClass = '.ui-button-icon-primary';
    var _disabledClass = 'month-picker-disabled';
    var _todayClass = 'ui-state-highlight';
    var _selectedClass = 'ui-state-active';
    var _defaultClass = 'ui-state-default';
    var _defaultPos = { my: 'left top+1', at: 'left bottom' };
    var _RTL_defaultPos = { my: 'right top+1', at: 'right bottom' };
    var _posErr = _setupErr + 'The jQuery UI position plug-in must be loaded.';
    var _badOptValErr = _setupErr + 'Unsupported % option value, supported values are: ';
    var _badMinMaxVal =  _setupErr + '"_" is not a valid %Month value.';
    var _openedInstance = null;
    var _hasPosition = !!$.ui.position;
    var _animVals = {
        Animation: ['slideToggle', 'fadeToggle', 'none'],
        ShowAnim: ['fadeIn', 'slideDown', 'none'],
        HideAnim: ['fadeOut', 'slideUp', 'none']
    };
    var _setOptionHooks = {
        ValidationErrorMessage: '_createValidationMessage',
        Disabled: '_setDisabledState',
        ShowIcon: '_updateButton',
        Button: '_updateButton',
        ShowOn: '_updateFieldEvents',
        IsRTL: '_setRTL',
        AltFormat: '_updateAlt',
        AltField: '_updateAlt',
        StartYear: '_setPickerYear',
        MinMonth: '_setMinMonth',
        MaxMonth: '_setMaxMonth',
        SelectedMonth: '_setSelectedMonth'
    };
    var $noop = $.noop;
    var $proxy = $.proxy;
    var $datepicker = $.datepicker;
    var click = 'click' + _eventsNs;

    function _toMonth(date) {
        return date.getMonth() + (date.getFullYear() * 12);
    }

    function _toYear(month) {
        return Math.floor(month / 12);
    }

    function _stayActive() {
        $(this).addClass(_selectedClass);
    }

    function _setActive( el, state ) {
        return el[ state ? 'on' : 'off' ]('mousenter mouseout',  _stayActive )
            .toggleClass(_selectedClass, state);
    }

    function _between(month, from, until) {
        return (!from || month >= from) && (!until || month <= until);
    }

    function _encodeMonth(_inst, _val) {
        if (_val === null) {
            return _val;
        } else if (_val instanceof Date) {
            return _toMonth(_val);
        } else if ($.isNumeric(_val)) {
            return _toMonth(new Date) + parseInt(_val, 10);
        }

        var _date = _inst._parseMonth(_val);
        if (_date) {
            return _toMonth(_date);
        }

        return _parsePeriod(_val);
    }

    function _event(_name, _inst) {
        return $proxy(_inst.options[_name] || $noop, _inst.element[0]);
    }

    function _parsePeriod(_val, _initDate) {
        // Parsing is done by replacing tokens in the value to form
        // a JSON object with it's keys and values reversed
        // (example '+1y +2m' will turn into {"+1":"y","+2":"m"})
        // After that we just revers the keys and values.
        var _json = $.trim(_val);
        _json = _json.replace(/y/i, '":"y"');
        _json = _json.replace(/m/i, '":"m"');

        try {
            var _rev = JSON.parse( '{"' + _json.replace(/ /g, ',"') + '}' ), obj = {};

            for (var key in _rev) {
                obj[ _rev[key] ] = key;
            }

            var _month = _toMonth(new Date);
            _month += (parseInt(obj.m, 10) || 0);
            return _month + (parseInt(obj.y, 10) || 0) * 12;
        } catch (e) {
            return false;
        }
    }

    function _makeDefaultButton(options) {
        // this refers to the associated input field.
        return $('<span id="MonthPicker_Button_' + this.id + '" class="month-picker-open-button">' + options.i18n.buttonText + '</span>')
            .jqueryUIButton({
                text: false,
                icons: {
                    // Defaults to 'ui-icon-calculator'.
                    primary: options.ButtonIcon
                }
            });
    }

    function _applyArrowButton($el, dir) {
        $el.jqueryUIButton('option', {
            icons: {
                primary: 'ui-icon-circle-triangle-' + (dir ? 'w' : 'e')
            }
        });
    }

    function _isInline(elem) {
        return !elem.is('input');
    }

    $.MonthPicker = {
        VERSION: '3.0.4', // Added in version 2.4;
        i18n: {
            year: 'Year',
            prevYear: 'Previous Year',
            nextYear: 'Next Year',
            next12Years: 'Jump Forward 12 Years',
            prev12Years: 'Jump Back 12 Years',
            nextLabel: 'Next',
            prevLabel: 'Prev',
            buttonText: 'Open Month Chooser',
            jumpYears: 'Choose Years',
            backTo: 'Back to',
            months: ['Jan.', 'Feb.', 'Mar.', 'Apr.', 'May', 'June', 'July', 'Aug.', 'Sep.', 'Oct.', 'Nov.', 'Dec.']
        }
    };

    var _markup =
        '<div class="ui-widget-header month-picker-header ui-corner-all">' +
        '<table class="month-picker-year-table">' +
        '<tr>' +
        '<td class="month-picker-previous"><a /></td>' +
        '<td class="month-picker-title"><a /></td>' +
        '<td class="month-picker-next"><a /></td>' +
        '</tr>' +
        '</table>' +
        '</div>' +
        '<div>' +
        '<table class="month-picker-month-table" />' +
        '</div>';

    // Groups state and functionallity to fade in the jump years hint
    // when the user mouses over the Year 2016 text.
    // NOTE: An invocation of this function:
    // 1: Is an independent instance with it's own unique state.
    // 2: Assumes that there is no previous hint applied to the
    //    button (it dosen't remove the existing hint).
    function _applyButtonHint(_button, _hintText) {
        var _speed = 125, _currentLabel, _startTimeout, _labelElem = $();

        _button.on('mouseenter' + _eventsNs + 'h', _prepareToStart);

        // Setp 1: Wait to make sure the user isn't just mousing over and
        // away from the button.
        // NOTE: If _fadeOutHint() is triggered on mouseleave before the
        // timeout is triggered the animation is canceled.
        function _prepareToStart() {
            _startTimeout = setTimeout(_fadeOutLabel, 175);
        }

        // Setp 2: Fade out the label (Year 2016) text to 45%.
        function _fadeOutLabel() {
            _startTimeout = null;
            _labelElem = $('span', _button).animate({ opacity: 0.45 }, _speed, _fadeInHint);
        }

        // Setp 3: Fade in the hint text (Jump years).
        function _fadeInHint() {
            _currentLabel = _labelElem.text();
            _labelElem.animate({ opacity: 1 }, _speed).text(_hintText);
        }

        _button.on('mouseleave' + _eventsNs + 'h', _fadeOutHint);

        function _fadeOutHint() {
            if (_startTimeout) {
                // If the user is just moving over and away from the button, cancel
                // the animation completely.
                clearTimeout(_startTimeout);
            } else {
                // Setp 4: Fade out the hint text (Jump years) to 45%.
                _labelElem = $('span', _button).animate({ opacity: 0.45 }, _speed, _fadeInLabel);
            }
        }

        // Setp 5: Fade in the label (Year 2016) text.
        function _fadeInLabel() {
            _labelElem.text( _currentLabel ).animate({opacity: 1}, _speed);
        }

        // Adds a function to the button elemene which is called when the
        // user clicks the button (the hint needs to be removed).
        _button.data(_clearHint, function() {
            clearTimeout(_startTimeout);
            _labelElem.stop().css({ opacity: 1 });
            _button.off(_eventsNs + 'h');
        });
    } // End _applyButtonHint()

    function _setDisabled(_button, _value) {
        var _btnWidget = _button.data('ui-button');
        if (_btnWidget.option('disabled') !== _value) {
            _btnWidget.option('disabled', _value);
        }
    }

    $.widget("KidSysco.MonthPicker", {

        /******* Properties *******/

        options: {
            i18n: {},
            IsRTL: false,
            Position: null,
            StartYear: null,
            ShowIcon: true,
            UseInputMask: false,
            ValidationErrorMessage: null,
            Disabled: false,
            MonthFormat: 'mm/yy',
            Animation: 'fadeToggle',
            ShowAnim: null,
            HideAnim: null,
            ShowOn: null,
            MinMonth: null,
            MaxMonth: null,
            Duration: 'normal',
            Button: _makeDefaultButton,
            ButtonIcon: 'ui-icon-calculator'
        },

        _monthPickerButton: $(),
        _validationMessage: $(),
        _selectedBtn: $(),

        /******* jQuery UI Widget Factory Overrides ********/

        _destroy: function () {
            var _elem = this.element;
            if ($.mask && this.options.UseInputMask) {
                _elem.unmask();

                if (!this.GetSelectedDate()) {
                    _elem.val('');
                }
            }

            _elem.removeClass(_textfieldClass).off(_eventsNs);

            $(document).off(_eventsNs + this.uuid);

            this._monthPickerMenu.remove();

            var _button = this._monthPickerButton.off(click);
            if (this._removeOldBtn) {
                _button.remove();
            }

            this._validationMessage.remove();

            if (_openedInstance === this) {
                _openedInstance = null;
            }
        },

        _setOption: function (key, value) {
            switch (key) {
                case 'i18n':
                    // Pass a clone i18n object to the this._super.
                    value = $.extend({}, value);
                    break;
                case 'Position':
                    if (!_hasPosition) {
                        alert(_posErr);
                        return;
                    }
                    break;
                case 'MonthFormat':
                    var date = this.GetSelectedDate();
                    if (date) {
                        this.element.val( this.FormatMonth(date, value) );
                    }
                    break;
            }

            // Make sure the user passed in a valid Animation, ShowAnim and HideAnim options values.
            if (key in _animVals && $.inArray(value, _animVals[key]) === -1) {
                alert(_badOptValErr.replace(/%/, key) + _animVals[key]);
                return;
            }

            // In jQuery UI 1.8, manually invoke the _setOption method from the base widget.
            //$.Widget.prototype._setOption.apply(this, arguments);
            // In jQuery UI 1.9 and above, you use the _super method instead.
            this._super(key, value);

            _setOptionHooks[key] ? this[ _setOptionHooks[key] ](value) : 0;
        },

        _create: function () {
            var _el = this.element, _opts = this.options, _type = _el.attr('type');
            // According to http://www.w3.org/TR/html-markup/input.html#input
            // An input element with no type attribute specified represents the same thing as an
            // input element with its type attribute set to "text".
            // TLDR:
            // http://www.w3.org/TR/html5/forms.html#the-input-element
            // https://api.jquery.com/text-selector/

            // $.inArray(void 0, ['text', 'month', void 0]) returns -1 when searching for undefined in IE8 (#45)
            // This is only noticable in the real version of IE8, emulated versions
            // from the dev tools in modern browsers do not suffer from this issue.
            // if (!_el.is('input,div,span') || $.inArray(_el.attr('type'), ['text', 'month', void 0]) === -1) {
            if (!_el.is('input,div,span') || (_type !== 'text' && _type !== 'month' && _type !==  void 0)) {
                var error = _setupErr + 'MonthPicker can only be called on text or month inputs.';
                // Call alert first so that IE<10 won't trip over console.log and swallow all errors.
                alert(error + ' \n\nSee (developer tools) for more details.');

                console.error(error + '\n Caused by:');
                console.log(_el[0]);
                return false;
            }

            if (!$.mask && _opts.UseInputMask) {
                alert(_setupErr + 'The UseInputMask option requires the Input Mask Plugin. Get it from digitalbush.com');
                return false;
            }

            if (_opts.Position !== null && !_hasPosition) {
                alert(_posErr);
                return false;
            }

            // Make sure the user passed in a valid Animation, ShowAnim and HideAnim options values.
            for (var opt in _animVals) {
                if (_opts[opt] !== null && $.inArray(_opts[opt], _animVals[opt]) === -1) {
                    alert(_badOptValErr.replace(/%/, opt) + _animVals[opt]);
                    return false;
                }
            }

            this._isMonthInputType = _el.attr('type') === 'month';
            if (this._isMonthInputType) {
                this.options.MonthFormat = this.MonthInputFormat;
                _el.css('width', 'auto');
            }

            var _menu = this._monthPickerMenu = $('<div id="MonthPicker_' + _el[0].id + '" class="month-picker ui-widget ui-widget-content ui-corner-all"></div>').hide();
            var isInline = _isInline(_el);

            $(_markup).appendTo(_menu);
            _menu.appendTo( isInline ? _el : document.body );

            this._titleButton =
                $('.month-picker-title', _menu)
                    .click($proxy(this._showYearsClickHandler, this))
                    .find('a').jqueryUIButton()
                    .removeClass(_defaultClass);

            this._applyJumpYearsHint();
            this._createValidationMessage();

            this._prevButton = $('.month-picker-previous>a', _menu)
                .jqueryUIButton({ text: false })
                .removeClass(_defaultClass);

            this._nextButton = $('.month-picker-next>a', _menu)
                .jqueryUIButton({ text: false })
                .removeClass(_defaultClass);

            this._setRTL(_opts.IsRTL); //Assigns icons to the next/prev buttons.

            $(_iconClass, this._nextButton).text(this._i18n('nextLabel'));
            $(_iconClass, this._prevButton).text(this._i18n('prevLabel'));

            var $table = $('.month-picker-month-table', _menu);
            for (var i = 0; i < 12; i++) {
                var $tr = !(i % 3) ? $('<tr />').appendTo($table) : $tr;

                // Use <a> tag instead of <button> to avoid issues
                // only with Google Chrome (#50).
                $tr.append('<td><a class="button-' + (i + 1) + '" /></td>');
            }

            this._buttons = $('a', $table).jqueryUIButton();

            _menu.on('mousedown' + _eventsNs, function (event) {
                event.preventDefault();
            });

            // Checks and initailizes Min/MaxMonth properties
            // (creates _setMinMonth and _setMaxMonth methods).
            var me = this, Month = 'Month';
            $.each(['Min', 'Max'], function(i, type) {
                me["_set" + type + Month] = function(val) {
                    if ((me['_' + type + Month] = _encodeMonth(me, val)) === false) {
                        alert(_badMinMaxVal.replace(/%/, type).replace(/_/, val));
                    }
                };

                me._setOption(type + Month, me.options[type + Month]);
            });

            var _selMonth = _opts.SelectedMonth;
            if (_selMonth !== void 0) {
                var month = _encodeMonth(this, _selMonth);
                _el.val( this._formatMonth(new Date( _toYear(month), month % 12, 1)) );
            }

            this._updateAlt();

            this._setUseInputMask();
            this._setDisabledState();
            this.Destroy = this.destroy;

            if (isInline) {
                this.Open();
            } else {
                // Update the alt field if the user manually changes
                // the input field.
                _el.addClass(_textfieldClass);
                _el.change($proxy(this._updateAlt, this));
            }
        },

        /****** Publicly Accessible API functions ******/

        GetSelectedDate: function () {
            return this._parseMonth();
        },

        GetSelectedYear: function () {
            var date = this.GetSelectedDate();
            return date ? date.getFullYear() : NaN;
        },

        GetSelectedMonth: function () {
            var date = this.GetSelectedDate();
            return date ? date.getMonth()+1 : NaN;
        },

        Validate: function() {
            var _date = this.GetSelectedDate();

            if (this.options.ValidationErrorMessage !== null && !this.options.Disabled) {
                this._validationMessage.toggle(!_date);
            }

            return _date;
        },

        GetSelectedMonthYear: function () {
            var date = this.Validate();
            return date ? (date.getMonth() + 1) + '/' + date.getFullYear() : null;
        },

        Disable: function () {
            this._setOption("Disabled", true);
        },

        Enable: function () {
            this._setOption("Disabled", false);
        },

        ClearAllCallbacks: function () {
            for (var _opt in this.options) {
                if (_opt.indexOf('On') === 0) {
                    this.options[_opt] = $noop;
                }
            }
        },

        Clear: function () {
            this.element.val('');
            $(this.options.AltField).val('');
            this._validationMessage.hide();
        },

        Toggle: function (event) {
            return this._visible ? this.Close(event) : this.Open(event);
        },

        Open: function (event) {
            var _elem = this.element, _opts = this.options;
            if (!_opts.Disabled && !this._visible) {
                // Allow the user to prevent opening the menu.
                event = event || $.Event();
                if (_event('OnBeforeMenuOpen', this)(event) === false || event.isDefaultPrevented()) {
                    return;
                }

                this._visible = true;
                this._ajustYear(_opts);

                var _menu = this._monthPickerMenu;
                this._showMonths();

                if (_isInline(_elem)) {
                    _menu.css('position', 'static').show();
                    _event('OnAfterMenuOpen', this)();
                } else {
                    // If there is an open menu close it first.
                    if (_openedInstance) {
                        _openedInstance.Close(event);
                    }

                    _openedInstance = this;
                    $(document).on('mousedown' + _eventsNs + this.uuid, $proxy(this.Close, this))
                        .on('keydown' + _eventsNs + this.uuid, $proxy(this._keyDown, this));

                    // Trun off validation so that clicking one of the months
                    // won't blur the input field and trogger vlaidation
                    // befroe the month was chosen (click event was triggered).
                    // It is turned back on when Hide() is called.
                    _elem.off('blur' + _eventsNs).focus();

                    var _anim = _opts.ShowAnim || _opts.Animation,
                        _noAnim = _anim === 'none';

                    // jQuery UI overrides jQuery.show and dosen't
                    // call the start callback.
                    // see: http://api.jqueryui.com/show/
                    _menu[ _noAnim ? 'fadeIn' : _anim ]({
                        duration: _noAnim ? 0 : this._duration(),
                        start: $proxy(this._position, this, _menu),
                        complete: _event('OnAfterMenuOpen', this)
                    });
                }
            }
        },

        Close: function (event) {
            var _elem = this.element;
            if (!_isInline(_elem) && this._visible) {
                var _menu = this._monthPickerMenu,
                    _opts = this.options;

                event = event || $.Event();
                if (_event('OnBeforeMenuClose', this)(event) === false || event.isDefaultPrevented()) {
                    return;
                }

                // If the menu is closed while in jump years mode, bring back
                // the jump years hint.
                if (this._backToYear) {
                    this._applyJumpYearsHint();
                    this._backToYear = 0;
                }

                this._visible = false;
                _openedInstance = null;
                $(document).off('keydown' + _eventsNs + this.uuid)
                    .off('mousedown' + _eventsNs + this.uuid);

                this.Validate();
                _elem.on('blur' + _eventsNs, $proxy(this.Validate, this));
                var _callback = _event('OnAfterMenuClose', this);

                var _anim = _opts.HideAnim || _opts.Animation;
                if (_anim === 'none') {
                    _menu.hide(0, _callback);
                } else {
                    _menu[ _anim ](this._duration(), _callback);
                }
            }
        },

        /**
         * Methods the user can override to use a third party library
         * such as http://momentjs.com for parsing and formatting months.
         */
        MonthInputFormat: 'yy-mm',

        ParseMonth: function (str, format) {
            try {
                return $datepicker.parseDate('dd' + format, '01' + str);
            } catch (e) {
                return null;
            }
        },

        FormatMonth: function (date, format) {
            try {
                return $datepicker.formatDate(format, date) || null;
            } catch (e) {
                return null;
            }
        },

        /****** Private and Misc Utility functions ******/

        _setSelectedMonth: function (_selMonth) {
            var month = _encodeMonth(this, _selMonth), _el = this.element;

            if (month) {
                var date = new Date( _toYear(month), month % 12, 1 );
                _el.val( this._formatMonth( date ) );

                this._updateAlt(0, date);
                this._validationMessage.hide();
            } else {
                this.Clear();
            }

            this._ajustYear(this.options);
            this._showMonths();
        },

        _applyJumpYearsHint: function() {
            _applyButtonHint(this._titleButton, this._i18n('jumpYears'));
        },

        _i18n: function(str) {
            var _trans = this.options.i18n[str];

            if (typeof _trans === 'undefined') {
                return $.MonthPicker.i18n[str];
            } else {
                return _trans;
            }
        },

        _parseMonth: function (str, format) {
            return this.ParseMonth(str || this.element.val(), format || this.options.MonthFormat);
        },

        _formatMonth: function (date, format) {
            return this.FormatMonth(date || this._parseMonth(), format || this.options.MonthFormat);
        },

        _updateButton: function () {
            var isDisabled = this.options.Disabled;

            this._createButton();

            // If the button is a jQuery UI button,
            // plain HTML button or an input we support disable it,
            // otherwise the user must handle the diabled state
            // by creating an appropriate button by passing
            // a function. See Button option: Img tag tests for
            // more details.
            var _button = this._monthPickerButton;
            try {
                _button.jqueryUIButton('option', 'disabled', isDisabled);
            } catch (e) {
                _button.filter('button,input').prop('disabled', isDisabled);
            }

            this._updateFieldEvents();
        },

        _createButton: function () {
            var _elem = this.element, _opts = this.options;
            if (_isInline(_elem)) return;

            var _oldButton = this._monthPickerButton.off(_eventsNs);
            var _btnOpt = _opts.ShowIcon ? _opts.Button : false;

            if ($.isFunction(_btnOpt)) {
                var _params = $.extend(true, {i18n: $.extend(true, {}, $.MonthPicker.i18n)}, this.options);
                _btnOpt = _btnOpt.call(_elem[0], _params);
            }

            var _removeOldBtn = false;
            this._monthPickerButton = ( _btnOpt instanceof $ ? _btnOpt : $(_btnOpt) )
                .each(function() {
                    if (!$.contains(document.body, this)) {
                        _removeOldBtn = true;
                        $(this).insertAfter(_elem);
                    }
                })
                .on(click, $proxy(this.Toggle, this))
                .on('mousedown' + _eventsNs, function(r) {
                    r.preventDefault();
                });

            if (this._removeOldBtn) {
                _oldButton.remove();
            }

            this._removeOldBtn = _removeOldBtn;
        },

        _updateFieldEvents: function () {
            var _events = click + ' focus' + _eventsNs;
            this.element.off(_events);
            if (this.options.ShowOn === 'both' || !this._monthPickerButton.length) {
                this.element.on(_events, $proxy(this.Open, this));
            }
        },

        _createValidationMessage: function () {
            var _errMsg = this.options.ValidationErrorMessage, _elem = this.element;
            if ($.inArray(_errMsg, [null, '']) === -1) {
                var _msgEl = $('<span id="MonthPicker_Validation_' + _elem[0].id + '" class="month-picker-invalid-message">' + _errMsg + '</span>');

                var _button = this._monthPickerButton;
                this._validationMessage = _msgEl.insertAfter(_button.length ? _button : _elem);

                _elem.on('blur' + _eventsNs, $proxy(this.Validate, this));
            } else {
                this._validationMessage.remove();
            }
        },

        _setRTL: function(value) {
            _applyArrowButton( this._prevButton.css('float', value ? 'right' : 'left'), !value );
            _applyArrowButton( this._nextButton.css('float', value ? 'left' : 'right'), value );
        },

        _keyDown: function (event) {
            // Don't use $.ui.keyCode to help minification.
            switch (event.keyCode) {
                case 13: // Enter.
                    if (!this.element.val()) {
                        this._chooseMonth(new Date().getMonth() + 1);
                    }
                    this.Close(event);
                    break;
                case 27: // Escape
                case 9: // Tab
                    this.Close(event);
                    break;
            }
        },

        _duration: function() {
            var _dur = this.options.Duration;

            if ($.isNumeric(_dur)) {
                return _dur;
            }

            return _dur in _speeds ? _speeds[ _dur ] : _speeds._default;
        },

        _position: _hasPosition ?
            function($menu) {
                var _defauts = this.options.IsRTL ? _RTL_defaultPos : _defaultPos;
                var _posOpts = $.extend(_defauts, this.options.Position);

                // Only in IE and jQuery 1.12.0 or 2.2.0, .position() will add scrollTop to the top coordinate (#40)
                return $menu.position($.extend({of: this.element}, _posOpts));
            } :
            function($menu) {
                // Only in IE and jQuery 1.12.0 or 2.2.0, .offset() will add scrollTop to the top coordinate (#40)
                var _el = this.element,
                    _css = { top: (_el.offset().top + _el.height() + 7) + 'px' };

                if (this.options.IsRTL) {
                    _css.left = (_el.offset().left-$menu.width()+_el.width() + 7) + 'px';
                } else {
                    _css.left = _el.offset().left + 'px';
                }

                return $menu.css(_css);
            },

        _setUseInputMask: function () {
            if (!this._isMonthInputType) {
                try {
                    if (this.options.UseInputMask) {
                        this.element.mask( this._formatMonth(new Date).replace(/\d/g, 9) );
                    } else {
                        this.element.unmask();
                    }
                } catch (e) {}
            }
        },

        _setDisabledState: function () {
            var isDisabled = this.options.Disabled, _elem = this.element;

            // Disable the associated input field.
            _elem[0].disabled = isDisabled;
            _elem.toggleClass(_disabledClass, isDisabled);

            if (isDisabled) {
                this._validationMessage.hide();
            }

            this.Close();
            this._updateButton();

            _event('OnAfterSetDisabled', this)(isDisabled);
        },

        _getPickerYear: function () {
            return this._pickerYear;
        },

        _setPickerYear: function (year) {
            this._pickerYear = year || new Date().getFullYear();
            this._titleButton.jqueryUIButton({ label: this._i18n('year') + ' ' + this._pickerYear });
        },

        // When calling this method with a falsy (undefined) date
        // value, this.element.val() is used as the date value.
        //
        // Therefore it's important to update the input field
        // before calling this method.
        _updateAlt: function (noop, date) {
            var _field = $(this.options.AltField);
            if (_field.length) {
                _field.val( this._formatMonth(date, this.options.AltFormat) );
            }
        },

        _chooseMonth: function (month) {
            var _year = this._getPickerYear();
            var date = new Date(_year, month-1);
            this.element.val(this._formatMonth( date )).blur();
            this._updateAlt(0, date);

            _setActive( this._selectedBtn, false );
            this._selectedBtn = _setActive( $(this._buttons[month-1]), true );

            _event('OnAfterChooseMonth', this)(date);
        },

        _chooseYear: function (year) {
            this._backToYear = 0;
            this._setPickerYear(year);
            this._buttons.removeClass(_todayClass);
            this._showMonths();
            this._applyJumpYearsHint();

            _event('OnAfterChooseYear', this)();
        },

        _showMonths: function () {
            var _months = this._i18n('months');

            this._prevButton
                .attr('title', this._i18n('prevYear'))
                .off(click)
                .on(click, $proxy(this._addToYear, this, -1));

            this._nextButton
                .attr('title', this._i18n('nextYear'))
                .off(click)
                .on(click, $proxy(this._addToYear, this, 1));

            this._buttons.off(_eventsNs);

            var me = this, _onMonthClick = $proxy(me._onMonthClick, me);
            $.each(_months, function(index, monthName) {
                $(me._buttons[index])
                    .on(click, {month: index+1}, _onMonthClick )
                    .jqueryUIButton('option', 'label', monthName);
            });

            this._decorateButtons();
        },

        _showYearsClickHandler: function () {
            this._buttons.removeClass(_todayClass);
            if (!this._backToYear) {
                this._backToYear = this._getPickerYear();
                this._showYears();

                var _label = this._i18n('backTo') + ' ' + this._getPickerYear();
                this._titleButton.jqueryUIButton({ label: _label }).data( _clearHint )();

                _event('OnAfterChooseYears', this)();
            } else {
                this._setPickerYear(this._backToYear);
                this._applyJumpYearsHint();
                this._showMonths();
                this._backToYear = 0;
            }
        },

        _showYears: function () {
            var _currYear = this._getPickerYear(),
                _yearDifferential = -4,
                _firstYear = (_currYear + _yearDifferential),
                AMOUNT_TO_ADD = 12,
                _thisYear = new Date().getFullYear();

            var _minDate = this._MinMonth;
            var _maxDate = this._MaxMonth;
            var _minYear = _minDate ? _toYear(_minDate) : 0;
            var _maxYear = _maxDate ? _toYear(_maxDate) : 0;
            this._prevButton
                .attr('title', this._i18n('prev12Years'))
                .off(click)
                .on(click, $proxy(this._addToYears, this, -AMOUNT_TO_ADD));

            this._nextButton
                .attr('title', this._i18n('next12Years'))
                .off(click)
                .on(click, $proxy(this._addToYears, this, AMOUNT_TO_ADD));

            _setDisabled(this._prevButton, _minYear && (_firstYear - 1) < _minYear);
            _setDisabled(this._nextButton, _maxYear && (_firstYear + 12) -1 > _maxYear);

            this._buttons.off(_eventsNs);

            _setActive( this._selectedBtn, false );

            var _selYear = this.GetSelectedYear();
            var _onClick = $proxy(this._onYearClick, this);
            var _todayWithinBounds = _between(_thisYear, _minYear, _maxYear);
            var _selWithinBounds = _between(_selYear, _minYear, _maxYear);

            for (var _counter = 0; _counter < 12; _counter++) {
                var _year = _currYear + _yearDifferential;

                var _btn = $( this._buttons[_counter] ).jqueryUIButton({
                    disabled: !_between(_year, _minYear, _maxYear),
                    label: _year
                })
                    .toggleClass(_todayClass, _year === _thisYear && _todayWithinBounds) // Heighlight the current year.
                    .on(click, { year: _year }, _onClick );

                // Heighlight the selected year.
                if (_selWithinBounds && _selYear && _selYear === _year) {
                    this._selectedBtn = _setActive( _btn , true );
                }

                _yearDifferential++;
            }
        },

        _onMonthClick: function(event) {
            this._chooseMonth(event.data.month);
            this.Close(event);
        },

        _onYearClick: function(event) {
            this._chooseYear(event.data.year);
        },

        _addToYear: function(amount) {
            this._setPickerYear( this._getPickerYear() + amount );
            this.element.focus();
            this._decorateButtons();

            _event('OnAfter' + (amount > 0 ? 'Next' : 'Previous') + 'Year', this)();
        },

        _addToYears: function(amount) {
            this._pickerYear = this._getPickerYear() + amount;
            this._showYears();
            this.element.focus();

            _event('OnAfter' + (amount > 0 ? 'Next' : 'Previous') + 'Years', this)();
        },

        _ajustYear: function(_opts) {
            var _year = _opts.StartYear || this.GetSelectedYear() || new Date().getFullYear();
            if (this._MinMonth !== null) {
                _year = Math.max(_toYear(this._MinMonth), _year);
            }
            if (this._MaxMonth !== null) {
                _year = Math.min(_toYear(this._MaxMonth), _year);
            }

            this._setPickerYear( _year );
        },

        _decorateButtons: function() {
            var _curYear = this._getPickerYear(), _todaysMonth = _toMonth(new Date),
                _minDate = this._MinMonth, _maxDate = this._MaxMonth;

            // Heighlight the selected month.
            _setActive( this._selectedBtn, false );
            var _sel = this.GetSelectedDate();
            var _withinBounds = _between(_sel ? _toMonth(_sel) : null, _minDate, _maxDate);

            if (_sel && _sel.getFullYear() === _curYear) {
                this._selectedBtn = _setActive( $(this._buttons[_sel.getMonth()]) , _withinBounds );
            }

            // Disable the next/prev button if we've reached the min/max year.
            _setDisabled(this._prevButton,  _minDate && _curYear == _toYear(_minDate));
            _setDisabled(this._nextButton,  _maxDate && _curYear == _toYear(_maxDate));

            for (var i = 0; i < 12; i++) {
                // Disable the button if the month is not between the
                // min and max interval.
                var _month = (_curYear * 12) + i, _isBetween = _between(_month, _minDate, _maxDate);

                $(this._buttons[i])
                    .jqueryUIButton({ disabled: !_isBetween })
                    .toggleClass(_todayClass, _isBetween && _month == _todaysMonth); // Highlights today's month.
            }
        }
    });
}(jQuery, window, document, Date));

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
var DailyWinLossFilters = /** @class */ (function (_super) {
    __extends(DailyWinLossFilters, _super);
    function DailyWinLossFilters() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.dateFormat = "MM/DD/YYYY";
        return _this;
    }
    DailyWinLossFilters.prototype.renderCalendarPicker = function (options) {
        var _this = this;
        var dateRangePicker = this.$(options.selectorId);
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
    DailyWinLossFilters.prototype.getCalenderDefinedShortcuts = function (dateRangePicker) {
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
    return DailyWinLossFilters;
}(BaseReportFilters));
var SettlementWinLossFilters = /** @class */ (function (_super) {
    __extends(SettlementWinLossFilters, _super);
    function SettlementWinLossFilters() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    SettlementWinLossFilters.prototype.renderCalendarPicker = function (options) {
        var _this = this;
        var fromMonthPicker = this.$(options[0].selectorId).MonthPicker({
            Button: '<span class="icon-calendar" />',
            SelectedMonth: options[0].currenSelectedMonth,
            MinMonth: '-12m',
            MaxMonth: '0',
            Animation: 'none',
            HideAnim: 'none',
            ShowAnim: 'none',
            OnAfterChooseMonth: function (value) { return _this.fromDate = _this.formatDate(value); }
        });
        this.fromDate = this.formatDate(fromMonthPicker.MonthPicker('GetSelectedDate'));
        var toMonthPicker = this.$(options[1].selectorId).MonthPicker({
            Button: '<span class="icon-calendar" />',
            SelectedMonth: options[1].currenSelectedMonth,
            MinMonth: '-12m',
            MaxMonth: '0',
            Animation: 'none',
            HideAnim: 'none',
            ShowAnim: 'none',
            OnAfterChooseMonth: function (value) { return _this.toDate = _this.formatDate(value); }
        });
        this.toDate = this.formatDate(toMonthPicker.MonthPicker('GetSelectedDate'));
    };
    SettlementWinLossFilters.prototype.formatDate = function (date) {
        return moment(date).format('MM/DD/YYYY');
    };
    return SettlementWinLossFilters;
}(BaseReportFilters));
//# sourceMappingURL=WhiteLabelWinLossFilters.js.map
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
var DailyWinLossGrid = /** @class */ (function (_super) {
    __extends(DailyWinLossGrid, _super);
    function DailyWinLossGrid(options) {
        var _this = _super.call(this, options) || this;
        _this.gridOptions = {
            columns: DailyWinLossGrid.DefaultColumns,
            //numberOfFrozenColumn: 2,
            autoCalculateGridExpandHeight: false,
            fullHeight: !options.isMobile.phone,
            gridHeight: options.isMobile.phone ? $(window).height() - 70 : 700,
            showTitle: true,
            titleInfo: {
                customButtons: "<span id=" + options.excelBtnId + " title=\"Export Excel\" class=\"icon-excel\"></span>"
            },
            gridExpandHeight: 260,
            showPager: false,
            bottomBorderWidth: 0,
            showDistinctRow: false,
            allowTruncate: true,
            bodyRows: [],
            showFooter: true,
            footerRows: [],
            showHighlightRow: false
        };
        return _this;
    }
    DailyWinLossGrid.prototype.buildBodyRows = function (reportData) {
        var bodyData = [];
        if (reportData) {
            for (var _i = 0, reportData_1 = reportData; _i < reportData_1.length; _i++) {
                var element = reportData_1[_i];
                var WinLostDate = element.WinLostDate, WhiteLabelName = element.WhiteLabelName, Super = element.Super, Master = element.Master, Agent = element.Agent, MemberWinLoss = element.MemberWinLoss, Promotion = element.Promotion;
                bodyData.push({
                    Columns: [
                        { Name: "WinLostDate", Value: moment(WinLostDate).format("MM/DD/YYYY"), CellAlign: "center" },
                        { Name: "WhiteLabelName", Value: WhiteLabelName },
                        { Name: "Agent", Value: Agent },
                        { Name: "Master", Value: Master },
                        { Name: "Super", Value: Super },
                        { Name: "MemberWinLoss", Value: MemberWinLoss },
                        { Name: "Promotion", Value: Promotion }
                    ]
                });
            }
        }
        return bodyData;
    };
    DailyWinLossGrid.prototype.buildFooterRows = function (data) {
        var footerColumns = [];
        if (data) {
            footerColumns.push({
                Columns: [
                    { Name: "WinLostDate", Value: "Grand Total", MergeColNumber: 5, CellAlign: "right" },
                    { Name: "MemberWinLoss", Value: data.MemberWinLoss, CellAlign: "right" },
                    { Name: "Promotion", Value: data.Promotion, CellAlign: "right" },
                ]
            });
        }
        return footerColumns;
    };
    DailyWinLossGrid.prototype.getDefaultColumns = function () {
        return DailyWinLossGrid.DefaultColumns;
    };
    DailyWinLossGrid.prototype.getGroupHeaderColumns = function () {
        return [];
    };
    DailyWinLossGrid.prototype.getGridOptions = function () {
        return this.gridOptions;
    };
    DailyWinLossGrid.DefaultColumns = [
        { HeaderText: "Date", Name: "WinLostDate", Width: 100, Sort: true, HeaderAlign: "center" },
        { HeaderText: "White Label", Name: "WhiteLabelName", Width: 180, Sort: true, HeaderAlign: "center" },
        { HeaderText: "Agent", Name: "Agent", Width: 160, Sort: true, HeaderAlign: "center" },
        { HeaderText: "Master", Name: "Master", Width: 160, Sort: true, HeaderAlign: "center" },
        { HeaderText: "Super", Name: "Super", Width: 140, Sort: true, HeaderAlign: "center", },
        { HeaderText: "Member Win Loss", Name: "MemberWinLoss", Width: 150, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Promotion", Name: "Promotion", Width: 120, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" }
    ];
    DailyWinLossGrid.ExcelBtnId = 'exportWhiteLabelDailyWinLoss';
    return DailyWinLossGrid;
}(BaseReportGrid));
var SettlementWinLossGrid = /** @class */ (function (_super) {
    __extends(SettlementWinLossGrid, _super);
    function SettlementWinLossGrid(options) {
        var _this = _super.call(this, options) || this;
        _this.gridOptions = {
            customizeHeader: true,
            columns: SettlementWinLossGrid.DefaultColumns,
            groupHeaderRows: [{ Columns: SettlementWinLossGrid.GroupHeaderColumns }],
            //numberOfFrozenColumn: 4,
            autoCalculateGridExpandHeight: false,
            fullHeight: !options.isMobile.phone,
            gridHeight: options.isMobile.phone ? $(window).height() - 70 : 700,
            showTitle: true,
            titleInfo: {
                customButtons: "<span id=" + options.copyBtnId + " title=\"Copy\" class=\"icon-copy\"></span><span id=" + options.excelBtnId + " title=\"Export Excel\" class=\"icon-excel\"></span>"
            },
            gridExpandHeight: 320,
            showPager: false,
            bottomBorderWidth: 0,
            showDistinctRow: false,
            allowTruncate: true,
            bodyRows: [],
            showFooter: true,
            footerRows: [],
            showHighlightRow: false
        };
        _this.initSelectColumnsDropdown(SettlementWinLossGrid.DefaultColumns, [0, 1]);
        _this.initCopyBtn(options.copyBtnId);
        return _this;
    }
    SettlementWinLossGrid.prototype.initSelectColumnsDropdown = function (columns, ignoreColumns) {
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
            toggleWidth: this.options.isMobile.phone ? 255 : 265,
            minWidth: this.options.isMobile.phone ? 255 : 265
        });
    };
    SettlementWinLossGrid.prototype.buildBodyRows = function (reportData) {
        var bodyData = [];
        if (!reportData) {
            return bodyData;
        }
        for (var _i = 0, reportData_2 = reportData; _i < reportData_2.length; _i++) {
            var element = reportData_2[_i];
            bodyData.push({
                Columns: [
                    { Name: "Month", Value: element.Month, CellAlign: "center" },
                    { Name: "WhiteLabel", Value: element.WhiteLabel },
                    { Name: "Agent", Value: element.Agent },
                    { Name: "Master", Value: element.Master },
                    { Name: "Super", Value: element.Super },
                    { Name: "MemberWinLoss", Value: element.MemberWL },
                    { Name: "MemberPromotion", Value: element.MemberPromotion },
                    { Name: "LossCarryforward", Value: element.LossCarryforward },
                    { Name: "WhiteLabelWinLoss", Value: element.WhiteLabelWinLoss },
                    { Name: "WhiteLabelNetWinLoss", Value: element.WhiteLabelNetWinLoss },
                    { Name: "WhiteLabelFee", Value: element.WhiteLabelFee },
                    { Name: "WhiteLabelPromotionSettlement", Value: element.WhiteLabelPromotionSettlement },
                    { Name: "WhiteLabelPromotionShare", Value: element.WhiteLabelPromotionShare },
                    { Name: "WhiteLabelTotal", Value: element.WhiteLabelTotal },
                    { Name: "MasterEarning", Value: element.MasterEarning },
                    { Name: "SuperEarning", Value: element.SuperEarning },
                    { Name: "HeadlineEarning", Value: element.HeadlineEarning },
                    { Name: "CompanyIncome", Value: element.CompanyIncome },
                    { Name: "CompanyPromotionShare", Value: element.CompanyPromotionShare },
                    { Name: "CompanyTotal", Value: element.CompanyTotal }
                ]
            });
        }
        this.gridBodyRowsCache = bodyData;
        return bodyData;
    };
    SettlementWinLossGrid.prototype.buildFooterRows = function (data) {
        var footerColumns = [];
        data && footerColumns.push({
            Columns: [
                { Name: "Month", Value: "Grand Total", MergeColNumber: 5, CellAlign: "right" },
                { Name: "MemberWinLoss", Value: data.MemberWL, CellAlign: "right" },
                { Name: "MemberPromotion", Value: data.MemberPromotion, CellAlign: "right" },
                { Name: "LossCarryforward", Value: data.LossCarryforward, CellAlign: "right" },
                { Name: "WhiteLabelWinLoss", Value: data.WhiteLabelWinLoss, CellAlign: "right" },
                { Name: "WhiteLabelNetWinLoss", Value: data.WhiteLabelNetWinLoss, CellAlign: "right" },
                { Name: "WhiteLabelFee", Value: data.WhiteLabelFee, CellAlign: "right" },
                { Name: "WhiteLabelPromotionSettlement", Value: data.WhiteLabelPromotionSettlement, CellAlign: "right" },
                { Name: "WhiteLabelPromotionShare", Value: data.WhiteLabelPromotionShare, CellAlign: "right" },
                { Name: "WhiteLabelTotal", Value: data.WhiteLabelTotal, CellAlign: "right" },
                { Name: "MasterEarning", Value: data.MasterEarning, CellAlign: "right" },
                { Name: "SuperEarning", Value: data.SuperEarning, CellAlign: "right" },
                { Name: "HeadlineEarning", Value: data.HeadlineEarning, CellAlign: "right" },
                { Name: "CompanyIncome", Value: data.CompanyIncome, CellAlign: "right" },
                { Name: "CompanyPromotionShare", Value: data.CompanyPromotionShare, CellAlign: "right" },
                { Name: "CompanyTotal", Value: data.CompanyTotal, CellAlign: "right" },
            ]
        });
        this.gridFooterRowsCache = footerColumns;
        return footerColumns;
    };
    SettlementWinLossGrid.prototype.getDefaultColumns = function () {
        return SettlementWinLossGrid.DefaultColumns;
    };
    SettlementWinLossGrid.prototype.getGroupHeaderColumns = function () {
        return SettlementWinLossGrid.GroupHeaderColumns;
    };
    SettlementWinLossGrid.prototype.getGridOptions = function () {
        return this.gridOptions;
    };
    SettlementWinLossGrid.DefaultColumns = [
        { HeaderText: "Month", Name: "Month", Width: 80, Sort: true, HeaderAlign: "center", ColumnName2: "Month" },
        { HeaderText: "White Label", Name: "WhiteLabel", Width: 130, Sort: true, HeaderAlign: "center", ColumnName2: "White Label" },
        { HeaderText: "Agent", Name: "Agent", Width: 130, Sort: true, HeaderAlign: "center", ColumnName2: "Agent" },
        { HeaderText: "Master", Name: "Master", Width: 120, Sort: true, HeaderAlign: "center", ColumnName2: "Master" },
        { HeaderText: "Super", Name: "Super", Width: 110, Sort: true, HeaderAlign: "center", ColumnName2: "Super" },
        { HeaderText: "Win Loss", Name: "MemberWinLoss", Width: 110, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Member - Win Loss" },
        { HeaderText: "Promotion", Name: "MemberPromotion", Width: 110, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Member - Promotion" },
        { HeaderText: "Loss C/f", Name: "LossCarryforward", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "White Label - Loss C/f" },
        { HeaderText: "Win Loss", Name: "WhiteLabelWinLoss", Width: 110, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "White Label - Win Loss" },
        { HeaderText: "Net Win Loss", Name: "WhiteLabelNetWinLoss", Width: 110, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "White Label - Net Win Loss" },
        { HeaderText: "Gross Fee", Name: "WhiteLabelFee", Width: 110, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "White Label - Gross Fee", },
        { HeaderText: "Promotion Settlement", Name: "WhiteLabelPromotionSettlement", Width: 160, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "WhiteLabel - Promotion Settlement" },
        { HeaderText: "Promotion Share", Name: "WhiteLabelPromotionShare", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "White Label - Promotion Share" },
        { HeaderText: "Total Fee", Name: "WhiteLabelTotal", Width: 110, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "White Label - Total Fee" },
        { HeaderText: "Master Earning", Name: "MasterEarning", Width: 110, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Master Earning" },
        { HeaderText: "Super Earning", Name: "SuperEarning", Width: 110, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Super Earning" },
        { HeaderText: "Headline Earning", Name: "HeadlineEarning", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Headline Earning" },
        { HeaderText: "Income", Name: "CompanyIncome", Width: 110, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Company - Income" },
        { HeaderText: "Promotion Share", Name: "CompanyPromotionShare", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Company - Promotion Share" },
        { HeaderText: "Total", Name: "CompanyTotal", Width: 110, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$", ColumnName2: "Company - Total" }
    ];
    SettlementWinLossGrid.GroupHeaderColumns = [
        { HeaderText: "Month", Name: "Month", MergeRowNumber: 2 },
        { HeaderText: "White Label", Name: "WhiteLabel", MergeRowNumber: 2 },
        { HeaderText: "Agent", Name: "Agent", MergeRowNumber: 2 },
        { HeaderText: "Master", Name: "Master", MergeRowNumber: 2 },
        { HeaderText: "Super", Name: "Super", MergeRowNumber: 2 },
        { HeaderText: "Member", Name: "MemberWinLoss", MergeColNumber: 2 },
        { HeaderText: "White Label", Name: "LossCarryforward", MergeColNumber: 7 },
        { HeaderText: "Master Earning", Name: "MasterEarning", MergeRowNumber: 2 },
        { HeaderText: "Super Earning", Name: "SuperEarning", MergeRowNumber: 2 },
        { HeaderText: "Headline Earning", Name: "HeadlineEarning", MergeRowNumber: 2 },
        { HeaderText: "Company", Name: "CompanyIncome", MergeColNumber: 3 }
    ];
    SettlementWinLossGrid.ExcelBtnId = 'exportWhiteLabelSettlement';
    SettlementWinLossGrid.CopyBtnId = 'copyWhiteLabelSettlement';
    return SettlementWinLossGrid;
}(BaseReportGrid));
//# sourceMappingURL=WhiteLabelWinLossGrid.js.map
var SettlementReport = /** @class */ (function () {
    function SettlementReport() {
        this.typeReport = "settlement-win-loss";
        this.gridComponentConfig = {
            reportId: "settlement-win-loss",
            isMobile: isMobile,
            gridId: "settlement-win-loss-jqgrid",
            gridContainerId: "settlement-win-loss-mainGrid",
            selectColumnsId: "settlement-win-loss-ddl-columns",
            selectColumnsContainerId: "settlement-win-loss-ddl-columns-container",
            msgNoInfoId: "settlement-win-loss-msgNoInfo",
            excelBtnId: "settlement-win-loss-excel",
            copyBtnId: "settlement-win-loss-copy",
            pageSizeOptions: []
        };
        this.filterComponents = new SettlementWinLossFilters($);
        this.gridComponent = new SettlementWinLossGrid(this.gridComponentConfig);
    }
    SettlementReport.prototype.init = function () {
        var _this = this;
        $("#tabs").tabs();
        this.initDateRangePicker();
        this.initWhiteLabelsDropdown(JSON.parse($("#inp-whitelabels").val()));
        this.initUsernameFilter();
        $("#" + this.typeReport + "-submit-btn").click(function () { return _this.submit(); });
    };
    SettlementReport.prototype.initDateRangePicker = function () {
        var currentDate = moment($('#hidden-today').val(), 'MM/DD/YYYY');
        var prevSelectedMonth = currentDate.month() + "/" + currentDate.year();
        this.filterComponents.renderCalendarPicker([
            { selectorId: "#month-picker-from-date", currenSelectedMonth: prevSelectedMonth },
            { selectorId: "#month-picker-to-date", currenSelectedMonth: prevSelectedMonth }
        ]);
    };
    SettlementReport.prototype.initWhiteLabelsDropdown = function (whiteLabels) {
        var maxWhiteLabelLength = 0;
        var mappedDataDropDown = whiteLabels.map(function (e) {
            if (e.WhiteLabel.length > maxWhiteLabelLength) {
                maxWhiteLabelLength = e.WhiteLabel.length;
            }
            return {
                id: e.CustId,
                value: e.CustId,
                text: e.WhiteLabel,
                checked: true,
                enable: true,
            };
        });
        maxWhiteLabelLength *= 11;
        this.filterComponents.registerDropdownComponent({
            dropdownId: this.typeReport + "-ddl-white-label",
            dropdownName: "WhiteLabels",
            dropdownData: mappedDataDropDown,
            dropdownType: "fanexMultiSelect",
            minWidth: maxWhiteLabelLength > 200 ? maxWhiteLabelLength : 200
        });
    };
    SettlementReport.prototype.initUsernameFilter = function () {
        var usernameInput = {
            inputName: "username",
            inputId: this.typeReport + "-username-filter",
        };
        this.filterComponents.registerInputComponent(usernameInput);
        $("#" + usernameInput.inputId).keyup(function (e) {
            $(e.target).val().length > 1 && Fanex.ExtAcc.InitSuggestion("" + usernameInput.inputId);
        });
    };
    SettlementReport.prototype.submit = function () {
        var _this = this;
        Fanex.ExtAcc.BlockUI();
        if (this.unValidUserName()) {
            Fanex.ExtAcc.UnBlockUI();
            this.gridComponent.hideNoInfo(false);
        }
        else {
            $.ajax({
                url: resolveClientUrl("WhiteLabelReport/GetSettlement" + this.getQueryString()),
                type: "post",
                dataType: "json",
                cache: false,
                success: function (result) { return _this.submitSuccess(result); }
            });
        }
    };
    SettlementReport.prototype.getQueryString = function () {
        var _a = this.filterComponents.getAllFilterValues(), inputParams = _a.inputParams, dropdownParams = _a.dropdownParams, calendarParams = _a.calendarParams;
        return [
            '?FromDate=', calendarParams.fromDate,
            '&ToDate=', calendarParams.toDate,
            '&WhiteLabels=', dropdownParams[0].currentValue,
            '&Username=', inputParams[0].currentValue,
        ].join('');
    };
    SettlementReport.prototype.unValidUserName = function () {
        var inputParams = this.filterComponents.getAllFilterValues().inputParams;
        return inputParams[0].currentValue.includes("script");
    };
    SettlementReport.prototype.submitSuccess = function (result) {
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
    SettlementReport.prototype.initExportExcelBtn = function () {
        var _this = this;
        $("#" + this.gridComponentConfig.excelBtnId).off().click(function () {
            var url = resolveClientUrl("WhiteLabelReport/ExportSettlement" + _this.getQueryString());
            window.top.exportExcel(url, document);
        });
    };
    SettlementReport.prototype.renderGrid = function (data) {
        try {
            var gridOptions = this.gridComponent.getGridOptions();
            gridOptions.titleInfo.text = this.reportTitle();
            gridOptions.bodyRows = this.gridComponent.buildBodyRows(data.Items);
            gridOptions.footerRows = this.gridComponent.buildFooterRows(data.RowTotal);
            setPagerOptionOnMobile(gridOptions);
            this.gridComponent.setGridOptions(gridOptions);
            this.gridComponent.fanexGrid(gridOptions);
            workaroundForGridOnPhone();
        }
        catch (e) {
            console.log(e);
        }
    };
    SettlementReport.prototype.reportTitle = function () {
        var dateTimeParams = this.filterComponents.getCalendarValues();
        var fromDate = dateTimeParams.fromDate, toDate = dateTimeParams.toDate;
        var fromMonth = moment(fromDate, 'MM/DD/YYYY').format('MMM, YYYY');
        var month = fromDate === toDate
            ? "on " + fromMonth
            : "from " + fromMonth + " to " + moment(toDate, 'MM/DD/YYYY').format('MMM, YYYY');
        return "White Label Settlement Report " + month;
    };
    return SettlementReport;
}());
$(function () {
    new SettlementReport().init();
});
//# sourceMappingURL=SettlementReport.js.map
var DailyWinLossReport = /** @class */ (function () {
    function DailyWinLossReport() {
        this.typeReport = "daily-win-loss";
        this.gridComponentConfig = {
            reportId: "daily-win-loss",
            isMobile: isMobile,
            gridId: "daily-win-loss-jqgrid",
            gridContainerId: "daily-win-loss-mainGrid",
            selectColumnsId: "daily-win-loss-ddl-columns",
            selectColumnsContainerId: "daily-win-loss-ddl-columns-container",
            msgNoInfoId: "daily-win-loss-msgNoInfo",
            excelBtnId: "daily-win-loss-excel",
            copyBtnId: "daily-win-loss-copy",
            pageSizeOptions: []
        };
        this.filterComponents = new DailyWinLossFilters($);
        this.gridComponent = new DailyWinLossGrid(this.gridComponentConfig);
    }
    DailyWinLossReport.prototype.init = function () {
        var _this = this;
        this.initDateRangePicker();
        this.initWhiteLabelsDropdown(JSON.parse($("#inp-whitelabels").val()));
        this.initUsernameFilter();
        $("#" + this.typeReport + "-submit-btn").click(function () { return _this.submit(); });
    };
    DailyWinLossReport.prototype.initDateRangePicker = function () {
        this.filterComponents.renderCalendarPicker({
            selectorId: "#" + this.typeReport + "-date-range-picker"
        });
    };
    DailyWinLossReport.prototype.initWhiteLabelsDropdown = function (whiteLabels) {
        var maxWhiteLabelLength = 0;
        var mappedDataDropDown = whiteLabels.map(function (e) {
            if (e.WhiteLabel.length > maxWhiteLabelLength) {
                maxWhiteLabelLength = e.WhiteLabel.length;
            }
            return {
                id: e.CustId,
                value: e.CustId,
                text: e.WhiteLabel,
                checked: true,
                enable: true,
            };
        });
        maxWhiteLabelLength *= 11;
        this.filterComponents.registerDropdownComponent({
            dropdownId: this.typeReport + "-ddl-white-label",
            dropdownName: "WhiteLabels",
            dropdownData: mappedDataDropDown,
            dropdownType: "fanexMultiSelect",
            minWidth: maxWhiteLabelLength > 200 ? maxWhiteLabelLength : 200
        });
    };
    DailyWinLossReport.prototype.initUsernameFilter = function () {
        var usernameInput = {
            inputName: "username",
            inputId: this.typeReport + "-username-filter",
        };
        this.filterComponents.registerInputComponent(usernameInput);
        $("#" + usernameInput.inputId).keyup(function (e) {
            $(e.target).val().length > 1 && Fanex.ExtAcc.InitSuggestion("" + usernameInput.inputId);
        });
    };
    DailyWinLossReport.prototype.submit = function () {
        var _this = this;
        Fanex.ExtAcc.BlockUI();
        if (this.unValidUserName()) {
            Fanex.ExtAcc.UnBlockUI();
            this.gridComponent.hideNoInfo(false);
        }
        else {
            $.ajax({
                url: resolveClientUrl("WhiteLabelReport/GetDailyWinLoss" + this.getQueryString()),
                type: "post",
                dataType: "json",
                cache: false,
                success: function (result) { return _this.submitSuccess(result); }
            });
        }
    };
    DailyWinLossReport.prototype.getQueryString = function () {
        var _a = this.filterComponents.getAllFilterValues(), inputParams = _a.inputParams, dropdownParams = _a.dropdownParams, calendarParams = _a.calendarParams;
        return [
            '?FromDate=', calendarParams.fromDate,
            '&ToDate=', calendarParams.toDate,
            '&WhiteLabels=', dropdownParams[0].currentValue,
            '&Username=', inputParams[0].currentValue,
        ].join('');
    };
    DailyWinLossReport.prototype.unValidUserName = function () {
        var inputParams = this.filterComponents.getAllFilterValues().inputParams;
        return inputParams[0].currentValue.includes("script");
    };
    DailyWinLossReport.prototype.submitSuccess = function (result) {
        if (result.errorCode) {
            Fanex.ExtAcc.UnBlockUI();
            Fanex.ExtAcc.Form.Alert("Error", result.errorMessage.formatErrorMessage(), 350, 150, "alertWLD");
            return;
        }
        if (result.Items.length) {
            this.gridComponent.hideNoInfo(true);
            this.renderGrid(result);
            this.registerExportExcelBtn();
        }
        else {
            this.gridComponent.hideNoInfo(false);
        }
        Fanex.ExtAcc.UnBlockUI();
    };
    DailyWinLossReport.prototype.registerExportExcelBtn = function () {
        var _this = this;
        $("#" + this.gridComponentConfig.excelBtnId).off().click(function () {
            var url = resolveClientUrl("WhiteLabelReport/ExportDailyWinLoss" + _this.getQueryString());
            window.top.exportExcel(url, document);
        });
    };
    DailyWinLossReport.prototype.renderGrid = function (data) {
        try {
            var gridOptions = this.gridComponent.getGridOptions();
            gridOptions.titleInfo.text = this.reportTitle();
            gridOptions.bodyRows = this.gridComponent.buildBodyRows(data.Items);
            gridOptions.footerRows = this.gridComponent.buildFooterRows(data.RowTotal);
            setPagerOptionOnMobile(gridOptions);
            this.gridComponent.setGridOptions(gridOptions);
            this.gridComponent.fanexGrid(gridOptions);
            workaroundForGridOnPhone();
        }
        catch (e) {
            console.log(e);
        }
    };
    DailyWinLossReport.prototype.reportTitle = function () {
        var _a = this.filterComponents.getCalendarValues(), fromDate = _a.fromDate, toDate = _a.toDate;
        var formatedFromDate = moment(fromDate, 'MM/DD/YYYY').format('MMM DD, YYYY');
        var timeInfo = fromDate === toDate
            ? "on " + formatedFromDate
            : "from " + formatedFromDate + " to " + moment(toDate, 'MM/DD/YYYY').format('MMM DD, YYYY');
        return "White Label Daily Win Loss Report " + timeInfo;
    };
    return DailyWinLossReport;
}());
$(function () {
    new DailyWinLossReport().init();
});
//# sourceMappingURL=DailyWinLossReport.js.map