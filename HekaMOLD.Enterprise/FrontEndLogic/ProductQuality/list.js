app.controller('productQualityPlanListCtrl', function sidebarCtrl($scope, $http) {
    DevExpress.localization.locale('tr');

    // LIST FUNCTIONS
    $scope.loadReport = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $.getJSON(HOST_URL + 'ProductQuality/GetPlanList', function (data) {
                            
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
                { dataField: 'OrderNo', caption: 'Sıra No', width:100 },
                { dataField: 'ProductQualityCode', caption: 'Malzeme/Hammadde Adı' },
                { dataField: 'CheckProperties', caption: 'Kontrol Edilecek Özellikler' },
                { dataField: 'PeriodType', caption: 'Periyot' },
                { dataField: 'AcceptanceCriteria', caption: 'Kabul / Red Kriteri' },
                { dataField: 'ControlDevice', caption: 'Cihaz' },
                { dataField: 'Method', caption: 'Yöntem' },
                { dataField: 'Responsible', caption: 'Sorumlu' },
                {
                    type: "buttons",
                    buttons: [
                        {
                            name: 'preview', cssClass: '', text: 'Düzenle', onClick: function (e) {
                                var dataGrid = $("#dataList").dxDataGrid("instance");
                                dataGrid.deselectAll();
                                dataGrid.selectRowsByIndexes([e.row.rowIndex]);

                                window.location.href = HOST_URL + 'ProductQuality?rid=' + e.row.data.Id;
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
