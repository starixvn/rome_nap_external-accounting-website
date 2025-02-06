Fanex.ExtAcc.Customer.EditMultiple = function (div) {
    this.ResetFlags();
    var menuItems = {};

    this.isSuperEditMember = false;
    this.isMasterEditMember = false;
    this.IsFullInfo = false;
    this.IsMultiple = true;
    var ret = this.IdsAndNames();
    var info = this.CustomerInfo();
    var productIds = [];

    if (this.CurrentLevel == this.Super) {
        menuItems.doubleComm = {
            name: "Edit Multiple Double Comm",
            className: "iconEnableEdit",
            callback: function (key, opt) {
                top.shouldCalculatePopupHeight = false;
                Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/ConfigMultipleDoubleComm?id=' + Fanex.ExtAcc.Customer.CurrentLevel + '&custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName),
                    'Edit Multiple Double Commission For Losing Tickets',
                    430, 150, false, false, 1);
            }
        };

        if (!isBelongToSpecialGroups) {
            menuItems.productStatusSetting = {
                name: "Edit Multiple Product Status",
                className: "iconEnableEdit",
                callback: function (key, opt) {
                    Fanex.ExtAcc.Customer.ShowEditMultipleProductStatus(productIds);
                }
            };
        }

        menuItems.dataRange = {
            name: "Edit Multiple Data Range",
            className: "iconEnableEdit",
            callback: function (key, opt) {
                top.shouldCalculatePopupHeight = false;
                var url = resolveClientUrl('DataRange/Index?custIds=' + Fanex.ExtAcc.Customer.IdsAndNames().Ids);
                Fanex.ExtAcc.Form.Show(url, 'Edit Multiple Data Range For Customer(s)', 400, 170, false, false, 1);
            }
        };
    }
    else if (this.CurrentLevel == this.Master) {
        menuItems = {
        };

        function addSuperEditMultipleMastersMenuItem(productId, productName) {
            var productKey = 'product_super_master_' + productId;
            menuItems[productKey] = {
                name: "Edit Multiple " + productName,
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.ShowEditProductForMultipleCusts(4, 3, productId, productName);
                }
            };
            productIds.push(productId);
        }

        function addSuperEditMultipleGroupMastersMenuItem(groupId, groupName) {
            var key = 'product_group_super_master_' + groupId;
            menuItems[key] = {
                name: "Edit Multiple " + groupName,
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.ShowEditMultipleProductGroup(4, groupId, groupName);
                }
            };
            productIds.push(groupId);
        }

        for (var i = 0, max = this.ProductGroups.length; i < max; i++) {
            var productGroup = this.ProductGroups[i];

            addSuperEditMultipleGroupMastersMenuItem(productGroup.Id, productGroup.Name);
        }

        for (var i = 0, max = this.UplineProductSettings.length; i < max; i++) {
            var productSetting = this.UplineProductSettings[i];

            addSuperEditMultipleMastersMenuItem(productSetting.productId, productSetting.productName);
        }

        if (this.AllowDoubleComm) {
            menuItems.doubleComm = {
                name: "Edit Multiple Double Comm",
                className: "iconEnableEdit",
                callback: function (key, opt) {
                    top.shouldCalculatePopupHeight = false;
                    Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/ConfigMultipleDoubleComm?id=' + Fanex.ExtAcc.Customer.CurrentLevel + '&custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName),
                        'Edit Multiple Double Commission For Losing Tickets',
                        430, 150, false, false, 1);
                }
            };
        }
    }
    else if (this.CurrentLevel == this.Agent) {
        menuItems = {
        };

        function addMasterEditMultipleAgentsMenuItem(productId, productName) {
            var productKey = 'product_master_agent_' + productId;
            menuItems[productKey] = {
                name: "Edit Multiple " + productName,
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.ShowEditProductForMultipleCusts(3, 2, productId, productName);
                }
            };
            productIds.push(productId);
        }

        function addMasterEditMultipleGroupAgentsMenuItem(groupId, groupName) {
            var key = 'product_group_master_agent_' + groupId;
            menuItems[key] = {
                name: "Edit Multiple " + groupName,
                className: "iconEnableEdit",
                callback: function () {
                    Fanex.ExtAcc.Customer.ShowEditMultipleProductGroup(3, groupId, groupName);
                }
            };
            productIds.push(groupId);
        }

        for (var i = 0, max = this.ProductGroups.length; i < max; i++) {
            var productGroup = this.ProductGroups[i];

            addMasterEditMultipleGroupAgentsMenuItem(productGroup.Id, productGroup.Name);
        }

        for (var i = 0, max = this.UplineProductSettings.length; i < max; i++) {
            var productSetting = this.UplineProductSettings[i];

            addMasterEditMultipleAgentsMenuItem(productSetting.productId, productSetting.productName);
        }

        if (this.AllowDoubleComm) {
            menuItems.doubleComm = {
                name: "Edit Multiple Double Comm",
                className: "iconEnableEdit",
                callback: function (key, opt) {
                    top.shouldCalculatePopupHeight = false;
                    Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/ConfigMultipleDoubleComm?id=' + Fanex.ExtAcc.Customer.CurrentLevel + '&custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName),
                        'Edit Multiple Double Commission For Losing Tickets',
                        430, 150, false, false, 1);
                }
            };
        }
    }
    else {
        menuItems = {
        };

        function addUplineEditMemberMenuItems(productId, productName) {
            var productKey = 'product_' + productId;
            menuItems[productKey] = {
                name: productName,
                className: "iconEnableEdit",
                items: {}
            };

            menuItems[productKey].items['agent_' + productKey] = {
                name: 'Edit Multiple ' + productName,
                callback: function () {
                    Fanex.ExtAcc.Customer.isSuperEditMember = false;
                    Fanex.ExtAcc.Customer.isMasterEditMember = false;
                    Fanex.ExtAcc.Customer.ShowEditProductForMultipleCusts(2, 1, productId, productName);
                }
            };

            menuItems[productKey].items['super_' + productKey] = {
                name: 'Super Edit Multiple ' + productName,
                callback: function () {
                    Fanex.ExtAcc.Customer.isSuperEditMember = true;
                    Fanex.ExtAcc.Customer.isMasterEditMember = false;
                    Fanex.ExtAcc.Customer.ShowEditProductForMultipleCusts(4, 1, productId, productName);
                }
            };

            menuItems[productKey].items['master_' + productKey] = {
                name: 'Master Edit Multiple ' + productName,
                callback: function () {
                    Fanex.ExtAcc.Customer.isSuperEditMember = false;
                    Fanex.ExtAcc.Customer.isMasterEditMember = true;
                    Fanex.ExtAcc.Customer.ShowEditProductForMultipleCusts(3, 1, productId, productName);
                }
            };

            productIds.push(productId);
        }

        function addUplineEditGroupMemberMenuItems(groupId, groupName) {
            var key = 'product_group_' + groupId;
            menuItems[key] = {
                name: groupName,
                className: "iconEnableEdit",
                items: {}
            };

            menuItems[key].items['agent_' + key] = {
                name: 'Edit Multiple ' + groupName,
                callback: function () {
                    Fanex.ExtAcc.Customer.isSuperEditMember = false;
                    Fanex.ExtAcc.Customer.isMasterEditMember = false;
                    Fanex.ExtAcc.Customer.ShowEditMultipleProductGroup(2, groupId, groupName);
                }
            };

            menuItems[key].items['super_' + key] = {
                name: 'Super Edit Multiple ' + groupName,
                callback: function () {
                    Fanex.ExtAcc.Customer.isSuperEditMember = true;
                    Fanex.ExtAcc.Customer.isMasterEditMember = false;
                    Fanex.ExtAcc.Customer.ShowSuperMasterEditMultipleGroup(4, groupId, groupName);
                }
            };

            menuItems[key].items['master_' + key] = {
                name: 'Master Edit Multiple ' + groupName,
                callback: function () {
                    Fanex.ExtAcc.Customer.isSuperEditMember = false;
                    Fanex.ExtAcc.Customer.isMasterEditMember = true;
                    Fanex.ExtAcc.Customer.ShowSuperMasterEditMultipleGroup(3, groupId, groupName);
                }
            };

            productIds.push(groupId);
        }

        for (var i = 0, max = this.ProductGroups.length; i < max; i++) {
            var productGroup = this.ProductGroups[i];

            addUplineEditGroupMemberMenuItems(productGroup.Id, productGroup.Name);
        }

        for (var i = 0, max = this.UplineProductSettings.length; i < max; i++) {
            var productSetting = this.UplineProductSettings[i];

            addUplineEditMemberMenuItems(productSetting.productId, productSetting.productName);
        }

        if (this.AllowDoubleComm) {
            menuItems.doubleComm = {
                name: "Edit Multiple Double Comm",
                className: "iconEnableEdit",
                callback: function (key, opt) {
                    top.shouldCalculatePopupHeight = false;
                    Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/ConfigMultipleDoubleComm?id=' + Fanex.ExtAcc.Customer.CurrentLevel + '&custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName),
                        'Edit Multiple Double Commission For Losing Tickets',
                        430, 150, false, false, 1);
                }
            };
        }
    }

    if (!isBelongToSpecialGroups && productIds.length !== 0) {
        menuItems.productStatusSetting = {
            name: "Edit Multiple Product Status",
            className: "iconEnableEdit",
            callback: function (key, opt) {
                Fanex.ExtAcc.Customer.ShowEditMultipleProductStatus(productIds);
            }
        }
    }

    menuItems.suspend = {
        name: "Edit Multiple Suspended Status",
        className: "iconEnableEdit",
        callback: function (key, opt) {
            top.shouldCalculatePopupHeight = false;
            var url = resolveClientUrl('Customer/MultipleSuspend?custIdList=' + Fanex.ExtAcc.Customer.IdsAndNames().Ids + '&roleid=' + Fanex.ExtAcc.Customer.CurrentLevel);
            Fanex.ExtAcc.Customer.Status.ViewSuspendMultiple(Fanex.ExtAcc.Customer.IdsAndNames().Ids, Fanex.ExtAcc.Customer.CurrentLevel);
        }
    }

    menuItems.close = {
        name: "Edit Multiple Closed Status",
        className: "iconEnableEdit",
        callback: function (key, opt) {
            top.shouldCalculatePopupHeight = false;

            Fanex.ExtAcc.Customer.ClosedStatus.ShowMultipleOpenCustomerPopup(
                Fanex.ExtAcc.Customer.IdsAndNames().Ids,
                Fanex.ExtAcc.Customer.CurrentLevel);
        }
    };

    if (this.CurrentLevel !== this.Member && hasReadProtectCustomerFunction) {
        menuItems.protectedStatus = {
            name: "Edit Multiple Protection Status",
            className: "iconEnableEdit",
            callback: function (key, opt) {
                top.shouldCalculatePopupHeight = false;
                Fanex.ExtAcc.Customer.ShowEditMultipleProtectedStatus(Fanex.ExtAcc.Customer.GroupName().groupName, Fanex.ExtAcc.Customer.IdsAndNames().Ids, Fanex.ExtAcc.Customer.CurrentLevel);
            }
        };
    }

    if ($.contextMenu) $.contextMenu('destroy');

    $.contextMenu({
        selector: '.editMult',
        items: menuItems,
        trigger: "left",
        ignoreRightClick: true,
        autoHide: true
    });
};

