app.controller('finishedProductStateCtrl', function ($scope, $http) {
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
                    return $.getJSON(HOST_URL + 'Warehouse/GetFinishedProductState?dt1=' + $scope.filterModel.startDate +
                        '&dt2=' + $scope.filterModel.endDate, function (data) {

                        });
                },
                key: ['ItemId']
            },
            showColumnLines: false,
            showRowLines: true,
            rowAlternationEnabled: true,
            allowColumnResizing: true,
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
                /*{ dataField: 'ItemNo', caption: 'Ürün Kodu' },*/
                { dataField: 'ItemName', caption: 'Ürün Adı' },
                { dataField: 'InQty', caption: 'Giriş Miktar' },
                { dataField: 'OutQty', caption: 'Çıkış Miktar' },
                { dataField: 'TotalQty', caption: 'Mevcut Miktar' },
            ],
            summary: {
                totalItems: [{
                    column: "InQty",
                    summaryType: "sum",
                }, {
                    column: "OutQty",
                    summaryType: "sum",
                },
                {
                    column: "TotalQty",
                    summaryType: "sum",
                }
                ]
            }
        });
    }

    // ON LOAD EVENTS
    $scope.loadReport();
});
