app.controller('voyageListCtrl', function sidebarCtrl($scope, $http) {
    DevExpress.localization.locale('tr');

    // LIST FUNCTIONS
    $scope.loadReport = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $.getJSON(HOST_URL + 'Voyage/GetVoyageList', function (data) {

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
            editing: {
                allowUpdating: false,
                allowDeleting: false
            },
            onRowPrepared: function (e) {
                if (e.rowType != "data")
                    return;

                var deadlineClass = '', item = e.data;
                if (item.LoadStatusType == 1)
                    deadlineClass = 'bg-danger';
                if (item.LoadStatusType == 2)
                    deadlineClass = 'bg-secondary';
                if (item.LoadStatusType == 11)
                    deadlineClass = 'bg-success';
                if (!item.LoadStatusType == 11 || !item.LoadStatusType == 1 || !item.LoadStatusType == 2)
                    deadlineClass = 'bg-white';
                //else {
                //    if (item.DateOfNeedStr != null && item.DateOfNeedStr.length > 0) {
                //        var dtDeadline = moment(item.DateOfNeedStr, 'DD.MM.YYYY');
                //        if (moment().diff(dtDeadline, 'days') >= 0)
                //            deadlineClass = 'bg-secondary';
                //        else if (moment().diff(dtDeadline, 'days') > -5)
                //            deadlineClass = 'bg-warning';
                //    }
                //}
                //if (item.LoadStatusType == 5) {
                //    deadlineClass = 'bg-success';
                //}

                if (deadlineClass.length > 0)
                    e.rowElement.addClass(deadlineClass);
            },
            columns: [
                { dataField: 'VoyageCode', caption: 'Sefer Kodu' },
                { dataField: 'VoyageStatusStr', caption: 'Sefer Durumu' },
                { dataField: 'VoyageDateStr', caption: 'Sefer Tarihi' },
                { dataField: 'CarrierFirmName', caption: 'Taşıyıcı Firma' },
                { dataField: 'CustomsDoorEntryDateStr', caption: 'Giriş Gümrük Kapısı Giriş Tarihi' },
                { dataField: 'CustomsDoorEntryName', caption: 'Giriş Gümrük Kapısı' },
                { dataField: 'CustomsDoorExitDateStr', caption: 'Çıkış Gümrük Kapısı Çıkış Tarihi' },
                { dataField: 'DriverNameAndSurname', caption: 'Sürücü' },
                { dataField: 'StartDateStr', caption: 'Başlama Tarihi' },
                { dataField: 'LoadDateStr', caption: 'Bitiş Tarihi' },
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
                                var dataGrid = $("#dataList").dxDataGrid("instance");
                                dataGrid.deselectAll();
                                dataGrid.selectRowsByIndexes([e.row.rowIndex]);

                                window.location.href = HOST_URL + 'Voyage?rid=' + e.row.data.Id;
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
