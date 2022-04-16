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
            columnAutoWidth: true,

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
            columns: [
                { dataField: 'Plate', caption: 'Plaka' },
                { dataField: 'Mark', caption: 'Marka' },
                { dataField: 'Versiyon', caption: 'Model' },
                { dataField: 'KmHour', caption: 'Km/Saat' },
                { dataField: 'HasLoadPlannig', caption: 'Yük Planlanabilir' },
                { dataField: 'VehicleAllocationTypeStr', caption: 'Araç Tahsis Tip' },
                { dataField: 'VehicleTypeName', caption: 'Araç Tip' },
                { dataField: 'OwnerFirmName', caption: 'İşlem Firma' },
                { dataField: 'CareNotification', caption: 'Bakım Bildirim', dataType:'boolean' },
                { dataField: 'TireNotification', caption: 'Lastik Bildirim', dataType: 'boolean' },
                { dataField: 'KmHourControl', caption: 'Km Kontrol', dataType: 'boolean' },
                { dataField: 'HasLoadPlannig', caption: 'Yüke Dönüştürülebilir', dataType: 'boolean' },
                { dataField: 'CarePeriyot', caption: 'Bakım Periyot' },
                { dataField: 'ChassisNumber', caption: 'Şasi Numarası' },
                { dataField: 'Height', caption: 'Yükseklik' },
                { dataField: 'Width', caption: 'Genişlik' },
                { dataField: 'Length', caption: 'Uzunluk' },
                { dataField: 'LoadCapacity', caption: 'Yükleme Kapasite' },
                { dataField: 'ProportionalLimit', caption: 'Oransal Limit' },
                { dataField: 'ContractStartDateStr', caption: 'Söz. Baş. Tarih', dataType: 'date', format: 'dd.MM.yyyy' },
                { dataField: 'ContractEndDateStr', caption: 'Söz. Bit. Tarih', dataType: 'date',format :'dd.MM.yyyy' },

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
