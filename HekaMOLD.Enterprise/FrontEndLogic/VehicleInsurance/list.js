app.controller('vehicleInsuranceListCtrl', function sidebarCtrl($scope, $http) {
    DevExpress.localization.locale('tr');

    // LIST FUNCTIONS
    $scope.loadReport = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $.getJSON(HOST_URL + 'VehicleInsurance/GetVehicleInsuranceList', function (data) {

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
                { dataField: 'Plate', caption: 'Plaka' },
                { dataField: 'StartDateStr', caption: 'Başlangıç Tarihi' },
                { dataField: 'EndDateStr', caption: 'Bitiş Tarihi' },
                { dataField: 'VehicleInsuranceTypeName', caption: 'Belge Tip' },
                { dataField: 'FirmName', caption: 'İşlem Firma' },
                { dataField: 'KmHour', caption: 'Km/Saat' },
                { dataField: 'Amount', caption: 'Fiyat' },
                { dataField: 'ForexTypeCode', caption: 'Döviz Kodu' },
               {
                    type: "buttons",
                    buttons: [
                        {
                            name: 'preview', cssClass: '', text: 'Düzenle', onClick: function (e) {
                                var dataGrid = $("#dataList").dxDataGrid("instance");
                                dataGrid.deselectAll();
                                dataGrid.selectRowsByIndexes([e.row.rowIndex]);

                                window.location.href = HOST_URL + 'VehicleInsurance?rid=' + e.row.data.Id;
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
