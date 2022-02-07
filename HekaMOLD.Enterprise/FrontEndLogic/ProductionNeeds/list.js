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
            allowColumnResizing: true,
            wordWrapEnabled: true,
            rowAlternationEnabled: true,
            export: {
                enabled: true,
                allowExportSelectedData: true,
            },
            focusedRowEnabled: true,
            showBorders: true,
            filterRow: {
                visible: true
            },
            headerFilter: {
                visible: true
            },
            height:500,
            paging: {
                enabled: false,
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
            onContentReady: function (e) {
                try {
                    var dataGrid = $("#dataList").dxDataGrid("instance");
                    if (dataGrid != null) {
                        cFilter = dataGrid.getCombinedFilter(true);
                        console.log(cFilter);
                    }
                } catch (e) {

                }
            },
            columns: [
                { dataField: 'ItemOrderDateStr', caption: 'Sip. Tarih', dataType: 'date', format: 'dd.MM.yyyy' },
                { dataField: 'NeedsDateStr', caption: 'İhtiyaç Tar.', dataType: 'date', format: 'dd.MM.yyyy' },
                /* { dataField: 'WorkOrderNo', caption: 'İş Emri No' },*/
                /*{ dataField: 'ProductCode', caption: 'Ürün Kodu' },*/

                { dataField: 'ProductName', caption: 'Ürün Adı' },
                { dataField: 'ItemOrderNo', caption: 'Sipariş No' },
                /*{ dataField: 'DyeCode', caption: 'Renk Kodu' },*/
                { dataField: 'ItemNo', caption: 'Malzeme No' },
                { dataField: 'ItemName', caption: 'Malzeme Adı' },
                /*{ dataField: 'SaleOrderDeadline', caption: 'Sipariş Termin' },*/
                { dataField: 'RecipeQuantity', caption: 'Reçete Miktar', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'TargetQuantity', caption: 'Sipariş Miktar', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'WarehouseQuantity', caption: 'Depo Miktar', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'Quantity', caption: 'İhtiyaç Miktar', format: { type: "fixedPoint", precision: 2 } },
            ],
            summary: {
                totalItems: [{
                    column: "TargetQuantity",
                    summaryType: "sum",
                    valueFormat: { type: "fixedPoint", precision: 2 }
                    }, {
                    column: "WarehouseQuantity",
                    summaryType: "sum",
                    valueFormat: { type: "fixedPoint", precision: 2 }
                    },
                    {
                        column: "Quantity",
                        summaryType: "sum",
                        valueFormat: { type: "fixedPoint", precision: 2 }
                    }
                ]
            }
        });

        $('#dataListSummary').dxDataGrid({
            dataSource: {
                load: function () {
                    return $.getJSON(HOST_URL + 'ProductionNeeds/GetProductionNeedsSummary', function (data) {

                    });
                },
                key: 'Id'
            },
            showColumnLines: false,
            showRowLines: true,
            allowColumnResizing: true,
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
                { dataField: 'ItemNo', caption: 'Malzeme No' },
                { dataField: 'ItemName', caption: 'Malzeme Adı' },
                { dataField: 'TargetQuantity', caption: 'Sipariş Miktar', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'WarehouseQuantity', caption: 'Depo Miktar', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'Quantity', caption: 'İhtiyaç Miktar', format: { type: "fixedPoint", precision: 2 } },
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