Fanex.ExtAcc.Customer.UpdateMultipleDoubleComm = function (allow) {
    var ret = this.IdsAndNames();

    this.ConfigMultipleDoubleComm(ret.Ids, allow, 0);
};

Fanex.ExtAcc.Customer.ShowEditMultipleProductGroup = function (uplineRoleId, productGroupId, productGroupName) {
    var ret = this.IdsAndNames();
    top.shouldCalculatePopupHeight = true;
    var isDirectMember = Fanex.ExtAcc.Customer.CustomerList.cachedParams.isDirectMember;
    var roleId = isDirectMember ? Fanex.ExtAcc.Customer.Member : Fanex.ExtAcc.Customer.CurrentLevel;
    var agentId = isDirectMember ? (uplineRoleId == Fanex.ExtAcc.Customer.Super ? -4 : -3) : 0;
    if (!this.IsMultiple) {
        Fanex.ExtAcc.Form.ShowFrame(
            resolveClientUrl('Customer/EditCustomer?roleId=' + roleId + '&agentId=' + agentId + '&uplineRoleId=' + uplineRoleId + '&custId=' + Fanex.ExtAcc.Customer.UserId + '&productGroupId=' + productGroupId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), 'Edit ' + productGroupName + ' Setting ' + '<font color="#6dcff6">' + ret.Names[0] + '</font>',
            1250,
            Fanex.ExtAcc.Customer.GetPopupHeight(270, 480),
            false, false, 1);
    }
    else {
        var Usernames = '';
        var CustIds = '';
        var userNamesArr = [];
        var custIdsArr = [];

        if (Fanex.ExtAcc.Customer.CustIds.length > 0 && Fanex.ExtAcc.Customer.CustIds.indexOf(',') != -1)
            custIdsArr = Fanex.ExtAcc.Customer.CustIds.substring(0, Fanex.ExtAcc.Customer.CustIds.length - 1).split(',');

        if (Fanex.ExtAcc.Customer.Usernames.length > 0 && Fanex.ExtAcc.Customer.Usernames.indexOf(',') != -1)
            userNamesArr = Fanex.ExtAcc.Customer.Usernames.substring(0, Fanex.ExtAcc.Customer.Usernames.length - 2).split(', ');

        var customerlength = userNamesArr.length;
        for (var i = 0; i < customerlength; i++) {
            var linkChar = i == customerlength - 1 ? "" : ", ";
            Usernames += userNamesArr[i] + linkChar;
            CustIds += custIdsArr[i] + linkChar;
        }
        Fanex.ExtAcc.Form.ShowFrame(
            resolveClientUrl('Customer/EditMultiple?roleId=' + roleId + '&agentId=' + agentId + '&uplineRoleId=' + uplineRoleId + '&custIds=' + CustIds + '&productGroupId=' + productGroupId + '&usernames=' + Usernames + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), 'Edit Multiple ' + productGroupName + ' Setting',
            1250,
            Fanex.ExtAcc.Customer.GetPopupHeight(270, 480),
            false, false, 1);
    }
};

