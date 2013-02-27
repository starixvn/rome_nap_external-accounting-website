/* File Created: May 10, 2012 */

// Validators library
var Validators = {
    add: function (elementInput, expr, message) {
        // Bind onchange event of input
        // Validate with expr
        // Show message and animate background color
    },
    show: function (elementInput, message, xPos, pointerXPos, timeout) {
        // <span class="hint"><span>This is the message.<span><span class="hint-pointer">&nbsp;</span></span>
        var hideTimeout = timeout ? timeout : 1000;
        var hint = document.getElementById('fHint');
        var hintPointer = null;
        if (hint == null) {
            var hint = document.createElement("SPAN");
            hint.className = 'hint';
            var hintMsg = document.createElement("SPAN");
            hint.appendChild(hintMsg);
            hintPointer = document.createElement("SPAN");
            hintPointer.className = 'hint-pointer';
            hint.appendChild(hintPointer);
            hint.id = 'fHint';
            hint.style.top = '10px';
            hint.style.left = '10px';
            hint.style.zIndex = 999999;
            hint.onclick = function () { // Hide onclick
                hint.style.display = 'none';
            };
            document.body.appendChild(hint);
        };
        hintPointer = hint.lastChild;
        hint.firstChild.innerHTML = message;
        if (this.timeOutHideTooltip) {
            clearTimeout(this.timeOutHideTooltip);
            this.timeOutHideTooltip = null;
        };
        this.timeOutHideTooltip = setTimeout(function () {
            hint.style.display = 'none';
        }, hideTimeout); // Auto hide in 1s
        if (elementInput != null) {
            if (typeof xPos == 'undefined') {
                xPos = 0;
            }
            if (typeof pointerXPos == 'undefined') {
                pointerXPos = 10;
            }

            hint.style.backgroundPositionX = (pointerXPos + 'px');
            hintPointer.style.left = (pointerXPos + 'px');

            var pos = Validators.utils.findPosRelativeToViewport(elementInput);
            hint.style.left = (pos[0] + xPos) + 'px';
            hint.style.top = (pos[1] - 40) + 'px';
            elementInput.onchange = function () {
                hint.style.display = 'none';
            };
            elementInput.focus();
        };
        hint.style.display = 'block';
    },
    showDown: function (elementInput, message, xPos, pointerXPos, yPos, pointerYPos, height, timeout) {
        // <span class="hint"><span>This is the message.<span><span class="hint-pointer">&nbsp;</span></span>
        var hint = document.getElementById('fHintDown');
        var hintPointer = null;
        if (hint == null) {
            var hint = document.createElement("SPAN");
            hint.className = 'hint';
            var hintMsg = document.createElement("SPAN");
            hint.appendChild(hintMsg);
            hintPointer = document.createElement("SPAN");
            hintPointer.className = 'hint-pointer-down';
            hint.appendChild(hintPointer);
            hint.id = 'fHintDown';
            hint.style.top = '10px';
            hint.style.left = '10px';
            hint.style.zIndex = 999999;
            hint.onclick = function () { // Hide onclick
                hint.style.display = 'none';
            };
            document.body.appendChild(hint);
        };
        hintPointer = hint.lastChild;
        hint.firstChild.innerHTML = message;
        if (this.timeOutHideTooltip) {
            clearTimeout(this.timeOutHideTooltip);
            this.timeOutHideTooltip = null;
        };
        if (typeof (timeout) == 'undefined') {
            timeout = 1000;
        }
        this.timeOutHideTooltip = setTimeout(function () {
            if (hint == null) {
                return;
            }
            hint.style.display = 'none';
        }, timeout); // Auto hide in 1s
        if (elementInput != null) {
            if (typeof xPos == 'undefined') {
                xPos = 0;
            }
            if (typeof pointerXPos == 'undefined') {
                pointerXPos = 10;
            }
            if (typeof yPos == 'undefined') {
                yPos = -45;
            }
            if (typeof (pointerYPos) == 'undefined') {
                pointerYPos = 38;
            }
            if (typeof (height) == 'undefined') {
                height = 18;
            }
            hint.style.height = height + 'px';
            hintPointer.style.left = (pointerXPos + 'px');
            hintPointer.style.top = (pointerYPos + 'px');
            var pos = Validators.utils.findPosRelativeToViewport(elementInput);
            hint.style.left = (pos[0] + xPos) + 'px';
            hint.style.top = (pos[1] + yPos) + 'px';
            elementInput.onchange = function () {
                hint.style.display = 'none';
            };
            elementInput.focus();
        };
        hint.style.display = 'block';
    },
    hide: function () {
        if (document.getElementById('fHint')) {
            var hint = document.getElementById('fHint');
            hint.style.display = 'none';
        }
    },
    animate: function (input) {
        var previousColor = input.style.color;
        var previousBackground = input.style.backgroundColor;
        var c = 4;
        if (this.intervalAnimate) {
            clearInterval(this.intervalAnimate);
            this.intervalAnimate = null;
        }
        var int = this.intervalAnimate = setInterval(function () {
            c--;
            if (input.style.backgroundColor == previousBackground) {
                input.style.backgroundColor = '#ff7700';
                input.style.color = '#ffffff';
            } else {
                input.style.backgroundColor = previousBackground;
                input.style.color = previousColor;
            }
            if (c == 0) {
                clearInterval(int);
            }
        }, 300);
    },

    animateText: function (input, text) {
        var previousColor = input.style.color;
        var previousBackground = input.style.backgroundColor;
        var previousText = input.value;
        var preType = input.type;
        var c = 6;
        if (this.intervalAnimate) {
            clearInterval(this.intervalAnimate);
            this.intervalAnimate = null;
        }
        var int = this.intervalAnimate = setInterval(function () {
            c--;
            if (input.style.backgroundColor == previousBackground) {
                input.style.backgroundColor = '#fff7b2';
                input.style.color = '#fd1718';
                if (preType == 'password')
                    input.type = 'text';
                input.value = text;
            } else {
                input.type = preType;
                input.style.backgroundColor = previousBackground;
                input.style.color = previousColor;
                input.value = previousText;
            }
            if (c == 0) {
                clearInterval(int);
                input.focus();
            }
        }, 500);
    },

    utils: {
        // findPos() by quirksmode.org
        // Finds the absolute position of an element on a page
        findPos: function (obj) {
            var curleft = curtop = 0;
            if (obj.offsetParent) {
                do {
                    curleft += obj.offsetLeft;
                    curtop += obj.offsetTop;
                } while (obj = obj.offsetParent);
            }

            if (document.getElementById('windowPopup')) {
                var parent = document.getElementById('windowPopup');

                curleft -= parent.scrollLeft;
                curtop -= parent.scrollTop;
            }

            return [curleft, curtop];
        },
        // getPageScroll() by quirksmode.org
        // Finds the scroll position of a page
        getPageScroll: function () {
            var xScroll, yScroll;
            if (self.pageYOffset) {
                yScroll = self.pageYOffset;
                xScroll = self.pageXOffset;
            } else if (document.documentElement && document.documentElement.scrollTop) {
                yScroll = document.documentElement.scrollTop;
                xScroll = document.documentElement.scrollLeft;
            } else if (document.body) { // all other Explorers
                yScroll = document.body.scrollTop;
                xScroll = document.body.scrollLeft;
            }
            return [xScroll, yScroll]
        },
        // Finds the position of an element relative to the viewport.
        findPosRelativeToViewport: function (obj) {
            var objPos = this.findPos(obj)
            var scroll = this.getPageScroll()
            return [objPos[0] - scroll[0], objPos[1] - scroll[1]]
        }
    },

    validateNumber: function (value) {
        if (value && value != "") {
            value = value.replace(/^\s\s*/, '').replace(/\s\s*$/, '');
            var intRegex = /^\d+$/;

            if (!intRegex.test(value)) {
                return false;
            }

            return true;
        } else {
            return false;
        }
    },

    validateInput: function (value) {
        if (value && value != "") {
            value = value.replace(/^\s\s*/, '').replace(/\s\s*$/, '');

            if (value != "")
                return true;

            return false;
        }

        return false;
    },

    validatePassword: function (str) {
        var s = str.toLowerCase();
        var numbers = "0123456789";

        var i = 0;
        var nCount = 0;

        while (i < s.length) {
            (numbers.indexOf(s.substring(i, i + 1)) >= 0) ? nCount++ : false;
            i++;
        }

        var validNumber = ((nCount > (s.length - 2)) || nCount < 2 || s.length < 6) ? false : true;

        var letters = "abcdefghijklmnopqrstuvwxyz";

        i = 0;
        nCount = 0;

        while (i < s.length) {
            (letters.indexOf(s.substring(i, i + 1)) >= 0) ? nCount++ : false;
            i++;
        }

        var validLetter = ((nCount > (s.length - 2)) || nCount < 2 || s.length < 6) ? false : true;

        return validNumber & validLetter;
    },

    validateName: function (str) {
        var reg = /^[A-z0-9 ]+$/;

        if (!reg.test(str)) {
            return false;
        }

        return true;
    },

    validateKey: function (str) {
        var reg = /^[A-z0-9]+$/;

        if (!reg.test(str)) {
            return false;
        }

        return true;
    }
};