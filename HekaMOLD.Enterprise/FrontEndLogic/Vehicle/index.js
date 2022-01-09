app.controller('vehicleCtrl', function ($scope, $http) {
    $scope.modelObject = {};

    $scope.saveStatus = 0;

    $scope.selectedVehicleType = {};
    $scope.vehicleTypeList = [];

    $scope.selectedTrailerType = {};
    $scope.trailerTypeList = [{ Id: 1, Text: 'Çadırlı' },
        { Id: 2, Text: 'Frigo' }, { Id: 3, Text: 'Kapalı' }];

    $scope.selectedVehicleAllocationType = {};
    $scope.vehicleAllocationTypeList = [{ Id: 1, Text: 'Satın Alma' },
    { Id: 2, Text: 'Kiralık' }];

    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0 };
        $scope.selectedTrailerType = {};
        $scope.selectedVehicleType = {};
        $scope.selectedVehicleAllocationType = {};

    }
    $scope.loadSelectables = function () {
        var prmReq = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Vehicle/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.vehicleTypeList = resp.data.VehicleTypes;

                        resolve(resp.data);
                    }
                }).catch(function (err) { });
        });

        return prmReq;
    }
    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu firma tanımını silmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'Vehicle/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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

        if (typeof $scope.selectedTrailerType != 'undefined' && $scope.selectedTrailerType != null) {
            $scope.modelObject.TrailerType = $scope.selectedTrailerType.Id;
        }
        else
            $scope.modelObject.TrailerType = null;

        if (typeof $scope.selectedVehicleAllocationType != 'undefined' && $scope.selectedVehicleAllocationType != null) {
            $scope.modelObject.VehicleAllocationType = $scope.selectedVehicleAllocationType.Id;
        }
        else
            $scope.modelObject.VehicleAllocationType = null;
        alert($scope.modelObject.Plate);
        $http.post(HOST_URL + 'Vehicle/SaveModel', $scope.modelObject, 'json')
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
        $http.get(HOST_URL + 'Vehicle/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;
                    if ($scope.modelObject.TrailerType > 0) {
                        $scope.selectedTrailerType = $scope.trailerTypeList.find(d => d.Id == $scope.modelObject.TrailerType);
                    }
                    else {
                        $scope.selectedTrailerType = {};
                    }

                    if ($scope.modelObject.VehicleAllocationType > 0) {
                        $scope.selectedVehicleAllocationType = $scope.vehicleAllocationTypeList.find(d => d.Id == $scope.modelObject.VehicleAllocationType);
                    }
                    else {
                        $scope.selectedVehicleAllocationType = {};
                    }

                   // $scope.bindAuthorList();
                }
            }).catch(function (err) { });
    }

    $scope.bindAuthorList = function () {
        $('#authorList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $scope.modelObject.Authors;
                },
                update: function (key, values) {
                    var obj = $scope.modelObject.Authors.find(d => d.Id == key);
                    if (obj != null) {
                        if (typeof values.AuthorName != 'undefined') { obj.AuthorName = values.AuthorName; }
                        if (typeof values.Title != 'undefined') { obj.Title = values.Title; }
                        if (typeof values.Email != 'undefined') { obj.Email = values.Email; }
                        if (typeof values.Phone != 'undefined') { obj.Phone = values.Phone; }
                        if (typeof values.SendMailForPurchaseOrder != 'undefined') { obj.SendMailForPurchaseOrder = values.SendMailForPurchaseOrder; }
                    }
                },
                remove: function (key) {
                    var obj = $scope.modelObject.Authors.find(d => d.Id == key);
                    if (obj != null) {
                        $scope.modelObject.Authors.splice($scope.modelObject.Authors.indexOf(obj), 1);
                    }
                },
                insert: function (values) {
                    var newId = 1;
                    if ($scope.modelObject.Authors.length > 0) {
                        newId = $scope.modelObject.Authors.map(d => d.Id).reduce((max, n) => n > max ? n : max)
                        newId++;
                    }

                    var newObj = {
                        Id: newId,
                        AuthorName: values.AuthorName,
                        Title: values.Title,
                        Email: values.Email,
                        Phone: values.Phone,
                        SendMailForPurchaseOrder: values.SendMailForPurchaseOrder,
                        NewDetail: true
                    };

                    $scope.modelObject.Authors.push(newObj);
                },
                key: 'Id'
            },
            showColumnLines: true,
            showRowLines: true,
            rowAlternationEnabled: true,
            focusedRowEnabled: false,
            showBorders: true,
            filterRow: {
                visible: false
            },
            headerFilter: {
                visible: false
            },
            groupPanel: {
                visible: false
            },
            scrolling: {
                mode: "virtual"
            },
            height: 200,
            editing: {
                allowUpdating: true,
                allowDeleting: true,
                allowAdding: true,
                mode: 'cell'
            },
            columns: [
                { dataField: 'AuthorName', caption: 'Yetkili', validationRules: [{ type: "required" }] },
                { dataField: 'Title', caption: 'Ünvan' },
                { dataField: 'Email', caption: 'E-Posta' },
                { dataField: 'Phone', caption: 'Telefon' },
                { dataField: 'SendMailForPurchaseOrder', caption: 'Mail Gönder', dataType: 'boolean' },
            ]
        });
    }

    // ON LOAD EVENTS
    DevExpress.localization.locale('tr');
    $scope.loadSelectables().then(function (data) {
        if (PRM_ID > 0)
            $scope.bindModel(PRM_ID);
        else
            $scope.bindModel(0);
    });
});