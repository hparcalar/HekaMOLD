app.controller('moldCtrl', function ($scope, $http) {
    $scope.modelObject = { Id:0, Products: [] };

    $scope.saveStatus = 0;

    $scope.itemList = [];
    $scope.firmList = [];
    $scope.movementList = [];
    $scope.selectedFirm = {};

    // CRUD
    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0, Products: [] };
    }

    $scope.loadSelectables = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Mold/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.itemList = resp.data.Items;
                        $scope.firmList = resp.data.Firms;

                        var emptyFirmObj = { Id: 0, FirmCode: '-- Seçiniz --' };
                        $scope.firmList.splice(0, 0, emptyFirmObj);
                        $scope.selectedFirm = emptyFirmObj;

                        resolve();
                    }
                }).catch(function (err) { });
        });

        return prms;
    }

    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu kalıp tanımını silmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'Mold/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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

        if (typeof $scope.selectedFirm != 'undefined' && $scope.selectedFirm != null && $scope.selectedFirm.Id > 0)
            $scope.modelObject.FirmId = $scope.selectedFirm.Id;
        else
            $scope.modelObject.FirmId = null;

        $http.post(HOST_URL + 'Mold/SaveModel', $scope.modelObject, 'json')
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

    $scope.showMoldHistory = function () {
        try {
            $http.get(HOST_URL + 'Mold/GetMoldMovementHistory?moldId=' + $scope.modelObject.Id, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.movementList = resp.data;
                    }

                    $('#dial-history').dialog({
                        width: 700,
                        height: window.innerHeight * 0.6,
                        hide: true,
                        modal: true,
                        resizable: false,
                        show: true,
                        draggable: false,
                        closeText: "KAPAT"
                    });
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.printLabel = function () {
        $('#dial-print-label').dialog({
            width: 520,
            height: 520,
            hide: true,
            modal: true,
            resizable: false,
            show: true,
            draggable: false,
            closeText: "KAPAT"
        });
    }

    $scope.printMoldLabel = function () {
        var printContents = document.getElementById('mold-label').innerHTML;
        var originalContents = document.body.innerHTML;
        document.body.innerHTML = printContents;
        window.print();
        document.body.innerHTML = originalContents;
        window.location.reload();
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
                        { dataField: 'ItemNo', caption: 'Ürün Kodu' },
                        { dataField: 'ItemName', caption: 'Ürün Adı' },
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

    $scope.bindDetails = function () {
        $('#productList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $scope.modelObject.Products;
                },
                update: function (key, values) {
                    var obj = $scope.modelObject.Products.find(d => d.Id == key);
                    if (obj != null) {

                        if (typeof values.ItemId != 'undefined') {
                            var itemObj = $scope.itemList.find(d => d.Id == values.ProductId);
                            obj.ProductId = itemObj.Id;
                            obj.ProductCode = itemObj.ItemNo;
                            obj.ProductName = itemObj.ItemName;
                        }

                        if (typeof values.LineNumber != 'undefined') { obj.LineNumber = values.LineNumber; }
                    }
                },
                remove: function (key) {
                    var obj = $scope.modelObject.Products.find(d => d.Id == key);
                    if (obj != null) {
                        $scope.modelObject.Products.splice($scope.modelObject.Products.indexOf(obj), 1);
                    }
                },
                insert: function (values) {
                    var newId = 1;
                    if ($scope.modelObject.Products.length > 0) {
                        newId = $scope.modelObject.Products.map(d => d.Id).reduce((max, n) => n > max ? n : max)
                        newId++;
                    }

                    var itemObj = $scope.itemList.find(d => d.Id == values.ProductId);

                    var newObj = {
                        Id: newId,
                        ProductId: itemObj.Id,
                        ProductCode: itemObj.ItemNo,
                        ProductName: itemObj.ItemName,
                        LineNumber: newId,
                        NewDetail: true
                    };

                    $scope.modelObject.Products.push(newObj);
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
            height: 200,
            editing: {
                allowUpdating: true,
                allowDeleting: true,
                allowAdding: true,
                mode: 'cell'
            },
            onInitNewRow: function (e) {
                e.data.UnitPrice = 0;
                e.data.TaxIncluded = 0;
            },
            repaintChangesOnly: true,
            columns: [
                {
                    allowEditing: false, caption: '#', width:50,
                    cellTemplate: function (cellElement, cellInfo) {
                        cellElement.text(cellInfo.row.rowIndex + 1)
                    }
                },
                {
                    dataField: 'ProductId', caption: 'Ürün Kodu',
                    lookup: {
                        dataSource: $scope.itemList,
                        valueExpr: "Id",
                        displayExpr: "ItemNo"
                    },
                    allowSorting: false,
                    validationRules: [{ type: "required" }],
                    editCellTemplate: $scope.dropDownBoxEditorTemplate,
                    cellTemplate: function (container, options) {
                        if (typeof options.row.data.ProductCode != 'undefined'
                            && options.row.data.ProductCode != null && options.row.data.ProductCode.length > 0)
                            container.text(options.row.data.ProductCode);
                        else
                            container.text(options.displayValue);
                    }
                },
                { dataField: 'ProductName', caption: 'Ürün Adı', allowEditing: false },
            ]
        });
    }

    $scope.bindModel = function (id) {
        $http.get(HOST_URL + 'Mold/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;

                    if ($scope.modelObject.Id <= 0) {
                        $scope.modelObject.CreatedDateStr = moment().format('DD.MM.YYYY');
                        $scope.modelObject.Details = [];
                    }
                    else {
                        if ($scope.modelObject.CreatedDateStr == null)
                            $scope.modelObject.CreatedDateStr = moment().format('DD.MM.YYYY');
                    }

                    // BIND SELECTED FIRM
                    if (typeof $scope.modelObject.FirmId != 'undefined' && $scope.modelObject.FirmId != null)
                        $scope.selectedFirm = $scope.firmList.find(d => d.Id == $scope.modelObject.FirmId);
                    else
                        $scope.selectedFirm = $scope.firmList[0];

                    refreshArray($scope.firmList);

                    $scope.bindDetails();
                }
            }).catch(function (err) { });
    }

    // ON LOAD EVENTS
    DevExpress.localization.locale('tr');
    $scope.loadSelectables().then(function () {
        if (PRM_ID > 0)
            $scope.bindModel(PRM_ID);
        else
            $scope.bindModel(0);
    });
});