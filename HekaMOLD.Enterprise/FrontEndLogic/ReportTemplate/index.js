app.controller('reportTemplateCtrl', function ($scope, $http) {
    $scope.modelObject = {};

    $scope.saveStatus = 0;

    $scope.reportTypeList = [
        { Id: 1, Text: 'İrsaliye' },
        { Id: 2, Text: 'Sevkiyat Çeki Listesi' },
        { Id: 3, Text: 'İş Emri' },
        { Id: 4, Text: 'İş Emri Satır Formu' },
        { Id: 5, Text: 'Ürün Etiketi' },
    ];
    $scope.selectedReportType = { Id: 0 };

    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0 };
    }

    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu rapor şablonunu silmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'ReportTemplate/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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

        if (typeof $scope.selectedReportType != 'undefined' && $scope.selectedReportType != null) {
            $scope.modelObject.ReportType = $scope.selectedReportType.Id;
        }
        else
            $scope.modelObject.ReportType = null;

        if ($scope.modelObject.ReportType == null) {
            toastr.error('Şablon türü seçmelisiniz.');
            return;
        }

        $http.post(HOST_URL + 'ReportTemplate/SaveModel', $scope.modelObject, 'json')
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
        $http.get(HOST_URL + 'ReportTemplate/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;

                    // BIND EXTERNAL REPORT TYPE
                    if ($scope.modelObject.ReportType > 0) {
                        $scope.selectedReportType = $scope.reportTypeList.find(d => d.Id == $scope.modelObject.ReportType);
                    }
                    else {
                        $scope.selectedReportType = {};
                    }
                }
            }).catch(function (err) { });
    }

    // ON LOAD EVENTS
    if (PRM_ID > 0)
        $scope.bindModel(PRM_ID);
});