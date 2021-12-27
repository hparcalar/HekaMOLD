﻿app.controller('knitCtrl', function ($scope, $http) {
    $scope.modelObject = { Id: 0, KnitYarns:[]};

    $scope.saveStatus = 0;

    $scope.yarnRecipeList = [];
    $scope.firmList = [];

    $scope.selectedMachine = {};
    $scope.machineList = [];

    $scope.selectedQualityType = {};
    $scope.qualityTypeList = [];

    $scope.selectedDyeHouse = {};
    $scope.dyeHouseList = [{ Id: 1, Text: 'FR - FİKSE' }];

    $scope.selectedCutType = {};
    $scope.cutTypeList = [{ Id: 1, Text: 'Var' },
    { Id: 2, Text: 'Yok' }];

    $scope.selectedApparelType = {};
    $scope.apparelTypeList = [{ Id: 1, Text: 'Var' },
    { Id: 2, Text: 'Yok' }];

    $scope.selectedBulletType = {};
    $scope.bulletTypeList = [{ Id: 1, Text: 'Var' },
    { Id: 2, Text: 'Yok' }];


    // GET SELECTABLE DATA
    $scope.loadSelectables = function () {
        var prmReq = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Knit/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {

                        $scope.qualityTypeList = resp.data.QualityType;
                        $scope.machineList = resp.data.Machines;
                        $scope.yarnRecipeList = resp.data.YarnRecipes;
                        $scope.firmList = resp.data.Firms;
                        resolve(resp.data);
                    }
                }).catch(function (err) { });
        });

        return prmReq;
    }

    // CRUD
    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0,  KnitYarns: [] };
        $scope.bindWeftYarnList();
        $scope.bindWarpYarnList();
        $scope.selectedApparelType = {};
        $scope.selectedCutType = {};
        $scope.selectedBulletType = {};
        $scope.selectedMachine = {};
        $scope.selectedQualityType = {};
        $scope.selectedDyeHouse = {};
  }

    $scope.dropDownBoxEditorTemplate = function (cellElement, cellInfo) {
        return $("<div>").dxDropDownBox({
            dropDownOptions: { width: 700 },
            dataSource: $scope.yarnRecipeList,
            value: cellInfo.value,

            valueExpr: "Id",
            displayExpr: "YarnRecipeCode",
            contentTemplate: function (e) {
                return $("<div>").dxDataGrid({
                    dataSource: $scope.yarnRecipeList,
                    remoteOperations: true,
                    columns: [
                        { dataField: 'YarnRecipeCode', caption: 'İplik Kodu' },
                        { dataField: 'YarnRecipeName', caption: 'İplik Adı' },
                        { dataField: 'Denier', caption: 'Fiili Denye' },
                        { dataField: 'YarnColourName', caption: 'Renk Kodu' },
                        { dataField: 'Factor', caption: 'Katsayı' }
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

    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu stok tanımını silmek istediğinizden emin misiniz?",
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

        if (typeof $scope.selectedQualityType != 'undefined' && $scope.selectedQualityType != null)
            $scope.modelObject.ItemQualityTypeId = $scope.selectedQualityType.Id;
        else
            $scope.modelObject.ItemQualityTypeId = null;

        if (typeof $scope.selectedDyeHouse != 'undefined' && $scope.selectedDyeHouse != null)
            $scope.modelObject.ItemDyeHouseType = $scope.selectedDyeHouse.Id;
        else
            $scope.modelObject.ItemDyeHouseType = null;

        if (typeof $scope.selectedCutType != 'undefined' && $scope.selectedCutType != null)
            $scope.modelObject.ItemCutType = $scope.selectedCutType.Id;
        else
            $scope.modelObject.ItemCutType = null;

        if (typeof $scope.selectedBulletType != 'undefined' && $scope.selectedBulletType != null)
            $scope.modelObject.ItemBulletType = $scope.selectedBulletType.Id;
        else
            $scope.modelObject.ItemBulletType = null;

        if (typeof $scope.selectedApparelType != 'undefined' && $scope.selectedApparelType != null)
            $scope.modelObject.ItemApparelType = $scope.selectedApparelType.Id;
        else
            $scope.modelObject.ItemApparelType = null;

        if (typeof $scope.selectedMachine != 'undefined' && $scope.selectedMachine != null)
            $scope.modelObject.MachineId = $scope.selectedMachine.Id;
        else
            $scope.modelObject.MachineId = null;

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

                    // BIND EXTERNAL TYPES
                    if ($scope.modelObject.MachineId > 0)
                        $scope.selectedMachine = $scope.machineList.find(d => d.Id == $scope.modelObject.MachineId);
                    else
                        $scope.selectedMachine = {};

                    if ($scope.modelObject.ItemQualityTypeId > 0)
                        $scope.selectedQualityType = $scope.qualityTypeList.find(d => d.Id == $scope.modelObject.ItemQualityTypeId);
                    else
                        $scope.selectedQualityType = {};

                    if ($scope.modelObject.ItemCutType > 0) {
                        $scope.selectedCutType = $scope.cutTypeList.find(d => d.Id == $scope.modelObject.ItemCutType);
                    }
                    else
                        $scope.selectedCutType = {};

                    if ($scope.modelObject.ItemBulletType > 0) {
                        $scope.selectedBulletType = $scope.bulletTypeList.find(d => d.Id == $scope.modelObject.ItemBulletType);
                    }
                    else
                        $scope.selectedBulletType = {};

                    if ($scope.modelObject.ItemApparelType > 0) {
                        $scope.selectedApparelType = $scope.apparelTypeList.find(d => d.Id == $scope.modelObject.ItemApparelType);
                    }
                    else
                        $scope.selectedApparelType = {};

                    if ($scope.modelObject.ItemDyeHouseType > 0) {
                        $scope.selectedDyeHouse = $scope.dyeHouseList.find(d => d.Id == $scope.modelObject.ItemDyeHouseType);
                    }
                    else
                        $scope.selectedDyeHouse = {};

                    $scope.bindWarpYarnList();
                    $scope.bindWeftYarnList();
                }
            }).catch(function (err) { });
    }
    $scope.bindWeftYarnList = function () {
        $('#weftYarnList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $scope.modelObject.KnitYarns.filter(word => word.YarnType ==2);;
                },
                update: function (key, values) {
                    var obj = $scope.modelObject.KnitYarns.find(d => d.Id == key);
                    if (obj != null) {

                        if (typeof values.YarnRecipeId != 'undefined') {
                            var yarnRecipeObj = $scope.yarnRecipeList.find(d => d.Id == values.YarnRecipeId);
                            obj.YarnRecipeId = yarnRecipeObj.Id;
                            obj.YarnRecipeCode = itemObj.YarnRecipeCode;
                            obj.YarnRecipeName = itemObj.YarnRecipeName;
                            obj.Denier = itemObj.Denier;
                        }
                        if (typeof values.FirmId != 'undefined') {
                            var firmObj = $scope.firmList.find(d => d.Id == values.FirmId);
                            obj.FirmId = firmObj.Id;
                            obj.FirmName = firmObj.FirmName;
                        }

                        if (typeof values.ReportWireCount != 'undefined') { obj.ReportWireCount = values.ReportWireCount; }
                        if (typeof values.MeterWireCount != 'undefined') { obj.MeterWireCount = values.MeterWireCount; }
                        if (typeof values.Gramaj != 'undefined') { obj.Gramaj = values.Gramaj; }

                    }
                },
                remove: function (key) {
                    var obj = $scope.modelObject.KnitYarns.find(d => d.Id == key);
                    if (obj != null) {
                        $scope.modelObject.KnitYarns.splice($scope.modelObject.KnitYarns.indexOf(obj), 1);
                    }
                },
                insert: function (values) {
                    var newId = 1;

                    if ($scope.modelObject.KnitYarns.length > 0) {
                        newId = $scope.modelObject.KnitYarns.map(d => d.Id).reduce((max, n) => n > max ? n : max)
                        newId++;
                    }

                    var yarnRecipeObj = $scope.yarnRecipeList.find(d => d.Id == values.YarnRecipeId);
                    if (typeof values.FirmId != 'undefined') {
                        firmObj = $scope.firmList.find(d => d.Id == values.FirmId);
                    }
                    var newObj = {
                        Id: newId,
                        YarnRecipeId: yarnRecipeObj.Id,
                        YarnRecipeCode: yarnRecipeObj.YarnRecipeCode,
                        YarnRecipeName: yarnRecipeObj.YarnRecipeName,
                        Denier: yarnRecipeObj.Denier,
                        YarnType: 2,
                        FirmId: firmObj != null ? firmObj.id : null,
                        FirmName: firmObj != null ? firmObj.FirmName : '',
                        ReportWireCount: values.ReportWireCount,
                        MeterWireCount: values.MeterWireCount,
                        Gramaj: values.Gramaj,
                    };

                    $scope.modelObject.KnitYarns.push(newObj);
                    // $scope.calculateRow(newObj);
                },
                key: 'Id'
            },
            showColumnLines: true,
            showRowLines: true,
            rowAlternationEnabled: true,
            focusedRowEnabled: false,
            showBorders: true,
            filterRow: { visible: false }, headerFilter: { visible: false },
            groupPanel: { visible: false },
            scrolling: { mode: "virtual" },
            height: 420,
            editing: { allowUpdating: true, allowDeleting: true, allowAdding: true, mode: 'cell' },
            onInitNewRow: function (e) { },
            columns: [
                {
                    dataField: 'YarnRecipeId', caption: 'İpik Kodu',
                    lookup: {
                        dataSource: $scope.yarnRecipeList,
                        valueExpr: "Id",
                        displayExpr: "YarnRecipeCode"
                    },
                    allowSorting: false,
                    validationRules: [{ type: "required" }],
                    editCellTemplate: $scope.dropDownBoxEditorTemplate,
                    cellTemplate: function (container, options) {

                        if (typeof options.row.data.YarnRecipeCode != 'undefined'
                            && options.row.data.YarnRecipeCode != null && options.row.data.YarnRecipeCode.length > 0) {
                            container.text(options.row.data.YarnRecipeCode);
                        }
                        else
                            container.text(options.displayValue);
                    }
                },
                { dataField: 'YarnRecipeName', caption: 'İplik Adı', allowEditing: false },
                { dataField: 'Denier', caption: 'Fiili Denye', allowEditing: false },
                {
                    dataField: 'FirmId', caption: 'Firma',
                    allowSorting: false,
                    lookup: {
                        dataSource: $scope.firmList,
                        valueExpr: "Id",
                        displayExpr: "FirmName"
                    }
                },
                { dataField: 'ReportWireCount', caption: 'Rapor Tel Sayısı', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, validationRules: [{ type: "required" }] },
                { dataField: 'MeterWireCount', caption: 'Metre Tel Sayısı', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, validationRules: [{ type: "required" }] },
                { dataField: 'Gramaj', caption: 'Gramaj', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, validationRules: [{ type: "required" }] },
                { dataField: 'Density', caption: 'Sıklık', dataType: 'number', format: { type: "number", precision: 2 } },
            ]
        });
    }
    $scope.bindWarpYarnList = function () {
        $('#warpYarnList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $scope.modelObject.KnitYarns.filter(word => word.YarnType == 1);
                },
                update: function (key, values) {
                    var obj = $scope.modelObject.KnitYarns.find(d => d.Id == key);
                    if (obj != null) {

                        if (typeof values.YarnRecipeId != 'undefined') {
                            var yarnRecipeObj = $scope.yarnRecipeList.find(d => d.Id == values.YarnRecipeId);
                            obj.YarnRecipeId = yarnRecipeObj.Id;
                            obj.YarnRecipeCode = itemObj.YarnRecipeCode;
                            obj.YarnRecipeName = itemObj.YarnRecipeName;
                           obj.Denier = itemObj.Denier;
                        }
                        if (typeof values.FirmId != 'undefined') {
                            var firmObj = $scope.firmList.find(d => d.Id == values.FirmId);
                            obj.FirmId = firmObj.Id;
                            obj.FirmName = firmObj.FirmName;
                        }

                        if (typeof values.ReportWireCount != 'undefined') { obj.ReportWireCount = values.ReportWireCount; }
                        if (typeof values.MeterWireCount != 'undefined') { obj.MeterWireCount = values.MeterWireCount; }
                        if (typeof values.Gramaj != 'undefined') { obj.Gramaj = values.Gramaj; }

                    }
                },
                remove: function (key) {
                    var obj = $scope.modelObject.KnitYarns.find(d => d.Id == key);
                    if (obj != null) {
                        $scope.modelObject.KnitYarns.splice($scope.modelObject.KnitYarns.indexOf(obj), 1);
                    }
                },
                insert: function (values) {
                    var newId = 1;
                  
                    if ($scope.modelObject.KnitYarns.length > 0) {
                        newId = $scope.modelObject.KnitYarns.map(d => d.Id).reduce((max, n) => n > max ? n : max)
                        newId++;
                    }
                    
                    var yarnRecipeObj = $scope.yarnRecipeList.find(d => d.Id == values.YarnRecipeId);
                    if (typeof values.FirmId != 'undefined') {
                        firmObj = $scope.firmList.find(d => d.Id == values.FirmId);
                    }
                    var newObj = {
                        Id: newId,
                        YarnRecipeId: yarnRecipeObj.Id,
                        YarnRecipeCode: yarnRecipeObj.YarnRecipeCode,
                        YarnRecipeName: yarnRecipeObj.YarnRecipeName,
                        Denier: yarnRecipeObj.Denier,
                        YarnType: 1,
                        FirmId: firmObj != null ? firmObj.id : null,
                        FirmName: firmObj != null ? firmObj.FirmName : '',
                        ReportWireCount: values.ReportWireCount,
                        MeterWireCount: values.MeterWireCount,
                        Gramaj: values.Gramaj,
                    };

                    $scope.modelObject.KnitYarns.push(newObj);
                    // $scope.calculateRow(newObj);
                },
                key: 'Id'
            },
            showColumnLines: true,
            showRowLines: true,
            rowAlternationEnabled: true,
            focusedRowEnabled: false,
            showBorders: true,
            filterRow: { visible: false }, headerFilter: { visible: false },
            groupPanel: { visible: false },
            scrolling: { mode: "virtual" },
            height: 420,
            editing: { allowUpdating: true, allowDeleting: true, allowAdding: true, mode: 'cell' },
            onInitNewRow: function (e) { },
            columns: [
                {
                    dataField: 'YarnRecipeId', caption: 'İpik Kodu',
                    lookup: {
                        dataSource: $scope.yarnRecipeList,
                        valueExpr: "Id",
                        displayExpr: "YarnRecipeCode"
                    },
                    allowSorting: false,
                    validationRules: [{ type: "required" }],
                    editCellTemplate: $scope.dropDownBoxEditorTemplate,
                    cellTemplate: function (container, options) {

                        if (typeof options.row.data.YarnRecipeCode != 'undefined'
                            && options.row.data.YarnRecipeCode != null && options.row.data.YarnRecipeCode.length > 0) {
                            container.text(options.row.data.YarnRecipeCode);
                        }
                        else
                            container.text(options.displayValue);
                    }
                },
                { dataField: 'YarnRecipeName', caption: 'İplik Adı', allowEditing: false },
                { dataField: 'Denier', caption: 'Fiili Denye', allowEditing: false },
                {
                    dataField: 'FirmId', caption: 'Firma',
                    allowSorting: false,
                    lookup: {
                        dataSource: $scope.firmList,
                        valueExpr: "Id",
                        displayExpr: "FirmName"
                    }
                },
                { dataField: 'ReportWireCount', caption: 'Rapor Tel Sayısı', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, validationRules: [{ type: "required" }] },
                { dataField: 'MeterWireCount', caption: 'Metre Tel Sayısı', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, validationRules: [{ type: "required" }] },
                { dataField: 'Gramaj', caption: 'Gramaj', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, validationRules: [{ type: "required" }] },
                { dataField: 'Density', caption: 'Sıklık', dataType: 'number', format: { type: "number", precision: 2 } },
            ]
        });
    }

    $scope.showAttachmentList = function () {
        $scope.$broadcast('showAttachmentList',
            { RecordId: $scope.modelObject.Id, RecordType: 2 });

        $('#dial-attachments').dialog({
            width: 500,
            height: 400,
            //height: window.innerHeight * 0.6,
            hide: true,
            modal: true,
            resizable: false,
            show: true,
            draggable: false,
            closeText: "KAPAT"
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