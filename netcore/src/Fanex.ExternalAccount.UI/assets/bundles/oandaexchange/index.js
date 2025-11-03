"use strict";
var DateRangePickerJQueryOption = /** @class */ (function () {
    function DateRangePickerJQueryOption(onClosed, language, selectorId) {
        if (onClosed === void 0) { onClosed = Function(); }
        if (language === void 0) { language = "default"; }
        if (selectorId === void 0) { selectorId = "daterange-picker"; }
        this.onClosed = onClosed;
        this.language = language;
        this.selectorId = selectorId;
        this.onClosed = onClosed;
        this.language = language;
        this.selectorId = selectorId;
    }
    return DateRangePickerJQueryOption;
}());
var DateRangePickerJQueryComponent = /** @class */ (function () {
    function DateRangePickerJQueryComponent() {
    }
    DateRangePickerJQueryComponent.prototype.render = function (options) {
        var _this = this;
        var dateRangePicker = $("#" + options.selectorId);
        this.historicPivotDateText = dateRangePicker.data("historicpivotdate");
        var historicPivotDate = moment(this.historicPivotDateText, "MM/dd/yyyy");
        this.fromDate = dateRangePicker.data("fromdate");
        this.toDate = dateRangePicker.data("todate");
        this.historyMaxDateRange = dateRangePicker.data("historymaxdaterange");
        this.historyMaxMonthRange = dateRangePicker.data("historymaxmonthrange");
        dateRangePicker
            .dateRangePicker({
            format: "MM/DD/YYYY",
            separator: " - ",
            language: options.language,
            startDate: dateRangePicker.data("mindate"),
            endDate: dateRangePicker.data("maxdate"),
            showShortcuts: true,
            shortcuts: null,
            customShortcuts: this.getPredefinedRanges(dateRangePicker),
            duration: 0,
            singleMonth: true,
            monthSelect: true,
            yearSelect: true
        }).bind("datepicker-change", function (_, obj) {
            var startDate = moment(obj.date1, "MM/dd/yyyy");
            var endDate = moment(obj.date2, "MM/dd/yyyy");
            if (endDate.isBefore(historicPivotDate)
                && (_this.historyMaxDateRange != undefined && endDate.diff(startDate, "d") > _this.historyMaxDateRange
                    || _this.historyMaxMonthRange != undefined && endDate.diff(moment(obj.date1, "MM/dd/yyyy").add(_this.historyMaxMonthRange, "months"), "d") > 0)) {
                try {
                    Fanex.ExtAcc.Form.Alert('Date Range Error', dateRangePicker.data("label-maxdataretrievehistoryerror"), 400, 150, null, null, 0);
                }
                catch (e) {
                    alert(dateRangePicker.data("label-maxdataretrievehistoryerror"));
                }
                dateRangePicker.data("dateRangePicker").setDateRange(_this.fromDate, _this.toDate, false);
                return;
            }
            if (startDate.isBefore(historicPivotDate) && endDate.isSameOrAfter(historicPivotDate)
                && (_this.historyMaxDateRange != undefined && endDate.diff(startDate, "d") > _this.historyMaxDateRange
                    || _this.historyMaxMonthRange != undefined && endDate.diff(moment(obj.date1, "MM/dd/yyyy").add(_this.historyMaxMonthRange, "months"), "d") > 0)) {
                try {
                    Fanex.ExtAcc.Form.Alert('Date Range Error', dateRangePicker.data("label-maxdataretrievehistoryerror"), 400, 150, null, null, 0);
                }
                catch (e) {
                    alert(dateRangePicker.data("label-maxdataretrievehistoryerror"));
                }
                dateRangePicker.data("dateRangePicker").setDateRange(_this.fromDate, _this.toDate, false);
                return;
            }
            _this.fromDate = startDate.format("MM/DD/YYYY");
            _this.toDate = endDate.format("MM/DD/YYYY");
        })
            .bind("datepicker-closed", function () { return options.onClosed(_this.getDateRangeValues()); })
            .data("dateRangePicker").setDateRange(this.fromDate, this.toDate, false);
    };
    DateRangePickerJQueryComponent.prototype.getPredefinedRanges = function (dateRangePicker) {
        var minDate = moment(dateRangePicker.data("mindate"));
        var today = moment(dateRangePicker.data("currentdate"));
        var todayValue = today.toDate();
        var yesterday = moment(today).subtract(1, "d");
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
            },
            {
                name: dateRangePicker.data("label-thismonth"),
                dates: function () {
                    var thisMonthStartOfDay = moment(today).startOf("M");
                    return [
                        thisMonthStartOfDay.diff(minDate) >= 0 ? thisMonthStartOfDay.toDate() : minDate.toDate(),
                        todayValue
                    ];
                }
            },
            {
                name: dateRangePicker.data("label-lastmonth"),
                dates: function () {
                    return [
                        lastMonthStartOfDay.diff(minDate) >= 0 ? lastMonthStartOfDay.toDate() : minDate.toDate(),
                        moment(lastMonthStartOfDay).endOf("M").toDate()
                    ];
                }
            },
            {
                name: dateRangePicker.data("label-sincelastmonth"),
                dates: function () {
                    return [
                        lastMonthStartOfDay.diff(minDate) >= 0 ? lastMonthStartOfDay.toDate() : minDate.toDate(),
                        todayValue
                    ];
                }
            }
        ];
    };
    DateRangePickerJQueryComponent.prototype.getDateRangeValues = function () {
        return { fromDate: this.fromDate, toDate: this.toDate };
    };
    DateRangePickerJQueryComponent.prototype.getHistoricPivotDate = function () {
        return this.historicPivotDateText;
    };
    return DateRangePickerJQueryComponent;
}());
//# sourceMappingURL=daterangepicker-jquery.js.map
/*!
 * clipboard.js v1.5.15
 * https://zenorocha.github.io/clipboard.js
 *
 * Licensed MIT © Zeno Rocha
 */
