﻿app.controller('knitCtrl', function ($scope, $http) {
    $scope.modelObject = {};

    $scope.saveStatus = 0;

    $scope.selectedGroup = {};
    $scope.groupList = [];

    $scope.selectedKnitCutType = {};
    $scope.KnitCutTypeList = [{ Id: 1, Text: 'Var' },
        { Id: 2, Text: 'Yok' }];

    $scope.selectedKnitApparelType = {};
    $scope.KnitApparelTypeList = [{ Id: 1, Text: 'Var' },
        { Id: 2, Text: 'Yok' }];

    $scope.selectedKnitBulletType = {};
    $scope.KnitBulletTypeList = [{ Id: 1, Text: 'Var' },
    { Id: 2, Text: 'Yok' }];

    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0 };
        $scope.selectedyarnRecipeType = {};
    }
    // GET SELECTABLE DATA
    $scope.loadSelectables = function () {
        var prmReq = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Knit/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.groupList = resp.data.Groups;

                        resolve(resp.data);
                    }
                }).catch(function (err) { });
        });

        return prmReq;
    }
    $scope.performDelete = function () {
        bootbox.conyarnRecipe({
            message: "Bu Dokuma tanımını silmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'Knit/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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

        if (typeof $scope.selectedKnitCutType != 'undefined' && $scope.selectedKnitCutType != null)
            $scope.modelObject.KnitCutType = $scope.selectedKnitCutType.Id;
        else
            $scope.modelObject.KnitCutType = null;
        if (typeof $scope.selectedKnitApparelType != 'undefined' && $scope.selectedKnitApparelType != null)
            $scope.modelObject.KnitApparelType = $scope.selectedKnitApparelType.Id;
        else
            $scope.modelObject.KnitApparelType = null;
        if (typeof $scope.selectedKnitBulletType != 'undefined' && $scope.selectedKnitBulletType != null)
            $scope.modelObject.KnitBulletType = $scope.selectedKnitBulletType.Id;
        else
            $scope.modelObject.KnitCutType = null;
        //}
        //else
        //    $scope.modelObject.FirmType = null;

        $http.post(HOST_URL + 'Knit/SaveModel', $scope.modelObject, 'json')
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
        $http.get(HOST_URL + 'Knit/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;

                    // BIND EXTERNAL FIRM TYPE
                    //if ($scope.modelObject.FirmType > 0) {
                    //    $scope.selectedFirmType = $scope.firmTypeList.find(d => d.Id == $scope.modelObject.FirmType);
                    //}
                    //else {
                    //    $scope.selectedFirmType = {};
                    //}

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
    if (PRM_ID > 0)
        $scope.bindModel(PRM_ID);
});