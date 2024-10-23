declare var Promise: any;
declare var numeral: any;

interface IReportGrid {
    buildBodyRows( reportData: any ): Array<any>;

    buildFooterRows( reportData: any ): Array<any>;

    getDefaultColumns(): Array<any>

    getGroupHeaderColumns(): Array<any>;

    getGridOptions(): any;

    setGridOptions( gridOptions: any );

    fanexGrid( gridOptions: any ): void;

    hideNoInfo( isHide: boolean );

    getGridOptions(): string;
}

type IGridComponentConfig = {
    reportId: string, gridId: string, gridContainerId: string, isMobile: any,
    msgNoInfoId: string,
    selectColumnsId: string, selectColumnsContainerId: string,
    excelBtnId: string, pageSizeOptions: number[],
    copyBtnId?: string
}

const defaultOptions: IGridComponentConfig = {
    reportId: "",
    gridId: "",
    gridContainerId: "",
    selectColumnsId: "",
    selectColumnsContainerId: "",
    msgNoInfoId: "",
    isMobile: { phone: false },
    excelBtnId: "",
    pageSizeOptions: [ 10, 20, 50 ]
}

class BaseReportGrid implements IReportGrid {
    protected gridOptions: any;

    protected options: IGridComponentConfig = defaultOptions;

    protected gridBodyRowsCache: Array<any> = [];
    protected gridFooterRowsCache: Array<any> = [];

    constructor( options: IGridComponentConfig ) {
        this.options = options;
    }

    protected initSelectColumnsDropdown( columns: any, ignoreColumns: any ) {
        const options = [];
        const numberOfColumns = columns.length;
        for (let i = 0; i < numberOfColumns; i++) {
            if (ignoreColumns.includes(i)) continue;
            options.push({
                id: i.toString(),
                value: i.toString(),
                text: columns[i].ColumnName2,
                checked: true,
                enable: true
            })
        }

        $(`#${this.options.selectColumnsId}`).fanexMultiSelect({
            showFilter: true,
            minWidth: 280,
            checkAllText: "Select All",
            noneSelectedText: "None Selected",
            useCustomCheckAllText: true,
            customCheckAllText: "All",
            isCheckAll: true,
            listData: options,
            onLeave: () => this.showHideColumns()
        });
    }

    protected showHideColumns() {
        const unSelectedColumns = $(`#${this.options.selectColumnsId}`).fanexMultiSelect("getItems", false);
        $(`#${this.options.gridId}`).fanexGrid("hideColumns", this.gridOptions, unSelectedColumns.map(colIndex => Number(colIndex)));
    }

    buildBodyRows( reportData: any ): Array<any> {
        return undefined;
    }

    buildFooterRows( reportData: any ): Array<any> {
        return undefined;
    }

    getDefaultColumns(): Array<any> {
        return undefined;
    }

    getGridOptions(): any {
        return this.gridOptions;
    }

    setGridOptions( gridOptions: any ) {
        this.gridOptions = gridOptions;
    }

    getGroupHeaderColumns(): Array<any> {
        return undefined;
    }

    hideNoInfo( isHide: boolean ) {
        const msgNoInfo = $(`#${this.options.msgNoInfoId}`);
        const gridContainer = $(`#${this.options.gridContainerId}`);
        const selectColumnsContainer = $(`#${this.options.selectColumnsContainerId}`);
        if (isHide) {
            msgNoInfo.hide();
            gridContainer.show();
            selectColumnsContainer.show();
        } else {
            msgNoInfo.show();
            gridContainer.hide();
            selectColumnsContainer.hide();
        }
    }

    protected getDateTime( date: Date ): string {
        if (!date) return "";
        return moment(date).format("MM/DD/YYYY hh:mm A");
    }

    protected getDate( date: any ): string {
        if (!date) return "";
        return moment(date).format("MM/DD/YYYY");
    }

    fanexGrid( gridOptions: any ) {
        $(`#${this.options.gridId}`).fanexGrid(gridOptions);
        if (this.options.copyBtnId) {
            this.initCopyBtn(this.options.copyBtnId);
        }
    }