Fanex.ExtAcc.Customer.ShowEditProductForSingleCust = function (custid, uplineRoleId, targetRoleId, productId, productName) {
    document.getElementById('language').value = "en-US";
    document.getElementById('productId').value = productId;
    document.getElementById('customerLevel').value = targetRoleId;
    document.getElementById('customerId').value = custid;
    document.getElementById('isMultiple').value = false;
    document.getElementById('uplineLevel').value = "";
    top.shouldCalculatePopupHeight = true;

    if (this.CurrentLevel == this.Super) {
        document.getElementById('uplineLevel').value = "";
        document.getElementById('IsReadOnly').value = false;
    }
    else if (this.CurrentLevel == this.Master || this.CurrentLevel == this.Agent) {
        document.getElementById('uplineLevel').value = "";
        document.getElementById('IsReadOnly').value = false;
    }
    else {
        document.getElementById('uplineLevel').value = uplineRoleId;
        document.getElementById('IsReadOnly').value = false;
    }

    Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('DCS/ViewCustomerSetting'), 'Edit ' + productName + ' Setting', 900, Fanex.ExtAcc.Customer.GetPopupHeight(270, 480), false, false, 1);
    var targetForm = $("#targetForm")
        .attr("method", "get")
        .attr("action", resolveClientUrl('DCS/ViewCustomerSetting'))
        .submit();
    // Reset form method in order to it does NOT affect other Products
    targetForm.attr("method", "post");
};

