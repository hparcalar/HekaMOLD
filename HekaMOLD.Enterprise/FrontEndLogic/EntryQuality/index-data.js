app.controller('entryQualityPlanFormCtrl', function ($scope, $http) {
    $scope.modelObject = { Id: 0, CreatedDateStr: moment().format('DD.MM.YYYY'), Details: [] };
    $scope.hourList = [];
    $scope.planList = [];

    $scope.firmList = [];
    $scope.productList = [];
    $scope.selectedFirm = {};
    $scope.selectedProduct = {};
    $scope.lastProductId = 0;

    $scope.saveStatus = 0;

    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0, CreatedDateStr: moment().format('DD.MM.YYYY'), Details: [] };
    }

    $scope.onItemChanged = function (e) {
        $scope.lastProductId = $scope.selectedProduct.Id;
        $scope.bindModel(0);
    }

    $scope.loadSelectables = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'EntryQuality/GetSelectablesOfData', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.productList = resp.data.Items;
                        $scope.firmList = resp.data.Firms;

                        var emptyFirmObj = { Id: 0, MachineCode: '-- Seçiniz --' };
                        $scope.firmList.splice(0, 0, emptyFirmObj);
                        $scope.selectedFirm = emptyFirmObj;

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
            message: "Bu giriş kalite formunu silmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'EntryQuality/DeletePlanFormModel', { rid: $scope.modelObject.Id }, 'json')
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

    $scope.getQualityValue = function (planId, hourNo) {
        try {
            var qData = $scope.modelObject.Details.find(d => d.EntryQualityPlanDetailId == planId &&
                d.OrderNo == hourNo);

            if (typeof qData != 'undefined' && qData != null) {
                return qData.SampleQuantity;
            }
        } catch (e) {

        }

        return null;
    }

    $scope.getFaultValue = function (planId, hourNo) {
        try {
            var qData = $scope.modelObject.Details.find(d => d.EntryQualityPlanDetailId == planId &&
                d.OrderNo == hourNo);

            if (typeof qData != 'undefined' && qData != null) {
                return qData.FaultExplanation;
            }
        } catch (e) {

        }

        return null;
    }

    $scope.saveModel = function () {
        $scope.saveStatus = 1;

        // VALIDATE SAMPLE QUANTITY DATA
        $.each($('.plan-numeric'), function (ix, elm) {
            var planId = parseInt($(elm).attr('data-plan-id'));
            var hourData = parseInt($(elm).attr('data-hour'));

            var faultExp = '';
            if (hourData == 1) {
                faultExp = $('input.fault-exp[data-plan-id="' + planId + '"]').val();
            }

            if (hourData > 0) {
                var existingData = $scope.modelObject.Details.find(d => d.EntryQualityPlanDetailId == planId &&
                    d.OrderNo == hourData);
                if (typeof existingData != 'undefined' && existingData != null) {
                    existingData.SampleQuantity = parseFloat($(elm).val());
                    existingData.FaultExplanation = faultExp;
                }
                else {
                    var newData = {
                        EntryQualityPlanDetailId: planId,
                        SampleQuantity: parseFloat($(elm).val()),
                        FaultExplanation: faultExp,
                        OrderNo: hourData,
                        NewDetail: true,
                    };
                    $scope.modelObject.Details.push(newData);
                }
            }
        });

        if (typeof $scope.selectedFirm != 'undefined' && $scope.selectedFirm != null)
            $scope.modelObject.FirmId = $scope.selectedFirm.Id;
        else
            $scope.modelObject.FirmId = null;

        if (typeof $scope.selectedProduct != 'undefined' && $scope.selectedProduct != null)
            $scope.modelObject.ItemId = $scope.selectedProduct.Id;
        else
            $scope.modelObject.ItemId = null;

        $http.post(HOST_URL + 'EntryQuality/SavePlanFormModel', $scope.modelObject, 'json')
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
        $http.get(HOST_URL + 'EntryQuality/BindPlanFormModel?rid=' + id + '&sid=' + $scope.selectedProduct.Id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data.Model;

                    if ($scope.lastProductId > 0) {
                        $scope.modelObject.ItemId = $scope.lastProductId;
                        $scope.lastProductId = 0;
                    }

                    if ($scope.modelObject.Id <= 0) {
                        $scope.modelObject.CreatedDateStr = moment().format('DD.MM.YYYY');
                        $scope.modelObject.Details = [];
                    }
                    else {
                        if ($scope.modelObject.CreatedDateStr == null)
                            $scope.modelObject.CreatedDateStr = moment().format('DD.MM.YYYY');
                    }

                    if (typeof $scope.modelObject.FirmId != 'undefined' && $scope.modelObject.FirmId != null)
                        $scope.selectedFirm = $scope.firmList.find(d => d.Id == $scope.modelObject.FirmId);
                    else
                        $scope.selectedFirm = $scope.firmList[0];

                    if (typeof $scope.modelObject.ItemId != 'undefined' && $scope.modelObject.ItemId != null)
                        $scope.selectedProduct = $scope.productList.find(d => d.Id == $scope.modelObject.ItemId);
                    else
                        $scope.selectedProduct = $scope.productList[0];

                    refreshArray($scope.firmList);
                    refreshArray($scope.productList);

                    $scope.planList = resp.data.Plans;
                }

                $scope.hourList.splice(0, $scope.hourList.length);

                for (var i = 0; i < 14; i++) {
                    $scope.hourList.push({
                        HourText:
                            (i + 1).toString(),
                        HourNo: i + 1
                    });
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