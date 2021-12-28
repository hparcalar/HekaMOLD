app.controller('itemCtrl', function ($scope, $http) {
    $scope.modelObject = {};

    $scope.saveStatus = 0;

    $scope.selectedItemType = {};
    $scope.itemTypeList = [{ Id: 1, Text: 'Hammadde' }, { Id: 2, Text: 'Ticari Mal' },
        { Id: 3, Text: 'Yarı Mamul' }, { Id: 3, Text: 'Mamul' }];

    $scope.selectedCategory = {};
    $scope.categoryList = [];
    $scope.firmList = [];

    $scope.selectedFirm = {};
    $scope.selectedGroup = {};
    $scope.groupList = [];

    $scope.unitList = [];

    // GET SELECTABLE DATA
    $scope.loadSelectables = function () {
        var prmReq = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Item/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.categoryList = resp.data.Categories;
                        $scope.groupList = resp.data.Groups;
                        $scope.firmList = resp.data.Firms;
                        $scope.unitList = resp.data.Units;

                        resolve(resp.data);
                    }
                }).catch(function (err) { });
        });

        return prmReq;
    }

    // CRUD
    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0 };
        $scope.selectedFirmType = {};
    }

    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu stok tanımını silmek istediğinizden emin misiniz?",
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

    $scope.bindModel = function (id) {
        $http.get(HOST_URL + 'Item/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;

                    // BIND EXTERNAL TYPES
                    if ($scope.modelObject.ItemType > 0)
                        $scope.selectedItemType = $scope.itemTypeList.find(d => d.Id == $scope.modelObject.ItemType);
                    else
                        $scope.selectedItemType = {};

                    if ($scope.modelObject.ItemCategoryId > 0)
                        $scope.selectedCategory = $scope.categoryList.find(d => d.Id == $scope.modelObject.ItemCategoryId);
                    else
                        $scope.selectedCategory = {};

                    if ($scope.modelObject.ItemGroupId > 0)
                        $scope.selectedGroup = $scope.groupList.find(d => d.Id == $scope.modelObject.ItemGroupId);
                    else
                        $scope.selectedGroup = {};

                    if (typeof $scope.modelObject.SupplierFirmId != 'undefined' && $scope.modelObject.SupplierFirmId != null)
                        $scope.selectedFirm = $scope.firmList.find(d => d.Id == $scope.modelObject.SupplierFirmId);
                    else
                        $scope.selectedFirm = {};

                    $scope.bindWarehousePrm();
                    $scope.bindUnitPrm();
                }
            }).catch(function (err) { });
    }

    $scope.bindWarehousePrm = function () {
        var getProperWarehouses = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Item/GetProperWarehouses?itemType=' + $scope.selectedItemType.Id +
                '&itemId=' + $scope.modelObject.Id, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        resolve(resp.data);
                    }
                }).catch(function (err) { });
        });

        getProperWarehouses.then(function (warehouseData) {
            $scope.modelObject.Warehouses = warehouseData;

            $('#warehouseList').dxDataGrid({
                dataSource: {
                    load: function () {
                        return $scope.modelObject.Warehouses;
                    },
                    update: function (key, values) {
                        var obj = $scope.modelObject.Warehouses.find(d => d.WarehouseId == key);
                        if (obj != null) {
                            if (typeof values.IsAllowed != 'undefined') { obj.IsAllowed = values.IsAllowed; }
                            if (typeof values.MinimumQuantity != 'undefined') { obj.MinimumQuantity = values.MinimumQuantity; }
                            if (typeof values.MaximumQuantity != 'undefined') { obj.MaximumQuantity = values.MaximumQuantity; }
                            if (typeof values.MinimumBehaviour != 'undefined') { obj.MinimumBehaviour = values.MinimumBehaviour; }
                            if (typeof values.MaximumBehaviour != 'undefined') { obj.MaximumBehaviour = values.MaximumBehaviour; }
                        }
                    },
                    remove: function (key) { },
                    insert: function (values) { },
                    key: 'WarehouseId'
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
                height: 200,
                editing: {
                    allowUpdating: true,
                    allowDeleting: false,
                    allowAdding: false,
                    mode: 'cell'
                },
                columns: [
                    { dataField: 'WarehouseName', caption: 'Depo', allowEditing: false },
                    { dataField: 'IsAllowed', caption: 'Hareket Görebilir' },
                    { dataField: 'MinimumQuantity', caption: 'Minimum', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                    { dataField: 'MaximumQuantity', caption: 'Maksimum', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                    {
                        dataField: 'MinimumBehaviour', caption: 'Minimum Aksiyonu',
                        allowSorting: false,
                        lookup: {
                            dataSource: [{ Id: 1, Text: 'Yok' }, { Id: 2, Text: 'Uyar' }, { Id: 3, Text: 'İzin Verme' }],
                            valueExpr: "Id",
                            displayExpr: "Text"
                        }
                    },
                    {
                        dataField: 'MaximumBehaviour', caption: 'Maksimum Aksiyonu',
                        allowSorting: false,
                        lookup: {
                            dataSource: [{ Id: 1, Text: 'Yok' }, { Id: 2, Text: 'Uyar' }, { Id: 3, Text: 'İzin Verme' }],
                            valueExpr: "Id",
                            displayExpr: "Text"
                        }
                    }
                ]
            });
        });
    }

    $scope.bindUnitPrm = function () {
        $('#unitList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $scope.modelObject.Units;
                },
                update: function (key, values) {
                    var obj = $scope.modelObject.Units.find(d => d.Id == key);
                    if (obj != null) {

                        if (typeof values.UnitId != 'undefined') {
                            var unitObj = $scope.unitList.find(d => d.Id == values.UnitId);
                            obj.UnitId = unitObj.Id;
                            obj.UnitCode = itemObj.UnitCode;
                        }

                        if (typeof values.IsMainUnit != 'undefined') { obj.IsMainUnit = values.IsMainUnit; }
                        if (typeof values.MultiplierFactor != 'undefined') { obj.MultiplierFactor = values.MultiplierFactor; }
                        if (typeof values.DividerFactor != 'undefined') { obj.DividerFactor = values.DividerFactor; }
                    }
                },
                remove: function (key) {
                    var obj = $scope.modelObject.Units.find(d => d.Id == key);
                    if (obj != null) {
                        $scope.modelObject.Units.splice($scope.modelObject.Units.indexOf(obj), 1);
                    }
                },
                insert: function (values) {
                    var newId = 1;
                    if ($scope.modelObject.Units.length > 0) {
                        newId = $scope.modelObject.Units.map(d => d.Id).reduce((max, n) => n > max ? n : max)
                        newId++;
                    }

                    var unitObj = $scope.unitList.find(d => d.Id == values.UnitId);

                    var newObj = {
                        Id: newId,
                        UnitId: typeof unitObj != 'undefined' && unitObj != null ? unitObj.Id : null,
                        UnitCode: typeof unitObj != 'undefined' && unitObj != null ? unitObj.UnitCode : null,
                        IsMainUnit: values.IsMainUnit,
                        MultiplierFactor: values.MultiplierFactor,
                        DividerFactor: values.DividerFactor,
                        NewDetail: true
                    };

                    $scope.modelObject.Units.push(newObj);
                },
                key: 'Id'
            },
            showColumnLines: true,
            showRowLines: true,
            rowAlternationEnabled: true,
            focusedRowEnabled: false,
            showBorders: true,
            onInitNewRow: function (e) {
                e.data.IsMainUnit = false;
            },
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
            height: 200,
            editing: {
                allowUpdating: true,
                allowDeleting: true,
                allowAdding: true,
                mode: 'cell'
            },
            columns: [
                {
                    dataField: 'UnitId', caption: 'Birim',
                    validationRules: [{ type: "required" }],
                    allowSorting: false,
                    lookup: {
                        dataSource: $scope.unitList,
                        valueExpr: "Id",
                        displayExpr: "UnitCode"
                    }
                },
                { dataField: 'IsMainUnit', caption: 'Ana Birim', dataType:'boolean' },
                { dataField: 'MultiplierFactor', caption: 'Çarpan', dataType: 'number', format: { type: "fixedPoint", precision: 0 } },
                { dataField: 'DividerFactor', caption: 'Bölen', dataType: 'number', format: { type: "fixedPoint", precision: 0 } }
            ]
        });
    }

    $scope.showAttachmentList = function () {
        $scope.$broadcast('showAttachmentList',
            { RecordId: $scope.modelObject.Id, RecordType: 2 });

        $('#dial-attachments').dialog({
            width: 500,
            height: 400,
            //height: window.innerHeight * 0.6,
            hide: true,
            modal: true,
            resizable: false,
            show: true,
            draggable: false,
            closeText: "KAPAT"
        });
    }

    $scope.showPrintLabelDialog = function () {
        $scope.$broadcast('showItemPrinting',
            $scope.modelObject.Id);

        $('#dial-printing').dialog({
            width: 500,
            height: 400,
            //height: window.innerHeight * 0.6,
            hide: true,
            modal: true,
            resizable: false,
            show: true,
            draggable: false,
            closeText: "KAPAT"
        });
    }

    // ON LOAD EVENTS
    DevExpress.localization.locale('tr');
    $scope.loadSelectables().then(function (data) {
        if (PRM_ID > 0)
            $scope.bindModel(PRM_ID);
        else
            $scope.bindModel(0);
    });
});