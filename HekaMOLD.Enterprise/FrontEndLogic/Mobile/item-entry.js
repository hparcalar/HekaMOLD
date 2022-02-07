app.controller('itemEntryCtrl', function ($scope, $http) {
    $scope.modelObject = {
        Id: 0,
        DocumentNo: '', FirmId: 0,
        FirmCode: '', FirmName: '',
        Details: []
    };

    $scope.bindModel = function (id) {
        $http.get(HOST_URL + 'Mobile/GetItemEntryData?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;

                    if (typeof $scope.modelObject.InWarehouseId != 'undefined' && $scope.modelObject.InWarehouseId != null)
                        $scope.selectedWarehouse = $scope.warehouseList.find(d => d.Id == $scope.modelObject.InWarehouseId);
                    else
                        $scope.selectedWarehouse = $scope.warehouseList[0];

                    if (typeof $scope.modelObject.FirmId != 'undefined' && $scope.modelObject.FirmId != null)
                        $scope.selectedFirm = $scope.firmList.find(d => d.Id == $scope.modelObject.FirmId);
                    else
                        $scope.selectedFirm = $scope.firmList[0];

                    refreshArray($scope.warehouseList);
                    refreshArray($scope.firmList);
                }
            }).catch(function (err) { });
    }

    $scope.selectedWarehouse = { Id: 0, WarehouseName: '' };
    $scope.warehouseList = [];

    $scope.selectedFirm = { Id: 0, FirmCode: '', FirmName: '' };
    $scope.firmList = [];

    $scope.showOrderList = function () {
        // DO BROADCAST
        $scope.$broadcast('loadOpenPoList');

        $('#dial-orderlist').dialog({
            width: window.innerWidth * 0.95,
            height: window.innerHeight * 0.95,
            hide: true,
            modal: true,
            resizable: false,
            show: true,
            draggable: false,
            closeText: "KAPAT"
        });
    }

    $scope.showItemList = function () {
        // DO BROADCAST
        $scope.$broadcast('loadItemList');

        $('#dial-itemlist').dialog({
            width: window.innerWidth * 0.95,
            height: window.innerHeight * 0.95,
            hide: true,
            modal: true,
            resizable: false,
            show: true,
            draggable: false,
            closeText: "KAPAT"
        });
    }

    $scope.loadSelectables = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Mobile/GetSelectables?action=ItemEntry', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {

                        $scope.warehouseList = resp.data.Warehouses;
                        var emptyWrObj = { Id: 0, WarehouseName: '-- Seçiniz --' };
                        $scope.warehouseList.splice(0, 0, emptyWrObj);
                        $scope.selectedWarehouse = emptyWrObj;

                        $scope.firmList = resp.data.Firms;
                        var emptyFirmObj = { Id: 0, FirmCode: '-- Seçiniz --', FirmName: '' };
                        $scope.firmList.splice(0, 0, emptyFirmObj);
                        $scope.selectedFirm = emptyFirmObj;

                        resolve();
                    }
                }).catch(function (err) { });
        });

        return prms;
    }

    $scope.approveItemEntry = function () {
        bootbox.confirm({
            message: "Bu irsaliye girişini onaylıyor musunuz?",
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

                    $scope.modelObject.ReceiptDate = moment().format('DD.MM.YYYY');

                    if (typeof $scope.selectedWarehouse != 'undefined'
                        && $scope.selectedWarehouse != null)
                        $scope.modelObject.InWarehouseId = $scope.selectedWarehouse.Id;
                    else
                        $scope.modelObject.InWarehouseId = null;

                    if (typeof $scope.selectedFirm != 'undefined' && $scope.selectedFirm != null)
                        $scope.modelObject.FirmId = $scope.selectedFirm.Id;
                    else
                        $scope.modelObject.FirmId = null;

                    $http.post(HOST_URL + 'Mobile/SaveItemEntry', $scope.modelObject, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Status == 1) {
                                    toastr.success('Kayıt başarılı.', 'Bilgilendirme');

                                    $scope.bindModel(resp.data.RecordId);
                                    //$scope.modelObject = {
                                    //    DocumentNo: '', FirmId: 0,
                                    //    FirmCode: '', FirmName: '',
                                    //    Details: []
                                    //};
                                }
                                else
                                    toastr.error(resp.data.ErrorMessage, 'Hata');
                            }
                        }).catch(function (err) { });
                }
            }
        });
    }

    // EMIT ORDER DETAIL DATA
    $scope.$on('transferOrderDetails', function (e, d) {
        var firstOrderRecord = null;

        d.forEach(x => {
            if (firstOrderRecord == null)
                firstOrderRecord = x;

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
                    FirmName: x.FirmName,
                    ItemId: x.ItemId,
                    ItemNo: x.ItemNo,
                    ItemName: x.ItemName,
                    UnitId: x.UnitId,
                    UnitCode: x.UnitCode,
                    UnitName: x.UnitCode,
                    Quantity: x.Quantity,
                    UnitPrice: 0,
                    NewDetail: true,
                    ItemOrderDetailId: x.Id
                });
            }
        });

        if (firstOrderRecord != null) {
            $scope.modelObject.FirmId = firstOrderRecord.FirmId;

            if (typeof $scope.modelObject.FirmId != 'undefined' && $scope.modelObject.FirmId != null)
                $scope.selectedFirm = $scope.firmList.find(d => d.Id == $scope.modelObject.FirmId);
            else
                $scope.selectedFirm = $scope.firmList[0];

            refreshArray($scope.firmList);

            $scope.modelObject.FirmCode = firstOrderRecord.FirmCode;
            $scope.modelObject.FirmName = firstOrderRecord.FirmName;
        }

        $('#dial-orderlist').dialog('close');
    });

    $scope.$on('transferItems', function (e, d) {
        d.forEach(x => {
            var newId = 1;
            if ($scope.modelObject.Details.length > 0) {
                newId = $scope.modelObject.Details.map(d => d.Id).reduce((max, n) => n > max ? n : max)
                newId++;
            }

            $scope.modelObject.Details.push({
                Id: newId,
                ItemId: x.Id,
                ItemNo: x.ItemNo,
                ItemName: x.ItemName,
                Quantity: 0,
                UnitPrice: 0,
                NewDetail: true,
            });
        });

        $('#dial-itemlist').dialog('close');
    });

    // #region PRINTING LOGIC
    $scope.reportTemplateType = 6;
    $scope.showPrintTemplates = function () {
        if ($scope.selectedRow.Id > 0) {
            // DO BROADCAST
            $scope.$broadcast('loadTemplateList', [6]);
            $scope.reportTemplateType = 6;

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
                        reportType: $scope.reportTemplateType,
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
                recordType: $scope.reportTemplateType,
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

    $scope.selectedRow = { Id: 0 };
    $scope.reportTemplateId = 0;
    $scope.printMaterialLabel = function (row) {
        $scope.selectedRow = row;
        $scope.showPrintTemplates();
    }
    // #endregion

    // LOAD EVENTS
    $scope.loadSelectables().then(function () {
        refreshArray($scope.warehouseList);

        var recordId = getParameterByName('rid');
        if (typeof recordId != 'undefined' &&
            recordId != null && recordId.length > 0) {
            $scope.bindModel(parseInt(recordId));
        }
    });
});