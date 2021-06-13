app.controller('productRecipeCtrl', function ($scope, $http) {
    $scope.modelObject = { Id:0, CreatedDate: moment().format('DD.MM.YYYY'), Details:[] };

    $scope.itemList = [];
    $scope.unitList = [];
    $scope.warehouseList = [];

    $scope.productList = [];
    $scope.selectedProduct = {};

    $scope.saveStatus = 0;

    // VISUAL EVENTS
    $scope.lastProductId = 0;
    $scope.onProductChanged = function (e) {
        try {
            if ($scope.lastProductId != $scope.selectedProduct.Id) {
                $http.get(HOST_URL + 'ProductRecipe/FindRecipeByProductId?productId='
                    + $scope.selectedProduct.Id, {}, 'json')
                    .then(function (resp) {
                        if (typeof resp.data != 'undefined' && resp.data != null
                            && resp.data.Id > 0) {
                            $scope.modelObject = resp.data;
                            $scope.modelObject.CreatedDate = $scope.modelObject.CreatedDateStr;

                            if (typeof $scope.modelObject.ProductId != 'undefined' && $scope.modelObject.ProductId != null)
                                $scope.selectedProduct = $scope.productList.find(d => d.Id == $scope.modelObject.ProductId);
                            else
                                $scope.selectedProduct = { Id: 0 };

                            $scope.lastProductId = $scope.selectedProduct.Id;

                            refreshArray($scope.productList);

                            $scope.bindDetails();
                        }
                    }).catch(function (err) { });
            }
        } catch (e) {

        }
    }

    $scope.onIsActiveChanged = function (e) {
        if ($scope.modelObject.IsActive != true) {
            bootbox.confirm({
                message: "Bu reçeteyi pasif duruma getirmek istediğinizden emin misiniz?",
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
                    if (!result) {
                        $scope.modelObject.IsActive = true;
                    }
                }
            });
        }
    }

    // RECIPE FUNCTIONS
    $scope.getNextRecipeNo = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'ProductRecipe/GetNextRecipeNo', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        if (resp.data.Result) {
                            resolve(resp.data.RecipeNo);
                        }
                        else {
                            toastr.error('Sıradaki reçete numarası üretilemedi. Lütfen ekranı yenileyip tekrar deneyiniz.', 'Uyarı');
                            resolve('');
                        }
                    }
                }).catch(function (err) { });
        });

        return prms;
    }

    // CRUD
    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0, CreatedDate: moment().format('DD.MM.YYYY'), Details: [] };
        $scope.getNextRecipeNo().then(function (rNo) {
            $scope.modelObject.ProductRecipeCode = rNo;
            $scope.selectedProduct = {};
            $scope.$apply();
        });
        $scope.bindDetails();
    }

    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu reçeteyi silmek istediğinizden emin misiniz?",
            closeButton:false,
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
                    $http.post(HOST_URL + 'ProductRecipe/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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

        if (typeof $scope.selectedProduct != 'undefined' && $scope.selectedProduct != null)
            $scope.modelObject.ProductId = $scope.selectedProduct.Id;
        else
            $scope.modelObject.ProductId = null;

        $http.post(HOST_URL + 'ProductRecipe/SaveModel', $scope.modelObject, 'json')
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

    $scope.dropDownBoxEditorTemplate = function (cellElement, cellInfo) {
        return $("<div>").dxDropDownBox({
            dropDownOptions: { width: 500 },
            dataSource: $scope.itemList,
            value: cellInfo.value,
            valueExpr: "Id",
            displayExpr: "ItemNo",
            contentTemplate: function (e) {
                return $("<div>").dxDataGrid({
                    dataSource: $scope.itemList,
                    remoteOperations: true,
                    columns: [
                        { dataField: 'ItemNo', caption: 'Stok Kodu' },
                        { dataField: 'ItemName', caption: 'Stok Adı' },
                        { dataField: 'ItemTypeStr', caption: 'Stok Türü' },
                        { dataField: 'GroupName', caption: 'Grup' },
                        { dataField: 'CategoryName', caption: 'Kategori' }
                    ],
                    hoverStateEnabled: true,
                    keyExpr: "Id",
                    scrolling: { mode: "virtual" },
                    height: 250,
                    filterRow: { visible: true },
                    selection: { mode: "single" },
                    selectedRowKeys: [cellInfo.value],
                    focusedRowEnabled: true,
                    focusedRowKey: cellInfo.value,
                    onSelectionChanged: function (selectionChangedArgs) {
                        e.component.option("value", selectionChangedArgs.selectedRowKeys[0]);
                        cellInfo.setValue(selectionChangedArgs.selectedRowKeys[0]);
                        if (selectionChangedArgs.selectedRowKeys.length > 0) {
                            e.component.close();
                        }
                    }
                });
            },
        });
    }

    $scope.bindModel = function (id) {
        $http.get(HOST_URL + 'ProductRecipe/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;
                    $scope.modelObject.CreatedDate = $scope.modelObject.CreatedDateStr;

                    if (typeof $scope.modelObject.ProductId != 'undefined' && $scope.modelObject.ProductId != null)
                        $scope.selectedProduct = $scope.productList.find(d => d.Id == $scope.modelObject.ProductId);
                    else
                        $scope.selectedProduct = { Id:0 };

                    $scope.lastProductId = $scope.selectedProduct.Id;

                    refreshArray($scope.productList);

                    $scope.bindDetails();
                }
            }).catch(function (err) { });
    }

    $scope.bindDetails = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $scope.modelObject.Details;
                },
                update: function (key, values) {
                    var obj = $scope.modelObject.Details.find(d => d.Id == key);
                    if (obj != null) {
                        if (typeof values.ItemId != 'undefined') {
                            var itemObj = $scope.itemList.find(d => d.Id == values.ItemId);
                            obj.ItemId = itemObj.Id;
                            obj.ItemNo = itemObj.ItemNo;
                            obj.ItemName = itemObj.ItemName;
                        }
                        if (typeof values.UnitId != 'undefined') {
                            var unitObj = $scope.unitList.find(d => d.Id == values.UnitId);
                            obj.UnitId = unitObj.Id;
                            obj.UnitName = unitObj.UnitCode;
                        }

                        if (typeof values.WarehouseId != 'undefined') {
                            var wrObj = $scope.warehouseList.find(d => d.Id == values.WarehouseId);
                            obj.WarehouseId = wrObj.Id;
                            obj.WarehouseCode = wrObj.WarehouseCode;
                            obj.WarehouseName = wrObj.WarehouseName;
                        }

                        if (typeof values.Quantity != 'undefined') { obj.Quantity = values.Quantity; }
                        if (typeof values.ProcessType != 'undefined') { obj.ProcessType = values.ProcessType; }
                        if (typeof values.WastagePercentage != 'undefined') { obj.WastagePercentage = values.WastagePercentage; }
                    }
                },
                remove: function (key) {
                    var obj = $scope.modelObject.Details.find(d => d.Id == key);
                    if (obj != null) {
                        $scope.modelObject.Details.splice($scope.modelObject.Details.indexOf(obj), 1);
                    }
                },
                insert: function (values) {
                    var newId = 1;
                    if ($scope.modelObject.Details.length > 0) {
                        newId = $scope.modelObject.Details.map(d => d.Id).reduce((max, n) => n > max ? n : max)
                        newId++;
                    }

                    var itemObj = $scope.itemList.find(d => d.Id == values.ItemId);
                    var unitObj = $scope.unitList.find(d => d.Id == values.UnitId);
                    var wrObj = $scope.warehouseList.find(d => d.Id == values.WarehouseId);

                    var newObj = {
                        Id: newId,
                        ItemId: itemObj.Id,
                        ItemNo: itemObj.ItemNo,
                        ItemName: itemObj.ItemName,
                        UnitId: typeof unitObj != 'undefined' && unitObj != null ? unitObj.Id : null,
                        UnitName: typeof unitObj != 'undefined' && unitObj != null ? unitObj.UnitCode : null,
                        Quantity: values.Quantity,
                        ProcessType: values.ProcessType,
                        WastagePercentage: values.WastagePercentage,
                        WarehouseId: typeof wrObj != 'undefined' && wrObj != null ? wrObj.Id : null,
                        WarehouseCode: typeof wrObj != 'undefined' && wrObj != null ? wrObj.WarehouseCode : null,
                        WarehouseName: typeof wrObj != 'undefined' && wrObj != null ? wrObj.WarehouseName : null,
                        NewDetail: true
                    };

                    $scope.modelObject.Details.push(newObj);
                },
                key: 'Id'
            },
            showColumnLines: true,
            showRowLines: true,
            rowAlternationEnabled: true,
            focusedRowEnabled: false,
            showBorders: true,
            filterRow: {
                visible: false
            },
            headerFilter: {
                visible: false
            },
            groupPanel: {
                visible: false
            },
            scrolling: {
                mode: "virtual"
            },
            height: 420,
            editing: {
                allowUpdating: true,
                allowDeleting: true,
                allowAdding: true,
                mode: 'cell'
            },
            onInitNewRow: function (e) {
                
            },
            columns: [
                {
                    dataField: 'ItemId', caption: 'Stok Kodu',
                    lookup: {
                        dataSource: $scope.itemList,
                        valueExpr: "Id",
                        displayExpr: "ItemNo"
                    },
                    allowSorting: false,
                    validationRules: [{ type: "required" }],
                    editCellTemplate: $scope.dropDownBoxEditorTemplate,
                    cellTemplate: function (container, options) {
                        if (typeof options.row.data.ItemNo != 'undefined'
                            && options.row.data.ItemNo != null && options.row.data.ItemNo.length > 0)
                            container.text(options.row.data.ItemNo);
                        else
                            container.text(options.displayValue);
                    }
                },
                { dataField: 'ItemName', caption: 'Stok Adı', allowEditing: false },
                {
                    dataField: 'UnitId', caption: 'Birim',
                    allowSorting: false,
                    lookup: {
                        dataSource: $scope.unitList,
                        valueExpr: "Id",
                        displayExpr: "UnitCode"
                    },
                    validationRules: [{ type: "required" }]
                },
                { dataField: 'Quantity', caption: 'Miktar', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, validationRules: [{ type: "required" }] },
                {
                    dataField: 'ProcessType', caption: 'İşlem Türü',
                    allowSorting: false,
                    lookup: {
                        dataSource: [{ Id:1, Text:'Sarf Malzeme' }, {Id:2, Text:'Hurda'} ],
                        valueExpr: "Id",
                        displayExpr: "Text"
                    },
                    validationRules: [{ type: "required" }]
                },
                {
                    dataField: 'WarehouseId', caption: 'Depo',
                    allowSorting: false,
                    lookup: {
                        dataSource: $scope.warehouseList,
                        valueExpr: "Id",
                        displayExpr: "WarehouseName"
                    }
                },
                { dataField: 'WastagePercentage', caption: 'Fire %', dataType: 'number', format: { type: "fixedPoint", precision: 2 } }
            ]
        });
    }

    $scope.loadSelectables = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'ProductRecipe/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.itemList = resp.data.Items;
                        $scope.unitList = resp.data.Units;
                        $scope.warehouseList = resp.data.Warehouses;
                        $scope.productList = resp.data.Products;

                        resolve();
                    }
                }).catch(function (err) { });
        });

        return prms;
    }

    // ON LOAD EVENTS
    DevExpress.localization.locale('tr');
    $scope.loadSelectables().then(function () {
        if (PRM_ID > 0)
            $scope.bindModel(PRM_ID);
        else {
            $scope.getNextRecipeNo().then(function (rNo) {
                $scope.modelObject.ProductRecipeCode = rNo;
                $scope.$apply();

                $scope.bindDetails();
            });
        }
    });
});