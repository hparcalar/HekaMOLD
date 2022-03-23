app.controller('voyageCtrl', function ($scope, $http) {
    $scope.modelObject = { Id: 0, VoayageDateStr: moment().format('DD.MM.YYYY'), VoyageDetails: [], VoyageDrivers: [], VoyageTowingVehicles: [] };

    $scope.driverList = [];
    $scope.plannedLoadList = [];
    $scope.rotaList = [];
    $scope.towingVehicleList = [];
    $scope.trailerVehicleList = [];
    $scope.firmList = [];
    $scope.cDoorList = [];
    $scope.cityList = [];
    $scope.countryList = [];
    $scope.customsList = [];
    $scope.waitingLoadList = [];

    $scope.selectedDriver = {}
    $scope.selectedTraillerVehicle = {}
    $scope.selectedOrderTransactionDirectionType = {};
    $scope.orderTransactionDirectionTypeList = [{ Id: 1, Text: 'İhracat' }, { Id: 2, Text: 'İthalat' },
    { Id: 3, Text: 'Yurt İçi' }, { Id: 4, Text: 'Transit' }];
    $scope.selectedCarrierFirm = {};
    $scope.selectedCDoorExtry = {}
    $scope.selectedCDoorExit = {}
    $scope.selectedTraillerType = {};
    $scope.selectedTowinfVehicle = {};

    $scope.selectedStartCity = {}
    $scope.selectedStartCountry = {}
    $scope.selectedStartAddress = {}

    $scope.selectedLoadCity = {}
    $scope.selectedLoadCountry = {}
    $scope.selectedLoadAddress = {}

    $scope.selectedDischargeCity = {}
    $scope.selectedDischargeCountry = {}
    $scope.selectedDischargeAddress = {}

    $scope.selectedEntryCustoms = {}
    $scope.selectedExitCustoms = {}

    $scope.traillerTypeList = [{ Id: 1, Text: 'Çadırlı' },
    { Id: 2, Text: 'Frigo' }, { Id: 3, Text: 'Kapalı Kasa' }, { Id: 4, Text: 'Optima' }, { Id: 5, Text: 'Mega' }
        , { Id: 6, Text: 'Konteyner' }, { Id: 7, Text: 'Swapboddy' }, { Id: 8, Text: 'Lowbed' }
        , { Id: 9, Text: 'Kamyon Romörk' }, { Id: 10, Text: 'Standart' }, { Id: 10, Text: 'Minivan' }];

    $scope.selectedVoyageStatus = { Id: 0 };
    $scope.voyageStatusList = [{ Id: 2, Text: 'Hazır Bekliyor' }, { Id: 3, Text: 'Depoda' }, { Id: 4, Text: 'Müşteriden Yüklendi' },
    { Id: 5, Text: 'Yurtiçi Gümrükte' }, { Id: 6, Text: 'Kapıkulede' }, { Id: 7, Text: 'Yurtdışı Yolda' }, { Id: 8, Text: 'Yurtdışı Gümrükte' }, { Id: 9, Text: 'Boşaltmada' },
    { Id: 10, Text: 'Boşaltıldı' }, { Id: 11, Text: 'Yüklemede' }, { Id: 12, Text: 'Yüklendi' }, { Id: 13, Text: 'Tamamlandı' }];

    $scope.saveStatus = 0;

    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0, VoyageDateStr: moment().format('DD.MM.YYYY'), VoyageDetails: [], VoyageDrivers: [], VoyageTowingVehicles: [] };

        $scope.selectedDriver = {}
        $scope.selectedTraillerVehicle = {}
        $scope.selectedOrderTransactionDirectionType = {};

        $scope.selectedStartCity = {}
        $scope.selectedStartCountry = {}
        $scope.selectedStartCity = {}
        $scope.selectedStartCountry = {}

        $scope.selectedLoadCity = {}
        $scope.selectedLoadCountry = {}

        $scope.selectedDischargeCity = {}
        $scope.selectedDischargeCountry = {}

        $scope.selectedEntryCustoms = {}
        $scope.selectedExitCustoms = {}
        $scope.bindVoyageDetails();

    }
    $scope.loadSelectables = function () {
        var prmReq = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Voyage/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.towingVehicleList = resp.data.TowinfVehicles;
                        $scope.trailerVehicleList = resp.data.TrailerVehicles;
                        $scope.driverList = resp.data.Drivers;
                        $scope.VoyageDateStr = moment().format('DD.MM.YYYY')
                        $scope.rotaList = resp.data.Rotas;
                        $scope.firmList = resp.data.Firms;
                        $scope.cDoorList = resp.data.CDoors;
                        $scope.cityList = resp.data.Citys;
                        $scope.countryList = resp.data.Countrys;
                        $scope.customsList = resp.data.Customs;
                        $scope.waitingLoadList = resp.data.WaitingLoads;

                        resolve(resp.data);
                    }
                }).catch(function (err) { });
        });

        return prmReq;
    }
    $scope.getNextRecord = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Voyage/GetNextRecord?Id=' + $scope.modelObject.Id, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        if (resp.data.Result) {
                            window.location.href = HOST_URL + 'Voyage?rid=' + resp.data.NextNo;
                        }
                        else {
                            toastr.warning('Sıradaki sefer numarasına ulaşılamadı. Lütfen ekranı yenileyip tekrar deneyiniz.', 'Uyarı');
                            resolve('');
                        }
                    }
                }).catch(function (err) { });
        });

        return prms;
    }
    $scope.getBackRecord = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Voyage/GetBackRecord?Id=' + $scope.modelObject.Id, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        if (resp.data.Result) {
                            window.location.href = HOST_URL + 'Voyage?rid=' + resp.data.NextNo;
                        }
                        else {
                            toastr.warning('Sıradaki sefer numarasına ulaşılamadı. Lütfen ekranı yenileyip tekrar deneyiniz.', 'Uyarı');
                            resolve('');
                        }
                    }
                }).catch(function (err) { });
        });

        return prms;
    }
    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu siparişi silmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'Voyage/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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

        if (typeof $scope.selectedCarrierFirm != 'undefined' && $scope.selectedCarrierFirm != null)
            $scope.modelObject.CarrierFirmId = $scope.selectedCarrierFirm.Id;
        else
            $scope.modelObject.CarrierFirmId = null;

        if (typeof $scope.selectedCDoorEntry != 'undefined' && $scope.selectedCDoorEntry != null)
            $scope.modelObject.CustomsDoorEntryId = $scope.selectedCDoorEntry.Id;
        else
            $scope.modelObject.CustomsDoorEntryId = null;

        if (typeof $scope.selectedCDoorExit != 'undefined' && $scope.selectedCDoorExit != null)
            $scope.modelObject.CustomsDoorExitId = $scope.selectedCDoorExit.Id;
        else
            $scope.modelObject.CustomsDoorExitId = null;

        if (typeof $scope.selectedTraillerVehicle != 'undefined' && $scope.selectedTraillerVehicle != null)
            $scope.modelObject.TraillerVehicleId = $scope.selectedTraillerVehicle.Id;
        else
            $scope.modelObject.TraillerVehicleId = null;

        if (typeof $scope.selectedTowinfVehicle != 'undefined' && $scope.selectedTowinfVehicle != null)
            $scope.modelObject.TowinfVehicleId = $scope.selectedTowinfVehicle.Id;
        else
            $scope.modelObject.TowinfVehicleId = null;

        if (typeof $scope.selectedTraillerType != 'undefined' && $scope.selectedTraillerType != null)
            $scope.modelObject.TraillerType = $scope.selectedTraillerType.Id;
        else
            $scope.modelObject.TraillerType = null;

        if (typeof $scope.selectedDriver != 'undefined' && $scope.selectedDriver != null)
            $scope.modelObject.DriverId = $scope.selectedDriver.Id;
        else
            $scope.modelObject.DriverId = null;

        if (typeof $scope.selectedOrderTransactionDirectionType != 'undefined' && $scope.selectedOrderTransactionDirectionType != null)
            $scope.modelObject.OrderTransactionDirectionType = $scope.selectedOrderTransactionDirectionType.Id;
        else
            $scope.modelObject.OrderTransactionDirectionType = null;

        if (typeof $scope.selectedDischargeCity != 'undefined' && $scope.selectedDischargeCity != null)
            $scope.modelObject.DischargeCityId = $scope.selectedDischargeCity.Id;
        else
            $scope.modelObject.DischargeCityId = null;

        if (typeof $scope.selectedForexType != 'undefined' && $scope.selectedForexType != null)
            $scope.modelObject.ForexTypeId = $scope.selectedForexType.Id;
        else
            $scope.modelObject.ForexTypeId = null;

        if (typeof $scope.selectedStartCity != 'undefined' && $scope.selectedStartCity != null)
            $scope.modelObject.StartCityId = $scope.selectedStartCity.Id;
        else
            $scope.modelObject.StartCityId = null;

        if (typeof $scope.selectedStartCountry != 'undefined' && $scope.selectedStartCountry != null)
            $scope.modelObject.StartCountryId = $scope.selectedStartCountry.Id;
        else
            $scope.modelObject.StartCountryId = null;

        if (typeof $scope.selectedLoadCity != 'undefined' && $scope.selectedLoadCity != null)
            $scope.modelObject.LoadCityId = $scope.selectedLoadCity.Id;
        else
            $scope.modelObject.LoadCityId = null;

        if (typeof $scope.selectedLoadCountry != 'undefined' && $scope.selectedLoadCountry != null)
            $scope.modelObject.LoadCountryId = $scope.selectedLoadCountry.Id;
        else
            $scope.modelObject.LoadCountryId = null;

        if (typeof $scope.selectedDischargeCity != 'undefined' && $scope.selectedDischargeCity != null)
            $scope.modelObject.DischargeCityId = $scope.selectedDischargeCity.Id;
        else
            $scope.modelObject.DischargeCityId = null;

        if (typeof $scope.selectedDischargeCountry != 'undefined' && $scope.selectedDischargeCountry != null)
            $scope.modelObject.DischargeCountryId = $scope.selectedDischargeCountry.Id;
        else
            $scope.modelObject.DischargeCountryId = null;

        if (typeof $scope.selectedEntryCustoms != 'undefined' && $scope.selectedEntryCustoms != null)
            $scope.modelObject.EntryCustomsId = $scope.selectedEntryCustoms.Id;
        else
            $scope.modelObject.EntryCustomsId = null;

        if (typeof $scope.selectedExitCustoms != 'undefined' && $scope.selectedExitCustoms != null)
            $scope.modelObject.ExitCustomsId = $scope.selectedExitCustoms.Id;
        else
            $scope.modelObject.ExitCustomsId = null;

        if (typeof $scope.selectedVoyageStatus != 'undefined' && $scope.selectedVoyageStatus != null)
            $scope.modelObject.VoyageStatus = $scope.selectedVoyageStatus.Id;
        else
            $scope.modelObject.VoyageStatus = null;

        $http.post(HOST_URL + 'LPlanning/SaveModel', $scope.modelObject, 'json')
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
            dropDownOptions: { width: 600 },
            dataSource: $scope.itemList,
            value: cellInfo.value,
            valueExpr: "Id",
            displayExpr: "ItemNo",
            contentTemplate: function (e) {
                return $("<div>").dxDataGrid({
                    dataSource: $scope.itemList,
                    remoteOperations: true,
                    columns: [
                        { dataField: 'ItemNo', caption: 'Mal Kodu' },
                        { dataField: 'ItemName', caption: 'Mal Adı' },
                        { dataField: 'ItemTypeStr', caption: 'Mal Türü' },
                        { dataField: 'GroupName', caption: 'Grup' },
                        { dataField: 'CategoryName', caption: 'Kategori' }
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
    $scope.dropDownBoxEditorWaitingLoadTemplate = function (cellElement, cellInfo) {
        return $("<div>").dxDropDownBox({
            dropDownOptions: { width: 1200 },
            dataSource: $scope.waitingLoadList,
            value: cellInfo.value,
            valueExpr: "Id",
            displayExpr: "LoadCode",
            contentTemplate: function (e) {
                return $("<div>").dxDataGrid({
                    dataSource: $scope.waitingLoadList,
                    remoteOperations: true,
                    columns: [
                        { dataField: 'LoadCode', caption: 'Yük Kodu' },
                        { dataField: 'CustomerFirmName', caption: 'Müşteri' },
                        { dataField: 'ShipperFirmName', caption: 'Gönderici Firma' },
                        { dataField: 'BuyerFirmName', caption: 'Alıcı Firma' },
                        { dataField: 'OveralQuantity', caption: 'Toplam Miktar' },
                        { dataField: 'OveralWeight', caption: 'Toplam Ağırlık(KG)' },
                        { dataField: 'DischargeLineNo', caption: 'Boşaltma Sırası' },
                        { dataField: 'BuyerCityName', caption: 'Boş. Şehri' },
                        { dataField: 'BuyerCountryName', caption: 'Boş. Ülke' },
                    ],
                    hoverStateEnabled: true,
                    keyExpr: "Id",
                    scrolling: { mode: "virtual" },
                    columnAutoWidth: true,
                    height: 500,
                    width: 1200,
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
    $scope.dropDownBoxEditorDriverTemplate = function (cellElement, cellInfo) {
        return $("<div>").dxDropDownBox({
            dropDownOptions: { width: 600 },
            dataSource: $scope.driverList,
            value: cellInfo.value,
            valueExpr: "Id",
            displayExpr: "DriverName",
            contentTemplate: function (e) {
                return $("<div>").dxDataGrid({
                    dataSource: $scope.driverList,
                    remoteOperations: true,
                    columns: [
                        { dataField: 'DriverName', caption: 'Ad' },
                        { dataField: 'DriverSurName', caption: 'Soyad' }
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
                        { dataField: 'Plate', caption: 'Plaka' },
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


    $scope.bindModel = function (id) {
        $http.get(HOST_URL + 'Voyage/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;

                    if (typeof $scope.modelObject.CarrierFirmId != 'undefined' && $scope.modelObject.CarrierFirmId != null)
                        $scope.selectedCarrierFirm = $scope.firmList.find(d => d.Id == $scope.modelObject.CarrierFirmId);
                    else
                        $scope.selectedCarrierFirm = {};

                    if ($scope.modelObject.CustomsDoorEntryId > 0)
                        $scope.selectedCDoorEntry = $scope.cDoorList.find(d => d.Id == $scope.modelObject.CustomsDoorEntryId);
                    else
                        $scope.selectedCDoorEntry = {};

                    if ($scope.modelObject.CustomsDoorExitId > 0)
                        $scope.selectedCDoorExit = $scope.cDoorList.find(d => d.Id == $scope.modelObject.CustomsDoorExitId);
                    else
                        $scope.selectedCDoorExit = {};

                    if ($scope.modelObject.DriverId > 0)
                        $scope.selectedDriver = $scope.driverList.find(d => d.Id == $scope.modelObject.DriverId);
                    else
                        $scope.selectedDriver = {};

                    if ($scope.modelObject.TraillerVehicleId > 0)
                        $scope.selectedTraillerVehicle = $scope.trailerVehicleList.find(d => d.Id == $scope.modelObject.TraillerVehicleId);
                    else
                        $scope.selectedTraillerVehicle = {};


                    if ($scope.modelObject.TowinfVehicleId > 0)
                        $scope.selectedTowinfVehicle = $scope.towingVehicleList.find(d => d.Id == $scope.modelObject.TowinfVehicleId);
                    else
                        $scope.selectedTowinfVehicle = {};

                    if ($scope.modelObject.TraillerType > 0)
                        $scope.selectedTraillerType = $scope.traillerTypeList.find(d => d.Id == $scope.modelObject.TraillerType);
                    else
                        $scope.selectedTraillerType = {};

                    if ($scope.modelObject.OrderTransactionDirectionType > 0)
                        $scope.selectedOrderTransactionDirectionType = $scope.orderTransactionDirectionTypeList.find(d => d.Id == $scope.modelObject.OrderTransactionDirectionType);
                    else
                        $scope.selectedOrderTransactionDirectionType = {};

                    if ($scope.modelObject.DischargeCityId > 0)
                        $scope.selectedDischargeCity = $scope.cityList.find(d => d.Id == $scope.modelObject.DischargeCityId);
                    else
                        $scope.selectedDischargeCity = {};

                    if (typeof $scope.modelObject.ForexTypeId != 'undefined' && $scope.modelObject.ForexTypeId != null)
                        $scope.selectedForexType = $scope.forexList.find(d => d.Id == $scope.modelObject.ForexTypeId);
                    else
                        $scope.selectedForexType = {};

                    if (typeof $scope.modelObject.StartCityId != 'undefined' && $scope.modelObject.StartCityId != null)
                        $scope.selectedStartCity = $scope.cityList.find(d => d.Id == $scope.modelObject.StartCityId);
                    else
                        $scope.selectedStartCity = {};

                    if (typeof $scope.modelObject.LoadCityId != 'undefined' && $scope.modelObject.LoadCityId != null)
                        $scope.selectedLoadCity = $scope.cityList.find(d => d.Id == $scope.modelObject.LoadCityId);
                    else
                        $scope.selectedLoadCity = {};

                    if (typeof $scope.modelObject.DischargeCityId != 'undefined' && $scope.modelObject.DischargeCityId != null)
                        $scope.selectedDischargeCity = $scope.cityList.find(d => d.Id == $scope.modelObject.DischargeCityId);
                    else
                        $scope.selectedDischargeCity = {};

                    if (typeof $scope.modelObject.StartCountryId != 'undefined' && $scope.modelObject.StartCountryId != null)
                        $scope.selectedStartCountry = $scope.countryList.find(d => d.Id == $scope.modelObject.StartCountryId);
                    else
                        $scope.selectedStartCountry = {};

                    if (typeof $scope.modelObject.LoadCountryId != 'undefined' && $scope.modelObject.LoadCountryId != null)
                        $scope.selectedLoadCountry = $scope.countryList.find(d => d.Id == $scope.modelObject.LoadCountryId);
                    else
                        $scope.selectedLoadCountry = {};

                    if (typeof $scope.modelObject.DischargeCountryId != 'undefined' && $scope.modelObject.DischargeCountryId != null)
                        $scope.selectedDischargeCountry = $scope.countryList.find(d => d.Id == $scope.modelObject.DischargeCountryId);
                    else
                        $scope.selectedDischargeCountry = {};

                    if (typeof $scope.modelObject.ExitCustomsId != 'undefined' && $scope.modelObject.ExitCustomsId != null)
                        $scope.selectedExitCustoms = $scope.customsList.find(d => d.Id == $scope.modelObject.ExitCustomsId);
                    else
                        $scope.selectedExitCustoms = {};

                    if (typeof $scope.modelObject.EntryCustomsId != 'undefined' && $scope.modelObject.EntryCustomsId != null)
                        $scope.selectedEntryCustoms = $scope.customsList.find(d => d.Id == $scope.modelObject.EntryCustomsId);
                    else
                        $scope.selectedEntryCustoms = {};

                    if (typeof $scope.modelObject.VoyageStatus != 'undefined' && $scope.modelObject.VoyageStatus != null)
                        $scope.selectedVoyageStatus = $scope.voyageStatusList.find(d => d.Id == $scope.modelObject.VoyageStatus);
                    else
                        $scope.selectedVoyageStatus = {};

                    $scope.bindVoyageDetails();
                    $scope.bindVoyageDrivers();
                }
            }).catch(function (err) { });
    }

    $scope.bindVoyageDetails = function () {
        $('#plannedLoadList').dxDataGrid({
            dataSource:
            {
                load: function () { return $scope.modelObject.VoyageDetails },
                remove: function (key) {
                    var obj = $scope.modelObject.VoyageDetails.find(d => d.Id == key);
                    if (obj != null) {
                        $scope.modelObject.VoyageDetails.splice($scope.modelObject.VoyageDetails.indexOf(obj), 1);
                    }
                },
                update: function (key, values) {
                    var obj = $scope.modelObject.VoyageDetails.find(d => d.Id == key);
                    if (obj != null) {
                        if (typeof values.LoadingLineNo != 'undefined') { obj.LoadingLineNo = values.LoadingLineNo; }
                        if (typeof values.DischargeLineNo != 'undefined') { obj.DischargeLineNo = values.DischargeLineNo; }
                    }
                },
                insert: function (values) {
                    var newId = 1;
                    if ($scope.modelObject.VoyageDetails.length > 0) {
                        newId = $scope.modelObject.VoyageDetails.map(d => d.Id).reduce((max, n) => n > max ? n : max)
                        newId++;
                    }

                    var loadObj = $scope.waitingLoadList.find(d => d.Id == values.LoadCode);

                    var newObj = {
                        Id: newId,
                        DischargeLineNo: newId,
                        LoadingLineNo: loadObj.LoadingLineNo,
                        ItemLoadId: loadObj.Id,
                        LoadCode: loadObj.LoadCode,
                        LoadingDateStr: loadObj.LoadingDateStr,
                        LoadOutDate: loadObj.LoadOutDateStr,
                        CustomerFirmName: loadObj.CustomerFirmName,
                        ShipperFirmName: loadObj.ShipperFirmName,
                        BuyerFirmName: loadObj.BuyerFirmName,
                        OrderTransactionDirectionType: loadObj.OrderTransactionDirectionType,
                        OrderTransactionDirectionTypeStr: loadObj.OrderTransactionDirectionTypeStr,
                        OrderCalculationType: loadObj.OrderCalculationType,
                        OrderCalculationTypeStr: loadObj.OrderCalculationTypeStr,
                        OveralQuantity: loadObj.OveralQuantity,
                        OveralWeight: loadObj.OveralWeight,
                        OveralLadametre: loadObj.OveralLadametre,
                        OveralVolume: loadObj.OveralVolume,
                        OverallTotal: loadObj.OverallTotal,
                        OrderNo: loadObj.OrderNo,
                        LoadDate: loadObj.LoadDateStr,
                        DischargeDate: loadObj.DischargeDateStr,
                        CalculationTypePrice: loadObj.CalculationTypePrice,
                        DocumentNo: loadObj.DocumentNo,
                        OrderUploadType: loadObj.OrderUploadType,
                        OrderUploadTypeStr: loadObj.OrderUploadTypeStr,
                        OrderUploadPointType: loadObj.OrderUploadPointType,
                        OrderUploadPointTypeStr: loadObj.OrderUploadPointTypeStr,
                        ScheduledUploadDate: loadObj.ScheduledUploadDateStr,
                        DateOfNeed: loadObj.DateOfNeedStr,
                        InvoiceId: loadObj.InvoiceId,
                        ForexTypeId: loadObj.ForexTypeId,
                        TraillerVehicleId: loadObj.VehicleTraillerId,
                        InvoiceStatus: loadObj.InvoiceStatus,
                        InvoiceFreightPrice: loadObj.InvoiceFreightPrice,
                        CmrNo: loadObj.CmrNo,
                        CmrStatus: loadObj.CmrStatus,
                        ShipperFirmExplanation: loadObj.ShipperFirmExplanation,
                        BuyerFirmExplanation: loadObj.BuyerFirmExplanation,
                        ReadinessDate: loadObj.ReadinessDateStr,
                        DeliveryFromCustomerDate: loadObj.DeliveryFromCustomerDateStr,
                        IntendedArrivalDate: loadObj.IntendedArrivalDateStr,
                        FirmCustomsArrivalId: loadObj.FirmCustomsArrivalId,
                        CustomsExplanation: loadObj.CustomsExplanation,
                        T1T2No: loadObj.T1T2No,
                        TClosingDate: loadObj.TClosingDateStr,
                        HasCmrDeliveryed: loadObj.HasCmrDeliveryed,
                        ItemPrice: loadObj.ItemPrice,
                        TrailerType: loadObj.TrailerType,
                        HasItemInsurance: loadObj.HasItemInsurance,
                        HasItemDangerous: loadObj.HasItemDangerous,
                        CmrCustomerDeliveryDate: loadObj.CmrCustomerDeliveryDateStr,
                        BringingToWarehousePlate: loadObj.BringingToWarehousePlate,
                        ShipperCityId: loadObj.ShipperCityId,
                        ShipperCityName: loadObj.ShipperCityName,
                        BuyerCityId: loadObj.BuyerCityId,
                        BuyerCityName: loadObj.BuyerCityName,
                        ShipperCountryId: loadObj.ShipperCountryId,
                        ShipperCountryName: loadObj.ShipperCountryName,
                        BuyerCountryId: loadObj.BuyerCountryId,
                        BuyerCountryName: loadObj.BuyerCountryName,
                        CustomerFirmId: loadObj.CustomerFirmId,
                        ShipperFirmId: loadObj.ShipperFirmId,
                        BuyerFirmId: loadObj.BuyerFirmId,
                        EntryCustomsId: loadObj.EntryCustomsId,
                        ExitCustomsId: loadObj.ExitCustomsId,
                        PlantId: loadObj.PlantId,
                        RotaId: null,
                        NewDetail: true,
                    };

                    $scope.modelObject.VoyageDetails.push(newObj);
                },
                key: 'Id'
            },
            showColumnLines: true,
            showRowLines: true,
            rowAlternationEnabled: true,
            focusedRowEnabled: false,
            columnAutoWidth: true,
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
            remoteOperations: false,
            scrolling: {
                mode: "single"
            },
            height: 400,
            editing: {
                allowUpdating: true,
                allowDeleting: true,
                allowAdding: true,
                mode: 'cell'
            },
            repaintChangesOnly: true,
            columns: [
                {
                    dataField: 'LoadCode', caption: 'Yük Kodu',
                    lookup: {
                        dataSource: $scope.waitingLoadList,
                        valueExpr: "LoadCode",
                        displayExpr: "Yük Kodu"
                    },
                    allowSorting: false,
                    //width: 70,
                    validationRules: [{ type: "required" }],
                    editCellTemplate: $scope.dropDownBoxEditorWaitingLoadTemplate,
                    cellTemplate: function (container, options) {
                        if (typeof options.row.data.LoadCode != 'undefined'
                            && options.row.data.LoadCode != null && options.row.data.LoadCode.length > 0)
                            container.text(options.row.data.LoadCode);
                        else
                            container.text(options.displayValue);
                    }
                },
                //{ dataField: 'LoadCode', caption: 'Yük Kodu', allowEditing: false, width:150},
                //{ dataField: 'VoyageStatusStr', caption: 'Yük Durum', dataType: 'date', format: 'dd.MM.yyyy', allowEditing: false },
                { dataField: 'CustomerFirmName', caption: 'Müşteri', allowEditing: false },
                { dataField: 'ShipperFirmName', caption: 'Gönderici Firma', allowEditing: false },
                { dataField: 'BuyerFirmName', caption: 'Alıcı Firma', allowEditing: false },
                //{ dataField: 'OrderTransactionDirectionTypeStr', caption: 'İşlem Yönü', allowEditing: false },
                { dataField: 'OveralQuantity', caption: 'Toplam Miktar', allowEditing: false, dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'OveralWeight', caption: 'Toplam Ağırlık(KG)', allowEditing: false },
                { dataField: 'DischargeLineNo', caption: 'Boşaltma Sırası', allowEditing: true },
                { dataField: 'LoadingLineNo', caption: 'Yükleme Sırası', allowEditing: true },
                { dataField: 'BuyerCityName', caption: 'Boş. Şehri', allowEditing: false },
                { dataField: 'BuyerCountryName', caption: 'Boş. Ülke', allowEditing: false },
                //{ dataField: 'OveralLadametre', caption: 'Toplam Ladametre', allowEditing: false, dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                //{ dataField: 'OveralVolume', caption: 'Toplam Hacim(M3)', allowEditing: false, dataType: 'number', format: { type: "fixedPoint", precision: 2 } },

            ]
        });
    }
    $scope.bindVoyageDrivers = function () {
        $('#driverList').dxDataGrid({
            dataSource:
            {
                load: function () { return $scope.modelObject.VoyageDrivers },
                remove: function (key) {
                    var obj = $scope.modelObject.VoyageDrivers.find(d => d.Id == key);
                    if (obj != null) {
                        $scope.modelObject.VoyageDrivers.splice($scope.modelObject.VoyageDrivers.indexOf(obj), 1);
                    }
                },
                update: function (key, values) {
                    var obj = $scope.modelObject.VoyageDrivers.find(d => d.Id == key);
                    if (obj != null) {
                        if (typeof values.DriverId != 'undefined') {
                            var driverObj = $scope.driverList.find(d => d.Id == values.DriverId);
                            obj.DriverId = driverObj.Id;
                        }
                        if (typeof values.TowinfVehicleId != 'undefined') {
                            var tVehicleObj = $scope.towingVehicleList.find(d => d.Id == values.TowinfVehicleId);
                            obj.TowinfVehicleId = tVehicleObj.Id;
                        }
                        if (typeof values.StartDateStr != 'undefined') { obj.StartDateStr = values.StartDateStr; }
                        if (typeof values.EndDateStr != 'undefined') { obj.EndDateStr = values.EndDateStr; }
                        if (typeof values.StartKmHour != 'undefined') { obj.StartKmHour = values.StartKmHour; }
                        if (typeof values.EndKmHour != 'undefined') { obj.EndKmHour = values.EndKmHour; }
                    }
                },
                insert: function (values) {
                    var newId = 1;
                    if ($scope.modelObject.VoyageDrivers.length > 0) {
                        newId = $scope.modelObject.VoyageDrivers.map(d => d.Id).reduce((max, n) => n > max ? n : max)
                        newId++;
                    }
                    var driverObj = $scope.driverList.find(d => d.Id == values.DriverId);

                    var towingVehicleObj = $scope.towingVehicleList.find(d => d.Id == values.TowingVehicleId);

                    var newObj = {
                        Id: newId,
                        DriverId: driverObj.Id,
                        TowingVehicleId: towingVehicleObj.Id,
                        StartDateStr: values.StartDateStr,
                        StartDate: values.StartDateStr,
                        EndDateStr: values.EndDateStr,
                        EndDate: values.EndDateStr,
                        StartKmHour: values.StartKmHour,
                        EndKmHour: values.EndKmHour,
                        NewDetail: true,
                    };

                    $scope.modelObject.VoyageDrivers.push(newObj);
                },
                key: 'Id'
            },
            showColumnLines: true,
            showRowLines: true,
            rowAlternationEnabled: true,
            focusedRowEnabled: false,
            columnAutoWidth: true,
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
            remoteOperations: false,
            scrolling: {
                mode: "single"
            },
            height: 300,
            editing: {
                allowUpdating: true,
                allowDeleting: true,
                allowAdding: true,
                mode: 'cell'
            },
            repaintChangesOnly: true,
            columns: [
                {
                    dataField: 'DriverId', caption: 'Şoför',
                    lookup: {
                        dataSource: $scope.driverList,
                        valueExpr: "Id",
                        displayExpr: "DriverName"
                    },
                    allowSorting: false,
                    validationRules: [{ type: "required" }],
                    editCellTemplate: $scope.dropDownBoxEditorDriverTemplate,
                    cellTemplate: function (container, options) {
                        if (typeof options.row.data.DriverId != 'undefined'
                            && options.row.data.DriverId != null && options.row.data.DriverId.length > 0)
                            container.text(options.row.data.DriverId);
                        else
                            container.text(options.displayValue);
                    }
                },
                { dataField: 'StartDateStr', caption: 'Başlama Tarih', dataType: 'date', format: 'dd.MM.yyyy', allowEditing: true },
                { dataField: 'EndDate', caption: 'Bitiş Tarih', dataType: 'date', format: 'dd.MM.yyyy', allowEditing: true },
                { dataField: 'StartKmHour', caption: 'Başlama KM', allowEditing: true },
                { dataField: 'EndKmHour', caption: 'Bitiş KM', allowEditing: true },
                {
                    dataField: 'TowingVehicleId', caption: 'Çekici',
                    lookup: {
                        dataSource: $scope.towingVehicleList,
                        valueExpr: "Id",
                        displayExpr: "Plate"
                    },
                    allowSorting: false,
                    validationRules: [{ type: "required" }],
                    editCellTemplate: $scope.dropDownBoxEditorTowingVehicleTemplate,
                    cellTemplate: function (container, options) {
                        if (typeof options.row.data.Plate != 'undefined'
                            && options.row.data.Plate != null && options.row.data.Plate.length > 0)
                            container.text(options.row.data.Plate);
                        else
                            container.text(options.displayValue);
                    }
                },            ]
        });
    }

    // INFORMATIONS & ATTACHMENTS
    $scope.showRecordInformation = function () {
        $scope.$broadcast('showRecordInformation', { Id: $scope.modelObject.Id, DataType: 'ItemOrder' });
    }

    $scope.showAttachmentList = function () {
        $scope.$broadcast('showAttachmentList',
            { RecordId: $scope.modelObject.Id, RecordType: 1 });

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

    //// ROW MENU ACTIONS
    //$scope.showRowMenu = function () {
    //    if ($scope.selectedRow && $scope.selectedRow.Id > 0) {
    //        $scope.$apply();

    //        $('#dial-row-menu').dialog({
    //            width: 300,
    //            //height: window.innerHeight * 0.6,
    //            hide: true,
    //            modal: true,
    //            resizable: false,
    //            show: true,
    //            draggable: false,
    //            closeText: "KAPAT"
    //        });
    //    }
    //}


    // ON LOAD EVENTS
    DevExpress.localization.locale('tr');
    $scope.loadSelectables().then(function () {
        if (PRM_ID > 0)
            $scope.bindModel(PRM_ID);
        else
            $scope.bindModel();

    });

});