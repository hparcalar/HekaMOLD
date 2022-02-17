app.controller('costCtrl', function ($scope, $http) {
    $scope.modelObject = { Id: 0, CostMixes: [] };

    $scope.saveStatus = 0;

    $scope.costCategoryList = [];
    $scope.forexTypeList = [];
    $scope.unitTypeList = [];


    $scope.selectedCostCategory = {};
    $scope.selectedUnitType = {};
    $scope.selectedForexType = {};


    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0 };
        $scope.selectedCostCategory = {};
        $scope.selectedUnitType = {};
        $scope.selectedForexType = {};
    }

    // GET SELECTABLE DATA
    $scope.loadSelectables = function () {
        var prmReq = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Cost/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {

                        $scope.costCategoryList = resp.data.CostCategorys;
                        $scope.unitTypeList = resp.data.UnitTypes;
                        $scope.forexTypeList = resp.data.ForexTypes;
                        resolve(resp.data);
                    }
                }).catch(function (err) { });
        });

        return prmReq;
    }

    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu maliyet tanımını silmek istediğinizden emin misiniz?",
            closeButton: false,
            buttons: {
                conyarnRecipe: {
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
                    $http.post(HOST_URL + 'Cost/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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

        if (typeof $scope.selectedForexType != 'undefined' && $scope.selectedForexType != null) {
            $scope.modelObject.ForexTypeId = $scope.selectedForexType.Id;
        }
        else
            $scope.modelObject.ForexTypeId = null;

        if (typeof $scope.selectedUnitType != 'undefined' && $scope.selectedUnitType != null) {
            $scope.modelObject.UnitTypeId = $scope.selectedUnitType.Id;
        }
        else
            $scope.modelObject.UnitTypeId = null;

        if (typeof $scope.selectedCostCategory != 'undefined' && $scope.selectedCostCategory != null) {
            $scope.modelObject.CostCategoryId = $scope.selectedCostCategory.Id;
        }
        else
            $scope.modelObject.CostCategoryId = null;

        $http.post(HOST_URL + 'Cost/SaveModel', $scope.modelObject, 'json')
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
        $http.get(HOST_URL + 'Cost/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;

                    // BIND EXTERNAL FIRM TYPE

                    if ($scope.modelObject.UnitTypeId > 0) {
                        $scope.selectedUnitType = $scope.unitTypeList.find(d => d.Id == $scope.modelObject.UnitTypeId);
                    }
                    else {
                        $scope.selectedUnitType = {};
                    }
                    if ($scope.modelObject.ForexTypeId > 0) {
                        $scope.selectedForexType = $scope.forexTypeList.find(d => d.Id == $scope.modelObject.ForexTypeId);
                    }
                    else {
                        $scope.selectedForexType = {};
                    }
                    if ($scope.modelObject.CostCategoryId > 0) {
                        $scope.selectedCostCategory = $scope.costCategoryList.find(d => d.Id == $scope.modelObject.CostCategoryId);
                    }
                    else {
                        $scope.selectedCostCategory = {};
                    }
               
                }
            }).catch(function (err) { });
    }

    // ON LOAD EVENTS
    DevExpress.localization.locale('tr');
    $scope.loadSelectables().then(function (data) {
        if (PRM_ID > 0)
            $scope.bindModel(PRM_ID);
        else
            $scope.bindModel(0);
    });
});