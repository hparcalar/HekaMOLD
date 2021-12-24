app.controller('scrapListCtrl', function ($scope, $http) {
    DevExpress.localization.locale('tr');

    // LIST FUNCTIONS
    $scope.loadReport = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $.getJSON(HOST_URL + 'ProductQuality/GetScrapList', function (data) {

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
