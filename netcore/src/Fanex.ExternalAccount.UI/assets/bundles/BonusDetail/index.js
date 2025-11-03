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
var CancelBonus = /** @class */ (function () {
    function CancelBonus($, callback) {
        this.resources = {
            popupTitle: "Cancel Bonus",
            confirmMessage: '<div id="div-msgCancelPending" class="hide"></div>Are you sure you want to cancel this Bonus (Ref. No.: {0})?'
        };
        this.$ = $;
        this.callback = callback;
    }
    CancelBonus.prototype.RegisterCancelPending = function (elements) {
        var self = this;
        elements
            .unbind("click")
            .bind("click", function () {
            var $this = self.$(this);
            var custId = $this.data("custid");
            var transId = $this.data("transid");
            Fanex.ExtAcc.Form.ConfirmBox(self.resources.popupTitle, self.resources.confirmMessage.replace("{0}", transId), 450, "auto", false, function () { return self.cancelPendingPost(custId, transId); }, null, "cancel-bonus-modal");
            self.buttonSubmit = self.$("#cancel-bonus-modal").next(".ui-dialog-buttonpane").find(".btn.-submit");
        });
    };
    CancelBonus.prototype.cancelPendingPost = function (custId, transId) {
        var _this = this;
        Fanex.ExtAcc.BlockUI();
        this.buttonSubmit.prop("disabled", true);
        this.$.post(resolveClientUrl("BonusDetail/CancelPending"), { custId: custId, transId: transId, groupName: this.$("#selectedGroupName").html() }, function (data) {
            var isSuccess = data.isSuccess || data.IsSuccess;
            if (!isSuccess) {
                _this.showError(data.errorMessage || data.ErrorMessage);
                _this.buttonSubmit.prop("disabled", false);
                _this.callback();
            }
            else {
                _this.showSuccess(data.ErrorMessage);
                setTimeout(function () {
                    _this.callback();
                    Fanex.ExtAcc.Form.Close(null, null, "cancel-bonus-modal");
                }, 1000 * 3);
            }
        })
            .always(Fanex.ExtAcc.UnBlockUI);
    };
    CancelBonus.prototype.showError = function (error) {
        this.$("#div-msgCancelPending").html(error).removeClass().addClass("errMsg");
    };
    CancelBonus.prototype.showSuccess = function (message) {
        this.$("#div-msgCancelPending").html(message).removeClass().addClass("successMsg");
    };
    CancelBonus.prototype.hideMessage = function () {
        this.$("#div-msgCancelPending").removeClass().addClass("hide").html("");
    };
    return CancelBonus;
}());
//# sourceMappingURL=Fanex.ExtAcc.CancelBonus.js.map
(function ($) {
    $.fn.inputFilter = function (inputFilter) {
        return this.on("input keydown keyup mousedown mouseup select contextmenu drop", function () {
            if (inputFilter(this.value)) {
                this.oldValue = this.value;
                this.oldSelectionStart = this.selectionStart;
                this.oldSelectionEnd = this.selectionEnd;
            }
            else if (this.hasOwnProperty("oldValue")) {
                this.value = this.oldValue;
                this.setSelectionRange(this.oldSelectionStart, this.oldSelectionEnd);
            }
            else {
                this.value = "";
            }
        });
    };
}(jQuery));
var AddNewBonus = /** @class */ (function () {
    function AddNewBonus($, callback) {
        this.resources = {
            popupTitle: "Add New Bonus",
            usernamesRequiredError: "Member Username(s) is required.",
            usernamesNotAllowBackslashError: "You are not allowed to input backslash '\\' into the Username field.",
            usernamesTooManyError: "The function can only add new up to {0} usernames per request.",
            usernamesDuplicateError: "Duplicated usernames: {0}.",
            amountRequiredError: "Amount is required.",
            amountInvalidError: "Amount must be a positive number."
        };
        this.maxUsernamesLength = 100;
        this.$ = $;
        this.callback = callback;
    }
    AddNewBonus.prototype.RegisterAddNewBonus = function (button, popupAddNewBonus) {
        var _this = this;
        button.click(function () {
            Fanex.ExtAcc.Form.ConfirmBox(_this.resources.popupTitle, popupAddNewBonus.html(), 500, "auto", false, function () { return _this.addNewBonusPost(); }, null, "addnew-bonus-modal", true, null, true);
            _this.cacheElements();
            _this.bindEvents();
        });
    };
    AddNewBonus.prototype.cacheElements = function () {
        this.msgAddNewBonus = this.$("#div-msgAddNewBonus");
        this.formAddNewBonus = this.$("#form-addNewBonus");
        this.txtMembers = this.formAddNewBonus.find("#txt-MemberUsernames");
        this.txtAmount = this.formAddNewBonus.find("#txt-amount");
        this.groupName = this.$("#selectedGroupName").html();
        this.buttonSubmit = this.$("#addnew-bonus-modal").next(".ui-dialog-buttonpane").find(".btn.-submit");
    };
    AddNewBonus.prototype.bindEvents = function () {
        var _this = this;
        this.formAddNewBonus.unbind("submit").bind("submit", function (e) { e.preventDefault(); });
        this.txtMembers
            .unbind("change")
            .bind("change", function () {
            var membersValue = _this.$.trim(_this.txtMembers.val());
            if (membersValue.length > 0) {
                if (membersValue.indexOf("\n") >= 0) {
                    membersValue = membersValue.replace(/\n/g, " ");
                }
                if (membersValue.indexOf("\r") >= 0) {
                    membersValue = membersValue.replace(/\r/g, " ");
                }
                if (membersValue.indexOf("\t") >= 0) {
                    membersValue = membersValue.replace(/\t/g, " ");
                }
                membersValue = membersValue.replace(/\s/g, ",");
                var usernamesArray = membersValue.split(",").filter(function (item) { return item.length > 0; });
                _this.txtMembers.val(usernamesArray.join(","));
            }
        });
        this.txtAmount.inputFilter(function (value) {
            return /^-?\d*[.]?\d{0,2}$/.test(value);
        });
    };
    AddNewBonus.prototype.addNewBonusPost = function () {
        var _this = this;
        if (this.isValidMembers() && this.isValidAmount()) {
            Fanex.ExtAcc.BlockUI();
            this.buttonSubmit.prop("disabled", true);
            var formData = this.formAddNewBonus.serializeArray();
            formData.push({ name: "groupName", value: this.$("#selectedGroupName").html() });
            this.$.post(resolveClientUrl("BonusDetail/AddNew"), formData, function (data) {
                var isSuccess = data.isSuccess || data.IsSuccess;
                if (!isSuccess) {
                    _this.showError(data.errorMessage || data.ErrorMessage);
                    _this.buttonSubmit.prop("disabled", false);
                }
                else {
                    _this.showSuccess(data.ErrorMessage);
                    setTimeout(function () {
                        _this.callback();
                        Fanex.ExtAcc.Form.Close(null, null, "addnew-bonus-modal");
                    }, 1000 * 3);
                }
            }).always(Fanex.ExtAcc.UnBlockUI);
        }
    };
    AddNewBonus.prototype.isValidMembers = function () {
        var membersValue = this.$.trim(this.txtMembers.val());
        if (membersValue.length === 0) {
            this.txtMembers.focus();
            this.showError(this.resources.usernamesRequiredError);
            return false;
        }
        // Check usernames contains '\'.
        if (membersValue.indexOf("\\") >= 0) {
            this.txtMembers.focus();
            this.showError(this.resources.usernamesNotAllowBackslashError);
            return false;
        }
        var membersArray = membersValue.split(",");
        if (membersArray.length > this.maxUsernamesLength) {
            this.txtMembers.focus();
            this.showError(this.resources.usernamesTooManyError.replace("{0}", this.maxUsernamesLength.toString()));
            return false;
        }
        // Check duplicate.
        var duplicatedMembers = membersArray.filter(function (item, index) { return membersArray.indexOf(item) !== index; });
        if (duplicatedMembers.length > 0) {
            this.txtMembers.focus();
            this.showError(this.resources.usernamesDuplicateError.replace("{0}", duplicatedMembers.join(", ")));
            return false;
        }
        this.hideMessage();
        return true;
    };
    AddNewBonus.prototype.isValidAmount = function () {
        var amountValue = this.$.trim(this.txtAmount.val());
        if (amountValue.length === 0) {
            this.txtAmount.focus();
            this.showError(this.resources.amountRequiredError);
            return false;
        }
        amountValue = amountValue.replace(/\,/g, "");
        if (!this.isNumeric(amountValue)) {
            this.txtAmount.focus();
            this.showError(this.resources.amountInvalidError);
            return false;
        }
        var number = parseFloat(amountValue);
        if (number <= 0) {
            this.txtAmount.focus();
            this.showError(this.resources.amountInvalidError);
            return false;
        }
        this.hideMessage();
        return true;
    };
    AddNewBonus.prototype.isNumeric = function (text) {
        return text.match(/^-?(\d+|\d+\.\d*|\d*\.\d+)$/);
    };
    AddNewBonus.prototype.showError = function (error) {
        this.msgAddNewBonus.html(error).removeClass().addClass("errMsg");
    };
    AddNewBonus.prototype.showSuccess = function (message) {
        this.msgAddNewBonus.html(message).removeClass().addClass("successMsg");
    };
    AddNewBonus.prototype.hideMessage = function () {
        this.msgAddNewBonus.removeClass().addClass("hide").html("");
    };
    return AddNewBonus;
}());
//# sourceMappingURL=Fanex.ExtAcc.AddNewBonus.js.map
var BonusDetail = /** @class */ (function () {
    function BonusDetail($) {
        this.cachedPageIndex = 0;
        this.cachedPageSize = 500;
        this.$ = $;
        this.dateTimeControls = new DateTimeControls($);
    }
    BonusDetail.prototype.Init = function () {
        var _this = this;
        this.cacheElements();
        this.bindEvents();
        this.dateTimeControls.Init();
        this.$("#userGroup input")
            .attr("readonly", "true")
            .bind("click", function (event) { return _this.$(event.currentTarget).autocomplete("search", ""); });
        this.initCombobox("#ddl-bonusStatus");
        this.initCombobox("#ddl-memberFeedback");
    };
    BonusDetail.prototype.cacheElements = function () {
        var _this = this;
        this.iconFilter = this.$("#span-iconFilter");
        this.formBonusDetail = this.$("#form-bonusDetail");
        this.gridContainer = this.$("#freegrid-container");
        this.grid = this.$("#jqgrid");
        this.msgNoInfo = this.$("#msgNoinfo");
        this.getBonusDetailURL = this.formBonusDetail.attr("action") + "?";
        var popupContainer = this.$("#div-popupAddNewBonus");
        this.hasHostPermission = popupContainer.length > 0;
        this.groupName = this.$("#selectedGroupName").html();
        if (this.hasHostPermission) {
            this.popupAddNewBonus = popupContainer.clone();
            popupContainer.remove();
            this.cancelBonus = new CancelBonus(this.$, function () { return _this.formBonusDetail.submit(); });
            this.addNewBonus = new AddNewBonus(this.$, function () { return _this.formBonusDetail.submit(); });
        }
    };
    BonusDetail.prototype.bindEvents = function () {
        var _this = this;
        this.iconFilter.click(function () { return _this.showHideFilter(); });
        this.formBonusDetail.submit(function (event) {
            event.preventDefault();
            _this.cachedPageIndex = 0;
            _this.searchBonusDetailQueryString = _this.formBonusDetail.serialize();
            _this.getBonusDetail();
        });
        this.hasHostPermission && this.addNewBonus.RegisterAddNewBonus(this.$("#btn-addNewBonus"), this.popupAddNewBonus);
        Fanex.ExtAcc.OnResize = function () { return _this.resizeReportGrid; };
    };
    BonusDetail.prototype.initCombobox = function (query) {
        var _this = this;
        var $dropdownList = this.$(query).combobox();
        $dropdownList.next("input.ui-autocomplete-input")
            .val($dropdownList.find("option:selected").text())
            .attr("readonly", "true")
            .css("width", "120px")
            .bind("click", function (event) { return _this.$(event.currentTarget).autocomplete("search", ""); });
    };
    BonusDetail.prototype.showHideFilter = function () {
        if (this.iconFilter.hasClass("active")) {
            this.iconFilter.removeClass("active");
            this.formBonusDetail.addClass("hide");
        }
        else {
            this.iconFilter.addClass("active");
            this.formBonusDetail.removeClass("hide");
        }
        this.resizeReportGrid();
    };
    BonusDetail.prototype.resizeReportGrid = function () {
        try {
            this.gridOptions && this.grid.fanexGrid("resizeGrid");
        }
        catch (err) {
            console.log(err);
        }
    };
    BonusDetail.prototype.getBonusDetail = function () {
        var _this = this;
        Fanex.ExtAcc.BlockUI();
        $.getJSON(this.buildGetBonusDetailURL(), function (data) {
            if (data.TotalItems) {
                _this.renderBonusDetail(data);
            }
            else {
                _this.renderNoData();
            }
        }).done(Fanex.ExtAcc.UnBlockUI);
    };
    BonusDetail.prototype.buildGetBonusDetailURL = function () {
        return [
            this.getBonusDetailURL,
            this.searchBonusDetailQueryString,
            "&groupName=",
            this.groupName,
            "&pageIndex=",
            this.cachedPageIndex,
            "&pageSize=",
            this.cachedPageSize
        ].join("");
    };
    BonusDetail.prototype.renderBonusDetail = function (data) {
        var bodyRows = [];
        for (var i = 0, max = data.Bonus.length; i < max; i++) {
            var superBonus = data.Bonus[i];
            var rowMerge = superBonus.Bonus.length;
            for (var j = 0; j < rowMerge; j++) {
                var memberBonus = superBonus.Bonus[j];
                var columns = [
                    { Name: "colCreatedDate", Value: memberBonus.CreatedDate, },
                    { Name: "colRefNo", Value: memberBonus.TransId }
                ];
                if (j === 0) {
                    columns.push({ Name: "colSuper", Value: superBonus.SuperName, MergeRowNumber: rowMerge });
                    columns.push({ Name: "colSuperAmount", Value: superBonus.TotalAmount, MergeRowNumber: rowMerge });
                }
                columns.push({ Name: "colMember", Value: memberBonus.Username });
                columns.push({ Name: "colMemberAmount", Value: memberBonus.Amount, CssClass: memberBonus.IsCancelled ? "red" : "" });
                columns.push({ Name: "colPaymentDate", Value: memberBonus.PaymentDate, });
                columns.push({ Name: "colBonusStatus", Value: this.buildBonusStatusCell(memberBonus), CssClass: memberBonus.IsCancelled ? "red" : "" });
                columns.push({ Name: "colMemberFeedback", Value: memberBonus.MemberFeedback, CssClass: memberBonus.IsNotReceived ? "red" : "" });
                bodyRows.push({ Columns: columns });
            }
        }
        this.gridOptions = this.gridOptions || this.initGridOptions();
        this.gridOptions.bodyRows = bodyRows;
        this.gridOptions.footerRows = [{
                Columns: [
                    { Name: "colCreatedDate", Value: "Grand Total", CssClass: "text-left" },
                    { Name: "colRefNo", Value: "" },
                    { Name: "colSuper", Value: "" },
                    { Name: "colSuperAmount", Value: data.TotalAmount },
                    { Name: "colMember", Value: "" },
                    { Name: "colMemberAmount", Value: data.TotalAmount },
                    { Name: "colPaymentDate", Value: "" },
                    { Name: "colBonusStatus", Value: "" },
                    { Name: "colMemberFeedback", Value: "" }
                ]
            }];
        this.gridOptions.pagerOption.currentPage = this.cachedPageIndex;
        this.gridOptions.pagerOption.itemsPerPage = this.cachedPageSize;
        this.gridOptions.pagerOption.totalItem = data.TotalItems;
        this.gridContainer.removeClass("hide");
        this.msgNoInfo.removeAttr("style");
        this.grid.fanexGrid(this.gridOptions);
        this.hasHostPermission && this.cancelBonus.RegisterCancelPending(this.grid.find(".cancel-pending"));
    };
    BonusDetail.prototype.initGridOptions = function () {
        var _this = this;
        return {
            customizeHeader: true,
            groupHeaderRows: [
                {
                    Columns: [
                        { HeaderText: "Created Date", Name: "colCreatedDate", MergeRowNumber: 2 },
                        { HeaderText: "Ref. No.", Name: "colRefNo", MergeRowNumber: 2 },
                        { HeaderText: "Super", Name: "colSuper", MergeColNumber: 2 },
                        { HeaderText: "Member", Name: "colMember", MergeColNumber: 3 },
                        { HeaderText: "Bonus Status", Name: "colBonusStatus", MergeRowNumber: 2 },
                        { HeaderText: "Member Feedback", Name: "colMemberFeedback", MergeRowNumber: 2 }
                    ]
                }
            ],
            columns: [
                { HeaderText: "Created Date", Width: 140, Name: "colCreatedDate", CellAlign: "center" },
                { HeaderText: "Ref. No.", Width: 100, Name: "colRefNo", HeaderAlign: "center", CellAlign: "right" },
                { HeaderText: "Username", Width: 160, Name: "colSuper", AllowTruncate: true },
                { HeaderText: "Amount", Width: 120, Name: "colSuperAmount", AllowTruncate: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00" },
                { HeaderText: "Username", Width: 160, Name: "colMember", AllowTruncate: true },
                { HeaderText: "Amount", Width: 120, Name: "colMemberAmount", AllowTruncate: true, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00" },
                { HeaderText: "Payment Date", Width: 100, Name: "colPaymentDate", CellAlign: "center" },
                { HeaderText: "Bonus Status", Width: 100, Name: "colBonusStatus", CellAlign: "center" },
                { HeaderText: "Member Feedback", Width: 120, Name: "colMemberFeedback", CellAlign: "center" }
            ],
            autoCalculateGridExpandHeight: true,
            fullHeight: !isMobile.phone,
            gridHeight: isMobile.phone ? $(window).height() - 70 : 0,
            gridExpandHeight: this.hasHostPermission ? 255 : 200,
            showDistinctRow: false,
            showPager: true,
            customPager: true,
            pagerOption: {
                itemsPerPageArray: [50, 100, 500],
                pagerSelectCallBack: function (pageIndex, pageSize) {
                    _this.cachedPageIndex = pageIndex;
                    _this.cachedPageSize = pageSize;
                    _this.getBonusDetail();
                }
            },
            showFooter: true
        };
    };
    BonusDetail.prototype.buildBonusStatusCell = function (memberBonus) {
        if (this.hasHostPermission && memberBonus.CanCancelPending) {
            return [
                '<a href="javascript:;" class="cancel-pending" data-custid="',
                memberBonus.CustId,
                '" data-transid="',
                memberBonus.TransId,
                '">',
                memberBonus.BonusStatus,
                '</a>'
            ].join("");
        }
        return memberBonus.BonusStatus;
    };
    BonusDetail.prototype.renderNoData = function () {
        this.gridContainer.removeAttr("style").addClass("hide");
        this.grid.removeAttr("style").html("");
        this.msgNoInfo.css("display", "block");
    };
    return BonusDetail;
}());
$(function () {
    var bonusDetail = new BonusDetail($);
    bonusDetail.Init();
});
//# sourceMappingURL=Fanex.ExtAcc.BonusDetail.js.map