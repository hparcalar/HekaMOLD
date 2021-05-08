app.controller('userRoleCtrl', function ($scope, $http) {
    $scope.modelObject = {};

    $scope.saveStatus = 0;

    $scope.authTypeList = [];

    // DATA LISTS
    $scope.loadAuthTypeList = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'UserRole/GetAuthTypeList', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        resolve(resp.data);
                    }
                }).catch(function (err) { });
        });

        return prms;
    }

    // CRUD
    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0 };
    }

    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu stok kategorisini silmek istediğinizden emin misiniz?",
            closeButton:false,
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
                    $http.post(HOST_URL + 'UserRole/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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

        $scope.modelObject.AuthTypes = [];
        $scope.authTypeList.map(d => {
            $scope.modelObject.AuthTypes.push({ AuthTypeId: d.Id, IsGranted: d.IsGranted });
        });

        $http.post(HOST_URL + 'UserRole/SaveModel', $scope.modelObject, 'json')
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
        $scope.loadAuthTypeList().then(function (authData) {
            $http.get(HOST_URL + 'UserRole/BindModel?rid=' + id, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.modelObject = resp.data;

                        authData.forEach(m => {
                            var userAuthObj = resp.data.AuthTypes.find(d => d.AuthTypeId == m.Id);
                            if (userAuthObj != null)
                                m.IsGranted = userAuthObj.IsGranted;
                        });

                        $scope.authTypeList = authData;
                    }
                }).catch(function (err) { console.log(err); });
        });
    }

    // ON LOAD EVENTS
    if (PRM_ID > 0)
        $scope.bindModel(PRM_ID);
    else
        $scope.loadAuthTypeList();
});