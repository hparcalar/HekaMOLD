app.controller('conditionalApprovalsCtrl', function ($scope, $http) {
    DevExpress.localization.locale('tr');

    // LIST FUNCTIONS
    $scope.loadReport = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $.getJSON(HOST_URL + 'ProductQuality/GetConditionalApprovedList', function (data) {

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
                { dataField: 'CreatedDateStr', caption: 'Tarih' },
                { dataField: 'ItemNo', caption: 'Ürün Kodu' },
                { dataField: 'ItemName', caption: 'Ürün Adı' },
                { dataField: 'MachineCode', caption: 'Makine' },
                { dataField: 'QualityExplanation', caption: 'Açıklama' },
                { dataField: 'FirstQuantity', caption: 'Miktar' },
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
                        column: "FirstQuantity",
                    summaryType: "sum",
                }
                ]
            }
        });
    }

    // ON LOAD EVENTS
    $scope.loadReport();
});
