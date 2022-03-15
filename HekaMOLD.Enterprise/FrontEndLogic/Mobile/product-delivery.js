app.controller('productDeliveryCtrl', function ($scope, $http, $timeout) {
    $scope.modelObject = {
        Id:0,
        DocumentNo: '', FirmId: 0,
        FirmCode:'', FirmName:'',
        Details: [], OrderDetails: [],
    };

    $scope.barcodeBox = '';

    $scope.lastRecordId = 0;
    $scope.reportTemplateId = 0;

    $scope.pickupList = [];
    $scope.filteredPickupList = [];
    $scope.summaryList = [];
    $scope.selectedProducts = [];

    $scope.sumTotalCount = 0;
    $scope.sumTotalQty = 0;

    $scope.bindModel = function () {
        $scope.pickupList = [];
        $scope.filteredPickupList = [];
        $scope.summaryList = [];

        //$http.get(HOST_URL + 'Mobile/GetProductsForDelivery', {}, 'json')
        //    .then(function (resp) {
        //        if (typeof resp.data != 'undefined' && resp.data != null) {
        //            $scope.pickupList = resp.data.Serials;
        //            $scope.filteredPickupList = $scope.pickupList;
        //            $scope.summaryList = resp.data.Summaries;
        //        }
        //    }).catch(function (err) { });
    }

    $scope.onBarcodeKeyUp = function (e) {
        if (e.keyCode == '13') {
            $scope.getSerialByBarcode($scope.barcodeBox);
        }
    }

    $scope.getSerialByBarcode = function (barcode) {
        if ($scope.pickupList.some(m => m.SerialNo == barcode)) {
            toastr.warning('Okutulan koli zaten çeki listesine eklenmiş.');
            return;
        }

        $http.get(HOST_URL + 'Mobile/GetItemSerialByBarcode?barcode=' + barcode, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $timeout(function () {
                        if (resp.data.Id > 0) {
                            $scope.pickupList.push({ ...resp.data });
                            $scope.pickupList = $scope.pickupList.filter(d => d.ItemNo != null);

                            toastr.success('Okutulan koli çeki listesine eklendi.');
                            $scope.updateSummaryList();
                        }
                        else
                            toastr.error('Okutulan barkoda ait bir koli bulunamadı.');

                        $scope.barcodeBox = '';
                    });
                }
            }).catch(function (err) { });
    }

    $scope.removeFromPickup = function (item) {
        try {
            $timeout(function () {
                var indexOfItem = $scope.pickupList.indexOf(item);
                $scope.pickupList.splice(indexOfItem, 1);

                $scope.updateSummaryList();
            })
        } catch (e) {

        }
    }

    $scope.updateSummaryList = function () {
        $timeout(function () {
            $scope.summaryList.splice(0, $scope.summaryList.length);
            $scope.sumTotalCount = 0;
            $scope.sumTotalQty = 0;

            $scope.summaryList = [...$scope.pickupList.map((d) => {
                return {
                    ItemName: d.ItemName,
                    SerialSum: d.FirstQuantity,
                    SerialCount: 1,
                };
            }).reduce(
                (map, item) => {
                    const { ItemName: key, SerialCount, SerialSum } = item;
                    const prev = map.get(key);

                    $scope.sumTotalQty += SerialSum;
                    $scope.sumTotalCount += SerialCount;

                    if (prev) {
                        prev.SerialSum += SerialSum
                        prev.SerialCount += SerialCount
                    } else {
                        map.set(key, Object.assign({}, item))
                    }

                    return map
                },
                new Map())
                ];
        });

        try {
            $scope.$applyAsync();
        } catch (e) {

        }
    }

    $scope.getListSum = function (list, key) {
        if (list != null && list.length > 0)
            return getSumOf(list, key);

        return '';
    }

    $scope.selectedWarehouse = { Id: 0, WarehouseName: '' };
    $scope.warehouseList = [];

    $scope.selectedFirm = { Id: 0, FirmName: '' };
    $scope.firmList = [];

    $scope.selectProduct = function (item) {
        if ($scope.selectedProducts.some(d => d.Id == item.Id))
            $scope.selectedProducts.splice($scope.selectedProducts.indexOf(item), 1);
        else
            $scope.selectedProducts.push(item);
    }

    $scope.isSelectedProduct = function (item) {
        return $scope.selectedProducts.some(d => d.Id == item.Id);
    }

    $scope.processBarcodeResult = function (barcode) {
        var product = $scope.pickupList.find(d => d.SerialNo == barcode);
        if (product != null && typeof product != 'undefined') {
            $scope.selectProduct(product);
            $scope.isBarcodeRead = true;
            try {
                $scope.$apply();
            } catch (e) {

            }
        }
    }

    $scope.isBarcodeRead = false;

    $scope.qrScanner = null;

    $scope.readBarcode = function () {
        $scope.isBarcodeRead = false;
        bootbox.alert({
            message: '<div class="mx-auto" id="reader"></div>',
            closeButton: false,
            locale: 'tr',
            callback: function () {
                try {
                    $scope.qrScanner.stop();
                } catch (e) {

                }

                bootbox.hideAll();
            }
        });

        // TO REQUEST WEB CAM ACCESS
        Html5Qrcode.getCameras().then(devices => {
            /**
             * devices would be an array of objects of type:
             * { id: "id", label: "label" }
             */
            if (devices && devices.length) {
                var cameraId = devices[0].id;
                // .. use this to start scanning.
            }
        }).catch(err => { });

        // WEB CAM READER OBJECT
        $scope.qrScanner = new Html5Qrcode(
            "reader");

        $scope.qrScanner.start(
            { facingMode: "environment" }, // prefers back, for the front camera use 'user'
            {
                fps: 10,    // sets the framerate to 10 frame per second
                qrbox: 250,  // sets only 250 X 250 region of viewfinder to
                // scannable, rest shaded.
            },
            qrCodeMessage => {
                if (!$scope.isBarcodeRead) {
                    $scope.getSerialByBarcode(qrCodeMessage);
                    /*$scope.processBarcodeResult(qrCodeMessage);*/
                    setTimeout(() => { $scope.isBarcodeRead = false; }, 1200);
                }
            },
            errorMessage => {
            })
            .catch(err => {
            });
    }

    $scope.loadSelectables = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Mobile/GetSelectables?action=Delivery', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {

                        $scope.warehouseList = resp.data.Warehouses;
                        $scope.firmList = resp.data.Firms;

                        var emptyWrObj = { Id: 0, WarehouseName: '-- Seçiniz --' };
                        $scope.warehouseList.splice(0, 0, emptyWrObj);
                        $scope.selectedWarehouse = emptyWrObj;

                        var emptyFrObj = { Id: 0, FirmName: '-- Seçiniz --' };
                        $scope.firmList.splice(0, 0, emptyFrObj);
                        $scope.selectedFirm = emptyFrObj;

                        resolve();
                    }
                }).catch(function (err) { });
        });

        return prms;
    }

    // EXTERNAL COMPONENT CALLING
    $scope.showDeliveryPlan = function () {
        // DO BROADCAST
        $scope.$broadcast('loadDeliveryPlanList');

        $('#dial-deliveryplans').dialog({
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

    $scope.showPrintTemplates = function () {
        if ($scope.lastRecordId > 0) {
            // DO BROADCAST
            $scope.$broadcast('loadTemplateList', [2]);

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

    $scope.showOrderList = function () {
        // DO BROADCAST
        $scope.$broadcast('loadOpenSoList');

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

    $scope.showSelectProductDialog = function () {
        // DO BROADCAST
        $scope.$broadcast('loadProductList', { productSelection: true, list: $scope.pickupList, });

        $('#dial-wr-manager').dialog({
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

    // RECEIVE EMIT DELIVERY PLAN DATA
    $scope.$on('transferDeliveryPlans', function (e, d) {
        var firstOrderRecord = null;
        var productsOfPlan = [];

        if (d.length && d.length > 0)
            firstOrderRecord = d[0];

        d.forEach(x => {
            if (!productsOfPlan.some(m => m == x.ItemOrder.ItemId))
                productsOfPlan.push(x.ItemOrder.ItemId);
        });

        $scope.filteredPickupList = $scope.pickupList.filter(m => productsOfPlan.includes(m.ItemId));

        if (firstOrderRecord != null) {
            $scope.selectedFirm = $scope.firmList.find(m => m.Id == firstOrderRecord.FirmId);
            refreshArray($scope.firmList);
        }

        $('#dial-deliveryplans').dialog('close');
    });

    // RECEIVE EMIT REPORT PRINT DATA
    $scope.$on('printTemplate', function (e, d) {
        if (d.templateId > 0) {
            $scope.reportTemplateId = d.templateId;

            if (d.exportType == 'PDF') {
                try {
                    $http.post(HOST_URL + 'Printing/ExportAsPdf', {
                        objectId: $scope.lastRecordId,
                        reportId: $scope.reportTemplateId,
                        reportType: 2,
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
                objectId: $scope.lastRecordId,
                reportId: $scope.reportTemplateId,
                printerId: d.PrinterId,
                recordType: 5, // delivery list type
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

    // RECEIVE EMIT SALE ORDER DATA
    $scope.$on('transferOrderDetails', function (e, d) {
        var firstOrderRecord = null;

        d.forEach(x => {
            if (firstOrderRecord == null)
                firstOrderRecord = x;

            if ($scope.modelObject.OrderDetails.filter(m => m.ItemOrderDetailId == x.Id).length > 0) {
                toastr.warning(x.OrderNo + ' nolu sipariş, ' + x.ItemNo + ' / ' + x.ItemName + ', ' + x.Quantity
                    + ' miktarlı sipariş detayı zaten aktarıldığı için tekrar dahil edilmedi.', 'Uyarı');
            }
            else {
                var newId = 1;
                if ($scope.modelObject.OrderDetails.length > 0) {
                    newId = $scope.modelObject.OrderDetails.map(d => d.Id).reduce((max, n) => n > max ? n : max)
                    newId++;
                }

                $scope.modelObject.OrderDetails.push({
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
            $scope.selectedFirm = $scope.firmList.find(d => d.Id == firstOrderRecord.FirmId);
            refreshArray($scope.firmList);
        }

        $('#dial-orderlist').dialog('close');
    });

    // RECEIVE EMIT SELECTED PACKAGE DATA
    $scope.$on('packageSelected', function (e, d) {
        $scope.getSerialByBarcode(d.SerialNo);

        //$('#dial-wr-manager').dialog('close');
    });

    $scope.clearSelectedOrders = function () {
        $scope.modelObject.OrderDetails.splice(0, $scope.modelObject.OrderDetails.length);
    }

    $scope.approveDelivery = function () {
        bootbox.confirm({
            message: "Bu sevkiyat listesini onaylıyor musunuz?",
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

                    if (typeof $scope.selectedFirm != 'undefined'
                        && $scope.selectedFirm != null)
                        $scope.modelObject.FirmId = $scope.selectedFirm.Id;
                    else
                        $scope.modelObject.FirmId = null;

                    //if ($scope.modelObject.InWarehouseId == null) {
                    //    toastr.error('Depo seçmelisiniz.');
                    //    return;
                    //}

                    if ($scope.modelObject.FirmId == null) {
                        toastr.error('Firma seçmelisiniz.');
                        return;
                    }

                    //if ($scope.selectedWarehouse.WarehouseType != 2) {
                    //    toastr.error('Ürün deposu seçmelisiniz.');
                    //    return;
                    //}

                    $http.post(HOST_URL + 'Mobile/SaveProductDelivery', {
                        receiptModel: $scope.modelObject,
                        model: $scope.pickupList,
                        orderDetails: $scope.modelObject.OrderDetails.map(m => m.ItemOrderDetailId),
                    }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Status == 1) {
                                    toastr.success('Kayıt başarılı.', 'Bilgilendirme');

                                    $scope.lastRecordId = resp.data.RecordId;
                                    $scope.showPrintTemplates();

                                    $scope.modelObject = {
                                        DocumentNo: '', FirmId: 0,
                                        FirmCode: '', FirmName: '',
                                        Details: [], OrderDetails: [],
                                    };

                                    $scope.bindModel();
                                }
                                else
                                    toastr.error(resp.data.ErrorMessage, 'Hata');
                            }
                        }).catch(function (err) { });
                }
            }
        });
    }

    // LOAD EVENTS
    $scope.loadSelectables().then(function () {
        refreshArray($scope.warehouseList);
        $scope.bindModel();
    });
});