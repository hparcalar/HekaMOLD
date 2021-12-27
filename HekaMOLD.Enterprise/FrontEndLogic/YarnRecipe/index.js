app.controller('yarnRecipeCtrl', function ($scope, $http) {
    $scope.modelObject = {};

    $scope.saveStatus = 0;

    $scope.selectedYarnBreed = {};
    $scope.yarnBreedList = [];

    $scope.selectedFirm = { };
    $scope.firmList = [];

    $scope.selectedYarnColour = {};
    $scope.yarnColourList = [];

    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0 };
    }

    // GET SELECTABLE DATA
    $scope.loadSelectables = function () {
        var prmReq = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'YarnRecipe/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {

                        $scope.firmList = resp.data.Firms;
                        $scope.yarnColourList = resp.data.Colours;
                        $scope.yarnBreedList = resp.data.YarnBreed;

                        //var emptyMcObj = { Id: 0, MachineName: '-- Seçiniz --' };
                        //$scope.machineList.splice(0, 0, emptyMcObj);
                        //$scope.selectedMachine = emptyMcObj;
                        //$scope.firmList = resp.data.Firms;
                        //$scope.unitList = resp.data.Units;

                        resolve(resp.data);
                    }
                }).catch(function (err) { });
        });

        return prmReq;
    }

    $scope.performDelete = function () {
        bootbox.conyarnRecipe({
            message: "Bu iplik tanımını silmek istediğinizden emin misiniz?",
            closeButton: false,
            buttons: {
                conyarnRecipe: {
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
                    $http.post(HOST_URL + 'YarnRecipe/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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

        if (typeof $scope.selectedFirm != 'undefined' && $scope.selectedFirm != null) {
            $scope.modelObject.FirmId = $scope.selectedFirm.Id;
        }
        else
            $scope.modelObject.FirmId = null;

        if (typeof $scope.selectedYarnBreed != 'undefined' && $scope.selectedYarnBreed != null) {
            $scope.modelObject.YarnBreedId = $scope.selectedYarnBreed.Id;
        }
        else
            $scope.modelObject.YarnBreedId = null;

        if (typeof $scope.selectedYarnColour != 'undefined' && $scope.selectedYarnColour != null) {
            $scope.modelObject.YarnColourId = $scope.selectedYarnColour.Id;
        }
        else
            $scope.modelObject.YarnBreedId = null;

    

        $http.post(HOST_URL + 'YarnRecipe/SaveModel', $scope.modelObject, 'json')
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
        $http.get(HOST_URL + 'YarnRecipe/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;

                    // BIND EXTERNAL FIRM TYPE

                    if ($scope.modelObject.FirmId > 0) {
                        $scope.selectedFirm = $scope.firmList.find(d => d.Id == $scope.modelObject.FirmId);
                    }
                    else {
                        $scope.selectedFirm = {};
                    }
                    if ($scope.modelObject.YarnColourId > 0) {
                        $scope.selectedYarnColour = $scope.yarnColourList.find(d => d.Id == $scope.modelObject.YarnColourId);
                    }
                    else {
                        $scope.selectedYarnColour = {};
                    }
                    if ($scope.modelObject.YarnBreedId > 0) {
                        $scope.selectedYarnBreed = $scope.yarnBreedList.find(d => d.Id == $scope.modelObject.YarnBreedId);
                    }
                    else {
                        $scope.selectedYarnBreed = {};
                    }
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