
app.controller('knitVariantCtrl', function ($scope, $http) {
    $scope.modelObject = { Id: 0, KnitYarns: [] };
    $scope.saveStatus = 0;

    $scope.yarnRecipeList = [];
    $scope.firmList = [];

    $scope.selectedWeavingDraft = {};
    $scope.weavingDraftList = [];

    $scope.selectedQualityType = {};
    $scope.qualityTypeList = [];
    $scope.itemVariantList = [];

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

    //GET TEST NO
    $scope.getNextAttemptNo = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Knit/GetNextAttemptNo', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        if (resp.data.Result) {
                            resolve(resp.data.AttemptNo);
                        }
                        else {
                            toastr.error('Sıradaki deneme numarası üretilemedi. Lütfen ekranı yenileyip tekrar deneyiniz.', 'Uyarı');
                            resolve('');
                        }
                    }
                }).catch(function (err) { });
        });

        return prms;
    }
    // GET SELECTABLE DATA
    $scope.loadSelectables = function () {
        var prmReq = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Knit/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {

                        $scope.qualityTypeList = resp.data.QualityType;
                        $scope.weavingDraftList = resp.data.WeavingDrafts;
                        $scope.yarnRecipeList = resp.data.YarnRecipes;
                        $scope.firmList = resp.data.Firms;
                        $scope.itemVariantList = resp.data.ItemVariants;
                        resolve(resp.data);
                    }
                }).catch(function (err) { });
        });

        return prmReq;
    }

    $scope.valueChanged = function (oldValue, type) {

        if (type == 1 && $scope.modelObject.CrudeGramaj != oldValue) {
            let rate = $scope.modelObject.CrudeGramaj / oldValue;
            $scope.modelObject.MeterGramaj = $scope.modelObject.MeterGramaj * rate;
            $scope.modelObject.KnitYarns.forEach(element => { element.ReportWireCount = element.ReportWireCount * rate; element.Gramaj = element.Gramaj * rate; });
            $scope.bindWarpYarnList();
            $scope.bindWeftYarnList();

        }
        if (type == 3 && $scope.modelObject.MeterGramaj != oldValue) {
            let rate = $scope.modelObject.MeterGramaj / oldValue;
            $scope.modelObject.CrudeGramaj = $scope.modelObject.CrudeGramaj * rate;
            $scope.modelObject.KnitYarns.forEach(element => { element.ReportWireCount = element.ReportWireCount * rate; element.Gramaj = element.Gramaj * rate; });
            $scope.bindWarpYarnList();
            $scope.bindWeftYarnList();
        }
    }
    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0, KnitYarns: [] };
        $scope.getNextAttemptNo().then(function (rNo) {
            $scope.modelObject.AttemptNo = rNo;
            $scope.$apply();
        });
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
            dropDownOptions: { width: 1000 },
            dataSource: $scope.yarnRecipeList,
            value: cellInfo.value,

            valueExpr: "Id",
            displayExpr: "YarnRecipeCode",
            contentTemplate: function (e) {
                return $("<div>").dxDataGrid({
                    dataSource: $scope.yarnRecipeList,
                    remoteOperations: true,
                    columns: [
                        { dataField: 'YarnRecipeCode', caption: 'İplik Kodu', width: 200 },
                        { dataField: 'YarnRecipeName', caption: 'İplik Adı', width: 350 },
                        { dataField: 'FirmName', caption: 'Firma', width: 200 },
                        { dataField: 'Denier', caption: 'Fiili Denye', width: 100 },
                        { dataField: 'YarnColourName', caption: 'Renk Kodu', width: 100 },
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

        if (typeof $scope.selectedWeavingDraft != 'undefined' && $scope.selectedWeavingDraft != null)
            $scope.modelObject.WeavingDraftId = $scope.selectedWeavingDraft.Id;
        else
            $scope.modelObject.WeavingDraftId = null;

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
    $scope.creatVariant = function () {
        $scope.saveStatus = 1;
        //$scope.modelObject.ItemId = $scope.modelObject.Id;
        //$scope.modelObject.Id = 0;

        //if (typeof $scope.selectedQualityType != 'undefined' && $scope.selectedQualityType != null)
        //    $scope.modelObject.ItemQualityTypeId = $scope.selectedQualityType.Id;
        //else
        //    $scope.modelObject.ItemQualityTypeId = null;

        //if (typeof $scope.selectedDyeHouse != 'undefined' && $scope.selectedDyeHouse != null)
        //    $scope.modelObject.ItemDyeHouseType = $scope.selectedDyeHouse.Id;
        //else
        //    $scope.modelObject.ItemDyeHouseType = null;

        //if (typeof $scope.selectedCutType != 'undefined' && $scope.selectedCutType != null)
        //    $scope.modelObject.ItemCutType = $scope.selectedCutType.Id;
        //else
        //    $scope.modelObject.ItemCutType = null;

        //if (typeof $scope.selectedBulletType != 'undefined' && $scope.selectedBulletType != null)
        //    $scope.modelObject.ItemBulletType = $scope.selectedBulletType.Id;
        //else
        //    $scope.modelObject.ItemBulletType = null;

        //if (typeof $scope.selectedApparelType != 'undefined' && $scope.selectedApparelType != null)
        //    $scope.modelObject.ItemApparelType = $scope.selectedApparelType.Id;
        //else
        //    $scope.modelObject.ItemApparelType = null;

        //if (typeof $scope.selectedWeavingDraft != 'undefined' && $scope.selectedWeavingDraft != null)
        //    $scope.modelObject.WeavingDraftId = $scope.selectedWeavingDraft.Id;
        //else
        //    $scope.modelObject.WeavingDraftId = null;



        if ($scope.modelObject.Id > 0)
            window.location.href = HOST_URL + 'KnitVariant?rid=' + 0, '&kid?=' + $scope.modelObject.Id;
        else
            toastr.error(resp.data.ErrorMessage, 'Hata');

    }
    $scope.creatKnit = function () {
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

        if (typeof $scope.selectedWeavingDraft != 'undefined' && $scope.selectedWeavingDraft != null)
            $scope.modelObject.WeavingDraftId = $scope.selectedWeavingDraft.Id;
        else
            $scope.modelObject.WeavingDraftId = null;

        $http.post(HOST_URL + 'Knit/creatKnit', $scope.modelObject, 'json').then(function (resp) {
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

    $scope.bindModel = function (id, kid) {
        $http.get(HOST_URL + 'KnitVariant/BindModel?rid=' + id, '&kid=' + kid, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;
                    // BIND EXTERNAL TYPES
                    if ($scope.modelObject.WeavingDraftId > 0)
                        $scope.selectedWeavingDraft = $scope.weavingDraftList.find(d => d.Id == $scope.modelObject.WeavingDraftId);
                    else
                        $scope.selectedWeavingDraft = {};

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
                    return $scope.modelObject.KnitYarns.filter(word => word.YarnType == 2);;
                },
                update: function (key, values) {
                    var obj = $scope.modelObject.KnitYarns.find(d => d.Id == key);
                    if (obj != null) {
                        let calculateRowAgain = false;

                        if (typeof values.YarnRecipeId != 'undefined') {
                            var yarnRecipeObj = $scope.yarnRecipeList.find(d => d.Id == values.YarnRecipeId);
                            obj.YarnRecipeId = yarnRecipeObj.Id;
                            obj.YarnRecipeCode = yarnRecipeObj.YarnRecipeCode;
                            obj.YarnRecipeName = yarnRecipeObj.YarnRecipeName;
                            obj.Denier = yarnRecipeObj.Denier;
                            obj.FirmName = yarnRecipeObj.FirmName;
                            calculateRowAgain = true;
                        }

                        if (typeof values.ReportWireCount != 'undefined') { obj.ReportWireCount = values.ReportWireCount; calculateRowAgain = true; }
                        if (typeof values.MeterWireCount != 'undefined') { obj.MeterWireCount = values.MeterWireCount; calculateRowAgain = true; }
                        if (typeof values.Gramaj != 'undefined') { obj.Gramaj = values.Gramaj; calculateRowAgain = true; }
                        if (typeof values.Density != 'undefined') { obj.Density = values.Density; calculateRowAgain = true; }
                        if (calculateRowAgain)
                            $scope.calculateWeftRow(obj);
                    }
                },
                remove: function (key) {
                    var obj = $scope.modelObject.KnitYarns.find(d => d.Id == key);
                    if (obj != null) {
                        $scope.modelObject.KnitYarns.splice($scope.modelObject.KnitYarns.indexOf(obj), 1);
                        $scope.calculatorCrudeGramaj();
                    }
                },
                insert: function (values) {
                    var newId = 1;

                    if ($scope.modelObject.KnitYarns.length > 0) {
                        newId = $scope.modelObject.KnitYarns.map(d => d.Id).reduce((max, n) => n > max ? n : max)
                        newId++;
                    }

                    var yarnRecipeObj = $scope.yarnRecipeList.find(d => d.Id == values.YarnRecipeId);

                    var newObj = {
                        Id: newId,
                        YarnRecipeId: yarnRecipeObj.Id,
                        YarnRecipeCode: yarnRecipeObj.YarnRecipeCode,
                        YarnRecipeName: yarnRecipeObj.YarnRecipeName,
                        Denier: yarnRecipeObj.Denier,
                        YarnType: 2,
                        FirmId: yarnRecipeObj.FirmId,
                        FirmName: yarnRecipeObj != null ? yarnRecipeObj.FirmName : '',
                        ReportWireCount: values.ReportWireCount,
                        MeterWireCount: values.MeterWireCount,
                        Gramaj: values.Gramaj,
                        Density: values.Density,
                        NewDetail: true
                    };

                    $scope.modelObject.KnitYarns.push(newObj);
                    $scope.calculateWeftRow(newObj);

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
            height: 300,
            editing: { allowUpdating: true, allowDeleting: true, allowAdding: true, mode: 'cell' },
            onInitNewRow: function (e) { },
            columns: [
                {
                    caption: '#',
                    cellTemplate: function (cellElement, cellInfo) {
                        cellElement.text(cellInfo.row.rowIndex)
                    },
                    width: 30,
                },
                {
                    dataField: 'YarnRecipeId', caption: 'İplik Kod',
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
                    , width: 200
                },
                { dataField: 'YarnRecipeName', caption: 'İplik Ad', allowEditing: false, width: 350 },
                { dataField: 'Denier', caption: 'Fiili Denye', allowEditing: false, width: 100 },
                { dataField: 'FirmName', caption: 'Firma', allowEditing: false, width: 150 },
                { dataField: 'ReportWireCount', caption: 'Rapor Tel Say.', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, validationRules: [{ type: "required" }], width: 100 },
                { dataField: 'MeterWireCount', caption: 'Metre Tel Say.', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, width: 100 },
                { dataField: 'Gramaj', caption: 'Gramaj', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, width: 100 },
                { dataField: 'Density', caption: 'Sıklık', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, width: 100 },
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
                        let calculateRowAgain = false;

                        if (typeof values.YarnRecipeId != 'undefined') {
                            var yarnRecipeObj = $scope.yarnRecipeList.find(d => d.Id == values.YarnRecipeId);
                            obj.YarnRecipeId = yarnRecipeObj.Id;
                            obj.YarnRecipeCode = yarnRecipeObj.YarnRecipeCode;
                            obj.YarnRecipeName = yarnRecipeObj.YarnRecipeName;
                            obj.Denier = yarnRecipeObj.Denier;
                            obj.FirmName = yarnRecipeObj.FirmName;
                            calculateRowAgain = true;

                        }

                        if (typeof values.ReportWireCount != 'undefined') { obj.ReportWireCount = values.ReportWireCount; calculateRowAgain = true; }
                        if (typeof values.MeterWireCount != 'undefined') { obj.MeterWireCount = values.MeterWireCount; calculateRowAgain = true; }
                        if (typeof values.Gramaj != 'undefined') { obj.Gramaj = values.Gramaj; calculateRowAgain = true; }
                        if (typeof values.Density != 'undefined') { obj.Density = values.Density; calculateRowAgain = true; }
                        if (calculateRowAgain)
                            $scope.calculateWarpRow(obj);
                    }
                },
                remove: function (key) {
                    var obj = $scope.modelObject.KnitYarns.find(d => d.Id == key);
                    if (obj != null) {
                        $scope.modelObject.KnitYarns.splice($scope.modelObject.KnitYarns.indexOf(obj), 1);
                        $scope.calculatorCrudeGramaj();
                    }
                },
                insert: function (values) {
                    var newId = 1;

                    if ($scope.modelObject.KnitYarns.length > 0) {
                        newId = $scope.modelObject.KnitYarns.map(d => d.Id).reduce((max, n) => n > max ? n : max)
                        newId++;
                    }

                    var yarnRecipeObj = $scope.yarnRecipeList.find(d => d.Id == values.YarnRecipeId);
                    var newObj = {
                        Id: newId,
                        YarnRecipeId: yarnRecipeObj.Id,
                        YarnRecipeCode: yarnRecipeObj.YarnRecipeCode,
                        YarnRecipeName: yarnRecipeObj.YarnRecipeName,
                        Denier: yarnRecipeObj.Denier,
                        YarnType: 1,
                        FirmId: yarnRecipeObj.FirmId,
                        FirmName: yarnRecipeObj != null ? yarnRecipeObj.FirmName : '',
                        ReportWireCount: values.ReportWireCount,
                        MeterWireCount: values.MeterWireCount,
                        Gramaj: values.Gramaj,
                        Density: values.Density,
                        NewDetail: true
                    };
                    $scope.modelObject.KnitYarns.push(newObj);
                    $scope.calculateWarpRow(newObj);

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
            height: 300,
            editing: { allowUpdating: true, allowDeleting: true, allowAdding: true, mode: 'cell' },
            onInitNewRow: function (e) { },
            columns: [
                {
                    caption: '#',
                    cellTemplate: function (cellElement, cellInfo) {
                        cellElement.text(cellInfo.row.rowIndex)
                    },
                    width: 30,
                },
                {
                    dataField: 'YarnRecipeId', caption: 'İplik Kod',
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
                    , width: 200
                },
                { dataField: 'YarnRecipeName', caption: 'İplik Ad', allowEditing: false, width: 350 },
                { dataField: 'Denier', caption: 'Fiili Denye', allowEditing: false, width: 100 },
                { dataField: 'FirmName', caption: 'Firma', allowEditing: false, width: 150 },
                { dataField: 'ReportWireCount', caption: 'Rapor Tel Say.', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, validationRules: [{ type: "required" }], width: 100 },
                { dataField: 'MeterWireCount', caption: 'Metre Tel Say.', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, width: 100 },
                { dataField: 'Gramaj', caption: 'Gramaj', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, width: 100 },
                { dataField: 'Density', caption: 'Sıklık', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, width: 100 },
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
    $scope.calculateWarpRow = function (row) {

        row.MeterWireCount = parseInt(row.ReportWireCount * 100 / $scope.modelObject.WarpReportLength);
        row.Gramaj = row.MeterWireCount * row.Denier * $scope.modelObject.CombWidth / 900000;
        $scope.calculatorCrudeGramaj();
        $scope.calculatorWarpDensity();
    }
    $scope.calculateWeftRow = function (row) {
        row.MeterWireCount = parseInt(row.ReportWireCount * 100 / $scope.modelObject.WeftReportLength);
        row.Gramaj = row.MeterWireCount * row.Denier * $scope.modelObject.CombWidth / 900000;
        $scope.calculatorCrudeGramaj();
        $scope.calculatorWeftDensity();
    }
    $scope.calculatorCrudeGramaj = function () {
        let crudeGramaj = 0;
        $scope.modelObject.KnitYarns.forEach(element => { crudeGramaj += parseFloat(element.Gramaj != null ? element.Gramaj : 0); });
        $scope.modelObject.CrudeGramaj = crudeGramaj;
        let meterGramaj = 0;
        meterGramaj = crudeGramaj / ($scope.modelObject.CombWidth / 100);
        $scope.modelObject.MeterGramaj = meterGramaj;
        $scope.calculatorWeftDensity();
        $scope.calculatorWarpDensity();

    }
    $scope.calculatorWarpDensity = function () {
        let averageWarpDensity = 0;
        let count = 0;
        $scope.modelObject.KnitYarns.filter(d => d.YarnType == 1).forEach(element => { averageWarpDensity += parseFloat(element.Density != null ? element.Density : 0); element.Density != null ? count++ : count; });
        $scope.modelObject.AverageWarpDensity = parseInt(averageWarpDensity / count);
    }
    $scope.calculatorWeftDensity = function () {
        let averageWeftDensity = 0;
        let count = 0;
        $scope.modelObject.KnitYarns.filter(d => d.YarnType == 2).forEach(element => { averageWeftDensity += parseFloat(element.Density != null ? element.Density : 0); element.Density != null ? count++ : count; });
        $scope.modelObject.AverageWeftDensity = parseInt(averageWeftDensity / count);
    }
    // ON LOAD EVENTS
    DevExpress.localization.locale('tr');
    $scope.loadSelectables().then(function (data) {
        if (PRM_ID > 0)
            $scope.bindModel(PRM_ID);
        else {
            $scope.getNextAttemptNo().then(function (rNo) {
                $scope.modelObject.AttemptNo = rNo;
                $scope.$apply();
            });
            $scope.bindModel(0);
        }
    });
});
app.controller('variantCtrl', function ($scope, $http) {
    $scope.modelObject = { Id: 0, KnitYarns: [] };
    $scope.saveStatus = 0;

    $scope.yarnRecipeList = [];
    $scope.firmList = [];

    $scope.selectedWeavingDraft = {};
    $scope.weavingDraftList = [];

    $scope.selectedQualityType = {};
    $scope.qualityTypeList = [];
    $scope.itemVariantList = [];

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
                        $scope.weavingDraftList = resp.data.WeavingDrafts;
                        $scope.yarnRecipeList = resp.data.YarnRecipes;
                        $scope.firmList = resp.data.Firms;
                        $scope.itemVariantList = resp.data.ItemVariants;
                        console.log($scope.itemVariantList);
                        resolve(resp.data);
                    }
                }).catch(function (err) { });
        });

        return prmReq;
    }
    // ON LOAD EVENTS
    DevExpress.localization.locale('tr');
    $scope.loadSelectables().then(function (data) {
        if (PRM_ID > 0)
            $scope.bindModel(PRM_ID);
        else {
            $scope.getNextAttemptNo().then(function (rNo) {
                $scope.modelObject.AttemptNo = rNo;
                $scope.$apply();
            });
            $scope.bindModel(0);
        }
    });
});