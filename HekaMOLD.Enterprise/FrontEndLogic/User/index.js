var app = angular.module('moldApp', []);
app.controller('userCtrl', function ($scope, $http) {
    $scope.modelObject = {};

    $scope.saveStatus = 0;

    $scope.selectedRole = {};
    $scope.roleList = [];

    // GET SELECTABLE DATA
    $scope.loadSelectables = function () {
        var prmReq = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'User/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.roleList = resp.data.Roles;
                        resolve(resp.data);
                    }
                }).catch(function (err) { });
        });

        return prmReq;
    }

    // CRUD
    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0 };
        $scope.selectedRole = {};
    }

    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu kullanıcıyı silmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'User/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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

        if (typeof $scope.selectedRole != 'undefined' && $scope.selectedRole != null)
            $scope.modelObject.UserRoleId = $scope.selectedRole.Id;
        else
            $scope.modelObject.UserRoleId = null;

        $http.post(HOST_URL + 'User/SaveModel', $scope.modelObject, 'json')
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
        $http.get(HOST_URL + 'User/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;

                    // BIND EXTERNAL TYPES
                    if ($scope.modelObject.UserRoleId > 0)
                        $scope.selectedRole = $scope.roleList.find(d => d.Id == $scope.modelObject.UserRoleId);
                    else
                        $scope.selectedRole = {};
                }
            }).catch(function (err) { });
    }

    // ON LOAD EVENTS
    $scope.loadSelectables().then(function (data) {
        if (PRM_ID > 0)
            $scope.bindModel(PRM_ID);
    });
});