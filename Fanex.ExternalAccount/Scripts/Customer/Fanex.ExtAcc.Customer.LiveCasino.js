/// <reference path="../_references.js" />

if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!");
};

Fanex.ExtAcc.Customer.LiveCasino = {
    IsMultiple: Fanex.ExtAcc.Customer.IsMultiple,

    Init: function () {
        if (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Master) {
            this.GetMasterSetting();
        }
        else if (Fanex.ExtAcc.Customer.CurrentLevel == Fanex.ExtAcc.Customer.Agent) {
            this.GetAgentSetting();
        }
        else {
            if (Fanex.ExtAcc.Customer.isSuperEditMember == true) {
                this.SuperGetMemberSetting();
            }
            else if (Fanex.ExtAcc.Customer.isMasterEditMember == true) {
                this.MasterGetMemberSetting();
            }
            else {
                this.GetMemberSetting();
            }
        }
    },

    GetMasterSetting: function () {
        Fanex.ExtAcc.BlockUI();

        var data = new Object();
        data.custId = Fanex.ExtAcc.Customer.UserId;
        data.uplineId = Fanex.ExtAcc.Customer.superId;

        $.ajax({
            url: resolveClientUrl('LiveCasino/EditMasterPost'),
            dataType: 'json',
            type: 'post',
            data: data
        })
        .success(function(data){
            if (typeof console != 'undefined') console.log(data);
            $.unblockUI(document);
        })
        .error(function(data){
            if (typeof console != 'undefined') console.log(data.responseText);
            $.unblockUI(document);
        });
    },

    GetAgentSetting: function () {
    },

    GetMemberSetting: function () {
    },

    SuperGetMemberSetting: function () {
    },

    MasterGetMemberSetting: function () {
    },

    UpdateMasterSetting: function () {
    },

    UpdateAgentSetting: function () {
    },

    UpdateMemberSetting: function () {
    },

    SuperUpdateMemberSetting: function () {
    },

    MasterUpdateMemberSetting: function () {
    },
};

$(document).ready(function(){
    Fanex.ExtAcc.Customer.LiveCasino.Init();
});