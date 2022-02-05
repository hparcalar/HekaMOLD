﻿app.controller('offerCtrl', ['$scope', '$http', 'Upload',
function ($scope, $http, Upload) {
    $scope.modelObject = { Id: 0, OfferDateStr: moment().format('DD.MM.YYYY'), Details: [] };
    $scope.offerModel = {};

    $scope.itemList = [];
    $scope.unitList = [];
    $scope.firmList = [];
    $scope.forexList = [];
    $scope.routeList = [];

    $scope.rawSheetPrice = 0;
    $scope.wastagePrice = 0;
    $scope.manPrice = 0;
    $scope.profitRate = 0;
    $scope.expirationVal = '';

    $scope.priceToleranceMin = 0;
    $scope.priceToleranceMax = 0;

    $scope.selectedDocx = null;
    $scope.selectedFirm = { Id:0, FirmCode: '', FirmName : '' };
    $scope.selectedRow = { Id: 0 };
    $scope.viewingRow = null;

    $scope.saveStatus = 0;

    // RECEIPT FUNCTIONS
    $scope.getNextOrderNo = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'SIOffer/GetNextOfferNo', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        if (resp.data.Result) {
                            resolve(resp.data.ReceiptNo);
                        }
                        else {
                            toastr.error('Sıradaki teklif numarası üretilemedi. Lütfen ekranı yenileyip tekrar deneyiniz.', 'Uyarı');
                            resolve('');
                        }
                    }
                }).catch(function (err) { });
        });

        return prms;
    }

    $scope.getToFixed = function(data, points) {
        try {
            if (isNaN(data))
                data = 0;

            var formatter = new Intl.NumberFormat('tr', {
                style: 'decimal',
            });

            return formatter.format(data);

            //if (typeof data != 'undefined')
            //    return data.toFixed(points);
        } catch (e) {
            
        }

        return '';
    }

    $scope.redirectToOrder = function () {
        window.location.href = HOST_URL + 'SIOrder?rid=' + $scope.modelObject.ItemOrderId;
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

    $scope.showOfferDialog = function () {
        if ($scope.modelObject.Id <= 0) {
            $scope.offerModel.firmaismi = $scope.selectedFirm.FirmName;
            $scope.offerModel.sackg = $scope.rawSheetPrice;
            $scope.offerModel.hurdakg = $scope.wastagePrice;
            $scope.offerModel.isci = $scope.manPrice;
            $scope.offerModel.marj = $scope.profitRate;
            $scope.offerModel.vadeyuzde = $scope.expirationVal;
        }

        $("#dial-offer").modal("show");
    }

    $scope.closeOfferDialog = function () {
        $("#dial-offer").modal("hide");
    }

    $scope.uploadSent = false;
    // CRUD
    $scope.uploadDocxFile = function () {
        if ($scope.modelObject.Id > 0) {
            $scope.modelObject.SheetWeight = $scope.offerModel.sackg;
            $scope.modelObject.WastageWeight = $scope.offerModel.hurdakg;
            $scope.modelObject.LaborCost = $scope.offerModel.isci;
            $scope.modelObject.ProfitRate = $scope.offerModel.marj;
            $scope.modelObject.CreditMonths = $scope.offerModel.vadeay;
            $scope.modelObject.CreditRate = $scope.offerModel.vadeyuzde;

            if ($scope.modelObject.CreditMonths == 0)
                $scope.modelObject.Expiration = 'PEŞİN';
            else
                $scope.modelObject.Expiration = $scope.modelObject.CreditMonths + ' AY';

            $scope.saveModel();
            $scope.closeOfferDialog();
            return;
        }

        if ($scope.uploadSent)
            return;

        $scope.uploadSent = true;
        let form_submit = $("#offer-parameters").serialize();

        if ($scope.selectedDocx) {
            Upload.upload({
                url: METALIX_URL + '?' + form_submit,
                data: {
                    docx: $scope.selectedDocx,
                }
            }).then(function (resp) {
                $scope.uploadSent = false;
                if (resp.data.status == true) {
                    var headerData = resp.data.mydata[0];
                    $scope.modelObject.Expiration = headerData.vade;

                    $scope.modelObject.SheetWeight = $scope.offerModel.sackg;
                    $scope.modelObject.WastageWeight = $scope.offerModel.hurdakg;
                    $scope.modelObject.LaborCost = $scope.offerModel.isci;
                    $scope.modelObject.ProfitRate = $scope.offerModel.marj;
                    $scope.modelObject.CreditMonths = $scope.offerModel.vadeay;
                    $scope.modelObject.CreditRate = $scope.offerModel.vadeyuzde;

                    for (var i = 0; i < headerData.data.length; i++) {
                        var dataRow = headerData.data[i];

                        // PUSH AS NEW OFFER DETAIL
                        var newId = 1;
                        if ($scope.modelObject.Details.length > 0) {
                            newId = $scope.modelObject.Details.map(d => d.Id).reduce((max, n) => n > max ? n : max)
                            newId++;
                        }

                        var visualData = dataRow['resim'].replace('<p><img src="data:image/png;base64,', '')
                            .replace('" ></p>', '');

                        $scope.modelObject.Details.push({
                            Id: newId,
                            NewDetail: true,
                            SheetWeight: dataRow.agirlik,
                            WastageWeight: parseInt(dataRow.tickness),
                            Quantity: dataRow.adet,
                            ItemExplanation: dataRow.parca_adi,
                            TotalPrice: dataRow.maliyet,
                            QualityExplanation: dataRow.malzeme,
                            UnitPrice: parseFloat(dataRow["adet-fiyat"]),
                            OrgUnitPrice: parseFloat(dataRow["adet-fiyat"]),
                            ItemVisualStr: visualData,
                        });
                    }

                    $scope.bindDetails();
                    $scope.closeOfferDialog();
                }
            }, function (resp) {
            }, function (evt) {
                
            });
        }
    }

    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0, OfferDate: moment().format('DD.MM.YYYY'), Details: []};
        $scope.selectedFirm = {};

        $scope.getNextOrderNo().then(function (rNo) {
            $scope.modelObject.OfferNo = rNo;
            $scope.$apply();
        });
        $scope.bindDetails();
    }

    $scope.createSaleOrder = function () {
        $scope.saveStatus = 1;

        $http.post(HOST_URL + 'SIOffer/CreateSaleOrder', { rid: $scope.modelObject.Id }, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.saveStatus = 0;

                    if (resp.data.Status == 1) {
                        toastr.success('Kayıt başarılı.', 'Bilgilendirme');

                        bootbox.confirm({
                            message: "Oluşturulan siparişi görüntülemek istiyor musunuz?",
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
                                    window.location.href = HOST_URL + 'SIOrder?rid=' + resp.data.RecordId;
                                }
                            }
                        });

                        $scope.bindModel($scope.modelObject.Id);
                    }
                    else
                        toastr.error(resp.data.ErrorMessage, 'Hata');
                }
            }).catch(function (err) { });
    }

    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu teklifi silmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'SIOffer/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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

        for (var i = 0; i < $scope.modelObject.Details.length; i++) {
            var dtObj = $scope.modelObject.Details[i];
            if (dtObj.ItemVisualStr != null && dtObj.ItemVisualStr.indexOf('"') > 0) {
                var badIndex = dtObj.ItemVisualStr.indexOf('"');
                dtObj.ItemVisualStr = dtObj.ItemVisualStr.substr(0, badIndex);
            }
        }

        $http.post(HOST_URL + 'SIOffer/SaveModel', $scope.modelObject, 'json')
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

    $scope.downloadOfferPdf = function () {
        // #region PREPARE PDF INPUT FORMAT
        var formatter = new Intl.NumberFormat('tr', {
            style: 'currency',
            currency: 'TRY',
        });

        var calcDetails = [];

        for (var i = 0; i < $scope.modelObject.Details.length; i++) {
            var dObj = $scope.modelObject.Details[i];
            calcDetails.push({
                "agirlik": $scope.modelObject.SheetWeight,
                "parca_adi": dObj.ItemExplanation,
                "adet": parseInt(dObj.Quantity),
                "maliyet": dObj.TotalPrice,
                "tickness": $scope.modelObject.WastageWeight.toString(),
                "maliyet-cur": formatter.format(dObj.TotalPrice),
                "malzeme": dObj.QualityExplanation,
                "resim": '<p><img src=\"data:image/png;base64,' + dObj.ItemVisualStr + '" ></p>',
                "adet-fiyat": dObj.UnitPrice.toFixed(2),
                "adet-fiyat-cur": formatter.format(dObj.UnitPrice),
                "tickness": "8",
            });
        }

        var calcObj = [{
            msg: 'ok', filename: 'wb.docx',
            sackg: $scope.modelObject.SheetWeight,
            hurdakg: $scope.modelObject.WastageWeight,
            isci: $scope.modelObject.isci,
            marj: $scope.modelObject.ProfitRate,
            vadeay: $scope.modelObject.CreditMonths,
            vadeyuzde: $scope.modelObject.CreditRate,
            vade: $scope.modelObject.Expiration,
            total: $scope.modelObject.TotalPrice,
            "to-company": $scope.selectedFirm.FirmName,
            "from-company": 'London Metal',
            "total-cur": $scope.modelObject.TotalPrice,
            data: calcDetails,
        }];
        // #endregion

        $http.post(METALIX_URL.replace('upload-docx', 'acceptOffer'), { goes: JSON.stringify(calcObj) }, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.downloadFile(METALIX_URL.replace('upload-docx','') + '/' + resp.data.pdf + ".pdf", resp.data.pdf);
                }
            }).catch(function (err) { });
    }

    $scope.downloadFile = function (urlToSend, token) {
        var req = new XMLHttpRequest();
        req.open("GET", urlToSend, true);
        req.responseType = "blob";
        req.onload = function (event) {
            var blob = req.response;
            var fileName = token; //if you have the fileName header available
            var link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = fileName;
            link.click();
        };

        req.send();
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

    $scope.dropDownBoxEditorTemplateRoute = function (cellElement, cellInfo) {
        return $("<div>").dxDropDownBox({
            dropDownOptions: { width: 600 },
            dataSource: $scope.routeList,
            value: cellInfo.value,
            showClearButton: true,
            valueExpr: "Id",
            displayExpr: "RouteCode",
            onValueChanged: function (e) {
                if (e.value == null && cellInfo.data.RouteId != null) {
                    cellInfo.data.RouteId = null;
                    cellInfo.data.RouteCode = '';
                    cellInfo.data.RouteName = '';
                    cellInfo.data.RoutePrice = 0;

                    var gridInst = $('#dataList').dxDataGrid('instance');
                    gridInst.refresh();

                    e.component.close();
                }
            },
            contentTemplate: function (e) {
                return $("<div>").dxDataGrid({
                    dataSource: $scope.routeList,
                    remoteOperations: true,
                    columns: [
                        { dataField: 'RouteCode', caption: 'Rota Kodu' },
                        { dataField: 'RouteName', caption: 'Rota Adı' },
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
        $http.get(HOST_URL + 'SIOffer/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;
                    //$scope.modelObject.DateOfNeed = $scope.modelObject.DateOfNeedStr;
                    //$scope.modelObject.OrderDate = $scope.modelObject.OrderDateStr;

                    if (typeof $scope.modelObject.FirmId != 'undefined' && $scope.modelObject.FirmId != null)
                        $scope.selectedFirm = $scope.firmList.find(d => d.Id == $scope.modelObject.FirmId);
                    else
                        $scope.selectedFirm = {};

                    $scope.offerModel.firmaismi = $scope.selectedFirm.FirmName;
                    $scope.offerModel.sackg = $scope.modelObject.SheetWeight;
                    $scope.offerModel.hurdakg = $scope.modelObject.WastageWeight;
                    $scope.offerModel.isci = $scope.modelObject.LaborCost;
                    $scope.offerModel.marj = $scope.modelObject.ProfitRate;
                    $scope.offerModel.vadeyuzde = $scope.modelObject.CreditRate;
                    $scope.offerModel.vadeay = $scope.modelObject.CreditMonths;

                    $scope.bindDetails();
                    $scope.calculateHeader();
                }
            }).catch(function (err) { });
    }

    $scope.bindConstants = function () {
        try {
            $http.get(HOST_URL + 'Common/GetOfferConstants', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.rawSheetPrice = resp.data.RawSheetPrice;
                        $scope.wastagePrice = resp.data.WastagePrice;
                        $scope.manPrice = resp.data.ManPrice;
                        $scope.profitRate = resp.data.ProfitRate;
                        $scope.expirationVal = resp.data.ExpirationVal;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.calculateRow = function (row) {
        if (typeof row != 'undefined' && row != null) {
            try {
                $http.post(HOST_URL + 'SIOffer/CalculateRow', row, 'json')
                    .then(function (resp) {
                        if (typeof resp.data != 'undefined' && resp.data != null) {
                            row.TotalPrice = resp.data.TotalPrice;
                            row.UnitPrice = resp.data.UnitPrice;

                            $scope.calculateHeader();

                            var gridInst = $('#dataList').dxDataGrid('instance');
                            gridInst.refresh();
                        }
                    }).catch(function (err) { });
            } catch (e) {

            }
        }
    }

    $scope.calculateHeader = function () {
        $scope.modelObject.TotalPrice = $scope.modelObject.Details.map(d => d.TotalPrice).reduce((n, x) => n + x);
        if (isNaN($scope.modelObject.TotalPrice))
            $scope.modelObject.TotalPrice = 0;
    }

    $scope.updateProcessListOfDetail = function (detailObj) {
        try {
            if (detailObj.RouteId != null && detailObj.RouteId > 0) {
                $http.get(HOST_URL + 'SIOffer/GetProcessList?routeId=' + detailObj.RouteId, {}, 'json')
                    .then(function (resp) {
                        if (typeof resp.data != 'undefined' && resp.data != null) {
                            detailObj.ProcessList = resp.data;
                        }
                    }).catch(function (err) { });
            }
            else
                detailObj.ProcessList = [];
        } catch (e) {

        }
    }

    $scope.bindDetails = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    $scope.modelObject.Details.forEach(d => {
                        if (typeof d.OrgUnitPrice == 'undefined' || d.OrgUnitPrice == null)
                            d.OrgUnitPrice = d.UnitPrice;
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

                        let refreshProcessList = false;
                        if (typeof values.RouteId != 'undefined') {
                            if (obj.RouteId != values.RouteId) {
                                refreshProcessList = true;
                            }

                            var itemObj = $scope.routeList.find(d => d.Id == values.RouteId);
                            obj.RouteId = itemObj.Id;
                            obj.RouteCode = itemObj.RouteCode;

                            if (refreshProcessList == true)
                                obj.RoutePrice = itemObj.UnitPrice;

                            calculateRowAgain = true;
                        }
                        else {
                            obj.RoutePrice = 0;
                            obj.RouteId = null;
                        }

                        let dontUpdateUnitPrice = false;
                        if ($scope.priceToleranceMax > 0 && $scope.priceToleranceMin > 0 && obj.OrgUnitPrice > 0) {
                            if (obj.OrgUnitPrice + (obj.OrgUnitPrice * $scope.priceToleranceMax / 100.0) < values.UnitPrice) {
                                dontUpdateUnitPrice = true;
                                toastr.error('Fiyat değişikliği maksimum töleransın(' + $scope.priceToleranceMax + '%) üzerindedir. '
                                    + 'Bu kalem için girilebilecek en yüksek fiyat: ' + (obj.OrgUnitPrice + (obj.OrgUnitPrice * $scope.priceToleranceMax / 100.0)).toFixed(2));
                            }
                            else if (obj.OrgUnitPrice - (obj.OrgUnitPrice * $scope.priceToleranceMin / 100.0) > values.UnitPrice) {
                                dontUpdateUnitPrice = true;
                                toastr.error('Fiyat değişikliği minimum töleransın(' + $scope.priceToleranceMin + '%) üzerindedir. '
                                    + 'Bu kalem için girilebilecek en düşük fiyat: ' + (obj.OrgUnitPrice - (obj.OrgUnitPrice * $scope.priceToleranceMin / 100.0)).toFixed(2));
                            }
                        }

                        if (typeof values.Quantity != 'undefined') { obj.Quantity = values.Quantity; calculateRowAgain = true; }
                        if (typeof values.UnitPrice != 'undefined' && !dontUpdateUnitPrice) { obj.UnitPrice = values.UnitPrice; calculateRowAgain = true; }
                        if (typeof values.ItemExplanation != 'undefined') { obj.ItemExplanation = values.ItemExplanation; }
                        if (typeof values.QualityExplanation != 'undefined') { obj.QualityExplanation = values.QualityExplanation; }
                        if (typeof values.SheetWeight != 'undefined') { obj.SheetWeight = values.SheetWeight; }
                        if (typeof values.LaborCost != 'undefined') { obj.LaborCost = values.LaborCost; }
                        if (typeof values.WastageWeight != 'undefined') { obj.WastageWeight = values.WastageWeight; }
                        if (typeof values.ProfitRate != 'undefined') { obj.ProfitRate = values.ProfitRate; }
                        if (typeof values.CreditMonths != 'undefined') { obj.CreditMonths = values.CreditMonths; }
                        if (typeof values.CreditRate != 'undefined') { obj.CreditRate = values.CreditRate; }

                        if (refreshProcessList) {
                            $scope.updateProcessListOfDetail(obj);
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
                    var routeObj = $scope.routeList.find(d => d.Id == values.RouteId);

                    var newObj = {
                        Id: newId,
                        ItemId: itemObj.Id,
                        ItemNo: itemObj.ItemNo,
                        ItemName: itemObj.ItemName,
                        RouteId: values.RouteId,
                        RouteCode: routeObj != null ? routeObj.RouteCode : '',
                        ItemExplanation: itemObj.ItemExplanation,
                        QualityExplanation: itemObj.QualityExplanation,
                        SheetWeight: itemObj.SheetWeight,
                        LaborCost: itemObj.LaborCost,
                        WastageWeight: itemObj.WastageWeight,
                        ProfitRate: itemObj.ProfitRate,
                        CreditMonths: itemObj.CreditMonths,
                        CreditRate: itemObj.CreditRate,
                        Quantity: values.Quantity,
                        ItemVisual: values.ItemVisual,
                        UnitPrice: values.UnitPrice,
                        ProcessList: [],
                        NewDetail: true
                    };

                    $scope.modelObject.Details.push(newObj);
                    $scope.calculateRow(newObj);

                    if (routeObj != null && routeObj.Id > 0)
                        $scope.updateProcessListOfDetail(newObj);
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
            allowColumnResizing: true,
            wordWrapEnabled: true,
            onInitNewRow: function (e) {
                e.data.UnitPrice = 0;
                e.data.TaxIncluded = 0;
            },
            repaintChangesOnly: true,
            onCellPrepared: function (e) {
                
            },
            columns: [
                {
                    dataField: 'ItemVisualStr', caption: 'Görsel', allowEditing: false,
                    cellTemplate: function (element, info) {
                        
                        if (info.displayValue != null && info.displayValue.length > 0)
                            element.append('<image src="data:image/png;base64,' + info.displayValue + '" />');
                    }
                },
                {
                    dataField: 'ItemId', caption: 'Stok Kodu',
                    lookup: {
                        dataSource: $scope.itemList,
                        valueExpr: "Id",
                        displayExpr: "ItemNo"
                    },
                    allowSorting: false,
                    /*validationRules: [{ type: "required" }],*/
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
                { dataField: 'ItemExplanation', caption: 'Parça Adı', allowEditing: false },
                { dataField: 'QualityExplanation', caption: 'Kalite', allowEditing: false },
                { dataField: 'Quantity', caption: 'Miktar', allowEditing:false, dataType: 'number', format: { type: "fixedPoint", precision: 2 }, },
                {
                    dataField: 'UnitPrice', caption: 'Birim Fiyat',
                    dataType: 'number',
                    format: { type: "fixedPoint", precision: 2 },
                },
                {
                    dataField: 'RouteId', caption: 'Rota Kodu',
                    lookup: {
                        dataSource: $scope.routeList,
                        valueExpr: "Id",
                        displayExpr: "RouteCode"
                    },
                    allowClearing: true,
                    allowSorting: false,
                    editCellTemplate: $scope.dropDownBoxEditorTemplateRoute,
                    cellTemplate: function (container, options) {
                        if (typeof options.row.data.RouteCode != 'undefined'
                            && options.row.data.RouteCode != null && options.row.data.RouteCode.length > 0)
                            container.text(options.row.data.RouteCode);
                        else
                            container.text(options.displayValue);
                    }
                },
                { dataField: 'RoutePrice', caption: 'Proses Fiyatı', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, },
                //{ dataField: 'SheetWeight', caption: 'Sac Kg', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, },
                //{ dataField: 'LaborCost', caption: 'İşçilik', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, },
                //{ dataField: 'WastageWeight', caption: 'Hurda Kg', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, },
                //{ dataField: 'ProfitRate', caption: 'Kar Marji', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, },
                //{ dataField: 'CreditMonths', caption: 'Vade Ay', dataType: 'number', format: { type: "fixedPoint", precision: 0 }, },
                //{ dataField: 'CreditRate', caption: 'Vade %', dataType: 'number', format: { type: "fixedPoint", precision: 0 }, },
                { dataField: 'TotalPrice', allowEditing: false, caption: 'Satır Tutarı', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                {
                    type: "buttons",
                    buttons: [
                        {
                            name: 'delete', cssClass: '', text: '', onClick: function (e) {
                                $('#dataList').dxDataGrid('instance').deleteRow(e.row.rowIndex);
                            }
                        },
                        {
                            name: 'routes', cssClass: 'fas fa-list', text: '', onClick: function (e) {
                                $scope.showOfferProcess(e.row.data);
                            }
                        },
                    ]
                }
            ]
        });
    }

    $scope.loadSelectables = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'SIOffer/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.itemList = resp.data.Items;
                        $scope.routeList = resp.data.Routes;

                        $scope.firmList = resp.data.Firms;
                        var emptyFirmObj = { Id: 0, FirmCode: '-- Seçiniz --' };
                        $scope.firmList.splice(0, 0, emptyFirmObj);
                        $scope.selectedFirm = emptyFirmObj;

                        $scope.priceToleranceMin = resp.data.PriceToleranceMin;
                        $scope.priceToleranceMax = resp.data.PriceToleranceMax;

                        resolve();
                    }
                }).catch(function (err) { });
        });

        return prms;
    }

    // PROCESS LIST PRICING DIALOG
    $scope.showOfferProcess = function (detailObj) {
        $scope.viewingRow = detailObj;
        $scope.$broadcast('loadProcessList', detailObj.ProcessList);

        $('#dial-offer-process').dialog({
            width: 600,
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

    $scope.$on('transferProcessList', function (e, d) {
        try {
            $scope.viewingRow.ProcessList = d;
            $scope.viewingRow.RoutePrice = $scope.viewingRow.ProcessList.map(d => d.UnitPrice).reduce((n, x) => n + x);
            $scope.calculateRow($scope.viewingRow);

            var gridInst = $('#dataList').dxDataGrid('instance');
            gridInst.refresh();
        } catch (e) {

        }

        $('#dial-offer-process').dialog('close');
    });

    // OFFER CONSTANT PRICES DIALOG
    $scope.showOfferConstants = function () {
        $scope.$broadcast('loadOfferConstants', null);

        $('#dial-offer-constants').dialog({
            width: 600,
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

    $scope.$on('editOfferConstantsEnd', function (e, d) {
        $scope.bindConstants();

        $('#dial-offer-constants').dialog('close');
    });

    // INFORMATIONS & ATTACHMENTS
    $scope.showRecordInformation = function () {
        $scope.$broadcast('showRecordInformation', { Id: $scope.modelObject.Id, DataType:'ItemOffer' });
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

    // ON LOAD EVENTS
    DevExpress.localization.locale('tr');
    $scope.loadSelectables().then(function () {
        if (PRM_ID > 0)
            $scope.bindModel(PRM_ID);
        else {
            $scope.getNextOrderNo().then(function (rNo) {
                $scope.modelObject.OfferNo = rNo;
                $scope.$apply();

                $scope.bindDetails();
            });
        }
    });
}]);