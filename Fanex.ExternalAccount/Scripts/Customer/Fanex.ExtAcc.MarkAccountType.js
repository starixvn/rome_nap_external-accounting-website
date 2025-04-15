if(typeof Fanex === 'undefined' || typeof Fanex.ExtAcc === 'undefined') {
    alert("Fanex.ExtAcc.js module is required!");
}

Fanex.ExtAcc.MarkAccountType = {
    AccountTypes: [ { Id: 1, Name: "VIP" }, { Id: 2, Name: "Suspicious" }, { Id: 3, Name: "Problem" }],
    cachedAccountTypes: [],
    cachedEdittingAccountTypes: [],
    DOMElements: {},
    cachedParams: {
        pageSize: 50,
        pageIndex: 0,
        selLevel: 0,
        selType: 0
    },
    Init: function () {
        var $this = Fanex.ExtAcc.MarkAccountType;
        $this.DOMElements.txtUsername = $('#txtUserName');
        $this.DOMElements.selLevel = $('#selLevel');
        $this.DOMElements.selType = $('#selType');
        $this.DOMElements.btnSearch = $('#btnSearch');
        $this.DOMElements.btnMarkAccountType = $('#btnMarkAccountType');
        $this.DOMElements.markaccountListGrid = $('#markaccountListGrid');
        $this.DOMElements.jqgrid = $('#jqgrid');
        $this.DOMElements.selectBar = $('#selectBar');
        $this.DOMElements.msgNoinfo = $('#msgNoinfo');
        $this.DOMElements.editMult = $('#editMult');
        $this.BindingEvents();
        $this.DOMElements.btnSearch.click();

        var dataList = [];
        dataList.push({
            id: "optSuper",
            value: 4,
            text: "Super",
            checked: true,
            enable: true
        });
        dataList.push({
            id: "optMaster",
            value: 3,
            text: "Master",
            checked: true,
            enable: true
        });
        dataList.push({
            id: "optAgent",
            value: 2,
            text: "Agent",
            checked: true,
            enable: true
        });
        dataList.push({
            id: "Member",
            value: 1,
            text: "Member",
            checked: true,
            enable: true
        });

        $this.DOMElements.selLevel = $("#selLevel").fanexMultiSelect({
            minWidth: 220,
            checkAllText: "All",
            noneSelectedText: "Select none",
            useCustomCheckAllText: true,
            customCheckAllText: "All",
            isCheckAll: true,
            listData: dataList,
            onCheck: function () {
                $this.UpdateSearchBtnStatus();
            }
        });

        var typeList = [];
        typeList.push({
            id: "optVIP",
            value: 1,
            text: "VIP",
            checked: true,
            enable: true
        });
        typeList.push({
            id: "optSuspicious",
            value: 2,
            text: "Suspicious",
            checked: true,
            enable: true
        });
        typeList.push({
            id: "optProblem",
            value: 3,
            text: "Problem",
            checked: true,
            enable: true
        });

        $this.DOMElements.selType = $("#selType").fanexMultiSelect({
            minWidth: 220,
            checkAllText: "All",
            noneSelectedText: "Select none",
            useCustomCheckAllText: true,
            customCheckAllText: "All",
            isCheckAll: true,
            listData: typeList,
            onCheck: function () {
                $this.UpdateSearchBtnStatus();
            }
        });
        
    },

    UpdateSearchBtnStatus: function () {
        var $this = Fanex.ExtAcc.MarkAccountType;

        var level = $this.DOMElements.selLevel.fanexMultiSelect("getItems", true);
        var type = $this.DOMElements.selType.fanexMultiSelect("getItems", true);

        if (level.length && type.length) {
            $this.DOMElements.btnSearch.removeClass("-default");
            $this.DOMElements.btnSearch.addClass("-submit");
            $this.DOMElements.btnSearch.prop('disabled', false);
        } else {
            $this.DOMElements.btnSearch.removeClass("-submit");
            $this.DOMElements.btnSearch.addClass("-default");
            $this.DOMElements.btnSearch.prop('disabled', true);
        }
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
            Fanex.ExtAcc.Form.Show(resolveClientUrl('MarkAccountType/MarkAccount'), 'Mark Account', 470, "auto", false);
        });
    },

    BindingAddNewEvents: function () {
        $("#Usernames").on("paste", function (e) {
            var element = this;
            setTimeout(function () {
                var context = $(element).val();
                context = context.replace(/(\r\n|\n|\r)/gm, " ");
                context = context.trim().replace(/\s+/g, ",");
                $('#Usernames').empty().val(context);
            }, 100);
        });
    },

    SubmitSearch: function (selLevel, selType, itemsPerPage, currentIndex) {
        var $this = Fanex.ExtAcc.MarkAccountType;
        var pageSize = !isNullOrUndefined(itemsPerPage) ? itemsPerPage : $this.cachedParams.pageSize;
        var pageIndex = !isNullOrUndefined(currentIndex) ? currentIndex : $this.cachedParams.pageIndex;

        $this.cachedParams.pageIndex = pageIndex;
        $this.cachedParams.pageSize = pageSize;

        Fanex.ExtAcc.BlockUI();
        var level = $this.DOMElements.selLevel.fanexMultiSelect("getItems", true);
        var type = $this.DOMElements.selType.fanexMultiSelect("getItems", true);
        var params = {
            groupName: $('#selectedGroupName').html() === '' ? 'ACCDDD' : $('#selectedGroupName').html(),
            groupId: $('#ddlGroupName').val(),
            userName: Fanex.encodeHtml($.trim(this.DOMElements.txtUsername.val())),
            level: level.length ? level.join(",") : "",
            type: type.length ? type.join(",") : "",
            pageSize: pageSize,
            pageIndex: pageIndex
        };

        $this.Search(params);
    },

    ReloadAfterEdit: function () {
        var $this = Fanex.ExtAcc.MarkAccountType;
        $this.Search($this.cachedParams.previousParams);
    },

    Search: function (params) {
        var $this = Fanex.ExtAcc.MarkAccountType;
        $this.cachedParams.previousParams = params;
        $('#errUpdateMsg').html('');
        $('#errUpdateMsg').css('display', 'none');
        $.post(resolveClientUrl('MarkAccountType/Seach'), params, function (result) {
            if (result.errorMessage) {
                $('#errUpdateMsg').removeClass('successMsg');
                $('#errUpdateMsg').addClass('errMsg');
                $('#errUpdateMsg').css('display', 'block');
                $('#errUpdateMsg').html(result.errorMessage);

                $this.DOMElements.msgNoinfo.css('display', 'block');
                $this.DOMElements.jqgrid.css('display', 'none');

                return;
            }
            if (result.Customers.length === 0 && params.pageIndex >= 1) {
                params.pageIndex = params.pageIndex - 1;
                $this.Search(params);
                return;
            }

            var bodyRows = [];
            var rowLength = result.Customers.length;
            for (var currentIndex = 0; currentIndex < rowLength; currentIndex++) {
                var currentRow = result.Customers[currentIndex];

                currentRow.Remark = Fanex.ExtAcc.EscapeHtml(currentRow.Remark);
                var columns = [];
                columns.push({ Name: 'RowNumber', Value: currentIndex + params.pageSize * params.pageIndex + 1 });
                columns.push({ Name: 'Check', Value: currentRow });
                columns.push({ Name: 'Username', Value: currentRow});
                columns.push({ Name: 'Level', Value: $this.GetLevelName(currentRow.Level) });
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
            headers.push({
                HeaderText: 'Username', Name: 'Username', Width: 200, CellAlign: 'left', HeaderAlign: 'center',
                CustomCellRender: function (cell) {
                    return cell.Value.AgentId >= 0 ? cell.Value.Username : cell.Value.Username + '<span class="icon-directmember"></span>';
                }
            });
            headers.push({ HeaderText: 'Level', Name: 'Level', Width: 60, CellAlign: 'center' });
            headers.push({
                HeaderText: 'Type', Name: 'Type', Width: 100, CellAlign: 'center',
                CustomCellRender: function (cell) {
                    return `<div id="sglTypeId${cell.Value.CustId}" class="line-viewing"><div class="view-item">${$this.GetTypeName(cell.Value.TypeId)}</div><select id="selType${cell.Value.CustId}" class="edit-item">'${Fanex.ExtAcc.MarkAccountType.BuildEditTypeOption(cell.Value.TypeId)}</select> </div>`;
                }
            });
            headers.push({ HeaderText: 'Marked Date', Name: 'MarkedDate', Width: 180, CellAlign: 'center' });
            headers.push({
                HeaderText: 'Remark', Name: 'Remark', Width: 250, CellAlign: 'left', HeaderAlign: 'center', AllowTruncate: true,
                CustomCellRender: function (cell) {
                    return `<div id="sglRemark${cell.Value.CustId}" class="line-viewing">`
                        + `<div class="view-item" title="${cell.Value.Remark}">${cell.Value.Remark}</div>`
                        + `<textarea id="inputRemark${cell.Value.CustId}" type="text" class="edit-item width-250" maxLength=2000 >${cell.Value.Remark}</textarea>`
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
                $this.DOMElements.selectBar.css('display', 'none');
            } else {
                $this.DOMElements.msgNoinfo.css('display', 'none');
                $this.DOMElements.jqgrid.css('display', 'block');
                $this.DOMElements.selectBar.css('display', 'block');
                setPagerOptionOnMobile(gridOptions);
                $this.DOMElements.jqgrid.fanexGrid(gridOptions);

                workaroundForGridOnPhone();
            }

            $this.cachedAccountTypes = result.Customers;
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

    GetLevelName: function(level) {
        switch (level) {
            case 4:
                return "Super";
            case 3:
                return "Master";
            case 2:
                return "Agent";
            case 1:
                return "Member";
            default:
                return "";
        }
    },

    GetTypeName: function(type) {
        switch (type) {
            case 1:
                return "VIP";
            case 2:
                return "Suspicious";
            case 3:
                return "Problem";
            default:
                return "";
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
                    $('#errMsg').html(data.ErrorMessage || data.errorMessage);
                }
                else {
                    $('#errMsg').removeClass('errMsg');
                    $('#errMsg').addClass('successMsg');
                    $('#errMsg').css('display', 'block');
                    $('#errMsg').html("Accounts marked successfully!");

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

    CancelEdit: function (div, custId) {
        custId = custId || $(div).data("custId");
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
            remark: Fanex.ExtAcc.EscapeHtml($("#inputRemark" + custId).val())
        };

        var editAccount = $this.cachedAccountTypes.find(function (account) { return account.CustId === custId && account.TypeId.toString() === params.typeId && account.Remark === params.remark;});
        if (editAccount) {
            
            $('#errUpdateMsg').removeClass('successMsg');
            $('#errUpdateMsg').addClass('errMsg');
            $('#errUpdateMsg').css('display', 'block');
            $('#errUpdateMsg').html("There is no change to update.");
            $this.CancelEdit(null, custId);
            var timeout = setTimeout(function () {
                clearTimeout(timeout);
                $('#errUpdateMsg').css('display', 'none');
            }, 2000);
            return;
        }

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
                    $('#errUpdateMsg').html(data.ErrorMessage || data.errorMessage);
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
                    $('#errUpdateMsg').html(data.ErrorMessage || data.errorMessage);
                }
                else {
                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);
                        $this.Search($this.cachedParams.previousParams);
                    }, 2000);
                }
            });
    },

    UnmarkAll: function (accounts) {
        Fanex.ExtAcc.Form.Close(false, false, "unmark-modal");
        var $this = Fanex.ExtAcc.MarkAccountType;
        var params = {
            Accounts: accounts
        };
        Fanex.ExtAcc.BlockUI();
        $.ajax({ url: resolveClientUrl("MarkAccountType/UnmarkAll"), data: params, type: 'POST' })
            .done(function (data) {
                if (!data.IsSuccess) {
                    Fanex.ExtAcc.UnBlockUI();

                    $('#errUpdateMsg').removeClass('successMsg');
                    $('#errUpdateMsg').addClass('errMsg');
                    $('#errUpdateMsg').css('display', 'block');
                    $('#errUpdateMsg').html(data.ErrorMessage || data.errorMessage);
                }
                else {

                    $('#errUpdateMsg').removeClass('errMsg');
                    $('#errUpdateMsg').addClass('successMsg');
                    $('#errUpdateMsg').css('display', 'block');
                    $('#errUpdateMsg').html("Accounts unmarked successfully.");
                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);
                        $this.Search($this.cachedParams.previousParams);
                    }, 2000);
                }
            });
    },

    UnmarkMultiple: function () {
        var $this = Fanex.ExtAcc.MarkAccountType;

        $this.cachedEdittingAccountTypes = [];
        var usernames = [];

        for (var i = 0; i < $this.cachedAccountTypes.length; i++) {
            if ($("#ckbEdit" + $this.cachedAccountTypes[i].CustId).hasClass('menuActive')) {
                $this.cachedEdittingAccountTypes.push($this.cachedAccountTypes[i]);
                usernames.push($this.cachedAccountTypes[i].Username);
            }
        }

        Fanex.ExtAcc.Form.ConfirmBox("Unmark Account Type", `<div style="display:block;max-height: 300px;overflow-y: scroll;">Selected account(s): <b>${usernames.join(", ")}</b></div></br><div style="margin-top: 15px">Are you going to unmark account type of the following accounts?</div>`, 450, "auto", false, function () { return $this.UnmarkAll($this.cachedEdittingAccountTypes); }, null, "unmark-modal");
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

    InitDataMultipleUpdate: function () {
        var $this = Fanex.ExtAcc.MarkAccountType;

        $this.cachedEdittingAccountTypes = [];

        for (var i = 0; i < $this.cachedAccountTypes.length; i++) {
            if ($("#ckbEdit" + $this.cachedAccountTypes[i].CustId).hasClass('menuActive')) {
                $this.cachedEdittingAccountTypes.push($this.cachedAccountTypes[i]);
            }
        }

        var tableDiv = $("<table>").addClass("tbl-multiple-mark-account");
        tableDiv.append(`<thead><tr><th class="mul-edit-no">No.</th><th class="mul-edit-username">Username</th><th class="mul-edit-type">Type</th><th class="mul-edit-remark">Remark</th></tr></thead>`);

        var divBody = $("<tbody>");
        for (var currentIndex = 0; currentIndex < $this.cachedEdittingAccountTypes.length; currentIndex++) {
            var iconCopyType = "";
            var iconCopyRemark = "";
            if (currentIndex === 0) {
                iconCopyType = `<span id="mulTypeApplyAll" class="icon-note-multiple"></span>`;
                iconCopyRemark = `<span id="mulRemarkApplyAll" class="icon-note-multiple"></span>`;
            }
            var currentRow = $this.cachedEdittingAccountTypes[currentIndex];
            var divTR = $("<tr>");
            divTR.append(`<td class="text-center">${currentIndex + 1}</td>`);
            divTR.append(`<td>${currentRow.Username}</td>`);
            divTR.append(`<td><div class="mul-edit-row mul-edit-type"><select id="selMultipleType${currentRow.CustId}" class="select-mul-type">${Fanex.ExtAcc.MarkAccountType.BuildEditTypeOption(currentRow.TypeId)}</select>${iconCopyType}</div></td>`);
            divTR.append(`<td><div class="mul-edit-row mul-edit-remark"><input id="inputMultipleRemark${currentRow.CustId}" class="input-mul-remark" type="text" value="${currentRow.Remark}" />${iconCopyRemark}</div></td>`);
            divBody.append(divTR);
        }   

        tableDiv.append(divBody);
        
        $("#multiple-update-grid").append(tableDiv);

        $("#mulTypeApplyAll").on("click", function () {
            var typeSelected = $("#selMultipleType" + $this.cachedEdittingAccountTypes[0].CustId).val();
            $(".select-mul-type").each(function(_, selType){
                $(selType).val(typeSelected).trigger('change');
            });
        });

        $("#mulRemarkApplyAll").on("click", function () {
            var remarktxt = $("#inputMultipleRemark" + $this.cachedEdittingAccountTypes[0].CustId).val();
            $(".input-mul-remark").each(function (_, iptRemark) {
                $(iptRemark).val(remarktxt).trigger('change');
            });
        });
    },

    SubmitUpdateMultiple: function () {
        var $this = Fanex.ExtAcc.MarkAccountType;
        var changedAccounts = [];
        for (var i = 0; i < $this.cachedEdittingAccountTypes.length; i++) {
            var typeId = $("#selMultipleType" + $this.cachedEdittingAccountTypes[i].CustId).val();
            var remark = Fanex.ExtAcc.EscapeHtml($("#inputMultipleRemark" + $this.cachedEdittingAccountTypes[i].CustId).val());
            if ($this.cachedEdittingAccountTypes[i].TypeId.toString() !== typeId || $this.cachedEdittingAccountTypes[i].Remark.toString() !== remark) {
                $this.cachedEdittingAccountTypes[i].TypeId = typeId;
                $this.cachedEdittingAccountTypes[i].Remark = remark;
                changedAccounts.push($this.cachedEdittingAccountTypes[i]);
            }
        }
        
        if (changedAccounts.length === 0) {
            $('#errMultEditMsg').removeClass('successMsg');
            $('#errMultEditMsg').addClass('errMsg');
            $('#errMultEditMsg').css('display', 'block');
            $('#errMultEditMsg').html("There is no change to update.");
            return;
        }

        var params = {
            UpdateAccounts: changedAccounts
        };
        Fanex.ExtAcc.BlockUI();
        $.ajax({ url: resolveClientUrl("MarkAccountType/UpdateMultiple"), data: params, type: 'POST' })
            .done(function (data) {
                if (!data.IsSuccess) {
                    Fanex.ExtAcc.UnBlockUI();

                    $('#errMultEditMsg').removeClass('successMsg');
                    $('#errMultEditMsg').addClass('errMsg');
                    $('#errMultEditMsg').css('display', 'block');
                    $('#errMultEditMsg').html(data.ErrorMessage || data.errorMessage);
                }
                else {

                    $('#errMultEditMsg').removeClass('errMsg');
                    $('#errMultEditMsg').addClass('successMsg');
                    $('#errMultEditMsg').css('display', 'block');
                    $('#errMultEditMsg').html("Accounts updated successfully.");

                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);
                        Fanex.ExtAcc.Form.Close(false);
                        $this.Search($this.cachedParams.previousParams);
                    }, 2000);
                }
            });
    },
    ResetMultipleEdit: function () {
        var $this = Fanex.ExtAcc.MarkAccountType;
        $this.cachedEdittingAccountTypes = [];

        for (var i = 0; i < $this.cachedAccountTypes.length; i++) {
            if ($("#ckbEdit" + $this.cachedAccountTypes[i].CustId).hasClass('menuActive')) {
                $this.cachedEdittingAccountTypes.push($this.cachedAccountTypes[i]);
                $("#selMultipleType" + $this.cachedAccountTypes[i].CustId).val($this.cachedAccountTypes[i].TypeId).trigger('change');
                $("#inputMultipleRemark" + $this.cachedAccountTypes[i].CustId).val($this.cachedAccountTypes[i].Remark);
            }
        }
    }
};

Fanex.ExtAcc.MarkAccountType.EditMultiple = function (div) {
    var menuItems = {};
    
    menuItems.editMultipe = {
        name: "Edit Account Type",
        className: "iconEnableEdit",
        callback: function (key, opt) {
            top.shouldCalculatePopupHeight = false;
            Fanex.ExtAcc.Form.Show(resolveClientUrl('MarkAccountType/MultipleEditAccountType'), 'Edit Account Type', 730, 'auto', false);
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