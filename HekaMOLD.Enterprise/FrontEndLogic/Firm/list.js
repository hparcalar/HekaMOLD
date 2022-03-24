app.controller('firmListCtrl', function sidebarCtrl($scope, $http) {
    DevExpress.localization.locale('tr');

    // LIST FUNCTIONS
    $scope.loadReport = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $.getJSON(HOST_URL + 'Firm/GetFirmList', function (data) {
                            
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
            //    const worksheet = workbook.addWorksheet('Firma Listesi');

            //    DevExpress.excelExporter.exportDataGrid({
            //        component: e.component,
            //        worksheet,
            //        autoFilterEnabled: true,
            //    }).then(() => {
            //        workbook.xlsx.writeBuffer().then((buffer) => {
            //            saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'Firma Listesi.xlsx');
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
                { dataField: 'FirmCode', caption: 'Firma Kodu' },
                { dataField: 'FirmName', caption: 'Firma Adı' },
                { dataField: 'FirmTypeStr', caption: 'Firma Türü' },
                { dataField: 'IsApproved', caption: 'Onaylı Tedarikçi' },
                {
                    type: "buttons",
                    buttons: [
                        {
                            name: 'preview', cssClass: '', text: 'Düzenle', onClick: function (e) {
                                var dataGrid = $("#dataList").dxDataGrid("instance");
                                dataGrid.deselectAll();
                                dataGrid.selectRowsByIndexes([e.row.rowIndex]);

                                window.location.href = HOST_URL + 'Firm?rid=' + e.row.data.Id;
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
