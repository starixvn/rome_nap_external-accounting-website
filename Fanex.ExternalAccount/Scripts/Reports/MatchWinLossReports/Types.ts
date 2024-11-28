interface ReportSummaryParams {
    "FromDate": string;
    "ToDate": string;
    "Sport": string;
    "Leagues": string;
    "Page"?: number;
    "PageSize"?: number;
    "SortBy"?: string;
    "SortType"?: number;
    "Currencies"?: string;
    "BaseCurrency"?: string;
}

interface ReportDetailParams extends ReportSummaryParams {
    "Matches": string,
    "BetTypes": string,
}

type SortInfo = {
    readonly "sortBy": string;
    readonly "sortType": number;
}
type PageInfo = {
    readonly "page": number;
    readonly "pageSize": number;
}


