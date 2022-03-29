app.controller('receiptCtrl', function ($scope, $http) {
    $scope.modelObject = {
        Id: 0, ReceiptDate: moment().format('DD.MM.YYYY'),
        Details: [], ReceiptStatus: 0, ReceiptType:0
    };

    $scope.itemList = [];
    $scope.itemReceiptSatatusList = [];
    $scope.unitList = [];
    $scope.forexList = [];

    $scope.selectedFirm = {Id:0, FirmCode:''};
    $scope.firmList = [];

    $scope.selectedWarehouse = {Id:0, WarehouseName:''};
    $scope.warehouseList = [];

    $scope.selectedReceiptType = {};
    $scope.receiptTypeList = [];

    $scope.receiptCategory = null;
    $scope.saveStatus = 0;

    // RECEIPT FUNCTIONS
    $scope.getNextReceiptNo = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'ItemReceipt/GetNextReceiptNo?receiptType='
                + $scope.modelObject.ReceiptType, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        if (resp.data.Result) {
                            resolve(resp.data.ReceiptNo);
                        }
                        else {
                            toastr.error('Sıradaki sipariş numarası üretilemedi. Lütfen ekranı yenileyip tekrar deneyiniz.', 'Uyarı');
                            resolve('');
                        }
                    }
                }).catch(function (err) { });
        });

        return prms;
    }

    // SELECTABLES
    $scope.showFirmDialog = function () {
        $('#dial-firm').dialog({
            position: { my: 'left top', at: 'right top', of: $('#btnSelectFirm') },
            hide: true,
            modal: false,
            resizable: false,
            show: true,
            draggable: false,
            closeText: "KAPAT"
        });
    }
    $scope.selectFirm = function (item) {
        $scope.modelObject.FirmId = item.Id;
        $scope.modelObject.FirmCode = item.FirmCode;
        $scope.modelObject.FirmName = item.FirmName;

        $('#dial-firm').dialog("close");
    }

    // CRUD
    $scope.openNewRecord = function () {
        $scope.modelObject = {
            Id: 0, ReceiptDate: moment().format('DD.MM.YYYY'),
            Details: [], ReceiptStatus: 0, ReceiptType:0
        };

        $scope.selectedFirm = $scope.firmList[0];
        $scope.selectedWarehouse = $scope.warehouseList[0];

        $scope.getNextReceiptNo().then(function (rNo) {
            $scope.modelObject.ReceiptNo = rNo;
            $scope.$apply();
        });
        $scope.bindDetails();
    }

    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu irsaliyeyi silmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'ItemReceipt/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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

    $scope.printTemplate = function () {
        $http.post(HOST_URL + 'ItemReceipt/TestPrintDelivery', { receiptId: $scope.modelObject.Id }, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    window.open(HOST_URL + 'Outputs/' + resp.data.Path);
                }
            }).catch(function (err) { });
    }

    $scope.saveModel = function () {
        $scope.saveStatus = 1;

        if (typeof $scope.selectedFirm != 'undefined' && $scope.selectedFirm != null)
            $scope.modelObject.FirmId = $scope.selectedFirm.Id;
        else
            $scope.modelObject.FirmId = null;

        if (typeof $scope.selectedWarehouse != 'undefined' && $scope.selectedWarehouse != null)
            $scope.modelObject.InWarehouseId = $scope.selectedWarehouse.Id;
        else
            $scope.modelObject.InWarehouseId = null;

        $scope.setReceiptTypeFromSelected();

        $http.post(HOST_URL + 'ItemReceipt/SaveModel', $scope.modelObject, 'json')
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
            dataSource: $scope.itemReceiptSatatusList.filter(d => d.TotalQty > 0),
            value: cellInfo.value,
            valueExpr: "ItemId",
            displayExpr: "ItemNo",
            contentTemplate: function (e) {
                return $("<div>").dxDataGrid({
                    dataSource: $scope.itemReceiptSatatusList.filter(d => d.TotalQty > 0),
                    remoteOperations: true,
                    columns: [
                        { dataField: 'ItemNo', caption: 'Stok Kodu' },
                        { dataField: 'ItemName', caption: 'Stok Adı' },
                        { dataField: 'TotalQty', caption: 'Kalan' },
                        //{ dataField: 'GroupName', caption: 'Grup' },
                        //{ dataField: 'CategoryName', caption: 'Kategori' }
                    ],
                    hoverStateEnabled: true,
                    keyExpr: "ItemId",
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
        $http.get(HOST_URL + 'ItemReceipt/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;
                    $scope.modelObject.ReceiptDate = $scope.modelObject.ReceiptDateStr;

                    if (typeof $scope.modelObject.FirmId != 'undefined' && $scope.modelObject.FirmId != null)
                        $scope.selectedFirm = $scope.firmList.find(d => d.Id == $scope.modelObject.FirmId);
                    else
                        $scope.selectedFirm = $scope.firmList[0];

                    if (typeof $scope.modelObject.InWarehouseId != 'undefined' && $scope.modelObject.InWarehouseId != null)
                        $scope.selectedWarehouse = $scope.warehouseList.find(d => d.Id == $scope.modelObject.InWarehouseId);
                    else
                        $scope.selectedWarehouse = $scope.warehouseList[0];

                    if (typeof $scope.modelObject.ReceiptType != 'undefined' && $scope.modelObject.ReceiptType != null)
                        $scope.selectedReceiptType = $scope.receiptTypeList.find(d => d.Id == $scope.modelObject.ReceiptType);
                    else
                        $scope.selectedReceiptType = $scope.receiptTypeList[0];

                    refreshArray($scope.firmList);
                    refreshArray($scope.warehouseList);

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
                        let calculateRowAgain = false;

                        if (typeof values.ItemId != 'undefined') {
                            var itemObj = $scope.itemReceiptSatatusList.find(d => d.ItemId == values.ItemId);
                            obj.ItemId = itemObj.ItemId;
                            obj.ItemNo = itemObj.ItemNo;
                            obj.ItemName = itemObj.ItemName;

                            calculateRowAgain = true;
                        }
                        if (typeof values.UnitId != 'undefined') {
                            var unitObj = $scope.unitList.find(d => d.Id == values.UnitId);
                            obj.UnitId = unitObj.Id;
                            //obj.UnitName = itemObj.UnitCode;
                            calculateRowAgain = true;
                        }

                        if (typeof values.Explanation != 'undefined') { obj.Explanation = values.Explanation; }
                        if (typeof values.Quantity != 'undefined') {
                            if (values.Quantity > itemObj.TotalQty) {
                                toastr.warning('En fazla ekleyebileceğiniz miktar ' + itemObj.TotalQty, 'Uyarı');
                            }
                            obj.Quantity = values.Quantity; calculateRowAgain = true;
                        }
                        if (typeof values.ShortWidth != 'undefined') { obj.ShortWidth = values.ShortWidth; calculateRowAgain = true; }
                        if (typeof values.LongWidth != 'undefined') { obj.LongWidth = values.LongWidth; calculateRowAgain = true; }
                        if (typeof values.Height != 'undefined') { obj.Height = values.Height; calculateRowAgain = true; }
                        if (typeof values.Volume != 'undefined') { obj.Volume = values.Volume; calculateRowAgain = true; }
                        if (typeof values.Weight != 'undefined') { obj.Weight = values.Weight; calculateRowAgain = true; }
                        if (typeof values.Ladametre != 'undefined') { obj.Ladametre = values.Ladametre; calculateRowAgain = true; }
                        if (typeof values.Stackable != 'undefined') { obj.Stackable = values.Stackable; calculateRowAgain = true; }
                        if (typeof values.PackageInNumber != 'undefined') { obj.PackageInNumber = values.PackageInNumber; calculateRowAgain = true; }
                        //if (typeof values.TaxRate != 'undefined') { obj.TaxRate = values.TaxRate; calculateRowAgain = true; }
                        //if (typeof values.TaxIncluded != 'undefined') { obj.TaxIncluded = values.TaxIncluded; calculateRowAgain = true; }
                        //if (typeof values.UnitPrice != 'undefined') { obj.UnitPrice = values.UnitPrice; calculateRowAgain = true; }
                        //if (typeof values.ForexRate != 'undefined') { obj.ForexRate = values.ForexRate; calculateRowAgain = true; }
                        //if (typeof values.ForexUnitPrice != 'undefined') {
                        //    obj.ForexUnitPrice = values.ForexUnitPrice;
                        //    if (typeof obj.ForexId != 'undefined' && obj.ForexId != null) {
                        //        obj.UnitPrice = obj.ForexUnitPrice * obj.ForexRate;
                        //        calculateRowAgain = true;
                        //    }
                        //}
                        //if (typeof values.ForexId != 'undefined') {
                        //    obj.ForexId = values.ForexId;
                        //    var forexObj = $scope.forexList.find(d => d.Id == obj.ForexId);

                        //    $http.get(HOST_URL + 'Common/GetForexRate?forexCode=' + forexObj.ForexTypeCode
                        //        + '&forexDate=' + $scope.modelObject.OrderDate, {}, 'json')
                        //        .then(function (resp) {
                        //            if (typeof resp.data != 'undefined' && resp.data != null) {
                        //                if (typeof resp.data.SalesForexRate != 'undefined') {
                        //                    obj.ForexRate = resp.data.SalesForexRate;
                        //                    $scope.calculateRow(obj);
                        //                }
                        //            }
                        //        }).catch(function (err) { });
                        //}

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

                    var itemObj = $scope.itemReceiptSatatusList.find(d => d.ItemId == values.ItemId);
                    var unitObj = $scope.unitList.find(d => d.Id == values.UnitId);
                    var newObj = {
                        Id: newId,
                        ItemId: itemObj.ItemId,
                        ItemNo: itemObj.ItemNo,
                        ItemName: itemObj.ItemName,
                        UnitId: typeof unitObj != 'undefined' && unitObj != null ? unitObj.Id : null,
                        //UnitName: typeof unitObj != 'undefined' && unitObj != null ? unitObj.UnitCode : null,

                        Quantity: values.Quantity,
                        ShortWidth: values.ShortWidth,
                        LongWidth: values.LongWidth,
                        Length: values.Length,
                        Weight: values.Weight,
                        Height: values.Height,
                        Volume: values.Volume,
                        Ladametre: values.Ladametre,
                        Stackable: values.Stackable,
                        PackageInNumber: values.PackageInNumber,
                        //TaxRate: values.TaxRate,
                        //TaxIncluded: values.TaxIncluded,
                        //UnitPrice: values.UnitPrice,
                        //ForexRate: values.ForexRate,
                        //ForexUnitPrice: values.ForexUnitPrice,
                        //ForexId: values.ForexId,
                        ItemOrderDetailId: null,
                        Explanation: values.Explanation,
                        NewDetail: true
                    };
                    if (values.Quantity > itemObj.TotalQty) {
                        toastr.warning('Ekleyebileceğiniz maximum miktar ' + itemObj.TotalQty, 'Uyarı');
                    } else {
                        $scope.modelObject.Details.push(newObj);
                        $scope.calculateRow(newObj);
                    }
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
            columns: [
                {
                    dataField: 'ItemId', caption: 'Stok Kodu',
                    lookup: {
                        dataSource: $scope.itemReceiptSatatusList,
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
                { dataField: 'ShortWidth', caption: 'Kısa En (CM)', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'LongWidth', caption: 'Uzun En (CM)', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'Height', caption: 'Yükseklik (CM)', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'Weight', caption: 'Ağırlık (KG)', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'Volume', caption: 'Hacim (M3)', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'Ladametre', caption: 'Ladametre', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'Stackable', caption: 'İstiflenebilir', dataType: 'boolean', width: 80 },
                { dataField: 'PackageInNumber', caption: 'Koli İç Adet', dataType: 'number' },
                //{ dataField: 'TaxRate', caption: 'Kdv %', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                //{
                //    dataField: 'TaxIncluded', caption: 'Kdv D/H',
                //    allowSorting: false,
                //    lookup: {
                //        dataSource: [{ Id: 1, Text: 'Dahil' }, { Id: 0, Text: 'Hariç' }],
                //        valueExpr: "Id",
                //        displayExpr: "Text"
                //    },
                //    validationRules: [{ type: "required" }]
                //},
                //{ dataField: 'UnitPrice', caption: 'Birim Fiyat', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, validationRules: [{ type: "required" }] },
                //{
                //    dataField: 'ForexId', caption: 'Döviz Cinsi',
                //    allowSorting: false,
                //    lookup: {
                //        dataSource: $scope.forexList,
                //        valueExpr: "Id",
                //        displayExpr: "ForexTypeCode"
                //    }
                //},
                //{ dataField: 'ForexRate', caption: 'Döviz Kuru', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                //{ dataField: 'ForexUnitPrice', caption: 'Döviz Fiyatı', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                //{ dataField: 'TaxAmount', allowEditing: false, caption: 'Kdv Tutarı', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                //{ dataField: 'OverallTotal', allowEditing: false, caption: 'Satır Tutarı', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'Explanation', caption: 'Açıklama' }
            ],
            summary: {
                totalItems: [{
                    column: 'Quantity',
                    summaryType: 'sum',
                },
                {
                    column: 'Weight',
                    summaryType: 'sum',
                    format: { type: "fixedPoint", precision: 2 }
                },
                {
                    column: 'Volume',
                    summaryType: 'sum',
                    format: { type: "fixedPoint", precision: 2 }
                },
                {
                    column: 'Ladametre',
                    summaryType: 'sum',
                    format: { type: "fixedPoint", precision: 2 }
                }]
            },

        });
    }

    $scope.bindParameters = function () {
        $scope.receiptCategory = getParameterByName('receiptCategory');
        $scope.modelObject.ReceiptType = getParameterByName('receiptType');
        if ($scope.modelObject.ReceiptType == null)
            $scope.modelObject.ReceiptType = 0;
    }

    $scope.onReceiptTypeChanged = function (e) {
        $scope.setReceiptTypeFromSelected();
        $scope.getNextReceiptNo();
    }

    $scope.setReceiptTypeFromSelected = function () {
        if (typeof $scope.selectedReceiptType != 'undefined'
            && $scope.selectedReceiptType != null
            && typeof $scope.selectedReceiptType.Id != 'undefined')
            $scope.modelObject.ReceiptType = $scope.selectedReceiptType.Id;
        else
            $scope.modelObject.ReceiptType = 0;
    }
    $scope.calculateRow = function (row) {
        $scope.calculateValumeAndLadametre(row);

    }
    $scope.calculateValumeAndLadametre = function (row) {
        if (row.ShortWidth == 'undefined' || row.ShortWidth == null)
            row.ShortWidth = 0;
        if (row.LongWidth == 'undefined' || row.LongWidth == null)
            row.LongWidth = 0;
        if (row.Height == 'undefined' || row.Height == null)
            row.Height = 0;
        row.Volume = row.Quantity * row.ShortWidth * row.LongWidth * row.Height / 1000000;
        //CALCULATE VALUME AND WEIGHT AND LADAMETRE
        let sumVolume = 0;
        let sumWeight = 0;
        let = sumLadametre = 0;
        let = sumQuantity = 0;
        $scope.modelObject.Details.forEach(element => {
            sumVolume += parseFloat(element.Volume != null ? element.Volume : 0);
            sumWeight += parseFloat(element.Weight != null ? element.Weight : 0);
            sumLadametre += parseFloat(element.Ladametre != null ? element.Ladametre : 0);
            sumQuantity += parseFloat(element.Quantity != null ? element.Quantity : 0);
        });
        $scope.modelObject.OveralVolume = sumVolume;
        $scope.modelObject.OveralWeight = sumWeight;
        $scope.modelObject.OveralLadametre = sumLadametre;
        $scope.modelObject.OveralQuantity = sumQuantity;

    }

    $scope.loadSelectables = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'ItemReceipt/GetSelectables?receiptCategory=' +
                $scope.receiptCategory, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.itemList = resp.data.Items;
                        $scope.itemReceiptSatatusList = resp.data.ItemReceiptStatusList;
                        $scope.unitList = resp.data.Units;
                        $scope.forexList = resp.data.Forexes;

                        $scope.firmList = resp.data.Firms;
                        var emptyFirmObj = { Id: 0, FirmCode: '-- Seçiniz --' };
                        $scope.firmList.splice(0, 0, emptyFirmObj);
                        $scope.selectedFirm = emptyFirmObj;
                        
                        $scope.warehouseList = resp.data.Warehouses;
                        var emptyWrObj = { Id: 0, WarehouseName: '-- Seçiniz --' };
                        $scope.warehouseList.splice(0, 0, emptyWrObj);
                        $scope.selectedWarehouse = emptyWrObj;

                        $scope.receiptTypeList = resp.data.ReceiptTypes;
                        $scope.selectedReceiptType = $scope.receiptTypeList[0];

                        resolve();
                    }
                }).catch(function (err) { });
        });

        return prms;
    }

    // INFORMATIONS
    $scope.showRecordInformation = function () {
        $scope.$broadcast('showRecordInformation', { Id: $scope.modelObject.Id, DataType:'ItemReceipt' });
    }

    $scope.showOrderInformation = function () {
        $scope.$broadcast('loadRelatedOrderList', $scope.modelObject.Id);

        $('#dial-related-orders').dialog({
            width: 500,
            //height: window.innerHeight * 0.6,
            hide: true,
            modal: true,
            resizable: false,
            show: true,
            draggable: false,
            closeText: "KAPAT"
        });
    }

    // APPROVALS
    $scope.showItemOrderList = function () {
        // DO BROADCAST
        $scope.$broadcast('loadOpenPoList');

        $('#dial-orderlist').dialog({
            width: window.innerWidth * 0.6,
            height: window.innerHeight * 0.6,
            hide: true,
            modal: true,
            resizable: false,
            show: true,
            draggable: false,
            closeText: "KAPAT"
        });
    }

    $scope.$on('transferOrderDetails', function (e, d) {
        d.forEach(x => {
            if ($scope.modelObject.Details.filter(m => m.ItemOrderDetailId == x.Id).length > 0) {
                toastr.warning(x.OrderNo + ' nolu sipariş, ' + x.ItemNo + ' / ' + x.ItemName + ', ' + x.Quantity
                    + ' miktarlı sipariş detayı zaten aktarıldığı için tekrar dahil edilmedi.', 'Uyarı');
            }
            else {
                var newId = 1;
                if ($scope.modelObject.Details.length > 0) {
                    newId = $scope.modelObject.Details.map(d => d.Id).reduce((max, n) => n > max ? n : max)
                    newId++;
                }

                $scope.modelObject.Details.push({
                    Id: newId,
                    ItemId: x.ItemId,
                    ItemNo: x.ItemNo,
                    ItemName: x.ItemName,
                    UnitId: x.UnitId,
                    UnitName: x.UnitCode,
                    Quantity: x.Quantity,
                    TaxIncluded: false,
                    TaxRate:0,
                    UnitPrice: 0,
                    NewDetail: true,
                    ItemOrderDetailId: x.Id
                });

                var detailsGrid = $("#dataList").dxDataGrid("instance");
                detailsGrid.refresh();
            }
        });

        $('#dial-orderlist').dialog('close');
    });

    // ON LOAD EVENTS
    DevExpress.localization.locale('tr');
    $scope.bindParameters();
    $scope.loadSelectables().then(function () {
        if (PRM_ID > 0)
            $scope.bindModel(PRM_ID);
        else {
            $scope.getNextReceiptNo().then(function (rNo) {
                $scope.modelObject.ReceiptNo = rNo;
                $scope.$apply();

                $scope.bindDetails();
            });
        }
    });
});