/* File Created: July 21, 2014 */

var fanex = fanex || {}; // jshint ignore:line

//// Core function
fanex.cloneObject = function (obj) {
    'use strict';
    var newObj = (obj instanceof Array) ? [] : {};
    for (var i in obj) {
        if (i === 'clone') {
            continue;
        }
        if (obj[i] && typeof obj[i] === "object") {
            newObj[i] = fanex.cloneObject(obj[i]);
        } else {
            newObj[i] = obj[i];
        }
    }
    return newObj;
};


fanex.roundNumber = function (number, decimals) {
    'use strict';
    decimals = decimals || 2;
    var newnumber = Number(number + '').toFixed(parseInt(decimals));
    return parseFloat(newnumber); // Output the result to the form field (change for your purposes)
};

fanex.createOptionsFromRange = function (current, from, to, gap, decimals) {
    'use strict';
    var options = [];
    decimals = decimals || 2;
    to = fanex.roundNumber(to, decimals);

    if (current < from) {
        options.push({
            id: current,
            value: current
        });
    }

    for (var i = from; i <= to; i = fanex.roundNumber(i + gap, decimals)) {
        options.push({
            id: i,
            value: i
        });
    }

    if (current > to) {
        options.push({
            id: current,
            value: current
        });
    }

    return options.reverse();
};



//// live casino setting app

fanex.goldDeluxeSetting = angular.module('fanex.goldDeluxeSetting', ['ngSanitize']);

fanex.goldDeluxeSetting.config(['$httpProvider', function ($httpProvider) {
    'use strict';
    $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';
} ]);

fanex.goldDeluxeSetting.formType = {
    Add: 0,
    Edit: 1,
    ReadOnly: 2
};

fanex.goldDeluxeSetting.customerLevel = {
    Super: 4,
    Master: 3,
    Agent: 2,
    Member: 1
};

fanex.goldDeluxeSetting.updateType = {
    AllSetting: 4,
    WinLimit: 3,
    GameSetting: 2,
    PositionTakingAndCommission: 1
};
//// Constant
fanex.goldDeluxeSetting.UN_AUTHORIZED_CODE = 401;
fanex.goldDeluxeSetting.PT_INTERVAL = 0.01;
fanex.goldDeluxeSetting.COM_INTERVAL = 0.0005;
fanex.goldDeluxeSetting.COM_DECIMAL = 4;
fanex.goldDeluxeSetting.PT_DECIMAL = 2;
fanex.goldDeluxeSetting.TIME_OUT_SHOW_MESSAGE = 4000;
fanex.goldDeluxeSetting.templateVersion = '?v=' + Math.random();
fanex.goldDeluxeSetting.buildTemplateUrlWithVersion = function (templateUrl) {
    'use strict';
    return fanex.goldDeluxeSetting.rootUrl + templateUrl + fanex.goldDeluxeSetting.templateVersion;
};
fanex.goldDeluxeSetting.errorResult = {
    CommandResultsList: [
        {
            Results: [
                {
                    IsSuccessful: false,
                    Message: ''
                }
            ]
        }
    ],
    IsSuccessful: false,
    isUnAuthorized: true
};


fanex.goldDeluxeSetting.rootUrl = document.getElementById('LiveCasinoRoot') ? document.getElementById('LiveCasinoRoot').value : '/';


fanex.goldDeluxeSetting.config([ '$httpProvider', function ($httpProvider) {
    'use strict';
    $httpProvider.responseInterceptors.push('loadingInterceptor');

    var spinnerFunction = function spinnerFunction(data) {
        try { $.blockUI(); } catch (e) { }
        return data;
    };

    $httpProvider.defaults.transformRequest.push(spinnerFunction);
    var antiXSRFToken = document.getElementsByName("__RequestVerificationToken");
    if (antiXSRFToken.length > 0) {
        $httpProvider.defaults.headers.post.__RequestVerificationToken = antiXSRFToken[0].value
    }
}]);

fanex.goldDeluxeSetting.factory('loadingInterceptor', [ '$q', function ($q) {
    'use strict';
    return function (promise) {
        return promise.then(function (response) {
            try { $.unblockUI(); } catch (e) { }
            return response;
        }, function (response) {
            try { $.unblockUI(); } catch (e) { }
            return $q.reject(response);
        });
    };
}]);