Fanex.ExtAcc.Customer.ShowEditProductForMultipleCusts = function (uplineRoleId, targetRoleId, productId, productName) {
    var levelText = this.isSuperEditMember ? 'Super ' : this.isMasterEditMember ? 'Master ' : '';

    if (this.IsMultiple) {
        var Usernames = '';
        var CustIds = '';
        var userNamesArr = [];
        var custIdsArr = [];

        if (Fanex.ExtAcc.Customer.CustIds.length > 0 && Fanex.ExtAcc.Customer.CustIds.indexOf(',') != -1)
            custIdsArr = Fanex.ExtAcc.Customer.CustIds.substring(0, Fanex.ExtAcc.Customer.CustIds.length - 1).split(',');

        if (Fanex.ExtAcc.Customer.Usernames.length > 0 && Fanex.ExtAcc.Customer.Usernames.indexOf(',') != -1)
            userNamesArr = Fanex.ExtAcc.Customer.Usernames.substring(0, Fanex.ExtAcc.Customer.Usernames.length - 2).split(', ');

        var customerlength = userNamesArr.length;
        for (var i = 0; i < customerlength; i++) {
            var linkChar = i == customerlength - 1 ? "" : ", ";
            Usernames += userNamesArr[i] + linkChar;
            CustIds += custIdsArr[i] + linkChar;
        }

        document.getElementById('language').value = "en-US";
        document.getElementById('productId').value = productId;
        document.getElementById('customerLevel').value = targetRoleId;
        document.getElementById('isMultiple').value = true;
        document.getElementById('customerIds').value = CustIds;
        document.getElementById('customerNames').value = Usernames;
        document.getElementById('uplineLevel').value = "";
        top.shouldCalculatePopupHeight = true;

        if (document.getElementById('customerIds').value == "") {
            Fanex.ExtAcc.Form.ShowFrame(
                resolveClientUrl('Views/_blank.html'), levelText + 'Edit Multiple ' + productName + ' Setting',
                900,
                Fanex.ExtAcc.Customer.GetPopupHeight(270, 480),
                false, false, 1);
            return;
        }

        if (this.CurrentLevel == this.Master || this.CurrentLevel == this.Agent || this.CurrentLevel == this.Super) {
            document.getElementById('uplineLevel').value = "";
        }
        else {
            document.getElementById('uplineLevel').value = uplineRoleId;
        }

        Fanex.ExtAcc.Form.ShowFrame(
            resolveClientUrl('DCS/ViewCustomerSetting'), levelText + 'Edit Multiple ' + productName + ' Setting',
            900,
            Fanex.ExtAcc.Customer.GetPopupHeight(270, 480),
            false, false, 1);
        var targetForm = $("#targetForm")
            .attr("method", "get")
            .attr("action", resolveClientUrl('DCS/ViewCustomerSetting'))
            .submit();
        // Reset form method in order to it does NOT affect other Products
        targetForm.attr("method", "post");
    }
    else {
        Fanex.ExtAcc.Customer.ShowEditProductForSingleCust(Fanex.ExtAcc.Customer.UserId, uplineRoleId, targetRoleId, productId, productName);
    }
};

