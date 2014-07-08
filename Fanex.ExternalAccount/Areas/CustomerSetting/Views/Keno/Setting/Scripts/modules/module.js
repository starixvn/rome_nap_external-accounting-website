"use strict";

var KenoModule = angular.module('kenoSettingModule', ['ngDialog']);

KenoModule.run(function ($templateCache) {
    $templateCache.put('betSettingTemplate', domain + 'Areas/CustomerSetting/Views/Keno/Setting/Views/Partial/BetSetting/BetSetting.htm');
    $templateCache.put('superPositionTakingTemplate', domain + 'Areas/CustomerSetting/Views/Keno/Setting/Views/Partial/PositionTaking/SuperPositionTaking.htm');
    $templateCache.put('masterPositionTakingTemplate', domain + 'Areas/CustomerSetting/Views/Keno/Setting/Views/Partial/PositionTaking/MasterPositionTaking.htm');
    $templateCache.put('agentPositionTakingTemplate', domain + 'Areas/CustomerSetting/Views/Keno/Setting/Views/Partial/PositionTaking/AgentPositionTaking.htm');
    $templateCache.put('memberPositionTakingTemplate', domain + 'Areas/CustomerSetting/Views/Keno/Setting/Views/Partial/PositionTaking/MemberPositionTaking.htm');
    $templateCache.put('superOnMemberPositionTakingTemplate', domain + 'Areas/CustomerSetting/Views/Keno/Setting/Views/Partial/PositionTaking/SuperOnMemberPositionTaking.htm');
    $templateCache.put('masterOnMemberPositionTakingTemplate', domain + 'Areas/CustomerSetting/Views/Keno/Setting/Views/Partial/PositionTaking/MasterOnMemberPositionTaking.htm');
    $templateCache.put('confirmUpdateAllMember', domain + 'Areas/CustomerSetting/Views/Keno/Setting/Views/Partial/PositionTaking/ConfirmUpdateAllMembers.htm');
});

