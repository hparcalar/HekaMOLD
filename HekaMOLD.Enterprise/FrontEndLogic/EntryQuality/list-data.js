app.controller('entryQualityPlanFormListCtrl', function sidebarCtrl($scope, $http) {
    DevExpress.localization.locale('tr');

    // LIST FUNCTIONS
    $scope.loadReport = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $.getJSON(HOST_URL + 'EntryQuality/GetPlanFormList', function (data) {
                            
                        });
                },
                key: 'Id'
            },
            showColumnLines: false,
            showRowLines: true,
            rowAlternationEnabled: true,
            focusedRowEnabled: true,
            showBorders: true,
            wordWrapEnabled: true,
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
                { dataField: 'ControlDateStr', caption: 'Kontrol Tarihi' },
                { dataField: 'FirmName', caption: 'Firma' },
                { dataField: 'ItemNo', caption: 'Malzeme Kodu' },
                { dataField: 'ItemName', caption: 'Malzeme Adı' },
                { dataField: 'ItemName', caption: 'Malzeme Adı' },
                { dataField: 'WaybillNo', caption: 'İrsaliye No' },
                { dataField: 'EntryQuantity', caption: 'Gelen Miktar', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'CheckedQuantity', caption: 'Kontrol Miktar', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                {
                    type: "buttons",
                    buttons: [
                        {
                            name: 'preview', cssClass: '', text: 'Düzenle', onClick: function (e) {
                                var dataGrid = $("#dataList").dxDataGrid("instance");
                                dataGrid.deselectAll();
                                dataGrid.selectRowsByIndexes([e.row.rowIndex]);

                                window.location.href = HOST_URL + 'EntryQuality/IndexData?rid=' + e.row.data.Id;
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
