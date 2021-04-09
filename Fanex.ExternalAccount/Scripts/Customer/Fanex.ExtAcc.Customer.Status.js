Fanex.ExtAcc.Customer.Status = new function () {
    var self = this;
    self.cachedDownlinesDetailInfoData;
    self.DOMElements;
    self.CustIdList;
    self.RoleId;
    self.OldFormStatuses;
    self.CurrentFormStatuses;

    self.clearMessage = function () {
        self.DOMElements.successMsg.empty().hide();
        self.DOMElements.errMsg.empty().hide();
    };

    self.btnUnsuspendSingle_OnClick = function () {
        var params = {
            custId: self.DOMElements.custId.val(),
            suspended: false
        };

        self.UpdateCustomerStatus(
            resolveClientUrl('Customer/UpdateSuspendSingle'),
            params,
            'btnUnsuspendSingle',
            'suspend-confirm-box',
            top.window.LanguageResources.UnsuspendSingleConfirmMesssage,
            top.window.LanguageResources.UpdateSuspendSuccess);
    };

    self.btnOpenSingle_OnClick = function () {
        var params = {
            custId: self.DOMElements.custId.val(),
            isClosed: false
        };

        self.UpdateCustomerStatus(
            resolveClientUrl('Customer/SingleOpenCustomer'),
            params,
            'btnOpenSingle',
            'open-confirm-box',
            top.window.LanguageResources.OpenSingleConfirmMesssage,
            top.window.LanguageResources.UpdateCloseSuccess);
    };

    self.UpdateCustomerStatus = function (url, params, domElementName, confirmBoxId, confirmBoxMsg, updateSuccessMsg) {
        Fanex.ExtAcc.Form.ConfirmBox('Information', confirmBoxMsg, 400, 180, null, function () {
            parent.Fanex.ExtAcc.Form.Close(null, null, confirmBoxId);
            parent.Fanex.ExtAcc.BlockUI();
            self.DOMElements[domElementName].attr("disabled", true);
            self.clearMessage();

            $.ajax({
                type: 'POST',
                url,
                dataType: "json",
                data: params,
                success: function (result) {
                    if (result.ErrorCode == 0) {
                        self.DOMElements.successMsg.html(updateSuccessMsg).show();
                        setTimeout(function () {
                            parent.$('#windowPopup').dialog('close');
                            parent.$('#windowPopup').dialog('destroy');
                            top.window.frames['leftFrame'].src = top.window.frames['leftFrame'].src;
                        }, 3000);
                    }
                    else {
                        self.DOMElements.errMsg.html(result.ErrorMessage).show();
                        self.DOMElements[domElementName].removeAttr("disabled");
                    }
                },
                complete: function () {
                    self.DOMElements[domElementName].removeAttr("disabled");
                    parent.Fanex.ExtAcc.UnBlockUI();
                }
            });
        }, null, confirmBoxId, false, null, false);
    };

    self.btnBack_OnClick = function () {
        self.DOMElements.tabDetail.fadeOut(function () {
            self.DOMElements.tabMain.fadeIn(function () {
                parent.$('#windowPopup').dialog({ position: { my: "center", at: "center", of: top.window } });
            });
        });
    };

    self.btnReset_OnClick = function () {
        parent.Fanex.ExtAcc.BlockUI();
        var params = {
            custIdList: self.CustIdList,
            roleId: self.RoleId
        };

        $.ajax({
            type: 'POST',
            url: resolveClientUrl('Customer/MultipleSuspend'),
            dataType: 'html',
            data: params,
            success: function (result) {
                $('#windowPopup > div').eq(0).html(result);
                self.Init();
            },
            complete: function () {
                parent.Fanex.ExtAcc.UnBlockUI();
            }
        });
    };

    self.btnSuspendMultiple_OnClick = function () {
        Fanex.ExtAcc.Form.ConfirmBox('Information', top.window.LanguageResources.UnsuspendSingleConfirmMesssage, 'auto', 'auto', null, function () {
            parent.Fanex.ExtAcc.Form.Close(null, null, 'suspend-confirm-box');
            parent.Fanex.ExtAcc.BlockUI();
            self.DOMElements.btnReset.attr("disabled", true);
            self.DOMElements.btnSuspendMultiple.attr("disabled", true);
            self.clearMessage();
            var params = {
                request: $('.button-suspend-status.is-changed').map(function () {
                    var $this = $(this);
                    return { CustId: $this.attr('data-custid'), UserName: $this.attr('data-username'), Suspended: $this.hasClass('suspend-switch') };
                }).get()
            };

            $.ajax({
                type: 'POST',
                url: resolveClientUrl('Customer/UpdateSuspendMultiple'),
                data: params,
                success: function (result) {
                    if (parent.Fanex.ExtAcc.ShowSecurityCodePage(result, parent.Fanex.ExtAcc.UnBlockUI)) {
                        return;
                    }

                    var isSuccessAll = true;
                    for (var i = 0; i < result.length; i++) {
                        if (result[i].IsSuccess) {
                            self.DOMElements.successMsg.append('<p style="margin: 0px">' + result[i].ErrorMessage + '</p>').show();
                        }
                        else {
                            self.DOMElements.errMsg.append('<p style="margin: 0px">' + result[i].ErrorMessage + '</p>').show();
                            isSuccessAll = false;
                        }
                    }

                    if (isSuccessAll) {
                        setTimeout(function () {
                            parent.$('#windowPopup').dialog('close');
                            parent.$('#windowPopup').dialog('destroy');
                            Fanex.ExtAcc.Customer.CallBackCustomerList();
                        }, 3000);
                    }
                },
                complete: function () {
                    self.DOMElements.btnReset.removeAttr("disabled");
                    self.DOMElements.btnSuspendMultiple.removeAttr("disabled");
                    parent.Fanex.ExtAcc.UnBlockUI();
                }
            });
        }, null, 'suspend-confirm-box', false, null, false);
    };

    self.btnSuspendStatus_OnClick = function () {
        var $this = $(this);
        var custId = $this.attr('data-custid');
        $this.toggleClass('suspend-switch unsuspend-switch');
        $this.toggleClass('is-changed');
        if ($this.hasClass('suspend-switch')) {
            $this.html('  Suspended');
        }
        else {
            $this.html('Unsuspended');
        }

        $('#row-remain-unchanged-empty-' + custId).toggle();
        $('#row-remain-unchanged-SMA-' + custId).toggle();
        $('#row-affected-empty-' + custId).toggle();
        $('#row-affected-SMA-' + custId).toggle();
        var currentCustomerStatusItem = self.CurrentFormStatuses.find(function (customerStatus) {
            return customerStatus.custId == this;
        }, custId);

        currentCustomerStatusItem.suspend = !currentCustomerStatusItem.suspend;
        if (self.IsFormChanged()) {
            self.DOMElements.btnSuspendMultiple.removeAttr("disabled");
            self.DOMElements.btnSuspendMultiple.removeClass("ui-state-disabled");
        }
        else {
            self.DOMElements.btnSuspendMultiple.attr("disabled", "disabled");
            self.DOMElements.btnSuspendMultiple.addClass("ui-state-disabled");
        }
    };

    self.IsFormChanged = function () {
        for (var i = 0; i < self.CurrentFormStatuses.length; i++) {
            if (self.CurrentFormStatuses[i].suspend != self.OldFormStatuses[i].suspend) {
                return true;
            }
        }

        return false;
    };

    self.btnSuspendAll_OnClick = function () {
        $('.button-suspend-status:not(.suspend-switch)').click();
    };

    self.btnUnsuspendAll_OnClick = function () {
        $('.button-suspend-status:not(.unsuspend-switch)').click();
    };

    self.informationBox_OnClick = function () {
        Fanex.ExtAcc.Form.Alert('Information', top.window.LanguageResources.SuspendInfoMessage, 750, 350, 'suspend-information-box');
    };

    self.informationBoxSingleOpen_OnClick = function () {
        Fanex.ExtAcc.Form.Alert('Information', top.window.LanguageResources.OpenInfoMessage, 750, 350, 'open-information-box');
    };

    self.Init = function (custIdList, roleId) {
        self.cachedDownlinesDetailInfoData = {
            custId: 0,
            isAffected: false,
            downlineRoleId: 0
        };

        self.DOMElements = {
            btnUnsuspendSingle: $('#btnUnsuspend'),
            btnOpenSingle: $('#btnOpen'),
            custId: $('#custId'),
            successMsg: $('#successMsg'),
            errMsg: $('#errMsg'),
            btnBack: $('#btnBack'),
            tabDetail: $('#tab-detail'),
            tabMain: $('#tab-main'),
            gridContainer: $('#gridContainer'),
            gridId: $('#gridId'),
            btnReset: $('#btnReset'),
            btnSuspendMultiple: $('#btnSuspendMultiple'),
            btnSuspendStatus: $('.button-suspend-status'),
            btnSuspendAll: $('#button-suspend-all'),
            btnUnsuspendAll: $('#button-unsuspend-all'),
            informationBox: $('#information-box'),
            informationBoxSingleOpen: $('#single-open-customer-information-box'),
            tblCustSuspend: $('#tbl-cust-suspend'),
            detailHeaderTitle: $('#detail-header-title')
        };

        self.OldFormStatuses = []; self.CurrentFormStatuses = [];
        $('.button-suspend-status', self.DOMElements.tblCustSuspend).each(function () {
            var $this = $(this);
            self.OldFormStatuses.push({ custId: $this.attr('data-custid'), userName: $this.attr('data-username'), suspend: $this.attr('data-suspend-status') == 'true' });
            self.CurrentFormStatuses.push({ custId: $this.attr('data-custid'), userName: $this.attr('data-username'), suspend: $this.attr('data-suspend-status') == 'true' });
        });

        self.DOMElements.btnUnsuspendSingle.click(function () {
            return self.btnUnsuspendSingle_OnClick();
        });

        self.DOMElements.btnOpenSingle.click(function () {
            return self.btnOpenSingle_OnClick();
        });

        self.DOMElements.btnBack.click(function () {
            return self.btnBack_OnClick();
        });

        self.DOMElements.btnReset.click(function () {
            return self.btnReset_OnClick();
        });

        self.DOMElements.btnSuspendMultiple.click(function () {
            return self.btnSuspendMultiple_OnClick();
        });

        self.DOMElements.btnSuspendStatus.click(self.btnSuspendStatus_OnClick);

        self.DOMElements.btnSuspendAll.click(function () {
            return self.btnSuspendAll_OnClick();
        });

        self.DOMElements.btnUnsuspendAll.click(function () {
            return self.btnUnsuspendAll_OnClick();
        });

        self.DOMElements.informationBox.click(function () {
            return self.informationBox_OnClick();
        });

        self.DOMElements.informationBoxSingleOpen.click(function () {
            return self.informationBoxSingleOpen_OnClick();
        });
    };

    self.ViewSuspendSingle = function (custId, userName) {
        Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/UnsuspendSingle') + '?custId=' + custId + '&userName=' + userName, 'Update Unsuspend Status for ' + userName, 'auto', 'auto', false, self.Init);
    };

    self.ViewSuspendMultiple = function (custIdList, roleId) {
        Fanex.ExtAcc.Form.Show(resolveClientUrl('Customer/MultipleSuspend?custIdList=' + custIdList + '&roleid=' + roleId), 'Edit Multiple Suspend Status', 'auto', 'auto', false, self.Init);
        self.CustIdList = custIdList.join();
        self.RoleId = roleId;
    };

    self.ShowSingleOpenCustomerPopup = function (custId, userName) {
        var url = resolveClientUrl('Customer/SingleOpenCustomerView') + '?custId=' + custId + '&userName=' + userName;
        var title = 'Open ' + userName;

        Fanex.ExtAcc.Form.Show(url, title, 'auto', 'auto', false, self.Init);
    };

    self.ShowMultipleOpenCustomerPopup = function (custIdList, roleId) {
        var url = resolveClientUrl('Customer/MultipleOpenCustomerView?custIdList=' + custIdList + '&roleid=' + roleId);
        var title = 'Update Multiple Closed Status';

        Fanex.ExtAcc.Form.Show(url, title, 'auto', 'auto', false, self.Init);

        self.CustIdList = custIdList.join();
        self.RoleId = roleId;
    };

    self.RerenderPagingDownlineDetailInfo = function (currentPageIndex, itemsPerPage) {
        self.GetDownlinesDetailInfo(self.cachedDownlinesDetailInfoData.custId, self.cachedDownlinesDetailInfoData.userName, self.cachedDownlinesDetailInfoData.isAffected, self.cachedDownlinesDetailInfoData.downlineRoleId, itemsPerPage, currentPageIndex);
    };

    self.RerenderPagingSingleOpenDownlineDetailInfo = function (currentPageIndex, itemsPerPage) {
        self.GetSingleOpenDownlinesDetailInfo(
            self.cachedDownlinesDetailInfoData.custId,
            self.cachedDownlinesDetailInfoData.userName,
            self.cachedDownlinesDetailInfoData.isAffected,
            self.cachedDownlinesDetailInfoData.downlineRoleId,
            itemsPerPage,
            currentPageIndex);
    };

    self.GetDownlinesDetailInfo = function (custId, userName, isAffected, downlineRoleId, pageSize, pageIndex) {
        self.CallGetDownlinesDetailInfo(
            resolveClientUrl('Customer/GetSuspendDownlinesDetail'),
            custId,
            userName,
            isAffected,
            downlineRoleId,
            pageSize,
            pageIndex,
            top.window.LanguageResources.UnsuspendedTitleAffectedDownlines,
            top.window.LanguageResources.UnsuspendedTitleRemainUnchangedDownlines,
            self.RerenderPagingDownlineDetailInfo);
    };

    self.GetSingleOpenDownlinesDetailInfo = function (custId, userName, isAffected, downlineRoleId, pageSize, pageIndex) {
        self.CallGetDownlinesDetailInfo(
            resolveClientUrl('Customer/GetSingleOpenDownlinesInfo'),
            custId,
            userName,
            isAffected,
            downlineRoleId,
            pageSize,
            pageIndex,
            top.window.LanguageResources.OpenedAffectedDownlinesTitle,
            top.window.LanguageResources.OpenedRemainUnchangedDownlinesTitle,
            self.RerenderPagingSingleOpenDownlineDetailInfo);
    };

    self.CallGetDownlinesDetailInfo = function (
        url,
        custId,
        userName,
        isAffected,
        downlineRoleId,
        pageSize,
        pageIndex,
        affectedDownlinesTitle,
        remainUnchangedDownlinesTitle,
        pagerSelectCallBack) {
        parent.Fanex.ExtAcc.BlockUI();

        self.DOMElements.detailHeaderTitle.html(isAffected
            ? affectedDownlinesTitle.replace('{0}', userName)
            : remainUnchangedDownlinesTitle.replace('{0}', userName));

        var params = {
            custId: custId,
            isAffected: isAffected,
            downlineRoleId: downlineRoleId,
            pageSize: pageSize,
            pageIndex: pageIndex
        };

        self.cachedDownlinesDetailInfoData.custId = custId;
        self.cachedDownlinesDetailInfoData.userName = userName;
        self.cachedDownlinesDetailInfoData.isAffected = isAffected;
        self.cachedDownlinesDetailInfoData.downlineRoleId = downlineRoleId;

        $.ajax({
            type: 'POST',
            url,
            dataType: 'json',
            data: params,
            success: function (result) {
                var bodyRows = [];
                var rowCount = result.AffectedDownlines.length;
                var startMergedRowIndex = 0;
                var endMergedRowIndex = 0;
                var prevLevel = '';
                var currentLevel = '';

                for (var i = 0; i < rowCount; i++) {
                    var row = { Columns: [] };
                    currentLevel = result.AffectedDownlines[i].LevelName;
                    endMergedRowIndex = i;
                    row.Columns.push({ Name: 'No', Value: 0 });

                    if (prevLevel != currentLevel) {
                        if (startMergedRowIndex != endMergedRowIndex) {
                            bodyRows[startMergedRowIndex].Columns[1].MergeRowNumber = endMergedRowIndex - startMergedRowIndex;
                        }

                        startMergedRowIndex = endMergedRowIndex;
                        row.Columns.push({ Name: 'Level', Value: result.AffectedDownlines[i].LevelName });
                    }

                    row.Columns.push({ Name: 'Username', Value: result.AffectedDownlines[i].UserName });
                    bodyRows.push(row);
                    prevLevel = currentLevel;
                }

                if (startMergedRowIndex != endMergedRowIndex) {
                    bodyRows[startMergedRowIndex].Columns[1].MergeRowNumber = endMergedRowIndex - startMergedRowIndex + 1;
                }

                var gridOptions =
                    {
                        columns: [
                            { HeaderText: "No.", Width: 50, Name: "No", HeaderAlign: "center", CellAlign: "center" },
                            { HeaderText: "Level", Width: 100, Name: "Level", HeaderAlign: "center" },
                            { HeaderText: "Username", Width: 200, Name: "Username", HeaderAlign: "center" }
                        ],
                        bodyRows: bodyRows,
                        showDistinctRow: false,
                        gridExpandHeight: top.window.innerHeight / 3,
                        showNumberColumn: true,
                        showPager: true,
                        customPager: true,
                        pagerOption: {
                            itemsPerPage: pageSize,
                            currentPage: pageIndex,
                            itemsPerPageArray: [50, 100, 200, 500],
                            pagerSelectCallBack,
                            totalItem: result.TotalRows
                        }
                    };

                self.DOMElements.tabMain.fadeOut(function () {
                    self.DOMElements.tabDetail.fadeIn(function () {
                        self.DOMElements.gridContainer.show();
                        self.DOMElements.gridId.fanexGrid(gridOptions);
                        parent.$('#windowPopup').dialog({ position: { my: "center", at: "center", of: top.window } });
                    });
                });
            },
            complete: function () {
                parent.Fanex.ExtAcc.UnBlockUI();
            }
        });
    };
};