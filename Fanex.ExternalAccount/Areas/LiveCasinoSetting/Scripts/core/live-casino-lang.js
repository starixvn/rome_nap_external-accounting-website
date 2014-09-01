/*global currentLaguageKey*/
var language = language || {};

language.defaultCode = 'en-US';
language.code = currentLaguageKey || language.defaultCode;

language.setLanguageCode = function (languageCode) {
    'use strict';
    language.code = languageCode;
};

language.getString = function (key) {
    'use strict';
    var value = language.liveCasino[language.code][key];
    if (!value) {
        value = language.liveCasino[language.defaultCode][key];
        if (!value) {
            value = key;
        }
    }

    return value;
};