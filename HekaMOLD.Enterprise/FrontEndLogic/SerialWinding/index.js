app.controller('serialWindingCtrl', function ($scope, $http) {
    $scope.modelObject = {};
    $scope.windingList = [];
    $scope.activeFaultList = [];
    $scope.windingId = 0;
    $scope.selectedSerial = { Id: 0 };

    $scope.saveStatus = 0;

    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0 };
    }

    $scope.getListSum = function (list, key) {
        if (list != null && list.length > 0)
            return getSumOf(list, key);

        return '';
    }

    $scope.selectSerial = function (item) {
        if ($scope.selectedSerial.Id == item.Id)
            $scope.selectedSerial = { Id: 0 };
        else
            $scope.selectedSerial = item;

        $scope.windingId = $scope.selectedSerial.Id;
        $scope.bindFaults();
    }

    $scope.showFaultSelection = function () {
        $scope.$broadcast('loadFaultTypeList', null);

        $('#dial-fault-types').dialog({
            width: window.innerWidth * 0.7,
            height: window.innerHeight * 0.7,
            hide: true,
            modal: true,
            resizable: false,
            show: true,
            draggable: false,
            closeText: "KAPAT"
        });
    }

    $scope.$on('faultTypeSelected', function (e, d) {
        $scope.addFault(d.Id);

        $('#dial-fault-types').dialog('close');
    });

    $scope.addFault = function (faultId) {
        if (faultId <= 0) {
            toastr.error('Bir hata tipi seçmelisiniz.');
            return;
        }

        bootbox.prompt({
            title: "Metre",
            inputType: 'number',
            value: moment().format('YYYY-MM-DD'),
            closeButton: false,
            buttons: {
                confirm: {
                    label: 'Bitir',
                    className: 'btn-primary'
                },
                cancel: {
                    label: 'Vazgeç',
                    className: 'btn-light'
                }
            },
            callback: function (result) {
                if (result) {
                    $scope.saveStatus = 1;

                    $http.post(HOST_URL + 'SerialWinding/AddFaultToWinding', {
                        windingId: $scope.windingId, faultTypeId: faultId,
                        meter: result, quantity: 0, isDotted:true,
                    }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Status == 1) {
                                    toastr.success('Hata girildi.', 'Bilgilendirme');

                                    $scope.bindFaults();
                                }
                                else
                                    toastr.error(resp.data.ErrorMessage, 'Hata');
                            }
                        }).catch(function (err) { });
                }
            }
        });
    }

    $scope.showWorkOrders = function () {
        $scope.$broadcast('loadOpenWoList', null);

        $('#dial-work-orders').dialog({
            width: window.innerWidth * 0.7,
            height: window.innerHeight * 0.7,
            hide: true,
            modal: true,
            resizable: false,
            show: true,
            draggable: false,
            closeText: "KAPAT"
        });
    }

    $scope.$on('transferWorkOrderDetails', function (e, d) {
        d.forEach(x => {
            $scope.modelObject = x;
            $scope.bindModel();
        });

        $('#dial-work-orders').dialog('close');
    });

    $scope.startWinding = function () {
        $scope.saveStatus = 1;

        $http.post(HOST_URL + 'SerialWinding/StartWinding', { workOrderDetailId: $scope.modelObject.Id }, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.saveStatus = 0;

                    if (resp.data.Status == 1) {
                        toastr.success('Sarım başlatıldı.', 'Bilgilendirme');

                        $scope.windingId = resp.data.RecordId;
                        $scope.bindModel();
                        $scope.bindFaults();
                    }
                    else
                        toastr.error(resp.data.ErrorMessage, 'Hata');
                }
            }).catch(function (err) { });
    }

    $scope.endWinding = function () {
        bootbox.prompt({
            title: "Metre",
            inputType: 'number',
            value: moment().format('YYYY-MM-DD'),
            closeButton: false,
            buttons: {
                confirm: {
                    label: 'Bitir',
                    className: 'btn-primary'
                },
                cancel: {
                    label: 'Vazgeç',
                    className: 'btn-light'
                }
            },
            callback: function (result) {
                if (result) {
                    $scope.saveStatus = 1;

                    $http.post(HOST_URL + 'SerialWinding/EndWinding', { windingId: $scope.windingId, meter: result, quantity: 0 }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Status == 1) {
                                    toastr.success('Sarım işlemi tamamlandı.', 'Bilgilendirme');

                                    $scope.windingId = 0;
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

    $scope.bindModel = function () {
        $http.get(HOST_URL + 'SerialWinding/BindModel?rid=' + $scope.modelObject.Id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.windingList = resp.data;

                    if ($scope.windingId > 0) {
                        $scope.selectedSerial = $scope.windingList.find(d => d.Id == $scope.windingId);
                    }
                }
            }).catch(function (err) { });
    }

    $scope.bindFaults = function () {
        $http.get(HOST_URL + 'SerialWinding/GetFaultsOfSerial?rid=' + $scope.selectedSerial.Id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.activeFaultList = resp.data;
                }
            }).catch(function (err) { });
    }

    $scope.removeFault = function (item) {
        bootbox.confirm({
            message: "Bu hatayı silmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'SerialWinding/DeleteFault', { rid: item.Id }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Status == 1) {
                                    toastr.success('Kayıt başarıyla silindi.', 'Bilgilendirme');

                                    $scope.bindFaults();
                                }
                                else
                                    toastr.error(resp.data.ErrorMessage, 'Hata');
                            }
                        }).catch(function (err) { });
                }
            }
        });
    }

    // ON LOAD EVENTS
    /*$scope.bindModel();*/
});