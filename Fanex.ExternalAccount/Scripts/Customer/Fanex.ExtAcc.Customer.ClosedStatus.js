Fanex.ExtAcc.Customer.ClosedStatus = new function () {
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

    self.btnOpenSingle_OnClick = function () {
        Fanex.ExtAcc.Form.ConfirmBox('Information', top.window.LanguageResources.OpenSingleConfirmMesssage, 400, 180, null, function () {
            parent.Fanex.ExtAcc.Form.Close(null, null, 'open-confirm-box');
            parent.Fanex.ExtAcc.BlockUI();
            self.DOMElements.btnOpenSingle.attr("disabled", true);
            self.clearMessage();
            var params = {
                custId: self.DOMElements.custId.val(),
                isClosed: false
            };

            $.ajax({
                type: 'POST',
                url: resolveClientUrl('Customer/SingleOpenCustomer'),
                dataType: "json",
                data: params,
                success: function (result) {
                    if (result && result.errorCode && result.errorCode == 1) {
                        return self.DOMElements.errMsg.html(result.errorMessage).show();
                    }

                    if (result.ErrorCode == 0) {
                        self.DOMElements.successMsg.html(top.window.LanguageResources.UpdateCloseSuccess).show();
                        setTimeout(function () {
                            parent.$('#windowPopup').dialog('close');
                            parent.$('#windowPopup').dialog('destroy');
                            top.window.frames['leftFrame'].src = top.window.frames['leftFrame'].src;
                        }, 3000);
                    }
                    else {
                        self.DOMElements.errMsg.html(result.ErrorMessage).show();
                        self.DOMElements.btnOpenSingle.removeAttr("disabled");
                    }
                },
                complete: function () {
                    self.DOMElements.btnOpenSingle.removeAttr("disabled");
                    parent.Fanex.ExtAcc.UnBlockUI();
                }
            });
        }, null, 'open-confirm-box', false, null, false);
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
            url: resolveClientUrl('Customer/MultipleOpenCustomerView'),
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

    self.btnCloseMultiple_OnClick = function () {
        var message = top.window.LanguageResources.OpenMultipleSMAConfirmMessage;
        if (self.RoleId == 1) {
            var openUserNames = '',
                closeUserNames = '';
            $('.button-close-status.is-changed').each(function () {
                var $this = $(this);
                if ($this.hasClass('suspend-switch')) {
                    closeUserNames += $this.attr('data-username') + ', ';
                } else {
                    openUserNames += $this.attr('data-username') + ', ';
                }
            });

            openUserNames = openUserNames.substring(0, openUserNames.length - 2);
            closeUserNames = closeUserNames.substring(0, closeUserNames.length - 2);
            message = openUserNames.length > 0
                ? top.window.LanguageResources.OpenMultipleConfirmMessage.replace('{0}', openUserNames) + '<br />'
                : '';
            message += closeUserNames.length > 0
                ? top.window.LanguageResources.CloseMultipleConfirmMessage.replace('{0}', closeUserNames) + '<br />'
                : '';
            message += top.window.LanguageResources.ProceedConfirmMessage;
        }

        Fanex.ExtAcc.Form.ConfirmBox('Information', message, 'auto', 'auto', null, function () {
            parent.Fanex.ExtAcc.Form.Close(null, null, 'open-confirm-box');
            parent.Fanex.ExtAcc.BlockUI();
            self.DOMElements.btnReset.attr("disabled", true);
            self.DOMElements.btnCloseMultiple.attr("disabled", true);
            self.clearMessage();
            var params = {
                request: $('.button-close-status.is-changed').map(function () {
                    var $this = $(this);
                    return { CustId: $this.attr('data-custid'), UserName: $this.attr('data-username'), IsClosed: $this.hasClass('suspend-switch') };
                }).get(),
                groupName: $('#selectedGroupName').html(),
                roleId: self.RoleId
            };

            $.ajax({
                type: 'POST',
                url: resolveClientUrl('Customer/MultipleOpenCustomer'),
                data: params,
                success: function (result) {
                    if (parent.Fanex.ExtAcc.ShowSecurityCodePage(result, parent.Fanex.ExtAcc.UnBlockUI)) {
                        return;
                    }

                    if (result && result.errorCode && result.errorCode == 1) {
                        return self.DOMElements.errMsg.html(result.errorMessage).show();
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
                    self.DOMElements.btnCloseMultiple.removeAttr("disabled");
                    parent.Fanex.ExtAcc.UnBlockUI();
                }
            });
        }, null, 'open-confirm-box', false, null, false);
    };

    self.btnCloseStatus_OnClick = function () {
        var $this = $(this);
        var custId = $this.attr('data-custid');
        $this.toggleClass('suspend-switch unsuspend-switch');
        $this.toggleClass('is-changed');
        if ($this.hasClass('suspend-switch')) {
            $this.html('  Closed');
        }
        else {
            $this.html('Opened');
        }

        $('#row-remain-unchanged-empty-' + custId).toggle();
        $('#row-remain-unchanged-SMA-' + custId).toggle();
        $('#row-affected-empty-' + custId).toggle();
        $('#row-affected-SMA-' + custId).toggle();
        var currentCustomerStatusItem = self.CurrentFormStatuses.find(function (customerStatus) {
            return customerStatus.custId == this;
        }, custId);

        currentCustomerStatusItem.isClosed = !currentCustomerStatusItem.isClosed;
        if (self.IsFormChanged()) {
            self.DOMElements.btnCloseMultiple.removeAttr("disabled");
            self.DOMElements.btnCloseMultiple.removeClass("ui-state-disabled");
        }
        else {
            self.DOMElements.btnCloseMultiple.attr("disabled", "disabled");
            self.DOMElements.btnCloseMultiple.addClass("ui-state-disabled");
        }
    };

    self.IsFormChanged = function () {
        for (var i = 0; i < self.CurrentFormStatuses.length; i++) {
            if (self.CurrentFormStatuses[i].isClosed != self.OldFormStatuses[i].isClosed) {
                return true;
            }
        }

        return false;
    };

    self.btnCloseAll_OnClick = function () {
        $('.button-close-status:not(.suspend-switch)').click();
    };

    self.btnOpenAll_OnClick = function () {
        $('.button-close-status:not(.unsuspend-switch)').click();
    };

    self.informationBox_OnClick = function () {
        Fanex.ExtAcc.Form.Alert('Information', top.window.LanguageResources.OpenInfoMessage, 750, 350, 'open-information-box');
    };

    self.Init = function (custIdList, roleId) {
        self.cachedDownlinesDetailInfoData = {
            custId: 0,
            isAffected: false,
            downlineRoleId: 0
        };

        self.DOMElements = {
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
            btnCloseMultiple: $('#btnCloseMultiple'),
            btnCloseStatus: $('.button-close-status'),
            btnCloseAll: $('#button-close-all'),
            btnOpenAll: $('#button-open-all'),
            informationBox: $('#information-box'),
            tblCustClose: $('#tbl-cust-close'),
            detailHeaderTitle: $('#detail-header-title')
        };

        self.OldFormStatuses = []; self.CurrentFormStatuses = [];
        $('.button-close-status', self.DOMElements.tblCustClose).each(function () {
            var $this = $(this);
            self.OldFormStatuses.push({ custId: $this.attr('data-custid'), userName: $this.attr('data-username'), isClosed: $this.attr('data-close-status') == 'true' });
            self.CurrentFormStatuses.push({ custId: $this.attr('data-custid'), userName: $this.attr('data-username'), isClosed: $this.attr('data-close-status') == 'true' });
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

        self.DOMElements.btnCloseMultiple.click(function () {
            return self.btnCloseMultiple_OnClick();
        });

        self.DOMElements.btnCloseStatus.click(self.btnCloseStatus_OnClick);

        self.DOMElements.btnCloseAll.click(function () {
            return self.btnCloseAll_OnClick();
        });

        self.DOMElements.btnOpenAll.click(function () {
            return self.btnOpenAll_OnClick();
        });

        self.DOMElements.informationBox.click(function () {
            return self.informationBox_OnClick();
        });
    };

    self.ShowSingleOpenCustomerPopup = function (custId, userName) {
        var url = resolveClientUrl('Customer/SingleOpenCustomerView') + '?custId=' + custId + '&userName=' + userName;
        var title = 'Open ' + userName;

        Fanex.ExtAcc.Form.Show(url, title, 'auto', 'auto', false, self.Init);
    };

    self.ShowMultipleOpenCustomerPopup = function (custIdList, roleId) {
        var url = resolveClientUrl('Customer/MultipleOpenCustomerView?custIdList=' + custIdList + '&roleid=' + roleId);
        var title = 'Edit Multiple Closed Status';

        Fanex.ExtAcc.Form.Show(url, title, 'auto', 'auto', false, self.Init);

        self.CustIdList = custIdList.join();
        self.RoleId = roleId;
    };

    self.RerenderPagingDownlineDetailInfo = function (currentPageIndex, itemsPerPage) {
        self.GetDownlinesDetailInfo(
            self.cachedDownlinesDetailInfoData.custId,
            self.cachedDownlinesDetailInfoData.userName,
            self.cachedDownlinesDetailInfoData.isAffected,
            self.cachedDownlinesDetailInfoData.downlineRoleId,
            itemsPerPage,
            currentPageIndex);
    };

    self.GetDownlinesDetailInfo = function (custId, userName, isAffected, downlineRoleId, pageSize, pageIndex) {
        parent.Fanex.ExtAcc.BlockUI();
        self.DOMElements.detailHeaderTitle.html(isAffected
            ? top.window.LanguageResources.OpenedAffectedDownlinesTitle.replace('{0}', userName)
            : top.window.LanguageResources.OpenedRemainUnchangedDownlinesTitle.replace('{0}', userName));

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
            url: resolveClientUrl('Customer/GetSingleOpenDownlinesInfo'),
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

                var gridContainerWidth = calculatedGridContainerWidth(result.TotalRows, pageSize);
                var yScrollBarWidth = 20,
                    columnNoWidth = 50,
                    columnLevelWidth = 100,
                    columnUsernameWidth = gridContainerWidth > 0
                        ? gridContainerWidth - columnNoWidth - columnLevelWidth - 2 * yScrollBarWidth
                        : 200;
                var gridOptions =
                    {
                        columns: [
                            { HeaderText: "No.", Width: columnNoWidth, Name: "No", HeaderAlign: "center", CellAlign: "center" },
                            { HeaderText: "Level", Width: columnLevelWidth, Name: "Level", HeaderAlign: "center" },
                            { HeaderText: "Username", Width: columnUsernameWidth, Name: "Username", HeaderAlign: "center" }
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
                            pagerSelectCallBack: self.RerenderPagingDownlineDetailInfo,
                            totalItem: result.TotalRows
                        }
                    };

                self.DOMElements.tabMain.fadeOut(function () {
                    self.DOMElements.tabDetail.fadeIn(function () {
                        self.DOMElements.gridContainer.css('width', gridContainerWidth);
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

    var calculatedGridContainerWidth = function (totalRows, pageSize) {
        var numberOfPages = Math.ceil(totalRows / pageSize);
        if (numberOfPages > 0) {
            var yScrollBarWidth = 20,
                itemPerPageWidth = 120,
                detailTextWidth = 200,
                numberListMargin = 12,
                prevNextButtonWidth = 48,
                pageNumberButtonWidth = 24,
                spareSpaceWidth = 50;

            return yScrollBarWidth + itemPerPageWidth + detailTextWidth
                + numberListMargin + prevNextButtonWidth
                + pageNumberButtonWidth * numberOfPages
                + spareSpaceWidth;
        }

        return 0;
    };
};