Fanex.ExtAcc.Customer.ShowSuperMasterEditMultipleGroup = function (uplineRoleId, productGroupId, productGroupName) {
    var ret = this.IdsAndNames();
    var levelText = '';

    if (this.isSuperEditMember) {
        levelText = 'Super ';
    } else if (this.isMasterEditMember) {
        levelText = 'Master ';
    }

    if (!this.IsMultiple) {
        Fanex.ExtAcc.Form.ShowFrame(
            resolveClientUrl('Customer/EditCustomer?roleId=' + Fanex.ExtAcc.Customer.CurrentLevel + '&uplineRoleId=' + uplineRoleId + '&productGroupId=' + productGroupId + '&custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), levelText + 'Edit ' + productGroupName + ' Setting ' + '<font color="#6dcff6">' + ret.Names[0] + '</font>',
            1250,
            Fanex.ExtAcc.Customer.GetPopupHeight(270, 480),
            false, false, 1);
    } else {
        var Usernames = '';
        var CustIds = '';
        var userNamesArr = [];
        var custIdsArr = [];

        if (Fanex.ExtAcc.Customer.CustIds.length > 0 && Fanex.ExtAcc.Customer.CustIds.indexOf(',') != -1)
            custIdsArr = Fanex.ExtAcc.Customer.CustIds.substring(0, Fanex.ExtAcc.Customer.CustIds.length - 1).split(',');

        if (Fanex.ExtAcc.Customer.Usernames.length > 0 && Fanex.ExtAcc.Customer.Usernames.indexOf(',') != -1)
            userNamesArr = Fanex.ExtAcc.Customer.Usernames.substring(0, Fanex.ExtAcc.Customer.Usernames.length - 2).split(', ');

        var customerlength = userNamesArr.length;
        for (var i = 0; i < customerlength; i++) {
            var linkChar = i == customerlength - 1 ? "" : ", ";
            Usernames += userNamesArr[i] + linkChar;
            CustIds += custIdsArr[i] + linkChar;
        }

        Fanex.ExtAcc.Form.ShowFrame(
            resolveClientUrl('Customer/EditMultiple?roleId=' + Fanex.ExtAcc.Customer.CurrentLevel + '&uplineRoleId=' + uplineRoleId + '&custIds=' + CustIds + '&productGroupId=' + productGroupId + '&usernames=' + Usernames + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), levelText + 'Edit Multiple ' + productGroupName + ' Setting',
            1250,
            Fanex.ExtAcc.Customer.GetPopupHeight(270, 480),
            false, false, 1);
    }
};

Fanex.ExtAcc.Customer.ShowEditMultipleCasino = function () {
    var ret = this.IdsAndNames();
    var data = this.CustomerInfo();
    var levelText = '';
    var actionName = 'CustomerSetting/Casino/';

    switch (this.CurrentLevel) {
        case this.Master:
            actionName += 'Master/EditMaster?id=' + this.Ids()[0] + '&uplineid=' + this.superId + '&groupname=' + Fanex.ExtAcc.Customer.GroupName().groupName;
            break;
        case this.Agent:
            actionName += 'Agent/EditAgent?id=' + this.Ids()[0] + '&uplineid=' + this.masterId + '&groupname=' + Fanex.ExtAcc.Customer.GroupName().groupName;
            break;
        case this.Member:
            if (this.isSuperEditMember)
                actionName += 'Member/EditMemberBySuper?id=' + this.Ids()[0] + '&uplineid=' + this.superId + '&groupname=' + Fanex.ExtAcc.Customer.GroupName().groupName;
            else if (this.isMasterEditMember)
                actionName += 'Member/EditMemberByMaster?id=' + this.Ids()[0] + '&uplineid=' + this.masterId + '&groupname=' + Fanex.ExtAcc.Customer.GroupName().groupName;
            else
                actionName += 'Member/EditMember?id=' + this.Ids()[0] + '&uplineid=' + this.agentId + '&groupname=' + Fanex.ExtAcc.Customer.GroupName().groupName;
            break;
        default:
            break;
    }

    if (this.isSuperEditMember) {
        levelText = 'Super ';
    } else if (this.isMasterEditMember) {
        levelText = 'Master ';
    }
    top.shouldCalculatePopupHeight = true;
    if (!this.IsMultiple) {
        Fanex.ExtAcc.Form.ShowFrame(
            resolveClientUrl(actionName), levelText + 'Edit Casino Setting ' + '<font color="#6dcff6">' + ret.Names[0] + '</font>',
            1150,
            Fanex.ExtAcc.Customer.GetPopupHeight(270, 480),
            false, false, 1, null);
    } else {
        window.IsMultiple = true;
        window.Ids = this.Ids();
        window.Names = this.Names();
        Fanex.ExtAcc.Form.ShowFrame(
            resolveClientUrl(actionName), levelText + 'Edit Multiple Casino Setting',
            1150,
            Fanex.ExtAcc.Customer.GetPopupHeight(270, 480),
            false, false, 1, null);
    }
};

Fanex.ExtAcc.Customer.ConfigMultipleDoubleComm = function (custids, allow, index) {
    Fanex.ExtAcc.BlockUI();

    if (index == 0) {
        this.MultipleDblCommError = '';
    }

    var len = custids.length - 1;
    var current = index;

    $.ajax({
        url: resolveClientUrl('Customer/ConfigDoubleComm'),
        type: 'post',
        data: {
            custid: parseInt(custids[current]),
            allow: allow,
            groupName: Fanex.ExtAcc.Customer.GroupName().groupName
        }
    })
        .success(function (result) {
            if (Fanex.ExtAcc.ShowSecurityCodePage(result, Fanex.ExtAcc.UnBlockUI)) {
                return;
            }

            if (result.errorCode && result.errorCode == '1') {
                Fanex.ExtAcc.UnBlockUI();
                Fanex.ExtAcc.Form.Alert('Error', result.errorMessage, 400, 150, 'alert');
                Fanex.ExtAcc.Form.Close();
            } else {
                if (!result.isSuccess) {
                    Fanex.ExtAcc.Customer.MultipleDblCommError += (result.errorCode == '600' ? ('Account ' + Fanex.ExtAcc.Customer.Names()[current] + ' was disableed<br />') : (result.errorMessage + '<br/>'));
                }

                if (current == len) {
                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);
                        Fanex.ExtAcc.Form.Close();
                        Fanex.ExtAcc.UnBlockUI();

                        if (Fanex.ExtAcc.Customer.MultipleDblCommError != '') {
                            Fanex.ExtAcc.Form.Alert('Error', Fanex.ExtAcc.Customer.MultipleDblCommError.formatErrorMessage(), 300, 150, 'alert');
                        }

                        Fanex.ExtAcc.Customer.CallBackCustomerList();
                    }, 2000);
                }

                current++;

                if (current <= len) {
                    Fanex.ExtAcc.Customer.ConfigMultipleDoubleComm(custids, allow, current);
                }
            }
        })
        .error(function (xhr, status, err) {
            if (typeof console != 'undefined') {
                console.log(err);
            }

            Fanex.ExtAcc.Customer.MultipleDblCommError += err + '<br/>';

            current++;

            if (current <= len) {
                Fanex.ExtAcc.Customer.ConfigMultipleDoubleComm(custids, allow, current);
            }

            Fanex.ExtAcc.UnBlockUI();
        });
};

