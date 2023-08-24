app.controller('crmComplaintCtrl', function ($scope, $http, $timeout) {
    $scope.modelObject = { Id: 0, FormDateText: moment().format('DD.MM.YYYY'), FormStatus: 0 };

    $scope.incomingTypeList = [
        { Id: 0, Code: 'Telefon' },
        { Id: 1, Code: 'Email' },
    ];
    $scope.selectedIncomingType = $scope.incomingTypeList[0];

    $scope.firmList = [];
    $scope.selectedFirm = null;

    $scope.saveStatus = 0;

    // RECEIPT FUNCTIONS
    $scope.getNextFormNo = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'CrmComplaint/GetNextFormNo', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        if (resp.data.Result) {
                            resolve(resp.data.ReceiptNo);
                        }
                        else {
                            toastr.error('Sıradaki form numarası üretilemedi. Lütfen ekranı yenileyip tekrar deneyiniz.', 'Uyarı');
                            resolve('');
                        }
                    }
                }).catch(function (err) { });
        });

        return prms;
    }
    $scope.getToFixed = function (data, points) {
        try {
            if (typeof data != 'undefined')
                return data.toFixed(points);
        } catch (e) {

        }

        return '';
    }

    // CRUD
    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0, FormDateText: moment().format('DD.MM.YYYY'), FormStatus: 0 };

        $scope.getNextFormNo().then(function (rNo) {
            $scope.modelObject.FormNo = rNo;
            $scope.$apply();
        });
        $scope.bindDetails();
    }

    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu formu silmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'CrmComplaint/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Status == 1) {
                                    toastr.success('Kayıt başarıyla silindi.', 'Bilgilendirme');

                                    $scope.openNewRecord();
                                }
                                else
                                    toastr.error(resp.data.ErrorMessage, 'Hata');
                            }
                        }).catch(function (err) { });
                }
            }
        });
    }

    $scope.saveModel = function () {
        $scope.saveStatus = 1;

        if (typeof $scope.selectedIncomingType != 'undefined' && $scope.selectedIncomingType != null)
            $scope.modelObject.IncomingType = $scope.selectedIncomingType.Id;
        else
            $scope.modelObject.IncomingType = null;

        if (typeof $scope.selectedFirm != 'undefined' && $scope.selectedFirm != null)
            $scope.modelObject.FirmId = $scope.selectedFirm.Id;
        else
            $scope.modelObject.FirmId = null;

        $http.post(HOST_URL + 'CrmComplaint/SaveModel', $scope.modelObject, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.saveStatus = 0;

                    if (resp.data.Status == 1) {
                        toastr.success('Kayıt başarılı.', 'Bilgilendirme');

                        $scope.bindModel(resp.data.RecordId);
                    }
                    else
                        toastr.error(resp.data.ErrorMessage, 'Hata');
                }
            }).catch(function (err) { });
    }

    $scope.formatNumber = function (numberData) {
        try {
            var formatter = new Intl.NumberFormat('tr', {
                style: 'decimal',
                currency: 'TRY',
            });

            return formatter.format(numberData);
        } catch (e) {
            return 0;
        }
    }

    $scope.bindModel = function (id) {
        $http.get(HOST_URL + 'CrmComplaint/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;

                    if (typeof $scope.modelObject.IncomingType != 'undefined' && $scope.modelObject.IncomingType != null)
                        $scope.selectedIncomingType = $scope.incomingTypeList.find(d => d.Id == $scope.modelObject.IncomingType);
                    else
                        $scope.selectedIncomingType = $scope.incomingTypeList[0];

                    if (typeof $scope.modelObject.FirmId != 'undefined' && $scope.modelObject.FirmId != null)
                        $scope.selectedFirm = $scope.firmList.find(d => d.Id == $scope.modelObject.FirmId);
                    else
                        $scope.selectedFirm = null;

                    refreshArray($scope.firmList);
                }
            }).catch(function (err) {
                refreshArray($scope.firmList);
            });
    }

    $scope.createAction = function () {
        bootbox.confirm({
            message: "Bu şikayet" + ($scope.modelObject.ApproverUserId == null ? ' için faaliyet oluşturmak'
                    : 'in faaliyetini geri almak') + "istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'CrmComplaint/CreateAction', { rid: $scope.modelObject.Id }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Status == 1) {
                                    toastr.success('Başarıyla onaylandı.', 'Bilgilendirme');

                                    $scope.bindModel($scope.modelObject.Id);
                                }
                                else
                                    toastr.error(resp.data.ErrorMessage, 'Hata');
                            }
                        }).catch(function (err) { });
                }
            }
        });
    }

    $scope.closeForm = function () {
        bootbox.confirm({
            message: "Bu şikayeti " + ($scope.modelObject.FormStatus != 2 ? 'sonlandırmak' : 'geri açmak') + " istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'CrmComplaint/CloseForm', { rid: $scope.modelObject.Id }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Status == 1) {
                                    toastr.success('Başarıyla kapatıldı.', 'Bilgilendirme');

                                    $scope.bindModel($scope.modelObject.Id);
                                }
                                else
                                    toastr.error(resp.data.ErrorMessage, 'Hata');
                            }
                        }).catch(function (err) { });
                }
            }
        });
    }

    $scope.loadSelectables = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'CrmComplaint/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.firmList = resp.data.Firms;

                        resolve();
                    }
                }).catch(function (err) { });
        });

        return prms;
    }

    // ON LOAD EVENTS
    DevExpress.localization.locale('tr');
    $scope.loadSelectables().then(function () {
        if (PRM_ID > 0)
            $scope.bindModel(PRM_ID);
        else {
            $scope.getNextFormNo().then(function (rNo) {
                $scope.modelObject.FormNo = rNo;
                refreshArray($scope.firmList);

                $timeout(function () {
                    $scope.$applyAsync();
                });
            });
        }
    });
});