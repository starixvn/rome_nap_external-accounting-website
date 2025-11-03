String.prototype.encrypt = function (secretKey) {
    var plainText = this;
    plainText = plainText.toString();
    var passPhrase = 'a5s8d2e9c172';
    var key = CryptoJS.enc.Utf8.parse(passPhrase + secretKey);
    var iv = key;

    var pwd = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(plainText), key,
    {
        keySize: 128 / 8,
        iv: iv,
        mode: CryptoJS.mode.CBC,
        padding: CryptoJS.pad.Pkcs7
    });

    return pwd.toString();
}