Fanex.ExtAcc.Customer.EditDCS_CheckUnSyncedProducts = function (customerIds, productId) {
    var customerIdRow, unsyncedProductsAttribute;
    var customerIdsArray = customerIds.split(",");

    for (var i = 0, max = customerIdsArray.length; i < max; i++) {
        customerIdRow = document.getElementById(customerIdsArray[i].replace(/[\s,]+/g, ""));

        if (customerIdRow) {
            unsyncedProductsAttribute = customerIdRow.attributes.getNamedItem("data-unsyncedproducts")

            if (unsyncedProductsAttribute && Fanex.ExtAcc.Customer.EditMultipleDCS_ItemInString(productId, unsyncedProductsAttribute.value)) {
                return true;
            }
        }
    }

    return false;
};

Fanex.ExtAcc.Customer.EditMultipleDCS_ItemInString = function (item, stringValue) {
    var stringArray = stringValue.split(",");

    for (var i = 0, max = stringArray.length; i < max; i++) {
        if (item === parseInt(stringArray[i])) {
            return true;
        }
    }

    return false;
};

Fanex.ExtAcc.Customer.SetIsForcingModeHiddenField = function (value) {
    var isForcingModeElement = document.getElementById("isForcingMode");

    if (isForcingModeElement) {
        isForcingModeElement.value = value;
    }
};

