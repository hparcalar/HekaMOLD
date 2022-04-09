app.controller('productEntryCtrl', function ($scope, $http, $timeout) {
    $scope.modelObject = {
        Id: 0,
        MachineId: 0,
        Barcode: '',
        PrinterId: 0,
        IsProdChief: false,
        PrintLabel: true,
    };

    $scope.activeDetails = [];
    $scope.labelCount = 1;
    $scope.historyWorkOrderDetailId = 0;
    $scope.selectedActiveDetailId = 0;
    $scope.selectedProductPart = { Id: 0 };

    $scope.bindModel = function (id) {
        $scope.modelObject = {
            Id: 0,
            MachineId: 0,
            Barcode: '',
            PrintLabel: true,
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
        try {
            $http.get(HOST_URL + 'Common/GetMachineList', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        var macList = resp.data;
                        if (macList.length == 1) {
                            $scope.selectedMachine = macList[0];
                            $scope.selectedActiveDetailId = 0;
                            $scope.historyWorkOrderDetailId = 0;
                            $scope.loadActiveWorkOrder();
                        }
                        else {
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
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.loadMoldTest = function () {
        try {
            $http.get(HOST_URL + 'Common/GetMoldTestOfProduct?productCode=' + $scope.activeWorkOrder.WorkOrder.ProductCode, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.modelObject.MoldData = resp.data;
                        if ($scope.modelObject.MoldData != null && $scope.modelObject.MoldData.InPackageQuantity > 0)
                            $scope.activeWorkOrder.WorkOrder.InPackageQuantity = $scope.modelObject.MoldData.InPackageQuantity;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.loadActiveWorkOrder = function () {
        try {
            if ($scope.historyWorkOrderDetailId > 0 || $scope.selectedActiveDetailId > 0) {
                var searchedId = $scope.historyWorkOrderDetailId > 0 ? $scope.historyWorkOrderDetailId : $scope.selectedActiveDetailId;

                $http.get(HOST_URL + 'Common/GetHistoryWorkOrderOnMachine?workOrderDetailId=' + searchedId, {}, 'json')
                    .then(function (resp) {
                        if (typeof resp.data != 'undefined' && resp.data != null) {
                            console.log(resp.data);
                            $scope.activeWorkOrder = resp.data;
                            //$scope.loadMoldTest();

                            $scope.buildSummary();
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

                            $scope.buildSummary();
                            //$scope.loadMoldTest();
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

    $scope.buildSummary = function () {
        try {
            $timeout(function () {
                for (var i = 0; i < $scope.activeWorkOrder.SheetUsages.length; i++) {
                    var partData = $scope.activeWorkOrder.SheetUsages[i];
                    var serialsData = $scope.activeWorkOrder.Serials
                        .filter(d => d.ItemId == partData.ItemId);
                    if (serialsData && serialsData != null && serialsData.length > 0) {
                        var partTotal = serialsData.map(d => d.FirstQuantity).reduce((p, n) => p + n);
                        partData['TotalQuantity'] = partTotal;
                    }
                    else
                        partData['TotalQuantity'] = 0;
                }
            });

            $scope.$applyAsync();
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
                        if (resp.data.Status == 1)
                            toastr.success('Yazdırma kuyruğuna eklendi.');
                        else
                            toastr.error(resp.data.ErrorMessage ? resp.data.ErrorMessage : 'Bir hata oluştu.');
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

    $scope.directPrintWorkOrder = async function (itemOrderDetailId) {
        try {
            $http.post(HOST_URL + 'Mobile/SaveProductEntry', {
                itemOrderDetailId: itemOrderDetailId,
                workOrderDetailId: $scope.activeWorkOrder.WorkOrderDetailId,
                inPackageQuantity: $scope.activeWorkOrder.WorkOrder.InPackageQuantity,
                barcode: '',
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
        } catch (e) {

        }
    }

    $scope.showMultipleDetails = function () {
        // DO BROADCAST
        $scope.$broadcast('loadParts', $scope.activeWorkOrder.WorkOrderDetailId);

        $('#dial-part-variants').dialog({
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

    $scope.$on('productPartSelected', function (e, d) {
        $scope.selectedProductPart = d;
        $scope.directPrintWorkOrder(parseInt($scope.selectedProductPart.ItemOrderDetailId));
        $('#dial-part-variants').dialog('close');
    });

    $scope.approveProductEntry = async function () {
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
            callback: async function (result) {
                if (result) {
                    $scope.saveStatus = 1;

                    $scope.showMultipleDetails();
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

    if (IS_PRODCHIEF)
        setTimeout($scope.showMachineList, 500);
    else {
        $scope.selectedMachine.Id = MACHINE_ID;
        $scope.selectedMachine.MachineName = MACHINE_NAME;
        $scope.modelObject.IsProdChief = IS_PRODCHIEF;

        $scope.loadActiveWorkOrder();
    }
});