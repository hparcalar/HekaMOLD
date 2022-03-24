app.controller('yarnRecipeListCtrl', function ($scope, $http) {
    DevExpress.localization.locale('tr');

    // LIST FUNCTIONS
    $scope.loadReport = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $.getJSON(HOST_URL + 'YarnRecipe/GetYarnRecipeList', function (data) {

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
            //onExporting(e) {
            //    const workbook = new ExcelJS.Workbook();
            //    const worksheet = workbook.addWorksheet('İplik Listesi');

            //    DevExpress.excelExporter.exportDataGrid({
            //        component: e.component,
            //        worksheet,
            //        autoFilterEnabled: true,
            //    }).then(() => {
            //        workbook.xlsx.writeBuffer().then((buffer) => {
            //            saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'İplik Listesi.xlsx');
            //        });
            //    });
            //    e.cancel = true;
            //},
            columnFixing: {
                enabled: true,
            },
            showColumnLines: false,
            showRowLines: true,
            rowAlternationEnabled: false,
            focusedRowEnabled: true,
            showBorders: true,
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
                { dataField: 'YarnRecipeCode', caption: 'İpik Kodu' },
                { dataField: 'YarnRecipeName', caption: 'İplik Adı' },
                { dataField: 'YarnBreedName', caption: 'İplik Cinsi' },
                { dataField: 'FirmName', caption: 'Firma' },
                { dataField: 'YarnColourName', caption: 'Renk' },
                { dataField: 'Denier', caption: 'Fiili Denye' },
                { dataField: 'YarnDenier', caption: 'İplik Denye' },
                { dataField: 'Factor', caption: 'Katsayı' },
                { dataField: 'Twist', caption: 'Büküm' },
                { dataField: 'Center', caption: 'Punta' },
                { dataField: 'Mix', caption: 'Karışım' },
               {
                    type: "buttons",
                    buttons: [
                        {
                            name: 'preview', cssClass: '', text: 'Düzenle', onClick: function (e) {
                                var dataGrid = $("#dataList").dxDataGrid("instance");
                                dataGrid.deselectAll();
                                dataGrid.selectRowsByIndexes([e.row.rowIndex]);

                                window.location.href = HOST_URL + 'YarnRecipe?rid=' + e.row.data.Id;
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
