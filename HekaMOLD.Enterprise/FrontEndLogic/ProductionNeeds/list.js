app.controller('productionNeedsCtrl', function sidebarCtrl($scope, $http) {
    DevExpress.localization.locale('tr');

    // LIST FUNCTIONS
    $scope.loadReport = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $.getJSON(HOST_URL + 'ProductionNeeds/GetProductionNeeds', function (data) {

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
                { dataField: 'WorkOrderDateStr', caption: 'Tarih', dataType: 'date', format: 'dd.MM.yyyy' },
/*                { dataField: 'WorkOrderNo', caption: 'İş Emri No' },*/
                { dataField: 'ProductCode', caption: 'Ürün Kodu' },
                { dataField: 'ProductName', caption: 'Ürün Adı' },
                /*{ dataField: 'DyeCode', caption: 'Renk Kodu' },*/
                { dataField: 'ItemNo', caption: 'Malzeme No' },
                { dataField: 'ItemName', caption: 'Malzeme Adı' },
                /*{ dataField: 'SaleOrderDeadline', caption: 'Sipariş Termin' },*/
                { dataField: 'Quantity', caption: 'Miktar' },
                //{
                //    type: "buttons",
                //    buttons: [
                //        {
                //            name: 'preview', cssClass: '', text: 'Düzenle', onClick: function (e) {
                //                var dataGrid = $("#dataList").dxDataGrid("instance");
                //                dataGrid.deselectAll();
                //                dataGrid.selectRowsByIndexes([e.row.rowIndex]);

                //                window.location.href = HOST_URL + 'WorkOrder?rid=' + e.row.data.Id;
                //            }
                //        }
                //    ]
                //}
            ]
        });
    }

    // VISUAL TRIGGERS
    $scope.calculateNeeds = function () {
        bootbox.confirm({
            message: "İhtiyaçlar yeniden hesaplanacaktır. Devam istediğinizden emin misiniz?",
            closeButton: false,
            buttons: {
                confirm: {
                    label: 'Evet',
                    className: 'btn-primary'
                },
                cancel: {
                    label: 'Hayır',
                    className: 'btn-light'
                }
            },
            callback: function (result) {
                if (result) {
                    $scope.saveStatus = 1;
                    $http.post(HOST_URL + 'ProductionNeeds/CalculateProductionNeeds', { }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Result == true) {
                                    toastr.success('İhtiyaçlar yeniden hesaplandı', 'Bilgilendirme');

                                    $scope.loadReport();
                                }
                                else
                                    toastr.error(resp.data.ErrorMessage, 'Hata');
                            }
                        }).catch(function (err) { });
                }
            }
        });
    }

    // ON LOAD EVENTS
    $scope.loadReport();
});
