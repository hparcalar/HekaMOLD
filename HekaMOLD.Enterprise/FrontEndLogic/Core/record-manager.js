app.controller('recordManagerCtrl', function ($scope, $http) {

    $scope._showRecordInformation = function (data) {
        if (typeof data != 'undefined' && data != null) {
            $http.get(HOST_URL + 'Common/GetRecordInformation?id=' + data.Id
                + '&dataType=' + data.DataType, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        bootbox.alert({
                            message:'<label class="form-label fs-6 fw-bolder text-gray-700 mb-3">Oluşturan</label>'+
                                    '<div class="mb-5">'+
                                        '<input type="text" class="form-control form-control-sm form-control-solid" readonly="readonly" value="'+ resp.data.CreatedUserName +'" />'+
                                    '</div>' +
                                    '<label class="form-label fs-6 fw-bolder text-gray-700 mb-3">Oluşturulma Tarihi</label>' +
                                    '<div class="mb-5">' +
                                    '<input type="text" class="form-control form-control-sm form-control-solid" readonly="readonly" value="' + resp.data.CreatedDateStr + '" />' +
                                    '</div>' +
                                    '<label class="form-label fs-6 fw-bolder text-gray-700 mb-3">Güncelleyen</label>' +
                                    '<div class="mb-5">' +
                                    '<input type="text" class="form-control form-control-sm form-control-solid" readonly="readonly" value="' + resp.data.UpdatedUserName + '" />' +
                                    '</div>' +
                                    '<label class="form-label fs-6 fw-bolder text-gray-700 mb-3">Güncellenme Tarihi</label>' +
                                    '<div class="mb-5">' +
                                    '<input type="text" class="form-control form-control-sm form-control-solid" readonly="readonly" value="' + resp.data.UpdatedDateStr + '" />' +
                                    '</div>'
                            ,
                            closeButton: false,
                            backdrop: true,
                            locale:'tr'
                        });
                    }
                }).catch(function (err) { });
        }
        else
            toastr.error('Kayıt bilgisine ulaşılamıyor.', 'Uyarı');
    }

    // ON LOAD EVENTS
    $scope.$on('showRecordInformation', function (e, d) {
        $scope._showRecordInformation(d);
    });
});