app.controller('moldCtrl', function ($scope, $http) {
    $scope.modelObject = {};

    $scope.saveStatus = 0;

    $scope.productList = [];

    // CRUD
    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0 };
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

    $scope.dropDownBoxEditorTemplate = function (cellElement, cellInfo) {
        return $("<div>").dxDropDownBox({
            dropDownOptions: { width: 500 },
            dataSource: $scope.productList,
            value: cellInfo.value,
            valueExpr: "Id",
            displayExpr: "ItemNo",
            contentTemplate: function (e) {
                return $("<div>").dxDataGrid({
                    dataSource: $scope.productList,
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
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $scope.modelObject.ProductList;
                },
                update: function (key, values) {
                    var obj = $scope.modelObject.Details.find(d => d.Id == key);
                    if (obj != null) {
                        let calculateRowAgain = false;

                        if (typeof values.ItemId != 'undefined') {
                            var itemObj = $scope.itemList.find(d => d.Id == values.ItemId);
                            obj.ItemId = itemObj.Id;
                            obj.ItemNo = itemObj.ItemNo;
                            obj.ItemName = itemObj.ItemName;

                            calculateRowAgain = true;
                        }
                        if (typeof values.UnitId != 'undefined') {
                            var unitObj = $scope.unitList.find(d => d.Id == values.UnitId);
                            obj.UnitId = unitObj.Id;
                            obj.UnitName = itemObj.UnitCode;
                            calculateRowAgain = true;
                        }

                        if (typeof values.Explanation != 'undefined') { obj.Explanation = values.Explanation; }
                        if (typeof values.Quantity != 'undefined') { obj.Quantity = values.Quantity; calculateRowAgain = true; }
                        if (typeof values.TaxRate != 'undefined') { obj.TaxRate = values.TaxRate; calculateRowAgain = true; }
                        if (typeof values.TaxIncluded != 'undefined') { obj.TaxIncluded = values.TaxIncluded; calculateRowAgain = true; }
                        if (typeof values.UnitPrice != 'undefined') { obj.UnitPrice = values.UnitPrice; calculateRowAgain = true; }
                        if (typeof values.ForexRate != 'undefined') { obj.ForexRate = values.ForexRate; calculateRowAgain = true; }
                        if (typeof values.ForexUnitPrice != 'undefined') {
                            obj.ForexUnitPrice = values.ForexUnitPrice;
                            if (typeof obj.ForexId != 'undefined' && obj.ForexId != null) {
                                obj.UnitPrice = obj.ForexUnitPrice * obj.ForexRate;
                                calculateRowAgain = true;
                            }
                        }
                        if (typeof values.ForexId != 'undefined') {
                            obj.ForexId = values.ForexId;
                            var forexObj = $scope.forexList.find(d => d.Id == obj.ForexId);

                            $http.get(HOST_URL + 'Common/GetForexRate?forexCode=' + forexObj.ForexTypeCode
                                + '&forexDate=' + $scope.modelObject.OrderDate, {}, 'json')
                                .then(function (resp) {
                                    if (typeof resp.data != 'undefined' && resp.data != null) {
                                        if (typeof resp.data.SalesForexRate != 'undefined') {
                                            obj.ForexRate = resp.data.SalesForexRate;
                                            $scope.calculateRow(obj);
                                        }
                                    }
                                }).catch(function (err) { });
                        }

                        if (calculateRowAgain)
                            $scope.calculateRow(obj);
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

                    var newObj = {
                        Id: newId,
                        ItemId: itemObj.Id,
                        ItemNo: itemObj.ItemNo,
                        ItemName: itemObj.ItemName,
                        UnitId: typeof unitObj != 'undefined' && unitObj != null ? unitObj.Id : null,
                        UnitName: typeof unitObj != 'undefined' && unitObj != null ? unitObj.UnitCode : null,
                        Quantity: values.Quantity,
                        TaxRate: values.TaxRate,
                        TaxIncluded: values.TaxIncluded,
                        UnitPrice: values.UnitPrice,
                        ForexRate: values.ForexRate,
                        ForexUnitPrice: values.ForexUnitPrice,
                        ForexId: values.ForexId,
                        ItemRequestDetailId: null,
                        Explanation: values.Explanation,
                        NewDetail: true
                    };

                    $scope.modelObject.Details.push(newObj);
                    $scope.calculateRow(newObj);
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
            height: 400,
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
            onCellPrepared: function (e) {
                if (e.rowType === "data") {
                    if (e.data.OrderStatus == 3) {
                        e.cellElement.css("background-color", "#E8FFF3");
                        //e.cellElement.css("color", "white");
                    }
                }
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
                    }
                },
                { dataField: 'Quantity', caption: 'Miktar', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, validationRules: [{ type: "required" }] },
                { dataField: 'TaxRate', caption: 'Kdv %', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                {
                    dataField: 'TaxIncluded', caption: 'Kdv D/H',
                    allowSorting: false,
                    lookup: {
                        dataSource: [{ Id: 1, Text: 'Dahil' }, { Id: 0, Text: 'Hariç' }],
                        valueExpr: "Id",
                        displayExpr: "Text"
                    },
                    validationRules: [{ type: "required" }]
                },
                { dataField: 'UnitPrice', caption: 'Birim Fiyat', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, validationRules: [{ type: "required" }] },
                {
                    dataField: 'ForexId', caption: 'Döviz Cinsi',
                    allowSorting: false,
                    lookup: {
                        dataSource: $scope.forexList,
                        valueExpr: "Id",
                        displayExpr: "ForexTypeCode"
                    }
                },
                { dataField: 'ForexRate', caption: 'Döviz Kuru', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'ForexUnitPrice', caption: 'Döviz Fiyatı', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'TaxAmount', allowEditing: false, caption: 'Kdv Tutarı', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'OverallTotal', allowEditing: false, caption: 'Satır Tutarı', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'Explanation', caption: 'Açıklama' }
            ]
        });
    }

    $scope.bindModel = function (id) {
        $http.get(HOST_URL + 'Mold/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;
                }
            }).catch(function (err) { });
    }

    // ON LOAD EVENTS
    if (PRM_ID > 0)
        $scope.bindModel(PRM_ID);
});