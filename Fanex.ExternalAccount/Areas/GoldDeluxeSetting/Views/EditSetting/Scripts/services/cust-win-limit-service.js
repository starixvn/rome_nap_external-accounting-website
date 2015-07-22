if (fanex.goldDeluxeSetting) {
    fanex.goldDeluxeSetting.service('CustomerWinLimitService', ['$http', function ($http) {
        'use strict';
        var $this = this;

        $this.getCurrentMemberWinLimit = function (custId) {
            return $http({
                method: 'GET',
                url: $('#memberWinLimitLink').val() + '?custId=' + custId,
                headers: { 'Content-Type': 'application/json' }
            });
        };

        $this.updateWinLimitSetting = function (winLimitModel, settingContextScope) {
            
            settingViewModel.Setting.WinLimit = winLimitModel;

            $http({
                method: 'POST',
                url: $('#UpdateWinLimitUrl').val(),
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
    } ]);
}