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
var ProductControls = /** @class */ (function () {
    function ProductControls($) {
        this.$ = $;
        this.productDetailsData = [];
    }
    ProductControls.prototype.Init = function () {
        var _this = this;
        this.$.ajax({
            url: resolveClientUrl("Filter/ProductsList"),
            type: "GET",
            data: { groupName: $("#selectedGroupName").html(), groupId: Fanex.ExtAcc.Customer.GetGroupId() || '', isWinLossDetail: false },
            dataType: "json",
            cache: false,
            success: function (data) { return _this.OnSuccessLoadProduct(data); }
        });
    };
    ProductControls.prototype.OnSuccessLoadProduct = function (data) {
        var productDetails = data.productDetails;
        if (productDetails.length) {
            this.InitProductDetailsList(productDetails);
        }
        this.InitProductsList(data.products);
    };
    ProductControls.prototype.InitProductDetailsList = function (productDetails) {
        var self = this;
        var dataList = [];
        this.productDetailsDataMapToProductId = {};
        this.$.each(productDetails, function (i, item) {
            dataList.push({
                id: item.ResourceId,
                value: item.ResourceId,
                text: item.DisplayName,
                checked: true,
                enable: true
            });
            if (self.productDetailsDataMapToProductId.hasOwnProperty(item.ProductId)) {
                self.productDetailsDataMapToProductId[item.ProductId].push(item.ResourceId);
            }
            else {
                self.productDetailsDataMapToProductId[item.ProductId] = [item.ResourceId];
            }
        });
        this.$multiSelectProductDetail = this.$("#ddl-product-detail").fanexMultiSelect({
            minWidth: 220,
            checkAllText: "Select all",
            noneSelectedText: "Select none",
            useCustomCheckAllText: true,
            customCheckAllText: "Select all",
            isCheckAll: true,
            listData: dataList
        });
        $("#ddl-product-detail-container").removeClass("hidden");
        this.productDetailsData = productDetails;
    };
    ProductControls.prototype.InitProductsList = function (products) {
        var self = this;
        var $ddlProductDetail = this.$("#ddl-product-detail");
        //const $checkAllProductDetail = $("#ddl-product-detailHeaderCheckBox");
        var buildProductData = function (productList) {
            var dataList = [];
            self.$.each(productList, function (i, product) {
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
        this.$multiSelectProduct = this.$("#ddl-product").fanexMultiSelect({
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
                    if (isNaN(productId)) { // Select all
                        self.ProductsListOnCheckAll(checked, $ddlProductDetail);
                    }
                    else {
                        var $groupCheckbox = $(this);
                        if ($groupCheckbox.data("is-group")) {
                            self.ProductsListOnCheckGroup(checked, $ddlProductDetail, $groupCheckbox);
                        }
                        else {
                            self.ProductsListOnCheck(checked, productId, $ddlProductDetail);
                        }
                    }
                }
            }
        });
    };
    ProductControls.prototype.ProductsListOnCheckAll = function (checked, $ddlProductDetail) {
        $ddlProductDetail.fanexMultiSelect("setCheckAll", checked);
        $ddlProductDetail.fanexMultiSelect("setEnable", checked);
        var optionsElement = $ddlProductDetail.next(".fanex-multiselect-options");
        $.each(optionsElement.find('input[type="checkbox"].show-item'), function () {
            if ($(this).val) {
                $(this).prop("checked", checked);
                $(this).prop("disabled", !checked);
                $(this).trigger('change');
            }
            if ($(this).is("[disabled]")) {
                $(this).parent().addClass("hide-item");
            }
            else {
                $(this).parent().removeClass("hide-item");
            }
        });
    };
    ProductControls.prototype.ProductsListOnCheckGroup = function (checked, $ddlProductDetail, $groupCheckbox) {
        var self = this;
        $groupCheckbox
            .closest(".fanex-multiselect-div")
            .next(".fanex-multiselect-ul-group")
            .find('input[type="checkbox"]')
            .each(function () {
            self.ProductsListOnCheck(checked, parseInt(this.value), $ddlProductDetail);
        });
    };
    ProductControls.prototype.ProductsListOnCheck = function (checked, productId, $ddlProductDetail) {
        var filteredProductDetails = this.productDetailsData.filter(function (p) {
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
            $.each(optionsElement.find('input[type="checkbox"].show-item'), function () {
                if ($(this).is("[disabled]")) {
                    $(this).parent().addClass("hide-item");
                }
                else {
                    $(this).parent().removeClass("hide-item");
                }
            });
        });
        var maxAnabledItem = optionsElement.find('input[type="checkbox"].show-item:enabled').length;
        var checkedItem = optionsElement.find('input[type="checkbox"].show-item:enabled:checked').length;
        if (maxAnabledItem === checkedItem) {
            $("#ddl-product-detailHeaderCheckBox").prop("checked", true);
        }
        var enabledCheckboxes = optionsElement.find('input[type="checkbox"]:enabled').length - 1;
        if (checked) {
            $ddlProductDetail.fanexMultiSelect("setEnable", checked);
        }
        else if (!enabledCheckboxes) {
            $ddlProductDetail.fanexMultiSelect("setEnable", checked);
        }
    };
    ProductControls.prototype.GetParameterProducts = function () {
        var productIds = this.GetProductIds();
        return productIds.length ? "&productIds=" + productIds.join(",") : "";
    };
    ProductControls.prototype.GetParameterProductDetails = function () {
        if (this.$multiSelectProductDetail) {
            var resourceIds = this.$multiSelectProductDetail.fanexMultiSelect("getItems", true);
            return resourceIds.length ? "&resourceIds=" + resourceIds.join(",") : "";
        }
        return "";
    };
    ProductControls.prototype.GetProductIds = function () {
        this.UpdateUncheckedProductsDropdown();
        return this.$multiSelectProduct.fanexMultiSelect("getItems", true);
    };
    ProductControls.prototype.GetResourceIds = function () {
        return this.$multiSelectProductDetail
            ? this.$multiSelectProductDetail.fanexMultiSelect("getItems", true)
            : [];
    };
    ProductControls.prototype.UpdateUncheckedProductsDropdown = function () {
        if (this.productDetailsDataMapToProductId) {
            var self_1 = this;
            Object.keys(self_1.productDetailsDataMapToProductId).forEach(function (productId) {
                if (self_1.IsProductDetailsUnChecked(self_1.productDetailsDataMapToProductId[productId])) {
                    $('input[id^="fanex-multiselect-ddl-product-' + productId + '-"]').prop("checked", false).trigger("change");
                }
            });
        }
    };
    ProductControls.prototype.IsProductDetailsUnChecked = function (resourceIds) {
        for (var i = 0, max = resourceIds.length; i < max; i++) {
            if ($('input[id^="fanex-multiselect-ddl-product-detail-' + resourceIds[i] + '-"]').is(":checked")) {
                return false;
            }
        }
        return true;
    };
    return ProductControls;
}());
//# sourceMappingURL=ProductControls.js.map
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
var SuperWinLossDetailBySubGroup = /** @class */ (function () {
    function SuperWinLossDetailBySubGroup($, topWindow) {
        this.$ = $;
        this.topWindow = topWindow;
        this.productControls = new ProductControls($);
        this.dateTimeControls = new DateTimeControls($);
    }
    SuperWinLossDetailBySubGroup.prototype.Init = function () {
        var _this = this;
        this.cacheElements();
        this.bindEvents();
        this.productControls.Init();
        this.initSubGroupDropdown();
        this.dateTimeControls.Init();
        this.$("#userGroup input")
            .attr("readonly", "true")
            .bind("click", function (event) { return _this.$(event.currentTarget).autocomplete("search", ""); });
    };
    SuperWinLossDetailBySubGroup.prototype.cacheElements = function () {
        this.iconFilter = this.$("#span-iconFilter");
        this.filterContainer = this.$("#filter-container");
        this.ddlSubGroup = this.$("#ddl-subGroup");
        this.btnExportExcel = this.$("#exporttoexcel");
        this.gridContainer = this.$("#mainGrid");
        this.grid = this.$("#jqgrid");
        this.msgNoInfo = this.$("#msgNoinfo");
        this.cacheFormData = {
            pageSize: 200,
            groupId: this.$("#hidden-groupId").val(),
            groupName: this.$("#hidden-groupName").val()
        };
    };
    SuperWinLossDetailBySubGroup.prototype.bindEvents = function () {
        var _this = this;
        this.iconFilter.click(function () { return _this.showHideFilter(); });
        this.$("#btn-submit").click(function () {
            _this.setCacheFormData();
            _this.getSuperWinLossDetailBySubGroupReport();
        });
        this.btnExportExcel.click(function () { return _this.exportExcel(); });
        Fanex.ExtAcc.OnResize = function () { return _this.resizeReportGrid; };
    };
    SuperWinLossDetailBySubGroup.prototype.showHideFilter = function () {
        if (this.iconFilter.hasClass("active")) {
            this.iconFilter.removeClass("active");
            this.filterContainer.addClass("hide");
        }
        else {
            this.iconFilter.addClass("active");
            this.filterContainer.removeClass("hide");
        }
        this.resizeReportGrid();
    };
    SuperWinLossDetailBySubGroup.prototype.resizeReportGrid = function () {
        try {
            this.gridOptions && this.grid.fanexGrid("resizeGrid");
        }
        catch (err) {
            console.log(err);
        }
    };
    SuperWinLossDetailBySubGroup.prototype.setCacheFormData = function () {
        var fromtoDateParams = this.dateTimeControls.GetFromDateToDate();
        this.cacheFormData.productIds = this.productControls.GetProductIds().join(",");
        this.cacheFormData.productDetails = this.productControls.GetResourceIds().join(",");
        var subGroup = this.ddlSubGroup.val();
        this.cacheFormData.subGroup = subGroup === "All" ? null : subGroup;
        this.cacheFormData.fromDate = fromtoDateParams.fromdate;
        this.cacheFormData.toDate = fromtoDateParams.todate;
        this.cacheFormData.pageIndex = 0;
    };
    SuperWinLossDetailBySubGroup.prototype.getSuperWinLossDetailBySubGroupReport = function () {
        var _this = this;
        Fanex.ExtAcc.BlockUI();
        var urlAndQueryString = resolveClientUrl("SuperWinLossDetailBySubGroup/GetData?") + this.$.param(this.cacheFormData);
        $.getJSON(urlAndQueryString, function (data) {
            if (data.totalItems) {
                _this.renderSuperWinLossDetailBySubGroup(data);
            }
            else {
                _this.renderNoData();
            }
        }).done(Fanex.ExtAcc.UnBlockUI);
    };
    SuperWinLossDetailBySubGroup.prototype.renderSuperWinLossDetailBySubGroup = function (data) {
        var rewardHeader = this.getRewardHeaderText(data);
        this.gridOptions = this.gridOptions || this.initGridOptions(rewardHeader);
        setPagerOptionOnMobile(this.gridOptions);
        this.gridOptions.titleInfo.text = this.buildGridTitle();
        this.gridOptions.columns = this.initGridHeaderColumns(rewardHeader);
        this.gridOptions.bodyRows = this.buildBodyRows(data.superWinLossDetails);
        this.gridOptions.footerRows = [this.buildGrandTotalColumns(data.total)];
        this.gridOptions.pagerOption.currentPage = this.cacheFormData.pageIndex;
        this.gridOptions.pagerOption.itemsPerPage = this.cacheFormData.pageSize;
        this.gridOptions.pagerOption.totalItem = data.totalItems;
        this.gridContainer.removeClass("hide");
        this.btnExportExcel.removeClass("hide");
        this.msgNoInfo.removeAttr("style");
        this.grid.fanexGrid(this.gridOptions);
        this.addTitleToSubGroupUsernameColumn();
        workaroundForGridOnPhone();
    };
    SuperWinLossDetailBySubGroup.prototype.initGridOptions = function (rewardHeader) {
        var _this = this;
        return {
            showTitle: true,
            titleInfo: { text: "" },
            customizeHeader: true,
            groupHeaderRows: [{ Columns: this.initGridGroupHeaderColumns(rewardHeader) }],
            columns: this.initGridHeaderColumns(rewardHeader),
            numberOfFrozenColumn: this.topWindow.$(top.window).width() < 768 ? 1 : 3,
            autoCalculateGridExpandHeight: true,
            fullHeight: !isMobile.phone,
            gridHeight: isMobile.phone ? $(window).height() - 70 : 0,
            gridExpandHeight: 256,
            numOfPaddingForGridExpandHeight: 40,
            showDistinctRow: false,
            showPager: true,
            customPager: true,
            pagerOption: {
                itemsPerPageArray: [50, 100, 200],
                pagerSelectCallBack: function (pageIndex, pageSize) {
                    _this.cacheFormData.pageIndex = pageIndex;
                    _this.cacheFormData.pageSize = pageSize;
                    _this.getSuperWinLossDetailBySubGroupReport();
                }
            },
            showFooter: true,
            showTree: true,
            marginRight: 10,
            bottomBorderWidth: 0,
            sortOption: {
                customGridSort: function (rows, sortField, sortDirection, customSortFunction, sortType) { return _this.getSortedBodyRows(rows, sortField, sortDirection, customSortFunction, sortType); },
                afterGridSorted: function (rows, sortField, sortDirection) {
                    _this.addTitleToSubGroupUsernameColumn();
                    return rows;
                }
            },
            allowTruncate: true
        };
    };
    SuperWinLossDetailBySubGroup.prototype.initGridGroupHeaderColumns = function (rewardHeader) {
        var initMergeRow = function (text, name) {
            return { HeaderText: text, Name: name, MergeRowNumber: 2 };
        };
        var initMergeColumn = function (text, name) {
            return { HeaderText: text, Name: name, MergeColNumber: 3 };
        };
        return [
            initMergeRow("Sub Group/Username", "colSubGroupUsername"),
            initMergeRow("Turnover", "colTurnover"),
            initMergeRow("Net Turnover", "colNetTurnover"),
            initMergeRow("GrossComm", "colGrossComm"),
            initMergeColumn("Member", "colMember"),
            initMergeColumn("Agent", "colAgent"),
            initMergeColumn("Master", "colMaster"),
            initMergeColumn("Super", "colSuper"),
            initMergeRow("Company", "colCompany"),
            initMergeRow(rewardHeader, "colRewardAmount")
        ];
    };
    SuperWinLossDetailBySubGroup.prototype.initGridHeaderColumns = function (rewardHeader) {
        var initGridHeaderColumn = function (text, name) {
            return {
                HeaderText: text,
                Name: name,
                Sort: true,
                Width: 130,
                HeaderAlign: "center",
                CellAlign: "right",
                FormatNumber: "0,0.00",
                FormatNegative: "$"
            };
        };
        return [
            { HeaderText: "Sub Group/Username", Name: "colSubGroupUsername", Sort: true, SortDirection: "asc", SortType: "string", Width: 150, ShowCollapseExpand: true },
            initGridHeaderColumn("Turnover", "colTurnover"),
            initGridHeaderColumn("Net Turnover", "colNetTurnover"),
            initGridHeaderColumn("GrossComm", "colGrossComm"),
            initGridHeaderColumn("Win/Loss", "colMember"),
            initGridHeaderColumn("Comm", "colMemberComm"),
            initGridHeaderColumn("Total", "colMemberTotal"),
            initGridHeaderColumn("Win/Loss", "colAgent"),
            initGridHeaderColumn("Comm", "colAgentComm"),
            initGridHeaderColumn("Total", "colAgentTotal"),
            initGridHeaderColumn("Win/Loss", "colMaster"),
            initGridHeaderColumn("Comm", "colMasterComm"),
            initGridHeaderColumn("Total", "colMasterTotal"),
            initGridHeaderColumn("Win/Loss", "colSuper"),
            initGridHeaderColumn("Comm", "colSuperComm"),
            initGridHeaderColumn("Total", "colSuperTotal"),
            initGridHeaderColumn("Company", "colCompany"),
            initGridHeaderColumn("" + rewardHeader, "colRewardAmount")
        ];
    };
    SuperWinLossDetailBySubGroup.prototype.buildGridTitle = function () {
        var title = ["Super Win Loss Detail By Sub Group"];
        if (this.cacheFormData.fromDate === this.cacheFormData.toDate) {
            title.push("on");
            title.push(this.cacheFormData.fromDate);
        }
        else {
            title.push("from");
            title.push(this.cacheFormData.fromDate);
            title.push("to");
            title.push(this.cacheFormData.toDate);
        }
        return title.join(" ");
    };
    SuperWinLossDetailBySubGroup.prototype.getRewardHeaderText = function (data) {
        if (data.superWinLossDetails && data.superWinLossDetails.length > 0) {
            return "Reward (" + data.superWinLossDetails[0].rewardCurrency + ")";
        }
        ;
        return "Reward";
    };
    SuperWinLossDetailBySubGroup.prototype.buildBodyRows = function (superWinLossDetailBySubGroups) {
        var _this = this;
        var bodyRows = [];
        this.$.each(superWinLossDetailBySubGroups, function (i, winlossSubGroup) {
            var subBobyRows = [];
            _this.$.each(winlossSubGroup.superWinLossDetails, function (j, winloss) {
                subBobyRows.push(_this.buildUsernameColumns(winloss));
            });
            bodyRows.push(_this.buildSubGroupColumns(winlossSubGroup, subBobyRows));
        });
        return bodyRows;
    };
    SuperWinLossDetailBySubGroup.prototype.buildSubGroupColumns = function (winlossSubGroup, subBobyRows) {
        return {
            Columns: this.buildGridColumns(winlossSubGroup.subGroup, winlossSubGroup),
            SubRows: subBobyRows,
            CssClass: "sub-row-class"
        };
    };
    SuperWinLossDetailBySubGroup.prototype.buildUsernameColumns = function (winloss) {
        return {
            Columns: this.buildGridColumns(winloss.username, winloss)
        };
    };
    SuperWinLossDetailBySubGroup.prototype.buildGrandTotalColumns = function (totalWinloss) {
        return {
            Columns: this.buildGridColumns("Grand Total", totalWinloss)
        };
    };
    SuperWinLossDetailBySubGroup.prototype.buildGridColumns = function (subGroupOrUsername, winloss) {
        return [
            { Name: "colSubGroupUsername", Value: subGroupOrUsername, CssClass: "cellSubGroupOrUsername" },
            { Name: "colTurnover", Value: winloss.turnover },
            { Name: "colNetTurnover", Value: winloss.netTurnover },
            { Name: "colGrossComm", Value: winloss.grossComm },
            { Name: "colMember", Value: winloss.memberWinLoss },
            { Name: "colMemberComm", Value: winloss.memberComm },
            { Name: "colMemberTotal", Value: winloss.memberTotal },
            { Name: "colAgent", Value: winloss.agentWinLoss },
            { Name: "colAgentComm", Value: winloss.agentComm },
            { Name: "colAgentTotal", Value: winloss.agentTotal },
            { Name: "colMaster", Value: winloss.masterWinLoss },
            { Name: "colMasterComm", Value: winloss.masterComm },
            { Name: "colMasterTotal", Value: winloss.masterTotal },
            { Name: "colSuper", Value: winloss.superWinLoss },
            { Name: "colSuperComm", Value: winloss.superComm },
            { Name: "colSuperTotal", Value: winloss.superTotal },
            { Name: "colCompany", Value: winloss.company },
            { Name: "colRewardAmount", Value: winloss.rewardAmount }
        ];
    };
    SuperWinLossDetailBySubGroup.prototype.renderNoData = function () {
        this.gridContainer.removeAttr("style").addClass("hide");
        this.grid.removeAttr("style").html("");
        this.btnExportExcel.addClass("hide");
        this.msgNoInfo.css("display", "block");
    };
    SuperWinLossDetailBySubGroup.prototype.exportExcel = function () {
        var urlAndQueryString = resolveClientUrl("SuperWinLossDetailBySubGroup/ExportExcel?") + this.$.param(this.cacheFormData);
        this.topWindow.exportExcel(urlAndQueryString, document);
    };
    SuperWinLossDetailBySubGroup.prototype.initSubGroupDropdown = function () {
        var _this = this;
        this.ddlSubGroup.combobox();
        this.ddlSubGroup.next("input.ui-autocomplete-input")
            .val(this.ddlSubGroup.find("option:selected").text())
            .attr("readonly", "true")
            //.css("width", "150px")
            .bind("click", function (event) { return _this.$(event.currentTarget).autocomplete("search", ""); });
    };
    SuperWinLossDetailBySubGroup.prototype.getSortedBodyRows = function (rows, sortField, sortDirection, customSortFunction, sortType) {
        if (!sortField || !rows.length) {
            return rows;
        }
        var sortColIndex = this.getSortedColumnIndex(rows, sortField);
        var compareFunction;
        if (sortType === "string") {
            compareFunction = sortDirection === "asc"
                ? this.compareStringASC
                : this.compareStringDESC;
        }
        else {
            compareFunction = sortDirection === "asc"
                ? this.compareNumberASC
                : this.compareNumberDESC;
        }
        // sort by sub group
        rows.sort(function (row1, row2) {
            return compareFunction(row1.Columns[sortColIndex].Value, row2.Columns[sortColIndex].Value);
        });
        // sort by username
        this.$.each(rows, function (index, row) {
            row.SubRows.sort(function (subrow1, subrow2) {
                return compareFunction(subrow1.Columns[sortColIndex].Value, subrow2.Columns[sortColIndex].Value);
            });
        });
        return rows;
    };
    SuperWinLossDetailBySubGroup.prototype.getSortedColumnIndex = function (rows, sortField) {
        var firstRowColumns = rows[0].Columns;
        for (var i = 0, max = firstRowColumns.length; i < max; i++) {
            if (firstRowColumns[i].Name === sortField) {
                return i;
            }
        }
        return -1;
    };
    SuperWinLossDetailBySubGroup.prototype.compareStringASC = function (text1, text2) {
        var uppercaseText1 = text1.toUpperCase();
        var uppercaseText2 = text2.toUpperCase();
        if (uppercaseText1 === "UNASSIGNED SUPERS") {
            return 1;
        }
        if (uppercaseText2 === "UNASSIGNED SUPERS") {
            return -1;
        }
        if (uppercaseText1 < uppercaseText2) {
            return -1;
        }
        if (uppercaseText1 > uppercaseText2) {
            return 1;
        }
        return 0;
    };
    SuperWinLossDetailBySubGroup.prototype.compareStringDESC = function (text1, text2) {
        var uppercaseText1 = text1.toUpperCase();
        var uppercaseText2 = text2.toUpperCase();
        if (uppercaseText1 === "UNASSIGNED SUPERS") {
            return 1;
        }
        if (uppercaseText2 === "UNASSIGNED SUPERS") {
            return -1;
        }
        if (uppercaseText1 > uppercaseText2) {
            return -1;
        }
        if (uppercaseText1 < uppercaseText2) {
            return 1;
        }
        return 0;
    };
    SuperWinLossDetailBySubGroup.prototype.compareNumberASC = function (number1, number2) {
        if (number1 < number2) {
            return -1;
        }
        if (number1 > number2) {
            return 1;
        }
        return 0;
    };
    SuperWinLossDetailBySubGroup.prototype.compareNumberDESC = function (number1, number2) {
        if (number1 > number2) {
            return -1;
        }
        if (number1 < number2) {
            return 1;
        }
        return 0;
    };
    SuperWinLossDetailBySubGroup.prototype.addTitleToSubGroupUsernameColumn = function () {
        $(".cellSubGroupOrUsername").each(function () {
            var $this = $(this);
            $this.attr("title", $this.text());
        });
    };
    return SuperWinLossDetailBySubGroup;
}());
$(function () {
    new SuperWinLossDetailBySubGroup($, window.top).Init();
});
//# sourceMappingURL=Fanex.ExtAcc.SuperWinLossDetailBySubGroup.js.map