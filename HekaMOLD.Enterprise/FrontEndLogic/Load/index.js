app.controller('loadCtrl', function ($scope, $http) {
    $scope.modelObject = { Id: 0, Details: [], LoadStatus: 0 };

    $scope.itemList = [];
    $scope.unitList = [];
    $scope.firmList = [];
    $scope.forexList = [];
    $scope.driverList = [];

    $scope.itemOrderDetailList = [];

    $scope.customsList = [];
    $scope.usersList = [];
    $scope.cityList = [];
    $scope.countryList = [];
    $scope.firmArrivalCustoms = [];
    $scope.vehicleTrailerList = [];
    $scope.vehicleTowinfList = [];

    $scope.selectedUser = {};
    $scope.selectedDriver = {};
    $scope.selectedCustomerFirm = {};
    $scope.selectedRow = { Id: 0 };
    $scope.selectedVehicleTrailer = {};
    $scope.selectedTowinfVehicle = {};

    $scope.selectedEntryCustoms = {};
    $scope.selectedExitCustoms = {};
    $scope.selectedBuyerCity = {};
    $scope.selectedBuyerCountry = {};
    $scope.selectedShipperCity = {};
    $scope.selectedShipperCountry = {};
    $scope.selectedCmrBuyerCity = {};
    $scope.selectedCmrBuyerCountry = {};
    $scope.selectedCmrShipperCity = {};
    $scope.selectedCmrShipperCountry = {};
    $scope.selectedForexType = {};
    $scope.selectedFirmCustomsArrival = {};
    $scope.selectedFirmCustomsExit = {};
    $scope.selectedFirmReelOwner = {};
    $scope.selectedFirmManufacturer = {};
    $scope.selectedOrderUploadType = {};
    $scope.selectedBuyerFirm = {};
    $scope.selectedShipperFirm = {};
    $scope.selectedCmrBuyerFirm = {};
    $scope.selectedCmrShipperFirm = {};

    $scope.selectedVoyageStartCity = {};
    $scope.selectedVoyageStartCountry = {};
    $scope.selectedVoyageEndCity = {};
    $scope.selectedVoyageEndCountry = {};

    $scope.modelObject.ShipperFirmAddress = "";
    $scope.modelObject.BuyerFirmAddress = "";
    $scope.orderUploadTypeList = [{ Id: 1, Text: 'Grupaj' }, { Id: 2, Text: 'Komple' }];

    $scope.selectedOrderTransactionDirectionType = {};
    $scope.orderTransactionDirectionTypeList = [{ Id: 1, Text: 'İhracat' }, { Id: 2, Text: 'İthalat' },
    { Id: 3, Text: 'Yurt İçi' }, { Id: 4, Text: 'Transit' }];

    $scope.selectedOrderUploadPointType = {};
    $scope.orderUploadPointTypeList = [{ Id: 1, Text: 'Müşteriden Yükleme' }, { Id: 2, Text: 'Depodan Yükleme' }];

    $scope.selectedOrderCalculationType = { Id: 0 };
    $scope.orderCalculationTypeList = [{ Id: 1, Text: 'Ağırlık' }, { Id: 2, Text: 'Metreküp' }, { Id: 3, Text: 'Ladametre' }, { Id: 4, Text: 'Komple' }, { Id: 5, Text: 'Minimun' }];

    $scope.selectedLoadStatusType = { Id: 0 };
    $scope.loadStatusTypeList = [{ Id: 2, Text: 'Hazır Bekliyor' }, { Id: 3, Text: 'Yük Depoda' }, { Id: 4, Text: 'Müşteriden Yüklendi' },
    { Id: 5, Text: 'Yurtiçi Gümrükte' }, { Id: 6, Text: 'Kapıkulede' }, { Id: 7, Text: 'Yurtdışı Yolda' }, { Id: 8, Text: 'Yurtdışı Gümrükte' }, { Id: 9, Text: 'Boşaltmada' },
    { Id: 10, Text: 'Boşaltıldı' }, { Id: 11, Text: 'Yüklemede' }, { Id: 12, Text: 'Yüklendi' }, { Id: 13, Text: 'Tamamlandı' }];

    $scope.selectedTrailerType = {};
    $scope.trailerTypeList = [{ Id: 1, Text: 'Çadırlı' },
    { Id: 2, Text: 'Frigo' }, { Id: 3, Text: 'Kapalı Kasa' }, { Id: 4, Text: 'Optima' }, { Id: 5, Text: 'Mega' }
        , { Id: 6, Text: 'Konteyner' }, { Id: 7, Text: 'Swapboddy' }, { Id: 8, Text: 'Lowbed' }
        , { Id: 9, Text: 'Kamyon Romörk' }, { Id: 10, Text: 'Standart' }, { Id: 10, Text: 'Minivan' }];

    $scope.saveStatus = 0;

    // #region PRINTING BUSINESS
    $scope.showPrintTemplates = function () {
        if ($scope.modelObject.Id > 0) {
            // DO BROADCAST
            $scope.$broadcast('loadTemplateList', [6]);

            $('#dial-reports').dialog({
                width: window.innerWidth * 0.65,
                height: window.innerHeight * 0.65,
                hide: true,
                modal: true,
                resizable: false,
                show: true,
                draggable: false,
                closeText: "KAPAT"
            });
        }
    }

    $scope.showPrintOptions = function () {
        $scope.$broadcast('showPrintOptions');

        $('#dial-print-options').dialog({
            hide: true,
            modal: true,
            resizable: false,
            width: 300,
            show: true,
            draggable: false,
            closeText: "KAPAT"
        });
    }


    // RECEIVE EMIT REPORT PRINT DATA
    $scope.$on('printTemplate', function (e, d) {
        if (d.templateId > 0) {
            $scope.reportTemplateId = d.templateId;

            if (d.exportType == 'PDF') {
                try {
                    $http.post(HOST_URL + 'Printing/ExportAsPdf', {
                        objectId: $scope.modelObject.Id,
                        reportId: $scope.reportTemplateId,
                        reportType: 6,
                    }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                window.open(HOST_URL + 'Outputs/' + resp.data.Path);
                            }
                        }).catch(function (err) { });
                } catch (e) {

                }
            }
            else {
                $scope.showPrintOptions();
            }
        }

        $('#dial-reports').dialog('close');
    });

    // RECEIVE EMIT PRINTING OPTIONS DATA
    $scope.$on('printOptionsApproved', function (e, d) {
        $('#dial-print-options').dialog('close');

        try {
            $http.post(HOST_URL + 'Printing/AddToPrintQueue', {
                objectId: $scope.modelObject.Id,
                reportId: $scope.reportTemplateId,
                printerId: d.PrinterId,
                recordType: 6, // delivery list type
            }, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        if (resp.data.Status == 1)
                            toastr.success('İstek yazıcıya iletildi.', 'Bilgilendirme');
                        else
                            toastr.error('Hata: ' + resp.data.ErrorMessage, 'Uyarı');
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    });
    // #endregion
    // INFORMATIONS & ATTACHMENTS
    $scope.showRecordInformation = function () {
        $scope.$broadcast('showRecordInformation', { Id: $scope.modelObject.Id, DataType: 'ItemOrder' });
    }
    // FUNCTIONS
    $scope.getNextOrderNo = function () {

        let directionId = 0;

        if (typeof $scope.selectedOrderTransactionDirectionType.Id == 'undefined')
            directionId = 0;
        else
            directionId = $scope.selectedOrderTransactionDirectionType.Id;

        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Load/GetNextLoadCode?directionId=' + directionId, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        if (resp.data.Result) {
                            resolve(resp.data.LoadCode);
                            $scope.modelObject.LoadCode = resp.data.LoadCode;
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
    $scope.getNextRecord = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Load/GetNextRecord?Id=' + $scope.modelObject.Id, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        if (resp.data.Result) {
                            window.location.href = HOST_URL + 'Load?rid=' + resp.data.NextNo;
                        }
                        else {
                            toastr.warning('Sıradaki yük numarasına ulaşılamadı. Lütfen ekranı yenileyip tekrar deneyiniz.', 'Uyarı');
                            resolve('');
                        }
                    }
                }).catch(function (err) { });
        });

        return prms;
    }
    $scope.getBackRecord = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Load/GetBackRecord?Id=' + $scope.modelObject.Id, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        if (resp.data.Result) {
                            window.location.href = HOST_URL + 'Load?rid=' + resp.data.NextNo;
                        }
                        else {
                            toastr.warning('Sıradaki yük numarasına ulaşılamadı. Lütfen ekranı yenileyip tekrar deneyiniz.', 'Uyarı');
                            resolve('');
                        }
                    }
                }).catch(function (err) { });
        });

        return prms;
    }
    // SELECTABLES
    // #region VOYAGE MANAGEMENT
    $scope.showNewShipperFirmAddressForm = function () {
        $scope.$broadcast('loadShipperFirmAddress', { Id: $scope.selectedShipperFirm.Id });

        $('#dial-ShipperFirmAddress').dialog({
            hide: true,
            modal: true,
            resizable: false,
            width: window.innerWidth * 0.6,
            height: window.innerHeight * 0.7,
            show: true,
            draggable: false,
            closeText: "KAPAT"
        });
    }
    $scope.showNewBuyerFirmAddressForm = function () {
        $scope.$broadcast('loadBuyerFirmAddress', { Id: $scope.selectedBuyerFirm.Id });

        $('#dial-BuyerFirmAddress').dialog({
            hide: true,
            modal: true,
            resizable: false,
            width: window.innerWidth * 0.6,
            height: window.innerHeight * 0.7,
            show: true,
            draggable: false,
            closeText: "KAPAT"
        });
    }

    // CRUD
    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0, Details: [], LoadStatus: 0 };
        $scope.selectedFirm = {};
        $scope.selectedUser = {};
        $scope.selectedDriver = {};

        $scope.selectedOrderUploadType = {};
        $scope.selectedOrderTransactionDirectionType = {};
        $scope.selectedOrderCalculationType = { Id: 0 };
        $scope.selectedOrderUploadPointType = {};
        $scope.selectedEntryCustoms = {};
        $scope.selectedExitCustoms = {};
        $scope.selectedBuyerCity = {};
        $scope.selectedBuyerCountry = {};
        $scope.selectedShipperCity = {};
        $scope.selectedShipperCountry = {};
        $scope.modelObject.ShipperFirmAddress = "";
        $scope.modelObject.BuyerFirmAddress = "";
        $scope.modelObject.Explanation = "";
        $scope.selectedBuyerFirm = {};
        $scope.selectedShipperFirm = {};
        $scope.selectedCustomerFirm = {};
        $scope.selectedCmrShipperFirm = {};
        $scope.selectedCmrBuyerFirm = {};
        $scope.modelObject.Explanation = "";
        $scope.modelObject.OrderNo = "";
        $scope.selectedFirmCustomsArrival = {};
        $scope.selectedFirmCustomsExit = {};
        $scope.selectedVehicleTrailer = {};
        $scope.selectedFirmReelOwner = {};
        $scope.selectedFirmManufacturer = {};
        $scope.selectedTowinfVehicle = {};
        $scope.selectedVoyageStartCity = {};
        $scope.selectedVoyageStartCountry = {};
        $scope.selectedVoyageEndCity = {};
        $scope.selectedVoyageEndCountry = {};
        $scope.selectedCmrBuyerCity = {};
        $scope.selectedCmrBuyerCountry = {};
        $scope.selectedCmrShipperCity = {};
        $scope.selectedCmrShipperCountry = {};
        //$scope.getNextOrderNo().then(function (rNo) {
        //    $scope.modelObject.OrderNo = rNo;
        //    $scope.$apply();
        //});
        $scope.bindDetails();
    }

    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu yükü silmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'Load/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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

        if (typeof $scope.selectedCustomerFirm != 'undefined' && $scope.selectedCustomerFirm != null)
            $scope.modelObject.CustomerFirmId = $scope.selectedCustomerFirm.Id;
        else
            $scope.modelObject.CustomerFirmId = null;

        if (typeof $scope.selectedShipperFirm != 'undefined' && $scope.selectedShipperFirm != null)
            $scope.modelObject.ShipperFirmId = $scope.selectedShipperFirm.Id;
        else
            $scope.modelObject.ShipperFirmId = null;

        if (typeof $scope.selectedBuyerFirm != 'undefined' && $scope.selectedBuyerFirm != null)
            $scope.modelObject.BuyerFirmId = $scope.selectedBuyerFirm.Id;
        else
            $scope.modelObject.BuyerFirmId = null;

        if (typeof $scope.selectedCmrShipperFirm != 'undefined' && $scope.selectedCmrShipperFirm != null)
            $scope.modelObject.CmrShipperFirmId = $scope.selectedCmrShipperFirm.Id;
        else
            $scope.modelObject.CmrShipperFirmId = null;

        if (typeof $scope.selectedCmrBuyerFirm != 'undefined' && $scope.selectedCmrBuyerFirm != null)
            $scope.modelObject.CmrBuyerFirmId = $scope.selectedCmrBuyerFirm.Id;
        else
            $scope.modelObject.CmrBuyerFirmId = null;

        if (typeof $scope.selectedEntryCustoms != 'undefined' && $scope.selectedEntryCustoms != null)
            $scope.modelObject.EntryCustomsId = $scope.selectedEntryCustoms.Id;
        else
            $scope.modelObject.EntryCustomsId = null;

        if (typeof $scope.selectedExitCustoms != 'undefined' && $scope.selectedExitCustoms != null)
            $scope.modelObject.ExitCustomsId = $scope.selectedExitCustoms.Id;
        else
            $scope.modelObject.ExitCustomsId = null;

        if (typeof $scope.selectedOrderUploadType != 'undefined' && $scope.selectedOrderUploadType != null)
            $scope.modelObject.OrderUploadType = $scope.selectedOrderUploadType.Id;
        else
            $scope.modelObject.OrderUploadType = null;

        if (typeof $scope.selectedOrderTransactionDirectionType != 'undefined' && $scope.selectedOrderTransactionDirectionType != null)
            $scope.modelObject.OrderTransactionDirectionType = $scope.selectedOrderTransactionDirectionType.Id;
        else
            $scope.modelObject.OrderTransactionDirectionType = null;

        if (typeof $scope.selectedOrderCalculationType != 'undefined' && $scope.selectedOrderCalculationType != null)
            $scope.modelObject.OrderCalculationType = $scope.selectedOrderCalculationType.Id;
        else
            $scope.modelObject.OrderCalculationType = null;

        if (typeof $scope.selectedOrderUploadPointType != 'undefined' && $scope.selectedOrderUploadPointType != null)
            $scope.modelObject.OrderUploadPointType = $scope.selectedOrderUploadPointType.Id;
        else
            $scope.modelObject.OrderUploadPointType = null;

        if (typeof $scope.selectedUser != 'undefined' && $scope.selectedUser != null)
            $scope.modelObject.UserAuthorId = $scope.selectedUser.Id;
        else
            $scope.modelObject.UserAuthorId = null;

        if (typeof $scope.selectedShipperCountry != 'undefined' && $scope.selectedShipperCountry != null)
            $scope.modelObject.ShipperCountryId = $scope.selectedShipperCountry.Id;
        else
            $scope.modelObject.ShipperCountryId = null;

        if (typeof $scope.selectedBuyerCountry != 'undefined' && $scope.selectedBuyerCountry != null)
            $scope.modelObject.BuyerCountryId = $scope.selectedBuyerCountry.Id;
        else
            $scope.modelObject.BuyerCountryId = null;

        if (typeof $scope.selectedCmrShipperCountry != 'undefined' && $scope.selectedCmrShipperCountry != null)
            $scope.modelObject.CmrShipperCountryId = $scope.selectedCmrShipperCountry.Id;
        else
            $scope.modelObject.CmrShipperCountryId = null;

        if (typeof $scope.selectedCmrBuyerCountry != 'undefined' && $scope.selectedCmrBuyerCountry != null)
            $scope.modelObject.CmrBuyerCountryId = $scope.selectedCmrBuyerCountry.Id;
        else
            $scope.modelObject.CmrBuyerCountryId = null;

        if (typeof $scope.selectedShipperCity != 'undefined' && $scope.selectedShipperCity != null)
            $scope.modelObject.ShipperCityId = $scope.selectedShipperCity.Id;
        else
            $scope.modelObject.ShipperCityId = null;

        if (typeof $scope.selectedBuyerCity != 'undefined' && $scope.selectedBuyerCity != null)
            $scope.modelObject.BuyerCityId = $scope.selectedBuyerCity.Id;
        else
            $scope.modelObject.BuyerCityId = null;

        if (typeof $scope.selectedCmrShipperCity != 'undefined' && $scope.selectedCmrShipperCity != null)
            $scope.modelObject.CmrShipperCityId = $scope.selectedCmrShipperCity.Id;
        else
            $scope.modelObject.CmrShipperCityId = null;

        if (typeof $scope.selectedCmrBuyerCity != 'undefined' && $scope.selectedCmrBuyerCity != null)
            $scope.modelObject.CmrBuyerCityId = $scope.selectedCmrBuyerCity.Id;
        else
            $scope.modelObject.CmrBuyerCityId = null;

        if (typeof $scope.selectedForexType != 'undefined' && $scope.selectedForexType != null)
            $scope.modelObject.ForexTypeId = $scope.selectedForexType.Id;
        else
            $scope.modelObject.ForexTypeId = null;

        if (typeof $scope.selectedTrailerType != 'undefined' && $scope.selectedTrailerType != null)
            $scope.modelObject.TrailerType = $scope.selectedTrailerType.Id;
        else
            $scope.modelObject.TrailerType = null;

        if (typeof $scope.selectedFirmCustomsArrival != 'undefined' && $scope.selectedFirmCustomsArrival != null)
            $scope.modelObject.FirmCustomsArrivalId = $scope.selectedFirmCustomsArrival.Id;
        else
            $scope.modelObject.FirmCustomsArrivalId = null;

        if (typeof $scope.selectedFirmCustomsExit != 'undefined' && $scope.selectedFirmCustomsExit != null)
            $scope.modelObject.FirmCustomsExitId = $scope.selectedFirmCustomsExit.Id;
        else
            $scope.modelObject.FirmCustomsExitId = null;

        if (typeof $scope.selectedVehicleTrailer != 'undefined' && $scope.selectedVehicleTrailer != null)
            $scope.modelObject.VehicleTraillerId = $scope.selectedVehicleTrailer.Id;
        else
            $scope.modelObject.VehicleTraillerId = null;

        if (typeof $scope.selectedLoadStatusType != 'undefined' && $scope.selectedLoadStatusType != null)
            $scope.modelObject.LoadStatusType = $scope.selectedLoadStatusType.Id;
        else
            $scope.modelObject.LoadStatusType = null;

        if (typeof $scope.selectedFirmManufacturer != 'undefined' && $scope.selectedFirmManufacturer != null)
            $scope.modelObject.ManufacturerFirmId = $scope.selectedFirmManufacturer.Id;
        else
            $scope.modelObject.ManufacturerFirmId = null;

        if (typeof $scope.selectedFirmReelOwner != 'undefined' && $scope.selectedFirmReelOwner != null)
            $scope.modelObject.ReelOwnerFirmId = $scope.selectedFirmReelOwner.Id;
        else
            $scope.modelObject.ReelOwnerFirmId = null;

        if (typeof $scope.selectedVehicleTrailer != 'undefined' && $scope.selectedVehicleTrailer != null)
            $scope.modelObject.VehicleTraillerId = $scope.selectedVehicleTrailer.Id;
        else
            $scope.modelObject.VehicleTraillerId = null;

        if (typeof $scope.selectedTowinfVehicle != 'undefined' && $scope.selectedTowinfVehicle != null)
            $scope.modelObject.TowinfVehicleId = $scope.selectedTowinfVehicle.Id;
        else
            $scope.modelObject.TowinfVehicleId = null;

        if (typeof $scope.selectedDriver != 'undefined' && $scope.selectedDriver != null)
            $scope.modelObject.DriverId = $scope.selectedDriver.Id;
        else
            $scope.modelObject.DriverId = null;

        if (typeof $scope.selectedVoyageStartCity != 'undefined' && $scope.selectedVoyageStartCity != null)
            $scope.modelObject.VoyageStartCityId = $scope.selectedVoyageStartCity.Id;
        else
            $scope.modelObject.VoyageStartCityId = null;

        if (typeof $scope.selectedVoyageStartCountry != 'undefined' && $scope.selectedVoyageStartCountry != null)
            $scope.modelObject.VoyageStartCountryId = $scope.selectedVoyageStartCountry.Id;
        else
            $scope.modelObject.VoyageStartCountryId = null;

        if (typeof $scope.selectedVoyageEndCity != 'undefined' && $scope.selectedVoyageEndCity != null)
            $scope.modelObject.VoyageEndCityId = $scope.selectedVoyageEndCity.Id;
        else
            $scope.modelObject.VoyageEndCityId = null;

        if (typeof $scope.selectedVoyageEndCountry != 'undefined' && $scope.selectedVoyageEndCountry != null)
            $scope.modelObject.VoyageEndCountryId = $scope.selectedVoyageEndCountry.Id;
        else
            $scope.modelObject.VoyageEndCountryId = null;

        $http.post(HOST_URL + 'Load/SaveModel', $scope.modelObject, 'json')
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

    $scope.bindModel = function (id) {
        $http.get(HOST_URL + 'Load/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;

                    if (typeof $scope.modelObject.ShipperFirmId != 'undefined' && $scope.modelObject.ShipperFirmId != null)
                        $scope.selectedShipperFirm = $scope.firmList.find(d => d.Id == $scope.modelObject.ShipperFirmId);
                    else
                        $scope.selectedShipperFirm = {};

                    if (typeof $scope.modelObject.BuyerFirmId != 'undefined' && $scope.modelObject.BuyerFirmId != null)
                        $scope.selectedBuyerFirm = $scope.firmList.find(d => d.Id == $scope.modelObject.BuyerFirmId);
                    else
                        $scope.selectedBuyerFirm = {};

                    if (typeof $scope.modelObject.CmrShipperFirmId != 'undefined' && $scope.modelObject.CmrShipperFirmId != null)
                        $scope.selectedCmrShipperFirm = $scope.firmList.find(d => d.Id == $scope.modelObject.CmrShipperFirmId);
                    else
                        $scope.selectedCmrShipperFirm = {};

                    if (typeof $scope.modelObject.CmrBuyerFirmId != 'undefined' && $scope.modelObject.CmrBuyerFirmId != null)
                        $scope.selectedCmrBuyerFirm = $scope.firmList.find(d => d.Id == $scope.modelObject.CmrBuyerFirmId);
                    else
                        $scope.selectedCmrBuyerFirm = {};

                    if (typeof $scope.modelObject.CustomerFirmId != 'undefined' && $scope.modelObject.CustomerFirmId != null)
                        $scope.selectedCustomerFirm = $scope.firmList.find(d => d.Id == $scope.modelObject.CustomerFirmId);
                    else
                        $scope.selectedCustomerFirm = {};

                    if (typeof $scope.modelObject.EntryCustomsId != 'undefined' && $scope.modelObject.EntryCustomsId != null)
                        $scope.selectedEntryCustoms = $scope.customsList.find(d => d.Id == $scope.modelObject.EntryCustomsId);
                    else
                        $scope.selectedEntryCustoms = {};

                    if (typeof $scope.modelObject.ExitCustomsId != 'undefined' && $scope.modelObject.ExitCustomsId != null)
                        $scope.selectedExitCustoms = $scope.customsList.find(d => d.Id == $scope.modelObject.ExitCustomsId);
                    else
                        $scope.selectedExitCustoms = {};

                    if (typeof $scope.modelObject.OrderUploadType != 'undefined' && $scope.modelObject.OrderUploadType != null)
                        $scope.selectedOrderUploadType = $scope.orderUploadTypeList.find(d => d.Id == $scope.modelObject.OrderUploadType);
                    else
                        $scope.selectedOrderUploadType = {};

                    if (typeof $scope.modelObject.OrderUploadPointType != 'undefined' && $scope.modelObject.OrderUploadPointType != null)
                        $scope.selectedOrderUploadPointType = $scope.orderUploadPointTypeList.find(d => d.Id == $scope.modelObject.OrderUploadPointType);
                    else
                        $scope.selectedOrderUploadPointType = {};

                    if (typeof $scope.modelObject.OrderTransactionDirectionType != 'undefined' && $scope.modelObject.OrderTransactionDirectionType != null)
                        $scope.selectedOrderTransactionDirectionType = $scope.orderTransactionDirectionTypeList.find(d => d.Id == $scope.modelObject.OrderTransactionDirectionType);
                    else
                        $scope.selectedOrderTransactionDirectionType = {};

                    if (typeof $scope.modelObject.OrderCalculationType != 'undefined' && $scope.modelObject.OrderCalculationType != null)
                        $scope.selectedOrderCalculationType = $scope.orderCalculationTypeList.find(d => d.Id == $scope.modelObject.OrderCalculationType);
                    else
                        $scope.selectedOrderCalculationType = {};

                    if (typeof $scope.modelObject.UserAuthorId != 'undefined' && $scope.modelObject.UserAuthorId != null)
                        $scope.selectedUser = $scope.usersList.find(d => d.Id == $scope.modelObject.UserAuthorId);
                    else
                        $scope.selectedUser = {};

                    if (typeof $scope.modelObject.ShipperCountryId != 'undefined' && $scope.modelObject.ShipperCountryId != null)
                        $scope.selectedShipperCountry = $scope.countryList.find(d => d.Id == $scope.modelObject.ShipperCountryId);
                    else
                        $scope.selectedShipperCountry = {};

                    if (typeof $scope.modelObject.BuyerCountryId != 'undefined' && $scope.modelObject.BuyerCountryId != null)
                        $scope.selectedBuyerCountry = $scope.countryList.find(d => d.Id == $scope.modelObject.BuyerCountryId);
                    else
                        $scope.selectedBuyerCountry = {};

                    if (typeof $scope.modelObject.CmrShipperCountryId != 'undefined' && $scope.modelObject.CmrShipperCountryId != null)
                        $scope.selectedCmrShipperCountry = $scope.countryList.find(d => d.Id == $scope.modelObject.CmrShipperCountryId);
                    else
                        $scope.selectedCmrShipperCountry = {};

                    if (typeof $scope.modelObject.CmrBuyerCountryId != 'undefined' && $scope.modelObject.CmrBuyerCountryId != null)
                        $scope.selectedCmrBuyerCountry = $scope.countryList.find(d => d.Id == $scope.modelObject.CmrBuyerCountryId);
                    else
                        $scope.selectedCmrBuyerCountry = {};

                    if (typeof $scope.modelObject.ShipperCityId != 'undefined' && $scope.modelObject.ShipperCityId != null)
                        $scope.selectedShipperCity = $scope.cityList.find(d => d.Id == $scope.modelObject.ShipperCityId);
                    else
                        $scope.selectedShipperCity = {};

                    if (typeof $scope.modelObject.BuyerCityId != 'undefined' && $scope.modelObject.BuyerCityId != null)
                        $scope.selectedBuyerCity = $scope.cityList.find(d => d.Id == $scope.modelObject.BuyerCityId);
                    else
                        $scope.selectedBuyerCity = {};

                    if (typeof $scope.modelObject.CmrShipperCityId != 'undefined' && $scope.modelObject.CmrShipperCityId != null)
                        $scope.selectedCmrShipperCity = $scope.cityList.find(d => d.Id == $scope.modelObject.CmrShipperCityId);
                    else
                        $scope.selectedCmrShipperCity = {};

                    if (typeof $scope.modelObject.CmrBuyerCityId != 'undefined' && $scope.modelObject.CmrBuyerCityId != null)
                        $scope.selectedCmrBuyerCity = $scope.cityList.find(d => d.Id == $scope.modelObject.CmrBuyerCityId);
                    else
                        $scope.selectedCmrBuyerCity = {};

                    if (typeof $scope.modelObject.ForexTypeId != 'undefined' && $scope.modelObject.ForexTypeId != null)
                        $scope.selectedForexType = $scope.forexList.find(d => d.Id == $scope.modelObject.ForexTypeId);
                    else
                        $scope.selectedForexType = {};

                    if (typeof $scope.modelObject.TrailerType != 'undefined' && $scope.modelObject.TrailerType != null)
                        $scope.selectedTrailerType = $scope.trailerTypeList.find(d => d.Id == $scope.modelObject.TrailerType);
                    else
                        $scope.selectedTrailerType = {};

                    if (typeof $scope.modelObject.FirmCustomsArrivalId != 'undefined' && $scope.modelObject.FirmCustomsArrivalId != null)
                        $scope.selectedFirmCustomsArrival = $scope.firmCustomsList.find(d => d.Id == $scope.modelObject.FirmCustomsArrivalId);
                    else
                        $scope.selectedFirmCustomsArrival = {};

                    if (typeof $scope.modelObject.FirmCustomsExitId != 'undefined' && $scope.modelObject.FirmCustomsExitId != null)
                        $scope.selectedFirmCustomsExit = $scope.firmCustomsList.find(d => d.Id == $scope.modelObject.FirmCustomsExitId);
                    else
                        $scope.selectedFirmCustomsExit = {};

                    if (typeof $scope.modelObject.LoadStatusType != 'undefined' && $scope.modelObject.LoadStatusType != null)
                        $scope.selectedLoadStatusType = $scope.loadStatusTypeList.find(d => d.Id == $scope.modelObject.LoadStatusType);
                    else
                        $scope.selectedLoadStatusType = {};

                    if (typeof $scope.modelObject.ManufacturerFirmId != 'undefined' && $scope.modelObject.ManufacturerFirmId != null)
                        $scope.selectedFirmManufacturer = $scope.firmList.find(d => d.Id == $scope.modelObject.ManufacturerFirmId);
                    else
                        $scope.selectedFirmManufacturer = {};

                    if (typeof $scope.modelObject.ReelOwnerFirmId != 'undefined' && $scope.modelObject.ReelOwnerFirmId != null)
                        $scope.selectedFirmReelOwner = $scope.firmList.find(d => d.Id == $scope.modelObject.ReelOwnerFirmId);
                    else
                        $scope.selectedFirmReelOwner = {};

                    if (typeof $scope.modelObject.VehicleTraillerId != 'undefined' && $scope.modelObject.VehicleTraillerId != null)
                        $scope.selectedVehicleTrailer = $scope.vehicleTrailerList.find(d => d.Id == $scope.modelObject.VehicleTraillerId);
                    else
                        $scope.selectedVehicleTrailer = {};

                    if (typeof $scope.modelObject.TowinfVehicleId != 'undefined' && $scope.modelObject.TowinfVehicleId != null)
                        $scope.selectedTowinfVehicle = $scope.vehicleTowinfList.find(d => d.Id == $scope.modelObject.TowinfVehicleId);
                    else
                        $scope.selectedTowinfVehicle = {};

                    if (typeof $scope.modelObject.DriverId != 'undefined' && $scope.modelObject.DriverId != null)
                        $scope.selectedDriver = $scope.driverList.find(d => d.Id == $scope.modelObject.DriverId);
                    else
                        $scope.selectedDriver = {};

                    if (typeof $scope.modelObject.VoyageStartCityId != 'undefined' && $scope.modelObject.VoyageStartCityId != null)
                        $scope.selectedVoyageStartCity = $scope.cityList.find(d => d.Id == $scope.modelObject.VoyageStartCityId);
                    else
                        $scope.selectedVoyageStartCity = {};

                    if (typeof $scope.modelObject.VoyageStartCountryId != 'undefined' && $scope.modelObject.VoyageStartCountryId != null)
                        $scope.selectedVoyageStartCountry = $scope.countryList.find(d => d.Id == $scope.modelObject.VoyageStartCountryId);
                    else
                        $scope.selectedVoyageStartCountry = {};

                    if (typeof $scope.modelObject.VoyageEndCityId != 'undefined' && $scope.modelObject.VoyageEndCityId != null)
                        $scope.selectedVoyageEndCity = $scope.cityList.find(d => d.Id == $scope.modelObject.VoyageEndCityId);
                    else
                        $scope.selectedVoyageEndCity = {};

                    if (typeof $scope.modelObject.VoyageEndCountryId != 'undefined' && $scope.modelObject.VoyageEndCountryId != null)
                        $scope.selectedVoyageEndCountry = $scope.countryList.find(d => d.Id == $scope.modelObject.VoyageEndCountryId);
                    else
                        $scope.selectedVoyageEndCountry = {};

                    $scope.bindDetails();
                    if (typeof $scope.modelObject.ItemOrderId != 'undefined' && $scope.modelObject.ItemOrderId != null) {
                        $scope.loadItemOrderDetailList();
                    }
                }
            }).catch(function (err) { });
    }

    $scope.calculateRow = function (row) {
        $scope.calculateValumeAndDesi(row);
        $scope.calculateOveralTotal();

    }
    //$scope.approveLoad = function () {
    //    bootbox.confirm({
    //        message: "Bu yük talebi onaylamak istediğinizden emin misiniz?",
    //        closeButton: false,
    //        buttons: {
    //            confirm: {
    //                label: 'Evet',
    //                className: 'btn-primary'
    //            },
    //            cancel: {
    //                label: 'Hayır',
    //                className: 'btn-light'
    //            }
    //        },
    //        callback: function (result) {
    //            if (result) {
    //                $scope.saveStatus = 1;
    //                $http.post(HOST_URL + 'Load/ApproveLoad', { rid: $scope.modelObject.Id }, 'json')
    //                    .then(function (resp) {
    //                        if (typeof resp.data != 'undefined' && resp.data != null) {
    //                            $scope.saveStatus = 0;

    //                            if (resp.data.Result) {
    //                                toastr.success('Onay işlemi başarılı.', 'Bilgilendirme');

    //                                $scope.bindModel($scope.modelObject.Id);
    //                            }
    //                            else
    //                                toastr.error(resp.data.ErrorMessage, 'Hata');
    //                        }
    //                    }).catch(function (err) { });
    //            }
    //        }
    //    });
    //}
    $scope.showOrderDetails = function () {
        $scope.orderDetailsVisible = !$scope.orderDetailsVisible;
    }
    $scope.calculateValumeAndDesi = function (row) {
        if (row.ShortWidth == 'undefined' || row.ShortWidth == null)
            row.ShortWidth = 0;
        if (row.LongWidth == 'undefined' || row.LongWidth == null)
            row.LongWidth = 0;
        if (row.Height == 'undefined' || row.Height == null)
            row.Height = 0;
        row.Volume = row.Quantity * row.ShortWidth * row.LongWidth * row.Height / 1000000;
        //CALCULATE VALUME AND WEIGHT AND LADAMETRE
        let sumVolume = 0;
        let sumWeight = 0;
        let = sumLadametre = 0;
        let = sumQuantity = 0;
        $scope.modelObject.Details.forEach(element => {
            sumVolume += parseFloat(element.Volume != null ? element.Volume : 0);
            sumWeight += parseFloat(element.Weight != null ? element.Weight : 0);
            sumLadametre += parseFloat(element.Ladametre != null ? element.Ladametre : 0);
            sumQuantity += parseFloat(element.Quantity != null ? element.Quantity : 0);
        });
        $scope.modelObject.OveralVolume = sumVolume;
        $scope.modelObject.OveralWeight = sumWeight;
        $scope.modelObject.OveralLadametre = sumLadametre;
        $scope.modelObject.OveralQuantity = sumQuantity;

    }

    $scope.calculateOveralTotal = function () {
        if ($scope.selectedOrderCalculationType.Id == 1)
            $scope.modelObject.OverallTotal = $scope.modelObject.OveralWeight * $scope.modelObject.CalculationTypePrice;
        else if ($scope.selectedOrderCalculationType.Id == 2)
            $scope.modelObject.OverallTotal = $scope.modelObject.OveralVolume * $scope.modelObject.CalculationTypePrice;
        else if ($scope.selectedOrderCalculationType.Id == 3)
            $scope.modelObject.OverallTotal = $scope.modelObject.OveralLadametre * $scope.modelObject.CalculationTypePrice;
        else if ($scope.selectedOrderCalculationType.Id == 0)
            toastr.error("Hesaplama Tip seçimi yapılmadı !", 'Hata');

    }

    $scope.bindDetails = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    $scope.modelObject.Details.forEach(d => {
                        if (d.TaxIncluded == true)
                            d.TaxIncluded = 1;
                        else if (d.TaxIncluded == false)
                            d.TaxIncluded = 0;
                    });

                    return $scope.modelObject.Details;
                },
                update: function (key, values) {
                    var obj = $scope.modelObject.Details.find(d => d.Id == key);
                    if (obj != null) {
                        let calculateRowAgain = false;

                        if (typeof values.ItemId != 'undefined') {
                            var itemObj = $scope.itemList.find(d => d.Id == values.ItemId);
                            obj.ItemId = itemObj.Id;
                            obj.ItemNo = itemObj.ItemNo;
                            obj.ItemName = itemObj.ItemName;

                            calculateRowAgain = true;
                        }
                        if (typeof values.UnitId != 'undefined') {
                            var unitObj = $scope.unitList.find(d => d.Id == values.UnitId);
                            obj.UnitId = unitObj.Id;
                            obj.UnitName = unitObj.UnitCode;
                            calculateRowAgain = true;
                        }

                        if (typeof values.Explanation != 'undefined') { obj.Explanation = values.Explanation; }
                        if (typeof values.Quantity != 'undefined') { obj.Quantity = values.Quantity; calculateRowAgain = true; }
                        if (typeof values.ShortWidth != 'undefined') { obj.ShortWidth = values.ShortWidth; calculateRowAgain = true; }
                        if (typeof values.LongWidth != 'undefined') { obj.LongWidth = values.LongWidth; calculateRowAgain = true; }
                        if (typeof values.Height != 'undefined') { obj.Height = values.Height; calculateRowAgain = true; }
                        if (typeof values.Volume != 'undefined') { obj.Volume = values.Volume; calculateRowAgain = true; }
                        if (typeof values.Weight != 'undefined') { obj.Weight = values.Weight; calculateRowAgain = true; }
                        if (typeof values.Ladametre != 'undefined') { obj.Ladametre = values.Ladametre; calculateRowAgain = true; }
                        if (typeof values.Stackable != 'undefined') { obj.Stackable = values.Stackable; calculateRowAgain = true; }
                        if (typeof values.PackageInNumber != 'undefined') { obj.PackageInNumber = values.PackageInNumber; calculateRowAgain = true; }
                        if (typeof values.Explanation != 'undefined') { obj.Explanation = values.Explanation }

                        if (calculateRowAgain)
                            $scope.calculateRow(obj);
                    }
                },
                remove: function (key) {
                    var obj = $scope.modelObject.Details.find(d => d.Id == key);
                    if (obj != null) {
                        $scope.modelObject.Details.splice($scope.modelObject.Details.indexOf(obj), 1);
                    }
                },
                insert: function (values) {
                    var newId = 1;
                    if ($scope.modelObject.Details.length > 0) {
                        newId = $scope.modelObject.Details.map(d => d.Id).reduce((max, n) => n > max ? n : max)
                        newId++;
                    }

                    var itemObj = $scope.itemList.find(d => d.Id == values.ItemId);
                    var unitObj = $scope.unitList.find(d => d.Id == values.UnitId);

                    var newObj = {
                        Id: newId,
                        ItemId: itemObj.Id,
                        ItemNo: itemObj.ItemNo,
                        ItemName: itemObj.ItemName,
                        UnitId: typeof unitObj != 'undefined' && unitObj != null ? unitObj.Id : null,
                        UnitName: typeof unitObj != 'undefined' && unitObj != null ? unitObj.UnitCode : null,
                        Quantity: values.Quantity,
                        ShortWidth: values.ShortWidth,
                        LongWidth: values.LongWidth,
                        Height: values.Height,
                        Weight: values.Weight,
                        Volume: values.Volume,
                        Stackable: values.Stackable,
                        PackageInNumber: values.PackageInNumber,
                        //Desi: values.Desi,
                        Ladametre: values.Ladametre,
                        //TaxIncluded: values.TaxIncluded,
                        //UnitPrice: values.UnitPrice,
                        //ForexRate: values.ForexRate,
                        //ForexUnitPrice: values.ForexUnitPrice,
                        //ForexId: values.ForexId,
                        ItemRequestDetailId: null,
                        Explanation: values.Explanation,
                        NewDetail: true
                    };

                    $scope.modelObject.Details.push(newObj);
                    $scope.calculateRow(newObj);
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
            height: 280,
            editing: {
                allowUpdating: true,
                allowDeleting: true,
                allowAdding: true,
                mode: 'cell'
            },
            allowColumnResizing: true,
            wordWrapEnabled: true,
            onInitNewRow: function (e) {
                e.data.UnitPrice = 0;
                e.data.TaxIncluded = 0;
            },
            repaintChangesOnly: true,
            onCellPrepared: function (e) {
                if (e.rowType === "data") {
                    if (e.data.OrderStatus == 5) {
                        e.cellElement.css("background-color", "Green");
                        //e.cellElement.css("color", "white");
                    }
                }
            },
            columns: [
                {
                    dataField: 'ItemId', caption: 'Mal Kodu',
                    lookup: {
                        dataSource: $scope.itemList,
                        valueExpr: "Id",
                        displayExpr: "ItemNo"
                    },
                    allowSorting: false,
                    width: 70,
                    validationRules: [{ type: "required" }],
                    editCellTemplate: $scope.dropDownBoxEditorTemplate,
                    cellTemplate: function (container, options) {
                        if (typeof options.row.data.ItemNo != 'undefined'
                            && options.row.data.ItemNo != null && options.row.data.ItemNo.length > 0)
                            container.text(options.row.data.ItemNo);
                        else
                            container.text(options.displayValue);
                    }
                },
                { dataField: 'ItemName', caption: 'Mal Adı', allowEditing: false },
                {
                    dataField: 'UnitId', caption: 'Birim',
                    allowSorting: false,
                    lookup: {
                        dataSource: $scope.unitList,
                        valueExpr: "Id",
                        displayExpr: "UnitCode"
                    }, width: 60
                },
                { dataField: 'Quantity', caption: 'Miktar', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, validationRules: [{ type: "required" }] },
                { dataField: 'ShortWidth', caption: 'Kısa En (CM)', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'LongWidth', caption: 'Uzun En (CM)', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'Height', caption: 'Yükseklik (CM)', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'Weight', caption: 'Ağırlık (KG)', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'Volume', caption: 'Hacim (M3)', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, allowEditing: false },
                { dataField: 'Ladametre', caption: 'Ladametre', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'Stackable', caption: 'İstiflenebilir', dataType: 'boolean', width: 90 },
                { dataField: 'PackageInNumber', caption: 'Koli iç Adet', width: 90 },
                { dataField: 'Explanation', caption: 'Açıklama', dataType: 'text', width: 120 },
                {
                    type: "buttons",
                    buttons: [
                        {
                            name: 'delete', cssClass: '', text: '', onClick: function (e) {
                                $('#dataList').dxDataGrid('instance').deleteRow(e.row.rowIndex);
                            }
                        }
                        //{
                        //    name: 'preview', cssClass: 'btn btn-sm btn-light-primary py-0 px-1', text: '...', onClick: function (e) {
                        //        var dataGrid = $("#dataList").dxDataGrid("instance");
                        //        $scope.selectedRow = e.row.data;
                        //        $scope.showRowMenu();
                        //    }
                        //}
                    ]
                }
            ]
            ,
            summary: {
                totalItems: [{
                    column: 'Quantity',
                    summaryType: 'sum',
                },
                {
                    column: 'Weight',
                    summaryType: 'sum',
                    format: { type: "fixedPoint", precision: 2 }
                },
                {
                    column: 'Volume',
                    summaryType: 'sum',
                    format: { type: "fixedPoint", precision: 2 }
                },
                {
                    column: 'Ladametre',
                    summaryType: 'sum',
                    format: { type: "fixedPoint", precision: 2 }
                }]
            },
        });
    }

    $scope.loadItemOrderDetailList = function () {
        $('#orderDetailList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $.getJSON(HOST_URL + 'LOrder/GetItemOrderDetails?ItemOrderId=' + $scope.modelObject.ItemOrderId, function (data) {

                    });
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
            allowColumnResizing: true,
            wordWrapEnabled: true,
            onInitNewRow: function (e) {
                e.data.UnitPrice = 0;
                e.data.TaxIncluded = 0;
            },
            repaintChangesOnly: true,
            onCellPrepared: function (e) {
                if (e.rowType === "data") {
                    e.cellElement.css("background-color", "Green");
                    //e.cellElement.css("color", "white");
                }
            },
            columns: [
                {
                    dataField: 'ItemId', caption: 'Mal Kodu',
                    lookup: {
                        dataSource: $scope.itemList,
                        valueExpr: "Id",
                        displayExpr: "ItemNo"
                    },
                    allowSorting: false,
                    width: 70,
                    validationRules: [{ type: "required" }],
                    editCellTemplate: $scope.dropDownBoxEditorTemplate,
                    cellTemplate: function (container, options) {
                        if (typeof options.row.data.ItemNo != 'undefined'
                            && options.row.data.ItemNo != null && options.row.data.ItemNo.length > 0)
                            container.text(options.row.data.ItemNo);
                        else
                            container.text(options.displayValue);
                    }
                },
                { dataField: 'ItemName', caption: 'Mal Adı', allowEditing: false },
                {
                    dataField: 'UnitId', caption: 'Birim',
                    allowSorting: false,
                    lookup: {
                        dataSource: $scope.unitList,
                        valueExpr: "Id",
                        displayExpr: "UnitCode"
                    }, width: 60
                },
                { dataField: 'Quantity', caption: 'Miktar', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, validationRules: [{ type: "required" }] },
                { dataField: 'ShortWidth', caption: 'Kısa En (CM)', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'LongWidth', caption: 'Uzun En (CM)', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'Height', caption: 'Yükseklik (CM)', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'Weight', caption: 'Ağırlık (KG)', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'Volume', caption: 'Hacim (M3)', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, allowEditing: false },
                { dataField: 'Ladametre', caption: 'Ladametre', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'Stackable', caption: 'İstiflenebilir', dataType: 'boolean', width: 90 },
                { dataField: 'PackageInNumber', caption: 'Koli iç Adet', width: 90 },
                { dataField: 'Explanation', caption: 'Açıklama' },
                {
                    type: "buttons",
                    buttons: [
                        {
                            name: 'delete', cssClass: '', text: '', onClick: function (e) {
                                $('#dataList').dxDataGrid('instance').deleteRow(e.row.rowIndex);
                            }
                        }
                    ]
                }
            ]
            ,
            summary: {
                totalItems: [{
                    column: 'Quantity',
                    summaryType: 'sum',
                },
                {
                    column: 'Weight',
                    summaryType: 'sum',
                    format: { type: "fixedPoint", precision: 2 }
                },
                {
                    column: 'Volume',
                    summaryType: 'sum',
                    format: { type: "fixedPoint", precision: 2 }
                },
                {
                    column: 'Ladametre',
                    summaryType: 'sum',
                    format: { type: "fixedPoint", precision: 2 }
                }]
            },
        });
    }

    $scope.findAddressCmrShipperFirm = function () {
        if (typeof $scope.selectedCmrShipperFirm != 'undefined' && $scope.selectedCmrShipperFirm != null) {
            $scope.selectedCmrShipperCountry.CountryName = "";
            $scope.selectedCmrShipperCountry = $scope.countryList.find(d => d.Id == $scope.selectedCmrShipperFirm.CountryId);
            $scope.selectedCmrShipperCity.CityName = "";
            $scope.selectedCmrShipperCity = $scope.cityList.find(d => d.Id == $scope.selectedCmrShipperFirm.CityId);
            $scope.modelObject.CmrShipperFirmAddress = "";
            $scope.modelObject.CmrShipperFirmAddress = $scope.selectedCmrShipperFirm.Address;
        }
        else
            $scope.selectedCmrShipperFirm = {};
    }
    $scope.findAddressCmrBuyerFirm = function () {
        if (typeof $scope.selectedCmrBuyerFirm != 'undefined' && $scope.selectedCmrBuyerFirm != null) {
            $scope.selectedCmrBuyerCountry.CountryName = "";
            $scope.selectedCmrBuyerCountry = $scope.countryList.find(d => d.Id == $scope.selectedCmrBuyerFirm.CountryId);
            $scope.selectedCmrBuyerCity.CityName = "";
            $scope.selectedCmrBuyerCity = $scope.cityList.find(d => d.Id == $scope.selectedCmrBuyerFirm.CityId);
            $scope.modelObject.CmrBuyerFirmAddress = "";
            $scope.modelObject.CmrBuyerFirmAddress = $scope.selectedCmrBuyerFirm.Address;
        }
        else
            $scope.selectedCmrBuyerFirm = {};
    }
    $scope.loadSelectables = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Load/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.itemList = resp.data.Items;
                        $scope.unitList = resp.data.Units;
                        $scope.firmList = resp.data.Firms;
                        $scope.forexList = resp.data.Forexes;
                        $scope.customsList = resp.data.Customs;
                        $scope.usersList = resp.data.Users;
                        $scope.cityList = resp.data.Citys;
                        $scope.countryList = resp.data.Countrys;
                        $scope.firmCustomsList = resp.data.FirmCustoms;
                        $scope.vehicleTrailerList = resp.data.VehicleTrailers;
                        $scope.vehicleTowinfList = resp.data.VehicleTowinfs;
                        $scope.driverList = resp.data.Drivers;
                        resolve();
                    }
                }).catch(function (err) { });
        });

        return prms;
    }
    //$scope.loadItemOrderDetailList = function () {
    //    var prms = new Promise(function (resolve, reject) {
    //        $http.get(HOST_URL + 'LOrder/GetItemOrderDetails?ItemOrderId='+$scope.modelObject.ItemOrderId, 'json')
    //            .then(function (resp) {
    //                if (typeof resp.data != 'undefined' && resp.data != null) {
    //                    $scope.itemOrderDetailList = resp.data.result;
    //                    console.log(resp.data);
    //                   console.log(resp.data.result);
    //                    resolve();
    //                }
    //            }).catch(function (err) { });
    //    });

    //    return prms;
    //}

    //$scope.showRequestInformation = function () {
    //    $scope.$broadcast('loadRelatedRequestList', $scope.modelObject.Id);

    //    $('#dial-related-requests').dialog({
    //        width: 500,
    //        //height: window.innerHeight * 0.6,
    //        hide: true,
    //        modal: true,
    //        resizable: false,
    //        show: true,
    //        draggable: false,
    //        closeText: "KAPAT"
    //    });
    //}

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


    //$scope.showItemRequestList = function () {
    //    // DO BROADCAST
    //    $scope.$broadcast('loadApprovedRequestDetails');

    //    $('#dial-requests').dialog({
    //        width: window.innerWidth * 0.6,
    //        height: window.innerHeight * 0.6,
    //        hide: true,
    //        modal: true,
    //        resizable: false,
    //        show: true,
    //        draggable: false,
    //        closeText: "KAPAT"
    //    });
    //}
    // Loading Firm Address
    $scope.$on('loadShipperFirmAddressEnd', function (e, d) {

        d.forEach(x => {

            $scope.selectedShipperCountry.CountryName = "";
            $scope.selectedShipperCountry = {}
            $scope.selectedShipperCountry = $scope.countryList.find(d => d.Id == x.CountryId);
            $scope.selectedShipperCity.CityName = "";
            $scope.selectedShipperCity = {}
            $scope.selectedShipperCity = $scope.cityList.find(d => d.Id == x.CityId);
            $scope.modelObject.ShipperFirmAddress = "";
            $scope.modelObject.ShipperFirmAddress = x.Address1;
        });

        $('#dial-ShipperFirmAddress').dialog('close');
    });
    $scope.$on('loadBuyerFirmAddressEnd', function (e, d) {

        d.forEach(x => {

            $scope.selectedBuyerCountry.CountryName = "";
            $scope.selectedBuyerCountry = {}
            $scope.selectedBuyerCountry = $scope.countryList.find(d => d.Id == x.CountryId);
            $scope.selectedBuyerCity.CityName = "";
            $scope.selectedBuyerCity = {}
            $scope.selectedBuyerCity = $scope.cityList.find(d => d.Id == x.CityId);
            $scope.modelObject.BuyerFirmAddress = "";
            $scope.modelObject.BuyerFirmAddress = x.Address1;
        });

        $('#dial-BuyerFirmAddress').dialog('close');
    });
    // #endregion


    // ON LOAD EVENTS
    DevExpress.localization.locale('tr');
    $scope.loadSelectables().then(function () {
        if (PRM_ID > 0) {
            $scope.bindModel(PRM_ID);
        }
        else {
            $scope.bindModel(0);

        }
    });

});