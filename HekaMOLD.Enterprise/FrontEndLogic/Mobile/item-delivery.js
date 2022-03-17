app.controller('itemDeliveryCtrl', function ($scope, $http) {
    $scope.modelObject = {
        Id: 0,
        DocumentNo: '', FirmId: 0,
        FirmCode: '', FirmName: '',
        Details: [], OrderDetails: [],
    };

    $scope.lastRecordId = 0;
    $scope.reportTemplateId = 0;

    $scope.pickupList = [];
    $scope.filteredPickupList = [];
    $scope.summaryList = [];
    $scope.selectedProducts = [];

    $scope.bindModel = function () {
        $http.get(HOST_URL + 'Mobile/GetItemsForDelivery', {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.pickupList = resp.data.Serials;
                    $scope.filteredPickupList = $scope.pickupList;
                    $scope.summaryList = resp.data.Summaries;
                }
            }).catch(function (err) { });
    }

    $scope.getListSum = function (list, key) {
        if (list != null && list.length > 0)
            return getSumOf(list, key);

        return '';
    }

    $scope.selectedWarehouse = { Id: 0, WarehouseName: '' };
    $scope.warehouseList = [];

    $scope.selectProduct = function (item) {
        
        if ($scope.selectedProducts.some(d => d.Id == item.Id))
            $scope.selectedProducts.splice($scope.selectedProducts.indexOf(item), 1);
        else {
            $scope.selectedProducts.splice(0, $scope.selectedProducts.length);
            $scope.selectedProducts.push(item);
        }
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
                    $scope.processBarcodeResult(qrCodeMessage);
                    setTimeout(() => { $scope.isBarcodeRead = false; }, 1500);
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

                        var emptyWrObj = { Id: 0, WarehouseName: '-- Seçiniz --' };
                        $scope.warehouseList.splice(0, 0, emptyWrObj);
                        $scope.selectedWarehouse = emptyWrObj;

                        resolve();
                    }
                }).catch(function (err) { });
        });

        return prms;
    }

    $scope.approveDelivery = function () {
        bootbox.prompt({
            message: "Miktar Girin",
            closeButton: false,
            title: 'Malzeme teslimatı için miktar girin',
            buttons: {
                confirm: {
                    label: 'Devam Et',
                    className: 'btn-primary'
                },
                cancel: {
                    label: 'Vazgeç',
                    className: 'btn-light'
                }
            },
            callback: function (resultQuantity) {
                if (resultQuantity != null && parseFloat(resultQuantity) > 0) {
                    bootbox.confirm({
                        message: "Bu malzeme teslimat listesini onaylıyor musunuz?",
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

                                if ($scope.modelObject.InWarehouseId == null) {
                                    toastr.error('Depo seçmelisiniz.');
                                    return;
                                }

                                $http.post(HOST_URL + 'Mobile/SaveItemDelivery', {
                                    itemReceiptDetailId: $scope.selectedProducts[0].Id,
                                    quantity: parseFloat(resultQuantity),
                                }, 'json')
                                    .then(function (resp) {
                                        if (typeof resp.data != 'undefined' && resp.data != null) {
                                            $scope.saveStatus = 0;

                                            if (resp.data.Status == 1) {
                                                toastr.success('Kayıt başarılı.', 'Bilgilendirme');

                                                $scope.lastRecordId = resp.data.RecordId;

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
            }
        });
    }

    // LOAD EVENTS
    $scope.loadSelectables().then(function () {
        refreshArray($scope.warehouseList);
        
        $scope.bindModel();
    });
});