﻿app.controller('serialApprovalCtrl', function ($scope, $http) {
    $scope.modelObject = {
        Id:0,
        DocumentNo: '', FirmId: 0,
        FirmCode: '', FirmName: '',
        ShowOnlyWaitings: false,
        Details: []
    };

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
                    $scope.pickupList = resp.data.Serials.sort((a, b) => a.WorkOrderDetailId - b.WorkOrderDetailId);
                    $scope.filteredPickupList = $scope.pickupList;
                    $scope.summaryList = resp.data.Summaries;
                    $scope.filteredSummaryList = $scope.summaryList;

                    $scope.updateFilteredList();
                }
            }).catch(function (err) { });
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

    $scope.selectAll = function () {
        if ($scope.selectedProducts.length == $scope.filteredPickupList.length && $scope.selectedProducts.length > 0) {
            $scope.selectedProducts.splice(0, $scope.selectedProducts.length);
        }
        else {
            $scope.selectedProducts.splice(0, $scope.selectedProducts.length);
            $scope.filteredPickupList.forEach(d => {
                $scope.selectedProducts.push(d);
            });
        }
    }

    $scope.selectProduct = function (item) {
        if ($scope.selectedProducts.some(d => d.Id == item.Id))
            $scope.selectedProducts.splice($scope.selectedProducts.indexOf(item), 1);
        else {
            $scope.selectedProducts.push(item);
        }
    }

    $scope.isSelectedProduct = function (item) {
        return $scope.selectedProducts.some(d => d.Id == item.Id);
    }

    $scope.selectParty = function (item) {
        if ($scope.selectedProducts.some(d => d.WorkOrderDetailId == item.WorkOrderDetailId)) {
            var selectedPartyItems = $scope.selectedProducts.filter(d => d.WorkOrderDetailId == item.WorkOrderDetailId);
            selectedPartyItems.forEach(d => {
                $scope.selectedProducts.splice($scope.selectedProducts.indexOf(d), 1);
            });
        }
        else {
            var partyItems = $scope.pickupList.filter(d => d.WorkOrderDetailId == item.WorkOrderDetailId);
            partyItems.forEach(d => {
                $scope.selectedProducts.push(d);
            });
        }
    }

    $scope.isSelectedParty = function (item) {
        return $scope.selectedProducts.some(d => d.WorkOrderDetailId == item.WorkOrderDetailId);
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

    $scope.approveProduct = function () {
        bootbox.confirm({
            message: "Seçilen ürünleri ONAYLAMAK istediğinizden emin misiniz?",
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

                    $http.post(HOST_URL + 'ProductQuality/ApproveSerials',
                        { model: $scope.selectedProducts }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Status == 1) {
                                    toastr.success('İşlem başarılı.', 'Bilgilendirme');

                                    $scope.modelObject = {
                                        DocumentNo: '', FirmId: 0,
                                        FirmCode: '', FirmName: '',
                                        ShowOnlyWaitings: false,
                                        Details: []
                                    };
                                    $scope.selectedProducts.splice(0, $scope.selectedProducts.length);

                                    $scope.bindModel();
                                }
                                else
                                    toastr.error(resp.data.ErrorMessage, 'Hata');
                            }
                        }).catch(function (err) { });
                }
            },
        });
    }

    $scope.denyProduct = function () {
        bootbox.confirm({
            message: "Seçilen ürünleri REDDETMEK istediğinizden emin misiniz?",
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

                    $http.post(HOST_URL + 'ProductQuality/DenySerials',
                        { model: $scope.selectedProducts }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Status == 1) {
                                    toastr.success('İşlem başarılı.', 'Bilgilendirme');

                                    $scope.modelObject = {
                                        DocumentNo: '', FirmId: 0,
                                        FirmCode: '', FirmName: '',
                                        ShowOnlyWaitings: false,
                                        Details: []
                                    };
                                    $scope.selectedProducts.splice(0, $scope.selectedProducts.length);

                                    $scope.bindModel();
                                }
                                else
                                    toastr.error(resp.data.ErrorMessage, 'Hata');
                            }
                        }).catch(function (err) { });
                }
            },
        });
    }

    $scope.waitProduct = function () {
        bootbox.confirm({
            message: "Seçilen ürünleri BEKLEMEYE ALMAK istediğinizden emin misiniz?",
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

                    $http.post(HOST_URL + 'ProductQuality/WaitSerials',
                        { model: $scope.selectedProducts }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Status == 1) {
                                    toastr.success('İşlem başarılı.', 'Bilgilendirme');

                                    $scope.modelObject = {
                                        DocumentNo: '', FirmId: 0,
                                        FirmCode: '', FirmName: '',
                                        ShowOnlyWaitings: false,
                                        Details: []
                                    };
                                    $scope.selectedProducts.splice(0, $scope.selectedProducts.length);

                                    $scope.bindModel();
                                }
                                else
                                    toastr.error(resp.data.ErrorMessage, 'Hata');
                            }
                        }).catch(function (err) { });
                }
            },
        });
    }

    $scope.loadSelectables = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Mobile/GetSelectables?action=ItemEntry', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {

                        $scope.shiftList = resp.data.Shifts;

                        resolve();
                    }
                }).catch(function (err) { });
        });

        return prms;
    }

    // LOAD EVENTS
    $scope.loadSelectables().then(function () {
        $scope.bindModel();
    });
});