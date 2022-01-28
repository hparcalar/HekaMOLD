﻿app.controller('loadCtrl', function ($scope, $http) {
    $scope.modelObject = { Id: 0, Details: [], LoadStatus: 0 };

    $scope.itemList = [];
    $scope.unitList = [];
    $scope.firmList = [];
    $scope.forexList = [];
    $scope.customsList = [];
    $scope.usersList = [];
    $scope.cityList = [];
    $scope.countryList = [];

    $scope.selectedUser = {};
    $scope.selectedCustomerFirm = {};
    $scope.selectedRow = { Id: 0 };

    $scope.selectedEntryCustoms = {};
    $scope.selectedExitCustoms = {};
    $scope.selectedBuyerCity = {};
    $scope.selectedBuyerCountry = {};
    $scope.selectedShipperCity = {};
    $scope.selectedShipperCountry = {};

    $scope.selectedOrderUploadType = {};
    $scope.orderUploadTypeList = [{ Id: 1, Text: 'Grupaj' }, { Id: 2, Text: 'Komple' }];

    $scope.selectedOrderTransactionDirectionType = {};
    $scope.orderTransactionDirectionTypeList = [{ Id: 1, Text: 'Yurt Dışı/İhracat' }, { Id: 2, Text: 'Yurt Dışı/İthalat' },
    { Id: 3, Text: 'Yurt İçi' }, { Id: 4, Text: 'Yurt içi Transfer' }, { Id: 5, Text: 'Yurt Dışı Transfer' }];

    $scope.selectedOrderUploadPointType = {};
    $scope.orderUploadPointTypeList = [{ Id: 1, Text: 'Müşteriden Yükleme' }, { Id: 2, Text: 'Depodan Yükleme' }];

    $scope.selectedOrderCalculationType = { Id: 0 };
    $scope.orderCalculationTypeList = [{ Id: 1, Text: 'Ağırlık' }, { Id: 2, Text: 'Metreküp' }, { Id: 3, Text: 'Ladametre' }];

    $scope.saveStatus = 0;

     // FUNCTIONS
    $scope.getNextOrderNo = function () {

        let directionId = 0;

        if (typeof $scope.selectedOrderTransactionDirectionType.Id == 'undefined')
            directionId = 0;
        else
            directionId = $scope.selectedOrderTransactionDirectionType.Id;

        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Load/GetNextLoadCode?directionId=' + directionId, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        if (resp.data.Result) {
                            resolve(resp.data.LoadCode);
                            $scope.modelObject.LoadCode = resp.data.LoadCode;
                        }
                        else {
                            toastr.error('Sıradaki yük numarası üretilemedi. Lütfen ekranı yenileyip tekrar deneyiniz.', 'Uyarı');
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
        $scope.modelObject = { Id: 0, Details: [], LoadStatus: 0 };
        $scope.selectedFirm = {};
        $scope.selectedUser = {};
        $scope.selectedOrderUploadType = {};
        $scope.selectedOrderTransactionDirectionType = {};
        $scope.selectedEntryCustoms = {};
        $scope.selectedExitCustoms = {};
        $scope.selectedOrderCalculationType = { Id: 0 };

        $scope.getNextOrderNo().then(function (rNo) {
            $scope.modelObject.OrderNo = rNo;
            $scope.$apply();
        });
        $scope.bindDetails();
    }

    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu yükü silmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'Load/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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

        if (typeof $scope.selectedShipperFirm != 'undefined' && $scope.selectedShipperFirm != null)
            $scope.modelObject.ShipperFirmId = $scope.selectedShipperFirm.Id;
        else
            $scope.modelObject.ShipperFirmId = null;

        if (typeof $scope.selectedBuyerFirm != 'undefined' && $scope.selectedBuyerFirm != null)
            $scope.modelObject.BuyerFirmId = $scope.selectedBuyerFirm.Id;
        else
            $scope.modelObject.BuyerFirmId = null;

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

        if (typeof $scope.selectedUser != 'undefined' && $scope.selectedUser != null)
            $scope.modelObject.UserAuthorId = $scope.selectedUser.Id;
        else
            $scope.modelObject.UserAuthorId = null;

        if (typeof $scope.selectedShipperCountry != 'undefined' && $scope.selectedShipperCountry != null)
            $scope.modelObject.ShipperCountryId = $scope.selectedShipperCountry.Id;
        else
            $scope.modelObject.ShipperCountryId = null;

        if (typeof $scope.selectedBuyerCountry != 'undefined' && $scope.selectedBuyerCountry != null)
            $scope.modelObject.BuyerCountryId = $scope.selectedBuyerCountry.Id;
        else
            $scope.modelObject.BuyerCountryId = null;

        if (typeof $scope.selectedShipperCity != 'undefined' && $scope.selectedShipperCity != null)
            $scope.modelObject.ShipperCityId = $scope.selectedShipperCity.Id;
        else
            $scope.modelObject.ShipperCityId = null;

        if (typeof $scope.selectedBuyerCity != 'undefined' && $scope.selectedBuyerCity != null)
            $scope.modelObject.BuyerCityId = $scope.selectedBuyerCity.Id;
        else
            $scope.modelObject.BuyerCityId = null;



        $http.post(HOST_URL + 'Load/SaveModel', $scope.modelObject, 'json')
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
                        { dataField: 'ItemNo', caption: 'Mal Kodu' },
                        { dataField: 'ItemName', caption: 'Mal Adı' },
                        { dataField: 'ItemTypeStr', caption: 'Mal Türü' },
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
        $http.get(HOST_URL + 'Load/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data; 
                    //$scope.modelObject.DateOfNeed = $scope.modelObject.DateOfNeedStr;
                    //$scope.modelObject.OrderDate = $scope.modelObject.OrderDateStr;
                    if (typeof $scope.modelObject.ShipperFirmId != 'undefined' && $scope.modelObject.ShipperFirmId != null)
                        $scope.selectedShipperFirm = $scope.firmList.find(d => d.Id == $scope.modelObject.ShipperFirmId);
                    else
                        $scope.selectedShipperFirm = {};

                    if (typeof $scope.modelObject.BuyerFirmId != 'undefined' && $scope.modelObject.BuyerFirmId != null)
                        $scope.selectedBuyerFirm = $scope.firmList.find(d => d.Id == $scope.modelObject.BuyerFirmId);
                    else
                        $scope.selectedBuyerFirm = {};

                    if (typeof $scope.modelObject.CustomerFirmId != 'undefined' && $scope.modelObject.CustomerFirmId != null)
                        $scope.selectedCustomerFirm = $scope.firmList.find(d => d.Id == $scope.modelObject.CustomerFirmId);
                    else
                        $scope.selectedCustomerFirm = {};

                    if (typeof $scope.modelObject.EntryCustomsId != 'undefined' && $scope.modelObject.EntryCustomsId != null)
                        $scope.selectedEntryCustoms = $scope.customsList.find(d => d.Id == $scope.modelObject.EntryCustomsId);
                    else
                        $scope.selectedEntryCustoms = {};

                    if (typeof $scope.modelObject.ExitCustomsId != 'undefined' && $scope.modelObject.ExitCustomsId != null)
                        $scope.selectedExitCustoms = $scope.customsList.find(d => d.Id == $scope.modelObject.ExitCustomsId);
                    else
                        $scope.selectedExitCustoms = {};

                    if (typeof $scope.modelObject.OrderUploadType != 'undefined' && $scope.modelObject.OrderUploadType != null)
                        $scope.selectedOrderUploadType = $scope.orderUploadTypeList.find(d => d.Id == $scope.modelObject.OrderUploadType);
                    else
                        $scope.selectedOrderUploadType = {};

                    if (typeof $scope.modelObject.OrderUploadPointType != 'undefined' && $scope.modelObject.OrderUploadPointType != null)
                        $scope.selectedOrderUploadPointType = $scope.orderUploadPointTypeList.find(d => d.Id == $scope.modelObject.OrderUploadPointType);
                    else
                        $scope.selectedOrderUploadPointType = {};

                    if (typeof $scope.modelObject.OrderTransactionDirectionType != 'undefined' && $scope.modelObject.OrderTransactionDirectionType != null)
                        $scope.selectedOrderTransactionDirectionType = $scope.orderTransactionDirectionTypeList.find(d => d.Id == $scope.modelObject.OrderTransactionDirectionType);
                    else
                        $scope.selectedOrderTransactionDirectionType = {};

                    if (typeof $scope.modelObject.OrderCalculationType != 'undefined' && $scope.modelObject.OrderCalculationType != null)
                        $scope.selectedOrderCalculationType = $scope.orderCalculationTypeList.find(d => d.Id == $scope.modelObject.OrderCalculationType);
                    else
                        $scope.selectedOrderCalculationType = {};

                    if (typeof $scope.modelObject.UserAuthorId != 'undefined' && $scope.modelObject.UserAuthorId != null)
                        $scope.selectedUser = $scope.usersList.find(d => d.Id == $scope.modelObject.UserAuthorId);
                    else
                        $scope.selectedUser = {};

                    if (typeof $scope.modelObject.ShipperCountryId != 'undefined' && $scope.modelObject.ShipperCountryId != null)
                        $scope.selectedShipperCountry = $scope.countryList.find(d => d.Id == $scope.modelObject.ShipperCountryId);
                    else
                        $scope.selectedShipperCountry = {};

                    if (typeof $scope.modelObject.BuyerCountryId != 'undefined' && $scope.modelObject.BuyerCountryId != null)
                        $scope.selectedBuyerCountry = $scope.countryList.find(d => d.Id == $scope.modelObject.BuyerCountryId);
                    else
                        $scope.selectedBuyerCountry = {};

                    if (typeof $scope.modelObject.ShipperCityId != 'undefined' && $scope.modelObject.ShipperCityId != null)
                        $scope.selectedShipperCity = $scope.cityList.find(d => d.Id == $scope.modelObject.ShipperCityId);
                    else
                        $scope.selectedShipperCity = {};

                    if (typeof $scope.modelObject.BuyerCityId != 'undefined' && $scope.modelObject.BuyerCityId != null)
                        $scope.selectedBuyerCity = $scope.cityList.find(d => d.Id == $scope.modelObject.BuyerCityId);
                    else
                        $scope.selectedBuyerCity = {};

                    $scope.bindDetails();
                }
            }).catch(function (err) { });
    }

    //$scope.calculateRow = function (row) {
    //    if (typeof row != 'undefined' && row != null) {
    //        try {
    //            $http.post(HOST_URL + 'PIOrder/CalculateRow', row, 'json')
    //                .then(function (resp) {
    //                    if (typeof resp.data != 'undefined' && resp.data != null) {
    //                        row.OverallTotal = resp.data.OverallTotal;
    //                        row.UnitPrice = resp.data.UnitPrice;
    //                        row.TaxAmount = resp.data.TaxAmount;
    //                        row.ForexUnitPrice = resp.data.ForexUnitPrice;

    //                        $scope.calculateHeader();
    //                    }
    //                }).catch(function (err) { });
    //        } catch (e) {

    //        }
    //    }
    //}

    //$scope.calculateHeader = function () {
    //    $scope.modelObject.SubTotal = $scope.modelObject.Details.map(d => d.OverallTotal - d.TaxAmount).reduce((n, x) => n + x);
    //    $scope.modelObject.TaxPrice = $scope.modelObject.Details.map(d => d.TaxAmount).reduce((n, x) => n + x);
    //    $scope.modelObject.OverallTotal = $scope.modelObject.Details.map(d => d.OverallTotal).reduce((n, x) => n + x);
    //}

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
                        if (typeof values.ShortWidth != 'undefined') { obj.ShortWidth = values.ShortWidth; calculateRowAgain = true; }
                        if (typeof values.LongWidth != 'undefined') { obj.LongWidth = values.LongWidth; calculateRowAgain = true; }
                        if (typeof values.Volume != 'undefined') { obj.Volume = values.Volume; calculateRowAgain = true; }
                        if (typeof values.Height != 'undefined') { obj.Height = values.Height; calculateRowAgain = true; }
                        if (typeof values.Weight != 'undefined') { obj.Weight = values.Weight; calculateRowAgain = true; }
                        if (typeof values.PackageInNumber != 'undefined') { obj.PackageInNumber = values.PackageInNumber; calculateRowAgain = true; }
                        if (typeof values.Stackable != 'undefined') { obj.Stackable = values.Stackable; calculateRowAgain = true; }
                        if (typeof values.Ladametre != 'undefined') { obj.Ladametre = values.Ladametre; calculateRowAgain = true; }
                        if (typeof values.Explanation != 'undefined') { obj.Explanation = values.Explanation; }
                        if (typeof values.Quantity != 'undefined') { obj.Quantity = values.Quantity; calculateRowAgain = true; }
                        //if (typeof values.TaxRate != 'undefined') { obj.TaxRate = values.TaxRate; calculateRowAgain = true; }
                        //if (typeof values.TaxIncluded != 'undefined') { obj.TaxIncluded = values.TaxIncluded; calculateRowAgain = true; }
                        if (typeof values.UnitPrice != 'undefined') { obj.UnitPrice = values.UnitPrice; calculateRowAgain = true; }
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

                    var itemObj = $scope.itemList.find(d => d.Id == values.ItemId);
                    var unitObj = $scope.unitList.find(d => d.Id == values.UnitId);

                    var newObj = {
                        Id: newId,
                        ItemId: itemObj.Id,
                        ItemNo: itemObj.ItemNo,
                        ItemName: itemObj.ItemName,
                        UnitId: typeof unitObj != 'undefined' && unitObj != null ? unitObj.Id : null,
                        UnitName: typeof unitObj != 'undefined' && unitObj != null ? unitObj.UnitCode : null,
                        //Height: values.Height,
                        //Weight: values.Weight,
                        //ShortWidth = values.ShortWidth,
                        //LongWidth: values.LongWidth,
                        //Ladametre: values.Ladametre,
                        //Volume: values.Volume,
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
                    dataField: 'ItemId', caption: 'Mal Kodu',
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
                { dataField: 'ItemName', caption: 'Mal Adı', allowEditing: false },
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
                { dataField: 'ShortWidth', caption: 'Kısa En (Cm)', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'LongWidth', caption: 'Uzun En (Cm)', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'Height', caption: 'Yükseklik (Cm)', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'Weight', caption: 'Ağırlık (Kg)', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'Volume', caption: 'Hacim (m3)', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, allowEditing: false },
                //{ dataField: 'Desi', caption: 'Desi', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, width: 60, allowEditing: false },
                { dataField: 'Ladametre', caption: 'Ladametre', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, width: 80 },
                { dataField: 'Stackable', caption: 'İstiflenebilir', dataType: 'boolean' },
                { dataField: 'PackageInNumber', caption: 'Koli iç Adet' },
                { dataField: 'Explanation', caption: 'Açıklama' }
            ]
        });
    }

    $scope.loadSelectables = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Load/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.itemList = resp.data.Items;
                        $scope.unitList = resp.data.Units;
                        $scope.firmList = resp.data.Firms;
                        $scope.forexList = resp.data.Forexes;
                        $scope.customsList = resp.data.Customs;
                        $scope.usersList = resp.data.Users;
                        $scope.cityList = resp.data.Citys;
                        $scope.countryList = resp.data.Countrys;
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
                $scope.modelObject.LoadCode = rNo;
                $scope.$apply();

                $scope.bindDetails();
            });
        }
    });
});