(function (f) { if (typeof exports === "object" && typeof module !== "undefined") { module.exports = f() } else if (typeof define === "function" && define.amd) { define([], f) } else { var g; if (typeof window !== "undefined") { g = window } else if (typeof global !== "undefined") { g = global } else if (typeof self !== "undefined") { g = self } else { g = this } g.Clipboard = f() } })(function () {
    var define, module, exports; return (function e(t, n, r) { function s(o, u) { if (!n[o]) { if (!t[o]) { var a = typeof require == "function" && require; if (!u && a) return a(o, !0); if (i) return i(o, !0); var f = new Error("Cannot find module '" + o + "'"); throw f.code = "MODULE_NOT_FOUND", f } var l = n[o] = { exports: {} }; t[o][0].call(l.exports, function (e) { var n = t[o][1][e]; return s(n ? n : e) }, l, l.exports, e, t, n, r) } return n[o].exports } var i = typeof require == "function" && require; for (var o = 0; o < r.length; o++)s(r[o]); return s })({
        1: [function (require, module, exports) {
            /**
             * A polyfill for Element.matches()
             */
            if (Element && !Element.prototype.matches) {
                var proto = Element.prototype;

                proto.matches = proto.matchesSelector ||
                    proto.mozMatchesSelector ||
                    proto.msMatchesSelector ||
                    proto.oMatchesSelector ||
                    proto.webkitMatchesSelector;
            }

            /**
             * Finds the closest parent that matches a selector.
             *
             * @param {Element} element
             * @param {String} selector
             * @return {Function}
             */
            function closest(element, selector) {
                while (element && element !== document) {
                    if (element.matches(selector)) return element;
                    element = element.parentNode;
                }
            }

            module.exports = closest;

        }, {}], 2: [function (require, module, exports) {
            var closest = require('./closest');

            /**
             * Delegates event to a selector.
             *
             * @param {Element} element
             * @param {String} selector
             * @param {String} type
             * @param {Function} callback
             * @param {Boolean} useCapture
             * @return {Object}
             */
            function delegate(element, selector, type, callback, useCapture) {
                var listenerFn = listener.apply(this, arguments);

                element.addEventListener(type, listenerFn, useCapture);

                return {
                    destroy: function () {
                        element.removeEventListener(type, listenerFn, useCapture);
                    }
                }
            }

            /**
             * Finds closest match and invokes callback.
             *
             * @param {Element} element
             * @param {String} selector
             * @param {String} type
             * @param {Function} callback
             * @return {Function}
             */
            function listener(element, selector, type, callback) {
                return function (e) {
                    e.delegateTarget = closest(e.target, selector);

                    if (e.delegateTarget) {
                        callback.call(element, e);
                    }
                }
            }

            module.exports = delegate;

        }, { "./closest": 1 }], 3: [function (require, module, exports) {
            /**
             * Check if argument is a HTML element.
             *
             * @param {Object} value
             * @return {Boolean}
             */
            exports.node = function (value) {
                return value !== undefined
                    && value instanceof HTMLElement
                    && value.nodeType === 1;
            };

            /**
             * Check if argument is a list of HTML elements.
             *
             * @param {Object} value
             * @return {Boolean}
             */
            exports.nodeList = function (value) {
                var type = Object.prototype.toString.call(value);

                return value !== undefined
                    && (type === '[object NodeList]' || type === '[object HTMLCollection]')
                    && ('length' in value)
                    && (value.length === 0 || exports.node(value[0]));
            };

            /**
             * Check if argument is a string.
             *
             * @param {Object} value
             * @return {Boolean}
             */
            exports.string = function (value) {
                return typeof value === 'string'
                    || value instanceof String;
            };

            /**
             * Check if argument is a function.
             *
             * @param {Object} value
             * @return {Boolean}
             */
            exports.fn = function (value) {
                var type = Object.prototype.toString.call(value);

                return type === '[object Function]';
            };

        }, {}], 4: [function (require, module, exports) {
            var is = require('./is');
            var delegate = require('delegate');

            /**
             * Validates all params and calls the right
             * listener function based on its target type.
             *
             * @param {String|HTMLElement|HTMLCollection|NodeList} target
             * @param {String} type
             * @param {Function} callback
             * @return {Object}
             */
            function listen(target, type, callback) {
                if (!target && !type && !callback) {
                    throw new Error('Missing required arguments');
                }

                if (!is.string(type)) {
                    throw new TypeError('Second argument must be a String');
                }

                if (!is.fn(callback)) {
                    throw new TypeError('Third argument must be a Function');
                }

                if (is.node(target)) {
                    return listenNode(target, type, callback);
                }
                else if (is.nodeList(target)) {
                    return listenNodeList(target, type, callback);
                }
                else if (is.string(target)) {
                    return listenSelector(target, type, callback);
                }
                else {
                    throw new TypeError('First argument must be a String, HTMLElement, HTMLCollection, or NodeList');
                }
            }

            /**
             * Adds an event listener to a HTML element
             * and returns a remove listener function.
             *
             * @param {HTMLElement} node
             * @param {String} type
             * @param {Function} callback
             * @return {Object}
             */
            function listenNode(node, type, callback) {
                node.addEventListener(type, callback);

                return {
                    destroy: function () {
                        node.removeEventListener(type, callback);
                    }
                }
            }

            /**
             * Add an event listener to a list of HTML elements
             * and returns a remove listener function.
             *
             * @param {NodeList|HTMLCollection} nodeList
             * @param {String} type
             * @param {Function} callback
             * @return {Object}
             */
            function listenNodeList(nodeList, type, callback) {
                Array.prototype.forEach.call(nodeList, function (node) {
                    node.addEventListener(type, callback);
                });

                return {
                    destroy: function () {
                        Array.prototype.forEach.call(nodeList, function (node) {
                            node.removeEventListener(type, callback);
                        });
                    }
                }
            }

            /**
             * Add an event listener to a selector
             * and returns a remove listener function.
             *
             * @param {String} selector
             * @param {String} type
             * @param {Function} callback
             * @return {Object}
             */
            function listenSelector(selector, type, callback) {
                return delegate(document.body, selector, type, callback);
            }

            module.exports = listen;

        }, { "./is": 3, "delegate": 2 }], 5: [function (require, module, exports) {
            function select(element) {
                var selectedText;

                if (element.nodeName === 'SELECT') {
                    element.focus();

                    selectedText = element.value;
                }
                else if (element.nodeName === 'INPUT' || element.nodeName === 'TEXTAREA') {
                    element.focus();
                    element.setSelectionRange(0, element.value.length);

                    selectedText = element.value;
                }
                else {
                    if (element.hasAttribute('contenteditable')) {
                        element.focus();
                    }

                    var selection = window.getSelection();
                    var range = document.createRange();

                    range.selectNodeContents(element);
                    selection.removeAllRanges();
                    selection.addRange(range);

                    selectedText = selection.toString();
                }

                return selectedText;
            }

            module.exports = select;

        }, {}], 6: [function (require, module, exports) {
            function E() {
                // Keep this empty so it's easier to inherit from
                // (via https://github.com/lipsmack from https://github.com/scottcorgan/tiny-emitter/issues/3)
            }

            E.prototype = {
                on: function (name, callback, ctx) {
                    var e = this.e || (this.e = {});

                    (e[name] || (e[name] = [])).push({
                        fn: callback,
                        ctx: ctx
                    });

                    return this;
                },

                once: function (name, callback, ctx) {
                    var self = this;
                    function listener() {
                        self.off(name, listener);
                        callback.apply(ctx, arguments);
                    };

                    listener._ = callback
                    return this.on(name, listener, ctx);
                },

                emit: function (name) {
                    var data = [].slice.call(arguments, 1);
                    var evtArr = ((this.e || (this.e = {}))[name] || []).slice();
                    var i = 0;
                    var len = evtArr.length;

                    for (i; i < len; i++) {
                        evtArr[i].fn.apply(evtArr[i].ctx, data);
                    }

                    return this;
                },

                off: function (name, callback) {
                    var e = this.e || (this.e = {});
                    var evts = e[name];
                    var liveEvents = [];

                    if (evts && callback) {
                        for (var i = 0, len = evts.length; i < len; i++) {
                            if (evts[i].fn !== callback && evts[i].fn._ !== callback)
                                liveEvents.push(evts[i]);
                        }
                    }

                    // Remove event from queue to prevent memory leak
                    // Suggested by https://github.com/lazd
                    // Ref: https://github.com/scottcorgan/tiny-emitter/commit/c6ebfaa9bc973b33d110a84a307742b7cf94c953#commitcomment-5024910

                    (liveEvents.length)
                        ? e[name] = liveEvents
                        : delete e[name];

                    return this;
                }
            };

            module.exports = E;

        }, {}], 7: [function (require, module, exports) {
            (function (global, factory) {
                if (typeof define === "function" && define.amd) {
                    define(['module', 'select'], factory);
                } else if (typeof exports !== "undefined") {
                    factory(module, require('select'));
                } else {
                    var mod = {
                        exports: {}
                    };
                    factory(mod, global.select);
                    global.clipboardAction = mod.exports;
                }
            })(this, function (module, _select) {
                'use strict';

                var _select2 = _interopRequireDefault(_select);

                function _interopRequireDefault(obj) {
                    return obj && obj.__esModule ? obj : {
                        default: obj
                    };
                }

                var _typeof = typeof Symbol === "function" && typeof Symbol.iterator === "symbol" ? function (obj) {
                    return typeof obj;
                } : function (obj) {
                    return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj;
                };

                function _classCallCheck(instance, Constructor) {
                    if (!(instance instanceof Constructor)) {
                        throw new TypeError("Cannot call a class as a function");
                    }
                }

                var _createClass = function () {
                    function defineProperties(target, props) {
                        for (var i = 0; i < props.length; i++) {
                            var descriptor = props[i];
                            descriptor.enumerable = descriptor.enumerable || false;
                            descriptor.configurable = true;
                            if ("value" in descriptor) descriptor.writable = true;
                            Object.defineProperty(target, descriptor.key, descriptor);
                        }
                    }

                    return function (Constructor, protoProps, staticProps) {
                        if (protoProps) defineProperties(Constructor.prototype, protoProps);
                        if (staticProps) defineProperties(Constructor, staticProps);
                        return Constructor;
                    };
                }();

                var ClipboardAction = function () {
                    /**
                     * @param {Object} options
                     */
                    function ClipboardAction(options) {
                        _classCallCheck(this, ClipboardAction);

                        this.resolveOptions(options);
                        this.initSelection();
                    }

                    /**
                     * Defines base properties passed from constructor.
                     * @param {Object} options
                     */


                    _createClass(ClipboardAction, [{
                        key: 'resolveOptions',
                        value: function resolveOptions() {
                            var options = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : {};

                            this.action = options.action;
                            this.emitter = options.emitter;
                            this.target = options.target;
                            this.text = options.text;
                            this.trigger = options.trigger;

                            this.selectedText = '';
                        }
                    }, {
                        key: 'initSelection',
                        value: function initSelection() {
                            if (this.text) {
                                this.selectFake();
                            } else if (this.target) {
                                this.selectTarget();
                            }
                        }
                    }, {
                        key: 'selectFake',
                        value: function selectFake() {
                            var _this = this;

                            var isRTL = document.documentElement.getAttribute('dir') == 'rtl';

                            this.removeFake();

                            this.fakeHandlerCallback = function () {
                                return _this.removeFake();
                            };
                            this.fakeHandler = document.body.addEventListener('click', this.fakeHandlerCallback) || true;

                            this.fakeElem = document.createElement('textarea');
                            // Prevent zooming on iOS
                            this.fakeElem.style.fontSize = '12pt';
                            // Reset box model
                            this.fakeElem.style.border = '0';
                            this.fakeElem.style.padding = '0';
                            this.fakeElem.style.margin = '0';
                            // Move element out of screen horizontally
                            this.fakeElem.style.position = 'absolute';
                            this.fakeElem.style[isRTL ? 'right' : 'left'] = '-9999px';
                            // Move element to the same position vertically
                            var yPosition = window.pageYOffset || document.documentElement.scrollTop;
                            this.fakeElem.addEventListener('focus', window.scrollTo(0, yPosition));
                            this.fakeElem.style.top = yPosition + 'px';

                            this.fakeElem.setAttribute('readonly', '');
                            this.fakeElem.value = this.text;

                            document.body.appendChild(this.fakeElem);

                            this.selectedText = (0, _select2.default)(this.fakeElem);
                            this.copyText();
                        }
                    }, {
                        key: 'removeFake',
                        value: function removeFake() {
                            if (this.fakeHandler) {
                                document.body.removeEventListener('click', this.fakeHandlerCallback);
                                this.fakeHandler = null;
                                this.fakeHandlerCallback = null;
                            }

                            if (this.fakeElem) {
                                document.body.removeChild(this.fakeElem);
                                this.fakeElem = null;
                            }
                        }
                    }, {
                        key: 'selectTarget',
                        value: function selectTarget() {
                            this.selectedText = (0, _select2.default)(this.target);
                            this.copyText();
                        }
                    }, {
                        key: 'copyText',
                        value: function copyText() {
                            var succeeded = void 0;

                            try {
                                succeeded = document.execCommand(this.action);
                            } catch (err) {
                                succeeded = false;
                            }

                            this.handleResult(succeeded);
                        }
                    }, {
                        key: 'handleResult',
                        value: function handleResult(succeeded) {
                            this.emitter.emit(succeeded ? 'success' : 'error', {
                                action: this.action,
                                text: this.selectedText,
                                trigger: this.trigger,
                                clearSelection: this.clearSelection.bind(this)
                            });
                        }
                    }, {
                        key: 'clearSelection',
                        value: function clearSelection() {
                            if (this.target) {
                                this.target.blur();
                            }

                            window.getSelection().removeAllRanges();
                        }
                    }, {
                        key: 'destroy',
                        value: function destroy() {
                            this.removeFake();
                        }
                    }, {
                        key: 'action',
                        set: function set() {
                            var action = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : 'copy';

                            this._action = action;

                            if (this._action !== 'copy' && this._action !== 'cut') {
                                throw new Error('Invalid "action" value, use either "copy" or "cut"');
                            }
                        },
                        get: function get() {
                            return this._action;
                        }
                    }, {
                        key: 'target',
                        set: function set(target) {
                            if (target !== undefined) {
                                if (target && (typeof target === 'undefined' ? 'undefined' : _typeof(target)) === 'object' && target.nodeType === 1) {
                                    if (this.action === 'copy' && target.hasAttribute('disabled')) {
                                        throw new Error('Invalid "target" attribute. Please use "readonly" instead of "disabled" attribute');
                                    }

                                    if (this.action === 'cut' && (target.hasAttribute('readonly') || target.hasAttribute('disabled'))) {
                                        throw new Error('Invalid "target" attribute. You can\'t cut text from elements with "readonly" or "disabled" attributes');
                                    }

                                    this._target = target;
                                } else {
                                    throw new Error('Invalid "target" value, use a valid Element');
                                }
                            }
                        },
                        get: function get() {
                            return this._target;
                        }
                    }]);

                    return ClipboardAction;
                }();

                module.exports = ClipboardAction;
            });

        }, { "select": 5 }], 8: [function (require, module, exports) {
            (function (global, factory) {
                if (typeof define === "function" && define.amd) {
                    define(['module', './clipboard-action', 'tiny-emitter', 'good-listener'], factory);
                } else if (typeof exports !== "undefined") {
                    factory(module, require('./clipboard-action'), require('tiny-emitter'), require('good-listener'));
                } else {
                    var mod = {
                        exports: {}
                    };
                    factory(mod, global.clipboardAction, global.tinyEmitter, global.goodListener);
                    global.clipboard = mod.exports;
                }
            })(this, function (module, _clipboardAction, _tinyEmitter, _goodListener) {
                'use strict';

                var _clipboardAction2 = _interopRequireDefault(_clipboardAction);

                var _tinyEmitter2 = _interopRequireDefault(_tinyEmitter);

                var _goodListener2 = _interopRequireDefault(_goodListener);

                function _interopRequireDefault(obj) {
                    return obj && obj.__esModule ? obj : {
                        default: obj
                    };
                }

                function _classCallCheck(instance, Constructor) {
                    if (!(instance instanceof Constructor)) {
                        throw new TypeError("Cannot call a class as a function");
                    }
                }

                var _createClass = function () {
                    function defineProperties(target, props) {
                        for (var i = 0; i < props.length; i++) {
                            var descriptor = props[i];
                            descriptor.enumerable = descriptor.enumerable || false;
                            descriptor.configurable = true;
                            if ("value" in descriptor) descriptor.writable = true;
                            Object.defineProperty(target, descriptor.key, descriptor);
                        }
                    }

                    return function (Constructor, protoProps, staticProps) {
                        if (protoProps) defineProperties(Constructor.prototype, protoProps);
                        if (staticProps) defineProperties(Constructor, staticProps);
                        return Constructor;
                    };
                }();

                function _possibleConstructorReturn(self, call) {
                    if (!self) {
                        throw new ReferenceError("this hasn't been initialised - super() hasn't been called");
                    }

                    return call && (typeof call === "object" || typeof call === "function") ? call : self;
                }

                function _inherits(subClass, superClass) {
                    if (typeof superClass !== "function" && superClass !== null) {
                        throw new TypeError("Super expression must either be null or a function, not " + typeof superClass);
                    }

                    subClass.prototype = Object.create(superClass && superClass.prototype, {
                        constructor: {
                            value: subClass,
                            enumerable: false,
                            writable: true,
                            configurable: true
                        }
                    });
                    if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass;
                }

                var Clipboard = function (_Emitter) {
                    _inherits(Clipboard, _Emitter);

                    /**
                     * @param {String|HTMLElement|HTMLCollection|NodeList} trigger
                     * @param {Object} options
                     */
                    function Clipboard(trigger, options) {
                        _classCallCheck(this, Clipboard);

                        var _this = _possibleConstructorReturn(this, (Clipboard.__proto__ || Object.getPrototypeOf(Clipboard)).call(this));

                        _this.resolveOptions(options);
                        _this.listenClick(trigger);
                        return _this;
                    }

                    /**
                     * Defines if attributes would be resolved using internal setter functions
                     * or custom functions that were passed in the constructor.
                     * @param {Object} options
                     */


                    _createClass(Clipboard, [{
                        key: 'resolveOptions',
                        value: function resolveOptions() {
                            var options = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : {};

                            this.action = typeof options.action === 'function' ? options.action : this.defaultAction;
                            this.target = typeof options.target === 'function' ? options.target : this.defaultTarget;
                            this.text = typeof options.text === 'function' ? options.text : this.defaultText;
                        }
                    }, {
                        key: 'listenClick',
                        value: function listenClick(trigger) {
                            var _this2 = this;

                            this.listener = (0, _goodListener2.default)(trigger, 'click', function (e) {
                                return _this2.onClick(e);
                            });
                        }
                    }, {
                        key: 'onClick',
                        value: function onClick(e) {
                            var trigger = e.delegateTarget || e.currentTarget;

                            if (this.clipboardAction) {
                                this.clipboardAction = null;
                            }

                            this.clipboardAction = new _clipboardAction2.default({
                                action: this.action(trigger),
                                target: this.target(trigger),
                                text: this.text(trigger),
                                trigger: trigger,
                                emitter: this
                            });
                        }
                    }, {
                        key: 'defaultAction',
                        value: function defaultAction(trigger) {
                            return getAttributeValue('action', trigger);
                        }
                    }, {
                        key: 'defaultTarget',
                        value: function defaultTarget(trigger) {
                            var selector = getAttributeValue('target', trigger);

                            if (selector) {
                                return document.querySelector(selector);
                            }
                        }
                    }, {
                        key: 'defaultText',
                        value: function defaultText(trigger) {
                            return getAttributeValue('text', trigger);
                        }
                    }, {
                        key: 'destroy',
                        value: function destroy() {
                            this.listener.destroy();

                            if (this.clipboardAction) {
                                this.clipboardAction.destroy();
                                this.clipboardAction = null;
                            }
                        }
                    }]);

                    return Clipboard;
                }(_tinyEmitter2.default);

                /**
                 * Helper function to retrieve attribute value.
                 * @param {String} suffix
                 * @param {Element} element
                 */
                function getAttributeValue(suffix, element) {
                    var attribute = 'data-clipboard-' + suffix;

                    if (!element.hasAttribute(attribute)) {
                        return;
                    }

                    return element.getAttribute(attribute);
                }

                module.exports = Clipboard;
            });

        }, { "./clipboard-action": 7, "good-listener": 4, "tiny-emitter": 6 }]
    }, {}, [8])(8)
});
"use strict";
var WinLossDetailCopyHtml = /** @class */ (function () {
    function WinLossDetailCopyHtml(clipboard) {
        this.clipboard = clipboard;
    }
    WinLossDetailCopyHtml.prototype.init = function (tableId) {
        var _this = this;
        if (tableId === void 0) { tableId = "tbl-report"; }
        $("#icon-copy").tooltip();
        var clipboard = new this.clipboard("#icon-copy", {
            text: function () { return _this._copyTableHtml($("#" + tableId)); }
        });
        if (window.document.queryCommandSupported && window.document.queryCommandSupported("copy")) {
            clipboard.on("success", function (e) {
                var $this = $(e.trigger);
                $this.tooltip({ content: $("#hiddenCopiedText").val() }).tooltip("open");
                setTimeout(function () {
                    $this.tooltip({ content: $("#hiddenCopyText").val() }).tooltip("close");
                }, 3000);
            }).on("error", function (e) {
                $(e.trigger).remove();
                console.error("Action:", e.action);
                console.error("Trigger:", e.trigger);
            });
        }
    };
    WinLossDetailCopyHtml.prototype._copyTableHtml = function (tableReport) {
        var html = ['<table>'];
        html.push(this._copyTableHeader(tableReport));
        html.push(this._copyTableBody(tableReport));
        html.push(this._copyTableFooter(tableReport));
        html.push('</table>');
        return html.join("");
    };
    WinLossDetailCopyHtml.prototype._copyTableHeader = function (tableReport) {
        var html = ['<thead><tr>'];
        tableReport.find("thead tr:first th:not(.hide)").each(function () {
            var $this = $(this);
            html.push('<th');
            var rowspan = $this.attr("rowspan");
            if (rowspan) {
                html.push(' rowspan="', rowspan, '"');
            }
            var colspan = $this.attr("colspan");
            if (colspan) {
                html.push(' colspan="', colspan, '"');
            }
            html.push('>', this.innerText, '</th>');
        });
        html.push('</tr>');
        if (tableReport.find("thead tr").length == 2) {
            html.push('<tr>');
            tableReport.find("thead tr:last th:not(.hide)").each(function () {
                html.push('<th>', this.innerText, '</th>');
            });
            html.push('</tr>');
        }
        html.push('</thead>');
        return html.join("");
    };
    WinLossDetailCopyHtml.prototype._copyTableBody = function (tableReport) {
        var html = ['<tbody>'];
        tableReport.find("tbody tr").each(function () {
            html.push('<tr>');
            $(this).find("td:not(.hide)").each(function () {
                var $this = $(this);
                html.push('<td');
                var rowspan = $this.attr("rowspan");
                if (rowspan) {
                    html.push(' rowspan="', rowspan, '"');
                }
                var colspan = $this.attr("colspan");
                if (colspan) {
                    html.push(' colspan="', colspan, '"');
                }
                html.push('>', this.innerText, '</td>');
            });
            html.push('</tr>');
        });
        html.push('</tbody>');
        return html.join("");
    };
    WinLossDetailCopyHtml.prototype._copyTableFooter = function (tableReport) {
        var html = ['<tfoot><tr>'];
        tableReport.find("tfoot tr td:not(.hide)").each(function () {
            html.push('<td>', this.innerText, '</td>');
        });
        html.push('</tr></tfoot>');
        return html.join("");
    };
    return WinLossDetailCopyHtml;
}());
//# sourceMappingURL=winlossdetail-copyhtml.js.map
var OandaExchange = /** @class */ (function () {
    function OandaExchange($, topWindow, dateRangePickerComponent, winLossDetailCopyHtml) {
        this.fromdate = moment(new Date()).format("MM/DD/YYYY");
        this.todate = moment(new Date()).format("MM/DD/YYYY");
        this.totalRows = 0;
        this.$ = $;
        this.topWindow = topWindow;
        this.dateRangePickerComponent = dateRangePickerComponent;
        this.winLossDetailCopyHtml = winLossDetailCopyHtml;
    }
    OandaExchange.prototype.Init = function () {
        var _this = this;
        this.dateRangePickerComponent.render(new DateRangePickerJQueryOption());
        this.$("#Submit").click(function () { return _this.CallSubmit(); });
        this.winLossDetailCopyHtml.init("tblContent_jqgrid");
        Fanex.ExtAcc.OnResize = this.ResizeReportGrid;
    };
    OandaExchange.prototype.BuildDefaultColumns = function () {
        return [
            { HeaderText: "Date", Name: "Date", Width: 130, HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Currency Name", Name: "Currency", Width: 130, HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Currency Code", Name: "CurrencyCode", Width: 130, HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "MYR/Unit", Name: "MYRUnit", Width: 130, HeaderAlign: "center", CellAlign: "right", FormatNegative: "$" },
            { HeaderText: "CNY/Unit", Name: "CNYUnit", Width: 130, HeaderAlign: "center", CellAlign: "right", FormatNegative: "$" },
            { HeaderText: "THB/Unit", Name: "THBUnit", Width: 130, HeaderAlign: "center", CellAlign: "right", FormatNegative: "$" },
            { HeaderText: "IDR/Unit", Name: "IDRUnit", Width: 130, HeaderAlign: "center", CellAlign: "right", FormatNegative: "$" }
        ];
    };
    OandaExchange.prototype.BuildBodyData = function (dataSource) {
        var bodyData = [];
        this.totalRows = dataSource.length;
        for (var i = 0; i < dataSource.length; i++) {
            var exchangeByDate = dataSource[i];
            bodyData.push({
                Columns: [
                    { Name: "Date", Value: exchangeByDate.ExchangeDateString, MergeRowNumber: exchangeByDate.Rates.length },
                    { Name: "Currency", Value: exchangeByDate.Rates[0].CurrencyName },
                    { Name: "CurrencyCode", Value: exchangeByDate.Rates[0].CurrencyCode },
                    { Name: "MYRUnit", Value: parseFloat(exchangeByDate.Rates[0].MYRRate) },
                    { Name: "CNYUnit", Value: parseFloat(exchangeByDate.Rates[0].CNYRate) },
                    { Name: "THBUnit", Value: parseFloat(exchangeByDate.Rates[0].THBRate) },
                    { Name: "IDRUnit", Value: parseFloat(exchangeByDate.Rates[0].IDRRate) }
                ]
            });
            for (var j = 1; j < exchangeByDate.Rates.length; j++) {
                var rate = exchangeByDate.Rates[j];
                bodyData.push({
                    Columns: [
                        { Name: "Currency", Value: rate.CurrencyName },
                        { Name: "CurrencyCode", Value: rate.CurrencyCode },
                        { Name: "MYRUnit", Value: parseFloat(rate.MYRRate) },
                        { Name: "CNYUnit", Value: parseFloat(rate.CNYRate) },
                        { Name: "THBUnit", Value: parseFloat(rate.THBRate) },
                        { Name: "IDRUnit", Value: parseFloat(rate.IDRRate) }
                    ]
                });
            }
        }
        return bodyData;
    };
    OandaExchange.prototype.Render = function (data) {
        var gridOption = {
            columns: this.BuildDefaultColumns(),
            bodyRows: this.BuildBodyData(data),
            fullHeight: !isMobile.phone,
            gridHeight: isMobile.phone ? $(window).height() - 70 : 0,
            gridExpandHeight: 256,
            autoCalculateGridExpandHeight: true,
            numOfPaddingForGridExpandHeight: 40,
            marginRight: 10,
            bottomBorderWidth: 0
        };
        this.$("#jqgrid").fanexGrid(gridOption);
        workaroundForGridOnPhone();
    };
    OandaExchange.prototype.CallSubmit = function () {
        var _this = this;
        var dateRange = this.dateRangePickerComponent.getDateRangeValues();
        this.fromdate = dateRange.fromDate;
        this.todate = dateRange.toDate;
        Fanex.ExtAcc.BlockUI();
        var parameterUrl = "?Groupname=" + $("#selectedGroupName").html() + "&GroupId=" + (Fanex.ExtAcc.Customer.GetGroupId() || '')
            + ("&Fromdate=" + this.fromdate + "&Todate=" + this.todate);
        this.$.ajax({
            url: resolveClientUrl("OandaExchange/GetExchangeRate") + parameterUrl,
            type: "post",
            dataType: "json",
            cache: false,
            success: function (data) { return _this.OnSuccess(data); }
        });
    };
    OandaExchange.prototype.OnSuccess = function (data) {
        if (data.errorCode) {
            Fanex.ExtAcc.UnBlockUI();
            Fanex.ExtAcc.Form.Alert("Error", data.errorMessage.formatErrorMessage(), 350, 150, "alertWLD");
            return;
        }
        var existedData = data !== null && data.length > 0;
        if (existedData) {
            this.$("#mainGrid").css("display", "block");
            this.$("#mainGrid").css("visibility", "visible");
            this.$("#msgNoinfo").css("display", "none");
            this.Render(data);
        }
        else {
            this.ShowNoData();
        }
        Fanex.ExtAcc.UnBlockUI();
    };
    OandaExchange.prototype.ShowNoData = function () {
        this.$("#mainGrid").css("display", "none");
        this.$("#mainGrid").css("visibility", "hidden");
        this.$("#msgNoinfo").css("display", "block");
    };
    OandaExchange.prototype.ResizeReportGrid = function () {
        try {
            $("#jqgrid").fanexGrid("resizeGrid");
        }
        catch (err) {
            console.log(err);
        }
    };
    return OandaExchange;
}());
$(document).ready(function () {
    var oandaExchange = new OandaExchange($, window.top, new DateRangePickerJQueryComponent(), new WinLossDetailCopyHtml(Clipboard));
    oandaExchange.Init();
});
//# sourceMappingURL=Fanex.ExtAcc.OandaExchange.js.map