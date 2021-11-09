app.controller('manageShiftsCtrl', function ($scope, $http) {
    $scope.modelObject = {
        Id: 0,
    };

    $scope.bindModel = function (id) {
        $http.get(HOST_URL + 'Common/GetShiftList', {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.shiftList = resp.data;
                }
            }).catch(function (err) { });
    }

    $scope.shiftList = [];

    $scope.saveChanges = function (item) {
        bootbox.confirm({
            message: item.ShiftCode + " vardiyası değişikliklerini onaylıyor musunuz?",
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

                    $http.post(HOST_URL + 'Mobile/SaveShift', item, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Result == true) {
                                    toastr.success('Değişiklikleriniz başarıyla kaydedildi.', 'Bilgilendirme');

                                    $scope.bindModel();
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
    $scope.bindModel();
});