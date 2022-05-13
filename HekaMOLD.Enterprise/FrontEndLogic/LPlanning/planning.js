app.controller('lPlanningCtrl', function ($scope, $http) {
    $scope.modelObject = { Id: 0, VoyageDateStr: moment().format('DD.MM.YYYY'), VoyageDetails: [], VoyageWeek: moment().year() + '-' + moment().week()};

    $scope.driverList = [];
    $scope.plannedLoadList = [];
    $scope.rotaList = [];
    $scope.traillerVehicleList = [];
    $scope.forexTypeList = [];
    $scope.VoyageWeek= moment().year() + '-' + moment().week();
    $scope.selectedDriver = {}
    $scope.selectedTraillerVehicle = {}
    $scope.selectedOrderTransactionDirectionType = {};
    $scope.orderTransactionDirectionTypeList = [{ Id: 1, Text: 'İhracat' }, { Id: 2, Text: 'İthalat' },
    { Id: 3, Text: 'Yurt İçi' }, { Id: 4, Text: 'Transit' }];

    $scope.selectedFirm = {};
    $scope.selectedForexType = {};

    $scope.saveStatus = 0;

    // CRUD
    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0, VoyageDateStr: moment().format('DD.MM.YYYY'), VoyageDetails: [], OrderStatus: 0 };

        $scope.selectedDriver = {}
        $scope.selectedTraillerVehicle = {}
        $scope.selectedOrderTransactionDirectionType = {};
        $scope.selectedForexType = {};
        $scope.bindVoyageDetails();
    }
    // RECEIPT FUNCTIONS
    //GET NEXT VOYAGE CODE
    $scope.getNextVoyageCode = function () {
        if (typeof $scope.selectedOrderTransactionDirectionType.Id == 'undefined' ^ typeof $scope.selectedTraillerVehicle.Id == 'undefined') {
            return "";
        }
        if (typeof $scope.selectedOrderTransactionDirectionType.Id == 'undefined')
            directionId = 0;
        else
            directionId = $scope.selectedOrderTransactionDirectionType.Id;
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Voyage/GetVoyageCode?strParam=' + directionId + '-' + $scope.selectedTraillerVehicle.VehicleAllocationType, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        if (resp.data.Result) {
                            resolve(resp.data.VoyageCode);
                            $scope.modelObject.VoyageCode = resp.data.VoyageCode;
                        }
                        else {
                            toastr.error('Sıradaki yük numarası üretilemedi. Lütfen ekranı yenileyip tekrar deneyiniz.', 'Uyarı');
                            resolve('');
                        }
                    }
                }).catch(function (err) { });
        });

        return prms;
    }
    // #region VOYAGE MANAGEMENT
    $scope.showNewVoyageForm = function () {
        $scope.$broadcast('loadVoyage', { id: 0 });

        $('#dial-voyage').dialog({
            hide: true,
            modal: true,
            resizable: false,
            width: window.innerWidth * 0.7,
            height: window.innerHeight * 0.8,
            show: true,
            draggable: false,
            closeText: "KAPAT"
        });
    }

    $scope.$on('editVoyageEnd', function (e, d) {

        d.forEach(x => {
            if ($scope.modelObject.VoyageDetails.filter(m => m.LoadCode == x.LoadCode).length > 0) {
                toastr.warning(x.LoadCode + ' nolu yük, ' + x.OveralVolume + ' / ' + x.OveralWeight + ', ' + x.OveralQuantity
                    + ' miktarlı yük detayı zaten aktarıldığı için tekrar dahil edilmedi.', 'Uyarı');
            }
            else {
                //alert();
                var newId = 1;
                if ($scope.modelObject.VoyageDetails.length > 0) {
                    newId = $scope.modelObject.VoyageDetails.map(d => d.Id).reduce((max, n) => n > max ? n : max)
                    newId++;
                }
                $scope.modelObject.VoyageDetails.push({
                    Id: newId,
                    DischargeLineNo: newId,
                    ItemLoadId: x.Id,
                    LoadCode: x.LoadCode,
                    LoadingDateStr: x.LoadingDateStr,
                    LoadOutDate: x.LoadOutDateStr,
                    CustomerFirmName: x.CustomerFirmName,
                    ShipperFirmName: x.ShipperFirmName,
                    BuyerFirmName: x.BuyerFirmName,
                    OrderTransactionDirectionType: x.OrderTransactionDirectionType,
                    OrderTransactionDirectionTypeStr: x.OrderTransactionDirectionTypeStr,
                    OrderCalculationType: x.OrderCalculationType,
                    OrderCalculationTypeStr: x.OrderCalculationTypeStr,
                    OveralQuantity: x.OveralQuantity,
                    OveralWeight: x.OveralWeight,
                    OveralLadametre: x.OveralLadametre,
                    OveralVolume: x.OveralVolume,
                    OverallTotal: x.OverallTotal,
                    OrderNo: x.OrderNo,
                    LoadDate: x.LoadDateStr,
                    DischargeDate: x.DischargeDateStr,
                    CalculationTypePrice: x.CalculationTypePrice,
                    DocumentNo: x.DocumentNo,
                    OrderUploadType: x.OrderUploadType,
                    OrderUploadTypeStr: x.OrderUploadTypeStr,
                    OrderUploadPointType: x.OrderUploadPointType,
                    OrderUploadPointTypeStr: x.OrderUploadPointTypeStr,
                    ScheduledUploadDate: x.ScheduledUploadDateStr,
                    DateOfNeed: x.DateOfNeedStr,
                    InvoiceId: x.InvoiceId,
                    ForexTypeId: x.ForexTypeId,
                    TraillerVehicleId: x.VehicleTraillerId,
                    InvoiceStatus: x.InvoiceStatus,
                    InvoiceFreightPrice: x.InvoiceFreightPrice,
                    CmrNo: x.CmrNo,
                    CmrStatus: x.CmrStatus,
                    ShipperFirmExplanation: x.ShipperFirmExplanation,
                    BuyerFirmExplanation: x.BuyerFirmExplanation,
                    ReadinessDate: x.ReadinessDateStr,
                    DeliveryFromCustomerDate: x.DeliveryFromCustomerDateStr,
                    IntendedArrivalDate: x.IntendedArrivalDateStr,
                    FirmCustomsArrivalId: x.FirmCustomsArrivalId,
                    CustomsExplanation: x.CustomsExplanation,
                    T1T2No: x.T1T2No,
                    TClosingDate: x.TClosingDateStr,
                    HasCmrDeliveryed: x.HasCmrDeliveryed,
                    ItemPrice: x.ItemPrice,
                    TrailerType: x.TrailerType,
                    HasItemInsurance: x.HasItemInsurance,
                    HasItemDangerous: x.HasItemDangerous,
                    CmrCustomerDeliveryDate: x.CmrCustomerDeliveryDateStr,
                    BringingToWarehousePlate: x.BringingToWarehousePlate,
                    ShipperCityId: x.ShipperCityId,
                    ShipperCityName: x.ShipperCityName,
                    BuyerCityId: x.BuyerCityId,
                    BuyerCityName: x.BuyerCityName,
                    ShipperCountryId: x.ShipperCountryId,
                    ShipperCountryName: x.ShipperCountryName,
                    BuyerCountryId: x.BuyerCountryId,
                    BuyerCountryName: x.BuyerCountryName,
                    CustomerFirmId: x.CustomerFirmId,
                    ShipperFirmId: x.ShipperFirmId,
                    BuyerFirmId: x.BuyerFirmId,
                    EntryCustomsId: x.EntryCustomsId,
                    ExitCustomsId: x.ExitCustomsId,
                    DeclarationX1No: x.DeclarationX1No,
                    PlantId: x.PlantId,
                    CmrShipperFirmId: x.CmrShipperFirmId,
                    CmrBuyerFirmId: x.CmrBuyerFirmId,
                    ManufacturerFirmId: x.ManufacturerFirmId,
                    FirmCustomsExitId: x.FirmCustomsExitId,
                    BuyerFirmAddress: x.BuyerFirmAddress,
                    ShipperFirmAddress: x.ShipperFirmAddress,
                    ReelOwnerFirmId: x.ReelOwnerFirmId,
                    T1T2StartDate: x.T1T2StartDate,
                    RotaId: null,
                    NewDetail: true,
                });
                $scope.plannedLoadList = $scope.modelObject.VoyageDetails;
                console.log($scope.plannedLoadList);
            }
        });
        $scope.bindVoyageDetails();
        console.log($scope.modelObject.VoyageDetails);
        $('#dial-voyage').dialog('close');
    });
    // #endregion

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
                    $http.post(HOST_URL + 'PIOrder/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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

        if (typeof $scope.selectedFirm != 'undefined' && $scope.selectedFirm != null)
            $scope.modelObject.FirmId = $scope.selectedFirm.Id;
        else
            $scope.modelObject.FirmId = null;

        if (typeof $scope.selectedTraillerVehicle != 'undefined' && $scope.selectedTraillerVehicle != null)
            $scope.modelObject.TraillerVehicleId = $scope.selectedTraillerVehicle.Id;
        else
            $scope.modelObject.TraillerVehicleId = null;

        if (typeof $scope.selectedDriver != 'undefined' && $scope.selectedDriver != null)
            $scope.modelObject.DriverId = $scope.selectedDriver.Id;
        else
            $scope.modelObject.DriverId = null;

        if (typeof $scope.selectedOrderTransactionDirectionType != 'undefined' && $scope.selectedOrderTransactionDirectionType != null)
            $scope.modelObject.OrderTransactionDirectionType = $scope.selectedOrderTransactionDirectionType.Id;
        else
            $scope.modelObject.OrderTransactionDirectionType = null;

        if (typeof $scope.selectedForexType != 'undefined' && $scope.selectedForexType != null)
            $scope.modelObject.ForexTypeId = $scope.selectedForexType.Id;
        else
            $scope.modelObject.ForexTypeId = null;

        $http.post(HOST_URL + 'LPlanning/SaveModel', $scope.modelObject, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.saveStatus = 0;

                    if (resp.data.Status == 1) {
                        toastr.success('Kayıt başarılı.', 'Bilgilendirme');
                        $scope.openNewRecord();
                    }
                    else
                        toastr.error(resp.data.ErrorMessage, 'Hata');
                }
            }).catch(function (err) { });
    }

    $scope.dropDownBoxEditorTemplate = function (cellElement, cellInfo) {
        return $("<div>").dxDropDownBox({
            dropDownOptions: { width: 600 },
            dataSource: $scope.rotaList,
            value: cellInfo.value,
            valueExpr: "Id",
            displayExpr: "Id",
            contentTemplate: function (e) {
                return $("<div>").dxDataGrid({
                    dataSource: $scope.rotaList,
                    remoteOperations: true,
                    columns: [
                        { dataField: 'CityStartPostCode', caption: 'Başlama Şehri Posta Kodu' },
                        { dataField: 'CityStartName', caption: 'Başlama Şehri' },
                        { dataField: 'CityEndPostCode', caption: 'Bitiş Şehri Posta Kodu' },
                        { dataField: 'CityEndName', caption: 'Bitiş Şehri' },
                        { dataField: 'KmHour', caption: 'Km' }
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
                        if (typeof values.RotaId != 'undefined') {
                            var rotaObj = $scope.rotaList.find(d => d.Id == values.RotaId);
                            obj.RotaId = rotaObj.Id;
                            obj.CityStartName = rotaObj.CityStartName;
                            obj.CityStartPostCode = rotaObj.CityStartPostCode;
                            obj.CityEndName = rotaObj.CityEndName;
                            obj.CityEndPostCode = rotaObj.CityEndPostCode;
                            obj.KmHour = rotaObj.KmHour;
                        }

                    }
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
            height: '50vh',
            editing: {
                allowUpdating: true,
                allowDeleting: true,
                allowAdding: false,
                mode: 'cell'
            },
            repaintChangesOnly: true,
            columns: [
                { dataField: 'LoadingLineNo', caption: 'Yükleme Sırası', allowEditing: true },
                { dataField: 'DischargeLineNo', caption: 'Boşaltma Sırası', allowEditing: true },
                { dataField: 'LoadCode', caption: 'Yük Kodu', allowEditing: false },
                { dataField: 'LoadingDateStr', caption: 'Yükleme Tarih', dataType: 'date', format: 'dd.MM.yyyy', allowEditing: false },
                { dataField: 'CustomerFirmName', caption: 'Müşteri', allowEditing: false },
                { dataField: 'ShipperFirmName', caption: 'Gönderici Firma', allowEditing: false },
                { dataField: 'BuyerFirmName', caption: 'Alıcı Firma', allowEditing: false },
                { dataField: 'OveralQuantity', caption: 'Toplam Miktar', allowEditing: false, dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'OveralWeight', caption: 'Toplam Ağırlık(KG)', allowEditing: false },
                { dataField: 'BuyerCityName', caption: 'Boş. Şehri', allowEditing: false },
                { dataField: 'BuyerCountryName', caption: 'Boş. Ülke', allowEditing: false },
                { dataField: 'ShipperCityName', caption: 'Yük. Şehri', allowEditing: false },
                { dataField: 'ShipperCountryName', caption: 'Yük. Ülke', allowEditing: false },
                //{ dataField: 'OveralLadametre', caption: 'Toplam Ladametre', allowEditing: false, dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                //{ dataField: 'OveralVolume', caption: 'Toplam Hacim', allowEditing: false, dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
            ]
        });
    }

    $scope.loadSelectables = function () {
        var prmReq = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'LPlanning/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.traillerVehicleList = resp.data.Vehicles;
                        $scope.driverList = resp.data.Drivers;
                        $scope.VoyageDateStr = moment().format('DD.MM.YYYY')
                        $scope.rotaList = resp.data.Rotas;
                        $scope.forexTypeList = resp.data.ForexTypes;

                        resolve(resp.data);
                    }
                }).catch(function (err) { });
        });

        return prmReq;
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

    // ON LOAD EVENTS
    DevExpress.localization.locale('tr');
    $scope.loadSelectables();
    $scope.bindVoyageDetails();
});