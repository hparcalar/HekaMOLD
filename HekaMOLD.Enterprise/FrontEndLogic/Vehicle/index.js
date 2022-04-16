app.controller('vehicleCtrl', function ($scope, $http) {
    $scope.modelObject = {};

    $scope.saveStatus = 0;

    $scope.selectedVehicleType = {};
    $scope.vehicleTypeList = [];
    $scope.firmList = [];
    $scope.selectedFirm = {};


    $scope.selectedTrailerType = {};
    $scope.trailerTypeList = [{ Id: 1, Text: 'Çadırlı' },
    { Id: 2, Text: 'Frigo' }, { Id: 3, Text: 'Kapalı Kasa' }, { Id: 4, Text: 'Optima' }, { Id: 5, Text: 'Mega' }
        , { Id: 6, Text: 'Konteyner' }, { Id: 7, Text: 'Swapboddy' }, { Id: 8, Text: 'Lowbed' }
        , { Id: 9, Text: 'Kamyon Romörk' }, { Id: 10, Text: 'Standart' }, { Id: 10, Text: 'Minivan' }];

    $scope.selectedVehicleAllocationType = {};
    $scope.vehicleAllocationTypeList = [{ Id: 1, Text: 'Öz Mal' },
    { Id: 2, Text: 'Kiralık' }];

    $scope.selectedForexType = {};
    $scope.forexTypeList = [];

    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0 };
        $scope.selectedTrailerType = {};
        $scope.selectedVehicleType = {};
        $scope.selectedVehicleAllocationType = {};
        $scope.selectedForexType = {};
        $scope.selectedFirm = {};
    }
    $scope.loadSelectables = function () {
        var prmReq = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Vehicle/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.vehicleTypeList = resp.data.VehicleTypes;
                        $scope.firmList = resp.data.Firms;

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
                { dataField: 'CareDateStr', caption: 'Bakım Tarihi', format: 'dd.MM.yyyy' },
                { dataField: 'VehicleCareTypeName', caption: 'Bakım Tip' },
                { dataField: 'FirmName', caption: 'İşlem Firma' },
                { dataField: 'Amount', caption: 'Fiyat' },
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
                { dataField: 'StartDateStr', caption: 'Başlangıç Tarihi', format: 'dd.MM.yyyy' },
                { dataField: 'EndDateStr', caption: 'Bitiş Tarihi', format: 'dd.MM.yyyy' },
                { dataField: 'VehicleInsuranceTypeName', caption: 'Belge Tip' },
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
                { dataField: 'MontageDateStr', caption: 'İşlem Tarihi', format: 'dd.MM.yyyy' },
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
        $http.post(HOST_URL + 'Vehicle/SaveModel', {
            Id: $scope.modelObject.Id,
            Plate: $scope.modelObject.Plate,
            Mark: $scope.modelObject.Mark,
            Versiyon: $scope.modelObject.Versiyon,
            Length: $scope.modelObject.Length,
            Width: $scope.modelObject.Width,
            Height: $scope.modelObject.Height,
            VehicleTypeId: $scope.selectedVehicleType.Id,
            TrailerHeadWeight: $scope.modelObject.TrailerHeadWeight,
            LoadCapacity: $scope.modelObject.LoadCapacity,
            ChassisNumber: $scope.modelObject.ChassisNumber,
            TrailerType: $scope.selectedTrailerType.Id,
            VehicleAllocationType: $scope.selectedVehicleAllocationType.Id,
            ContractStartDate: $scope.modelObject.ContractStartDateStr,
            ContractEndDate: $scope.modelObject.ContractEndDateStr,
            Price: $scope.modelObject.Price,
            CareNotification: $scope.modelObject.CareNotification,
            TireNotification: $scope.modelObject.TireNotification,
            KmHour: $scope.modelObject.KmHour,
            KmHourControl: $scope.modelObject.KmHourControl,
            HasLoadPlannig: $scope.modelObject.HasLoadPlannig,
            CarePeriyot: $scope.modelObject.CarePeriyot,
            ProportionalLimit: $scope.modelObject.ProportionalLimit,
            OwnerFirmId: $scope.selectedFirm.Id,
            Explanation: $scope.modelObject.Explanation
        }, 'json')
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
            }).catch(function (err) {


                alert(err);


            });
    }
    $scope.bindModel = function (id) {
        $http.get(HOST_URL + 'Vehicle/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;

                    if ($scope.modelObject.VehicleTypeId > 0)
                        $scope.selectedVehicleType = $scope.vehicleTypeList.find(d => d.Id == $scope.modelObject.VehicleTypeId);
                    else
                        $scope.selectedVehicleType = {};

                    if ($scope.modelObject.OwnerFirmId > 0)
                        $scope.selectedFirm = $scope.firmList.find(d => d.Id == $scope.modelObject.OwnerFirmId);
                    else
                        $scope.selectedFirm = {};

                    if ($scope.modelObject.VehicleAllocationType > 0)
                        $scope.selectedVehicleAllocationType = $scope.vehicleAllocationTypeList.find(d => d.Id == $scope.modelObject.VehicleAllocationType);
                    else
                        $scope.selectedOwnerFirm = {};

                    if ($scope.modelObject.TrailerType > 0)
                        $scope.selectedTrailerType = $scope.trailerTypeList.find(d => d.Id == $scope.modelObject.TrailerType);
                    else
                        $scope.selectedTrailerType = {};

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