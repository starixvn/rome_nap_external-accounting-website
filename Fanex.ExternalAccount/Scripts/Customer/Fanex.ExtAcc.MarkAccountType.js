if(typeof Fanex === 'undefined' || typeof Fanex.ExtAcc === 'undefined') {
    alert("Fanex.ExtAcc.js module is required!");
}

Fanex.ExtAcc.MarkAccountType = {
    AccountTypes: [{ Id: 0, Name: "All" }, { Id: 1, Name: "VIP" }, { Id: 2, Name: "Suspicious" }, { Id: 3, Name: "Problem" }],
    cachedAccountTypes : [],
    DOMElements: {},
    cachedParams: {
        pageSize: 50,
        pageIndex: 0,
        selLevel: 0,
        selType: 0
    },
    Init: function () {
        this.DOMElements.txtUsername = $('#txtUserName');
        this.DOMElements.selLevel = $('#selLevel');
        this.DOMElements.selType = $('#selType');
        this.DOMElements.btnSearch = $('#btnSearch');
        this.DOMElements.btnMarkAccountType = $('#btnMarkAccountType');
        this.DOMElements.markaccountListGrid = $('#markaccountListGrid');
        this.DOMElements.jqgrid = $('#jqgrid');
        this.DOMElements.msgNoinfo = $('#msgNoinfo');
        this.DOMElements.editMult = $('#editMult');
        this.BindingEvents();
        this.DOMElements.btnSearch.click();
    },

    BindingEvents: function () {
        var $this = this;
        this.DOMElements.btnSearch.off().click(function () {
            $this.SubmitSearch($this.cachedParams.selLevel, $this.cachedParams.selType, undefined, 0);
        });

        this.DOMElements.txtUsername.keypress(function (e) {
            var code = e.keyCode ? e.keyCode : e.which;
            if (code === 13) {
                $this.DOMElements.btnSearch.click();
            }
        });

        this.DOMElements.btnMarkAccountType.off().click(function () {
            top.shouldCalculatePopupHeight = false;
            Fanex.ExtAcc.Form.Show(resolveClientUrl('MarkAccountType/MarkAccount'), 'Mark Account', 470, 410, false);
        });
    },

    SubmitSearch: function (selLevel, selType, itemsPerPage, currentIndex) {
        var $this = Fanex.ExtAcc.MarkAccountType;
        var pageSize = !isNullOrUndefined(itemsPerPage) ? itemsPerPage : $this.cachedParams.pageSize;
        var pageIndex = !isNullOrUndefined(currentIndex) ? currentIndex : $this.cachedParams.pageIndex;

        $this.cachedParams.pageIndex = pageIndex;
        $this.cachedParams.pageSize = pageSize;

        Fanex.ExtAcc.BlockUI();

        var params = {
            groupName: $('#selectedGroupName').html() === '' ? 'ACCDDD' : $('#selectedGroupName').html(),
            groupId: $('#ddlGroupName').val(),
            userName: Fanex.encodeHtml(encodeURIComponent($.trim(this.DOMElements.txtUsername.val()))).replace(/[^a-zA-Z0-9~!*()-._]/g, ""),
            level: this.DOMElements.selLevel.val(),
            type: this.DOMElements.selType.val(),
            pageSize: pageSize,
            pageIndex: pageIndex
        };

        $this.cachedParams.previousParams = params;
        $this.Search(params);
    },

    ReloadAfterEdit: function () {
        var $this = Fanex.ExtAcc.MarkAccountType;
        $this.Search($this.cachedParams.previousParams);
    },

    Search: function (params) {
        $.post(resolveClientUrl('MarkAccountType/Seach'), params, function (result) {
            var $this = Fanex.ExtAcc.MarkAccountType;
            $this.cachedAccountTypes = result.Customers;
            var bodyRows = [];
            var rowLength = result.Customers.length;
            for (var currentIndex = 0; currentIndex < rowLength; currentIndex++) {
                var currentRow = result.Customers[currentIndex];
                var columns = [];
                columns.push({ Name: 'RowNumber', Value: currentIndex + params.pageSize * params.pageIndex + 1 });
                columns.push({ Name: 'Check', Value: currentRow });
                columns.push({ Name: 'Username', Value: currentRow.Username });
                columns.push({ Name: 'Level', Value: currentRow.Level });
                columns.push({ Name: 'Type', Value: currentRow });
                columns.push({ Name: 'MarkedDate', Value: currentRow.MarkDateTime });
                columns.push({ Name: 'Remark', Value: currentRow });
                columns.push({ Name: 'Action', Value: currentRow });
                bodyRows.push({ Columns: columns});
            }

            var headers = [];

            headers.push({ HeaderText: 'No.', Name: 'RowNumber', Width: 25, CellAlign: 'center' });
            headers.push({
                HeaderText: '', Name: 'Check', Width: 30, CellAlign: 'center',
                CustomHeaderRender: function (cell) {
                    return '<div id="editMult" class="editMult"></div>';
                },
                CustomCellRender: function (cell) {
                    return `<div onclick="Fanex.ExtAcc.MarkAccountType.Select(this)" id=ckbEdit${cell.Value.CustId} style="margin-left: 0px !important" data-cust-id="${cell.Value.CustId}" data-username="${cell.Value.Username}" class="menuInactive"></div>`;
                }
            });
            headers.push({ HeaderText: 'Username', Name: 'Username', Width: 100, CellAlign: 'center' });
            headers.push({ HeaderText: 'Level', Name: 'Level', Width: 50, CellAlign: 'center' });
            headers.push({
                HeaderText: 'Type', Name: 'Type', Width: 100, CellAlign: 'center',
                CustomCellRender: function (cell) {
                    return `<div id="sglTypeId${cell.Value.CustId}" class="line-viewing"><div class="view-item">${cell.Value.Type}</div><select id="selType${cell.Value.CustId}" class="edit-item">'${Fanex.ExtAcc.MarkAccountType.BuildEditTypeOption(cell.Value.Type)}</select> </div>`;
                }
            });
            headers.push({ HeaderText: 'MarkedDate', Name: 'MarkedDate', Width: 180, CellAlign: 'center' });
            headers.push({
                HeaderText: 'Remark', Name: 'Remark', Width: 200, CellAlign: 'center',
                CustomCellRender: function (cell) {
                    return `<div id="sglRemark${cell.Value.CustId}" class="line-viewing">`
                        + `<div class="view-item">${cell.Value.Remark}</div>`
                        + `<input id="inputRemark${cell.Value.CustId}" type="text" class="edit-item" value="${cell.Value.Remark}" />`
                        + `</div>`;
                }
            });
            headers.push({
                HeaderText: 'Action', Name: 'Action', Width: 50, CellAlign: 'center',
                CustomCellRender: function (cell) {
                    return `<div id="sglAction${cell.Value.CustId}" class="line-viewing mark-account-grid-action">`
                        + `<div class="icon-action iconEnableEdit view-item" onclick="Fanex.ExtAcc.MarkAccountType.EditSingle(this)" data-cust-id="${cell.Value.CustId}"></div><div class="icon-action icon-delete view-item" onclick="Fanex.ExtAcc.MarkAccountType.UnmarkAccountType(this)" data-cust-id="${cell.Value.CustId}" data-username="${cell.Value.Username}"></div>`
                        + `<div class="icon-action edit-item icon-save" data-cust-id="${cell.Value.CustId}" onclick="Fanex.ExtAcc.MarkAccountType.UpdateSingle(this)"></div><div class="icon-action edit-item icon-arrow-left-bold" onclick="Fanex.ExtAcc.MarkAccountType.CancelEdit(this)" data-cust-id="${cell.Value.CustId}"></div>`
                        + `</div>`;
                }
            });

            var gridOptions = {
                columns: headers,
                bodyRows: bodyRows,
                autoCalculateGridExpandHeight: true,
                bodyRowHeight: isMobile.phone ? 28 : 25,
                fullHeight: !isMobile.phone,
                gridHeight: isMobile.phone ? $(window).height() - 70 : 0,
                highlightFrozenWidth: 2,
                showPager: true,
                customPager: true,
                showNumberColumn: true,
                pagerOption: {
                    itemsPerPageArray: [50, 100, 500],
                    currentPage: result.PageIndex,
                    totalItem: result.TotalRows,
                    itemsPerPage: params.pageSize,
                    pagerSelectCallBack: function (currentPageIndex, itemsPerPage) {
                        Fanex.ExtAcc.MarkAccountType.DisableMultipleContextMenu();
                        $this.cachedParams.pageIndex = currentPageIndex;
                        $this.cachedParams.pageSize = itemsPerPage;
                        $this.SubmitSearch(params.selLevel, params.selType);
                    }
                },
                marginRight: 12,
                showDistinctRow: false,
                allowTruncate: true
            };

            if (bodyRows.length === 0) {
                $this.DOMElements.msgNoinfo.css('display', 'block');
                $this.DOMElements.jqgrid.css('display', 'none');
            } else {
                $this.DOMElements.msgNoinfo.css('display', 'none');
                $this.DOMElements.jqgrid.css('display', 'block');
                setPagerOptionOnMobile(gridOptions);
                $this.DOMElements.jqgrid.fanexGrid(gridOptions);

                workaroundForGridOnPhone();
            }

            $this.DOMElements.editMult.attr('style', 'cursor: default !important');
            $this.DOMElements.editMult.attr('onclick', '');
            $this.DOMElements.editMult.contextMenu(false);
            $('#editMult').contextMenu(false);
        }).always(function () { Fanex.ExtAcc.UnBlockUI(); });
    },
    DisableMultipleContextMenu: function () {
        var $editMultDiv = $('#editMult');
        if ($editMultDiv.length) {
            $editMultDiv
                .attr('style', 'cursor: default !important')
                .attr('onclick', '')
                .contextMenu(false);
        }
    },

    BuildEditTypeOption: function (value) {
        var options = "";
        Fanex.ExtAcc.MarkAccountType.AccountTypes.forEach(function(accountType) {
            var selected = "";
            if (accountType.Id === value) {
                selected = "selected";
            }

            options += `<option value="${accountType.Id}" ${selected}>${accountType.Name}</option>`;
        });

        return options;
    },

    Add: function () {
        var $this = Fanex.ExtAcc.MarkAccountType;
        if (!Validators.validateInput($('#TypeId').val())) {
            $('#TypeId').focus();
            Validators.showDown($('#TypeId').get(0), 'This field is required', 0, 10, 41, -10, 15);
            return false;
        }

        if (!Validators.validateInput($('#Usernames').val())) {
            $('#Usernames').focus();
            Validators.showDown($('#Usernames').get(0), 'This field is required', 0, 10, 41, -10, 15);
            return false;
        }

        Fanex.ExtAcc.BlockUI();

        var groupName = $('#selectedGroupName').html() === '' ? 'ACCDDD' : $('#selectedGroupName').html();
        var typeId = $('#TypeId').val();
        var usernames = $('#Usernames').val();
        var remark = $('#Remark').val();

        var editUrl = 'MarkAccountType/Add';
        var params = new Object();

        params.groupName = groupName;
        params.typeId = typeId;
        params.usernames = Fanex.ExtAcc.EscapeHtml(usernames);
        params.remark = Fanex.ExtAcc.EscapeHtml(remark);

        $.ajax({ url: resolveClientUrl(editUrl), data: params, type: 'POST' })
            .done(function (data) {
                if (!data.IsSuccess) {
                    Fanex.ExtAcc.UnBlockUI();

                    $('#errMsg').removeClass('successMsg');
                    $('#errMsg').addClass('errMsg');
                    $('#errMsg').css('display', 'block');
                    $('#errMsg').html(data.ErrorMessage);
                }
                else {
                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);

                        Fanex.ExtAcc.Form.Close(false);

                        $this.Search($this.cachedParams.previousParams);
                    }, 2000);
                }
            });
    },

    EditSingle: function (div) {
        var custId = $(div).data("custId");
        $("#sglTypeId" + custId).removeClass("line-viewing");
        $("#sglRemark" + custId).removeClass("line-viewing");
        $("#sglAction" + custId).removeClass("line-viewing");
        $("#sglTypeId" + custId).addClass("line-editting");
        $("#sglRemark" + custId).addClass("line-editting");
        $("#sglAction" + custId).addClass("line-editting");
    },

    CancelEdit: function (div) {
        var custId = $(div).data("custId");
        $("#sglTypeId" + custId).removeClass("line-editting");
        $("#sglRemark" + custId).removeClass("line-editting");
        $("#sglAction" + custId).removeClass("line-editting");
        $("#sglTypeId" + custId).addClass("line-viewing");
        $("#sglRemark" + custId).addClass("line-viewing");
        $("#sglAction" + custId).addClass("line-viewing");
    },

    UpdateSingle: function (div) {
        var $this = Fanex.ExtAcc.MarkAccountType;
        var custId = $(div).data("custId");
        var params = {
            custId: custId,
            typeId: $("#selType" + custId).val(),
            remark: $("#inputRemark" + custId).val()
        };

        $('#errUpdateMsg').css('display', 'hidden');
        $('#errUpdateMsg').html("");

        Fanex.ExtAcc.BlockUI();
        $.ajax({ url: resolveClientUrl("MarkAccountType/Update"), data: params, type: 'POST' })
            .done(function (data) {
                if (!data.IsSuccess) {
                    Fanex.ExtAcc.UnBlockUI();

                    $('#errUpdateMsg').removeClass('successMsg');
                    $('#errUpdateMsg').addClass('errMsg');
                    $('#errUpdateMsg').css('display', 'block');
                    $('#errUpdateMsg').html(data.ErrorMessage);
                }
                else {
                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);
                        $this.Search($this.cachedParams.previousParams);
                    }, 2000);
                }
            });

    },

    UnmarkAccountType: function (div) {
        var $this = Fanex.ExtAcc.MarkAccountType;
        var custId = $(div).data("custId");
        var username = $(div).data("username");

        var params = {
            custId: custId
        };

        $('#errUpdateMsg').css('display', 'hidden');
        $('#errUpdateMsg').html("");

        Fanex.ExtAcc.Form.ConfirmBox("Unmark Account Type", `Are you going to unmark account type of ${username}?`, 450, "auto", false, function () { return $this.Unmark(params); }, null, "unmark-modal");
    },

    Unmark: function (params) {
        Fanex.ExtAcc.Form.Close(false, false, "unmark-modal");
        var $this = Fanex.ExtAcc.MarkAccountType;
        Fanex.ExtAcc.BlockUI();
        $.ajax({ url: resolveClientUrl("MarkAccountType/Unmark"), data: params, type: 'POST' })
            .done(function (data) {
                if (!data.IsSuccess) {
                    Fanex.ExtAcc.UnBlockUI();

                    $('#errUpdateMsg').removeClass('successMsg');
                    $('#errUpdateMsg').addClass('errMsg');
                    $('#errUpdateMsg').css('display', 'block');
                    $('#errUpdateMsg').html(data.ErrorMessage);
                }
                else {
                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);
                        $this.Search($this.cachedParams.previousParams);
                    }, 2000);
                }
            });
    },

    UnmarkAll: function (custIds) {
        Fanex.ExtAcc.Form.Close(false, false, "unmark-modal");
        var $this = Fanex.ExtAcc.MarkAccountType;
        var params = {
            CustIds: custIds
        };
        Fanex.ExtAcc.BlockUI();
        $.ajax({ url: resolveClientUrl("MarkAccountType/UnmarkAll"), data: params, type: 'POST' })
            .done(function (data) {
                if (!data.IsSuccess) {
                    Fanex.ExtAcc.UnBlockUI();

                    $('#errUpdateMsg').removeClass('successMsg');
                    $('#errUpdateMsg').addClass('errMsg');
                    $('#errUpdateMsg').css('display', 'block');
                    $('#errUpdateMsg').html(data.ErrorMessage);
                }
                else {
                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);
                        $this.Search($this.cachedParams.previousParams);
                    }, 2000);
                }
            });
    },

    UnmarkMultiple: function () {
        var $this = Fanex.ExtAcc.MarkAccountType;
        var custIdAndNames = Fanex.ExtAcc.MarkAccountType.IdsAndNames();
        Fanex.ExtAcc.Form.ConfirmBox("Unmark Account Type", `Selected account(s):<b>${custIdAndNames.Names}</b></br>Are you going to unmark account type of the following accounts?`, 450, "auto", false, function () { return $this.UnmarkAll(custIdAndNames.CustIds); }, null, "unmark-modal");
    },

    IdsAndNames: function () {
        var custids = [];
        var divs = $('div.menuActive');
        var usernames = [];
        for (var i = 0; i < divs.length; i++) {
            var checkItem = $(divs[i]);
            if (!custids.contains(checkItem.data("custId"))) {
                custids.push(checkItem.data("custId"));
                usernames.push(checkItem.data("username"));
            }
        }

        return {
            CustIds: custids.join(),
            Names: usernames.join()
        };
    },

    SelectAll: function (className) {
        if (className === 'menuActive') {
            $('#editMult').attr('style', 'cursor: pointer !important');
            $('#editMult').attr('onclick', 'javascript:Fanex.ExtAcc.MarkAccountType.EditMultiple(this)');
            $('#editMult').contextMenu(true);
        } else {
            $('#editMult').attr('style', 'cursor: default !important');
            $('#editMult').attr('onclick', '');
            $('#editMult').contextMenu(false);
        }
        var divs = document.getElementsByTagName('div');

        for (var i = 0; i < divs.length; i++) {
            if (divs[i].id && (divs[i].className === 'menuActive' || divs[i].className === 'menuInactive')) {
                divs[i].className = className;
            }
        }

    },

    Select: function (div) {
        $('#editMult').attr('style', 'cursor: default !important');

        if (div.className === 'menuInactive') {
            div.className = 'menuActive';
            $('#editMult').attr('style', 'cursor: pointer !important');
            $('#editMult').attr('onclick', 'javascript:Fanex.ExtAcc.MarkAccountType.EditMultiple(this)');
            $('#editMult').contextMenu(true);
            
        } else {
            div.className = 'menuInactive';
            $('#editMult').attr('style', 'cursor: default !important');
            $('#editMult').attr('onclick', '');
            $('#editMult').contextMenu(false);
            
        }

        var divs = $('div');

        for (var i = 0; i < divs.length; i++) {
            if (divs[i].id === div.id && div.className === 'menuInactive') {
                divs[i].className = 'menuInactive';
            }
        }

        for (var i = 0; i < divs.length; i++) {
            if ((divs[i].className === 'menuActive' || divs[i].className === 'menuInactive') && divs[i].className === 'menuActive') {
                $('#editMult').attr('style', 'cursor: pointer !important');
                $('#editMult').attr('onclick', 'javascript:Fanex.ExtAcc.MarkAccountType.EditMultiple(this)');
                $('#editMult').contextMenu(true);

                break;
            }
        }
    },

    OpenMultipleUpdate: function () {
        Fanex.ExtAcc.Form.Show(resolveClientUrl('MarkAccountType/MultipleEditAccoutType'), 'Edit Account Type', 470, 410, false);
    },

    InitDataMultipleUpdate: function () {
        var $this = Fanex.ExtAcc.MarkAccountType;

        var bodyRows = [];
        for (var currentIndex = 0; currentIndex < 10; currentIndex++) {
            var currentRow = $this.cachedAccountTypes[0];
            var columns = [];
            columns.push({ Name: 'RowNumber', Value: currentIndex + 1 });
            columns.push({ Name: 'Username', Value: currentRow.Username });
            columns.push({ Name: 'Type', Value: currentRow });
            columns.push({ Name: 'Remark', Value: currentRow });
            bodyRows.push({ Columns: columns });
        }

        var headers = [];

        headers.push({ HeaderText: 'No.', Name: 'RowNumber', Width: 25, CellAlign: 'center' });
        headers.push({ HeaderText: 'Username', Name: 'Username', Width: 100, CellAlign: 'center' });
        headers.push({
            HeaderText: 'Type', Name: 'Type', Width: 100, CellAlign: 'center',
            CustomCellRender: function (cell) {
                return `<select id="selMultipleType${cell.Value.CustId}" >${Fanex.ExtAcc.MarkAccountType.BuildEditTypeOption(cell.Value.Type)}</select>`;
            }
        });
        headers.push({
            HeaderText: 'Remark', Name: 'Remark', Width: 200, CellAlign: 'center',
            CustomCellRender: function (cell) {
                return `<input id="inputMultipleRemark${cell.Value.CustId}" type="text" value="${cell.Value.Remark}" />`;
            }
        });

        var gridOptions = {
            columns: headers,
            bodyRows: bodyRows,
            autoCalculateGridExpandHeight: true,
            bodyRowHeight: isMobile.phone ? 28 : 25,
            fullHeight: !isMobile.phone,
            gridHeight: isMobile.phone ? $(window).height() - 70 : 0,
            marginRight: 12,
            showDistinctRow: false,
            allowTruncate: true
        };

        $("#multiEditGrid").fanexGrid(gridOptions);
    }
};

Fanex.ExtAcc.MarkAccountType.EditMultiple = function (div) {
    var menuItems = {};
    
    menuItems.editMultipe = {
        name: "Edit Account Type",
        className: "iconEnableEdit",
        callback: function (key, opt) {
            top.shouldCalculatePopupHeight = false;
            Fanex.ExtAcc.Form.Show(resolveClientUrl('MarkAccountType/MultipleEditAccountType'), 'Edit Account Type', 470, 410, false);
        }
    };
    menuItems.unmarkMultiple = {
        name: "Unmark Account Type",
        className: "iconEnableEdit",
        callback: function (key, opt) {
            top.shouldCalculatePopupHeight = false;
            Fanex.ExtAcc.MarkAccountType.UnmarkMultiple();
        }
    };

    if ($.contextMenu) $.contextMenu('destroy');

    $.contextMenu({
        selector: '.editMult',
        items: menuItems,
        trigger: "left",
        ignoreRightClick: true,
        autoHide: true
    });
};