app.controller('userRoleCtrl', function ($scope, $http) {
    $scope.modelObject = {};

    $scope.saveStatus = 0;

    $scope.authTypeList = [];
    $scope.subscriptionList = [];

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

    $scope.loadSelectables = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'UserRole/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.subscriptionList = resp.data;
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

        // UPDATE MODELS AUTH LIST
        $scope.modelObject.AuthTypes = [];
        $scope.authTypeList.map(d => {
            $scope.modelObject.AuthTypes.push({ AuthTypeId: d.Id, IsGranted: d.IsGranted });
        });

        // UPDATE MODELS SUBSCRIPTION LIST
        $scope.subscriptionList.forEach(d => {
            try {
                if (d.IsChecked == true) {
                    if (!$scope.modelObject.Subscriptions.some(m => m.SubscriptionCategory == d.Id))
                        $scope.modelObject.Subscriptions.push({ SubscriptionCategory: d.Id });
                }
                else {
                    if ($scope.modelObject.Subscriptions.some(m => m.SubscriptionCategory == d.Id)) {
                        var existingObj = $scope.modelObject.Subscriptions.find(m => m.SubscriptionCategory == d.Id);
                        $scope.modelObject.Subscriptions.splice($scope.modelObject.Subscriptions.indexOf(existingObj), 1);
                    }
                }
            } catch (e) {

            }
            
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

                        // UPDATE AUTH CHECK LIST
                        authData.forEach(m => {
                            var userAuthObj = resp.data.AuthTypes.find(d => d.AuthTypeId == m.Id);
                            if (userAuthObj != null)
                                m.IsGranted = userAuthObj.IsGranted;
                        });
                        $scope.authTypeList = authData;

                        // UPDATE SUBSCRIPTION CATEGORY CHECK LIST
                        $scope.subscriptionList.forEach(d => {
                            if ($scope.modelObject.Subscriptions.some(m => m.SubscriptionCategory == d.Id))
                                d.IsChecked = true;
                            else
                                d.IsChecked = false;
                        });
                    }
                }).catch(function (err) { console.log(err); });
        });
    }

    // ON LOAD EVENTS
    $scope.loadSelectables().then(() => {
        if (PRM_ID > 0)
            $scope.bindModel(PRM_ID);
        else
            $scope.loadAuthTypeList();
    });
});