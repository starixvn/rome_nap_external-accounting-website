Fanex.ExtAcc.Customer.CustomerList.SingleEditAgent = function (custId, title) {
    if ($.contextMenu) $.contextMenu('destroy');
    $.contextMenu({
        selector: '.editInfo',
        trigger: "left",
        ignoreRightClick: true,
        autoHide: true,
        build: function ($triggerElement, e) {
            var items;
            $.ajax({
                type: 'POST',
                url: resolveClientUrl('CustomerList/GetCustomerStatus'),
                data: { custId: custId, groupName: Fanex.ExtAcc.Customer.GroupName().groupName },
                dataType: 'json',
                async: false,
                success: function (result) {
                    Fanex.ExtAcc.Customer.TransferCondition = result.DisableTransfer;
                    Fanex.ExtAcc.Customer.UserId = custId;
                    Fanex.ExtAcc.Customer.TitleEditInfo = unescape(title);
                    Fanex.ExtAcc.Customer.ResetFlags();
                    items = {
                    };

                    function addEditProductGroupAgentContextMenuItem(id, key, name) {
                        items[key] = {
                            name: name,
                            className: "iconEnableEdit",
                            callback: function () {
                                Fanex.ExtAcc.Customer.IsFullInfo = true;
                                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('Customer/EditCustomer?roleId=2&uplineRoleId=3&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName + "&productGroupId=" + id + '&custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit ' + name + ' Setting ' + '<font color="#6dcff6">' + title + '</font>', 1250, Fanex.ExtAcc.Customer.GetPopupHeight(270, 480), false, false, 1);
                            }
                        };
                    }

                    function addEditProductAgentContextMenuItem(productId, productKey, productName) {
                        items[productKey] = {
                            name: productName,
                            className: "iconEnableEdit",
                            callback: function () {
                                Fanex.ExtAcc.Customer.IsFullInfo = true;
                                Fanex.ExtAcc.Customer.ShowEditProductForSingleCust(
                                    custId,
                                    3,
                                    2,
                                    productId,
                                    productName);
                            }
                        };
                    }

                    for (var i = 0, max = result.ProductGroups.length; i < max; i++) {
                        var productGroup = result.ProductGroups[i];
                        addEditProductGroupAgentContextMenuItem(
                            productGroup.Id,
                            'ProductGroup-' + productGroup.Id,
                            productGroup.Name);
                    }

                    var has3rdParty = false;                  

                    if (has3rdParty)
                        items.separator2 = "-----";
                    items.editInfo = {
                        name: "Edit Info",
                        className: "iconEnableEdit editInfoBg",
                        callback: function (key, opt) {
                            var height = isMobile.phone ? Fanex.ExtAcc.Customer.GetPopupHeight(270, 480) : 500;
                            Fanex.ExtAcc.Form.Show(
                                resolveClientUrl('Customer/EditInfo/?custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), titles[0] + ': <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>',
                                500,
                                height,
                                false);
                        }
                    };
                    items.changePassword = {
                        name: "Reset Password",
                        className: "iconEnableEdit editInfoBg",
                        callback: function (key, opt) {
                            var height = isMobile.phone ? Fanex.ExtAcc.Customer.GetPopupHeight(270, 480) : 400;
                            Fanex.ExtAcc.Form.Show(
                                resolveClientUrl('Customer/ResetPassword/?custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), 'Reset password: <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>',
                                415,
                                height,
                                false, false);
                        }
                    };

                    if (result.HasSecurityCode || result.UplineHasSecurityCode) {
                        var self = this;
                        items.securityCode = {
                            name: "Security Code",
                            className: "iconEnableEdit editInfoBg",
                            callback: function (key, opt) {
                                var params = {
                                    custId: custId,
                                    roleId: 2,
                                    username: Fanex.ExtAcc.Customer.TitleEditInfo,
                                    groupName: Fanex.ExtAcc.Customer.GroupName().groupName
                                };
                                var height = isMobile.phone ? Fanex.ExtAcc.Customer.GetPopupHeight(270, 480) : 500;
                                Fanex.ExtAcc.Form.ShowFrame(
                                    resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/GetSecurityCode?' + $.param(params)),
                                    'Reset security code: <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>',
                                    650,
                                    height,
                                    false, false);
                            }
                        };
                    }

                    if (!result.DisableTransfer) {
                        items.transferCondition = {
                            name: "Transfer Condition",
                            className: "iconEnableEdit editInfoBg",
                            callback: function (key, opt) {
                                Fanex.ExtAcc.Form.Show(
                                    resolveClientUrl('Customer/EditTransferCondition/?roleId=2&custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), titles[1] + ': <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>',
                                    600,
                                    300,
                                    false, false,
                                    1);
                            }
                        };
                    }
                }
            });

            return {
                ignoreRightClick: true,
                autoHide: true,
                items: items
            }
        }
    });
};