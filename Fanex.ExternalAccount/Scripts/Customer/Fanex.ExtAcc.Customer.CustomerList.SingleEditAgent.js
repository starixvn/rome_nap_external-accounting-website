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
                        sportbook: {
                            name: "Sportsbook",
                            className: "iconEnableEdit",
                            callback: function () {
                                Fanex.ExtAcc.Customer.IsFullInfo = true;
                                Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/EditMultiplePT?roleId=2&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName + '&custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit Sportsbook Setting ' + '<font color="#6dcff6">' + title + '</font>', 900, Fanex.ExtAcc.Customer.GetPopupHeight(270, 480), false, false, 1);
                            }
                        },
                        ptSportbook: {
                            name: "PT",
                            className: "iconEnableEdit",
                            callback: function (key, opt) {
                                Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/EditPT?groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName + '&custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit PT Setting ' + '<font color="#6dcff6">' + title + '</font>', 888, Fanex.ExtAcc.Customer.GetPopupHeight(270, 480), false, false, 1);
                            }
                        },
                        minPTSportbook: {
                            name: "Min PT",
                            className: "iconEnableEdit",
                            callback: function (key, opt) {
                                Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/EditMinPT?groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName + '&custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit Min PT ' + '<font color="#6dcff6">' + title + '</font>', 888, Fanex.ExtAcc.Customer.GetPopupHeight(270, 480), false, false, 1);
                            }
                        },
                        betSetting: {
                            name: "Bet Setting",
                            className: "iconEnableEdit",
                            callback: function (key, opt) {
                                var height = isMobile.phone ? Fanex.ExtAcc.Customer.GetPopupHeight(270, 480) : 560;
                                Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/EditBetSetting?groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName + '&custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit Bet Setting ' + '<font color="#6dcff6">' + title + '</font>', 871, height, false, false, 1);
                            }
                        },
                        commission: {
                            name: "Commission",
                            className: "iconEnableEdit",
                            callback: function (key, opt) {
                                Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/EditCommissionDetail?groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName + '&custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit Commission ' + '<font color="#6dcff6">' + title + '</font>', 871, 250, false, false, 1);
                            }
                        },
                        separator1: "-----"
                    }

                    function addEditProductAgentContextMenuItem(productId, productKey, productName, isDisabled, isSynced) {
                        isSynced = typeof (isSynced) == 'undefined' ? true : isSynced;

                        items[productKey] = {
                            name: productName,
                            className: !isSynced ? "iconUnsyncEdit" : (isDisabled == true ? "iconDisableEdit" : "iconEnableEdit"),
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

                    var has3rdParty = false;
                    if (!result.HideVirtualSport) {
                        has3rdParty = true;
                        addEditProductAgentContextMenuItem(5, 'virtualSport', 'Virtual Sports', result.DisableVirtualSport);
                    }                  

                    for (var i = 0, max = result.DCSProductsSetting.length; i < max; i++) {
                        has3rdParty = true;
                        var productSetting = result.DCSProductsSetting[i];

                        if (!productSetting.IsHidden && productSetting.IsSyncedUpline) {
                            addEditProductAgentContextMenuItem(
                                productSetting.ProductID,
                                'Product-' + productSetting.ProductID,
                                productSetting.ProductName,
                                productSetting.IsDisabled,
                                productSetting.IsSynced);
                        }
                    }

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

                    items.parlayMaxPayout = {
                        name: "Parlay Max Payout",
                        className: "iconEnableEdit editInfoBg",
                        callback: function (key, opt) {
                            top.shouldCalculatePopupHeight = false;
                            var url = resolveClientUrl('ParlayMaxPayout/Index?custId=' + custId + '&custName=' + title + '&roleId=2' + '&groupName=' + $('#selectedGroupName').html());
                            Fanex.ExtAcc.Form.ShowFrame(url, 'Edit Parlay Max Payout: <font color="#6dcff6">' + title + '</font>', 1050, 350, false, false, 1, null, null, true);
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

