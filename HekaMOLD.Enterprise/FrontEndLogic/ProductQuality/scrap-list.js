app.controller('scrapListCtrl', function ($scope, $http) {
    DevExpress.localization.locale('tr');

    $scope.filterModel = {
        startDate: moment().add(-1, 'M').format('DD.MM.YYYY'),
        endDate: moment().format('DD.MM.YYYY'),
    };

    // LIST FUNCTIONS
    $scope.loadReport = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $.getJSON(HOST_URL + 'ProductQuality/GetScrapList?dt1=' + $scope.filterModel.startDate +
                        '&dt2=' + $scope.filterModel.endDate, function (data) {

                    });
                },
                key: ['Id']
            },
            showColumnLines: false,
            showRowLines: true,
            rowAlternationEnabled: true,
            allowColumnResizing: true,
            wordWrapEnabled: true,
            focusedRowEnabled: true,
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
            height: 500,
            //paging: {
            //    enabled:true,
            //    pageSize: 13,
            //    pageIndex:0
            //},
            groupPanel: {
                visible: true
            },
            editing: {
                allowUpdating: false,
                allowDeleting: false
            },
            columns: [
                { dataField: 'EntryDateStr', caption: 'Tarih' },
                { dataField: 'ProductCode', caption: 'Ürün Kodu' },
                { dataField: 'ProductName', caption: 'Ürün Adı' },
                { dataField: 'MachineCode', caption: 'Makine' },
                { dataField: 'Quantity', caption: 'Miktar' },
                { dataField: 'Explanation', caption: 'Açıklama' },
                //{
                //    type: "buttons",
                //    buttons: [
                //        {
                //            name: 'preview', cssClass: 'btn btn-sm btn-light-primary', text: 'Seriler', onClick: function (e) {
                //                var dataGrid = $("#dataList").dxDataGrid("instance");

                //            }
                //        }
                //    ]
                //}
            ],
            summary: {
                totalItems: [
                {
                    column: "Quantity",
                    summaryType: "sum",
                }
                ]
            }
        });
    }

    // ON LOAD EVENTS
    $scope.loadReport();
});
