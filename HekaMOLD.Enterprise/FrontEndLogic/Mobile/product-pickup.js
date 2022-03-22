﻿app.controller('productPickupCtrl', function ($scope, $http, $timeout) {
    $scope.modelObject = {
        Id:0,
        DocumentNo: '', FirmId: 0,
        FirmCode: '', FirmName: '',
        ShowOnlyOk: false,
        Details: [],
    };

    $scope.barcodeBox = '';

    $scope.onBarcodeKeyUp = function (e) {
        try {
            if (e.keyCode == '13' || $scope.barcodeBox.length >= 8) {
                $scope.processBarcodeResult($scope.barcodeBox);
            }
        } catch (e) {

        }
        
    }

    $scope.pickupList = [];
    $scope.filteredPickupList = [];
    $scope.shiftList = [];
    $scope.selectedQualities = [];

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

                    $scope.updateFilteredList();
                }
            }).catch(function (err) { });
    }

    $scope.showQualityText = function (item) {
        if (item.QualityStatus == 2) {
            if (item.QualityExplanation != null && item.QualityExplanation.length > 0) {
                bootbox.alert({
                    title: "Ürün Red Nedeni",
                    message: item.QualityExplanation,
                });
            }
            else
                bootbox.alert({
                    title: "Ürün Red Nedeni",
                    message: 'Herhangi bir red nedeni girilmemiş.',
                });
        }
    }

    $scope.changePackageQuantity = function (packObj) {
        bootbox.prompt({
            title: "Koli içi miktarı giriniz",
            centerVertical: true,
            callback: function (result) {
                if (result != null && result.length > 0) {
                    var newQty = parseInt(result);
                    if (newQty <= 0) {
                        toastr.error('Miktar 0 dan büyük olmalıdır.');
                        return;
                    }

                    $http.post(HOST_URL + 'Mobile/UpdateWorkOrderSerial', {
                        serialId: packObj.Id,
                        newQuantity: newQty,
                    }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                if (resp.data.Result) {
                                    toastr.success('İşlem başarılı.', 'Bilgilendirme');

                                    $timeout(function () {
                                        $scope.bindModel();
                                    });
                                }
                                else
                                    toastr.error(resp.data.ErrorMessage, 'Hata');
                            }
                        }).catch(function (err) { });
                }
            }
        });
    }

    $scope.getListSum = function (list, key) {
        if (list != null && list.length > 0)
            return getSumOf(list, key);

        return '';
    }

    $scope.getShiftSum = function (shiftCode, key) {
        let list = $scope.filteredSummaryList.filter(d => d.ShiftCode == shiftCode);

        if (list != null && list.length > 0)
            return getSumOf(list, key);

        return '';
    }

    // -- BEGIN -- QUALITY STATUS FILTER FUNCTIONS
    $scope.toggleQualityFilter = function (qStatus) {
        if ($scope.selectedQualities.some(d => d == qStatus)) {
            $scope.selectedQualities.splice($scope.selectedQualities.indexOf(qStatus), 1);

            if (qStatus == 3)
                $scope.selectedQualities.splice($scope.selectedQualities.indexOf(0), 1);
        }
        else {
            $scope.selectedQualities.push(qStatus);

            if (qStatus == 3)
                $scope.selectedQualities.push(0);
        }

        $scope.updateFilteredList();
    }

    $scope.isQualityFilterSelected = function (qStatus) {
        return $scope.selectedQualities.some(d => d == qStatus);
    }
    // -- END -- QUALITY STATUS FILTER FUNCTIONS

    $scope.updateFilteredList = function () {
        $scope.selectedProducts.splice(0, $scope.selectedProducts.length);

        // FILTER QUALITY STATUS
        if ($scope.selectedQualities.length > 0) {
            $scope.filteredPickupList = $scope.pickupList.filter(d => $scope.selectedQualities.some(q => q == d.QualityStatus));
        }
        else
            $scope.filteredPickupList = $scope.pickupList;

        // FILTER SHIFT
        if ($scope.selectedShift.Id > 0) {
            $scope.filteredPickupList = $scope.filteredPickupList.filter(d => d.ShiftCode == $scope.selectedShift.ShiftCode);
            $scope.filteredSummaryList = $scope.summaryList.filter(d => d.ShiftCode == $scope.selectedShift.ShiftCode);
        }
        else
            $scope.filteredSummaryList = $scope.summaryList;

        // FILTER ITEM NAME (BY SUMMARY TABLE)
        if ($scope.selectedSummary.ItemName.length > 0)
            $scope.filteredPickupList = $scope.filteredPickupList.filter(d => d.ItemName == $scope.selectedSummary.ItemName);
    }

    $scope.selectAll = function () {
        if ($scope.selectedProducts.length > 0) {
            $scope.selectedProducts.splice(0, $scope.selectedProducts.length);
        }
        else {
            $scope.selectedProducts.splice(0, $scope.selectedProducts.length);
            $scope.filteredPickupList.forEach(d => {
                if ($scope.modelObject.ShowOnlyOk == true) {
                    if (d.QualityStatus == 1)
                        $scope.selectedProducts.push(d);
                }
                else {
                    if (d.QualityStatus == 2) {
                        toastr.error('Kalite RED almış ürün OK olana kadar depo hareketi göremez.');
                        return;
                    }
                    else if (d.QualityStatus == 3) {
                        toastr.error('Kalite BEKLEMEYE almış ürün OK olana kadar depo hareketi göremez.');
                        return;
                    }

                    $scope.selectedProducts.push(d);
                }
            });
        }
    }

    $scope.selectedWarehouse = { Id: 0, WarehouseName: '' };
    $scope.warehouseList = [];

    $scope.selectProduct = function (item) {
        if (item.QualityStatus == 2) {
            toastr.error('Kalite RED almış ürün OK olana kadar depo hareketi göremez.');
            return;
        }
        else if (item.QualityStatus == 3) {
            toastr.error('Kalite BEKLEMEYE almış ürün OK olana kadar depo hareketi göremez.');
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
            //$scope.filteredPickupList = $scope.pickupList;
        }
        else {
            $scope.selectedSummary.ItemName = item.ItemName;
            //$scope.filteredPickupList = $scope.pickupList.filter(d => d.ItemName == item.ItemName);
        }

        $scope.updateFilteredList();
    }

    $scope.selectShift = function (item) {
        if ($scope.selectedShift.Id == item.Id) {
            $scope.selectedShift.Id = 0;
            $scope.selectedShift.ShiftCode = '';
            //$scope.filteredSummaryList = $scope.summaryList;
            //$scope.filteredPickupList = $scope.pickupList;
        }
        else {
            $scope.selectedShift.Id = item.Id;
            $scope.selectedShift.ShiftCode = item.ShiftCode;
            //$scope.filteredSummaryList = $scope.summaryList.filter(d => d.ShiftCode == item.ShiftCode);
            //$scope.filteredPickupList = $scope.pickupList.filter(d => d.ShiftCode == item.ShiftCode);
        }

        $scope.updateFilteredList();
    }

    $scope.processBarcodeResult = function (barcode) {
        var product = $scope.pickupList.find(d => d.SerialNo == barcode);
        if (product != null && typeof product != 'undefined') {
            if (!$scope.selectedProducts.some(d => d.SerialNo == barcode))
                toastr.success('Ürün barkodu seçildi.');
            else {
                toastr.warning('Bu ürün barkodu zaten okutulmuş.');
                $scope.isBarcodeRead = true;
                $scope.barcodeBox = '';
                return;
            }

            $scope.selectProduct(product);
            $scope.isBarcodeRead = true;
            try {
                $scope.$apply();
            } catch (e) {

            }

            $scope.barcodeBox = '';
        }
        else {
            $scope.isBarcodeRead = true;
            toastr.error('Okutulan barkoda ait bir kayıt bulunamadı.');
            $scope.barcodeBox = '';
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

                    //if ($scope.modelObject.InWarehouseId == null) {
                    //    toastr.error('Depo seçmelisiniz.');
                    //    return;
                    //}

                    //if ($scope.selectedWarehouse.WarehouseType != 2) {
                    //    toastr.error('Ürün deposu seçmelisiniz.');
                    //    return;
                    //}

                    $http.post(HOST_URL + 'Mobile/SaveProductPickup', { receiptModel: $scope.modelObject, model: $scope.selectedProducts }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Status == 1) {
                                    toastr.success('Kayıt başarılı.', 'Bilgilendirme');

                                    $scope.modelObject = {
                                        DocumentNo: '', FirmId: 0,
                                        FirmCode: '', FirmName: '',
                                        ShowOnlyOk: false,
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

    $scope.deleteEntries = function () {
        for (var i = 0; i < $scope.selectedProducts.length; i++) {
            var prd = $scope.selectedProducts[i];
            if (prd.QualityStatus != null && prd.QualityStatus > 0) {
                toastr.error('Seçilen ürünlerin kalite durumları silmek için uygun değil.');
                return;
            }
        }

        bootbox.confirm({
            message: "Seçilen ürünleri silmek istediğinizden emin misiniz?",
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

                    $http.post(HOST_URL + 'Mobile/DeleteSerials', { model: $scope.selectedProducts }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Status == 1) {
                                    toastr.success('İşlem başarılı.', 'Bilgilendirme');

                                    $scope.modelObject = {
                                        DocumentNo: '', FirmId: 0,
                                        FirmCode: '', FirmName: '',
                                        ShowOnlyOk: false,
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