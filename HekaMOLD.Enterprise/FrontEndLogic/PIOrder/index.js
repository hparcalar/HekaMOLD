app.controller('orderCtrl', function ($scope, $http) {
    $scope.modelObject = { Id:0, OrderDate: moment().format('DD.MM.YYYY'), Details: [], OrderStatus:0 };

    $scope.itemList = [];
    $scope.unitList = [];
    $scope.firmList = [];
    $scope.forexList = [];

    $scope.selectedFirm = {};

    $scope.saveStatus = 0;

    // RECEIPT FUNCTIONS
    $scope.getNextOrderNo = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'PIOrder/GetNextOrderNo', {}, 'json')
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
        $scope.modelObject = { Id: 0, OrderDate: moment().format('DD.MM.YYYY'), Details: [], OrderStatus: 0 };
        $scope.selectedFirm = {};

        $scope.getNextOrderNo().then(function (rNo) {
            $scope.modelObject.OrderNo = rNo;
            $scope.$apply();
        });
        $scope.bindDetails();
    }

    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu siparişi silmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'PIOrder/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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

        $http.post(HOST_URL + 'PIOrder/SaveModel', $scope.modelObject, 'json')
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
            dropDownOptions: { width: 800 },
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
                    allowColumnResizing: true,
                    wordWrapEnabled: true,
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
        $http.get(HOST_URL + 'PIOrder/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;
                    $scope.modelObject.DateOfNeed = $scope.modelObject.DateOfNeedStr;
                    $scope.modelObject.OrderDate = $scope.modelObject.OrderDateStr;

                    if (typeof $scope.modelObject.FirmId != 'undefined' && $scope.modelObject.FirmId != null)
                        $scope.selectedFirm = $scope.firmList.find(d => d.Id == $scope.modelObject.FirmId);
                    else
                        $scope.selectedFirm = {};

                    $scope.bindDetails();
                }
            }).catch(function (err) { });
    }

    $scope.calculateRow = function (row) {
        if (typeof row != 'undefined' && row != null) {
            try {
                $http.post(HOST_URL + 'PIOrder/CalculateRow', row, 'json')
                    .then(function (resp) {
                        if (typeof resp.data != 'undefined' && resp.data != null) {
                            row.OverallTotal = resp.data.OverallTotal;
                            row.UnitPrice = resp.data.UnitPrice;
                            row.TaxAmount = resp.data.TaxAmount;
                            row.ForexUnitPrice = resp.data.ForexUnitPrice;

                            $scope.calculateHeader();
                        }
                    }).catch(function (err) { });
            } catch (e) {

            }
        }
    }

    $scope.calculateHeader = function () {
        $scope.modelObject.SubTotal = $scope.modelObject.Details.map(d => d.OverallTotal - d.TaxAmount).reduce((n, x) => n + x);
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
                        if (typeof values.TaxRate != 'undefined') { obj.TaxRate = values.TaxRate; calculateRowAgain = true; }
                        if (typeof values.TaxIncluded != 'undefined') { obj.TaxIncluded = values.TaxIncluded; calculateRowAgain = true; }
                        if (typeof values.UnitPrice != 'undefined') { obj.UnitPrice = values.UnitPrice; calculateRowAgain = true; }
                        if (typeof values.ForexRate != 'undefined') { obj.ForexRate = values.ForexRate; calculateRowAgain = true; }
                        if (typeof values.ForexUnitPrice != 'undefined') {
                            obj.ForexUnitPrice = values.ForexUnitPrice;
                            if (typeof obj.ForexId != 'undefined' && obj.ForexId != null) {
                                obj.UnitPrice = obj.ForexUnitPrice * 10;
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

    $scope.loadSelectables = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'PIOrder/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.itemList = resp.data.Items;
                        $scope.unitList = resp.data.Units;
                        $scope.firmList = resp.data.Firms;
                        $scope.forexList = resp.data.Forexes;

                        resolve();
                    }
                }).catch(function (err) { });
        });

        return prms;
    }

    // INFORMATIONS & ATTACHMENTS
    $scope.showRecordInformation = function () {
        $scope.$broadcast('showRecordInformation', { Id: $scope.modelObject.Id, DataType:'ItemOrder' });
    }

    $scope.showRequestInformation = function () {
        $scope.$broadcast('loadRelatedRequestList', $scope.modelObject.Id);

        $('#dial-related-requests').dialog({
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

    $scope.showAttachmentList = function () {
        $scope.$broadcast('showAttachmentList',
            { RecordId: $scope.modelObject.Id, RecordType: 1 });

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

    // APPROVALS
    $scope.approveOrderPrice = function () {
        bootbox.confirm({
            message: "Bu siparişin fiyatını onaylamak istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'PIOrder/ApproveOrderPrice', { rid: $scope.modelObject.Id }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Result) {
                                    toastr.success('Onay işlemi başarılı.', 'Bilgilendirme');

                                    $scope.bindModel($scope.modelObject.Id);
                                }
                                else
                                    toastr.error(resp.data.ErrorMessage, 'Hata');
                            }
                        }).catch(function (err) { });
                }
            }
        });
    }

    $scope.showItemRequestList = function () {
        // DO BROADCAST
        $scope.$broadcast('loadApprovedRequestDetails');

        $('#dial-requests').dialog({
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

    $scope.$on('transferRequestDetails', function (e, d) {
        d.forEach(x => {
            if ($scope.modelObject.Details.filter(m => m.ItemRequestDetailId == x.Id).length > 0) {
                toastr.warning(x.RequestNo + ' nolu talep, ' + x.ItemNo + ' / ' + x.ItemName + ', ' + x.Quantity
                    + ' miktarlı talep detayı zaten aktarıldığı için tekrar dahil edilmedi.', 'Uyarı');
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
                    UnitPrice: 0,
                    NewDetail: true,
                    ItemRequestDetailId: x.Id
                });

                var detailsGrid = $("#dataList").dxDataGrid("instance");
                detailsGrid.refresh();
            }
        });

        $('#dial-requests').dialog('close');
    });

    // ON LOAD EVENTS
    DevExpress.localization.locale('tr');
    $scope.loadSelectables().then(function () {
        if (PRM_ID > 0)
            $scope.bindModel(PRM_ID);
        else {
            $scope.getNextOrderNo().then(function (rNo) {
                $scope.modelObject.OrderNo = rNo;
                $scope.$apply();

                $scope.bindDetails();
            });
        }
    });
});