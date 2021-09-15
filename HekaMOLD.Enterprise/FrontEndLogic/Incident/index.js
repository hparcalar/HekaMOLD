app.controller('incidentCtrl', function ($scope, $http) {
    $scope.modelObject = {};
    $scope.categoryList = [];
    $scope.selectedCategory = {};

    $scope.saveStatus = 0;

    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0 };
    }

    // GET SELECTABLE DATA
    $scope.loadSelectables = function () {
        var prmReq = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Maintenance/GetIncidentCategoryList', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.categoryList = resp.data;
                        resolve(resp.data);
                    }
                }).catch(function (err) { });
        });

        return prmReq;
    }

    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu arıza kaydını silmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'Maintenance/DeleteIncident', { rid: $scope.modelObject.Id }, 'json')
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

        if (typeof $scope.selectedCategory != 'undefined' && $scope.selectedCategory != null)
            $scope.modelObject.IncidentCategoryId = $scope.selectedCategory.Id;
        else
            $scope.modelObject.IncidentCategoryId = null;

        $http.post(HOST_URL + 'Maintenance/SaveIncident', $scope.modelObject, 'json')
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
        $http.get(HOST_URL + 'Maintenance/BindIncident?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;

                    if ($scope.modelObject.IncidentCategoryId > 0)
                        $scope.selectedCategory = $scope.categoryList.find(d => d.Id == $scope.modelObject.IncidentCategoryId);
                    else
                        $scope.selectedCategory = {};
                }
            }).catch(function (err) { });
    }

    // ON LOAD EVENTS
    $scope.loadSelectables().then(function (data) {
        if (PRM_ID > 0)
            $scope.bindModel(PRM_ID);
        else
            $scope.bindModel(0);
    });
});