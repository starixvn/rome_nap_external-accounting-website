/* File Created: March 28, 2013 */

var helper = {
    checkDecNumber: function (e) {
        var code = "";
        if (e.which)
            code = e.which;
        else if (e.keyCode)
            code = e.keyCode;

        if (code == 44 || code == 37 || code == 39 || code == 36 || code == 35 || code == 118)
            return true;
               

        if ((code < 48 || code > 57) && code != 8 && code != 13 && code != 46 && code != 0 && code != 9) {
            return false;
        }

        return true;
    }
};