app.controller('warehouseStatesCtrl', function ($scope, $http) {
    DevExpress.localization.locale('tr');

    $scope.warehouseList = [];
    $scope.selectedWarehouseList = [];

    // DATA GET METHODS
    $scope.loadWarehouseList = function () {
        $http.get(HOST_URL + 'Common/GetWarehouseList', {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.warehouseList = resp.data;

                    if ($scope.selectedWarehouseList.length == 0) {
                        $scope.warehouseList.forEach(d => {
                            $scope.selectedWarehouseList.push(d);
                        });
                    }

                    $scope.loadWarehouseSelections();
                    $scope.loadReport();
                }
            }).catch(function (err) { });
    }

    // PREPARE FILTERS
    $scope.loadWarehouseSelections = function () {
        $("#warehouseSelection").dxDropDownBox({
            value: [],
            valueExpr: "Id",
            placeholder: "Depo Seçin",
            displayExpr: "WarehouseName",
            showClearButton: true,
            dataSource: $scope.warehouseList,
            contentTemplate: function (e) {
                var value = e.component.option("value"),
                    $dataGrid = $("<div>").dxDataGrid({
                        dataSource: $scope.warehouseList,
                        keyExpr: "Id",
                        columns: [{ dataField: 'WarehouseCode', caption: 'Kodu' }, { dataField: 'WarehouseName', caption: 'Adı' }],
                        hoverStateEnabled: true,
                        paging: { enabled: true, pageSize: 10 },
                        filterRow: { visible: true },
                        scrolling: { mode: "infinite" },
                        height: 345,
                        selection: { mode: "multiple" },
                        selectedRowKeys: $scope.selectedWarehouseList.map(m => m.Id),
                        onSelectionChanged: function (selectedItems) {
                            var keys = selectedItems.selectedRowKeys;

                            if ($scope.selectedWarehouseList.length > 0)
                                $scope.selectedWarehouseList.splice(0, $scope.selectedWarehouseList.length);

                            var selectedWrs = $scope.warehouseList.filter(d => keys.indexOf(d.Id) > -1);
                            selectedWrs.forEach(d => { $scope.selectedWarehouseList.push(d); });
                            e.component.option("value", keys);

                            $scope.loadReport();
                        }
                    });

                dataGrid = $dataGrid.dxDataGrid("instance");

                e.component.on("valueChanged", function (args) {
                    var value = args.value;
                    dataGrid.selectRows(value, false);

                    $scope.$apply();
                });

                return $dataGrid;
            }
        });
    }

    // LIST FUNCTIONS
    $scope.loadReport = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    let warehouseIds = '';
                    $scope.selectedWarehouseList.forEach(d => {
                        warehouseIds += d.Id + ',';
                    });

                    if (warehouseIds.length > 0)
                        warehouseIds = warehouseIds.substr(0, warehouseIds.length - 1);

                    return $.getJSON(HOST_URL + 'Warehouse/GetStatesData?warehouseList=' + warehouseIds, function (data) {
                            
                        });
                },
                key: ['ItemId','WarehouseId']
            },
            showColumnLines: false,
            showRowLines: true,
            rowAlternationEnabled: true,
            allowColumnResizing: true,
            export: {
                enabled: true,
                allowExportSelectedData: true,
            },
            wordWrapEnabled: true,
            focusedRowEnabled: true,
            showBorders: true,
            filterRow: {
                visible: true
            },
            headerFilter: {
                visible: true
            },
            height:500,
            //paging: {
            //    enabled:true,
            //    pageSize: 13,
            //    pageIndex:0
            //},
            groupPanel: {
                visible: true
            },
            editing: {
                allowUpdating: false,
                allowDeleting: false
            },
            columns: [
                /*{ dataField: 'WarehouseName', caption: 'Depo' },*/
                { dataField: 'ItemNo', caption: 'Stok Kodu' },
                { dataField: 'ItemName', caption: 'Stok Adı' },
                { dataField: 'InQty', caption: 'Giriş Miktar' },
                { dataField: 'OutQty', caption: 'Çıkış Miktar' },
                { dataField: 'TotalQty', caption: 'Mevcut Miktar' },
                //{
                //    type: "buttons",
                //    buttons: [
                //        {
                //            name: 'preview', cssClass: 'btn btn-sm btn-light-primary', text: 'Seriler', onClick: function (e) {
                //                var dataGrid = $("#dataList").dxDataGrid("instance");
                                
                //            }
                //        }
                //    ]
                //}
            ]
            });
        }

    // ON LOAD EVENTS
    $scope.loadWarehouseList();
});
