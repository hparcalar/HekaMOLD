app.controller('voyageListCtrl', function sidebarCtrl($scope, $http) {
    DevExpress.localization.locale('tr');

    // LIST FUNCTIONS
    $scope.loadReport = function () {
        $('#gridContainer').dxDataGrid({
            dataSource: {
                load: function () {
                    return $.getJSON(HOST_URL + 'Voyage/GetVoyageList', function (data) {

                    });
                },
                key: 'Id'
            },

            showBorders: true,
            columnAutoWidth: true,
            allowColumnReordering: true,
            allowColumnResizing: true,
            columnAutoWidth: true,
            export: {
                enabled: true,
                allowExportSelectedData: true,
            }, onExporting(e) {
                const workbook = new ExcelJS.Workbook();
                const worksheet = workbook.addWorksheet('Sefer Listesi');

                DevExpress.excelExporter.exportDataGrid({
                    component: e.component,
                    worksheet,
                    autoFilterEnabled: true,
                }).then(() => {
                    workbook.xlsx.writeBuffer().then((buffer) => {
                        saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'Sefer Listesi.xlsx');
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
                { dataField: 'Id', caption: 'Id', visible: false, sortOrder: "desc" },
                { dataField: 'VoyageCode', caption: 'Sefer Kodu' },
                { dataField: 'VoyageStatusStr', caption: 'Sefer Durumu' },
                { dataField: 'VoyageDateStr', caption: 'Sefer Tarihi', dataType: 'date', format: 'dd.MM.yyyy' },
                { dataField: 'VoyageWeek', caption: 'Sefer Hafta' },
                { dataField: 'CarrierFirmName', caption: 'Taşıyıcı Firma' },
                { dataField: 'CustomsDoorEntryDateStr', caption: 'Giriş Gümrük Kapısı Giriş Tarihi', dataType: 'date', format: 'dd.MM.yyyy' },
                { dataField: 'CustomsDoorEntryName', caption: 'Giriş Gümrük Kapısı' },
                { dataField: 'CustomsDoorExitDateStr', caption: 'Çıkış Gümrük Kapısı Çıkış Tarihi', dataType: 'date', format: 'dd.MM.yyyy' },
                { dataField: 'DriverNameAndSurname', caption: 'Sürücü' },
                { dataField: 'StartDateStr', caption: 'Başlama Tarihi', dataType: 'date', format: 'dd.MM.yyyy' },
                { dataField: 'LoadDateStr', caption: 'Yükleme Tarihi', dataType: 'date', format: 'dd.MM.yyyy' },
                { dataField: 'EndDateStr', caption: 'Bitiş Tarihi', dataType: 'date', format: 'dd.MM.yyyy' },
                { dataField: 'OrderTransactionDirectionTypeStr', caption: 'İşlem Yönü' },
                { dataField: 'TraillerVehiclePlate', caption: 'Romörk Plaka' },
                { dataField: 'TraillerVehicleMarkAndModel', caption: 'Romörk Marka/Model' },
                { dataField: 'TowinfVehiclePlate', caption: 'Çekici Plaka' },
                { dataField: 'TowinfVehicleMarkAndModel', caption: 'Çekici Marka/Model' },
                {
                    type: "buttons",
                    buttons: [
                        {
                            name: 'preview', cssClass: '', text: 'Düzenle', onClick: function (e) {
                                var dataGrid = $("#gridContainer").dxDataGrid("instance");
                                dataGrid.deselectAll();
                                dataGrid.selectRowsByIndexes([e.row.rowIndex]);

                                window.location.href = HOST_URL + 'Voyage?rid=' + e.row.data.Id;
                            }
                        }
                    ]
                }
            ],
            masterDetail: {
                enabled: true,
                template: 'detail',
            },
        });
    }
    $scope.getDetailGridSettings = function (key) {
        console.log(key)
        return {
            dataSource: new DevExpress.data.DataSource({
                store: new DevExpress.data.ArrayStore({
                    key: 'Id',
                    data: function () {
                        return $.getJSON(HOST_URL + 'Voyage/GetVoyageDetailList', function (data) {
                            console.log(data)
                        })
                    },
                }),
                filter: ['VoyageID', '=', key],
            }),
            columnAutoWidth: true,
            showBorders: true,
            columns: ['Subject', {
                dataField: 'StartDate',
                dataType: 'date',
            }, {
                    dataField: 'DueDate',
                    dataType: 'date',
                }, 'Priority', {
                    caption: 'Completed',
                    dataType: 'boolean',
                    calculateCellValue(rowData) {
                        return rowData.Status === 'Completed';
                    },
                }],
        };
    };
    $scope.loadReport();
});
