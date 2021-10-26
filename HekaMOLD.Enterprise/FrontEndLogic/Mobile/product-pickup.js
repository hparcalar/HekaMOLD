app.controller('productPickupCtrl', function ($scope, $http) {
    $scope.modelObject = {
        Id:0,
        DocumentNo: '', FirmId: 0,
        FirmCode:'', FirmName:'',
        Details: []
    };

    $scope.pickupList = [];
    $scope.filteredPickupList = [];
    $scope.shiftList = [];

    $scope.summaryList = [];
    $scope.filteredSummaryList = [];
    $scope.selectedProducts = [];
    $scope.selectedSummary = { ItemName: '' };
    $scope.selectedShift = { Id: 0 };

    $scope.bindModel = function () {
        $http.get(HOST_URL + 'Mobile/GetProductsForPickup', {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.pickupList = resp.data.Serials;
                    $scope.filteredPickupList = $scope.pickupList;
                    $scope.summaryList = resp.data.Summaries;
                    $scope.filteredSummaryList = $scope.summaryList;
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
        if (item.QualityStatus != 1) {
            toastr.error('Kalite onayı almamış ürün depoya girilemez.');
            return;
        }

        if ($scope.selectedProducts.some(d => d.Id == item.Id))
            $scope.selectedProducts.splice($scope.selectedProducts.indexOf(item), 1);
        else
            $scope.selectedProducts.push(item);
    }

    $scope.isSelectedProduct = function (item) {
        return $scope.selectedProducts.some(d => d.Id == item.Id);
    }

    $scope.selectSummary = function (item) {
        if ($scope.selectedSummary.ItemName == item.ItemName) {
            $scope.selectedSummary.ItemName = '';
            $scope.filteredPickupList = $scope.pickupList;
        }
        else {
            $scope.selectedSummary.ItemName = item.ItemName;
            $scope.filteredPickupList = $scope.pickupList.filter(d => d.ItemName == item.ItemName);
        }
    }

    $scope.selectShift = function (item) {
        if ($scope.selectedShift.Id == item.Id) {
            $scope.selectedShift.Id = 0;
            $scope.filteredSummaryList = $scope.summaryList;
            $scope.filteredPickupList = $scope.pickupList;
        }
        else {
            $scope.selectedShift.Id = item.Id;
            $scope.filteredSummaryList = $scope.summaryList.filter(d => d.ShiftCode == item.ShiftCode);
            $scope.filteredPickupList = $scope.pickupList.filter(d => d.ShiftCode == item.ShiftCode);
        }
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

    $scope.readBarcode = function () {
        $scope.isBarcodeRead = false;
        bootbox.alert({
            message: '<div style="width: 500px" id="reader"></div>',
            locale: 'tr'
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
        let qrScanner = new Html5Qrcode(
            "reader");

        qrScanner.start(
            { facingMode: "environment" }, // prefers back, for the front camera use 'user'
            {
                fps: 10,    // sets the framerate to 10 frame per second
                qrbox: 250  // sets only 250 X 250 region of viewfinder to
                // scannable, rest shaded.
            },
            qrCodeMessage => {
                bootbox.hideAll();
                if (!$scope.isBarcodeRead)
                    $scope.processBarcodeResult(qrCodeMessage);
                //qrScanner.stop();
                Html5Qrcode.stop();
            },
            errorMessage => {
            })
            .catch(err => {
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

                        $scope.shiftList = resp.data.Shifts;

                        resolve();
                    }
                }).catch(function (err) { });
        });

        return prms;
    }

    $scope.approveItemEntry = function () {
        bootbox.confirm({
            message: "Bu ürün teslimatını onaylıyor musunuz?",
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

                    if ($scope.selectedWarehouse.WarehouseType != 2) {
                        toastr.error('Ürün deposu seçmelisiniz.');
                        return;
                    }

                    $http.post(HOST_URL + 'Mobile/SaveProductPickup', { receiptModel: $scope.modelObject, model: $scope.selectedProducts }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Status == 1) {
                                    toastr.success('Kayıt başarılı.', 'Bilgilendirme');

                                    $scope.modelObject = {
                                        DocumentNo: '', FirmId: 0,
                                        FirmCode: '', FirmName: '',
                                        Details: []
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