app.controller('salesReportCtrl', function ($scope, $http) {
    DevExpress.localization.locale('tr');

    $scope.filterModel = {
        startDate: moment().format('DD.MM.YYYY'),
        endDate: moment().format('DD.MM.YYYY'),
    };

    // LIST FUNCTIONS
    $scope.loadReport = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $.getJSON(HOST_URL + 'ItemReceipt/GetSalesReport?dt1=' + $scope.filterModel.startDate +
                        '&dt2=' + $scope.filterModel.endDate, function (data) {

                        });
                },
                key: ['ItemId', 'FirmId']
            },
            showColumnLines: false,
            showRowLines: true,
            rowAlternationEnabled: true,
            allowColumnResizing: true,
            export: {
                enabled: true,
                allowExportSelectedData: true,
            },
            wordWrapEnabled: true,
            focusedRowEnabled: true,
            showBorders: true,
            filterRow: {
                visible: true
            },
            headerFilter: {
                visible: true
            },
            height: 700,
            groupPanel: {
                visible: true
            },
            editing: {
                allowUpdating: false,
                allowDeleting: false
            },
            columns: [
                { dataField: 'ItemNo', caption: 'Ürün Kodu' },
                { dataField: 'ItemName', caption: 'Ürün Adı' },
                { dataField: 'ItemGroupName', caption: 'Ürün Grubu' },
                { dataField: 'FirmName', caption: 'Müşteri' },
                { dataField: 'Quantity', caption: 'Adet' },
                { dataField: 'ConsumptionItemName', caption: 'Hammadde Adı' },
                { dataField: 'ConsumptionWeight', caption: 'Tüketilen Hammadde Kg' },
            ],
            summary: {
                totalItems: [{
                    column: "Quantity",
                    summaryType: "sum",
                }, {
                    column: "ConsumptionWeight",
                    summaryType: "sum",
                },
                ]
            }
        });
    }

    // ON LOAD EVENTS
    $scope.loadReport();
});
