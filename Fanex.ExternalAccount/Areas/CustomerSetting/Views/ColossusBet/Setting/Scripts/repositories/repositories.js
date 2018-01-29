"use strict";

ColossusBetModule.factory('colossusBetRepositories', function ($http, $q) {
    return {
        getData: function () {
            var deferred = $q.defer();
            deferred.resolve(colossusBetSetting);
            return deferred.promise;
        },
        save: function (url, colossusBetSetting) {
            var antiXSRFToken = document.getElementsByName("__RequestVerificationToken");
            $http.defaults.headers.post.__RequestVerificationToken = antiXSRFToken[0].value;
            var deferred = $q.defer();
            $http.post(url, colossusBetSetting)
                .success(function (data, status, headers, config) { deferred.resolve(data); })
                .error(function () { deferred.reject(); });
            return deferred.promise;
        }
    }
});

