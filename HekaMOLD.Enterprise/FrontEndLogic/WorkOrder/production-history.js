app.controller('productionHistoryCtrl', function ($scope, $http) {
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
                    return $.getJSON(HOST_URL + 'WorkOrder/GetProductionHistory?dt1=' + $scope.filterModel.startDate +
                        '&dt2=' + $scope.filterModel.endDate, function (data) {
                            
                        });
                },
                key: ['WorkOrderDetailId','WorkDateStr', 'ShiftCode']
            },
            showColumnLines: false,
            showRowLines: true,
            rowAlternationEnabled: true,
            allowColumnResizing: true,
            focusedRowEnabled: true,
            showBorders: true,
            filterRow: {
                visible: true
            },
            headerFilter: {
                visible: true
            },
            height:700,
            groupPanel: {
                visible: true
            },
            editing: {
                allowUpdating: false,
                allowDeleting: false
            },
            columns: [
                { dataField: 'WorkDateStr', caption: 'Tarih' },
                { dataField: 'ShiftCode', caption: 'Vardiya' },
                { dataField: 'UserName', caption: 'Personel' },
                { dataField: 'SaleOrderNo', caption: 'Sipariş No' },
                { dataField: 'ProductCode', caption: 'Ürün Kodu' },
                { dataField: 'ProductName', caption: 'Ürün Adı' },
                { dataField: 'MachineCode', caption: 'Makine' },
                { dataField: 'OrderQuantity', caption: 'Sipariş Miktar' },
                { dataField: 'CompleteQuantity', caption: 'Üretilen Miktar' },
                /*{ dataField: 'SerialCount', caption: 'Koli Adedi' },*/
                { dataField: 'WastageQuantity', caption: 'Fire Miktar' },
            ],
            summary: {
                totalItems: [{
                    column: "CompleteQuantity",
                    summaryType: "sum",
                }, {
                    column: "SerialCount",
                    summaryType: "sum",
                },
                {
                    column: "WastageQuantity",
                    summaryType: "sum",
                }
                ]
            }
            });
        }

    // ON LOAD EVENTS
    $scope.loadReport();
});
