app.controller('yarnColourCtrl', function ($scope, $http) {
    $scope.modelObject = {};

    $scope.saveStatus = 0;

    $scope.selectedGroup = {};
    $scope.groupList = [];

    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0 };
    }

    // GET SELECTABLE DATA
    $scope.loadSelectables = function () {
        var prmReq = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'YarnColour/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {

                        $scope.groupList = resp.data.ColourGroups;

                        resolve(resp.data);
                    }
                }).catch(function (err) { });
        });

        return prmReq;
    }

    $scope.performDelete = function () {
        bootbox.conyarnRecipe({
            message: "Bu iplik tanımını silmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'YarnColour/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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

        if (typeof $scope.selectedGroup != 'undefined' && $scope.selectedGroup != null) {
            $scope.modelObject.YarnColourGroupId = $scope.selectedGroup.Id;
            alert("asd:" + $scope.modelObject.YarnColourGroupId);
        }
        else
            $scope.modelObject.YarnColourGroupId = null;

        $http.post(HOST_URL + 'YarnColour/SaveModel', $scope.modelObject, 'json')
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
    // YARCOLOUR FUNCTIONS
    $scope.updateCode = function () {

        //$scope.getNextReceiptNo().then(function (rNo) {
        //    $scope.modelObject.YarnColourCode = rNo;
        //});
        $scope.modelObject.YarnColourCode = $scope.selectedGroup.YarnColourGroupCode;
        alert("asd:" + $scope.selectedGroup.YarnColourGroupCode);

    };
    $scope.getNextYarnColourNo = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'YarnColour/GetNextYarnColourNo', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        if (resp.data.Result) {
                            resolve(resp.data.ReceiptNo);
                        }
                        else {
                            toastr.error('Sıradaki sipariş numarası üretilemedi. Lütfen ekranı yenileyip tekrar deneyiniz.', 'Uyarı');
                            resolve('');
                        }
                    }
                }).catch(function (err) { });
        });

        return prms;
    }

    $scope.bindModel = function (id) {
        $http.get(HOST_URL + 'YarnColour/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;

                    // BIND EXTERNAL 

                    if ($scope.modelObject.YarnColourGroupId > 0) {
                        $scope.selectedGroup = $scope.groupList.find(d => d.Id == $scope.modelObject.YarnColourGroupId);
                    }
                    else {
                        $scope.selectedGroup = {};
                    }

                    $scope.bindAuthorList();
                }
            }).catch(function (err) { });
    }

    // ON LOAD EVENTS
    DevExpress.localization.locale('tr');
    $scope.loadSelectables().then(function (data) {
        if (PRM_ID > 0)
            $scope.bindModel(PRM_ID);
        else {
            $scope.getNextReceiptNo().then(function (rNo) {
                $scope.modelObject.YarnColourCode = rNo;
                $scope.bindModel(0);
            });
        }
    });
});