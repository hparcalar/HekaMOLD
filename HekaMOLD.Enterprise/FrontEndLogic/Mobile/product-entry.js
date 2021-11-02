app.controller('productEntryCtrl', function ($scope, $http) {
    $scope.modelObject = {
        Id: 0,
        MachineId: 0,
        Barcode: '',
        PrinterId: 0,
        PrintLabel: false,
    };

    $scope.activeDetails = [];

    $scope.historyWorkOrderDetailId = 0;
    $scope.selectedActiveDetailId = 0;

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
            if ($scope.historyWorkOrderDetailId > 0 || $scope.selectedActiveDetailId > 0) {
                var searchedId = $scope.historyWorkOrderDetailId > 0 ? $scope.historyWorkOrderDetailId : $scope.selectedActiveDetailId;

                $http.get(HOST_URL + 'Common/GetHistoryWorkOrderOnMachine?workOrderDetailId=' + searchedId, {}, 'json')
                    .then(function (resp) {
                        if (typeof resp.data != 'undefined' && resp.data != null) {
                            $scope.activeWorkOrder = resp.data;
                        }
                    }).catch(function (err) { });
            }
            else {
                $http.get(HOST_URL + 'Common/GetActiveWorkOrderOnMachine?machineId=' + $scope.selectedMachine.Id, {}, 'json')
                    .then(function (resp) {
                        if (typeof resp.data != 'undefined' && resp.data != null) {
                            $scope.activeWorkOrder = resp.data;
                            if ($scope.lastPackageQty > 0)
                                $scope.activeWorkOrder.WorkOrder.InPackageQuantity = $scope.lastPackageQty;
                        }
                    }).catch(function (err) { });
            }

            // CHECK IF MULTIPLE WORK ORDERS ACTIVE ON CURRENT MACHINE
            $http.get(HOST_URL + 'Mobile/GetMachineWorkList?machineId=' + $scope.selectedMachine.Id, {}, 'json')
                .then(function (respMac) {
                    if (typeof respMac.data != 'undefined' && respMac.data != null) {
                        $scope.activeDetails = respMac.data;
                    }
                });
        } catch (e) {

        }
    }

    $scope.getDefaultSerialPrinter = function () {
        try {
            $http.get(HOST_URL + 'Common/GetDefaultSerialPrinter', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.modelObject.PrinterId = resp.data;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    // PRODUCTION HISTORY
    $scope.showProductionHistory = function () {
        if ($scope.historyWorkOrderDetailId > 0) {
            $scope.historyWorkOrderDetailId = 0;
            $scope.loadActiveWorkOrder();
        }
        else {
            // DO BROADCAST
            $scope.$broadcast('loadProdHistory', $scope.selectedMachine.Id);

            $('#dial-prodlist').dialog({
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
    }

    // will be deleted
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

        if ($scope.modelObject.PrintLabel == true) {
            if ($scope.modelObject.PrinterId <= 0) {
                toastr.error('Varsayılan üretim yazıcısı ayarlanmamış.', 'Uyarı');
                return;
            }

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
        $scope.selectedActiveDetailId = 0;
        $scope.historyWorkOrderDetailId = 0;
        $scope.loadActiveWorkOrder();

        $('#dial-machinelist').dialog('close');
    });

    $scope.$on('prodSelected', function (e, d) {
        $scope.historyWorkOrderDetailId = d;
        $scope.loadActiveWorkOrder();

        $('#dial-prodlist').dialog('close');
    });

    $scope.directPrintWorkOrder = function (workOrderDetailId) {
        var workObj = $scope.activeDetails.find(d => d.Id == workOrderDetailId);

        workObj.InPackageQuantity = $scope.activeWorkOrder.WorkOrder.InPackageQuantity;

        $http.post(HOST_URL + 'Mobile/SaveProductEntry', {
            workOrderDetailId: workObj.Id,
            inPackageQuantity: workObj.InPackageQuantity,
            barcode: $scope.modelObject.Barcode,
            printLabel: $scope.modelObject.PrintLabel,
            printerId: $scope.modelObject.PrinterId,
        }, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.saveStatus = 0;

                    if (resp.data.Result == true) {
                        toastr.success('İşlem başarılı.', 'Bilgilendirme');
                        $scope.lastPackageQty = workObj.InPackageQuantity;
                        $scope.loadActiveWorkOrder();
                        $scope.modelObject.Barcode = '';
                    }
                    else
                        toastr.error(resp.data.ErrorMessage, 'Hata');
                }
            }).catch(function (err) { });
    }

    $scope.showMultipleDetails = function () {
        if ($scope.activeDetails.length > 1) {
            var buttonsHtml = '';

            for (var i = 0; i < $scope.activeDetails.length; i++) {
                var wOrder = $scope.activeDetails[i];
                buttonsHtml += '<button type="button" class="btn my-2 btn-sm btn-block btn-warning active-work" data-id="' +
                    wOrder.Id
                    + '">' + wOrder.ProductName + '</button>';
            }

            bootbox.alert({
                message: '<div class="d-flex flex-column">' + buttonsHtml + '</div>',
                closeButton: false,
                locale: 'tr',
                callback: function () {
                    bootbox.hideAll();
                }
            });

            setTimeout(() => {
                $('.active-work').on("click", function () {
                    var detailId = $(this).attr('data-id');
                    $scope.selectedActiveDetailId = parseInt(detailId);
                    $scope.loadActiveWorkOrder();
                    bootbox.hideAll();
                });
            }, 200);
        }
    }

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

                    $http.get(HOST_URL + 'Mobile/GetMachineWorkList?machineId=' + $scope.selectedMachine.Id, {}, 'json')
                        .then(function (respMac) {
                            if (typeof respMac.data != 'undefined' && respMac.data != null) {
                                if (respMac.data.length > 1) { // birden fazla ürün tek kalıptan çıkacaksa, giriş yapmak için seçtir
                                    $scope.activeDetails = respMac.data;

                                    var buttonsHtml = '';

                                    for (var i = 0; i < respMac.data.length; i++) {
                                        var wOrder = respMac.data[i];
                                        buttonsHtml += '<button type="button" class="btn my-2 btn-sm btn-block btn-warning active-work" data-id="' +
                                            wOrder.Id
                                            + '">' + wOrder.ProductName + '</button>';
                                    }

                                    bootbox.alert({
                                        message: '<div class="d-flex flex-column">' + buttonsHtml + '</div>',
                                        closeButton: false,
                                        locale: 'tr',
                                        callback: function () {
                                            bootbox.hideAll();
                                        }
                                    });

                                    setTimeout(() => {
                                        $('.active-work').on("click", function () {
                                            var detailId = $(this).attr('data-id');
                                            $scope.directPrintWorkOrder(parseInt(detailId));
                                            bootbox.hideAll();
                                        });
                                    }, 200);
                                }
                                else { // tek ürün ise doğrudan yazdır
                                    $http.post(HOST_URL + 'Mobile/SaveProductEntry', {
                                        workOrderDetailId: $scope.activeWorkOrder.WorkOrder.Id,
                                        inPackageQuantity: $scope.activeWorkOrder.WorkOrder.InPackageQuantity,
                                        barcode: $scope.modelObject.Barcode,
                                        printLabel: $scope.modelObject.PrintLabel,
                                        printerId: $scope.modelObject.PrinterId,
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
    $scope.getDefaultSerialPrinter();
    setTimeout($scope.showMachineList, 500);
});