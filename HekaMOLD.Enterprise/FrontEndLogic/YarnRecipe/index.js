﻿app.controller('yarnRecipeCtrl', function ($scope, $http) {
    $scope.modelObject = { Id: 0, YarnRecipeMixes: [] };

    $scope.saveStatus = 0;

    $scope.firmList = [];
    $scope.yarnBreedList = [];
    $scope.yarnColourList = [];
    $scope.forexTypeList = [];


    $scope.selectedYarnBreed = {};
    $scope.selectedFirm = {};
    $scope.selectedYarnColour = {};
    $scope.selectedCustomerYarnColour = {};
    $scope.selectedCenterType = {};
    $scope.selectedForexType = {};

    $scope.centerTypeList = [{ Id: 1, Text: 'Kuvvetli Punta' },
        { Id: 2, Text: 'Seyrek Punta' }, { Id: 3, Text: 'Yok' }];

    $scope.selectedTwistDirection = {};
    $scope.twistDirectionList = [{ Id: 1, Text: 'Z' },
        { Id: 2, Text: 'S' },{ Id: 3, Text: 'Yok' }];

    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0, YarnRecipeMixes: [] };
        $scope.selectedYarnBreed = {};
        $scope.selectedFirm = {};
        $scope.selectedCenterType = {};
        $scope.selectedYarnColour = {};
        $scope.selectedTwistDirection = {};
        $scope.selectedCustomerYarnColour = {};
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
                        $scope.forexTypeList = resp.data.ForexTypes;
                        resolve(resp.data);
                    }
                }).catch(function (err) { });
        });

        return prmReq;
    }

    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu iplik cinsini silmek istediğinizden emin misiniz?",
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
    $scope.GenerateCode = function () {
        var sBreedCode = $scope.selectedYarnBreed.YarnBreedCode;
        var nYanrDenier = $scope.modelObject.YarnDenier;
        var nColourCode = $scope.selectedYarnColour.YarnColourCode;
        var sFirmCode = $scope.selectedFirm.FirmCode;

        if (sBreedCode == null || sBreedCode == 'undefined') {
            toastr.error("Lütfen İplik Cinsi Seçiniz", 'Hata');
            return;
        }
        if (nYanrDenier == null || nYanrDenier == 'undefined') {
            toastr.error("Lütfen İplik Denyesi Giriniz", 'Hata');
            return;
        }
        if (nColourCode == null || nColourCode == 'undefined') {
            toastr.error("Lütfen İplik Renk Kodu Seçiniz", 'Hata');
            return;
        }
        if (sFirmCode == null || sFirmCode == 'undefined') {
            toastr.error("Lütfen Firma Kodu Seçiniz", 'Hata');
            return;
        }

        $scope.modelObject.YarnRecipeCode = sBreedCode + "-" + ("0000" + nYanrDenier).slice(-4) + "-" + nColourCode + "-" + sFirmCode;
    }
    $scope.saveModel = function () {
        $scope.saveStatus = 1;

        if (typeof $scope.selectedCenterType != 'undefined' && $scope.selectedCenterType != null) {
            $scope.modelObject.CenterType = $scope.selectedCenterType.Id;
        }
        else
            $scope.modelObject.CenterType = null;

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
            $scope.modelObject.YarnColourId = null;

        if (typeof $scope.selectedCustomerYarnColour != 'undefined' && $scope.selectedCustomerYarnColour != null) {
            $scope.modelObject.CustomerYarnColourId = $scope.selectedCustomerYarnColour.Id;
        }
        else
            $scope.modelObject.CustomerYarnColourId = null;

        if (typeof $scope.selectedForexType != 'undefined' && $scope.selectedForexType != null) {
            $scope.modelObject.ForexTypeId = $scope.selectedForexType.Id;
        }
        else
            $scope.modelObject.ForexTypeId = null;

        if (typeof $scope.selectedTwistDirection != 'undefined' && $scope.selectedTwistDirection != null) {
            $scope.modelObject.TwistDirection = $scope.selectedTwistDirection.Id;
        }
        else
            $scope.modelObject.TwistDirection = null;
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
    $scope.dropDownBoxEditorTemplate = function (cellElement, cellInfo) {
        return $("<div>").dxDropDownBox({
            dropDownOptions: { width: 700 },
            dataSource: $scope.yarnBreedList,
            value: cellInfo.value,

            valueExpr: "Id",
            displayExpr: "YarnRecipeCode",
            contentTemplate: function (e) {
                return $("<div>").dxDataGrid({
                    dataSource: $scope.yarnBreedList,
                    remoteOperations: true,
                    columns: [
                        { dataField: 'YarnBreedCode', caption: 'Cins Kodu' },
                        { dataField: 'YarnBreedName', caption: 'Cins Adı' }
                    ],
                    hoverStateEnabled: true,
                    keyExpr: "Id",
                    scrolling: { mode: "virtual" },
                    height: 250,
                    filterRow: { visible: true },
                    selection: { mode: "single" },
                    selectedRowKeys: [cellInfo.value],
                    focusedRowEnabled: true,
                    focusedRowKey: cellInfo.value,
                    onSelectionChanged: function (selectionChangedArgs) {
                        e.component.option("value", selectionChangedArgs.selectedRowKeys[0]);

                        cellInfo.setValue(selectionChangedArgs.selectedRowKeys[0]);

                        if (selectionChangedArgs.selectedRowKeys.length > 0) {
                            e.component.close();
                        }
                    }
                });
            },
        });
    }

    $scope.bindModel = function (id) {
        $http.get(HOST_URL + 'YarnRecipe/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;

                    // BIND EXTERNAL FIRM TYPE

                    if ($scope.modelObject.FirmId > 0) 
                        $scope.selectedFirm = $scope.firmList.find(d => d.Id == $scope.modelObject.FirmId);
                    else 
                        $scope.selectedFirm = {};

                    if ($scope.modelObject.YarnColourId > 0) 
                        $scope.selectedYarnColour = $scope.yarnColourList.find(d => d.Id == $scope.modelObject.YarnColourId);
                    else 
                        $scope.selectedYarnColour = {};

                    if ($scope.modelObject.CustomerYarnColourId > 0)
                        $scope.selectedCustomerYarnColour = $scope.yarnColourList.find(d => d.Id == $scope.modelObject.CustomerYarnColourId);
                    else
                        $scope.selectedCustomerYarnColour = {};

                    if ($scope.modelObject.YarnBreedId > 0) 
                        $scope.selectedYarnBreed = $scope.yarnBreedList.find(d => d.Id == $scope.modelObject.YarnBreedId);
                    else 
                        $scope.selectedYarnBreed = {};

                    if ($scope.modelObject.CenterType > 0) 
                        $scope.selectedCenterType = $scope.centerTypeList.find(d => d.Id == $scope.modelObject.CenterType);                 
                    else 
                        $scope.selectedCenterType = {};

                    if ($scope.modelObject.ForexTypeId > 0) 
                        $scope.selectedForexType = $scope.forexTypeList.find(d => d.Id == $scope.modelObject.ForexTypeId);
                    else 
                        $scope.selectedForexType = {};

                    if ($scope.modelObject.TwistDirection > 0) 
                        $scope.selectedTwistDirection = $scope.twistDirectionList.find(d => d.Id == $scope.modelObject.TwistDirection);

                    else 
                        $scope.selectedTwistDirection = {};
   
                    $scope.bindMixList();
                }
            }).catch(function (err) { });
    }

    $scope.bindMixList = function () {
        $('#mixList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $scope.modelObject.YarnRecipeMixes;
                },
                update: function (key, values) {
                    var obj = $scope.modelObject.YarnRecipeMixes.find(d => d.Id == key);
                    if (obj != null) {
                        if (typeof values.YarnBreedId != 'undefined') {
                            var yarnBreedObj = $scope.yarnBreedList.find(d => d.Id == values.YarnBreedId);
                            obj.YarnBreedId = yarnBreedObj.Id;
                            obj.YarnBreedCode = yarnBreedObj.YarnBreedCode;
                            obj.YarnBreedName = yarnBreedObj.YarnBreedName

                        }

                        if (typeof values.Percentage != 'undefined') { obj.Percentage = values.Percentage; }

                    }
                },
                remove: function (key) {
                    var obj = $scope.modelObject.YarnRecipeMixes.find(d => d.Id == key);
                    if (obj != null) {
                        $scope.modelObject.YarnRecipeMixes.splice($scope.modelObject.YarnRecipeMixes.indexOf(obj), 1);
                    }
                },
                insert: function (values) {
                    var newId = 1;
                    if ($scope.modelObject.YarnRecipeMixes.length > 0) {
                        newId = $scope.modelObject.YarnRecipeMixes.map(d => d.Id).reduce((max, n) => n > max ? n : max)
                        newId++;
                    }
                    var itemObj = $scope.yarnBreedList.find(d => d.Id == values.YarnBreedId);
                    var newObj = {
                        Id: newId,
                        YarnBreedId: itemObj.Id,
                        YarnBreedCode: itemObj.YarnBreedCode,
                        YarnBreedName: itemObj.YarnBreedName,
                        Percentage: values.Percentage,
                        NewDetail: true,
                    };
                    $scope.modelObject.YarnRecipeMixes.push(newObj);
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
                {
                    dataField: 'YarnBreedId', caption: 'Cins Kod',
                    lookup: {
                        dataSource: $scope.yarnBreedList,
                        valueExpr: "Id",
                        displayExpr: "YarnBreedCode"
                    },
                    allowSorting: false,
                    validationRules: [{ type: "required" }],
                    editCellTemplate: $scope.dropDownBoxEditorTemplate,
                    cellTemplate: function (container, options) {
                        if (typeof options.row.data.YarnBreedCode != 'undefined'
                            && options.row.data.YarnBreedCode != null && options.row.data.YarnBreedCode.length > 0)
                            container.text(options.row.data.YarnBreedCode);
                        else
                            container.text(options.displayValue);
                    }
                },
                { dataField: 'YarnBreedName', caption: 'Cins Ad', allowEditing: false },
                { dataField: 'Percentage', caption: '% oran' }
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