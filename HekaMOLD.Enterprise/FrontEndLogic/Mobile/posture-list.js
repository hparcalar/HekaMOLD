app.controller('postureListCtrl', function ($scope, $http) {
    $scope.modelObject = { Id: 0 };
    $scope.machineQueue = [];

    $scope.selectedPosture = { Id: 0 };

    $scope.bindModel = function (id) {

    }

    $scope.loadMachineQueue = function () {
        try {
            $http.get(HOST_URL + 'Mobile/GetOngoingPostureList', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.machineQueue = resp.data;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.selectPosture = function (item) {
        $scope.selectedPosture = item;
    }

    // CREATE POSTURE EVENT
    $scope.finishPosture = function () {
        bootbox.prompt({
            message: "Açıklama",
            closeButton: false,
            title: 'Bu duruş sona erecektir. Onaylıyor musunuz?',
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
                if (result != null) {
                    $scope.saveStatus = 1;

                    $http.post(HOST_URL + 'Mobile/FinishPosture', { postureId: $scope.selectedPosture.Id, description: result }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Result == true) {
                                    toastr.success('Kayıt başarılı.', 'Bilgilendirme');

                                    $scope.selectedPosture = { Id: 0 };

                                    $scope.loadMachineQueue();
                                }
                                else
                                    toastr.error(resp.data.ErrorMessage, 'Hata');
                            }
                        }).catch(function (err) { });
                }
            }
        });
    }

    // LOAD EVENTS
    $scope.isMechanics = IS_MECHANICS;
    setTimeout($scope.loadMachineQueue, 500);
});