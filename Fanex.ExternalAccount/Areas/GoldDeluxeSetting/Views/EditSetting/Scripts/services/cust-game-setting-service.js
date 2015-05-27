if (fanex.goldDeluxeSetting) {
    fanex.goldDeluxeSetting.service('CustomerGameSettingService', ['$http', function ($http) {
        'use strict';
        var $this = this;

        $this.updateGameSetting = function (gameSetting, settingContextScope) {
            settingViewModel.Setting.GameSetting = gameSetting;

            $http({
                method: 'POST',
                url: $('#UpdateGameSettingUrl').val(),
                headers: { 'Content-Type': 'application/json' },
                data: settingViewModel
            }).success(function (mainResult) {
                settingContextScope.afterServerReturnSuccess(mainResult, true);
            }).
            error(function (data, status) {
                if (status === fanex.goldDeluxeSetting.UN_AUTHORIZED_CODE) {
                    fanex.goldDeluxeSetting.errorResult.CommandResultsList[0].Results[0].Message = language.getString('UnauthorizeMsg');
                }
                else {
                    fanex.goldDeluxeSetting.errorResult.CommandResultsList[0].Results[0].Message = language.getString('lblUpdateFailed');
                }
                settingContextScope.mainResult = fanex.goldDeluxeSetting.errorResult;
            });
        };
    }]);
}