type DropDownInfo = {
    dropdownId: any; dropdownName: any;
    dropdownData?: any; defaultValue?: any;
    dropdownType?: "fanexMultiSelect" | "fanexMultiSelectSearchable" | "combobox" | "select2",
    onClosed?: any; allowRerender?: boolean,
    dropdownConfig?: any
    toggleWidth?: any;
    minWidth?: any;
};

type InputInfo = { inputName: string; inputId: string; inputType?: string; };
type TimeRange = { fromDate: string, toDate: string };
type CurrentValue = { name: string, currentValue: any, currentText?: string };
type FilterValue = {
    inputParams: Array<CurrentValue>,
    dropdownParams: Array<CurrentValue>,
    calendarParams: TimeRange
};

interface IReportFilters {
    renderCalendarPicker( options: any ): void;

    renderSingleDatePicker( options: any ): void;

    getCalendarValues(): TimeRange;

    registerInputComponent( inputInfo: InputInfo ): boolean;

    registerDropdownComponent( info: DropDownInfo ): boolean;

    getAllFilterValues(): FilterValue;

    isUnValidInputs(): boolean;
}

class BaseReportFilters implements IReportFilters {
    protected fromDate: string;
    protected toDate: string;
    protected inputIds: { inputName: string, inputId: string, value?: any }[] = [];
    protected dropdowns: DropDownInfo[] = [];
    protected $: JQueryStatic;

    constructor( $: JQueryStatic ) {
        this.$ = $;
    }

    registerInputComponent( inputInfo: { inputId: any; inputName: string; value?: any; } ) {
        if (!this.inputIds.filter(id => id.inputId === inputInfo.inputId).length) {
            this.inputIds.push(inputInfo);
            return true;
        }
        return false;
    }

    registerDropdownComponent( dropdownInfo: DropDownInfo ) {
        const isExist = this.dropdowns.filter(id => id.dropdownId === dropdownInfo.dropdownId).length;
        switch (dropdownInfo.dropdownType) {
            case "fanexMultiSelect":
                this.renderFanexMultiSelectDDL(dropdownInfo);
                !isExist && this.dropdowns.push(dropdownInfo);
                return true;
            case "fanexMultiSelectSearchable":
                this.renderFanexMultiSelectSearchableDDL(dropdownInfo);
                !isExist && this.dropdowns.push(dropdownInfo);
                return true;
            case "combobox":
                this.renderComboboxDDL(dropdownInfo);
                !isExist && this.dropdowns.push(dropdownInfo);
                return true;
            case "select2":
                this.renderSelect2DDL(dropdownInfo);
                !isExist && this.dropdowns.push(dropdownInfo);
                return true;
            default:
                throw new Error("Unknown dropdown type");
        }
    }

    getAllFilterValues() {
        const self = this;
        const inputParams = this.inputIds.map(function ( elem ) {
            return {
                name: elem.inputName,
                currentValue: self.$(`#${elem.inputId}`).val(),
            }
        })

        const getSelectedValueDropdown = ( dropdownInfo: DropDownInfo ) => {
            switch (dropdownInfo.dropdownType) {
                case "fanexMultiSelect":
                    return self.$(`#${dropdownInfo.dropdownId}`).fanexMultiSelect("getItems", true).toString();
                case "fanexMultiSelectSearchable":
                    return self.$(`#${dropdownInfo.dropdownId}`).fanexMultiSelectSearchable("getItems", true).toString();
                case "combobox":
                case "select2":
                    return self.$(`#${dropdownInfo.dropdownId}`).find(":selected").val();
                default:
                    throw new Error("Unknown dropdown type");
            }
        }
        const getSelectedTextDropdown = ( dropdownInfo: DropDownInfo ) => {
            switch (dropdownInfo.dropdownType) {
                case "fanexMultiSelect":
                    return self.$(`#${dropdownInfo.dropdownId}`).fanexMultiSelect("getSelectedTexts", true).toString();
                case "fanexMultiSelectSearchable":
                    return self.$(`#${dropdownInfo.dropdownId}`).fanexMultiSelectSearchable("getSelectedTexts", true).toString();
                case "combobox":
                case "select2":
                    return self.$(`#${dropdownInfo.dropdownId}`).find(":selected").text();
                default:
                    throw new Error("Unknown dropdown type");
            }
        }

        const dropdownParams = this.dropdowns.map(function ( elem ) {
            return {
                name: elem.dropdownName,
                currentValue: getSelectedValueDropdown(elem),
                currentText: getSelectedTextDropdown(elem),
            }
        })

        return {
            inputParams,
            dropdownParams,
            calendarParams: this.getCalendarValues(),
        }
    }

