app.controller('firmCtrl', function ($scope, $http) {
    $scope.modelObject = {};

    $scope.saveStatus = 0;
    $scope.forexTypeList = [];
    $scope.cityList = [];
    $scope.countryList = [];



    $scope.selectedForexType = {};
    $scope.selectedCity = {};
    $scope.selectedCountry = {};

    $scope.selectedFirmType = {};
    $scope.firmTypeList = [{ Id: 1, Text: 'Tedarikçi' },
        { Id: 2, Text: 'Müşteri' }, { Id: 3, Text: 'Gümrükçü' }, { Id: 4, Text: 'Tedarikçi + Müşteri' }];

    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0 };
        $scope.selectedFirmType = {};
        $scope.selectedForexType = {};
        $scope.selectedCity = {};
        $scope.selectedCountry = {};

        $scope.getNextFirmCode().then(function (rNo) {
            $scope.modelObject.FirmCode = rNo;
            $scope.$apply();
        });
    }
    $scope.loadSelectables = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Firm/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.forexTypeList = resp.data.ForexTypes;
                        $scope.cityList = resp.data.Citys;
                        $scope.countryList = resp.data.Countrys;

                        resolve();
                    }
                }).catch(function (err) { });
        });

        return prms;
    }


    $scope.getNextFirmCode = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Firm/GetFirmCode', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        if (resp.data.Result) {
                            resolve(resp.data.FirmCode);
                        }
                        else {
                            toastr.error('Sıradaki sipariş numarası üretilemedi. Lütfen ekranı yenileyip tekrar deneyiniz.', 'Uyarı');
                            resolve('');
                        }
                    }
                }).catch(function (err) { });
        });

        return prms;
    }
    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu firma tanımını silmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'Firm/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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

        if (typeof $scope.selectedFirmType != 'undefined' && $scope.selectedFirmType != null) {
            $scope.modelObject.FirmType = $scope.selectedFirmType.Id;
        }
        else
            $scope.modelObject.FirmType = null;

        if (typeof $scope.selectedCity != 'undefined' && $scope.selectedCity != null) {
            $scope.modelObject.CityId = $scope.selectedCity.Id;
        }
        else
            $scope.modelObject.CityId = null;

        if (typeof $scope.selectedCountry != 'undefined' && $scope.selectedCountry != null) {
            $scope.modelObject.CountryId = $scope.selectedCountry.Id;
        }
        else
            $scope.modelObject.CountryId = null;

        if (typeof $scope.selectedForexType != 'undefined' && $scope.selectedForexType != null)
            $scope.modelObject.ForexTypeId = $scope.selectedForexType.Id;
        else
            $scope.modelObject.ForexTypeId = null;

        $http.post(HOST_URL + 'Firm/SaveModel', $scope.modelObject, 'json')
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
        $http.get(HOST_URL + 'Firm/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;

                    // BIND EXTERNAL FIRM TYPE
                    if ($scope.modelObject.FirmType > 0) {
                        $scope.selectedFirmType = $scope.firmTypeList.find(d => d.Id == $scope.modelObject.FirmType);
                    }
                    else {
                        $scope.selectedFirmType = {};
                    }
                    if ($scope.modelObject.ForexTypeId > 0)
                        $scope.selectedForexType = $scope.forexTypeList.find(d => d.Id == $scope.modelObject.ForexTypeId);
                    else
                        $scope.selectedForexType = {};

                    if ($scope.modelObject.CityId > 0)
                        $scope.selectedCity = $scope.cityList.find(d => d.Id == $scope.modelObject.CityId);
                    else
                        $scope.selectedCity = {};

                    if ($scope.modelObject.CountryId > 0)
                        $scope.selectedCountry = $scope.countryList.find(d => d.Id == $scope.modelObject.CountryId);
                    else
                        $scope.selectedcity = {};

                    $scope.bindAuthorList();
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
                        if (typeof values.SendMailForPurchaseOrder != 'undefined')
                        { obj.SendMailForPurchaseOrder = values.SendMailForPurchaseOrder; }
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
                { dataField: 'SendMailForPurchaseOrder', caption: 'Mail Gönder', dataType:'boolean' },
            ]
        });
    }
    //$scope.bindTariffList = function () {
    //    $('#tariffList').dxDataGrid({
    //        dataSource: {
    //            load: function () {
    //                return $scope.modelObject.Tariff;
    //            },
    //            update: function (key, values) {
    //                var obj = $scope.modelObject.Tariffs.find(d => d.Id == key);
    //                if (obj != null) {
    //                    if (typeof values.LadametrePrice != 'undefined') { obj.LadametrePrice = values.LadametrePrice; }
    //                    if (typeof values.MeterCupPrice != 'undefined') { obj.MeterCupPrice = values.MeterCupPrice; }
    //                    if (typeof values.WeightPrice != 'undefined') { obj.WeightPrice = values.WeightPrice; }
    //                    if (typeof values. != 'undefined') { obj.Phone = values.Phone; }
    //                    if (typeof values.SendMailForPurchaseOrder != 'undefined') { obj.SendMailForPurchaseOrder = values.SendMailForPurchaseOrder; }
    //                }
    //            },
    //            remove: function (key) {
    //                var obj = $scope.modelObject.Authors.find(d => d.Id == key);
    //                if (obj != null) {
    //                    $scope.modelObject.Authors.splice($scope.modelObject.Authors.indexOf(obj), 1);
    //                }
    //            },
    //            insert: function (values) {
    //                var newId = 1;
    //                if ($scope.modelObject.Authors.length > 0) {
    //                    newId = $scope.modelObject.Authors.map(d => d.Id).reduce((max, n) => n > max ? n : max)
    //                    newId++;
    //                }

    //                var newObj = {
    //                    Id: newId,
    //                    AuthorName: values.AuthorName,
    //                    Title: values.Title,
    //                    Email: values.Email,
    //                    Phone: values.Phone,
    //                    SendMailForPurchaseOrder: values.SendMailForPurchaseOrder,
    //                    NewDetail: true
    //                };

    //                $scope.modelObject.Authors.push(newObj);
    //            },
    //            key: 'Id'
    //        },
    //        showColumnLines: true,
    //        showRowLines: true,
    //        rowAlternationEnabled: true,
    //        focusedRowEnabled: false,
    //        showBorders: true,
    //        filterRow: {
    //            visible: false
    //        },
    //        headerFilter: {
    //            visible: false
    //        },
    //        groupPanel: {
    //            visible: false
    //        },
    //        scrolling: {
    //            mode: "virtual"
    //        },
    //        height: 200,
    //        editing: {
    //            allowUpdating: true,
    //            allowDeleting: true,
    //            allowAdding: true,
    //            mode: 'cell'
    //        },
    //        columns: [
    //            { dataField: 'AuthorName', caption: 'Yetkili', validationRules: [{ type: "required" }] },
    //            { dataField: 'Title', caption: 'Ünvan' },
    //            { dataField: 'Email', caption: 'E-Posta' },
    //            { dataField: 'Phone', caption: 'Telefon' },
    //            { dataField: 'SendMailForPurchaseOrder', caption: 'Mail Gönder', dataType: 'boolean' },
    //        ]
    //    });
    //}

    // ON LOAD EVENTS
    DevExpress.localization.locale('tr');
    $scope.loadSelectables().then(function () {
        if (PRM_ID > 0)
            $scope.bindModel(PRM_ID);
        else {
            $scope.getNextFirmCode().then(function (rNo) {
                $scope.modelObject.FirmCode = rNo;
                $scope.$apply();
            });
        }
    });
});