app.controller('itemEntryCtrl', function ($scope, $http) {
    $scope.modelObject = {
        Id:0,
        DocumentNo: '', FirmId: 0,
        FirmCode:'', FirmName:'',
        Details: []
    };

    $scope.bindModel = function (id) {
        $http.get(HOST_URL + 'Mobile/GetItemEntryData?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;

                    if (typeof $scope.modelObject.InWarehouseId != 'undefined' && $scope.modelObject.InWarehouseId != null)
                        $scope.selectedWarehouse = $scope.warehouseList.find(d => d.Id == $scope.modelObject.InWarehouseId);
                    else
                        $scope.selectedWarehouse = $scope.warehouseList[0];

                    if (typeof $scope.modelObject.FirmId != 'undefined' && $scope.modelObject.FirmId != null)
                        $scope.selectedFirm = $scope.firmList.find(d => d.Id == $scope.modelObject.FirmId);
                    else
                        $scope.selectedFirm = $scope.firmList[0];

                    refreshArray($scope.warehouseList);
                    refreshArray($scope.firmList);
                }
            }).catch(function (err) { });
    }

    $scope.selectedWarehouse = { Id: 0, WarehouseName: '' };
    $scope.warehouseList = [];

    $scope.selectedFirm = { Id: 0, FirmCode: '', FirmName : '' };
    $scope.firmList = [];

    $scope.showOrderList = function () {
        // DO BROADCAST
        $scope.$broadcast('loadOpenPoList');

        $('#dial-orderlist').dialog({
            width: window.innerWidth * 0.95,
            height: window.innerHeight * 0.95,
            hide: true,
            modal: true,
            resizable: false,
            show: true,
            draggable: false,
            closeText: "KAPAT"
        });
    }

    $scope.showItemList = function () {
        // DO BROADCAST
        $scope.$broadcast('loadItemList');

        $('#dial-itemlist').dialog({
            width: window.innerWidth * 0.95,
            height: window.innerHeight * 0.95,
            hide: true,
            modal: true,
            resizable: false,
            show: true,
            draggable: false,
            closeText: "KAPAT"
        });
    }

    $scope.loadSelectables = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Mobile/GetSelectables?action=ItemEntry', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {

                        $scope.warehouseList = resp.data.Warehouses;
                        var emptyWrObj = { Id: 0, WarehouseName: '-- Seçiniz --' };
                        $scope.warehouseList.splice(0, 0, emptyWrObj);
                        $scope.selectedWarehouse = emptyWrObj;

                        $scope.firmList = resp.data.Firms;
                        var emptyFirmObj = { Id: 0, FirmCode: '-- Seçiniz --', FirmName: '' };
                        $scope.firmList.splice(0, 0, emptyFirmObj);
                        $scope.selectedFirm = emptyFirmObj;

                        resolve();
                    }
                }).catch(function (err) { });
        });

        return prms;
    }

    $scope.approveItemEntry = function () {
        bootbox.confirm({
            message: "Bu irsaliye girişini onaylıyor musunuz?",
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

                    $scope.modelObject.ReceiptDate = moment().format('DD.MM.YYYY');

                    if (typeof $scope.selectedWarehouse != 'undefined'
                        && $scope.selectedWarehouse != null)
                        $scope.modelObject.InWarehouseId = $scope.selectedWarehouse.Id;
                    else
                        $scope.modelObject.InWarehouseId = null;

                    if (typeof $scope.selectedFirm != 'undefined' && $scope.selectedFirm != null)
                        $scope.modelObject.FirmId = $scope.selectedFirm.Id;
                    else
                        $scope.modelObject.FirmId = null;

                    $http.post(HOST_URL + 'Mobile/SaveItemEntry', $scope.modelObject, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Status == 1) {
                                    toastr.success('Kayıt başarılı.', 'Bilgilendirme');

                                    $scope.modelObject = {
                                        DocumentNo: '', FirmId: 0,
                                        FirmCode: '', FirmName: '',
                                        Details: []
                                    };
                                }
                                else
                                    toastr.error(resp.data.ErrorMessage, 'Hata');
                            }
                        }).catch(function (err) { });
                }
            }
        });
    }

    // EMIT ORDER DETAIL DATA
    $scope.$on('transferOrderDetails', function (e, d) {
        var firstOrderRecord = null;

        d.forEach(x => {
            if (firstOrderRecord == null)
                firstOrderRecord = x;

            if ($scope.modelObject.Details.filter(m => m.ItemOrderDetailId == x.Id).length > 0) {
                toastr.warning(x.OrderNo + ' nolu sipariş, ' + x.ItemNo + ' / ' + x.ItemName + ', ' + x.Quantity
                    + ' miktarlı sipariş detayı zaten aktarıldığı için tekrar dahil edilmedi.', 'Uyarı');
            }
            else {
                var newId = 1;
                if ($scope.modelObject.Details.length > 0) {
                    newId = $scope.modelObject.Details.map(d => d.Id).reduce((max, n) => n > max ? n : max)
                    newId++;
                }

                $scope.modelObject.Details.push({
                    Id: newId,
                    FirmName:x.FirmName,
                    ItemId: x.ItemId,
                    ItemNo: x.ItemNo,
                    ItemName: x.ItemName,
                    UnitId: x.UnitId,
                    UnitCode: x.UnitCode,
                    UnitName: x.UnitCode,
                    Quantity: x.Quantity,
                    UnitPrice: 0,
                    NewDetail: true,
                    ItemOrderDetailId: x.Id
                });
            }
        });

        if (firstOrderRecord != null) {
            $scope.modelObject.FirmId = firstOrderRecord.FirmId;

            if (typeof $scope.modelObject.FirmId != 'undefined' && $scope.modelObject.FirmId != null)
                $scope.selectedFirm = $scope.firmList.find(d => d.Id == $scope.modelObject.FirmId);
            else
                $scope.selectedFirm = $scope.firmList[0];

            refreshArray($scope.firmList);

            $scope.modelObject.FirmCode = firstOrderRecord.FirmCode;
            $scope.modelObject.FirmName = firstOrderRecord.FirmName;
        }

        $('#dial-orderlist').dialog('close');
    });

    $scope.$on('transferItems', function (e, d) {
        d.forEach(x => {
            var newId = 1;
            if ($scope.modelObject.Details.length > 0) {
                newId = $scope.modelObject.Details.map(d => d.Id).reduce((max, n) => n > max ? n : max)
                newId++;
            }

            $scope.modelObject.Details.push({
                Id: newId,
                ItemId: x.Id,
                ItemNo: x.ItemNo,
                ItemName: x.ItemName,
                Quantity: 0,
                UnitPrice: 0,
                NewDetail: true,
            });
        });

        $('#dial-itemlist').dialog('close');
    });

    // LOAD EVENTS
    $scope.loadSelectables().then(function () {
        refreshArray($scope.warehouseList);

        var recordId = getParameterByName('rid');
        if (typeof recordId != 'undefined' &&
            recordId != null && recordId.length > 0) {
            $scope.bindModel(parseInt(recordId));
        }
    });
});