app.controller('managePalletsCtrl', function ($scope, $http) {
    $scope.modelObject = {
        Id: 0,
    };

    $scope.boxList = [];
    $scope.palletList = [];
    $scope.selectedProducts = [];

    $scope.lastRecordId = 0;
    $scope.reportTemplateId = 0;

    $scope.selectedPallet = { Id:0 };

    $scope.bindModel = function () {
        $http.get(HOST_URL + 'Mobile/GetBoxesWithoutPallet', {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.boxList = resp.data.Boxes;
                    $scope.palletList = resp.data.Pallets;
                }
            }).catch(function (err) { });
    }

    $scope.getListSum = function (list, key) {
        if (list != null && list.length > 0)
            return getSumOf(list, key);

        return '';
    }

    $scope.selectPallet = function (item) {
        $scope.selectedPallet = item;
    }

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
            $scope.addToPallet(product);
            /*$scope.selectProduct(product);*/
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

    // EXTERNAL COMPONENT CALLING
    $scope.showPrintTemplates = function () {
        if ($scope.lastRecordId > 0) {
            // DO BROADCAST
            $scope.$broadcast('loadTemplateList', [7]);

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

    $scope.showPalletView = function (palletItem) {
        // DO BROADCAST
        $scope.$broadcast('loadPalletView', palletItem.Id);

        $('#dial-pallet-view').dialog({
            width: window.innerWidth * 0.65,
            height: window.innerHeight * 0.65,
            hide: true,
            modal: true,
            resizable: false,
            show: true,
            draggable: false,
            close: function (event, ui) {
                $scope.bindModel();
            },
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
                recordType: 7, // pallet label type
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

    $scope.addToPallet = function (boxItem) {
        if ($scope.selectedPallet.Id <= 0) {
            toastr.error("Önce bir palet seçmelisiniz.");
            return;
        }

        $scope.saveStatus = 1;

        $http.post(HOST_URL + 'Mobile/AddToPallet', { palletId: $scope.selectedPallet.Id, itemSerialId: boxItem.Id }, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.saveStatus = 0;

                    if (resp.data.Status == 1) {
                        toastr.success('Palete başarıyla eklendi.', 'Bilgilendirme');

                        $scope.bindModel();
                    }
                    else
                        toastr.error(resp.data.ErrorMessage, 'Hata');
                }
            }).catch(function (err) { });
    }

    $scope.createPallet = function () {
        bootbox.confirm({
            message: "Yeni palet oluşturulacaktır. Onaylıyor musunuz?",
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

                    $http.post(HOST_URL + 'Mobile/CreatePallet', { }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Status == 1) {
                                    toastr.success('Kayıt başarılı.', 'Bilgilendirme');

                                    $scope.lastRecordId = resp.data.RecordId;
                                    /*$scope.showPrintTemplates();*/

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

    $scope.printPallet = function (pallet) {
        $scope.lastRecordId = pallet.Id;
        $scope.showPrintTemplates();
    }

    $scope.deletePallet = function (pallet) {
        bootbox.confirm({
            message: "Bu paleti silmek istediğinizden emin misiniz?",
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

                    $http.post(HOST_URL + 'Mobile/DeletePallet', { palletId: pallet.Id }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Status == 1) {
                                    toastr.success('Kayıt başarılı.', 'Bilgilendirme');

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
    $scope.bindModel();
});