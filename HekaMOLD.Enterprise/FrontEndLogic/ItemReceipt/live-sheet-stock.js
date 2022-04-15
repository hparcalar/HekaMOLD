app.controller('liveSheetStockCtrl', function ($scope, $http) {
    DevExpress.localization.locale('tr');

    // LIST FUNCTIONS
    $scope.loadReport = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $.getJSON(HOST_URL
                        + 'ItemReceipt/GetLiveSheetStock', function (data) {

                        });
                },
                key: 'Id'
            },
            showColumnLines: false,
            showRowLines: true,
            rowAlternationEnabled: true,
            focusedRowEnabled: true,
            allowColumnResizing: true,
            wordWrapEnabled: true,
            export: {
                enabled: true,
                allowExportSelectedData: true,
            },
            showBorders: true,
            filterRow: {
                visible: true
            },
            headerFilter: {
                visible: true
            },
            paging: {
                enabled: true,
                pageSize: 13,
                pageIndex: 0
            },
            groupPanel: {
                visible: true
            },
            editing: {
                allowUpdating: false,
                allowDeleting: false
            },
            columns: [
                { dataField: 'ItemQualityGroupCode', caption: 'Kalite' },
                { dataField: 'SheetThickness', caption: 'Kalınlık' },
                { dataField: 'SheetHeight', caption: 'Ebat Boy' },
                { dataField: 'SheetWidth', caption: 'Ebat En' },
                { dataField: 'TotalOverallQuantity', caption: 'Tabaka Adet', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'TotalOverallWeight', caption: 'Toplam Kg', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'AvgWeightPrice', caption: 'Kg Fiyatı', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'TotalPrice', caption: 'Toplam Tutar', format: { type: "fixedPoint", precision: 2 } },
            ]
        });
    }

    // ON LOAD EVENTS
    $scope.loadReport();
});
