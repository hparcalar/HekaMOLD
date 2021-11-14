app.controller('layoutObjectTypeCtrl', ['$scope', '$http', 'Upload',
function ($scope, $http, Upload) {
    $scope.modelObject = {};
    $scope.selectedObjectFile = null;
    $scope.objectViewer = null;

    $scope.saveStatus = 0;

    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0 };
    }

    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu görsel nesneyi silmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'LayoutObjectType/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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

        $http.post(HOST_URL + 'LayoutObjectType/SaveModel', $scope.modelObject, 'json')
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

    $scope.onDataSelected = function (invalids, valids) {
        if (valids.length > 0)
            $scope.selectedObjectFile = valids[0];
    }

    $scope.uploadObjectData = function () {
        console.log($scope.selectedObjectFile);
        if ($scope.selectedObjectFile != null) {
            Upload.upload({
                url: HOST_URL + 'LayoutObjectType/UploadData',
                data: {
                    file: $scope.selectedObjectFile,
                    recordId: $scope.modelObject.Id,
                }
            }).then(function (resp) {
                if (resp.data.Result) {
                    toastr.success('Nesne verisi başarıyla yüklendi.');
                    $scope.loadObjectData();
                    $scope.selectedFile = null;
                }
                else
                    toastr.error(resp.data.ErrorMessage);
            }, function (resp) {
            }, function (evt) {
                //var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
                //console.log('progress: ' + progressPercentage + '% ' + evt.config.data.file.name);
            });
        }
    }

    $scope.loadObjectData = function () {
        if ($scope.objectViewer != null) {
            setTimeout(() => {
                $scope.objectViewer.loadModel({
                    title: "",
                    subtitle: "",
                    url: HOST_URL + "Outputs/LObject_" + $scope.modelObject.Id + ".glb",
                });
            }, 500);
        }
    }

    $scope.prepareObjectViewer = function () {
        BabylonViewer.viewerManager.getViewerPromiseById('object-viewer').then(function (viewer) {
            $scope.objectViewer = viewer;

            if ($scope.modelObject.Id > 0)
                $scope.loadObjectData();
        });
    }

    $scope.bindModel = function (id) {
        $http.get(HOST_URL + 'LayoutObjectType/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;

                    if ($scope.objectViewer != null)
                        $scope.loadObjectData();
                }
            }).catch(function (err) { });
    }

    // ON LOAD EVENTS
    $scope.prepareObjectViewer();
    if (PRM_ID > 0)
        $scope.bindModel(PRM_ID);
}]);