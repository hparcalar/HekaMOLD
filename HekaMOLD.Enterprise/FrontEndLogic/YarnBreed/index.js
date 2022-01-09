﻿app.controller('yarnBreedCtrl', function ($scope, $http) {
    $scope.modelObject = {};

    $scope.saveStatus = 0;

    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0 };
        $scope.getNextOrderNo().then(function (rNo) {
            $scope.modelObject.YarnBreedCode = rNo;
            $scope.$apply();
        });
    }
    // RECEIPT FUNCTIONS
    $scope.getNextOrderNo = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'YarnBreed/GetNextYarnBreedCode', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        if (resp.data.Result) {
                            resolve(resp.data.ReceiptNo);
                        }
                        else {
                            toastr.error('Sıradaki cins numarası üretilemedi. Lütfen ekranı yenileyip tekrar deneyiniz.', 'Uyarı');
                            resolve('');
                        }
                    }
                }).catch(function (err) { });
        });

        return prms;
    }
    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu iplik cinsini silmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'YarnBreed/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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

        $http.post(HOST_URL + 'YarnBreed/SaveModel', $scope.modelObject, 'json')
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

    $scope.bindModel = function (id) {
        $http.get(HOST_URL + 'YarnBreed/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;
                }
            }).catch(function (err) { });
    }

    // ON LOAD EVENTS
    if (PRM_ID > 0)
        $scope.bindModel(PRM_ID);
    else {
        $scope.getNextOrderNo().then(function (rNo) {
            $scope.modelObject.YarnBreedCode = rNo;
            $scope.$apply();
        });
    //    $scope.bindModel(0);
    }
});