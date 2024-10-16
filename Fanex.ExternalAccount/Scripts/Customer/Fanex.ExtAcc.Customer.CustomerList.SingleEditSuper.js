Fanex.ExtAcc.Customer.CustomerList.SingleEditSuper = function (custId, title) {
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
                data: { custId: custId, groupName: $('#selectedGroupName').html() },
                dataType: 'json',
                async: false,
                success: function (result) {
                    Fanex.ExtAcc.Customer.TransferCondition = false;
                    Fanex.ExtAcc.Customer.UserId = custId;
                    Fanex.ExtAcc.Customer.TitleEditInfo = unescape(title);
                    Fanex.ExtAcc.Customer.CurrentLevel = Fanex.ExtAcc.Customer.Super;
                    Fanex.ExtAcc.Customer.ResetFlags();
                    top.shouldCalculatePopupHeight = true;
                    items = {
                    };

                    function addEditProductGroupSuperContextMenuItem(productGroupId, key, name) {
                        items[key] = {
                            name: name,
                            className: "iconViewEdit",
                            callback: function () {
                                Fanex.ExtAcc.Customer.IsFullInfo = true;
                                Fanex.ExtAcc.Form.ShowFrame(
                                    resolveClientUrl('Customer/EditCustomer' + '/?roleId=4&uplineRoleId=5&groupName=' + $('#selectedGroupName').html() + "&productGroupId=" + productGroupId + '&custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit ' + name + ' Setting ' + '<font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>',
                                    1250, Fanex.ExtAcc.Customer.GetPopupHeight(270, 480), false, false, 1);
                            }
                        };
                    }

                    function addEditProductSuperContextMenuItem(productId, productKey, productName) {
                        items[productKey] = {
                            name: productName,
                            className: "iconViewEdit",
                            callback: function () {
                                Fanex.ExtAcc.Customer.IsFullInfo = true;
                                Fanex.ExtAcc.Customer.ShowEditProductForSingleCust(
                                    Fanex.ExtAcc.Customer.UserId,
                                    5,
                                    4,
                                    productId,
                                    productName);
                            }
                        };
                    }

                    for (var i = 0, max = result.ProductGroups.length; i < max; i++) {
                        var productGroup = result.ProductGroups[i];
                        addEditProductGroupSuperContextMenuItem(productGroup.Id, 'ProductGroup-' + productGroup.Id, productGroup.Name);
                    }

                    items.separator4 = "-----";

                    items.editInfo = {
                        name: "Edit Info",
                        height: 25,
                        className: "iconEnableEdit editInfoBg",
                        callback: function (key, opt) {
                            var height = isMobile.phone ? Fanex.ExtAcc.Customer.GetPopupHeight(270, 480) : 500;
                            Fanex.ExtAcc.Form.Show(
                                resolveClientUrl('MainSuper/EditInfo/?custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), titles[0] + ': <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>',
                                500,
                                height,
                                false);
                        }
                    };

                    items.changePassword = {
                        name: "Reset Password",
                        className: "iconEnableEdit editInfoBg",
                        callback: function (key, opt) {
                            var height = isMobile.phone ? Fanex.ExtAcc.Customer.GetPopupHeight(270, 400) : 400;
                            Fanex.ExtAcc.Form.Show(
                                resolveClientUrl('Customer/ResetPassword/?custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), 'Reset password: <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>',
                                415,
                                height,
                                false,
                                false);
                        }
                    };

                    var self = this;
                    items.securityCode = {
                        name: "Security Code",
                        className: "iconEnableEdit editInfoBg",
                        callback: function (key, opt) {
                            var params = {
                                custId: Fanex.ExtAcc.Customer.UserId,
                                roleId: 4,
                                username: Fanex.ExtAcc.Customer.TitleEditInfo,
                                groupName: Fanex.ExtAcc.Customer.GroupName().groupName
                            };
                            var height = isMobile.phone ? Fanex.ExtAcc.Customer.GetPopupHeight(270, 480) : 500;
                            Fanex.ExtAcc.Form.ShowFrame(
                                resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/GetSecurityCode?' + $.param(params)),
                                'Reset security code: <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>',
                                650,
                                height,
                                false,
                                false);
                        }
                    };

                    items.transferCondition = {
                        name: "Transfer Condition",
                        className: "iconEnableEdit editInfoBg",
                        callback: function (key, opt) {
                            top.shouldCalculatePopupHeight = false;
                            Fanex.ExtAcc.Form.Show(
                                resolveClientUrl('Customer/EditTransferCondition/?roleId=4&custId=' + Fanex.ExtAcc.Customer.UserId + '&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName), titles[1] + ': <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>',
                                600,
                                300,
                                false,
                                false,
                                1);
                        }
                    };

                    if (showBetSetting == '1') {
                        items.betSetting = {
                            name: "Edit Betting Limit",
                            className: "iconEnableEdit editInfoBg",
                            callback: function (key, opt) {
                                Fanex.ExtAcc.Form.ShowFrame(
                                    resolveClientUrl('CustomerSetting/BetSetting/Sportsbook/Index?custId=' + Fanex.ExtAcc.Customer.UserId + "&roleId=" + Fanex.ExtAcc.Customer.CurrentLevel + '&uplineId=' + Fanex.ExtAcc.Customer.UserId + '&userId=' + userId + '&groupName=' + $('#selectedGroupName').html()), 'Edit Bet Setting',
                                    1250,
                                    Fanex.ExtAcc.Customer.GetPopupHeight(270, 480),
                                    false,
                                    null, 1);
                            }
                        };
                    }

                    items.customerDataRange = {
                        name: "Edit Data Range",
                        className: "iconEnableEdit editInfoBg",
                        callback: function (key, opt) {
                            top.shouldCalculatePopupHeight = false;
                            var url = resolveClientUrl('DataRange/Index?custIds=' + Fanex.ExtAcc.Customer.UserId);
                            Fanex.ExtAcc.Form.Show(url, 'Edit Data Range For <font color="#6dcff6">' + Fanex.ExtAcc.Customer.TitleEditInfo + '</font>', 400, 170, false, null, 1);
                        }
                    };
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