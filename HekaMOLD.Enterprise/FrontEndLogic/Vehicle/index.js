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

    $scope.selectedForexType = {};
    $scope.forexTypeList = [];

    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0 };
        $scope.selectedTrailerType = {};
        $scope.selectedVehicleType = {};
        $scope.selectedVehicleAllocationType = {};
        $scope.selectedForexType = {};
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
    $scope.bindCareList = function () {
        $('#careList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $scope.modelObject.VehicleCares;
                },
                update: function (key, values) {
                    var obj = $scope.modelObject.VehicleCares.find(d => d.Id == key);
                    if (obj != null) {
                        if (typeof values.AuthorName != 'undefined') { obj.AuthorName = values.AuthorName; }
                        if (typeof values.Title != 'undefined') { obj.Title = values.Title; }
                        if (typeof values.Email != 'undefined') { obj.Email = values.Email; }
                        if (typeof values.Phone != 'undefined') { obj.Phone = values.Phone; }
                        if (typeof values.SendMailForPurchaseOrder != 'undefined') { obj.SendMailForPurchaseOrder = values.SendMailForPurchaseOrder; }
                    }
                },
                remove: function (key) {
                    var obj = $scope.modelObject.VehicleCares.find(d => d.Id == key);
                    if (obj != null) {
                        $scope.modelObject.Authors.splice($scope.modelObject.VehicleCares.indexOf(obj), 1);
                    }
                },
                insert: function (values) {
                    var newId = 1;
                    if ($scope.modelObject.VehicleCares.length > 0) {
                        newId = $scope.modelObject.VehicleCares.map(d => d.Id).reduce((max, n) => n > max ? n : max)
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
                allowUpdating: false,
                allowDeleting: false,
                allowAdding: false,
                mode: 'cell'
            },
            columns: [
                { dataField: 'Plate', caption: 'Plaka', validationRules: [{ type: "required" }] },
                { dataField: 'CareDate', caption: 'Bakım Tarihi', format: 'dd.MM.yyyy' },
                { dataField: 'VehicleCareTypeName', caption: 'Bakım Tip' },
                { dataField: 'FirmName', caption: 'İşlem Firma' },
                { dataField: 'Amount', caption: 'Fiyat'},
                { dataField: 'ForexTypeCode', caption: 'Döviz Kodu' },
            ]
        });
    }
    $scope.bindInsuranceList = function () {
        $('#insuranceList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $scope.modelObject.VehicleInsurances;
                },
                update: function (key, values) {
                    var obj = $scope.modelObject.VehicleInsurances.find(d => d.Id == key);
                    if (obj != null) {
                        if (typeof values.AuthorName != 'undefined') { obj.AuthorName = values.AuthorName; }
                        if (typeof values.Title != 'undefined') { obj.Title = values.Title; }
                        if (typeof values.Email != 'undefined') { obj.Email = values.Email; }
                        if (typeof values.Phone != 'undefined') { obj.Phone = values.Phone; }
                        if (typeof values.SendMailForPurchaseOrder != 'undefined') { obj.SendMailForPurchaseOrder = values.SendMailForPurchaseOrder; }
                    }
                },
                remove: function (key) {
                    var obj = $scope.modelObject.VehicleInsurances.find(d => d.Id == key);
                    if (obj != null) {
                        $scope.modelObject.VehicleInsurances.splice($scope.modelObject.VehicleInsurances.indexOf(obj), 1);
                    }
                },
                insert: function (values) {
                    var newId = 1;
                    if ($scope.modelObject.VehicleInsurances.length > 0) {
                        newId = $scope.modelObject.VehicleInsurances.map(d => d.Id).reduce((max, n) => n > max ? n : max)
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
                allowUpdating: false,
                allowDeleting: false,
                allowAdding: false,
                mode: 'cell'
            },
            columns: [
                { dataField: 'Plate', caption: 'Plaka' },
                { dataField: 'StartDate', caption: 'Başlangıç Tarihi', format: 'dd.MM.yyyy' },
                { dataField: 'EndDate', caption: 'Bitiş Tarihi', format: 'dd.MM.yyyy' },
                { dataField: 'VehicleInsuranceTypeName', caption: 'Sigorta Tip' },
                { dataField: 'FirmName', caption: 'İşlem Firma' },
                { dataField: 'Amount', caption: 'Fiyat' },
                { dataField: 'ForexTypeCode', caption: 'Döviz Kodu' },
            ]
        });
    }
    $scope.bindTireList = function () {
        $('#tireList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $scope.modelObject.VehicleTires;
                },
                update: function (key, values) {
                    var obj = $scope.modelObject.VehicleTires.find(d => d.Id == key);
                    if (obj != null) {
                        if (typeof values.AuthorName != 'undefined') { obj.AuthorName = values.AuthorName; }
                        if (typeof values.Title != 'undefined') { obj.Title = values.Title; }
                        if (typeof values.Email != 'undefined') { obj.Email = values.Email; }
                        if (typeof values.Phone != 'undefined') { obj.Phone = values.Phone; }
                        if (typeof values.SendMailForPurchaseOrder != 'undefined') { obj.SendMailForPurchaseOrder = values.SendMailForPurchaseOrder; }
                    }
                },
                remove: function (key) {
                    var obj = $scope.modelObject.VehicleTires.find(d => d.Id == key);
                    if (obj != null) {
                        $scope.modelObject.VehicleTires.splice($scope.modelObject.VehicleTires.indexOf(obj), 1);
                    }
                },
                insert: function (values) {
                    var newId = 1;
                    if ($scope.modelObject.VehicleTires.length > 0) {
                        newId = $scope.modelObject.VehicleTires.map(d => d.Id).reduce((max, n) => n > max ? n : max)
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
                allowUpdating: false,
                allowDeleting: false,
                allowAdding: false,
                mode: 'cell'
            },
            columns: [
                { dataField: 'Plate', caption: 'Plaka' },
                { dataField: 'CareDate', caption: 'İşlem Tarihi', format: 'dd.MM.yyyy'},
                { dataField: 'VehicleTireDirectionTypeName', caption: 'Lastik Yön Tipi' },
                { dataField: 'VehicleTireTypeStr', caption: 'Lastik İşlem Tipi' },
                { dataField: 'FirmName', caption: 'İşlem Firma' },
                { dataField: 'KmHour', caption: 'Km/Saat' },
                { dataField: 'Amount', caption: 'Fiyat' },
                { dataField: 'ForexTypeCode', caption: 'Döviz Kodu' },
            ]
        });
    }

    $scope.saveModel = function () {
        $scope.saveStatus = 1;
        alert($scope.modelObject.Plate);
        alert($scope.modelObject.Model);
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
                    $scope.bindCareList();
                    $scope.bindInsuranceList();
                    $scope.bindTireList();
                }
            }).catch(function (err) { });
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