/// <reference path="../_references.js" />
if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
}

Fanex.ExtAcc.UserPolicy = {
    OldUserPolicy: null,

    Init: function () {
        this.OldUserPolicy = this.GetUserPolicy();
    },

    CheckPermission: function (isCheck, target) {
        if (isCheck == true) {
            $(target).removeClass('permInactive');
            $(target).addClass('permActive');
        }
        else {
            $(target).removeClass('permActive');
            $(target).addClass('permInactive');
        }
    },

    GrantPermissionsRead: function (target) {
        var targetId = $(target).attr('id');
        var isCheck = $(target).hasClass('permInactive');
        var parentId = $(target).attr('parent');

        var table = document.getElementById('tblInner');
        var divs = table.getElementsByTagName("div");

        this.CheckPermission(isCheck, target);
        $('#' + targetId.replace('r-', 'w-')).attr('val', '1');
        if (!isCheck)
            $('#' + targetId.replace('r-', 'w-')).attr('val', '0');

        for (var i = 0; i < divs.length; i++) {
            if ($(divs[i]).attr("parent") == targetId || (!isCheck && $(divs[i]).attr("parent") == targetId.replace('r-', 'w-') && $(divs[i]).attr("permission") == 2)) {
                this.CheckPermission(false, $(divs[i]).get(0));
                var id = divs[i].id.replace('r-', 'w-');
                $('#' + id).attr('val', '0');
            }

            if ($(divs[i]).attr("parent") == targetId && $(divs[i]).attr("permission") == 1) {
                this.CheckPermission(isCheck, $(divs[i]).get(0));
                var id = divs[i].id.replace('r-', 'w-');
                $('#' + id).attr('val', '1');
                if (!isCheck)
                    $('#' + id).attr('val', '0');
            }
        }
    },

    GrantPermissionsWrite: function (target) {
        var targetId = $(target).attr('id');
        var isCheck = $(target).hasClass('permInactive');

        this.CheckPermission(isCheck, target);

        $('#' + targetId).attr('val', '2');

        if (!isCheck)
            $('#' + targetId).attr('val', '0');

        var table = document.getElementById('tblInner');
        var divs = table.getElementsByTagName("div");

        for (var i = 0; i < divs.length; i++) {
            if (isCheck) {
                if (($(divs[i]).attr("parent") == targetId.replace('w-', 'r-') || $(divs[i]).attr("parent") == targetId.replace('r-', 'w-')) && ($(divs[i]).attr("permission") == 2 || $(divs[i]).attr("permission") == 1)) {
                    this.CheckPermission(isCheck, $(divs[i]).get(0));
                    $(divs[i]).attr('val', '2');
                    if (!isCheck)
                        $(divs[i]).attr('val', '0');
                }
            }
            else {
                if ($(divs[i]).attr("parent") == targetId || (!isCheck && $(divs[i]).attr("parent") == targetId.replace('r-', 'w-') && $(divs[i]).attr("permission") == 2)) {
                    this.CheckPermission(false, $(divs[i]).get(0));
                    var id = divs[i].id.replace('w-', 'r-');
                    if ($('#' + id.replace('w-', 'r-')).attr('class') == 'permActive') {
                        $(divs[i]).attr('val', '1');
                    }
                    else {
                        $(divs[i]).attr('val', '0');
                    }
                }
            }
        }
    },

    GrantPermissions: function (target) {
        var permission = $(target).attr('permission');
        var parentId = $(target).attr('parent');
        var targetId = $(target).attr('id');
        var isCheck = $(target).hasClass('permInactive');
        var allUnChecked = true;

        if (permission == 1) // read permission
        {
            this.CheckPermission(isCheck, target);
            $('#' + targetId.replace('r-', 'w-')).attr('val', '1');

            if (!isCheck) {
                this.CheckPermission(false, $('#' + targetId.replace('r-', 'w-')).get(0));
                $('#' + targetId.replace('r-', 'w-')).attr('val', '0');

                var divs = document.getElementById('tblInner').getElementsByTagName("div");

                for (var i = 0; i < divs.length; i++) {
                    if ($(divs[i]).attr("id") == parentId) {
                        continue;
                    }

                    if ($(divs[i]).attr("parent") == parentId) {
                        if ($(divs[i]).hasClass("permInactive")) {
                            allUnChecked = true;
                        }
                        else {
                            allUnChecked = false;
                            break;
                        }
                    }
                }

                if (allUnChecked) {
                    this.CheckPermission(false, $('#' + parentId)); //Uncheck parent node of read permission
                    this.CheckPermission(false, $('#' + parentId.replace('r-', 'w-'))); //Uncheck parent node of write permission

                    $('#' + parentId.replace('r-', 'w-')).attr('val', '0');
                    $('#' + parentId.replace('w-', 'r-')).attr('val', '0');
                }
            }
            else {
                this.CheckPermission(true, $('#' + parentId));
                $('#' + parentId.replace('r-', 'w-')).attr('val', '1');
            }
        }
        else { //Write permission
            this.CheckPermission(isCheck, target);

            if (!isCheck) { //Uncheck write permission
                this.CheckPermission(!isCheck, $('#' + targetId.replace('w-', 'r-')).get(0));

                if ($('#' + targetId.replace('w-', 'r-')).attr('class') == 'permActive') {
                    $('#' + targetId.replace('r-', 'w-')).attr('val', '1');
                }

                var divs = document.getElementById('tblInner').getElementsByTagName("div");

                for (var i = 0; i < divs.length; i++) {
                    if ($(divs[i]).attr("id") == parentId) {
                        continue;
                    }

                    if ($(divs[i]).attr("parent") == parentId) {
                        if ($(divs[i]).hasClass("permActive")) {
                            allUnChecked = false;
                            break;
                        }
                    }
                }

                if (allUnChecked) {
                    this.CheckPermission(false, $('#' + parentId)); //Uncheck parent node of write permission

                    if ($('#' + parentId.replace('w-', 'r-')).attr('class') == 'permActive') {
                        $('#' + parentId.replace('r-', 'w-')).attr('val', '1');
                    }
                    else {
                        $('#' + parentId.replace('r-', 'w-')).attr('val', '0'); //Set parent of Write column to None permission
                    }
                }
            }
            else { //Check write permission
                this.CheckPermission(true, target); //Check write permission
                this.CheckPermission(true, $('#' + targetId.replace('w-', 'r-')).get(0)); //Check read permission
                this.CheckPermission(true, $('#' + parentId)); //Check parent node of write permission
                this.CheckPermission(true, $('#' + parentId.replace('w-', 'r-')).get(0)); //Check parent node of read permission

                //Set write permission for both child and parent of Read & Write columns to Write permission
                $('#' + targetId.replace('r-', 'w-')).attr('val', '2');
                $('#' + targetId.replace('w-', 'r-')).attr('val', '2');
                $('#' + parentId.replace('r-', 'w-')).attr('val', '2');
                $('#' + parentId.replace('w-', 'r-')).attr('val', '2');
            }
        }
    },

    GetUserPolicy: function () {
        var divs = document.getElementById('tblInner').getElementsByTagName("div");
        var userPolicy = '';
        for (var i = 0; i < divs.length; i++) {
            if ($(divs[i]).attr("permission") == 2) {
                if ($(divs[i]).attr("id")) {
                    var val = $(divs[i]).attr("val");
                    var id = $(divs[i]).attr("id").replace('r-', '').replace('w-', '');
                    userPolicy += id + ',' + val + ';'
                }
            }
        }
        return userPolicy.substring(0, userPolicy.length - 1);
    },

    SaveUserPolicy: function () {
        Fanex.ExtAcc.BlockUI();
        var userPolicy = this.GetUserPolicy();

        var url = "User/GrantPermissionPost";
        var data = {
            userId: $('#UserID').val(),
            policy: userPolicy,
            oldPolicy: this.OldUserPolicy
        };

        $.ajax({
            url: resolveClientUrl(url),
            type: 'POST',
            data: data,
            dataType: 'html',
            cache: false,
            success: function (result) {
                var json = $.parseJSON(result);
                if (!json.isSuccess) {
                    $('#errMsg').css('display', 'block');
                    $('#errMsg').html(json.errorMessage.formatErrorMessage());
                    Fanex.ExtAcc.UnBlockUI();
                }
                else {
                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);
                        Fanex.ExtAcc.Form.Close(false);
                        Fanex.ExtAcc.UnBlockUI();
                    }, 2000);
                }
            }
        });
    }
}

$(document).ready(function () {
    Fanex.ExtAcc.UserPolicy.Init();
});