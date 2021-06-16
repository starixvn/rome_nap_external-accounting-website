Fanex.ExtAcc.Customer.CustomerList.SingleEditMember = function (custId, title) {
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
                                Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/EditMultiplePT?roleId=1&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName + '&custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit Sportsbook Setting ' + '<font color="#6dcff6">' + title + '</font>', 900, Fanex.ExtAcc.Customer.GetPopupHeight(270, 480), false, false, 1);
                            }
                        },
                        ptSportbook: {
                            name: "PT",
                            className: "iconEnableEdit",
                            callback: function (key, opt) {
                                Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/EditPT?groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName + '&custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit PT Setting ' + '<font color="#6dcff6">' + title + '</font>', 888, Fanex.ExtAcc.Customer.GetPopupHeight(270, 480), false, false, 1);
                            }
                        },
                        betSetting: {
                            name: "Bet Setting",
                            className: "iconEnableEdit",
                            callback: function (key, opt) {
                                var height = isMobile.phone ? Fanex.ExtAcc.Customer.GetPopupHeight(270, 480) : 560;
                                Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/EditBetSetting?groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName + '&custId=' + Fanex.ExtAcc.Customer.UserId) + '&roleid=1', 'Edit Bet Setting ' + '<font color="#6dcff6">' + title + '</font>', 871, height, false, false, 1);
                            }
                        },
                        commission: {
                            name: "Commission",
                            className: "iconEnableEdit",
                            callback: function (key, opt) {
                                var height = isMobile.phone ? Fanex.ExtAcc.Customer.GetPopupHeight(270, 350) : 350;
                                Fanex.ExtAcc.Form.Show(resolveClientUrl('SportsbookSetting/EditCommissionMemberDetail?groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName + '&custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit Commission ' + '<font color="#6dcff6">' + title + '</font>', 876, height, false, false, 1);
                            }
                        },
                        separator1: "-----"
                    };
                    var has3rdParty = false;
                    if (result.UplineSyncedCasino) {
                        has3rdParty = true;

                        items.casino = {
                            name: "Casino",
                            className: result.SyncedCasino
                                ? (result.DisableCasino ? "iconDisableEdit" : "iconEnableEdit")
                                : "iconUnsyncEdit",
                            callback: function () {
                                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('CustomerSetting/Casino/Member/EditMember?id=' + Fanex.ExtAcc.Customer.UserId + '&uplineid=' + Fanex.ExtAcc.Customer.agentId + '&groupname=' + $('#selectedGroupName').html()), 'Edit Casino Setting <font color="#6dcff6">' + title + '</font>', 1150, Fanex.ExtAcc.Customer.GetPopupHeight(270, 480), false, null, 1);
                            }
                        };
                    }

                    function addEditProductMemberContextMenuItem(productId, productKey, productName, isDisabled, isSynced) {
                        isSynced = typeof (isSynced) == 'undefined' ? true : isSynced;

                        items[productKey] = {
                            name: productName,
                            className: !isSynced ? "iconUnsyncEdit" : (isDisabled == true ? "iconDisableEdit" : "iconEnableEdit"),
                            callback: function () {
                                Fanex.ExtAcc.Customer.IsFullInfo = true;
                                Fanex.ExtAcc.Customer.ShowEditProductForSingleCust(
                                    custId,
                                    2,
                                    1,
                                    productId,
                                    productName);
                            }
                        };
                    }

                    if (!result.HideVirtualSport) {
                        has3rdParty = true;
                        addEditProductMemberContextMenuItem(5, 'virtualSport', 'Virtual Sports', result.DisableVirtualSport);
                    }

                    if (!result.HideKeno) {
                        has3rdParty = true;

                        items.keno = {
                            name: "RNG Keno",
                            className: result.DisableKeno ? "iconDisableEdit" : "iconEnableEdit",
                            callback: function () {
                                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('customersetting/KenoSetting/kenosetting/AgentEditMember?custId=' + Fanex.ExtAcc.Customer.UserId + "&userId=" + parent.userId), 'Edit RNG Keno Setting <font color="#6dcff6">' + title + '</font>', 900, Fanex.ExtAcc.Customer.GetPopupHeight(270, 480), false, null, 1);
                            }
                        };
                    }

                    if (!result.HideGoldDeluxe) {
                        has3rdParty = true;

                        items.goldDeluxe = {
                            name: "Gold Deluxe",
                            className: result.DisableGoldDeluxe == true ? "iconDisableEdit" : "iconEnableEdit",
                            callback: function () {
                                var height = isMobile.phone ? Fanex.ExtAcc.Customer.GetPopupHeight(270, 400) : 400;
                                Fanex.ExtAcc.Customer.UserId = custId;
                                url = "GoldDeluxeSetting/GoldDeluxe/EditSetting?upLineLevel=2&formType=1&level=1" + "&uplineId=" + Fanex.ExtAcc.Customer.agentId + "&actorId=" + parent.userId + "&custId=" + Fanex.ExtAcc.Customer.UserId + "&groupName=" + parent.$('#selectedGroupName').html();
                                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl(url), 'Edit Gold Deluxe Setting <font color="#6dcff6">' + title + '</font>', 900, height, false, false, 1);
                            }
                        };
                    }

                    for (var i = 0, max = result.DCSProductsSetting.length; i < max; i++) {
                        has3rdParty = true;
                        var productSetting = result.DCSProductsSetting[i];
                        if (!productSetting.IsHidden && productSetting.IsSyncedUpline) {
                            addEditProductMemberContextMenuItem(
                                productSetting.ProductID,
                                'Product-' + productSetting.ProductID,
                                productSetting.ProductName,
                                productSetting.IsDisabled,
                                productSetting.IsSynced);
                        }
                    }

                    if (has3rdParty)
                        items.separator5 = "-----";
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
                                false,
                                false);
                        }
                    };

                    items.parlayMaxPayout = {
                        name: "Parlay Max Payout",
                        className: "iconEnableEdit editInfoBg",
                        callback: function (key, opt) {
                            top.shouldCalculatePopupHeight = false;
                            var url = resolveClientUrl('ParlayMaxPayout/Index?custId=' + custId + '&custName=' + title + '&roleId=1');
                            Fanex.ExtAcc.Form.ShowFrame(url, 'Edit Parlay Max Payout: <font color="#6dcff6">' + title + '</font>', 1050, 350, false, false, 1);
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