    protected initCopyBtn( copyBtnId: string ) {
        const defaultColumns: any = this.getDefaultColumns();
        const groupHeaderColumns: any = this.getGroupHeaderColumns();

        const _self = this;

        function getHidedColumnsInGroup( groupLength, startIndex ) {
            if (groupLength === 0 || startIndex >= defaultColumns.length || startIndex < 0) return [];
            const hideColumns = $(`#${_self.options.selectColumnsId}`).fanexMultiSelect("getItems", false);
            const hideColumnsName = hideColumns.map(colIndex => defaultColumns[colIndex].Name);
            let count = 0;
            const hidedColumnsInGroup = []
            while (count < groupLength) {
                const columnName = defaultColumns[startIndex + count].Name;
                if (hideColumnsName.includes(columnName)) {
                    hidedColumnsInGroup.push(columnName);
                }
                count++;
            }
            return hidedColumnsInGroup;
        }

        function formatValue( value, options ) {
            let currentValue = value;
            if (options.FormatNumber && !isNaN(currentValue) && currentValue !== '') {
                currentValue = numeral(currentValue).format(options.FormatNumber);
            }
            if (options.FormatNegative) {
                if (value < 0) {
                    currentValue = '<span style="color:red">' + options.FormatNegative.replace('$', currentValue) + '</span>';
                }
            }
            return currentValue;
        }

        function createHeader( hideColumnsName ) {
            const firstHeaderRow = [ '<tr>' ];
            for (const col of groupHeaderColumns) {
                if (col.MergeColNumber) {
                    const startIndex = defaultColumns.findIndex(e => e.Name === col.Name);
                    const hidedColumnsInGroup = getHidedColumnsInGroup(col.MergeColNumber, startIndex);
                    if (col.MergeColNumber === hidedColumnsInGroup.length) continue;
                    const colSpan = ` colspan=${col.MergeColNumber - hidedColumnsInGroup.length}`;
                    firstHeaderRow.push(`<th ${colSpan}>${col.HeaderText}</th>`);
                } else if (col.MergeRowNumber) {
                    if (hideColumnsName.includes(col.Name)) continue
                    const rowSpan = ` rowspan=${col.MergeRowNumber}`
                    firstHeaderRow.push(`<th ${rowSpan}>${col.HeaderText}</th>`);
                }
            }
            firstHeaderRow.push('</tr>');

            const secondHeaderRow = [ '<tr>' ];
            const filteredColumns = defaultColumns.filter(e => !hideColumnsName.includes(e.Name));
            for (const col of filteredColumns) {
                const groupHeaderInfo = groupHeaderColumns.find(e => e.Name === col.Name);
                if (groupHeaderInfo && groupHeaderInfo.MergeRowNumber) continue;
                secondHeaderRow.push(`<th>${col.HeaderText}</th>`);
            }
            secondHeaderRow.push('</tr>');

            return firstHeaderRow.join("") + secondHeaderRow.join("")
        }

        function createBody( hideColumnsName ) {
            const bodyRows = [];
            for (const row of _self.gridBodyRowsCache) {
                const tds = [];
                const cols = row.Columns.filter(col => !hideColumnsName.includes(col.Name));
                console.log("cols",cols)
                cols.forEach(col => {
                        const definedCol = defaultColumns.find(e => e.Name === col.Name);
                        tds.push(`<td>${formatValue(col.Value, definedCol)}</td>`)
                    }
                )
                bodyRows.push(`<tr>${tds.join("")}</tr>`)
            }
            return bodyRows.join("")
        }

        function createFooter( hideColumnsName ) {
            const footerRows = [];
            for (const row of _self.gridFooterRowsCache) {
                const tds = [];
                const cols = row.Columns.filter(col => !hideColumnsName.includes(col.Name));
                cols.forEach(col => {
                    const startIndex = defaultColumns.findIndex(e => e.Name === col.Name);
                    const hidedColumnsInGroup = getHidedColumnsInGroup(col.MergeColNumber, startIndex);
                    const colSpan = col.MergeColNumber ? ` colspan="${col.MergeColNumber - hidedColumnsInGroup.length}"` : '';
                    tds.push(`<td ${colSpan}>${formatValue(col.Value, defaultColumns[startIndex])}</td>`)
                })
                footerRows.push(`<tr>${tds.join("")}</tr>`)
            }
            return footerRows.join("");
        }

        function createTable() {
            const hideColumns = $(`#${_self.options.selectColumnsId}`).fanexMultiSelect("getItems", false);
            const hideColumnsName = hideColumns.map(colIndex => defaultColumns[colIndex].Name);
            const theadContent = `<thead>${createHeader(hideColumnsName)}</thead>`;
            const tbodyContent = _self.gridBodyRowsCache.length > 0 ? `<tbody>${createBody(hideColumnsName)}</tbody>` : '';
            const tFooterContent = _self.gridFooterRowsCache.length > 0 ? `<tfoot>${createFooter(hideColumnsName)}</tfoot>` : '';
            return `<table>${theadContent}${tbodyContent}${tFooterContent}</table>`;
        }

        function createHiddenTextArea( text ) {
            const textArea = document.createElement("textarea");
            textArea.value = text;
            textArea.style.position = "absolute";
            textArea.style.left = "-9999px";
            document.body.appendChild(textArea);
            return textArea;
        }

        const copyBtn = $(`#${copyBtnId}`).tooltip({ content: "Copy" }).off("click").on("click", function () {
            const tableHtml = createTable();

            const copyToClipboard = ( text ) => {
                if (window.navigator["clipboard"] && window.isSecureContext) {
                    return window.navigator["clipboard"].writeText(text);
                } else {
                    const textArea = createHiddenTextArea(text);
                    textArea.select();
                    const successful = document.execCommand('copy');
                    textArea.remove();
                    return successful ? Promise.resolve() : Promise.reject();
                }
            };

            copyToClipboard(tableHtml).then(() => {
                copyBtn.tooltip({ content: "Copied" }).tooltip("open");
                setTimeout(() => copyBtn.tooltip({ content: "Copy" }).tooltip("close"), 2000);
            }).catch(err => console.error("Copy failed:", err));
        });
    }
}