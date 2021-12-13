app.controller('yarnBreedListCtrl', function ($scope, $http) {
    DevExpress.localization.locale('tr');

    // LIST FUNCTIONS
    $scope.loadReport = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $.getJSON(HOST_URL + 'YarnBreed/GetYarnBreedList', function (data) {

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
                { dataField: 'YarnBreedCode', caption: 'İplik Cins Kodu' },
                { dataField: 'YarnBreedName', caption: 'İplik Cins Adı' },
                {
                    type: "buttons",
                    buttons: [
                        {
                            name: 'preview', cssClass: '', text: 'Düzenle', onClick: function (e) {
                                var dataGrid = $("#dataList").dxDataGrid("instance");
                                dataGrid.deselectAll();
                                dataGrid.selectRowsByIndexes([e.row.rowIndex]);

                                window.location.href = HOST_URL + 'YarnBreed?rid=' + e.row.data.Id;
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
