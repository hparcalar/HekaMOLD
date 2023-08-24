app.controller('productQualityPlanFormListCtrl', function sidebarCtrl($scope, $http) {
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
                    return $.getJSON(HOST_URL + 'ProductQuality/GetPlanFormList?dt1=' + $scope.filterModel.startDate +
                        '&dt2=' + $scope.filterModel.endDate, function (data) {
                            
                        });
                },
                key: 'Id'
            },
            showColumnLines: false,
            showRowLines: true,
            rowAlternationEnabled: true,
            focusedRowEnabled: true,
            showBorders: true,
            export: {
                enabled: true,
                allowExportSelectedData: true,
            },
            wordWrapEnabled: true,
            filterRow: {
                visible: true
            },
            headerFilter: {
                visible: true
            },
            paging: {
                enabled:true,
                pageSize: 20,
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
                { dataField: 'ControlDateStr', caption: 'Kontrol Tarihi' },
                { dataField: 'MachineCode', caption: 'Makine' },
                { dataField: 'ProductCode', caption: 'Ürün Kodu' },
                { dataField: 'ProductName', caption: 'Ürün Adı' },
                {
                    type: "buttons",
                    buttons: [
                        {
                            name: 'preview', cssClass: '', text: 'Düzenle', onClick: function (e) {
                                var dataGrid = $("#dataList").dxDataGrid("instance");
                                dataGrid.deselectAll();
                                dataGrid.selectRowsByIndexes([e.row.rowIndex]);

                                window.location.href = HOST_URL + 'ProductQuality/IndexData?rid=' + e.row.data.Id;
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
