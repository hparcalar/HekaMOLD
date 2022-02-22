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
            allowColumnReordering: true,
            allowColumnResizing: true,
            columnAutoWidth: true,
            export: {
                enabled: true,
                allowExportSelectedData: true,
            },
            onExporting(e) {
                const workbook = new ExcelJS.Workbook();
                const worksheet = workbook.addWorksheet(e.data);

                DevExpress.excelExporter.exportDataGrid({
                    component: e.component,
                    worksheet,
                    autoFilterEnabled: true,
                }).then(() => {
                    workbook.xlsx.writeBuffer().then((buffer) => {
                        saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'Employees.xlsx');
                    });
                });
                e.cancel = true;
            },
            columnFixing: {
                enabled: true,
            },
            showColumnLines: false,
            showRowLines: true,
            rowAlternationEnabled: false,
            focusedRowEnabled: true,
            showBorders: true,
            //allowColumnReordering = true,
            filterRow: {
                visible: true,
            },
            headerFilter: {
                visible: true
            },
            pager: {
                allowedPageSizes: [5, 8, 15, 30],
                showInfo: true,
                showNavigationButtons: true,
                showPageSizeSelector: true,
                visible: true,
            },
            paging: {
                enabled: true,
                pageSize: 8,
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
                { dataField: 'WeavingDraftCode', caption: 'Tahar Kodu' },
                { dataField: 'CrudeWidth', caption: 'Ham En' },
                { dataField: 'CrudeGramaj', caption: 'Ham Gramaj' },
                { dataField: 'ProductWidth', caption: 'Mamül En' },
                { dataField: 'ProductGramaj', caption: 'Mamül Gramaj' },
              //  { dataField: 'WapWireCount', caption: 'Çözgü Tel Sayısı' },
                { dataField: 'MeterGramaj', caption: 'm² Gramaj' },
                { dataField: 'ItemCutTypeStr', caption: 'Kesme' },
                { dataField: 'ItemBulletTypeStr', caption: 'Kurşun' },
                { dataField: 'ItemApparelTypeStr', caption: 'Konfeksiyon' },
                { dataField: 'ItemDyeHouseTypeStr', caption: 'Boyahane' },
               // { dataField: 'CombWidth', caption: 'Atkı En' },
               // { dataField: 'WeftReportLength', caption: 'Atkı Rapor Boyu' },
               // { dataField: 'WapReportLength', caption: 'Çözgü Rapor Boyu' },
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
