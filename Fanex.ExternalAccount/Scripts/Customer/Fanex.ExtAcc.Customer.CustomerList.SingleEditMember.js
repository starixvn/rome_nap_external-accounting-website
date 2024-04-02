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
                                Fanex.ExtAcc.Form.ShowFrame(resolveClientUrl('Customer/EditCustomer?roleId=1&uplineRoleId=2&groupName=' + Fanex.ExtAcc.Customer.GroupName().groupName + "&productGroupId=1" + '&custId=' + Fanex.ExtAcc.Customer.UserId), 'Edit Sportsbook Setting ' + '<font color="#6dcff6">' + title + '</font>', 1250, Fanex.ExtAcc.Customer.GetPopupHeight(270, 480), false, false, 1);
                            }
                        },
                        separator1: "-----"
                    };

                    function addEditProductMemberContextMenuItem(productId, productKey, productName, isDisabled, isSynced) {
                        isSynced = typeof (isSynced) == 'undefined' ? true : isSynced;

                        items[productKey] = {
                            name: productName,
                            className: "iconEnableEdit",
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

                    var has3rdParty = false;
                    if (!result.HideVirtualSport) {
                        has3rdParty = true;
                        addEditProductMemberContextMenuItem(5, 'virtualSport', 'Virtual Sports', result.DisableVirtualSport);
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