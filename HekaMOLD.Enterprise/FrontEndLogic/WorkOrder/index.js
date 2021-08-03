app.controller('workOrderCtrl', function ($scope, $http) {
    $scope.modelObject = { Id: 0, OrderDate: moment().format('DD.MM.YYYY'), Details: [], OrderStatus: 0 };

    $scope.itemList = [];
    $scope.unitList = [];
    $scope.firmList = [];
    $scope.forexList = [];
    $scope.moldList = [];
    $scope.moldTestList = [];

    $scope.selectedFirm = {};

    $scope.saveStatus = 0;

    // WORK ORDER FUNCTIONS
    $scope.getNextWorkOrderNo = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'WorkOrder/GetNextOrderNo', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        if (resp.data.Result) {
                            resolve(resp.data.WorkOrderNo);
                        }
                        else {
                            toastr.error('Sıradaki iş emri numarası üretilemedi. Lütfen ekranı yenileyip tekrar deneyiniz.', 'Uyarı');
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
        $scope.modelObject = { Id: 0, WorkOrderDate: moment().format('DD.MM.YYYY'), Details: [], WorkOrderStatus: 0 };
        $scope.selectedFirm = {};

        $scope.getNextWorkOrderNo().then(function (rNo) {
            $scope.modelObject.WorkOrderNo = rNo;
            $scope.$apply();
        });
        $scope.bindDetails();
    }

    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu iş emrini silmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'WorkOrder/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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

        if (typeof $scope.selectedFirm != 'undefined' && $scope.selectedFirm != null)
            $scope.modelObject.FirmId = $scope.selectedFirm.Id;
        else
            $scope.modelObject.FirmId = null;

        $http.post(HOST_URL + 'WorkOrder/SaveModel', $scope.modelObject, 'json')
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

    $scope.dropDownBoxEditorTemplateMolds = function (cellElement, cellInfo) {
        return $("<div>").dxDropDownBox({
            dropDownOptions: { width: 500 },
            dataSource: $scope.moldList,
            value: cellInfo.value,
            valueExpr: "Id",
            displayExpr: "MoldCode",
            contentTemplate: function (e) {
                return $("<div>").dxDataGrid({
                    dataSource: $scope.moldList,
                    remoteOperations: true,
                    columns: [
                        { dataField: 'MoldCode', caption: 'Kalıp Kodu' },
                        { dataField: 'MoldName', caption: 'Kalıp Adı' },
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

    $scope.dropDownBoxEditorTemplateMoldTests = function (cellElement, cellInfo) {
        return $("<div>").dxDropDownBox({
            dropDownOptions: { width: 500 },
            dataSource: $scope.moldTestList,
            value: cellInfo.value,
            valueExpr: "Id",
            displayExpr: "ProductDescription",
            contentTemplate: function (e) {
                return $("<div>").dxDataGrid({
                    dataSource: $scope.moldTestList,
                    remoteOperations: true,
                    columns: [
                        { dataField: 'TestDateStr', caption: 'Tarih', dataType: 'date', format: 'dd.MM.yyyy' },
                        { dataField: 'ProductDescription', caption: 'Ürün Açıklama' },
                        { dataField: 'MoldName', caption: 'Kalıp' },
                        { dataField: 'RawMaterialName', caption: 'Hammadde' },
                        { dataField: 'RawMaterialGr', caption: 'Ham Gr' },
                        { dataField: 'InflationTimeSeconds', caption: 'Şişirme Zamanı' },
                        { dataField: 'TotalTimeSeconds', caption: 'Toplam Zamanı' },
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
        $http.get(HOST_URL + 'WorkOrder/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;
                    $scope.modelObject.WorkOrderDate = $scope.modelObject.WorkOrderDateStr;

                    if (typeof $scope.modelObject.FirmId != 'undefined' && $scope.modelObject.FirmId != null)
                        $scope.selectedFirm = $scope.firmList.find(d => d.Id == $scope.modelObject.FirmId);
                    else
                        $scope.selectedFirm = {};

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

                        //if (typeof values.UnitId != 'undefined') {
                        //    var unitObj = $scope.unitList.find(d => d.Id == values.UnitId);
                        //    obj.UnitId = unitObj.Id;
                        //    obj.UnitName = unitObj.UnitCode;
                        //}

                        if (typeof values.MoldId != 'undefined') {
                            var moldObj = $scope.moldList.find(d => d.Id == values.MoldId);
                            obj.MoldId = moldObj.Id;
                            obj.MoldCode = moldObj.MoldCode;
                            obj.MoldName = moldObj.MoldName;
                        }

                        if (typeof values.MoldTestId != 'undefined') {
                            var moldTestObj = $scope.moldTestList.find(d => d.Id == values.MoldTestId);
                            obj.MoldTestId = moldTestObj.Id;
                            obj.ProductDescription = moldTestObj.ProductDescription;
                            obj.InflationTimeSeconds = moldTestObj.InflationTimeSeconds;
                            obj.RawGr = moldTestObj.RawMaterialGr;
                            obj.RawGrToleration = moldTestObj.RawMaterialTolerationGr;
                        }

                        if (typeof values.Explanation != 'undefined') { obj.Explanation = values.Explanation; }
                        if (typeof values.Quantity != 'undefined') { obj.Quantity = values.Quantity; }
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
                    /*var unitObj = $scope.unitList.find(d => d.Id == values.UnitId);*/
                    var moldObj = null;
                    var moldTestObj = null;

                    if (typeof values.MoldId != 'undefined') {
                        moldObj = $scope.moldList.find(d => d.Id == values.MoldId);
                    }

                    if (typeof values.MoldTestId != 'undefined') {
                        moldTestObj = $scope.moldTestList.find(d => d.Id == values.MoldTestId);
                    }

                    var newObj = {
                        Id: newId,
                        ItemId: itemObj.Id,
                        ItemNo: itemObj.ItemNo,
                        ItemName: itemObj.ItemName,
                        //UnitId: typeof unitObj != 'undefined' && unitObj != null ? unitObj.Id : null,
                        //UnitName: typeof unitObj != 'undefined' && unitObj != null ? unitObj.UnitCode : null,
                        MoldId: moldObj != null ? moldObj.Id : null,
                        MoldCode: moldObj != null ? moldObj.MoldCode : '',
                        MoldName: moldObj != null ? moldObj.MoldName : '',
                        MoldTestId: moldTestObj != null ? moldTestObj.Id : null,
                        ProductDescription: moldTestObj != null ? moldTestObj.ProductDescription : '',
                        InflationTimeSeconds: moldTestObj != null ? moldTestObj.InflationTimeSeconds : null,
                        RawGr: moldTestObj != null ? moldTestObj.RawMaterialGr : null,
                        RawGrToleration: moldTestObj != null ? moldTestObj.RawMaterialTolerationGr : null,
                        Quantity: values.Quantity,
                        Explanation: values.Explanation,
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
                    dataField: 'ItemId', caption: 'Ürün Kodu',
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
                { dataField: 'ProductName', caption: 'Ürün Adı', allowEditing: false },
                //{
                //    dataField: 'UnitId', caption: 'Birim',
                //    allowSorting: false,
                //    lookup: {
                //        dataSource: $scope.unitList,
                //        valueExpr: "Id",
                //        displayExpr: "UnitCode"
                //    }
                //},
                { dataField: 'Quantity', caption: 'Miktar', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, validationRules: [{ type: "required" }] },
                {
                    dataField: 'MoldId', caption: 'Kalıp Kodu',
                    lookup: {
                        dataSource: $scope.moldList,
                        valueExpr: "Id",
                        displayExpr: "MoldCode"
                    },
                    allowSorting: false,
                    editCellTemplate: $scope.dropDownBoxEditorTemplateMolds,
                    cellTemplate: function (container, options) {
                        if (typeof options.row.data.MoldCode != 'undefined'
                            && options.row.data.MoldCode != null && options.row.data.MoldCode.length > 0)
                            container.text(options.row.data.MoldCode);
                        else
                            container.text(options.displayValue);
                    }
                },
                { dataField: 'MoldName', caption: 'Kalıp Adı', allowEditing: false },
                {
                    dataField: 'MoldTestId', caption: 'Kalıp Deneme',
                    lookup: {
                        dataSource: $scope.moldTestList,
                        valueExpr: "Id",
                        displayExpr: "ProductDescription"
                    },
                    allowSorting: false,
                    editCellTemplate: $scope.dropDownBoxEditorTemplateMoldTests,
                    cellTemplate: function (container, options) {
                        if (typeof options.row.data.ProductDescription != 'undefined'
                            && options.row.data.ProductDescription != null && options.row.data.ProductDescription.length > 0)
                            container.text(options.row.data.ProductDescription);
                        else
                            container.text(options.displayValue);
                    }
                },
                { dataField: 'InflationTimeSeconds', caption: 'Şişirme Zamanı (sn)', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'RawGr', caption: 'Hammadde Gr', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'RawGrToleration', caption: 'Hammadde Gr Töl.', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'Explanation', caption: 'Açıklama' }
            ]
        });
    }

    $scope.loadSelectables = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'WorkOrder/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.itemList = resp.data.Items;
                        $scope.unitList = resp.data.Units;
                        $scope.firmList = resp.data.Firms;
                        $scope.forexList = resp.data.Forexes;
                        $scope.moldList = resp.data.Molds;
                        $scope.moldTestList = resp.data.MoldTests;

                        resolve();
                    }
                }).catch(function (err) { });
        });

        return prms;
    }

    // INFORMATIONS & ATTACHMENTS
    $scope.showRecordInformation = function () {
        $scope.$broadcast('showRecordInformation', { Id: $scope.modelObject.Id, DataType: 'ItemOrder' });
    }

    //$scope.showAttachmentList = function () {
    //    $scope.$broadcast('showAttachmentList',
    //        { RecordId: $scope.modelObject.Id, RecordType: 1 });

    //    $('#dial-attachments').dialog({
    //        width: 500,
    //        height: 400,
    //        //height: window.innerHeight * 0.6,
    //        hide: true,
    //        modal: true,
    //        resizable: false,
    //        show: true,
    //        draggable: false,
    //        closeText: "KAPAT"
    //    });
    //}

    // ON LOAD EVENTS
    DevExpress.localization.locale('tr');
    $scope.loadSelectables().then(function () {
        if (PRM_ID > 0)
            $scope.bindModel(PRM_ID);
        else {
            $scope.getNextWorkOrderNo().then(function (rNo) {
                $scope.modelObject.WorkOrderNo = rNo;
                $scope.$apply();

                $scope.bindDetails();
            });
        }
    });
});