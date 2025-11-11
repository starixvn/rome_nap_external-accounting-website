(function ($) {

    var logger = { error: $.noop };

    Date.prototype.toStringFormat = function (format) {

        var shortMonthsInYear = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
            longMonthsInYear = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"],
            year = this.getFullYear(),
            month = this.getMonth(),
            day = this.getDate(),
            result = format;

        function strMonth(value) {
            var monthArrayIndex = parseInt(value);
            return shortMonthsInYear[monthArrayIndex] || value;
        }

        function strLongMonth(value) {
            var monthArrayIndex = parseInt(value);
            return longMonthsInYear[monthArrayIndex] || value;
        }

        result = result.replace(/yyyy/g, year);
        result = result.replace(/yy/g, year.toString().substr(2));

        result = result.replace(/MMMM/g, strLongMonth(month));
        result = result.replace(/MMM/g, strMonth(month));
        result = result.replace(/MM/g, month < 9 ? '0' + (month + 1) : (month + 1));

        result = result.replace(/dd/g, day < 10 ? '0' + day : day);

        return result;
    };

    var isInteger = function (value, writeLogToConsole) {
        var isValid = Math.ceil(value) == Math.floor(value);

        if (writeLogToConsole && !isValid) logger.error('Oops! "' + value + '" is an invalid Integer');

        return isValid;
    }

    var isValidDate = function (value, writeLogToConsole) {
        var _value = new Date(value),
            isValid = isInteger(_value.getDate()) && isInteger(_value.getMonth()) && isInteger(_value.getFullYear());

        if (writeLogToConsole && !isValid) logger.error('Oops! "' + value + '" is an invalid Date');

        return isValid;
    }

    var today = null,
        thisYear = null,
        thisMonth = null,
        thisDay = null,
        firstDayThisYear = null,
        firstDayThisMonth = null,
        lastDayThisMonth = null,

    /*
    * Private methods 
    */

        _getData = function (target) {
            var data = $(target).data('monPickr');
            if (!data) {
                logger.error('Oops! No data of monPickr is available');
                return null;
            }
            return data;
        },

        _validateMonth = function (minDate, maxDate, year, month) {
            var minYear = minDate.getFullYear(),
                minMonth = minDate.getMonth(),
                maxYear = maxDate.getFullYear(),
                maxMonth = maxDate.getMonth(),

                outOfLowBoundary = year < minYear || (year == minYear && month < minMonth),
                outOfHighBoundary = year > maxYear || (year == maxYear && month > maxMonth);

            return !outOfLowBoundary && !outOfHighBoundary;
        },

        _validateDate = function (minDate, maxDate, year, month, date) {
            // Re-new the params cauz of time ignorance
            var minDayInMonth = new Date(year, month, _getMinDayInMonth(minDate, year, month)),
                maxDayInMonth = new Date(year, month, _getMaxDayInMonth(maxDate, year, month)),
                date = new Date(date.getFullYear(), date.getMonth(), date.getDate());

            return minDate <= date && date <= maxDayInMonth;
        },

        _setMonth = function (data, year, month) {
            data.year = year;
            data.month = month;
        },

        _setMonthLabel = function (data) {
            data.lblSelectedMonth.html(new Date(data.year, data.month, 1).toStringFormat(data.monthFormat));
        },

        _selectMonth = function (data, year, month) {
            // invokes handler (if any) just before the event change month
            if (typeof data.beforeChangeMonth == 'function') data.beforeChangeMonth();
            else { logger.error('Oops! beforeChangeMonth event\'s handler (which has type "' + typeof data.beforeChangeMonth + '") is not a function'); }

            // highlight selected month
            data.dropDown.find('.monthWrap a[month].selected').removeClass('selected');
            data.dropDown.find('.monthWrap[year=' + year + '] a[month=' + (month + 1) + ']:first').addClass('selected');

            _setMonth(data, year, month);
            _setMonthLabel(data);

            // if selected year differs from current year then hide current year and show selected one
            if (data.dropDown.find('.monthWrap.selected').attr('year') != year) {
                _hideCurrentYear(data);
                _showSelectedYear(data, year);
            }

            if (data.showDateRange) {
                var minDayInMonth = _getMinDayInMonth(data.minDate, data.year, data.month),
                maxDayInMonth = _getMaxDayInMonth(data.maxDate, data.year, data.month);

                // change fromDate and toDate
                _selectDay(data, data.ddlFromDate, minDayInMonth, true);
                _selectDay(data, data.ddlToDate, maxDayInMonth, false);

                // re-render fromDay & toDay range
                var limitDayRange = function (idx) {
                    var $this = $(this),
                    day = idx + 1;

                    if (day < minDayInMonth || maxDayInMonth < day) $this.hide();
                    else $this.show();
                };

                data.ddlFromDate.dropDown.find('li').each(limitDayRange);
                data.ddlToDate.dropDown.find('li').each(limitDayRange);
            }

            // invokes handler (if any) just after the event change month
            if (typeof data.afterChangeMonth == 'function') data.afterChangeMonth();
            else { logger.error('Oops! afterChangeMonth event\'s handler (which has type "' + typeof data.afterChangeMonth + '") is not a function'); }
        },

        _setDay = function (data, value, isFromDay) {
            if (isFromDay) {
                data.fromDay = parseInt(value, 10);
                data.fromDate = new Date(data.year, data.month, value);
            } else {
                data.toDay = parseInt(value, 10);
                data.toDate = new Date(data.year, data.month, value);
            }
        },

        _setDayLabel = function (target, value) {
            target.find('.selectedDay').html(value);
        },

        _selectDay = function (data, target, value, isFromDay) {
            // invokes handler (if any) just before the event change day
            if (typeof data.beforeChangeDay == 'function') data.beforeChangeDay();
            else { logger.error('Oops! beforeChangeDay event\'s handler (which has type "' + typeof data.beforeChangeDay + '") is not a function'); }

            _setDayLabel(target, value);
            _setDay(data, value, isFromDay);

            // invokes handler (if any) just after the event change day
            if (typeof data.afterChangeDay == 'function') data.afterChangeDay();
            else { logger.error('Oops! afterChangeDay event\'s handler (which has type "' + typeof data.afterChangeDay + '") is not a function'); }
        },

        _getMinDayInMonth = function (minDate, year, month) {
            var _firstDayOfMonth = new Date(year, month, 1),
                _minDate = new Date(minDate.getFullYear(), minDate.getMonth(), minDate.getDate());

            return _minDate > _firstDayOfMonth ? _minDate.getDate() : 1;
        },

        _getMaxDayInMonth = function (maxDate, year, month) {
            var _lastDayOfMonth = new Date(year, month + 1, 0),
                _maxDate = new Date(maxDate.getFullYear(), maxDate.getMonth(), maxDate.getDate());

            return _lastDayOfMonth > _maxDate ? _maxDate.getDate() : _lastDayOfMonth.getDate();
        },

        _hideCurrentYear = function (data) {
            // collapse current year
            var currentMonthWrap = data.dropDown.find('.monthWrap.selected').eq(0);

            if (data.dropDown.is(':hidden')) {
                currentMonthWrap.hide();
            } else {
                currentMonthWrap.slideUp(data.showYearSpeed);
            }

            currentMonthWrap.removeClass('selected');
            data.dropDown.find('.year .selected').eq(0).removeClass('selected');
        },

        _showSelectedYear = function (data, year) {
            var target = data.dropDown.find('.monthWrap[year=' + year + ']').eq(0);

            target.prev('.year').find('a[year]').eq(0).addClass('selected');
            target.addClass('selected').slideDown(data.showYearSpeed);
        },

        _showDropDown = function (data, target) {
            target.dropDown.slideDown(data.showDropDownSpeed);
        },

        _hideDropDown = function (data, target) {
            target.dropDown.slideUp(data.showDropDownSpeed);
        },

    _methods = {
        init: function (options) {
            if ($.isFunction(options.log)) logger.error = options.log;

            if (!this.length) { // if the initial DOM does not exist
                logger.error('Oops! DOM element "' + this.selector + '" does not exist');
                return;
            }

            if (!isValidDate(options.today, true)) return;

            today = options.today ? new Date(options.today) : new Date();
            thisYear = today.getFullYear();
            thisMonth = today.getMonth();
            thisDay = today.getDate();
            firstDayThisYear = new Date(thisYear, 0, 1);
            firstDayThisMonth = new Date(thisYear, thisMonth, 1);
            lastDayThisMonth = new Date(thisYear, thisMonth + 1, 0);

            // Create some defaults, extending them with any options that were provided
            var settings = $.extend({
                minDate: firstDayThisYear,
                maxDate: today,
                month: today,

                showDateRange: false,
                fromDay: firstDayThisMonth.getDate(),
                toDay: today.getDate(),
                fromDayId: null, // required if showDateRange is true
                toDayId: null, // required if showDateRange is true

                expandYearOnhover: false,
                expandDayOnhover: false,
                showYearSpeed: 110,
                showDropDownSpeed: 10,

                disabled: false,
                monthFormat: 'MMM yyyy',
                nextText: 'Next Month',
                prevText: 'Previous Month',
                monPickrText: 'Show/hide Month Picker',
                zIndex: 9999,

                beforeChangeMonth: function () { },
                afterChangeMonth: function () { },
                beforeChangeDay: function () { },
                afterChangeDay: function () { }
            }, options);

            //#region VALIDATES INPUT VALUES

            // Validates minDate & maxDate
            if (!isValidDate(settings.minDate, true) || !isValidDate(settings.maxDate, true)) return;

            // Convert minDate & maxDate to Date type for easier later using
            settings.minDate = new Date(settings.minDate);
            settings.maxDate = new Date(settings.maxDate);

            // Validtes selected month
            if (!isValidDate(settings.month, true)) return;

            // Convert selected month to Date type for easier later using
            settings.month = new Date(settings.month);

            var year = settings.month.getFullYear(),
                month = settings.month.getMonth();

            // Make sure selected month are not out of range (minDate ~ maxDate)
            if (!_validateMonth(settings.minDate, settings.maxDate, year, month)) {
                logger.error('Oops! Selected month (' + settings.month.toStringFormat('MM/yyyy') +
                    ') is out of range (' + settings.minDate.toStringFormat('MM/yyyy') + ' ~ ' + settings.maxDate.toStringFormat('MM/yyyy') + ')');
                return;
            }

            // Makes sure selected fromDate & toDate are not out of Range (minDate ~ maxDate)
            var fromDate = new Date(year, month, settings.fromDay),
                toDate = new Date(year, month, settings.toDay);

            if (!_validateDate(settings.minDate, settings.maxDate, year, month, fromDate)) {
                logger.error('Oops! fromDate (' + fromDate.toStringFormat('MMM dd, yyyy') + ') is out of range');
                return;
            }

            if (!_validateDate(settings.minDate, settings.maxDate, year, month, toDate)) {
                logger.error('Oops! toDate (' + toDate.toStringFormat('MMM dd, yyyy') + ') is out of range');
                return;
            }

            //#endregion VALIDATES INPUT VALUES

            return this.each(function () {

                var $this = $(this),
                    data = $this.data('monPickr');

                // Initialize the plugin if it hasn't
                if (!data) {
                    $this.data('monPickr', {
                        target: $this,

                        minDate: settings.minDate,
                        maxDate: settings.maxDate,
                        originalYear: settings.year,
                        originalMonth: settings.month,
                        year: settings.month.getFullYear(),
                        month: settings.month.getMonth(),

                        showDateRange: settings.showDateRange,
                        originalFromDay: settings.fromDay,
                        originalToDay: settings.toDay,
                        fromDay: settings.fromDay,
                        toDay: settings.toDay,
                        fromDate: fromDate,
                        toDate: toDate,
                        fromDayId: settings.fromDayId,
                        toDayId: settings.toDayId,

                        expandYearOnhover: settings.expandYearOnhover,
                        expandDayOnhover: settings.expandDayOnhover,
                        showYearSpeed: settings.showYearSpeed,
                        showDropDownSpeed: settings.showDropDownSpeed,

                        disabled: settings.disabled,
                        monthFormat: settings.monthFormat,
                        nextText: settings.nextText,
                        prevText: settings.prevText,
                        monPickrText: settings.monPickrText,
                        zIndex: settings.zIndex,

                        beforeChangeMonth: settings.beforeChangeMonth,
                        afterChangeMonth: settings.afterChangeMonth,
                        beforeChangeDay: settings.beforeChangeDay,
                        afterChangeDay: settings.afterChangeDay
                    });

                    data = $this.data('monPickr')
                }

                //#region INIT DOM ELEMENTS

                //#region Month Picker

                $this.addClass('monPickr');
                data.btnPrev = $('<a href="javascript:;" class="prev" title="' + data.prevText + '"></a>');
                data.btnNext = $('<a href="javascript:;" class="next" title="' + data.nextText + '"></a>');
                data.lblSelectedMonth = $('<span class="selectedMonth" title="' + data.monPickrText + '"></span>');
                _setMonthLabel(data);
                $this.append(data.btnPrev, data.lblSelectedMonth, data.btnNext);

                $this.wrap('<div class="monPickrWrap" style="z-index: ' + data.zIndex + '" />');
                data.wrap = $this.parents('.monPickrWrap');
                data.wrap.wrap('<div class="monPickrWrapParent" style="position: relative" />')
                    .css({ 'position': 'absolute' }).parent().width(data.wrap.outerWidth()).height(data.wrap.outerHeight());

                data.dropDown = $('<div class="monPickrDropDown" />');

                var currentYear = data.minDate.getFullYear(),
                    currentMonth = data.minDate.getMonth(),
                    maxYear = data.maxDate.getFullYear(),
                    maxMonth = data.maxDate.getMonth(),
                    dropDownInner, temp;

                dropDownInner = '<ul class="monPickrDropDownInner">';

                while (currentYear < maxYear || (currentYear == maxYear && currentMonth <= maxMonth)) {
                    temp = currentYear == data.year ? 'selected' : '';
                    temp += data.expandYearOnhover ? ' hover' : '';

                    dropDownInner +=
                        '<li class="year">' +
                            '<a year="' + currentYear + '" class="' + temp + '" href="javascript:;">' +
                                currentYear +
                            '</a>' +
                        '</li>';

                    if (currentYear == data.year) {
                        dropDownInner += '<li class="monthWrap selected" year="' + currentYear + '" style="display: list-item"><ul>';
                    } else {
                        dropDownInner += '<li class="monthWrap" year="' + currentYear + '"><ul>';
                    }

                    while ((currentYear < maxYear || (currentYear == maxYear && currentMonth <= maxMonth)) && currentMonth < 12) {
                        temp = currentYear == data.year && currentMonth == data.month ? 'class="selected"' : '';

                        dropDownInner +=
                        '<li>' +
                            '<a month="' + (currentMonth + 1) + '" ' + temp + ' href="javascript:;">' +
                                new Date(currentYear, currentMonth).toStringFormat('MMMM') + '</a>'
                        '<li>';

                        currentMonth++;
                    }

                    dropDownInner += '</ul></li>';

                    currentYear++;
                    currentMonth = 0;
                }

                dropDownInner += '</ul>';

                data.dropDown.append(dropDownInner);
                $this.after(data.dropDown);

                //#region Events

                // Toggle the dropdown when users click on selected month label
                data.wrap.on('click', function (e) {
                    if ($(e.target).hasClass('selectedMonth')) {
                        if (data.dropDown.is(':hidden')) {
                            _showDropDown(data, data);
                        } else {
                            _hideDropDown(data, data);
                        }
                    }
                    e.stopPropagation();
                });

                // Collapse all dropdowns when users click outside the picker
                $('html').on('click', function () {
                    _hideDropDown(data, data);
                });

                // Toggle a dropdown when users click/hover on its "the year" link
                var expectedEvent = data.expandYearOnhover ? 'mouseenter' : 'click';
                data.dropDown.on(expectedEvent, '.year a', function () {
                    var $this = $(this), year = $this.attr('year'),
                        thisIsNotSelected = $this.hasClass('selected') == false;

                    switch (expectedEvent) {
                        case 'mouseenter':
                            {
                                if (thisIsNotSelected) {
                                    _hideCurrentYear(data);
                                    _showSelectedYear(data, year); // expand this
                                }
                            } break;
                        case 'click':
                            {
                                _hideCurrentYear(data);

                                if (thisIsNotSelected) {
                                    // We have to expand "the year" which was clicked.
                                    // Else means this is the current year itself --> we don't need to expand it
                                    _showSelectedYear(data, year);
                                }
                            } break;
                    }
                });

                // Change current month when users select a month
                data.dropDown.on('click', '.monthWrap a[month]', function () {
                    var $this = $(this);
                    _selectMonth(data, $this.parents('.monthWrap').attr('year'), $this.attr('month') - 1);
                    _hideDropDown(data, data);
                });

                // Move to next month when users click next button
                data.btnNext.on('click', function () {
                    var nextMonth = data.month + 1,
                        year = data.year;

                    if (nextMonth > 11) {
                        nextMonth = 0;
                        year++;
                    }

                    if (_validateMonth(data.minDate, data.maxDate, year, nextMonth)) {
                        _selectMonth(data, year, nextMonth);
                    }
                });

                // Move to previous month when users click prev button
                data.btnPrev.on('click', function () {
                    var prevMonth = data.month - 1,
                        year = data.year;

                    if (prevMonth < 0) {
                        prevMonth = 11;
                        year--;
                    }

                    if (_validateMonth(data.minDate, data.maxDate, year, prevMonth)) {
                        _selectMonth(data, year, prevMonth);
                    }
                });

                //#endregion Events

                //#endregion Month Picker

                //#region Day Picker

                if (data.showDateRange) { // Renders date range component (fromDate & toDate)

                    var renderDayPicker = function (data, dayPickr, dayPickrId, selectedDay) {

                        dayPickr = $('#' + dayPickrId);

                        if (dayPickr.length > 0) {
                            dayPickr.addClass('dayPickr')
                                .append('<span class="selectedDay">' + selectedDay + '</span>')
                                .append('<a class="darr" href="javascript:;"></a>')
                                .wrap('<div class="selectedDayWrap" style="z-index: ' + data.zIndex + '" />');

                            dayPickr.wrap = dayPickr.parents('.selectedDayWrap')
                                .wrap('<div class="selectedDayWrapParent" style="position: relative" />')
                                .css({ 'position': 'absolute' });
                            dayPickr.wrap.parent().width(dayPickr.wrap.outerWidth()).height(dayPickr.wrap.outerHeight());
                            dayPickr.dropDown = $('<div class="dayPickrDropDown" />');

                            var currentDay = 1,
                                minDay = _getMinDayInMonth(data.minDate, data.year, data.month),
                                maxDay = _getMaxDayInMonth(data.maxDate, data.year, data.month),
                                dayPickrDropDown;

                            dayPickrDropDown = '<ul>';

                            while (currentDay <= 31) {
                                temp = currentDay < minDay || currentDay > maxDay ? 'style="display: none"' : '';
                                dayPickrDropDown += '<li ' + temp + '><a href="javascript:;">' + currentDay + '</a></li>';
                                currentDay++;
                            }

                            dayPickrDropDown += '</ul>';

                            dayPickr.dropDown.append(dayPickrDropDown);
                            dayPickr.after(dayPickr.dropDown);

                            return dayPickr;
                        } else {
                            logger.error('Oops! fromDayId (' + data.fromDayId + ') or toDayId (' + data.toDayId + ') does not exist');
                        }
                    }

                    data.ddlFromDate = renderDayPicker(data, data.ddlFromDate, data.fromDayId, data.fromDay);
                    data.ddlToDate = renderDayPicker(data, data.ddlToDate, data.toDayId, data.toDay);

                    //#region Events

                    var dayExpectedEvent = data.expandDayOnhover ? 'hover' : 'click',
                        toggleDayPickr = function (e) {
                            if (data.isClosingDropDown) {
                                data.isClosingDropDown = false;
                                return;
                            }

                            var target = e.data.target;

                            if (target.dropDown.is(':hidden')) {
                                _showDropDown(data, target);
                            } else {
                                _hideDropDown(data, target);
                            }
                            e.stopPropagation();
                        };

                    // Toggle the day picker dropdown when users click/hover on selected day label
                    data.ddlFromDate.wrap.on(dayExpectedEvent, { target: data.ddlFromDate }, toggleDayPickr);
                    data.ddlToDate.wrap.on(dayExpectedEvent, { target: data.ddlToDate }, toggleDayPickr);

                    if (!data.expandDayOnhover) {
                        // Collapse all day picker dropdowns when users click outside the picker
                        $('html').on('click', function () {
                            _hideDropDown(data, data.ddlFromDate);
                            _hideDropDown(data, data.ddlToDate);
                        });
                    }

                    var changeCurrentDay = function (e) {
                        var $this = $(this),
                            selectedDay = $this.find('a').html();

                        // prevent conflict when closing day dropdown
                        if (data.expandDayOnhover) data.isClosingDropDown = true;

                        $this.parents('.dayPickrDropDown').hide();
                        _selectDay(data, e.data.target, selectedDay, e.data.isFromDay);

                        e.stopPropagation();
                    };

                    // Change current fromDay/toDay when users select a day
                    data.ddlFromDate.dropDown.on('click', 'li', { target: data.ddlFromDate, isFromDay: true }, changeCurrentDay);
                    data.ddlToDate.dropDown.on('click', 'li', { target: data.ddlToDate, isFromDay: false }, changeCurrentDay);

                    //#endregion Events
                }

                //#endregion Day Picker

                //#endregion INIT DOM ELEMENTS
            });
        },

        // Returns the current month or null if the invoker's data does not exist
        // format may be '' which tell this to use default format
        getMonth: function (format) {
            var data = _getData(this);
            if (!data) return null;

            if (format != undefined) {
                format = format.length > 0 ? format : data.monthFormat;
                return new Date(data.year, data.month).toStringFormat(format);
            } else {
                return data.month + 1;
            }
        },

        // Sets the current month for the month picker.
        // value may be a Date object or a string but it must be a full date (include year, month & day)
        setMonth: function (value) {
            var data = _getData(this);
            if (!data) return;

            var year, month, _value = value;

            if ((typeof value).toLowerCase() != 'object') {
                _value = new Date(_value);
            }

            year = _value.getFullYear();
            month = _value.getMonth();

            if (isInteger(year) && isInteger(month)) {
                if (_validateMonth(data.minDate, data.maxDate, year, month)) {
                    _selectMonth(data, year, month);
                } else {
                    logger.error('Oops! Can\'t set month because input (' + value.toString() +
                        ') is out of range (' + data.minDate.toStringFormat('MMM yyyy') + ' ~ ' + data.maxDate.toStringFormat('MMM yyyy') + ')');
                }
            } else {
                logger.error('Oops! Can\'t set month because of invalid input (' + value.toString() + ')');
            }
        },

        // Returns the current fromDate or null if the invoker's data does not exist
        // format may be '' which tell this to use default format
        getFromDate: function (format) {
            var data = _getData(this);
            if (!data) return null;

            if (format != undefined) {
                format = format.length > 0 ? format : data.monthFormat;
                return data.fromDate.toStringFormat(format);
            }

            return data.fromDate;
        },

        // Returns the current toDate or null if the invoker's data does not exist
        // format may be '' which tell this to use default format
        getToDate: function (format) {
            var data = _getData(this);
            if (!data) return null;

            if (format != undefined) {
                format = format.length > 0 ? format : data.monthFormat;
                return data.toDate.toStringFormat(format);
            }

            return data.toDate;
        },

        // Sets the current fromDay
        setFromDay: function (value) {
            var data = _getData(this);
            if (!data) return;

            var selectedDate = new Date(data.year, data.month, value);

            if (isInteger(value) && _validateDate(data.minDate, data.maxDate, data.year, data.month, selectedDate)) {
                _selectDay(data, data.ddlFromDate, value, true);
            } else {
                logger.error('Oops! input value (' + value + ') is invalid');
            }
        },

        // Sets the current toDay
        setToDay: function (value) {
            var data = _getData(this);
            if (!data) return;

            var selectedDate = new Date(data.year, data.month, value);

            if (isInteger(value) && _validateDate(data.minDate, data.maxDate, data.year, data.month, selectedDate)) {
                _selectDay(data, data.ddlToDate, value, false);
            } else {
                logger.error('Oops! input value (' + value + ') is invalid');
            }
        },

        // Resets picker to its original initial state
        reset: function () {
            var data = _getData(this);
            if (!data) return;

            _selectMonth(data, data.originalYear, data.originalMonth);

        }
    };

    /*
    * Public methods 
    */

    $.fn.monPickr = function (method) {

        if (_methods[method]) { // call a method but init
            return _methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) { // call init method
            return _methods.init.apply(this, arguments);
        } else { // 
            logger.error('Oops! Method "' + method + '" does not exist on jQuery.monPickr');
        }
    };

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
if (typeof Fanex == "undefined" || typeof Fanex.ExtAcc == "undefined") {
    alert("Fanex.ExtAcc.js module is required!");
}

NexDatePickerHelper = {
    getShortDateString: function (date) {
        // month is 0 based (0 - 11)
        var strDate = NexDatePickerHelper.getDateStr(date.getMonth() + 1) + "/";
        strDate += NexDatePickerHelper.getDateStr(date.getDate()) + "/";
        strDate += NexDatePickerHelper.getDateStr(date.getFullYear());
        return strDate;
    },
    getDateStr: function (date) {
        if (!date) {
            return '';
        }
        if (date < 10) {
            return '0' + date.toString();
        }
        return date.toString();
    },
    getDateRangeOfCurrentWeek: function (today) {
        var date = new Date(today);
        var lastDate = new Date(today);
        var baseDate = NexDatePickerHelper.getBaseDateOfWeek(date);
        var startDate = new Date(date.setDate(baseDate));

        return [startDate, lastDate];
    },
    getDateRangeOfLastWeek: function (today) {
        var baseDateOfCurrentWeek = NexDatePickerHelper.getBaseDateOfWeek(new Date(today)); // Monday
        var baseDateOfLastWeek = baseDateOfCurrentWeek - 7; // Seek 7 days

        var lastWeek = new Date(today);
        lastWeek.setDate(baseDateOfLastWeek);
        var startDate = NexDatePickerHelper.getShortDateString(lastWeek);

        lastWeek.setDate(lastWeek.getDate() + 6); // Seek 6 days: Monday->Sunday
        var endDate = NexDatePickerHelper.getShortDateString(lastWeek);

        return [new Date(startDate), new Date(endDate)];
    },
    getBaseDateOfWeek: function (date) {
        // Base date from Monday
        var day = date.getDay();            // Sun:0 -> Sat:6

        return day === 0                    // Sun:0: default of system
            ? date.getDate() - 6            // Sunday<-Mon: 6
            : date.getDate() - day + 1;     // other days<-Mon: date.getDay() - 1
    }
};

Fanex.ExtAcc.WinLossDetail = {
    groupId: "",
    groupName: "",
    username: "",
    srecommend: 0,
    mrecommend: 0,
    recommend: 0,
    custId: 0,
    supername: "",
    mastername: "",
    agentname: "",
    fromdate: new Date(),
    todate: new Date(),
    fixedsortdata: new Array(),
    removedsortdata: new Array(),
    fixedsortlist: new Array(),
    url: resolveClientUrl("WinLossDetail/GetData"),
    viewlevel: 5,
    dataSource: [],
    productDetailsData: [],
    productDetailsDataMapToProductId: null,
    $multiSelectProduct: null,
    $multiSelectProductDetail: null,
    $this: null,
    drillDownName: "",
    SetRange: function () {
        return {
            minDate: new Date($("#MinDate").val()),
            maxDate: new Date($("#MaxDate").val())
        };
    },
    SwitchCalendarType: function (showType) {
        this.ProductsList();

        if (showType == 1) {
            $("#GroupViewHistory").css("display", "none");
            $("#GroupBackToCurrent").css("display", "block");
            $('.from-to-date').css('display', 'none');
            $('.group-month-day').css('display', 'inline-block');
        }
        else {
            $("#GroupViewHistory").css("display", "block");
            $("#GroupBackToCurrent").css("display", "none");
            $('.from-to-date').css('display', 'inline-block');
            $('.group-month-day').css('display', 'none');
        }
    },
    CallSubmit: function () {
        this.groupName = $("#selectedGroupName").html();
        this.ResetParameters();
        this.username = Fanex.encodeHtml($("#txtUserName").val());
        this.Databind(this.username);
    },
    initMonthlyCalendar: function () {
        $("#monPickr1").monPickr({
            today: $("#MaxDate").val(),
            minDate: "2010/1/1",
            showDateRange: true,
            fromDayId: "fromDay1",
            toDayId: "toDay1",
            log: function (msg) {
                if (typeof console != "undefined")
                    console.warn(msg);
            }
        });

        var today = new Date($("#MaxDate").val());
        $("#monPickr1").monPickr("setMonth", today);
        $("#monPickr1").monPickr("setToDay", today.getDate());
        $("#monPickr1").monPickr("setFromDay", 1);
    },
    initLevelSelection: function (options) {
        $(`#${options.dropdownId}`).empty().select2({
            placeholder: "Select",
            data: options.dropdownData,
            dropdownAutoWidth: true,
            templateSelection: function (data) {
                return data.text;
            },
            sorter: function (data) {
                return data;
            }
        }).bind("select2:select", function () {
            if (options.onClosed) {
                options.onClosed($(`#${options.dropdownId}`).val())
            }
        })
    },
    getDayOfYear: function (date) {
        var day = date.getDate();
        var month = date.getMonth() + 1;
        var year = date.getFullYear();
        var totalDays = day + (2 * month) + parseInt(3 * (month + 1) / 5) + year + parseInt(year / 4) - parseInt(year / 100) + parseInt(year / 400) + 2;
        var dayNo = (totalDays % 7);
        if (dayNo == 0) {
            dayNo = 7;
        }
        return dayNo;
    },
    getParameterProducts: function () {
        Fanex.ExtAcc.WinLossDetail.updateUncheckedProductsDropdown();
        var productIds = Fanex.ExtAcc.WinLossDetail.$multiSelectProduct.fanexMultiSelect("getItems", true);
        return productIds.length ? "&productIds=" + productIds.join(",") : "";
    },
    updateUncheckedProductsDropdown: function () {
        if (Fanex.ExtAcc.WinLossDetail.productDetailsDataMapToProductId) {
            Object.keys(Fanex.ExtAcc.WinLossDetail.productDetailsDataMapToProductId).forEach(function (productId) {
                var resourceIds = Fanex.ExtAcc.WinLossDetail.productDetailsDataMapToProductId[productId];
                if (Fanex.ExtAcc.WinLossDetail.isProductDetailsUnChecked(resourceIds)) {
                    $('input[id^="fanex-multiselect-ddl-product-' + productId + '-"]').prop("checked", false).trigger("change");
                }
            });
        }
    },
    isProductDetailsUnChecked: function (resourceIds) {
        for (var i = 0, max = resourceIds.length; i < max; i++) {
            if ($('input[id^="fanex-multiselect-ddl-product-detail-' + resourceIds[i] + '-"]').is(":checked")) {
                return false;
            }
        }

        return true;
    },
    getParameterProductDetails: function () {
        if (Fanex.ExtAcc.WinLossDetail.$multiSelectProductDetail) {
            var resourceIds = Fanex.ExtAcc.WinLossDetail.$multiSelectProductDetail.fanexMultiSelect("getItems", true);
            return resourceIds.length ? "&resourceIds=" + resourceIds.join(",") : "";
        }

        return "";
    },
    ProductsList: function () {
        $.ajax({
            context: $this,
            url: resolveClientUrl("Filter/ProductsList"),
            type: "GET",
            data: { groupName: this.groupName, groupId: this.groupId, isWinLossDetail: true },
            dataType: "json",
            cache: false,
            success: function (result) {
                var productDetails = result.productDetails;
                if (productDetails.length) {
                    Fanex.ExtAcc.WinLossDetail.InitProductDetailsList(productDetails);
                }

                Fanex.ExtAcc.WinLossDetail.InitProductsList(result.products);
            }
        });
    },
    InitProductsList: function (products) {
        var $ddlProductDetail = $("#ddl-product-detail");
        //var $checkAllProductDetail = $("#ddl-product-detailHeaderCheckBox");
        var buildProductData = function (productList) {
            var dataList = [];
            $.each(productList, function (i, product) {
                dataList.push({
                    id: product.ProductId,
                    value: product.ProductId,
                    text: product.ProductName,
                    checked: true,
                    enable: true,
                    items: product.IsParent ? buildProductData(product.Products) : []
                });
            });

            return dataList;
        };

        Fanex.ExtAcc.WinLossDetail.$multiSelectProduct = $("#ddl-product").fanexMultiSelect({
            checkAllText: "Select all",
            noneSelectedText: "Select Products",
            useCustomCheckAllText: true,
            customCheckAllText: "All Products",
            isCheckAll: true,
            listData: buildProductData(products),
            onCheck: function () {
                if ($ddlProductDetail.length) {
                    var checked = this.checked;
                    var productId = parseInt(this.value);

                    if (Number.isNaN(productId)) { // Select all
                        Fanex.ExtAcc.WinLossDetail.ProductsListOnCheckAll(checked, $ddlProductDetail);
                    } else {
                        var $groupCheckbox = $(this);
                        if ($groupCheckbox.data("is-group")) {
                            Fanex.ExtAcc.WinLossDetail.ProductsListOnCheckGroup(checked, $ddlProductDetail, $groupCheckbox);
                        } else {
                            Fanex.ExtAcc.WinLossDetail.ProductsListOnCheck(checked, productId, $ddlProductDetail);
                        }
                    }
                }
            }
        });
    },
    ProductsListOnCheckAll: function (checked, $ddlProductDetail) {
        $ddlProductDetail.fanexMultiSelect("setCheckAll", checked);
        $ddlProductDetail.fanexMultiSelect("setEnable", checked);
        var optionsElement = $ddlProductDetail.next(".fanex-multiselect-options");
        $.each(optionsElement.find('input[type="checkbox"].show-item'), function (i, j) {
            if ($(j).val) {
                $(j).prop("checked", checked);
                $(j).prop("disabled", !checked);
                $(j).trigger('change');
            }

            const attr = $(j).attr('disabled');
            if (typeof attr !== 'undefined' && attr !== false) {
                $(j).parent().addClass("hide-item");
            } else {
                $(j).parent().removeClass("hide-item");
            }
        });
    },

    ProductsListOnCheckGroup: function (checked, $ddlProductDetail, $groupCheckbox) {
        $groupCheckbox
            .closest(".fanex-multiselect-div")
            .next(".fanex-multiselect-ul-group")
            .find('input[type="checkbox"]')
            .each(function () {
                Fanex.ExtAcc.WinLossDetail.ProductsListOnCheck(checked, parseInt(this.value), $ddlProductDetail);
            });
    },
    ProductsListOnCheck: function (checked, productId, $ddlProductDetail) {
        var filteredProductDetails = Fanex.ExtAcc.WinLossDetail.productDetailsData.filter(function (p) {
            if (p.ProductId === productId) {
                return p;
            }
        });

        if (!filteredProductDetails.length) {
            return;
        }

        var optionsElement = $ddlProductDetail.next(".fanex-multiselect-options");
        $.each(filteredProductDetails, function (index, product) {
            optionsElement
                .find('input[type="checkbox"][value="' + product.ResourceId + '"]')
                .prop("checked", checked)
                .prop("disabled", !checked)
                .trigger('change');

        });

        $.each(optionsElement.find('input[type="checkbox"]'), function (i, p) {
            const attr = $(p).attr('disabled');
            if (typeof attr !== 'undefined' && attr !== false) {
                $(p).parent().addClass("hide-item");
            } else {
                $(p).parent().removeClass("hide-item");
            }
        })

        const maxAnabledItem = optionsElement.find('input[type="checkbox"].show-item:enabled').length;
        const checkedItem = optionsElement.find('input[type="checkbox"].show-item:enabled:checked').length;
        if (maxAnabledItem === checkedItem) {
            $("#ddl-product-detailHeaderCheckBox").prop("checked", true);
        }

        var enabledCheckboxes = optionsElement.find('input[type="checkbox"]:enabled').length - 1;
        if (checked) {
            $ddlProductDetail.fanexMultiSelect("setEnable", checked);
        } else if (!enabledCheckboxes) {
            $ddlProductDetail.fanexMultiSelect("setEnable", checked);
        }
    },

    InitProductDetailsList: function (productDetails) {
        var dataList = [];
        var dataListMapToProductId = {};
        $.each(productDetails, function (i, item) {
            dataList.push({
                id: item.ResourceId,
                value: item.ResourceId,
                text: item.DisplayName,
                checked: true,
                enable: true
            });

            if (dataListMapToProductId.hasOwnProperty(item.ProductId)) {
                dataListMapToProductId[item.ProductId].push(item.ResourceId);
            } else {
                dataListMapToProductId[item.ProductId] = [item.ResourceId];
            }
        });

        Fanex.ExtAcc.WinLossDetail.$multiSelectProductDetail = $("#ddl-product-detail").fanexMultiSelect(
            {
                minWidth: 220,
                checkAllText: "Select all",
                noneSelectedText: "Select none",
                useCustomCheckAllText: true,
                customCheckAllText: "Select all",
                isCheckAll: true,
                listData: dataList
            });

        $("#ddl-product-detail-container").removeClass("hidden");
        Fanex.ExtAcc.WinLossDetail.productDetailsData = productDetails;
        Fanex.ExtAcc.WinLossDetail.productDetailsDataMapToProductId = dataListMapToProductId;
    },
    Init: function () {
        $this = this;
        Fanex.ExtAcc.InitSuggestion();

        $("#DateList").combobox({
            selected: function (event, ui) {
                var dateRange;
                var itemIndex = event.target.selectedIndex;
                $("#DateList").prop("selectedIndex", itemIndex);

                var fdate = new Date($("#MaxDate").val());
                var tdate = new Date($("#MaxDate").val());
                if (itemIndex == 1) {
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth(), tdate.getDate() - 1);
                    tdate = fdate;
                } else if (itemIndex == 2) {
                    dateRange = NexDatePickerHelper.getDateRangeOfCurrentWeek(tdate);
                    fdate = dateRange[0];
                } else if (itemIndex == 3) {
                    dateRange = NexDatePickerHelper.getDateRangeOfLastWeek(tdate);
                    fdate = dateRange[0];
                    tdate = dateRange[1];
                } else if (itemIndex == 4) {
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth(), 1);
                    tdate = new Date(tdate.getFullYear(), tdate.getMonth(), tdate.getDate());
                } else if (itemIndex == 5) {
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth() - 1, 1);
                    tdate = new Date(tdate.getFullYear(), tdate.getMonth(), 0);
                } else if (itemIndex == 6) {
                    fdate = new Date(tdate.getFullYear(), tdate.getMonth() - 1, 1);
                } else if (itemIndex == 7) {
                    return;
                }

                $("#dtpFromDate").val($.datepicker.formatDate("M dd, yy", fdate));
                $("#dtpToDate").val($.datepicker.formatDate("M dd, yy", tdate));
            }
        });

        $("#userGroup input").attr("readonly", "true");
        $("#userGroup input").bind("click", function () {
            $(this).autocomplete("search", "");
        });

        $("#userGroup input").val($("#DateList")[0].options[0].value);

        $("#dtpFromDate").datepicker({
            onClose: function (date) {
                $("#userGroup input").val($("#DateList")[0].options[7].value);
            }, showOn: "both", buttonImageOnly: true, buttonImage: resolveClientUrl("Content/Images/calendar.png"), buttonText: "", beforeShow: $this.SetRange, dateFormat: "M dd, yy"
        });
        $("#dtpFromDate").val($.datepicker.formatDate("M dd, yy", new Date($("#MaxDate").val())));

        $("#dtpToDate").datepicker({
            onClose: function (date) {
                $("#userGroup input").val($("#DateList")[0].options[7].value);
            }, showOn: "both", buttonImageOnly: true, buttonImage: resolveClientUrl("Content/Images/calendar.png"), buttonText: "", beforeShow: $this.SetRange, dateFormat: "M dd, yy"
        });
        $("#dtpToDate").val($.datepicker.formatDate("M dd, yy", new Date($("#MaxDate").val())));

        $(".ui-datepicker").draggable();

        $this.initMonthlyCalendar();

        $("#ddlGroupName").bind("change", function (event) {
            var args = event.args;
            if (args) {
                var item = args.item;
                $this.groupName = item.innerHTML;

                $this.ResetParameters();
                $this.InitShowHideControls("none");
            }
        });

        this.groupId = Fanex.ExtAcc.Customer.GetGroupId() || "";
        $this.groupName = $("#selectedGroupName").html();

        this.ProductsList();
        
        $("#Submit").click(function (e) {
            $this.CallSubmit();
        });

        $("#txtUserName").keyup(function (e) {
            if ($("#txtUserName").val().length > 1) {
                Fanex.ExtAcc.InitSuggestion();
            }
        });

        $("#exporttoexcel").click(function () {
            var url = "";
            var params = "";
            if ($this.viewlevel !== 0) { 
                 params = "fromdate=" + $this.fromdate +
                    "&todate=" + $this.todate +
                    "&viewlevel=" + $this.viewlevel +
                    "&custid=" + $this.custId +
                    "&groupId=" + Fanex.ExtAcc.WinLossDetail.groupId +
                    "&groupname=" + $this.groupName +
                    "&username=" + Fanex.ExtAcc.WinLossDetail.username;
                url = resolveClientUrl("WinLossDetail/ExportWinLossDetailToExcel?") +
                    params +
                    Fanex.ExtAcc.WinLossDetail.getParameterProducts() +
                    Fanex.ExtAcc.WinLossDetail.getParameterProductDetails();
        } else {
                params = "fromdate=" + $this.fromdate +
                    "&todate=" + $this.todate +
                    "&custid=" + $this.custId +
                    "&username=" + $this.drillDownName;
                url = resolveClientUrl("WinLossDetail/ExportCashFeeToExcel?") + params;
        }

            top.exportExcel(url, document);
        });

        $("#ViewHistoryReport").click(function () {
            $this.SwitchCalendarType(1);
        });
        $("#BackToCurrentReport").click(function () {
            $this.SwitchCalendarType(0);
        });

        $("#tabSuper").click(function (e) {
            $this.viewlevel = 4;
            $this.srecommend = 0;
            $this.mrecommend = 0;
            $this.recommend = 0;

            if ($this.custId !== 0) {
                $this.custId = 0;
            }

            $this.ToggleTabs('none', 'none', 'none', 'none', 4, 'none');
            $this.Databind($this.supername);
        });

        $("#tabMaster").click(function (e) {
            $this.viewlevel = 4;
            $this.custId = $this.mastername === "" ? $this.srecommend : 0;
            $this.ToggleTabs('block', 'block', 'none', 'none', 3, 'none');

            $this.Databind($this.mastername);
        });

        $("#tabAgent").click(function (e) {
            $this.viewlevel = 3;
            $this.custId = $this.agentname === "" ? $this.mrecommend : 0;
            $this.ToggleTabs('block', 'block', 'block', 'none', 2, 'none');

            $this.Databind($this.agentname);
        });

        $("#tabMember").click(function (e) {
            $this.viewlevel = 2;
            $this.custId = $this.agentname === "" ? $this.recommend : 0;
            $this.ToggleTabs('block', 'block', 'block', 'none', 1, 'none');

            $this.Databind($this.agentname);
        });

        $("#txtUserName").keypress(function (e) {
            if (typeof e == "undefined") e = window.event;
            var key = (typeof e.keyCode == "undefined") ? e.which : e.keyCode;
            if (key == 13) {
                $this.CallSubmit();
                return true;
            } else {
                return true;
            }
        });

        $this.sorters = $(".col-sorter");
        $this.sorters.click(function () {
            var elem = $(this);
            this.sortTable(elem);
        });

        Fanex.ExtAcc.OnResize = $this.resizeReportGrid;
        $(".icon-filter").click($this.showHideFilter);

        $('#ViewExtraPT').click($this.GetExtraPT);
    },
    resizeReportGrid: function () {
        try {
            $("#jqgrid").fanexGrid("resizeGrid");
        }
        catch (err) { }
    },
    hideFilter: false,
    showHideFilter: function () {
        if ($this.hideFilter) {
            $(".icon-filter").addClass("active");

            $(".filterbox").removeClass("hide");
        } else {
            $(".icon-filter").removeClass("active");
            $(".filterbox").addClass("hide");
        }

        $this.resizeReportGrid();
        $this.hideFilter = !$this.hideFilter;
    },
    ToggleTabs: function (levelLine, statusSAg, statusMAg, statusAg, status, statusAgMember) {
        $("#tabSuper").css("display", statusSAg);
        $("#tabMaster").css("display", statusMAg);
        $("#tabAgent").css("display", statusAg);
        $("#tabMember").css("display", statusAgMember);

        $('#levelLine').css('display', levelLine);
        if (status == 1) {
            $("#SelectedLevel").html("Member");
        }
        else if (status == 2) {
            $("#SelectedLevel").html("Agent");
        }
        else if (status == 3) {
            $("#SelectedLevel").html("Master");
        }
        else if (status == 4) {
            $("#SelectedLevel").html("Super");
        }
    },

    RenderUsernameColumn: function (cell) {
        var isDormantAccounts = cell.Value.CustId === 0;

        if (cell.Value.Username === "CASH SERVICE BILLING") {
            var drillDownCashFeefunc = "javascript: Fanex.ExtAcc.WinLossDetail.GetCashFee('" + cell.Value.CustId + "','" + this.drillDownName + "')";
            return html = '<div class="cash-fee-row" style="line-height: 22px; text-align: left;">' +
                '<a class="text-truncate" style="text-decoration: none; margin: 0" href="' + drillDownCashFeefunc + '" title="' + cell.Value.Username + '">' + cell.Value.Username + '</a>' +
                '</div>';
        }

        if (cell == null || cell.Value.Username == "Grand Total" || $this.viewlevel < 2 || cell.Value.RoleId == 1) {
            return isDormantAccounts
                ? $this.RenderDormantAccountsCell()
                : '<div class="text-truncate" style="line-height: 22px; text-align: left; padding-left: 3px" title="' + cell.Value.Username + '">' + cell.Value.Username + "</div>";
        }

        if (isDormantAccounts) {
            return $this.RenderDormantAccountsCell();
        }

        var func = "javascript: Fanex.ExtAcc.WinLossDetail.GetDownlineWinLossDetail('" + cell.Value.CustId + "','" + cell.Value.Username + "')";
        return html = '<div style="line-height: 22px; text-align: left;">' +
            '<a class="text-truncate" style="text-decoration: none; margin: 0" href="' + func + '" title="' + cell.Value.Username + '">' + cell.Value.Username + '</a>' +
            '</div>';
    },
    RenderDormantAccountsCell: function () {
        return '<span style="line-height:22px;text-align:left;padding-left:3px">Dormant Accounts</span><span class="icon-alert-circle-outline" style="padding-left:3px" title="Summary of dormant accounts."></span>';
    },
    defaultColumns: [
        {
            HeaderText: "Username", Name: "Username", Width: 130, Sort: true, HeaderAlign: "center",
            CustomCellRender: function (cell) { return $this.RenderUsernameColumn(cell); }
        },
        { HeaderText: "Turnover", Name: "Turnover", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Net Turnover", Name: "NetTurnover", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "GrossComm", Name: "GrossComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Win/Loss", Name: "MemberWinlost", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Comm", Name: "PlayerComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Total", Name: "MemberTotal", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Win/Loss", Name: "AgWinlost", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Comm", Name: "AgComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Total", Name: "AgTotal", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Win/Loss", Name: "MAgWinlost", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Comm", Name: "MAgComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Total", Name: "MAgTotal", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Win/Loss", Name: "SAgWinlost", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Comm", Name: "SAgComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Total", Name: "SAgTotal", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Company", Name: "Company", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Win/Loss", Name: "ExtraWinLost", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Comm", Name: "ExtraComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Total", Name: "ExtraTotal", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Company (After Extra)", Name: "CompanyAfterExtraPT", Width: 180, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Reward", Name: "RewardAmount", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" }
    ],
    SAgColumns: [
        {
            HeaderText: "Username", Name: "Username", Width: 130, Sort: true, HeaderAlign: "center",
            CustomCellRender: function (cell) { return $this.RenderUsernameColumn(cell); }
        },
        { HeaderText: "Level", Name: "LevelType", Width: 130, Sort: true, HeaderAlign: "center" },
        { HeaderText: "Turnover", Name: "Turnover", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Net Turnover", Name: "NetTurnover", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "GrossComm", Name: "GrossComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Win/Loss", Name: "MemberWinlost", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Comm", Name: "PlayerComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Total", Name: "MemberTotal", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Win/Loss", Name: "AgWinlost", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Comm", Name: "AgComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Total", Name: "AgTotal", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Win/Loss", Name: "MAgWinlost", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Comm", Name: "MAgComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Total", Name: "MAgTotal", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Win/Loss", Name: "SAgWinlost", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Comm", Name: "SAgComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Total", Name: "SAgTotal", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Company", Name: "Company", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Reward", Name: "RewardAmount", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" }
    ],
    MAgColumns: [
        {
            HeaderText: "Username", Name: "Username", Width: 130,
            Sort: true, HeaderAlign: "center",
            CustomCellRender: function (cell) { return $this.RenderUsernameColumn(cell); }
        },
        { HeaderText: "Level", Name: "LevelType", Width: 130, Sort: true, HeaderAlign: "center" },
        { HeaderText: "Turnover", Name: "Turnover", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Net Turnover", Name: "NetTurnover", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "GrossComm", Name: "GrossComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Win/Loss", Name: "MemberWinlost", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Comm", Name: "PlayerComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Total", Name: "MemberTotal", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Win/Loss", Name: "AgWinlost", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Comm", Name: "AgComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Total", Name: "AgTotal", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Win/Loss", Name: "MAgWinlost", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Comm", Name: "MAgComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Total", Name: "MAgTotal", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Company", Name: "Company", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Reward", Name: "RewardAmount", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" }
    ],
    AgColumns: [
        {
            HeaderText: "Username", Name: "Username", Width: 130,
            Sort: true, HeaderAlign: "center",
            CustomCellRender: function (cell) { return $this.RenderUsernameColumn(cell); }
        },
        { HeaderText: "Turnover", Name: "Turnover", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Net Turnover", Name: "NetTurnover", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "GrossComm", Name: "GrossComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Win/Loss", Name: "MemberWinlost", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Comm", Name: "PlayerComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Total", Name: "MemberTotal", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Win/Loss", Name: "AgWinlost", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Comm", Name: "AgComm", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Total", Name: "AgTotal", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Company", Name: "Company", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" },
        { HeaderText: "Reward", Name: "RewardAmount", Width: 130, Sort: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: "$" }
    ],
    defaultHeaderColumns: [
        { HeaderText: "Username", Name: "Username", MergeRowNumber: 2 },
        { HeaderText: "Turnover", Name: "Turnover", MergeRowNumber: 2 },
        { HeaderText: "Net Turnover&nbsp;<span id=\"netTurnover\" class=\"bet-type-detail-icon\"></span>", Name: "NetTurnover", MergeRowNumber: 2 },
        { HeaderText: "GrossComm", Name: "GrossComm", MergeRowNumber: 2 },
        { HeaderText: "Member", Name: "MemberWinlost", MergeColNumber: 3 },
        { HeaderText: "Agent", Name: "AgWinlost", MergeColNumber: 3 },
        { HeaderText: "Master", Name: "MAgWinlost", MergeColNumber: 3 },
        { HeaderText: "Super", Name: "SAgWinlost", MergeColNumber: 3 },
        { HeaderText: "Company", Name: "Company", MergeRowNumber: 2 },
        { HeaderText: "Extra", Name: "ExtraWinLost", MergeColNumber: 3 },
        { HeaderText: "Company (After Extra)", Name: "CompanyAfterExtraPT", MergeRowNumber: 2 },
        { HeaderText: "Reward", Name: "RewardAmount", MergeRowNumber: 2 }
    ],
    SAgHeaderColumns: [
        { HeaderText: "Username", Name: "Username", MergeRowNumber: 2 },
        { HeaderText: "Level", Name: "LevelType", MergeRowNumber: 2 },
        { HeaderText: "Turnover", Name: "Turnover", MergeRowNumber: 2 },
        { HeaderText: "Net Turnover&nbsp;<span id=\"netTurnover\" class=\"bet-type-detail-icon\"></span>", Name: "NetTurnover", MergeRowNumber: 2 },
        { HeaderText: "GrossComm", Name: "GrossComm", MergeRowNumber: 2 },
        { HeaderText: "Member", Name: "MemberWinlost", MergeColNumber: 3 },
        { HeaderText: "Agent", Name: "AgWinlost", MergeColNumber: 3 },
        { HeaderText: "Master", Name: "MAgWinlost", MergeColNumber: 3 },
        { HeaderText: "Super", Name: "SAgWinlost", MergeColNumber: 3 },
        { HeaderText: "Company", Name: "Company", MergeRowNumber: 2 },
        { HeaderText: "Reward", Name: "RewardAmount", MergeRowNumber: 2 }
    ],
    MAgHeaderColumns: [
        { HeaderText: "Username", Name: "Username", MergeRowNumber: 2 },
        { HeaderText: "Level", Name: "LevelType", MergeRowNumber: 2 },
        { HeaderText: "Turnover", Name: "Turnover", MergeRowNumber: 2 },
        { HeaderText: "Net Turnover&nbsp;<span id=\"netTurnover\" class=\"bet-type-detail-icon\"></span>", Name: "NetTurnover", MergeRowNumber: 2 },
        { HeaderText: "GrossComm", Name: "GrossComm", MergeRowNumber: 2 },
        { HeaderText: "Member", Name: "MemberWinlost", MergeColNumber: 3 },
        { HeaderText: "Agent", Name: "AgWinlost", MergeColNumber: 3 },
        { HeaderText: "Master", Name: "MAgWinlost", MergeColNumber: 3 },
        { HeaderText: "Company", Name: "Company", MergeRowNumber: 2 },
        { HeaderText: "Reward", Name: "RewardAmount", MergeRowNumber: 2 }
    ],
    AgHeaderColumns: [
        { HeaderText: "Username", Name: "Username", MergeRowNumber: 2 },
        { HeaderText: "Turnover", Name: "Turnover", MergeRowNumber: 2 },
        { HeaderText: "Net Turnover&nbsp;<span id=\"netTurnover\" class=\"bet-type-detail-icon\"></span>", Name: "NetTurnover", MergeRowNumber: 2 },
        { HeaderText: "GrossComm", Name: "GrossComm", MergeRowNumber: 2 },
        { HeaderText: "Member", Name: "MemberWinlost", MergeColNumber: 3 },
        { HeaderText: "Agent", Name: "AgWinlost", MergeColNumber: 3 },
        { HeaderText: "Company", Name: "Company", MergeRowNumber: 2 },
        { HeaderText: "Reward", Name: "RewardAmount", MergeRowNumber: 2 }
    ],
    Render: function (dateInfo) {
        var arrColData = new Array();
        var arrGroupHeaders = new Array();

        var u = $this.viewlevel === 2 ? "Agent" : "Member";
        var v = $this.viewlevel === 3 ? "Master" : u;
        var level = $this.viewlevel === 4 ? "Super" : v;

        var numberOfFrozenColumn = 3;
        var widthDevice = top.$(top.window).width();
        if (widthDevice <= 768) {
            numberOfFrozenColumn = 1;
        }

        var titleInfo = {
            text: level + " Win Loss Detail " + dateInfo
        };

        if (this.viewlevel === 1) {
            arrColData = this.AgColumns;
            arrGroupHeaders = this.AgHeaderColumns;
        } else if (this.viewlevel === 2) {
            arrColData = this.MAgColumns;
            arrGroupHeaders = this.MAgHeaderColumns;
        } else if (this.viewlevel === 3) {
            arrColData = this.SAgColumns;
            arrGroupHeaders = this.SAgHeaderColumns;
        } else {
            arrColData = this.defaultColumns;
            arrGroupHeaders = this.defaultHeaderColumns;
            $.extend(titleInfo, {
                customButtons: '<span style="display: inline-block; position: relative; top: -3px; margin-right: 3px;">Show Extra PT</span>' +
                    '<div id="showHideExtras" style="display: inline-block" class= "disallowDblComm"></div>'
            });
        }
        arrColData[arrColData.length -1].HeaderText = `Reward (${this.dataSource[0].RewardCurrency})`;
        arrGroupHeaders[arrGroupHeaders.length -1].HeaderText = `Reward (${this.dataSource[0].RewardCurrency})`;
        
        var gridOption = {
            customizeHeader: true,
            groupHeaderRows: [{ Columns: arrGroupHeaders }],
            columns: arrColData,
            numberOfFrozenColumn: numberOfFrozenColumn,
            autoCalculateGridExpandHeight: true,
            fullHeight: !isMobile.phone,
            gridHeight: isMobile.phone ? $(window).height() - 70 : 0,
            showTitle: true,
            titleInfo,
            showFooter: true,
            gridExpandHeight: 256,
            showPager: true,
            pagerOption: {
                itemsPerPageArray: [50, 100, 500],
                currentPage: 0,
                totalItem: $this.totalRows,
                itemsPerPage: 50
            },
            marginRight: 10,
            bottomBorderWidth: 0,
            showDistinctRow: false,
            allowTruncate: true,
            sortOption:
                {
                    customGridSort: $this.CustomGridSort,
                    afterGridSorted: function (rows) {
                        return $this.customsortfunc(rows);
                    }
                },
            numOfPaddingForGridExpandHeight: 40
        };

        // For Summary Footer
        var sumTurnover = 0;
        var sumNetTurnover = 0;
        var sumGrossComm = 0;
        var sumMemberWinlost = 0;
        var sumPlayerComm = 0;
        var sumMemberTotal = 0;
        var sumAgWinlost = 0;
        var sumAgComm = 0;
        var sumAgTotal = 0;
        var sumMAgWinlost = 0;
        var sumMAgComm = 0;
        var sumMAgTotal = 0;
        var sumSAgWinlost = 0;
        var sumSAgComm = 0;
        var sumSAgTotal = 0;
        var sumExtraWinLost = 0;
        var sumExtraComm = 0;
        var sumExtraTotal = 0;
        var sumCompanyAfterExtraPT = 0;
        var sumCompany = 0;
        var sumRewardAmount = 0;

        //// Assing BodyData
        $this.InitSort();
        var bodyData = [];
        for (var i = 0; i < this.totalRows; i++) {
            if (this.viewlevel === 1) {
                bodyData.push({
                    Columns: [
                        { Name: "Username", Value: this.dataSource[i] },
                        { Name: "Turnover", Value: this.dataSource[i].Turnover },
                        { Name: "NetTurnover", Value: this.dataSource[i].NetTurnover },
                        { Name: "GrossComm", Value: this.dataSource[i].GrossComm },
                        { Name: "MemberWinlost", Value: this.dataSource[i].MemberWinlost },
                        { Name: "PlayerComm", Value: this.dataSource[i].PlayerComm },
                        { Name: "MemberTotal", Value: this.dataSource[i].MemberTotal },
                        { Name: "AgWinlost", Value: this.dataSource[i].AgWinlost },
                        { Name: "AgComm", Value: this.dataSource[i].AgComm },
                        { Name: "AgTotal", Value: this.dataSource[i].AgTotal },
                        { Name: "Company", Value: this.dataSource[i].Company },
                        { Name: "RewardAmount", Value: this.dataSource[i].RewardAmount }
                    ]
                });
            } else if (this.viewlevel === 2) {
                bodyData.push({
                    Columns: [
                        { Name: "Username", Value: this.dataSource[i] },
                        { Name: "LevelType", Value: this.dataSource[i].RoleId === 2 ? "Agent" : "Direct Member" },
                        { Name: "Turnover", Value: this.dataSource[i].Turnover },
                        { Name: "NetTurnover", Value: this.dataSource[i].NetTurnover },
                        { Name: "GrossComm", Value: this.dataSource[i].GrossComm },
                        { Name: "MemberWinlost", Value: this.dataSource[i].MemberWinlost },
                        { Name: "PlayerComm", Value: this.dataSource[i].PlayerComm },
                        { Name: "MemberTotal", Value: this.dataSource[i].MemberTotal },
                        { Name: "AgWinlost", Value: this.dataSource[i].AgWinlost },
                        { Name: "AgComm", Value: this.dataSource[i].AgComm },
                        { Name: "AgTotal", Value: this.dataSource[i].AgTotal },
                        { Name: "MAgWinlost", Value: this.dataSource[i].MAgWinlost },
                        { Name: "MAgComm", Value: this.dataSource[i].MAgComm },
                        { Name: "MAgTotal", Value: this.dataSource[i].MAgTotal },
                        { Name: "Company", Value: this.dataSource[i].Company },
                        { Name: "RewardAmount", Value: this.dataSource[i].RewardAmount }
                    ]
                });
            } else if (this.viewlevel === 3) {
                bodyData.push({
                    Columns: [
                        { Name: "Username", Value: this.dataSource[i] },
                        { Name: "LevelType", Value: this.dataSource[i].RoleId === 3 ? "Master Agent" : "Direct Member" },
                        { Name: "Turnover", Value: this.dataSource[i].Turnover },
                        { Name: "NetTurnover", Value: this.dataSource[i].NetTurnover },
                        { Name: "GrossComm", Value: this.dataSource[i].GrossComm },
                        { Name: "MemberWinlost", Value: this.dataSource[i].MemberWinlost },
                        { Name: "PlayerComm", Value: this.dataSource[i].PlayerComm },
                        { Name: "MemberTotal", Value: this.dataSource[i].MemberTotal },
                        { Name: "AgWinlost", Value: this.dataSource[i].AgWinlost },
                        { Name: "AgComm", Value: this.dataSource[i].AgComm },
                        { Name: "AgTotal", Value: this.dataSource[i].AgTotal },
                        { Name: "MAgWinlost", Value: this.dataSource[i].MAgWinlost },
                        { Name: "MAgComm", Value: this.dataSource[i].MAgComm },
                        { Name: "MAgTotal", Value: this.dataSource[i].MAgTotal },
                        { Name: "SAgWinlost", Value: this.dataSource[i].SAgWinlost },
                        { Name: "SAgComm", Value: this.dataSource[i].SAgComm },
                        { Name: "SAgTotal", Value: this.dataSource[i].SAgTotal },
                        { Name: "Company", Value: this.dataSource[i].Company },
                        { Name: "RewardAmount", Value: this.dataSource[i].RewardAmount }
                    ]
                });
            } else {
                bodyData.push({
                    Columns: [
                        { Name: "Username", Value: this.dataSource[i] },
                        { Name: "Turnover", Value: this.dataSource[i].Turnover },
                        { Name: "NetTurnover", Value: this.dataSource[i].NetTurnover },
                        { Name: "GrossComm", Value: this.dataSource[i].GrossComm },
                        { Name: "MemberWinlost", Value: this.dataSource[i].MemberWinlost },
                        { Name: "PlayerComm", Value: this.dataSource[i].PlayerComm },
                        { Name: "MemberTotal", Value: this.dataSource[i].MemberTotal },
                        { Name: "AgWinlost", Value: this.dataSource[i].AgWinlost },
                        { Name: "AgComm", Value: this.dataSource[i].AgComm },
                        { Name: "AgTotal", Value: this.dataSource[i].AgTotal },
                        { Name: "MAgWinlost", Value: this.dataSource[i].MAgWinlost },
                        { Name: "MAgComm", Value: this.dataSource[i].MAgComm },
                        { Name: "MAgTotal", Value: this.dataSource[i].MAgTotal },
                        { Name: "SAgWinlost", Value: this.dataSource[i].SAgWinlost },
                        { Name: "SAgComm", Value: this.dataSource[i].SAgComm },
                        { Name: "SAgTotal", Value: this.dataSource[i].SAgTotal },
                        { Name: "Company", Value: this.dataSource[i].Company },
                        { Name: 'ExtraWinLost', Value: this.dataSource[i].ExtraWinLost },
                        { Name: 'ExtraComm', Value: this.dataSource[i].ExtraComm },
                        { Name: 'ExtraTotal', Value: this.dataSource[i].ExtraTotal },
                        { Name: "CompanyAfterExtraPT", Value: this.dataSource[i].CompanyAfterExtraPT },
                        { Name: "RewardAmount", Value: this.dataSource[i].RewardAmount }
                    ]
                });
            }

            sumTurnover += this.dataSource[i].Turnover;
            sumNetTurnover += this.dataSource[i].NetTurnover;
            sumGrossComm += this.dataSource[i].GrossComm;
            sumMemberWinlost += this.dataSource[i].MemberWinlost;
            sumPlayerComm += this.dataSource[i].PlayerComm;
            sumMemberTotal += this.dataSource[i].MemberTotal;
            sumAgWinlost += this.dataSource[i].AgWinlost;
            sumAgComm += this.dataSource[i].AgComm;
            sumAgTotal += this.dataSource[i].AgTotal;
            if (this.viewlevel > 1) {
                sumMAgWinlost += this.dataSource[i].MAgWinlost;
                sumMAgComm += this.dataSource[i].MAgComm;
                sumMAgTotal += this.dataSource[i].MAgTotal;
            }
            if (this.viewlevel > 2) {
                sumSAgWinlost += this.dataSource[i].SAgWinlost;
                sumSAgComm += this.dataSource[i].SAgComm;
                sumSAgTotal += this.dataSource[i].SAgTotal;
            }
            if (this.viewlevel > 3) {
                sumExtraWinLost += this.dataSource[i].ExtraWinLost;
                sumExtraComm += this.dataSource[i].ExtraComm;
                sumExtraTotal += this.dataSource[i].ExtraTotal;
                sumCompanyAfterExtraPT += this.dataSource[i].CompanyAfterExtraPT;
            }
            sumCompany += this.dataSource[i].Company;
            sumRewardAmount += this.dataSource[i].RewardAmount;
        }

        gridOption.bodyRows = bodyData;
        gridOption.pagerOption.totalItem = this.totalRows;

        //// Asign Footer Data
        var footerData = [];
        if (this.viewlevel === 1) {
            footerData.push({
                Columns: [
                    { Name: "Username", Value: "Grand Total" },
                    { Name: "Turnover", Value: sumTurnover },
                    { Name: "NetTurnover", Value: sumNetTurnover },
                    { Name: "GrossComm", Value: sumGrossComm },
                    { Name: "MemberWinlost", Value: sumMemberWinlost },
                    { Name: "PlayerComm", Value: sumPlayerComm },
                    { Name: "MemberTotal", Value: sumMemberTotal },
                    { Name: "AgWinlost", Value: sumAgWinlost },
                    { Name: "AgComm", Value: sumAgComm },
                    { Name: "AgTotal", Value: sumAgTotal },
                    { Name: "Company", Value: sumCompany },
                    { Name: "RewardAmount", Value: sumRewardAmount }
                ]
            });
        } else if (this.viewlevel === 2) {
            footerData.push({
                Columns: [
                    { Name: "Username", Value: "Grand Total" , MergeColNumber: 2, CellAlign: "right"},
                    { Name: "Turnover", Value: sumTurnover },
                    { Name: "NetTurnover", Value: sumNetTurnover },
                    { Name: "GrossComm", Value: sumGrossComm },
                    { Name: "MemberWinlost", Value: sumMemberWinlost },
                    { Name: "PlayerComm", Value: sumPlayerComm },
                    { Name: "MemberTotal", Value: sumMemberTotal },
                    { Name: "AgWinlost", Value: sumAgWinlost },
                    { Name: "AgComm", Value: sumAgComm },
                    { Name: "AgTotal", Value: sumAgTotal },
                    { Name: "MAgWinlost", Value: sumMAgWinlost },
                    { Name: "MAgComm", Value: sumMAgComm },
                    { Name: "MAgTotal", Value: sumMAgTotal },
                    { Name: "Company", Value: sumCompany },
                    { Name: "RewardAmount", Value: sumRewardAmount }
                ]
            });
        } else if (this.viewlevel === 3) {
            footerData.push({
                Columns: [
                    { Name: "Username", Value: "Grand Total" , MergeColNumber: 2, CellAlign: "right"},
                    { Name: "Turnover", Value: sumTurnover },
                    { Name: "NetTurnover", Value: sumNetTurnover },
                    { Name: "GrossComm", Value: sumGrossComm },
                    { Name: "MemberWinlost", Value: sumMemberWinlost },
                    { Name: "PlayerComm", Value: sumPlayerComm },
                    { Name: "MemberTotal", Value: sumMemberTotal },
                    { Name: "AgWinlost", Value: sumAgWinlost },
                    { Name: "AgComm", Value: sumAgComm },
                    { Name: "AgTotal", Value: sumAgTotal },
                    { Name: "MAgWinlost", Value: sumMAgWinlost },
                    { Name: "MAgComm", Value: sumMAgComm },
                    { Name: "MAgTotal", Value: sumMAgTotal },
                    { Name: "SAgWinlost", Value: sumSAgWinlost },
                    { Name: "SAgComm", Value: sumSAgComm },
                    { Name: "SAgTotal", Value: sumSAgTotal },
                    { Name: "Company", Value: sumCompany },
                    { Name: "RewardAmount", Value: sumRewardAmount }
                ]
            });
        } else {
            footerData.push({
                Columns: [
                    { Name: "Username", Value: "Grand Total" },
                    { Name: "Turnover", Value: sumTurnover },
                    { Name: "NetTurnover", Value: sumNetTurnover },
                    { Name: "GrossComm", Value: sumGrossComm },
                    { Name: "MemberWinlost", Value: sumMemberWinlost },
                    { Name: "PlayerComm", Value: sumPlayerComm },
                    { Name: "MemberTotal", Value: sumMemberTotal },
                    { Name: "AgWinlost", Value: sumAgWinlost },
                    { Name: "AgComm", Value: sumAgComm },
                    { Name: "AgTotal", Value: sumAgTotal },
                    { Name: "MAgWinlost", Value: sumMAgWinlost },
                    { Name: "MAgComm", Value: sumMAgComm },
                    { Name: "MAgTotal", Value: sumMAgTotal },
                    { Name: "SAgWinlost", Value: sumSAgWinlost },
                    { Name: "SAgComm", Value: sumSAgComm },
                    { Name: "SAgTotal", Value: sumSAgTotal },
                    { Name: "Company", Value: sumCompany },
                    { Name: 'ExtraWinLost', Value: sumExtraWinLost },
                    { Name: 'ExtraComm', Value: sumExtraComm },
                    { Name: 'ExtraTotal', Value: sumExtraTotal },
                    { Name: 'CompanyAfterExtraPT', Value: sumCompanyAfterExtraPT },
                    { Name: "RewardAmount", Value: sumRewardAmount }
                ]
            });
        }

        gridOption.footerRows = footerData;

        setPagerOptionOnMobile(gridOption);

        $("#jqgrid").fanexGrid(gridOption);

        if (this.viewlevel >= 4) {
            $('#jqgrid').fanexGrid("hideColumns", gridOption, [17, 18, 19, 20]);

            $('#showHideExtras').click(function () {
                if ($('#showHideExtras').attr('class') === 'allowDblComm') {
                    $('#jqgrid').fanexGrid("hideColumns", gridOption, [17, 18, 19, 20]);
                    $('#showHideExtras').attr('class', 'disallowDblComm');
                } else {
                    $('#jqgrid').fanexGrid("hideColumns", gridOption, []);
                    $('#showHideExtras').attr('class', 'allowDblComm');
                }
            });
        }

        workaroundForGridOnPhone();
    },
    customsortfunc: function (rows) {
        var countFixedSort = $this.fixedsortlist.length;
        if (countFixedSort > 0) {
            for (var i = 0; i < countFixedSort; i++) {
                for (var j = 0; j < rows.length; j++) {
                    if ($this.fixedsortlist[i] == rows[j].Columns[0].Value.CustId) {
                        $this.fixedsortdata.push(rows.slice(j, j + 1)[0]);
                        rows.unshift(rows.splice(j, 1)[0]);
                        break;
                    }
                }
            }
        }

        return rows;
    },
    InitSort: function () {
        var countFixedSort = $this.fixedsortlist.length;
        if (countFixedSort > 0) {
            for (var i = 0; i < countFixedSort; i++) {
                for (var j = 0; j < $this.dataSource.length; j++) {
                    if ($this.fixedsortlist[i] == $this.dataSource[j].CustId) {
                        $this.fixedsortdata.push($this.dataSource.slice(j, j + 1)[0]);
                        $this.dataSource.unshift($this.dataSource.splice(j, 1)[0]);
                        break;
                    }
                }
            }
        }
    },
    ChangeHeadlineTab: function () {
        if (this.viewlevel >= 4) {
            this.srecommend = this.custId;
            this.ToggleTabs('none', 'none', 'none', 'none', 4, 'none');
        } else if (this.viewlevel == 3) {
            this.mrecommend = this.custId;
            this.ToggleTabs('block', 'block', 'none', 'none', 3, 'none');
            $("#tabSuper").attr("class", "tabActiveSuper");
        } else if (this.viewlevel == 2) {
            this.recommend = this.custId;
            this.ToggleTabs('block', 'block', 'block', 'none', 2, 'none');
            $("#tabSuper").attr("class", "tabDisableSuper");
            $("#tabMaster").attr("class", "tabActive");
        } else if (this.viewlevel == 1) {
            this.ToggleTabs('block', 'block', 'block', 'block', 1, 'none');
            $("#tabSuper").attr("class", "tabDisableSuper");
            $("#tabMaster").attr("class", "tabDisable");
            $("#tabAgent").attr("class", "tabActive");
        } else {
            this.ToggleTabs('block', 'block', 'block', 'block', 0, 'block');
            $("#tabSuper").attr("class", "tabDisableSuper");
            $("#tabMaster").attr("class", "tabDisable");
            $("#tabAgent").attr("class", "tabDisable");
            $("#tabMember").attr("class", "tabActive");
        }
        $("#headlineDaily").css("visibility", "visible");
    },
    GetDownlineWinLossDetail: function (custId, custName) {
        this.custId = custId;
        this.username = "";
        this.drillDownName = custName;
        this.Databind(this.username);
    },
    InitShowHideControls: function (status) {
        $("#headlineDaily").css("display", status);
        $("#mainGrid").css("display", status);
    },
    ResetParameters: function () {
        this.srecommend = 0;
        this.mrecommend = 0;
        this.recommend = 0;
        this.custId = 0;
        this.username = '';
        this.viewlevel = 4;
    },
    GetFromDateToDate: function () {
        if ($(".from-to-date").css('display') != 'none') {
            this.fromdate = $("#dtpFromDate").val();
            this.todate = $("#dtpToDate").val();
        }
        else {
            this.fromdate = $("#monPickr1").monPickr("getFromDate", "MM/dd/yyyy");
            this.todate = $("#monPickr1").monPickr("getToDate", "MM/dd/yyyy");
        }
    },
    Databind: function (username) {
        this.username = !username ? "" : username;
        this.GetFromDateToDate();
        this.groupName = $("#selectedGroupName").html();

        var dateInfo;
        if (this.fromdate == this.todate) {
            dateInfo = 'on ' + this.fromdate;
        }
        else {
            dateInfo = 'from ' + (new Date(Date.parse(this.fromdate))).toStringFormat('MMM dd, yyyy') + ' to ' + (new Date(Date.parse(this.todate))).toStringFormat('MMM dd, yyyy');
        }

        var parameterUrl = "?groupid=" + this.groupId
            + "&groupname=" + this.groupName
            + "&srecommend=" + this.srecommend
            + "&mrecommend=" + this.mrecommend
            + "&recommend=" + this.recommend
            + "&custid=" + this.custId
            + "&username=" + Fanex.ExtAcc.ReplaceAll(this.viewlevel == 5 ? "" : this.username, "&", "")
            + "&fromdate=" + this.fromdate
            + "&todate=" + this.todate
            + "&viewlevel=" + this.viewlevel
            + this.getParameterProducts()
            + this.getParameterProductDetails();

        Fanex.ExtAcc.BlockUI();

        $.ajax({
            context: $this,
            url: this.url + parameterUrl,
            type: "post",
            dataType: "json",
            cache: false,
            beforesend: function (xhr) { },
            success: function (data) {
                if (data.errorCode) {
                    Fanex.ExtAcc.UnBlockUI();
                    Fanex.ExtAcc.Form.Alert("Error", data.errorMessage.formatErrorMessage(), 350, 150, "alertWLD");
                    return;
                }

                $this.fixedsortdata = new Array();
                $this.removedsortdata = new Array();

                $this.dataSource = data.rows;
                $this.totalRows = data.rows.length;
                $this.fixedsortlist = data.sortingorderlist;

                $this.viewlevel = data.viewlevel;
                if ($this.viewlevel == 3) {
                    $this.srecommend = data.srecommend;
                } else if ($this.viewlevel == 2) {
                    $this.srecommend = data.srecommend;
                    $this.mrecommend = data.mrecommend;
                } else if ($this.viewlevel == 1) {
                    $this.srecommend = data.srecommend;
                    $this.mrecommend = data.mrecommend;
                    $this.recommend = data.recommend;
                }

                if ($this.custId == 0) {
                    $this.supername = data.supername;
                    $this.mastername = data.mastername;
                    $this.agentname = data.agentname;
                }

                $this.ChangeHeadlineTab();
    
                $("#mainGrid").css("display", $this.totalRows < 1 ? "none" : "block");
                $("#mainGrid").css("visibility", $this.totalRows < 1 ? "hidden" : "visible");
                $("#headlineDaily").css("display", $this.totalRows < 1 ? "none" : "block");
                $("#headlineDaily").css("visibility", $this.totalRows < 1 ? "hidden" : "visible");
                $("#exporttoexcel").css("display", $this.totalRows < 1 ? "none" : "block");
                $("#msgNoinfo").css("display", $this.totalRows < 1 ? "block" : "none");

                if ($this.totalRows > 0) {
                    $this.Render(dateInfo);
                    $("#exporttoexcel").removeClass("hide");
                    $(".groupBack").removeClass("hide");
                } else {
                    $("#exporttoexcel").addClass("hide");
                    $(".groupBack").addClass("hide");
                }

                $("#netTurnover").bind("click",
                    function () {
                        var message =
                            "- For Sportsbook, Net Turnover is Turnover which excludes the draw tickets and halves of the turnover of the half won, half lost tickets.";
                        var height = 170;

                        if (data.tooltips.liveCasino != "") {
                            message +=
                                "<br/>" +
                                "<br/>" +
                                data.tooltips.liveCasino;
                            height += 50;
                        }

                        if (data.tooltips.rngCasino != "") {
                            message +=
                                "<br/>" +
                                "<br/>" +
                                data.tooltips.rngCasino;
                            height += 70;
                        }
                        Fanex.ExtAcc.Form.Alert("Net Turnover Information", message, 430, height);
                        return false;
                    });

                Fanex.ExtAcc.UnBlockUI();
            }
        });
    },
    GetExtraPT: function () {
        Fanex.ExtAcc.BlockUI();

        $this.groupName = $("#selectedGroupName").html();

        var url = resolveClientUrl('WinLossDetail/GetExtraPT')
            + "?groupid=" + Fanex.ExtAcc.WinLossDetail.groupId
            + "&groupname=" + $this.groupName;
        $.ajax({
            context: $this,
            url,
            type: 'post',
            dataType: 'json',
            cache: false
        })
            .done(function (data) {
                Fanex.ExtAcc.WinLossDetail.ShowExtraPTDetail(data);
            })
            .fail(function (error) {
                console.log(error);
            })
            .always(function () {
                Fanex.ExtAcc.UnBlockUI();
            });
    },
    ShowExtraPTDetail: function (data) {
        var extraPT = '',
            height = 220;

        for (var i = 0; i < data.length; i++) {
            extraPT += '<li>' + data[i].ExtraPT + '% of ' + data[i].ProductNames + '</li>';
            height += 50;
        }

        var extraPTMessage = 'Extra PT% Details <ul>' + extraPT + '</ul>';

        Fanex.ExtAcc.Form.Alert("View Extra PT%", extraPTMessage, 500, height);
    },
    CustomGridSort: function (rows, sortField, sortDirection, customSortFunction, sortType) {
        if (!rows.length || !sortField) {
            return rows;
        }

        var sortColIndex = $this.GetSortedColumnIndex(rows, sortField);
        if (sortColIndex === -1) {
            return rows;
        }

        rows.sort(function (row1, row2) {
            if (row1.Columns[0].Value.Username === null || row1.Columns[0].Value.Username === "CASH SERVICE BILLING") { // 'Dormant Accounts, CASH SERVICE BILLING'
                return 1;
            }

            var num1, num2;
            if (sortColIndex === 0) { // Username
                num1 = row1.Columns[sortColIndex].Value.Username;
                num2 = row2.Columns[sortColIndex].Value.Username;
            } else {
                num1 = row1.Columns[sortColIndex].Value;
                num2 = row2.Columns[sortColIndex].Value;
            }
            
            if (sortDirection === 'asc') {
                return num1 > num2 ? 1 : -1;
            }

            return num1 < num2 ? 1 : -1;
        });

        return rows;
    },
    GetSortedColumnIndex: function (rows, sortField) {
        var firstRowColumns = rows[0].Columns;

        for (var i = 0, max = firstRowColumns.length; i < max; i++) {
            if (firstRowColumns[i].Name === sortField) {
                return i;
            }
        }

        return -1;
    },

    GetCashFee: function (custId, custName) {
        Fanex.ExtAcc.BlockUI();
        $this.viewlevel = 0;
        var url = resolveClientUrl('WinLossDetail/GetCashFee')
            + "?custId=" + custId
            + "&custName=" + custName
            + "&fromDate=" + $this.fromdate
            + "&toDate=" + $this.todate;
        $.ajax({
            context: $this,
            url,
            type: 'post',
            dataType: 'json',
            cache: false
        })
        .done(function (data) {
            $this.RenderCashFee(data);
            $this.ChangeHeadlineTab();
        })
        .fail(function (error) {
            console.log(error);
        })
        .always(function () {
            Fanex.ExtAcc.UnBlockUI();
        });
    },

    RenderCashFee: function (data) {
        var headerColumns = [
            { HeaderText: "Type", Width: 140, Name: "ColType", HeaderAlign: "center", CellAlign: "left" },
            { HeaderText: "Fee", Width: 100, Name: "ColFee", HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00" },
            { HeaderText: "Count", Width: 100, Name: "ColCount", HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0" },
            { HeaderText: "Agent", Width: 100, Name: "ColAgent", HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00" },
            { HeaderText: "Company", Width: 100, Name: "ColCompany", HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00" }
        ];

        var bodyData = [];
        var footerData = [];

        if (data && data.FeeGroups.length > 0) {
            var reportLength = data.FeeGroups.length;
            var dataItemColumns = null;

            for (var i = 0; i < reportLength; i++) {
                var feeGroup = data.FeeGroups[i];

                var dataItemColumns = [
                    { Name: "ColType", Value: feeGroup.Total.DisplayType },
                    { Name: "ColFee", Value: "" },
                    { Name: "ColCount", Value: feeGroup.Total.Count },
                    { Name: "ColAgent", Value: feeGroup.Total.Agent },
                    { Name: "ColCompany", Value: feeGroup.Total.Company }
                ];

                bodyData.push({
                    CssClass: "subTotal",
                    Columns: dataItemColumns
                });

                if (feeGroup && feeGroup.FeeItems.length > 0) {
                    var groupLength = feeGroup.FeeItems.length;
                    for (var j = 0; j < groupLength; j++) {
                        var dataItem = feeGroup.FeeItems[j];
                        dataItemColumns = [
                            { Name: "ColType", Value: dataItem.DisplayType },
                            { Name: "ColFee", Value: dataItem.Fee },
                            { Name: "ColCount", Value: dataItem.Count },
                            { Name: "ColAgent", Value: dataItem.Agent },
                            { Name: "ColCompany", Value: dataItem.Company }
                        ];

                        bodyData.push({
                            CssClass: "dataSubTotal",
                            Columns: dataItemColumns
                        });
                    }
                }
            }

            var grandTotal = data.GrandTotal; 
            footerData.push({
                Columns: [{ Name: "ColType", Value: grandTotal.DisplayType, MergeColNumber: 3, CssClass: "text-right" },
                    { Name: "ColAgent", Value: grandTotal.Agent },
                    { Name: "ColCompany", Value: grandTotal.Company }]
            });
        }

        var dateInfo = "";
        if (this.fromdate == this.todate) {
            dateInfo = 'on ' + this.fromdate;
        }
        else {
            dateInfo = 'from ' + (new Date(Date.parse(this.fromdate))).toStringFormat('MMM dd, yyyy') + ' to ' + (new Date(Date.parse(this.todate))).toStringFormat('MMM dd, yyyy');
        }

        var gridOptions = {
            columns: headerColumns,
            showTitle: true,
            showFooter: true,
            titleInfo:
            {
                text: "CASH SERVICE BILLING " + dateInfo + " - " + $this.drillDownName,
                showExcelButton: false
            },
            showTree: false,
            bodyRows: bodyData,
            footerRows: footerData,
            gridExpandHeight: 230,
            showDistinctRow: false,
            showHighlightRow: false,
            noDataText: "There is no information",
            marginRight: 20
        };

        $("#jqgrid").fanexGrid(gridOptions);
    }
};

$(document).ready(function () {
    Fanex.ExtAcc.WinLossDetail.Init();
});