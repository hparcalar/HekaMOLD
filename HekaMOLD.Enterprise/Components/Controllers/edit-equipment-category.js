app.controller('editEquipmentCategoryCtrl', function ($scope, $http) {
    $scope.modelObject = {
        Id: 0,
        EquipmentCategoryCode: '',
        EquipmentCategoryName: '',
    };

    $scope.saveCategory = function () {
        try {
            $http.post(HOST_URL + 'Mobile/UpdateEquipmentCategory', $scope.modelObject, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        if (resp.data.Result == true)
                            toastr.success('İşlem başarılı.', 'Bilgilendirme');
                        else
                            toastr.error('Hata: ' + resp.data.ErrorMessage, 'Uyarı');
                    }

                    $scope.$emit('editEquipmentCategoryEnd', $scope.modelObject);
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.deleteCategory = function () {
        bootbox.confirm({
            message: "Bu ekipman kategorisini ve buna bağlı olan bilgileri silmek istediğinizden emin misiniz?",
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
                    try {
                        $http.post(HOST_URL + 'Mobile/DeleteEquipmentCategory', { rid: $scope.modelObject.Id }, 'json')
                            .then(function (resp) {
                                if (typeof resp.data != 'undefined' && resp.data != null) {
                                    if (resp.data.Result == true)
                                        toastr.success('İşlem başarılı.', 'Bilgilendirme');
                                    else
                                        toastr.error('Hata: ' + resp.data.ErrorMessage, 'Uyarı');
                                }

                                $scope.$emit('editEquipmentCategoryEnd', $scope.modelObject);
                            }).catch(function (err) { });
                    } catch (e) {

                    }
                }
            }
        });
    }

    $scope.bindCategory = function () {
        try {
            $http.get(HOST_URL + 'Mobile/GetEquipmentCategory?rid=' + $scope.modelObject.Id, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.modelObject = resp.data;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    // ON LOAD EVENTS
    $scope.$on('loadEquipmentCategory', function (e, d) {
        $scope.modelObject.Id = d.id;

        $scope.bindCategory();
    });
});