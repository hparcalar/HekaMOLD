app.controller('vehicleListCtrl', function sidebarCtrl($scope, $http) {
    DevExpress.localization.locale('tr');

    // LIST FUNCTIONS
    $scope.loadReport = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $.getJSON(HOST_URL + 'Vehicle/GetVehicleList', function (data) {

                    });
                },
                key: 'Id'
            },
            showColumnLines: false,
            showRowLines: true,
            rowAlternationEnabled: false,
            focusedRowEnabled: true,
            showBorders: true,
            allowColumnResizing: true,
            wordWrapEnabled: true,

            export: {
                enabled: true,
                allowExportSelectedData: true,
            },
            onExporting(e) {
                const workbook = new ExcelJS.Workbook();
                const worksheet = workbook.addWorksheet('Araç Listesi');

                DevExpress.excelExporter.exportDataGrid({
                    component: e.component,
                    worksheet,
                    autoFilterEnabled: true,
                }).then(() => {
                    workbook.xlsx.writeBuffer().then((buffer) => {
                        saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'Araç Listesi.xlsx');
                    });
                });
                e.cancel = true;
            },
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
                { dataField: 'Plate', caption: 'Plaka' },
                { dataField: 'Mark', caption: 'Marka' },
                { dataField: 'Versiyon', caption: 'Model' },
                { dataField: 'KmHour', caption: 'Km/Saat' },
                { dataField: 'HasLoadPlannig', caption: 'Yük Planlanabilir' },
                { dataField: 'VehicleAllocationTypeStr', caption: 'Araç Tahsis Tip' },
                { dataField: 'VehicleTypeName', caption: 'Araç Tip' },
                //{ dataField: 'ContractStartDateStr', caption: 'Söz. Baş. Tarih', dataType: 'date', format: 'dd.MM.yyyy' },
                //{ dataField: 'ContractEndDateStr', caption: 'Söz. Bit. Tarih', dataType: 'date',format :'dd.MM.yyyy' },

               {
                    type: "buttons",
                    buttons: [
                        {
                            name: 'preview', cssClass: '', text: 'Düzenle', onClick: function (e) {
                                var dataGrid = $("#dataList").dxDataGrid("instance");
                                dataGrid.deselectAll();
                                dataGrid.selectRowsByIndexes([e.row.rowIndex]);

                                window.location.href = HOST_URL + 'Vehicle?rid=' + e.row.data.Id;
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
