app.controller('driverAccountCtrl', function ($scope, $http) {
    $scope.modelObject = { Id: 0, DriverAccountDetails: [] };

    $scope.saveStatus = 0;
    $scope.forexTypeList = [];
    $scope.driverList = [];
    $scope.countryList = [];
    $scope.costCategoryList = [];
    $scope.unitTypeList = [];
    $scope.voyageList = [];
    $scope.towingVehicleList = [];

    $scope.selectedForexType = {};
    $scope.selectedCity = {};
    $scope.selectedCountry = {};

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
            $http.get(HOST_URL + 'VoyageCost/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.forexTypeList = resp.data.ForexTypes;
                        $scope.unitTypeList = resp.data.UnitTypes;
                        $scope.driverList = resp.data.Drivers;
                        $scope.countryList = resp.data.Countrys;
                        $scope.costCategoryList = resp.data.CostCategorys;
                        $scope.towingVehicleList = resp.data.TowingVehicles;
                        resolve();
                    }
                }).catch(function (err) { });
        });

        return prms;
    }

    $scope.saveModel = function () {
        $scope.saveStatus = 1;

        //if (typeof $scope.selectedForexType != 'undefined' && $scope.selectedForexType != null)
        //    $scope.modelObject.ForexTypeId = $scope.selectedForexType.Id;
        //else
        //    $scope.modelObject.ForexTypeId = null;

        $http.post(HOST_URL + 'VoyageCost/SaveModel', $scope.modelObject, 'json')
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
        $http.get(HOST_URL + 'DriverAccount/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;

                    // BIND EXTERNAL FIRM TYPE
                    //if ($scope.modelObject.FirmType > 0) {
                    //    $scope.selectedFirmType = $scope.firmTypeList.find(d => d.Id == $scope.modelObject.FirmType);
                    //}
                    //else 
                    //    $scope.selectedFirmType = {};


                    $scope.bindDriverAccountDetailList();

                }
            }).catch(function (err) { });
    }
    $scope.dropDownBoxEditorTowingVehicleTemplate = function (cellElement, cellInfo) {
        return $("<div>").dxDropDownBox({
            dropDownOptions: { width: 600 },
            dataSource: $scope.towingVehicleList,
            value: cellInfo.value,
            valueExpr: "Id",
            displayExpr: "Plate",
            contentTemplate: function (e) {
                return $("<div>").dxDataGrid({
                    dataSource: $scope.towingVehicleList,
                    remoteOperations: true,
                    columns: [
                        { dataField: 'Plate', caption: 'Ad' },
                        { dataField: 'Mark', caption: 'Marka' },
                        { dataField: 'Versiyon', caption: 'Model' }
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
                    allowColumnResizing: true,
                    wordWrapEnabled: true,
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

    $scope.bindDriverAccountDetailList = function () {
        $('#driverAccountDetailList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $scope.modelObject.DriverAccountDetails;
                },
                update: function (key, values) {
                    var obj = $scope.modelObject.DriverAccountDetails.find(d => d.Id == key);
                    if (obj != null) {
                        if (typeof values.OverallTotal != 'undefined') { obj.OverallTotal = values.OverallTotal; }
                        if (typeof values.Quantity != 'undefined') { obj.Quantity = values.Quantity; }
                        if (typeof values.OperationDateStr != 'undefined') { obj.OperationDateStr = values.OperationDateStr; }
                        if (typeof values.PayType != 'undefined') { obj.PayType = values.PayType; }
                        if (typeof values.KmHour != 'undefined') { obj.KmHour = values.KmHour; }
                        if (typeof values.CountryId != 'undefined') {
                            var countryObj = $scope.countryList.find(d => d.Id == values.CountryId);
                            obj.CountryId = countryObj.Id;
                        }
                        if (typeof values.CostCategoryId != 'undefined') {
                            var costCategoryObj = $scope.costCategoryList.find(d => d.Id == values.CostCategoryId);
                            obj.CostCategoryId = costCategoryObj.Id;
                        }
                        if (typeof values.UnitTypeId != 'undefined') {
                            var unitTypeObj = $scope.unitTypeList.find(d => d.Id == values.UnitTypeId);
                            obj.UnitTypeId = unitTypeObj.Id;
                        }
                        if (typeof values.TowingVehicleId != 'undefined') {
                            var tVehicleObj = $scope.towingVehicleList.find(d => d.Id == values.TowingVehicleId)
                            obj.TowingVehicleId = tVehicleObj.Id;
                        }
                        if (typeof values.TowingVehicleId != 'undefined') {
                            var tVehicleObj = $scope.towingVehicleList.find(d => d.Id == values.TowingVehicleId)
                            obj.TowingVehicleId = tVehicleObj.Id;
                        }
                    }
                },
                remove: function (key) {
                    var obj = $scope.modelObject.DriverAccountDetails.find(d => d.Id == key);
                    if (obj != null) {
                        $scope.modelObject.DriverAccountDetails.splice($scope.modelObject.DriverAccountDetails.indexOf(obj), 1);
                    }
                },
                insert: function (values) {
                    var newId = 1;
                    if ($scope.modelObject.DriverAccountDetails.length > 0) {
                        newId = $scope.modelObject.DriverAccountDetails.map(d => d.Id).reduce((max, n) => n > max ? n : max)
                        newId++;
                    }

                    var newObj = {
                        Id: newId,
                        DriverId: values.DriverId,
                        CountryId: values.CountryId,
                        CostCategoryId: values.CostCategoryId,
                        OperationDate: values.OperationDateStr,
                        OperationDateStr: values.OperationDateStr,
                        Quantity: values.Quantity,
                        UnitTypeId: values.UnitTypeId,
                        OverallTotal: values.OverallTotal,
                        ForexTypeId: values.ForexTypeId,
                        PayType: values.PayType,
                        TowingVehicleId: values.TowingVehicleId,
                        KmHour: values.KmHour,
                        NewDetail: true
                    };

                    $scope.modelObject.DriverAccountDetails.push(newObj);
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
                visible: true
            },
            scrolling: {
                mode: "virtual"
            },
            height: 500,
            editing: {
                allowUpdating: false,
                allowDeleting: false,
                allowAdding: false,
                mode: 'cell'
            },
            columns: [
                { dataField: 'Id', caption: 'Id', visible: false, sortOrder: "desc" },
                { dataField: 'VoyageCode', caption: 'Sefer Kod' },
                { dataField: 'ActionTypeStr', caption: 'Aksiyon Tip', dataType: 'number', groupIndex: 0, },
                {
                    dataField: 'TowingVehicleId', caption: 'Çekici',
                    lookup: {
                        dataSource: $scope.towingVehicleList,
                        valueExpr: "Id",
                        displayExpr: "Plate"
                    },
                    allowSorting: true,
                    editCellTemplate: $scope.dropDownBoxEditorTowingVehicleTemplate,
                    cellTemplate: function (container, options) {
                        if (typeof options.row.data.Plate != 'undefined'
                            && options.row.data.Plate != null && options.row.data.Plate.length > 0)
                            container.text(options.row.data.Plate);
                        else
                            container.text(options.displayValue);
                    }
                },
                { dataField: 'KmHour', caption: 'Çekici KM', dataType: 'number' },
                {
                    dataField: 'CountryId', caption: 'Ülke',
                    allowSorting: false,
                    lookup: {
                        dataSource: $scope.countryList,
                        valueExpr: "Id",
                        displayExpr: "CountryName"
                    }
                },
                {
                    dataField: 'CostCategoryId', caption: 'Maliyet Kategori',
                    allowSorting: false,
                    lookup: {
                        dataSource: $scope.costCategoryList,
                        valueExpr: "Id",
                        displayExpr: "CostCategoryName"
                    }
                },
                { dataField: 'OperationDateStr', caption: 'İşlem Tarih', dataType: 'date', format: 'dd.MM.yyyy' },
                { dataField: 'Quantity', caption: 'Miktar', dataType: 'number' },
                {
                    dataField: 'UnitTypeId', caption: 'Birim',
                    allowSorting: false,
                    lookup: {
                        dataSource: $scope.unitTypeList,
                        valueExpr: "Id",
                        displayExpr: "UnitCode"
                    }
                },
                { dataField: 'OverallTotal', caption: 'Tutar', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'Explanation', caption: 'Açıklama', dataType: 'text' },

            ],
            summary: {
                groupItems: [ {
                    column: 'OverallTotal',
                    summaryType: 'sum',
                    displayFormat: 'Total: {0}',
                    showInGroupFooter: true,
                }],
            },
        });
    }

    // ON LOAD EVENTS
    DevExpress.localization.locale('tr');
    $scope.loadSelectables().then(function () {
        if (PRM_ID > 0)
            $scope.bindModel(PRM_ID);
        else
            $scope.bindModel(0);

    });
});