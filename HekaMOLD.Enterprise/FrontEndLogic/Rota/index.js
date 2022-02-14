app.controller('rotaCtrl', ['$scope', '$http', 'Upload',
    function ($scope, $http, Upload) {
        $scope.modelObject = {};

        $scope.selectedObjectFile = null;
        $scope.saveStatus = 0;

        $scope.selectedCityStart = {};
        $scope.selectedCityEnd = {};

        $scope.cityList = [];

        // GET SELECTABLE DATA
        $scope.loadSelectables = function () {
            var prmReq = new Promise(function (resolve, reject) {
                $http.get(HOST_URL + 'Rota/GetSelectables', {}, 'json')
                    .then(function (resp) {
                        if (typeof resp.data != 'undefined' && resp.data != null) {
                            $scope.cityList = resp.data.Citys;
                            resolve(resp.data);
                        }
                    }).catch(function (err) { });
            });

            return prmReq;
        }

        // CRUD
        $scope.openNewRecord = function () {
            $scope.modelObject = { Id: 0 };
            $scope.selectedCityStart = {};
            $scope.selectedCityEnd = {};        }

        $scope.onDataSelected = function (invalids, valids) {
            if (valids.length > 0)
                $scope.selectedObjectFile = valids[0];
        }

        $scope.uploadObjectData = function () {
            if ($scope.selectedObjectFile != null) {
                Upload.upload({
                    url: HOST_URL + 'Rota/UploadProfileImage',
                    data: {
                        file: $scope.selectedObjectFile,
                        userId: $scope.modelObject.Id,
                    }
                }).then(function (resp) {
                    if (resp.data.Result) {
                        toastr.success('Rota resmi başarıyla yüklendi.');
                        $scope.bindModel();
                        $scope.selectedObjectFile = null;
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

        $scope.performDelete = function () {
            bootbox.confirm({
                message: "Bu kullanıcıyı silmek istediğinizden emin misiniz?",
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
                        $http.post(HOST_URL + 'Rota/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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

            if (typeof $scope.selectedCityStart != 'undefined' && $scope.selectedCityStart != null)
                $scope.modelObject.CityStartId = $scope.selectedCityStart.Id;
            else
                $scope.modelObject.CityStartId = null;

            if (typeof $scope.selectedCityEnd != 'undefined' && $scope.selectedCityEnd != null)
                $scope.modelObject.CityEndId = $scope.selectedCityEnd.Id;
            else
                $scope.modelObject.CityEndId = null;

            $http.post(HOST_URL + 'Rota/SaveModel', $scope.modelObject, 'json')
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
            $http.get(HOST_URL + 'Rota/BindModel?rid=' + id, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.modelObject = resp.data;

                        // BIND EXTERNAL TYPES
                        if ($scope.modelObject.CityStartId > 0)
                            $scope.selectedCityStart = $scope.cityList.find(d => d.Id == $scope.modelObject.CityStartId);
                        else
                            $scope.selectedCityStart = {};

                        if ($scope.modelObject.CityEndId > 0)
                            $scope.selectedCityEnd = $scope.cityList.find(d => d.Id == $scope.modelObject.CityEndId);
                        else
                            $scope.selectedCityEnd = {};


                        if ($scope.modelObject.ProfileImage != null) {
                            $('#imgProfile').attr('src', $scope.modelObject.ProfileImageBase64);
                        }
                        else
                            $('#imgProfile').attr('src', '');
                    }
                }).catch(function (err) { });
        }

        // ON LOAD EVENTS
        $scope.loadSelectables().then(function (data) {
            if (PRM_ID > 0)
                $scope.bindModel(PRM_ID);
        });
    }]);