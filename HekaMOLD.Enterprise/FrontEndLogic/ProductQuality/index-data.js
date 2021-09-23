﻿app.controller('productQualityPlanFormCtrl', function ($scope, $http) {
    $scope.modelObject = { Id: 0, ControlDate: moment().format('DD.MM.YYYY'), Details: [] };
    $scope.hourList = [];
    $scope.planList = [];

    $scope.machineList = [];
    $scope.productList = [];
    $scope.selectedMachine = {};
    $scope.selectedProduct = {};

    $scope.saveStatus = 0;

    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0, ControlDate: moment().format('DD.MM.YYYY'), Details: [] };
    }

    $scope.loadSelectables = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'ProductQuality/GetSelectablesOfData', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.productList = resp.data.Items;
                        $scope.machineList = resp.data.Machines;

                        var emptyMachineObj = { Id: 0, MachineCode: '-- Seçiniz --' };
                        $scope.machineList.splice(0, 0, emptyMachineObj);
                        $scope.selectedMachine = emptyMachineObj;

                        var emptyProductObj = { Id: 0, ItemName: '-- Seçiniz --' };
                        $scope.productList.splice(0, 0, emptyProductObj);
                        $scope.selectedProduct = emptyProductObj;

                        resolve();
                    }
                }).catch(function (err) { });
        });

        return prms;
    }

    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu proses kalite formunu silmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'ProductQuality/DeletePlanFormModel', { rid: $scope.modelObject.Id }, 'json')
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

    $scope.getQualityValue = function (planId, hourNo, checkType) {
        try {
            var qData = $scope.modelObject.Details.find(d => d.ProductQualityPlanId == planId &&
                d.OrderNo == hourNo);

            if (typeof qData != 'undefined' && qData != null) {
                return checkType == 1 ?
                    qData.CheckResult : qData.NumericResult;
            }
        } catch (e) {

        }

        return checkType == 1 ? false : 0;
    }

    $scope.saveModel = function () {
        $scope.saveStatus = 1;

        // VALIDATE CHECK TYPE = 1 DATA
        $.each($('.plan-check'), function (ix, elm) {
            if ($(elm).attr('data-active') == 'true') {
                var planId = parseInt($(elm).attr('data-plan-id'));
                var hourData = parseInt($(elm).attr('data-hour'));

                var existingData = $scope.modelObject.Details.find(d => d.ProductQualityPlanId == planId &&
                    d.OrderNo == hourData);
                if (typeof existingData != 'undefined' && existingData != null) {
                    existingData.CheckResult = $(elm).is(':checked');
                    existingData.IsOk = existingData.CheckResult;
                }
                else {
                    var newData = {
                        ProductQualityPlanId: planId,
                        CheckResult: $(elm).is(':checked'),
                        IsOk: $(elm).is(':checked'),
                        OrderNo: hourData,
                        NewDetail: true,
                    };
                    $scope.modelObject.Details.push(newData);
                }
            }
        });

        // VALIDATE CHECK TYPE = 2 DATA
        $.each($('.plan-numeric'), function (ix, elm) {
            if ($(elm).attr('data-active') == 'true') {
                var planId = parseInt($(elm).attr('data-plan-id'));
                var hourData = parseInt($(elm).attr('data-hour'));

                var existingData = $scope.modelObject.Details.find(d => d.ProductQualityPlanId == planId &&
                    d.OrderNo == hourData);
                if (typeof existingData != 'undefined' && existingData != null) {
                    existingData.NumericResult = parseFloat($(elm).val());
                    existingData.IsOk = true;
                }
                else {
                    var newData = {
                        ProductQualityPlanId: planId,
                        NumericResult: parseFloat($(elm).val()),
                        IsOk: true,
                        OrderNo: hourData,
                        NewDetail: true,
                    };
                    $scope.modelObject.Details.push(newData);
                }
            }
        });

        if (typeof $scope.selectedMachine != 'undefined' && $scope.selectedMachine != null)
            $scope.modelObject.MachineId = $scope.selectedMachine.Id;
        else
            $scope.modelObject.MachineId = null;

        if (typeof $scope.selectedProduct != 'undefined' && $scope.selectedProduct != null)
            $scope.modelObject.ProductId = $scope.selectedProduct.Id;
        else
            $scope.modelObject.ProductId = null;

        $http.post(HOST_URL + 'ProductQuality/SavePlanFormModel', $scope.modelObject, 'json')
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
        $http.get(HOST_URL + 'ProductQuality/BindPlanFormModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data.Model;

                    if ($scope.modelObject.Id <= 0) {
                        $scope.modelObject.ControlDateStr = moment().format('DD.MM.YYYY');
                        $scope.modelObject.Details = [];
                    }
                    else {
                        if ($scope.modelObject.ControlDateStr == null)
                            $scope.modelObject.ControlDateStr = moment().format('DD.MM.YYYY');

                        if (typeof $scope.modelObject.MachineId != 'undefined' && $scope.modelObject.MachineId != null)
                            $scope.selectedMachine = $scope.machineList.find(d => d.Id == $scope.modelObject.MachineId);
                        else
                            $scope.selectedMachine = $scope.machineList[0];

                        if (typeof $scope.modelObject.ProductId != 'undefined' && $scope.modelObject.ProductId != null)
                            $scope.selectedProduct = $scope.productList.find(d => d.Id == $scope.modelObject.ProductId);
                        else
                            $scope.selectedProduct = $scope.productList[0];

                        refreshArray($scope.machineList);
                        refreshArray($scope.productList);
                    }

                    $scope.planList = resp.data.Plans;
                }

                $scope.hourList.splice(0, $scope.hourList.length);

                var startHour = 7;
                for (var i = 0; i < 25; i++) {
                    $scope.hourList.push({
                        HourText:
                            (startHour >= 10 ? startHour.toString() : ('0' + startHour.toString()))
                            + ':00',
                        HourNo: startHour
                    });

                    if (startHour == 23)
                        startHour = 0;
                    else
                        startHour++;
                }
            }).catch(function (err) { });
    }

    // ON LOAD EVENTS
    $scope.loadSelectables().then(function () {
        if (PRM_ID > 0)
            $scope.bindModel(PRM_ID);
        else
            $scope.bindModel(0);
    });
});