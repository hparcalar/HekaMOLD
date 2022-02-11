app.controller('driverCtrl', ['$scope', '$http', 'Upload',
    function ($scope, $http, Upload) {
        $scope.modelObject = {};

        $scope.selectedObjectFile = null;
        $scope.saveStatus = 0;

        $scope.selectedCountry = {};
        $scope.countryList = [];

        $scope.selectedVisaType = {};
        $scope.visaTypeList = [{ Id: 1, Text: "Schengen" }];


        // GET SELECTABLE DATA
        $scope.loadSelectables = function () {
            var prmReq = new Promise(function (resolve, reject) {
                $http.get(HOST_URL + 'Driver/GetSelectables', {}, 'json')
                    .then(function (resp) {
                        if (typeof resp.data != 'undefined' && resp.data != null) {
                            $scope.countryList = resp.data.Countrys;
                            resolve(resp.data);
                        }
                    }).catch(function (err) { });
            });

            return prmReq;
        }

        // CRUD
        $scope.openNewRecord = function () {
            $scope.modelObject = { Id: 0 };
            $scope.selectedCountry = {};
        }

        $scope.onDataSelected = function (invalids, valids) {
            if (valids.length > 0)
                $scope.selectedObjectFile = valids[0];
        }

        $scope.uploadObjectData = function () {
            if ($scope.selectedObjectFile != null) {
                Upload.upload({
                    url: HOST_URL + 'Driver/UploadProfileImage',
                    data: {
                        file: $scope.selectedObjectFile,
                        userId: $scope.modelObject.Id,
                    }
                }).then(function (resp) {
                    if (resp.data.Result) {
                        toastr.success('Profil resmi başarıyla yüklendi.');
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
                        $http.post(HOST_URL + 'Driver/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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

            if (typeof $scope.selectedCountry != 'undefined' && $scope.selectedCountry != null)
                $scope.modelObject.CountryId = $scope.selectedCountry.Id;
            else
                $scope.modelObject.CountryId = null;

            if (typeof $scope.selectedCountry != 'undefined' && $scope.selectedCountry != null)
                $scope.modelObject.CountryId = $scope.selectedCountry.Id;
            else
                $scope.modelObject.CountryId = null;

            if (typeof $scope.selectedVisaType != 'undefined' && $scope.selectedVisaType != null)
                $scope.modelObject.VisaType = $scope.selectedVisaType.Id;
            else
                $scope.modelObject.VisaType = null;

            $http.post(HOST_URL + 'Driver/SaveModel', $scope.modelObject, 'json')
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
            $http.get(HOST_URL + 'Driver/BindModel?rid=' + id, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.modelObject = resp.data;

                        // BIND EXTERNAL TYPES
                        if ($scope.modelObject.CountryId > 0)
                            $scope.selectedCountry = $scope.countryList.find(d => d.Id == $scope.modelObject.CountryId);
                        else
                            $scope.selectedCountry = {};

                        if ($scope.modelObject.VisaType > 0)
                            $scope.selectedVisaType = $scope.visaTypeList.find(d => d.Id == $scope.modelObject.VisaType);
                        else
                            $scope.selectedVisaType = {};


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