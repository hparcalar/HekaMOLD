app.controller('contractWorksCtrl', function ($scope, $http) {
    $scope.modelObject = {};

    $scope.saveStatus = 0;

    $scope.selectedWorkCategory = {};
    $scope.workCategoryList = [];

    $scope.selectedWarehouse = {};
    $scope.warehouseList = [];

    $scope.selectedWorkOrder = {};
    $scope.workOrderList = [];

    $scope.entryList = [];
    $scope.deliveryList = [];

    // GET SELECTABLE DATA
    $scope.loadSelectables = function () {
        var prmReq = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'ContractWorks/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.warehouseList = resp.data.Warehouses;
                        $scope.workCategoryList = resp.data.WorkCategories;

                        resolve(resp.data);
                    }
                }).catch(function (err) { });
        });

        return prmReq;
    }

    $scope.onCategoryChanged = function () {
        $scope.bindModel();
    }

    // CRUD
    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu stok tanımını silmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'Item/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Status == 1) {
                                    toastr.success('Kayıt başarıyla silindi.', 'Bilgilendirme');

                                    $scope.openNewRecord();
                                }
                                else
                                    toastr.error(resp.data.ErrorMessage, 'Hata');
                            }
                        }).catch(function (err) { });
                }
            }
        });
    }

    $scope.saveModel = function () {
        $scope.saveStatus = 1;

        if (typeof $scope.selectedItemType != 'undefined' && $scope.selectedItemType != null)
            $scope.modelObject.ItemType = $scope.selectedItemType.Id;
        else
            $scope.modelObject.ItemType = null;

        if (typeof $scope.selectedCategory != 'undefined' && $scope.selectedCategory != null)
            $scope.modelObject.ItemCategoryId = $scope.selectedCategory.Id;
        else
            $scope.modelObject.ItemCategoryId = null;

        if (typeof $scope.selectedGroup != 'undefined' && $scope.selectedGroup != null)
            $scope.modelObject.ItemGroupId = $scope.selectedGroup.Id;
        else
            $scope.modelObject.ItemGroupId = null;

        if (typeof $scope.selectedFirm != 'undefined' && $scope.selectedFirm != null)
            $scope.modelObject.SupplierFirmId = $scope.selectedFirm.Id;
        else
            $scope.modelObject.SupplierFirmId = null;

        $http.post(HOST_URL + 'Item/SaveModel', $scope.modelObject, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.saveStatus = 0;

                    if (resp.data.Status == 1) {
                        toastr.success('Kayıt başarılı.', 'Bilgilendirme');

                        $scope.bindModel(resp.data.RecordId);
                    }
                    else
                        toastr.error(resp.data.ErrorMessage, 'Hata');
                }
            }).catch(function (err) { });
    }

    $scope.bindModel = function () {
        if (typeof $scope.selectedWorkCategory == 'undefined' || $scope.selectedWorkCategory == null)
            $scope.selectedWorkCategory = { Id: 0 };

        $http.get(HOST_URL + 'ContractWorks/BindModel?workOrderCategory=' + $scope.selectedWorkCategory.Id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.workOrderList = resp.data.WorkOrders;
                    $scope.bindWorkOrders();
                }
            }).catch(function (err) { });
    }

    $scope.bindMovements = function () {
        $http.get(HOST_URL + 'ContractWorks/GetMovements?workOrderDetailId=' + $scope.selectedWorkOrder.Id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.deliveryList = resp.data.filter(d => d.DeliveredDetailId != null);
                    $scope.entryList = resp.data.filter(d => d.ReceivedDetailId != null);

                    $scope.bindMovementTables();
                }
            }).catch(function (err) { });
    }

    $scope.bindWorkOrders = function () {
        $('#workOrderList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $scope.workOrderList;
                },
                key: 'Id'
            },
            showColumnLines: true,
            showRowLines: true,
            rowAlternationEnabled: false,
            allowColumnResizing: true,
            wordWrapEnabled: true,
            focusedRowEnabled: false,
            showBorders: true,
            filterRow: {
                visible: true
            },
            headerFilter: {
                visible: true
            },
            groupPanel: {
                visible: false
            },
            scrolling: {
                mode: "virtual"
            },
            height: 500,
            editing: {
                allowUpdating: false,
                allowDeleting: false,
                allowAdding: false,
                mode: 'cell'
            },
            onRowPrepared: function (e) {
                if (e.rowType != "data")
                    return;

                if (e.data.Id == $scope.selectedWorkOrder.Id) {
                    e.rowElement.addClass('bg-warning');
                }
            },
            columns: [
                { dataField: 'WorkOrderDateStr', caption: 'Tarih', dataType: 'date', format: 'dd.MM.yyyy'},
                /*{ dataField: 'ProductCode', caption: 'Stok Kodu' },*/
                { dataField: 'ProductName', caption: 'Stok Adı' },
                { dataField: 'FirmName', caption: 'Firma' },
                { dataField: 'Quantity', caption: 'Miktar', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                {
                    type: "buttons",
                    buttons: [
                        {
                            name: 'preview', cssClass: 'btn btn-sm btn-light-primary py-0 px-1', text: 'Seç', onClick: function (e) {
                                var dataGrid = $("#workOrderList").dxDataGrid("instance");
                                $scope.selectedWorkOrder = e.row.data;
                                $scope.bindMovements();
                                $scope.bindWorkOrders();
                            }
                        }
                    ]
                }
            ],
        });
    }

    $scope.bindMovementTables = function () {
        $('#entryList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $scope.entryList;
                },
                key: 'Id'
            },
            showColumnLines: true,
            showRowLines: true,
            rowAlternationEnabled: true,
            allowColumnResizing: true,
            wordWrapEnabled: true,
            focusedRowEnabled: false,
            showBorders: true,
            filterRow: {
                visible: true
            },
            headerFilter: {
                visible: true
            },
            groupPanel: {
                visible: false
            },
            scrolling: {
                mode: "virtual"
            },
            height: 500,
            editing: {
                allowUpdating: false,
                allowDeleting: false,
                allowAdding: false,
                mode: 'cell'
            },
            columns: [
                { dataField: 'FlowDateStr', caption: 'Çıkış Tarih', dataType: 'date', format: 'dd.MM.yyyy' },
                { dataField: 'ItemName', caption: 'Stok' },
                { dataField: 'FirmName', caption: 'Firma' },
                { dataField: 'Quantity', caption: 'Miktar', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                {
                    type: "buttons",
                    buttons: [
                        {
                            name: 'preview', cssClass: 'btn btn-sm btn-light-primary py-0 px-1', text: 'Seç', onClick: function (e) {
                                var dataGrid = $("#dataList").dxDataGrid("instance");
                                $scope.selectedWorkOrder = e.row.data;
                                $scope.bindMovements();
                            }
                        }
                    ]
                }
            ],
        });

        $('#deliveryList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $scope.deliveryList;
                },
                key: 'Id'
            },
            showColumnLines: true,
            showRowLines: true,
            rowAlternationEnabled: true,
            allowColumnResizing: true,
            wordWrapEnabled: true,
            focusedRowEnabled: false,
            showBorders: true,
            filterRow: {
                visible: true
            },
            headerFilter: {
                visible: true
            },
            groupPanel: {
                visible: false
            },
            scrolling: {
                mode: "virtual"
            },
            height: 500,
            editing: {
                allowUpdating: false,
                allowDeleting: false,
                allowAdding: false,
                mode: 'cell'
            },
            columns: [
                { dataField: 'FlowDateStr', caption: 'Çıkış Tarih', dataType: 'date', format: 'dd.MM.yyyy' },
                { dataField: 'ItemName', caption: 'Stok' },
                { dataField: 'FirmName', caption: 'Firma' },
                { dataField: 'Quantity', caption: 'Miktar', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                {
                    type: "buttons",
                    buttons: [
                        {
                            name: 'preview', cssClass: 'btn btn-sm btn-light-primary py-0 px-1', text: 'Seç', onClick: function (e) {
                                var dataGrid = $("#dataList").dxDataGrid("instance");
                                $scope.selectedWorkOrder = e.row.data;
                                $scope.bindMovements();
                            }
                        }
                    ]
                }
            ],
        });
    }

    // ON LOAD EVENTS
    DevExpress.localization.locale('tr');
    $scope.loadSelectables().then(function (data) {
        $scope.bindModel();
    });
});