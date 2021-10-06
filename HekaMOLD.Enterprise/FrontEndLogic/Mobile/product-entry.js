﻿app.controller('productEntryCtrl', function ($scope, $http) {
    $scope.modelObject = {
        Id: 0,
        MachineId: 0,
        Barcode:'',
    };

    $scope.bindModel = function (id) {
        $scope.modelObject = {
            Id: 0,
            MachineId: 0,
            Barcode:'',
        };
    }

    $scope.lastPackageQty = 0;

    $scope.activeWorkOrder = { Id:0 };
    $scope.selectedMachine = { Id: 0, MachineName: '' };

    $scope.getHourPart = function (text) {
        try {
            return text.substr(11, 5);
        } catch (e) {

        }

        return text;
    }

    $scope.getDatePart = function (text) {
        try {
            return text.substr(0, 10);
        } catch (e) {

        }

        return text;
    }

    $scope.showMachineList = function () {
        // DO BROADCAST
        $scope.$broadcast('loadMachineList');

        $('#dial-machinelist').dialog({
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

    $scope.loadActiveWorkOrder = function () {
        try {
            $http.get(HOST_URL + 'Common/GetActiveWorkOrderOnMachine?machineId=' + $scope.selectedMachine.Id, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.activeWorkOrder = resp.data;
                        if ($scope.lastPackageQty > 0)
                            $scope.activeWorkOrder.WorkOrder.InPackageQuantity = $scope.lastPackageQty;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.printSerial = function (item) {
        try {
            $http.post(HOST_URL + 'Common/PrintSerial', { id: item.Id }, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.isBarcodeRead = false;

    $scope.readBarcode = function () {
        if ($scope.modelObject.Barcode != null && $scope.modelObject.Barcode.length > 0) {
            $scope.approveProductEntry();
            return;
        }

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
                if (!$scope.isBarcodeRead) {
                    $scope.modelObject.Barcode = qrCodeMessage;
                    $scope.approveProductEntry();
                }
                //qrScanner.stop();
                Html5Qrcode.stop();
            },
            errorMessage => {
            })
            .catch(err => {
            });
    }

    // EMIT SELECTED MACHINE DATA
    $scope.$on('machineSelected', function (e, d) {
        $scope.selectedMachine = d;
        $scope.loadActiveWorkOrder();

        $('#dial-machinelist').dialog('close');
    });

    $scope.approveProductEntry = function () {
        $scope.isBarcodeRead = true;

        bootbox.confirm({
            message: "Bu ürün girişini, KOLİ İÇİ ADEDİ: " + $scope.activeWorkOrder.WorkOrder.InPackageQuantity
                    + " ADET olarak onaylıyor musunuz?",
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

                    $http.post(HOST_URL + 'Mobile/SaveProductEntry', {
                        workOrderDetailId: $scope.activeWorkOrder.WorkOrder.Id,
                        inPackageQuantity: $scope.activeWorkOrder.WorkOrder.InPackageQuantity,
                        barcode: $scope.modelObject.Barcode,
                    }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Result == true) {
                                    toastr.success('İşlem başarılı.', 'Bilgilendirme');
                                    $scope.lastPackageQty = $scope.activeWorkOrder.WorkOrder.InPackageQuantity;
                                    $scope.loadActiveWorkOrder();
                                    $scope.modelObject.Barcode = '';
                                }
                                else
                                    toastr.error(resp.data.ErrorMessage, 'Hata');
                            }
                        }).catch(function (err) { });
                }
            }
        });
    }

    $scope.deleteSerial = function (item) {
        bootbox.confirm({
            message: "Bu ürün girişini geri almak istediğinizden emin misiniz?",
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

                    $http.post(HOST_URL + 'Mobile/DeleteProductEntry', {
                        id: item.Id
                    }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Result == true) {
                                    toastr.success('Ürün girişi geri alındı.', 'Bilgilendirme');
                                    $scope.lastPackageQty = $scope.activeWorkOrder.WorkOrder.InPackageQuantity;
                                    $scope.loadActiveWorkOrder();
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
    setTimeout($scope.showMachineList, 500);
});