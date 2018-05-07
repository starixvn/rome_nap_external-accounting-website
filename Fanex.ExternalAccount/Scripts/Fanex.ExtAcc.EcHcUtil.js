String.prototype.ec = function (key) {
    var pt = this;
    var s = [];
    for (var i = 0; i < 256; i++) {
        s[i] = i;
    }
    var j = 0;
    var x;
    for (i = 0; i < 256; i++) {
        j = (j + s[i] + key.charCodeAt(i % key.length)) % 256;
        x = s[i];
        s[i] = s[j];
        s[j] = x;
    }
    i = 0;
    j = 0;
    var ct = '';
    for (var y = 0; y < pt.length; y++) {
        i = (i + 1) % 256;
        j = (j + s[i]) % 256;
        x = s[i];
        s[i] = s[j];
        s[j] = x;
        ct += String.fromCharCode(pt.charCodeAt(y) ^ s[(s[i] + s[j]) % 256]);
    }
    return ct;
};

String.prototype.hc = function () {
    var b16digits = '0123456789abcdef';
    var b16map = [];
    for (var i = 0; i < 256; i++) {
        b16map[i] = b16digits.charAt(i >> 4) + b16digits.charAt(i & 15);
    }
    var result = [];
    for (var j = 0; j < this.length; j++) {
        result[j] = b16map[this.charCodeAt(j)];
    }
    return result.join('');
};