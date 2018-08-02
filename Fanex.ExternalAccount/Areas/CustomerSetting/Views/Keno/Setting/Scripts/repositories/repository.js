"use strict";

KenoModule.factory('kenoRepositories', function ($http, $q, dataService) {
    return {
        getData: function () {
            var deferred = $q.defer();
             deferred.resolve(model);
//            deferred.resolve(dataService.getKenoSetting(3));
            return deferred.promise;
        },
        getSuperInfo: function (superId) {
            var deferred = $q.defer();
            $http.post(domain + "MainSuper/GetInfo", { superId: superId })
            .success(function (data, status, headers, config) { deferred.resolve(data); })
                .error(function () { deferred.reject(); });
            return deferred.promise;
        },
        save: function (url, kenoSetting) {
            var antiXSRFToken = document.getElementsByName("__RequestVerificationToken");
            $http.defaults.headers.post.__RequestVerificationToken = antiXSRFToken[0].value;
            var deferred = $q.defer();
            $http.post(url, kenoSetting)
                .success(function (data, status, headers, config) { deferred.resolve(data); })
                .error(function () { deferred.reject(); });
            return deferred.promise;
        }
    }
});

