app.controller('costListCtrl', function sidebarCtrl($scope, $http) {
    DevExpress.localization.locale('tr');

    // LIST FUNCTIONS
    $scope.loadReport = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $.getJSON(HOST_URL + 'Cost/GetCostList', function (data) {

                    });
                },
                key: 'Id'
            },
            showColumnLines: false,
            showRowLines: true,
            rowAlternationEnabled: true,
            focusedRowEnabled: true,
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
                { dataField: 'CostCode', caption: 'Maliyet Kodu' },
                { dataField: 'CostName', caption: 'Maliyet Adı' },
                { dataField: 'Quantity', caption: 'Adet' },
                { dataField: 'UnitPrice', caption: 'Birim Fiyat' },
                { dataField: 'OverallTotal', caption: 'Maliyet Tutarı' },
                { dataField: 'UnitTypeCode', caption: 'Birim Kod' },
                { dataField: 'ForexTypeCode', caption: 'Doviz Kod' },
                { dataField: 'Explanation', caption: 'AÇıklama' },

                {
                    type: "buttons",
                    buttons: [
                        {
                            name: 'preview', cssClass: '', text: 'Düzenle', onClick: function (e) {
                                var dataGrid = $("#dataList").dxDataGrid("instance");
                                dataGrid.deselectAll();
                                dataGrid.selectRowsByIndexes([e.row.rowIndex]);

                                window.location.href = HOST_URL + 'Cost?rid=' + e.row.data.Id;
                            }
                        }
                    ]
                }
            ]
        });
    }

    // ON LOAD EVENTS
    $scope.loadReport();
});
