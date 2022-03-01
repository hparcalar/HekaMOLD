app.controller('voyageCtrl', function ($scope, $http) {
    $scope.modelObject = { Id: 0, VoayageDateStr: moment().format('DD.MM.YYYY'), VoyageDetails: []};

    $scope.driverList = [];
    $scope.plannedLoadList = [];
    $scope.rotaList = [];
    $scope.vehicleList = [];
    $scope.firmList = [];
    $scope.cDoorList = [];

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

    $scope.traillerTypeList = [{ Id: 1, Text: 'Çadırlı' },
    { Id: 2, Text: 'Frigo' }, { Id: 3, Text: 'Kapalı Kasa' }, { Id: 4, Text: 'Optima' }, { Id: 5, Text: 'Mega' }
        , { Id: 6, Text: 'Konteyner' }, { Id: 7, Text: 'Swapboddy' }, { Id: 8, Text: 'Lowbed' }
        , { Id: 9, Text: 'Kamyon Romörk' }, { Id: 10, Text: 'Standart' }, { Id: 10, Text: 'Minivan' }];

    $scope.saveStatus = 0;

    // RECEIPT FUNCTIONS
    $scope.getNextOrderNo = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'LOrder/GetNextOrderNo', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        if (resp.data.Result) {
                            resolve(resp.data.ReceiptNo);
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
    // SELECTABLES
    $scope.getNextRecord = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'LOrder/GetNextRecord?Id=' + $scope.modelObject.Id, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        if (resp.data.Result) {
                            window.location.href = HOST_URL + 'LOrder?rid=' + resp.data.NextNo;
                        }
                        else {
                            toastr.warning('Sıradaki sipariş numarasına ulaşılamadı. Lütfen ekranı yenileyip tekrar deneyiniz.', 'Uyarı');
                            resolve('');
                        }
                    }
                }).catch(function (err) { });
        });

        return prms;
    }
    $scope.getBackRecord = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'LOrder/GetBackRecord?Id=' + $scope.modelObject.Id, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        if (resp.data.Result) {
                            window.location.href = HOST_URL + 'LOrder?rid=' + resp.data.NextNo;
                        }
                        else {
                            toastr.warning('Sıradaki sipariş numarasına ulaşılamadı. Lütfen ekranı yenileyip tekrar deneyiniz.', 'Uyarı');
                            resolve('');
                        }
                    }
                }).catch(function (err) { });
        });

        return prms;
    }
    // CRUD
    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0, VoyageDateStr: moment().format('DD.MM.YYYY'), VoyageDetails: [], OrderStatus: 0 };

        $scope.selectedDriver = {}
        $scope.selectedTraillerVehicle = {}
        $scope.selectedOrderTransactionDirectionType = {};
        $scope.bindVoyageDetails();
    }
    $scope.loadSelectables = function () {
        var prmReq = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'LPlanning/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.vehicleList = resp.data.Vehicles;
                        $scope.driverList = resp.data.Drivers;
                        $scope.VoyageDateStr = moment().format('DD.MM.YYYY')
                        $scope.rotaList = resp.data.Rotas;
                        $scope.firmList = resp.data.Firms;
                        $scope.cDoorList = resp.data.CDoors;

                        resolve(resp.data);
                    }
                }).catch(function (err) { });
        });

        return prmReq;
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

    $scope.bindModel = function (id) {
        $http.get(HOST_URL + 'Voyage/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;
                    console.log(resp.data);
                    //$scope.modelObject.DateOfNeed = $scope.modelObject.DateOfNeedStr;
                    //$scope.modelObject.OrderDate = $scope.modelObject.OrderDateStr;

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
                        $scope.selectedTraillerVehicle = $scope.vehicleList.find(d => d.Id == $scope.modelObject.TraillerVehicleId);
                    else
                        $scope.selectedTraillerVehicle = {};


                    if ($scope.modelObject.TowinfVehicleId > 0)
                        $scope.selectedTowinfVehicle = $scope.vehicleList.find(d => d.Id == $scope.modelObject.TowinfVehicleId);
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

                    $scope.bindVoyageDetails();
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

                        if (typeof values.RotaId != 'undefined') {
                            var rotaObj = $scope.rotaList.find(d => d.Id == values.RotaId);
                            obj.RotaId = itemObj.Id;
                            obj.CityStartName = rotaObj.CityStartName;
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
            height: 200,
            editing: {
                allowDeleting: true,
                allowEditing: true,
                mode: 'cell'
            },
            repaintChangesOnly: true,
            columns: [
                { dataField: 'DischargeLineNo', caption: 'Boşaltma Sırası', allowEditing: true },
                { dataField: 'LoadCode', caption: 'Yük Kodu', allowEditing: false },
                { dataField: 'LoadingDateStr', caption: 'Yükleme Tarih', dataType: 'date', format: 'dd.MM.yyyy', allowEditing: false },
                { dataField: 'CustomerFirmName', caption: 'Firma', allowEditing: false },
                { dataField: 'OrderTransactionDirectionTypeStr', caption: 'İşlem Yönü', allowEditing: false },
                { dataField: 'OveralQuantity', caption: 'Toplam Miktar', allowEditing: false, dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'OveralWeight', caption: 'Toplam Ağırlık(KG)', allowEditing: false },
                { dataField: 'OveralLadametre', caption: 'Toplam Ladametre', allowEditing: false, dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'OveralVolume', caption: 'Toplam Hacim(M3)', allowEditing: false, dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                {
                    dataField: 'RotaId', caption: 'Rota',
                    lookup: {
                        dataSource: $scope.rotaList,
                        valueExpr: "Id",
                        displayExpr: "CityStartName"
                    },
                    allowSorting: false,
                    validationRules: [{ type: "required" }],
                    editCellTemplate: $scope.dropDownBoxEditorTemplate,
                    cellTemplate: function (container, options) {
                        if (typeof options.row.data.CityStartName != 'undefined'
                            && options.row.data.CityStartName != null && options.row.data.CityStartName.length > 0)
                            container.text(options.row.data.CityStartName);
                        else
                            container.text(options.displayValue);
                    }
                },
            ]
        });
    }
    // APPROVALS
    $scope.approveLOrder = function () {
        bootbox.confirm({
            message: "Bu Sipariş talebi onaylamak istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'LOrder/ApproveOrderPrice', { rid: $scope.modelObject.Id }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Result) {
                                    toastr.success('Onay işlemi başarılı.', 'Bilgilendirme');

                                    $scope.bindModel($scope.modelObject.Id);
                                }
                                else
                                    toastr.error(resp.data.ErrorMessage, 'Hata');
                            }
                        }).catch(function (err) { });
                }
            }
        });
    }

    $scope.cancelledLOrder = function () {
        bootbox.confirm({
            message: "Bu Sipariş talebi İptal etmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'LOrder/CancelledOrderPrice', { rid: $scope.modelObject.Id }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Result) {
                                    toastr.success('İptal işlemi başarılı.', 'Bilgilendirme');

                                    $scope.bindModel($scope.modelObject.Id);
                                }
                                else
                                    toastr.error(resp.data.ErrorMessage, 'Hata');
                            }
                        }).catch(function (err) { });
                }
            }
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

    // ROW MENU ACTIONS
    $scope.showRowMenu = function () {
        if ($scope.selectedRow && $scope.selectedRow.Id > 0) {
            $scope.$apply();

            $('#dial-row-menu').dialog({
                width: 300,
                //height: window.innerHeight * 0.6,
                hide: true,
                modal: true,
                resizable: false,
                show: true,
                draggable: false,
                closeText: "KAPAT"
            });
        }
    }
    $scope.toggleOrderDetailStatus = function () {
        if ($scope.selectedRow && $scope.selectedRow.Id > 0) {
            $('#dial-row-menu').dialog("close");

            bootbox.confirm({
                message: "Bu sipariş kaleminin durumunu değiştirmek istediğinizden emin misiniz?",
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
                        $http.post(HOST_URL + 'LOrder/ToggleOrderDetailStatus', { detailId: $scope.selectedRow.Id }, 'json')
                            .then(function (resp) {
                                if (typeof resp.data != 'undefined' && resp.data != null) {
                                    $scope.saveStatus = 0;

                                    if (resp.data.Status == 1) {
                                        toastr.success('İşlem başarılı.', 'Bilgilendirme');

                                        $scope.bindModel($scope.modelObject.Id);
                                    }
                                    else
                                        toastr.error(resp.data.ErrorMessage, 'Hata');
                                }
                            }).catch(function (err) { });
                    }
                }
            });
        }
    }

    // APPROVALS
    $scope.approveOrderPrice = function () {
        bootbox.confirm({
            message: "Bu siparişin fiyatını onaylamak istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'LOrder/ApproveOrderPrice', { rid: $scope.modelObject.Id }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Result) {
                                    toastr.success('Onay işlemi başarılı.', 'Bilgilendirme');

                                    $scope.bindModel($scope.modelObject.Id);
                                }
                                else
                                    toastr.error(resp.data.ErrorMessage, 'Hata');
                            }
                        }).catch(function (err) { });
                }
            }
        });
    }

    // ON LOAD EVENTS
    DevExpress.localization.locale('tr');
    $scope.loadSelectables().then(function () {
        if (PRM_ID > 0)
            $scope.bindModel(PRM_ID);
        else {
            $scope.getNextOrderNo().then(function (rNo) {
                $scope.modelObject.OrderNo = rNo;
                $scope.$apply();

                $scope.bindVoyageDetails();
            });
        }
    });
});