window.successfulUpdatingCallback = function (isPartialUpdate, isMultipleUpdate) {
    var isForcingModeElement = document.getElementById("isForcingMode");

    if (isForcingModeElement && isForcingModeElement.value === "true") {
        parent.Fanex.ExtAcc.Customer.Search(false);

        isForcingModeElement.value = "false";
        $("#targetForm").attr("action", resolveClientUrl("DCS/ViewCustomerSetting")).submit();
    }
};

Fanex.ExtAcc.Customer.ShowEditMultipleProductStatus = function (productIds) {
    var blankUrl = resolveClientUrl('Views/_blank.html'),
        title = 'Edit Multiple Product Status',
        width = 860,
        height = 540,
        custIds = this.Ids().join(',');

    if (custIds.len === 0) {
        Fanex.ExtAcc.Form.ShowFrame(blankUrl, title, width, height, false, false, 1);
    } else {
        document.getElementById('productIds').value = productIds.join(',');
        document.getElementById('custIds').value = custIds;
        document.getElementById('custNames').value = this.Names().join(', ');
        document.getElementById('custRole').value = this.CurrentLevel;
        document.getElementById('groupId').value = Fanex.ExtAcc.Customer.GetGroupId();
        Fanex.ExtAcc.Form.ShowFrame(blankUrl, title, width, height, false, false, 1);
        $('#targetProductStatusSettingForm').attr('action', resolveClientUrl('ProductStatusSetting/MultipleView')).submit();
    }
};

Fanex.ExtAcc.Customer.ShowEditMultipleProtectedStatus = function (groupName, custIds, roleId) {
    var $targetForm = $('#targetProtectedStatusForm');
    $targetForm.find('#protectedstatus-groupName').val(groupName);
    $targetForm.find('#protectedstatus-custIds').val(custIds);
    $targetForm.find('#protectedstatus-roleId').val(roleId);
    Fanex.ExtAcc.Form.ShowFrame(
        resolveClientUrl('Views/_blank.html'),
        'Edit Multiple Protection Status',
        500,
        540,
        false,
        null,
        1,
        null,
        null,
        true);
    $targetForm.attr('action', resolveClientUrl('ProtectedStatus/OpenMultiple')).submit();
};