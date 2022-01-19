app.controller('lOrderCtrl', function ($scope, $http) {
    $scope.modelObject = { Id: 0, OrderDateStr: moment().format('DD.MM.YYYY'), Details: [], OrderStatus: 0 };

    $scope.itemList = [];
    $scope.unitList = [];
    $scope.customerFirmList = [];
    $scope.forexList = [];
    $scope.customsList = [];

    $scope.selectedCustomerFirm = {};
    $scope.selectedRow = { Id: 0 };

    $scope.selectedEntryCustoms = {};
    $scope.selectedExitCustoms = {};

    $scope.selectedOrderUploadType = {};
    $scope.orderUploadTypeList = [{ Id: 1, Text: 'Grupaj' }, { Id: 2, Text: 'Komple' }];

    $scope.selectedOrderTransactionDirectionType = {};
    $scope.orderTransactionDirectionTypeList = [{ Id: 1, Text: 'Yurt Dışı/İhracat' }, { Id: 2, Text: 'Yurt Dışı/İthalat' },
        { Id: 3, Text: 'Yurt İçi' }, { Id: 4, Text: 'Yurt içi Transfer' }, { Id: 5, Text: 'Yurt Dışı Transfer' }];

    $scope.selectedOrderUploadPointType = {};
    $scope.orderUploadPointTypeList = [{ Id: 1, Text: 'Müşteriden Yükleme' }, { Id: 2, Text: 'Depodan Yükleme' }];

    $scope.selectedOrderCalculationType = {Id:0};
    $scope.orderCalculationTypeList = [{ Id: 1, Text: 'Ağırlık' }, { Id: 2, Text: 'Metreküp' }, { Id: 3, Text: 'Ladametre' }];

    $scope.saveStatus = 0;

    // RECEIPT FUNCTIONS
    $scope.getNextOrderNo = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'LOrder/GetNextOrderNo', {}, 'json')
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
        $scope.selectedCustomerFirm = {};
        $scope.selectedOrderCalculationType = { Id: 0 };
        $scope.selectedOrderUploadPointType = {};
        $scope.selectedOrderTransactionDirectionType = {};
        $scope.selectedOrderUploadType = {};
        $scope.selectedEntryCustoms = {};
        $scope.selectedExitCustoms = {};

        $scope.getNextOrderNo().then(function (rNo) {
            $scope.modelObject.OrderNo = rNo;
            $scope.$apply();
        });
        $scope.bindDetails();
    }

    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu siparişi silmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'LOrder/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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

        if (typeof $scope.selectedCustomerFirm != 'undefined' && $scope.selectedCustomerFirm != null)
            $scope.modelObject.CustomerFirmId = $scope.selectedCustomerFirm.Id;
        else
            $scope.modelObject.CustomerFirmId = null;

        if (typeof $scope.selectedEntryCustoms != 'undefined' && $scope.selectedEntryCustoms != null)
            $scope.modelObject.EntryCustomsId = $scope.selectedEntryCustoms.Id;
        else
            $scope.modelObject.EntryCustomsId = null;

        if (typeof $scope.selectedExitCustoms != 'undefined' && $scope.selectedExitCustoms != null)
            $scope.modelObject.ExitCustomsId = $scope.selectedExitCustoms.Id;
        else
            $scope.modelObject.ExitCustomsId = null;

        if (typeof $scope.selectedOrderUploadType != 'undefined' && $scope.selectedOrderUploadType != null)
            $scope.modelObject.OrderUploadType = $scope.selectedOrderUploadType.Id;
        else
            $scope.modelObject.OrderUploadType = null;

        if (typeof $scope.selectedOrderTransactionDirectionType != 'undefined' && $scope.selectedOrderTransactionDirectionType != null)
            $scope.modelObject.OrderTransactionDirectionType = $scope.selectedOrderTransactionDirectionType.Id;
        else
            $scope.modelObject.OrderTransactionDirectionType = null;

        if (typeof $scope.selectedOrderCalculationType != 'undefined' && $scope.selectedOrderCalculationType != null)
            $scope.modelObject.OrderCalculationType = $scope.selectedOrderCalculationType.Id;
        else
            $scope.modelObject.OrderCalculationType = null;

        if (typeof $scope.selectedOrderUploadPointType != 'undefined' && $scope.selectedOrderUploadPointType != null)
            $scope.modelObject.OrderUploadPointType = $scope.selectedOrderUploadPointType.Id;
        else
            $scope.modelObject.OrderUploadPointType = null;

        $http.post(HOST_URL + 'LOrder/SaveModel', $scope.modelObject, 'json')
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
                        { dataField: 'ItemNo', caption: 'Ürün Kodu' },
                        { dataField: 'ItemName', caption: 'Ürün Adı' },
                        { dataField: 'ItemTypeStr', caption: 'Ürün Türü' },
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
        $http.get(HOST_URL + 'LOrder/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;
                    //$scope.modelObject.DateOfNeed = $scope.modelObject.DateOfNeedStr;
                    //$scope.modelObject.OrderDate = $scope.modelObject.OrderDateStr;

                    if (typeof $scope.modelObject.CustomerFirmId != 'undefined' && $scope.modelObject.CustomerFirmId != null)
                        $scope.selectedCustomerFirm = $scope.customerFirmList.find(d => d.Id == $scope.modelObject.CustomerFirmId);
                    else
                        $scope.selectedCustomerFirm = {};

                    if ($scope.modelObject.OrderTransactionDirectionType > 0)
                        $scope.selectedOrderTransactionDirectionType = $scope.orderTransactionDirectionTypeList.find(d => d.Id == $scope.modelObject.OrderTransactionDirectionType);
                    else
                        $scope.selectedOrderTransactionDirectionType = {};

                    if ($scope.modelObject.OrderCalculationType > 0)
                        $scope.selectedOrderCalculationType = $scope.orderCalculationTypeList.find(d => d.Id == $scope.modelObject.OrderCalculationType);
                    else
                        $scope.selectedOrderCalculationType = {};

                    if ($scope.modelObject.OrderUploadPointType > 0)
                        $scope.selectedOrderUploadPointType = $scope.orderUploadPointTypeList.find(d => d.Id == $scope.modelObject.OrderUploadPointType);
                    else
                        $scope.selectedOrderUploadPointType = {};

                    if ($scope.modelObject.OrderUploadType > 0)
                        $scope.selectedOrderUploadType = $scope.orderUploadTypeList.find(d => d.Id == $scope.modelObject.OrderUploadType);
                    else
                        $scope.selectedOrderUploadType = {};

                    if ($scope.modelObject.EntryCustomsId > 0)
                        $scope.selectedEntryCustoms = $scope.customsList.find(d => d.Id == $scope.modelObject.EntryCustomsId);
                    else
                        $scope.selectedEntryCustoms = {};

                    if ($scope.modelObject.ExitCustomsId > 0)
                        $scope.selectedExitCustoms = $scope.customsList.find(d => d.Id == $scope.modelObject.ExitCustomsId);
                    else
                        $scope.selectedExitCustoms = {};

                    $scope.bindDetails();
                    $scope.calculateHeader();
                }
            }).catch(function (err) { });
    }

    $scope.calculateRow = function (row) {
        $scope.calculateValumeAndDesi(row);
        $scope.calculateOveralTotal();

        if (typeof row != 'undefined' && row != null) {
            try {
                $http.post(HOST_URL + 'LOrder/CalculateRow', row, 'json')
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
    $scope.calculateValumeAndDesi = function (row) {
        if (row.ShortWidth == 'undefined' || row.ShortWidth == null)
            row.ShortWidth = 0;
        if (row.LongWidth == 'undefined' || row.LongWidth == null)
            row.LongWidth = 0;
        if (row.Height == 'undefined' || row.Height == null)
            row.Height = 0; 
        row.Volume = row.Quantity * row.ShortWidth * row.LongWidth * row.Height /1000000;
        //CALCULATE VALUME AND WEIGHT AND LADAMETRE
        let sumVolume = 0;
        let sumWeight = 0;
        let = sumLadametre = 0 ;
        $scope.modelObject.Details.forEach(element => {
            sumVolume += parseFloat(element.Volume != null ? element.Volume : 0);
            sumWeight += parseFloat(element.Weight != null ? element.Weight : 0);
            sumLadametre += parseFloat(element.Ladametre != null ? element.Ladametre : 0);
        });
        $scope.modelObject.OveralVolume =  sumVolume;
        $scope.modelObject.OveralWeight = sumWeight;
        $scope.modelObject.OveralLadametre =  sumLadametre;

    }

    $scope.calculateHeader = function () {
        //$scope.modelObject.SubTotal = $scope.modelObject.Details.map(d => d.OverallTotal - d.TaxAmount).reduce((n, x) => n + x);
        //$scope.modelObject.TaxPrice = $scope.modelObject.Details.map(d => d.TaxAmount).reduce((n, x) => n + x);
       // $scope.modelObject.OverallTotal = $scope.modelObject.Details.map(d => d.OverallTotal).reduce((n, x) => n + x);
    }
    $scope.calculateOveralTotal = function () {
        if ($scope.selectedOrderCalculationType.Id == 1)
            $scope.modelObject.OverallTotal = $scope.modelObject.OveralWeight * $scope.modelObject.CalculationTypePrice;
        else if ($scope.selectedOrderCalculationType.Id == 2)
            $scope.modelObject.OverallTotal = $scope.modelObject.OveralVolume * $scope.modelObject.CalculationTypePrice;
        else if ($scope.selectedOrderCalculationType.Id == 3)
            $scope.modelObject.OverallTotal = $scope.modelObject.OveralLadametre * $scope.modelObject.CalculationTypePrice;
        else if ($scope.selectedOrderCalculationType.Id == 0)
            toastr.error("Hesaplama Tip seçimi yapılmadı !", 'Hata');

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
                            obj.UnitName = itemObj.UnitCode;
                            calculateRowAgain = true;
                        }

                        if (typeof values.Explanation != 'undefined') { obj.Explanation = values.Explanation; }
                        if (typeof values.Quantity != 'undefined') { obj.Quantity = values.Quantity; calculateRowAgain = true; }
                        if (typeof values.ShortWidth != 'undefined') { obj.ShortWidth = values.ShortWidth; calculateRowAgain = true; }
                        if (typeof values.LongWidth != 'undefined') { obj.LongWidth = values.LongWidth; calculateRowAgain = true; }
                        if (typeof values.Height != 'undefined') { obj.Height = values.Height; calculateRowAgain = true; }
                        if (typeof values.Volume != 'undefined') { obj.Volume = values.Volume; calculateRowAgain = true; }
                        if (typeof values.Weight != 'undefined') { obj.Weight = values.Weight; calculateRowAgain = true; }
                        //if (typeof values.Desi != 'undefined') { obj.Desi = values.Desi; calculateRowAgain = true; }
                        if (typeof values.Ladametre != 'undefined') { obj.Ladametre = values.Ladametre; calculateRowAgain = true; }
                        if (typeof values.Stackable != 'undefined') { obj.Stackable = values.Stackable; calculateRowAgain = true; }
                        if (typeof values.PackageInNumber != 'undefined') { obj.PackageInNumber = values.PackageInNumber; calculateRowAgain = true; }
                       //if (typeof values.TaxRate != 'undefined') { obj.TaxRate = values.TaxRate; calculateRowAgain = true; }
                        //if (typeof values.TaxIncluded != 'undefined') { obj.TaxIncluded = values.TaxIncluded; calculateRowAgain = true; }
                        //if (typeof values.UnitPrice != 'undefined') { obj.UnitPrice = values.UnitPrice; calculateRowAgain = true; }
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
                            alert($scope.modelObject.OrderDateStr);
                            $http.get(HOST_URL + 'Common/GetForexRate?forexCode=' + forexObj.ForexTypeCode
                                + '&forexDate=' + $scope.modelObject.OrderDateStr, {}, 'json')
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
                        ShortWidth: values.ShortWidth,
                        LongWidth: values.LongWidth,
                        Height: values.Height,
                        Weight: values.Weight,
                        Volume: values.Volume,
                        Stackable: values.Stackable,
                        PackageInNumber: values.PackageInNumber,
                        //Desi: values.Desi,
                        Ladametre: values.Ladametre,
                        //TaxIncluded: values.TaxIncluded,
                        //UnitPrice: values.UnitPrice,
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
            height: 280,
            editing: {
                allowUpdating: true,
                allowDeleting: true,
                allowAdding: true,
                mode: 'cell'
            },
            allowColumnResizing: true,
            wordWrapEnabled: true,
            onInitNewRow: function (e) {
                e.data.UnitPrice = 0;
                e.data.TaxIncluded = 0;
            },
            repaintChangesOnly: true,
            onCellPrepared: function (e) {
                if (e.rowType === "data") {
                    if (e.data.OrderStatus == 3) {
                        e.cellElement.css("background-color", "#e64040");
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
                    width : 70,
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
                { dataField: 'ItemName', caption: 'Ürün Adı', allowEditing: false },
                {
                    dataField: 'UnitId', caption: 'Birim',
                    allowSorting: false,
                    lookup: {
                        dataSource: $scope.unitList,
                        valueExpr: "Id",
                        displayExpr: "UnitCode"
                    }, width : 60
                }, 
                { dataField: 'Quantity', caption: 'Miktar', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, width: 60, validationRules: [{ type: "required" }] },
                { dataField: 'ShortWidth', caption: 'Kısa En (Cm)', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'LongWidth', caption: 'Uzun En (Cm)', dataType: 'number', format: { type: "fixedPoint", precision: 2 }},
                { dataField: 'Height', caption: 'Yükseklik (Cm)', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'Weight', caption: 'Ağırlık (Kg)', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'Volume', caption: 'Hacim (m3)', dataType: 'number', format: { type: "fixedPoint", precision: 2 },allowEditing: false},
                //{ dataField: 'Desi', caption: 'Desi', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, width: 60, allowEditing: false },
                { dataField: 'Ladametre', caption: 'Ladametre', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, width: 80 },
                { dataField: 'Stackable', caption: 'İstiflenebilir', dataType: 'boolean' },
                { dataField: 'PackageInNumber', caption: 'Koli iç Adet'},

                //{ dataField: 'TaxRate', caption: 'Kdv %', dataType: 'number', format: { type: "fixedPoint", width: 60, precision: 2 } },
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
                //{ dataField: 'UnitPrice', caption: 'B. Fiyat', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, validationRules: [{ type: "required" }] },
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
                //{ dataField: 'TaxAmount', allowEditing: false, caption: 'Kdv Tutarı', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'OverallTotal', allowEditing: false, caption: 'Satır Tutarı', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
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
    // APPROVALS
    $scope.approveLOrder = function () {
        bootbox.confirm({
            message: "Bu Sipariş talebi onaylamak istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'LOrder/ApproveOrderPrice', { rid: $scope.modelObject.Id }, 'json')
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
    
    $scope.cancelledLOrder = function () {
        bootbox.confirm({
            message: "Bu Sipariş talebi İptal etmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'LOrder/CancelledOrderPrice', { rid: $scope.modelObject.Id }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Result) {
                                    toastr.success('İptal işlemi başarılı.', 'Bilgilendirme');

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

    $scope.loadSelectables = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'LOrder/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.itemList = resp.data.Items;
                        $scope.unitList = resp.data.Units;
                        $scope.customerFirmList = resp.data.Firms;
                        $scope.forexList = resp.data.Forexes;
                        $scope.customsList = resp.data.Customers;
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

    // ROW MENU ACTIONS
    $scope.showRowMenu = function () {
        if ($scope.selectedRow && $scope.selectedRow.Id > 0) {
            $scope.$apply();

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
    $scope.toggleOrderDetailStatus = function () {
        if ($scope.selectedRow && $scope.selectedRow.Id > 0) {
            $('#dial-row-menu').dialog("close");

            bootbox.confirm({
                message: "Bu sipariş kaleminin durumunu değiştirmek istediğinizden emin misiniz?",
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
                        $http.post(HOST_URL + 'LOrder/ToggleOrderDetailStatus', { detailId: $scope.selectedRow.Id }, 'json')
                            .then(function (resp) {
                                if (typeof resp.data != 'undefined' && resp.data != null) {
                                    $scope.saveStatus = 0;

                                    if (resp.data.Status == 1) {
                                        toastr.success('İşlem başarılı.', 'Bilgilendirme');

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
                    $http.post(HOST_URL + 'LOrder/ApproveOrderPrice', { rid: $scope.modelObject.Id }, 'json')
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