    isUnValidInputs() {
        const { inputParams } = this.getAllFilterValues();
        return inputParams.some(e => e.currentValue.includes("script"));
    }

    getCalendarValues(): { fromDate: string; toDate: string } {
        return { fromDate: this.fromDate, toDate: this.toDate };
    }

    renderCalendarPicker( options: any ): void {
        throw new Error("Method not implemented.");
    }

    renderSingleDatePicker( options: any ): void {
        throw new Error("Method not implemented.");
    }

    renderFanexMultiSelectDDL( options: DropDownInfo ) {
        if (!options.dropdownData || !options.dropdownId) return;
        const { dropdownConfig } = options;
        const { checkAllText, noneSelectedText } = dropdownConfig || { checkAllText: "Select All", noneSelectedText: "None Selected" };
        return this.$(`#${options.dropdownId}`).fanexMultiSelect({
            showFilter: true,
            checkAllText: checkAllText,
            noneSelectedText: noneSelectedText,
            useCustomCheckAllText: false,
            customCheckAllText: "All",
            isCheckAll: true,
            listData: options.dropdownData,
            minWidth: options.minWidth,
            toggleWidth: options.toggleWidth,
            onLeave: function () {
                const leagueIDs = $(`#${options.dropdownId}`).fanexMultiSelect("getItems", true).toString();
                if (options.onClosed) {
                    options.onClosed(leagueIDs);
                }
            },
        });
    }

    renderFanexMultiSelectSearchableDDL( options: DropDownInfo ) {
        if (!options.dropdownData || !options.dropdownId) return;
        const { dropdownConfig } = options;
        const { checkAllText, noneSelectedText } = dropdownConfig || { checkAllText: "Select All", noneSelectedText: "None Selected" };
        return this.$(`#${options.dropdownId}`).fanexMultiSelectSearchable({
            showFilter: true,
            checkAllText: checkAllText,
            noneSelectedText: noneSelectedText,
            useCustomCheckAllText: false,
            customCheckAllText: "All",
            isCheckAll: true,
            listData: options.dropdownData,
            minWidth: options.minWidth,
            toggleWidth: options.toggleWidth,
            onLeave: function () {
                const leagueIDs = $(`#${options.dropdownId}`).fanexMultiSelectSearchable("getItems", true).toString();
                if (options.onClosed) {
                    options.onClosed(leagueIDs);
                }
            },
        });
    }

    renderComboboxDDL( options: DropDownInfo ) {
        if (!options.dropdownId) return;
        const $dropdownList = this.$(`#${options.dropdownId}`).combobox();
        $dropdownList.next("input.ui-autocomplete-input")
            .val($dropdownList.find("option:selected").text())
            .attr("readonly", "true")
            .bind("click", event => this.$(event.currentTarget).autocomplete("search", ""));
    }

    renderSelect2DDL( options: DropDownInfo ) {
        if (!options.dropdownId) return;
        this.$(`#${options.dropdownId}`).empty().select2({
            placeholder: "Select",
            data: options.dropdownData,
            dropdownAutoWidth: true, // Optional to auto-adjust the dropdown width
            templateSelection: function ( data ) {
                return data.text; // Customize how selected items are shown
            },
            sorter: function ( data ) {
                return data; // Keep original order, no sorting
            }
        }).bind("select2:select", () => {
            if (options.onClosed) {
                options.onClosed(this.$(`#${options.dropdownId}`).val())
            }
        })
    }

}
