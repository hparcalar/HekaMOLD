app.controller('moldTestListCtrl', function ($scope, $http) {
    DevExpress.localization.locale('tr');

    // LIST FUNCTIONS
    $scope.loadReport = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $.getJSON(HOST_URL + 'MoldTest/GetMoldTestList', function (data) {
                            
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
                enabled:true,
                pageSize: 13,
                pageIndex:0
            },
            groupPanel: {
                visible: true
            },
            editing: {
                allowUpdating: false,
                allowDeleting: false
            },
            columns: [
                { dataField: 'DyeCode', caption: 'Renk Kodu' },
                { dataField: 'DyeName', caption: 'Renk Adı' },
                { dataField: 'RalCode', caption: 'RAL Kodu' },
                {
                    type: "buttons",
                    buttons: [
                        {
                            name: 'preview', cssClass: '', text: 'Düzenle', onClick: function (e) {
                                var dataGrid = $("#dataList").dxDataGrid("instance");
                                dataGrid.deselectAll();
                                dataGrid.selectRowsByIndexes([e.row.rowIndex]);

                                window.location.href = HOST_URL + 'Dye?rid=' + e.row.data.Id;
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
