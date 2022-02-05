﻿app.controller('receiptCtrl', function ($scope, $http) {
    $scope.modelObject = {
        Id: 0, ReceiptDate: moment().format('DD.MM.YYYY'),
        Details: [], ReceiptStatus: 0, ReceiptType:0, PriceCalcType: 0,
    };

    // #region PAGE VARIABLES
    $scope.itemList = [];
    $scope.unitList = [];
    $scope.forexList = [];
    
    $scope.reportTemplateId = 0;

    $scope.selectedRow = { Id: 0 };

    $scope.selectedCalcType = { Id:0, Code:'Miktar' };
    $scope.calcTypeList = [{ Id: 0, Code: 'Miktar' }, { Id: 1, Code: 'Kg' }];

    $scope.selectedFirm = {Id:0, FirmCode:''};
    $scope.firmList = [];

    $scope.selectedPlant = { Id: 0, PlantCode:'', PlantName: '' };
    $scope.plantList = [];

    $scope.selectedWarehouse = {Id:0, WarehouseName:''};
    $scope.warehouseList = [];

    $scope.selectedReceiptType = {};
    $scope.receiptTypeList = [];

    $scope.receiptCategory = null;
    $scope.saveStatus = 0;
    // #endregion

    // #region CRUD AND RECEIPT LOGIC INTERACTIONS
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

    $scope.openNewRecord = function () {
        $scope.modelObject = {
            Id: 0, ReceiptDate: moment().format('DD.MM.YYYY'),
            Details: [], ReceiptStatus: 0, ReceiptType:0, PriceCalcType: 0,
        };

        $scope.selectedFirm = $scope.firmList[0];
        $scope.selectedWarehouse = $scope.warehouseList[0];
        $scope.selectedPlant = $scope.plantList[0];

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

        if (typeof $scope.selectedPlant != 'undefined' && $scope.selectedPlant != null)
            $scope.modelObject.ReceiverPlantId = $scope.selectedPlant.Id;
        else
            $scope.modelObject.ReceiverPlantId = null;

        if (typeof $scope.selectedCalcType != 'undefined' && $scope.selectedCalcType != null)
            $scope.modelObject.PriceCalcType = $scope.selectedCalcType.Id;
        else
            $scope.modelObject.PriceCalcType = null;

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
            dropDownOptions: { width: 600 },
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

                    if (typeof $scope.modelObject.ReceiverPlantId != 'undefined' && $scope.modelObject.ReceiverPlantId != null)
                        $scope.selectedPlant = $scope.plantList.find(d => d.Id == $scope.modelObject.ReceiverPlantId);
                    else
                        $scope.selectedPlant = $scope.plantList[0];

                    if (typeof $scope.modelObject.PriceCalcType != 'undefined' && $scope.modelObject.PriceCalcType != null)
                        $scope.selectedCalcType = $scope.calcTypeList.find(d => d.Id == $scope.modelObject.PriceCalcType);
                    else
                        $scope.selectedCalcType = $scope.calcTypeList[0];

                    refreshArray($scope.firmList);
                    refreshArray($scope.warehouseList);
                    refreshArray($scope.plantList);

                    $scope.bindDetails();
                    $scope.calculateHeader();
                }
            }).catch(function (err) { });
    }

    $scope.refreshDetailChanges = function () {
        var dataGrid = $("#dataList").dxDataGrid("instance");
        dataGrid.refresh(true);
    }

    $scope.calculateRow = function (row) {
        if (typeof row != 'undefined' && row != null) {
            try {
                row.PriceCalcType = $scope.selectedCalcType.Id;
                $http.post(HOST_URL + 'ItemReceipt/CalculateRow', row, 'json')
                    .then(function (resp) {
                        if (typeof resp.data != 'undefined' && resp.data != null) {
                            row.OverallTotal = resp.data.OverallTotal;
                            row.UnitPrice = resp.data.UnitPrice;
                            row.TaxAmount = resp.data.TaxAmount;
                            row.ForexUnitPrice = resp.data.ForexUnitPrice;
                            row.NetQuantity = resp.data.NetQuantity;
                            row.SubTotal = resp.data.SubTotal;

                            $scope.refreshDetailChanges();
                            $scope.calculateHeader();
                        }
                    }).catch(function (err) { });
            } catch (e) {

            }
        }
    }

    $scope.calculateHeader = function () {
        $scope.modelObject.SubTotal = $scope.modelObject.Details.map(d => d.SubTotal).reduce((n, x) => n + x);
        $scope.modelObject.TaxPrice = $scope.modelObject.Details.map(d => d.TaxAmount).reduce((n, x) => n + x);
        $scope.modelObject.OverallTotal = $scope.modelObject.Details.map(d => d.OverallTotal).reduce((n, x) => n + x);
    }

    $scope.bindDetails = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    $scope.modelObject.Details.forEach(d => {
                        if (d.TaxIncluded == true)
                            d.TaxIncluded = 1;
                        else if (d.TaxIncluded == false)
                            d.TaxIncluded = 0;
                    });

                    return $scope.modelObject.Details;
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
                            obj.UnitName = unitObj.UnitCode;
                            calculateRowAgain = true;
                        }

                        if (typeof values.Explanation != 'undefined') { obj.Explanation = values.Explanation; }
                        if (typeof values.Quantity != 'undefined') { obj.Quantity = values.Quantity; calculateRowAgain = true; }
                        if (typeof values.WeightQuantity != 'undefined') { obj.WeightQuantity = values.WeightQuantity; calculateRowAgain = true; }
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
                                + '&forexDate=' + $scope.modelObject.ReceiptDate, {}, 'json')
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
                        WeightQuantity: values.WeightQuantity,
                        TaxRate: values.TaxRate,
                        TaxIncluded: values.TaxIncluded,
                        UnitPrice: values.UnitPrice,
                        ForexRate: values.ForexRate,
                        ForexUnitPrice: values.ForexUnitPrice,
                        ForexId: values.ForexId,
                        ItemOrderDetailId: null,
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
            height: 250,
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
                {
                    dataField: 'Quantity', caption: 'Miktar', dataType: 'number',
                    format: { type: "fixedPoint", precision: 2 }, allowEditing:true,
                    editorOptions: { format: { type: "fixedPoint", precision: 2 } },
                    validationRules: [{ type: "required" }]
                },
                {
                    dataField: 'WeightQuantity', caption: 'Kg', dataType: 'number',
                    cssClass:'bg-light-primary',
                    format: { type: "fixedPoint", precision: 2 }, allowEditing: true,
                    editorOptions: { format: { type: "fixedPoint", precision: 2 } },
                },
                //{
                //    cssClass:'bg-secondary',
                //    dataField: 'NetQuantity', allowEditing: false, caption: 'Net Miktar', dataType: 'number',
                //    format: { type: "fixedPoint", precision: 2 },
                //},
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
                {
                    dataField: 'UnitPrice', caption: 'Birim Fiyat', dataType: 'number',
                    format: { type: "fixedPoint", precision: 2 },
                    editorOptions: { format: { type: "fixedPoint", precision: 2 } },
                    validationRules: [{ type: "required" }]
                },
                {
                    dataField: 'ForexId', caption: 'Döviz Cinsi',
                    allowSorting: false,
                    lookup: {
                        dataSource: $scope.forexList,
                        valueExpr: "Id",
                        displayExpr: "ForexTypeCode"
                    }
                },
                {
                    dataField: 'ForexRate', caption: 'Döviz Kuru', dataType: 'number',
                    format: { type: "fixedPoint", precision: 2 },
                    editorOptions: { format: { type: "fixedPoint", precision: 2 } }
                },
                {
                    dataField: 'ForexUnitPrice', caption: 'Döviz Fiyatı', dataType: 'number',
                    format: { type: "fixedPoint", precision: 2 },
                    editorOptions: { format: { type: "fixedPoint", precision: 2 } }
                },
                { dataField: 'TaxAmount', allowEditing: false, caption: 'Kdv Tutarı', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'SubTotal', allowEditing: false, caption: 'Satır Tutarı', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'Explanation', caption: 'Açıklama' },
                {
                    type: "buttons",
                    buttons: [
                        {
                            name: 'delete', cssClass: '', text: '', onClick: function (e) {
                                $('#dataList').dxDataGrid('instance').deleteRow(e.row.rowIndex);
                            }
                        },
                        {
                            name: 'preview', cssClass: 'btn btn-sm btn-light-primary py-0 px-1', text: '...', onClick: function (e) {
                                var dataGrid = $("#dataList").dxDataGrid("instance");
                                $scope.selectedRow = e.row.data;
                                $scope.showRowMenu();
                            }
                        }
                    ]
                }
            ]
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

    $scope.onCalcTypeChanged = function (e) {
        $scope.modelObject.Details.forEach(d => {
            $scope.calculateRow(d);
        });
    }

    $scope.setReceiptTypeFromSelected = function () {
        if (typeof $scope.selectedReceiptType != 'undefined'
            && $scope.selectedReceiptType != null
            && typeof $scope.selectedReceiptType.Id != 'undefined')
            $scope.modelObject.ReceiptType = $scope.selectedReceiptType.Id;
        else
            $scope.modelObject.ReceiptType = 0;
    }

    $scope.loadSelectables = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'ItemReceipt/GetSelectables?receiptCategory=' +
                $scope.receiptCategory, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.itemList = resp.data.Items;
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

                        $scope.plantList = resp.data.Plants;
                        var emptyPlantObj = { Id: 0, PlantCode: '', PlantName: '-- Seçiniz --' };
                        $scope.plantList.splice(0, 0, emptyPlantObj);
                        $scope.selectedPlant = emptyPlantObj;

                        $scope.receiptTypeList = resp.data.ReceiptTypes;
                        $scope.selectedReceiptType = $scope.receiptTypeList[0];

                        resolve();
                    }
                }).catch(function (err) { });
        });

        return prms;
    }
    // #endregion

    // #region ROW MENU ACTIONS
    $scope.showRowMenu = function () {
        if ($scope.selectedRow && $scope.selectedRow.Id > 0) {
            $('#dial-row-menu').dialog({
                width: 300,
                //height: window.innerHeight * 0.6,
                hide: true,
                modal: true,
                resizable: false,
                show: true,
                draggable: false,
                closeText: "KAPAT"
            });
        }
    }

    $scope.onRowMenuItemClicked = function () {
        $('#dial-row-menu').dialog("close");
    }
    // #endregion

    // #region PRINTING LOGIC
    $scope.showPrintTemplates = function () {
        if ($scope.selectedRow.Id > 0) {
            // DO BROADCAST
            $scope.$broadcast('loadTemplateList', [6]);

            $('#dial-reports').dialog({
                width: window.innerWidth * 0.65,
                height: window.innerHeight * 0.65,
                hide: true,
                modal: true,
                resizable: false,
                show: true,
                draggable: false,
                closeText: "KAPAT"
            });
        }
    }

    $scope.showPrintOptions = function () {
        $scope.$broadcast('showPrintOptions');

        $('#dial-print-options').dialog({
            hide: true,
            modal: true,
            resizable: false,
            width: 300,
            show: true,
            draggable: false,
            closeText: "KAPAT"
        });
    }

    // RECEIVE EMIT REPORT PRINT DATA
    $scope.$on('printTemplate', function (e, d) {
        if (d.templateId > 0) {
            $scope.reportTemplateId = d.templateId;

            if (d.exportType == 'PDF') {
                try {
                    $http.post(HOST_URL + 'Printing/ExportAsPdf', {
                        objectId: $scope.selectedRow.Id,
                        reportId: $scope.reportTemplateId,
                        reportType: 6,
                    }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                window.open(HOST_URL + 'Outputs/' + resp.data.Path);
                            }
                        }).catch(function (err) { });
                } catch (e) {

                }
            }
            else {
                $scope.showPrintOptions();
            }
        }

        $('#dial-reports').dialog('close');
    });

    // RECEIVE EMIT PRINTING OPTIONS DATA
    $scope.$on('printOptionsApproved', function (e, d) {
        $('#dial-print-options').dialog('close');

        try {
            $http.post(HOST_URL + 'Printing/AddToPrintQueue', {
                objectId: $scope.selectedRow.Id,
                reportId: $scope.reportTemplateId,
                printerId: d.PrinterId,
                recordType: 6, // item receipt detail type
            }, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        if (resp.data.Status == 1)
                            toastr.success('İstek yazıcıya iletildi.', 'Bilgilendirme');
                        else
                            toastr.error('Hata: ' + resp.data.ErrorMessage, 'Uyarı');
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    });

    $scope.printMaterialLabel = function () {
        $scope.onRowMenuItemClicked();
        $scope.showPrintTemplates();
    }
    // #endregion

    // #region INFORMATIONS
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
    // #endregion

    // #region TRANSFER ORDERS TO RECEIPT
    $scope.showItemOrderList = function () {
        if ($scope.modelObject.ReceiptType > 100) {
            // DO BROADCAST
            $scope.$broadcast('loadOpenSoList');

            $('#dial-orderlist-so').dialog({
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
        else {
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

                if (x.FirmId != null) {
                    $scope.selectedFirm = $scope.firmList.find(d => d.Id == x.FirmId);
                    refreshArray($scope.firmList);
                }
            }
        });

        if ($scope.modelObject.ReceiptType > 100)
            $('#dial-orderlist-so').dialog('close');
        else
            $('#dial-orderlist').dialog('close');
    });
    // #endregion

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