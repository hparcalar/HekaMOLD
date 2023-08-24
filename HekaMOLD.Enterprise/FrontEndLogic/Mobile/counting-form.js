app.controller('countingFormCtrl', function ($scope, $http) {
    $scope.modelObject = {
        Id: 0,
        DocumentNo: '', FirmId: 0,
        FirmCode: '', FirmName: '',
        ReadCount: 1, DecreaseCount:0,
        ShowOnlyOk: false,
        Details: []
    };

    $scope.staticWarehouseId = 4;

    $scope.detailList = [];
    $scope.serialList = [];

    $scope.bindModel = function () {
        $http.get(HOST_URL + 'Mobile/GetActiveCountingData?warehouseId=' + $scope.staticWarehouseId, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.detailList = resp.data.Details;
                    $scope.serialList = resp.data.Serials;
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

    $scope.processBarcodeResult = function (barcode) {
        $scope.isBarcodeRead = true;
        
        $http.post(HOST_URL + 'Mobile/AddCountingBarcode', {
            barcode: barcode,
            warehouseId: $scope.staticWarehouseId,
            readCount: $scope.modelObject.ReadCount,
            decreaseCount: $scope.modelObject.DecreaseCount,
        }, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.saveStatus = 0;

                    if (resp.data.Result == true) {
                        toastr.success('Barkod kaydedildi.', 'Bilgilendirme');

                        $scope.bindModel();

                        // RESET PARAMETERS
                        $scope.modelObject.ReadCount = 1;
                        $scope.modelObject.DecreaseCount = 0;
                    }
                    else
                        toastr.error(resp.data.ErrorMessage, 'Hata');
                }
            }).catch(function (err) { });
    }

    $scope.removeSerial = function (serialId) {
        $http.post(HOST_URL + 'Mobile/DeleteCountingBarcode', { serialId: serialId }, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.saveStatus = 0;

                    if (resp.data.Result == true) {
                        toastr.success('Barkod silindi.', 'Bilgilendirme');

                        $scope.bindModel();
                    }
                    else
                        toastr.error(resp.data.ErrorMessage, 'Hata');
                }
            }).catch(function (err) { });
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
                console.log(qrCodeMessage);
                if (!$scope.isBarcodeRead) {
                    $scope.processBarcodeResult(qrCodeMessage);
                    setTimeout(() => { $scope.isBarcodeRead = false; }, 3000);
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

                        resolve();
                    }
                }).catch(function (err) { });
        });

        return prms;
    }

    // LOAD EVENTS
    $scope.loadSelectables().then(function () {
        refreshArray($scope.warehouseList);
        $scope.bindModel();
    });
});