app.controller('knitListCtrl', function sidebarCtrl($scope, $http) {
    DevExpress.localization.locale('tr');

    // LIST FUNCTIONS
    $scope.loadReport = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $.getJSON(HOST_URL + 'Knit/GetKnitList', function (data) {

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
                { dataField: 'ItemNo', caption: 'Desen No' },
                { dataField: 'TestNo', caption: 'Deneme No' },
                { dataField: 'QualityTypeName', caption: 'Kalite Türü' },
                { dataField: 'MachineName', caption: 'Makine' },
                //{ dataField: 'CrudeWidth', caption: 'Ham En' },
               // { dataField: 'CrudeGramaj', caption: 'Ham Gramaj' },
               // { dataField: 'ProductWidth', caption: 'Mamül En' },
              //  { dataField: 'ProductGramaj', caption: 'Mamül Gramaj' },
               // { dataField: 'WapWireCount', caption: 'Çözgü Tel Sayısı' },
                //{ dataField: 'MeterGramaj', caption: 'm² Gramaj' },
                { dataField: 'ItemCutTypeStr', caption: 'Kesme' },
                { dataField: 'ItemBulletTypeStr', caption: 'Kurşun' },
                { dataField: 'ItemApparelTypeStr', caption: 'Konfeksiyon' },
                { dataField: 'ItemDyeHouseTypeStr', caption: 'Boyahane' },
                { dataField: 'CombWidth', caption: 'Atkı En' },
                { dataField: 'WeftReportLength', caption: 'Atkı Rapor Boyu' },
                { dataField: 'WapReportLength', caption: 'Çözgü Rapor Boyu' },
              {
                    type: "buttons",
                    buttons: [
                        {
                            name: 'preview', cssClass: '', text: 'Düzenle', onClick: function (e) {
                                var dataGrid = $("#dataList").dxDataGrid("instance");
                                dataGrid.deselectAll();
                                dataGrid.selectRowsByIndexes([e.row.rowIndex]);

                                window.location.href = HOST_URL + 'Knit?rid=' + e.row.data.Id;
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
