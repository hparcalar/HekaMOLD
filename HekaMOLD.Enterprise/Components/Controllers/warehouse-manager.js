app.controller('warehouseManagerCtrl', function ($scope, $http,$timeout) {
    DevExpress.localization.locale('tr');

    $scope.modelObject = {
        Id: 0,
        Quantity: 0,
        CompleteQuantity: 0,
        WastageQuantity: 0,
        LabelConfigData: {
            ShowFirm: true,
        }
    };

    $scope.productSelection = false;
    $scope.inverseSelectionList = [];

    $scope.tpIndex = 0;
    $scope.packageList = [];
    $scope.itemList = [];
    $scope.warehouseList = [];
    $scope.selectedWarehouse = { Id: 0, WarehouseName: '' };
    $scope.selectedItem = { ItemId: 0, ItemName: '' };
    $scope.selectedPackage = { Id: 0 };

    $scope.onWarehouseChanged = function (e) {
        $timeout(function () {
            $scope.selectedWarehouse = e;
            $scope.bindList();
        });

        try {
            $scope.$applyAsync();
        } catch (e) {

        }
    }

    $scope.setPage = function (pageIndex) {
        $scope.tpIndex = pageIndex;

        $timeout(function () {
            if ($scope.tpIndex == 0) {
                $scope.bindList();
            }
            else if ($scope.tpIndex == 1) {
                $scope.bindPackages();
            }
        });

        try {
            $scope.$applyAsync();
        } catch (e) {

        }
    }

    $scope.changePackageQuantity = function () {
        bootbox.prompt({
            title: "Koli içi miktarı giriniz",
            centerVertical: true,
            callback: function (result) {
                if (result != null && result.length > 0) {
                    var newQty = parseInt(result);
                    if (newQty <= 0) {
                        toastr.error('Miktar 0 dan büyük olmalıdır.');
                        return;
                    }

                    $http.post(HOST_URL + 'Warehouse/UpdateItemSerial', {
                        serialId: $scope.selectedPackage.Id,
                        newQuantity: newQty,
                    }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                if (resp.data.Result) {
                                    toastr.success('İşlem başarılı.', 'Bilgilendirme');

                                    $timeout(function () {
                                        $scope.bindPackages();
                                    });
                                }
                                else
                                    toastr.error(resp.data.ErrorMessage, 'Hata');
                            }
                        }).catch(function (err) { });
                }
            }
        });
    }

    $scope.removePackage = function () {
        bootbox.confirm({
            message: "Silmek istediğinizden emin misiniz?",
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
            callback: function(result) {
                if (result) {
                    $http.post(HOST_URL + 'Warehouse/DeleteItemSerial', {
                        serialId: $scope.selectedPackage.Id,
                    }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                if (resp.data.Result) {
                                    toastr.success('İşlem başarılı.', 'Bilgilendirme');

                                    $timeout(function () {
                                        $scope.bindPackages();
                                    });
                                }
                                else
                                    toastr.error(resp.data.ErrorMessage, 'Hata');
                            }
                        }).catch(function (err) { });
                }
            }
        });
    }

    $scope.bindList = function () {
        $http.get(HOST_URL + 'Warehouse/GetStatesData?warehouseList=' + $scope.selectedWarehouse.Id + '&all=1', {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.itemList = resp.data;

                    $('#dataList').dxDataGrid({
                        dataSource: {
                            load: function () {
                                return $scope.itemList;
                            },
                            key: ['ItemId']
                        },
                        showColumnLines: false,
                        showRowLines: true,
                        rowAlternationEnabled: true,
                        allowColumnResizing: true,
                        wordWrapEnabled: true,
                        focusedRowEnabled: true,
                        showBorders: true,
                        filterRow: {
                            visible: true
                        },
                        headerFilter: {
                            visible: true
                        },
                        paging: {
                            enabled: false,
                            pageSize: 13,
                            pageIndex: 0
                        },
                        scrolling: {
                            mode: "virtual",
                            columnRenderingMode: "virtual"
                        },
                        height: parseInt($('body').height() * 0.6),
                        groupPanel: {
                            visible: false
                        },
                        editing: {
                            allowUpdating: false,
                            allowDeleting: false
                        },
                        columns: [
                            { dataField: 'ItemNo', caption: 'Stok Kodu' },
                            { dataField: 'ItemName', caption: 'Stok Adı' },
                            { dataField: 'ItemGroupName', caption: 'Grup' },
                            { dataField: 'TotalQty', caption: 'Mevcut Miktar' },
                            {
                                type: "buttons",
                                buttons: [
                                    {
                                        name: 'preview', cssClass: 'fas fa-edit', text: '', onClick: function (e) {
                                            var dataGrid = $("#dataList").dxDataGrid("instance");
                                            $scope.selectedItem = e.row.data;

                                            $scope.setPage(1);

                                            try {
                                                $scope.$applyAsync();
                                            } catch (e) {

                                            }
                                        }
                                    }
                                ]
                            }
                        ],
                        summary: {
                            totalItems: [{
                                column: "InQty",
                                summaryType: "sum",
                            }, {
                                column: "OutQty",
                                summaryType: "sum",
                            },
                            {
                                column: "TotalQty",
                                summaryType: "sum",
                            }
                            ]
                        }
                    });
                }
            }).catch(function (err) { });
    }

    $scope.bindPackages = function () {
        $http.get(HOST_URL + 'Warehouse/GetCurrentSerials?warehouseId='
            + $scope.selectedWarehouse.Id + '&itemId=' + $scope.selectedItem.ItemId, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.packageList = resp.data;

                    $('#packList').dxDataGrid({
                        dataSource: {
                            load: function () {
                                return $scope.packageList;
                            },
                            key: ['Id']
                        },
                        showColumnLines: false,
                        showRowLines: true,
                        rowAlternationEnabled: false,
                        allowColumnResizing: true,
                        wordWrapEnabled: true,
                        focusedRowEnabled: true,
                        showBorders: true,
                        filterRow: {
                            visible: true
                        },
                        headerFilter: {
                            visible: true
                        },
                        paging: {
                            enabled: false,
                            pageSize: 13,
                            pageIndex: 0
                        },
                        scrolling: {
                            mode: "virtual",
                            columnRenderingMode: "virtual"
                        },
                        height: parseInt($('body').height() * 0.6),
                        groupPanel: {
                            visible: false
                        },
                        editing: {
                            allowUpdating: false,
                            allowDeleting: false
                        },
                        onRowPrepared: function (e) {
                            if (e.rowType != "data")
                                return;

                            if ($scope.productSelection == true) {
                                var rowData = e.data;

                                if ($scope.inverseSelectionList.some(m => m.SerialNo == rowData.SerialNo))
                                    e.rowElement.addClass('bg-light-success');
                            }
                        },
                        columns: [
                            { dataField: 'SerialNo', caption: 'Barkod' },
                            { dataField: 'CreatedDateStr', caption: 'Tarih' },
                            { dataField: 'MachineCode', caption: 'Makine' },
                            { dataField: 'FirstQuantity', caption: 'Koli İçi Adet' },
                            {
                                type: "buttons",
                                buttons: [
                                    {
                                        name: 'editPack', cssClass: 'fas fa-edit', text: '',
                                        visible: $scope.productSelection == false,
                                        onClick: function (e) {
                                            var dataGrid = $("#packList").dxDataGrid("instance");
                                            $scope.selectedPackage = e.row.data;
                                            $scope.changePackageQuantity();
                                        }
                                    },
                                    {
                                        name: 'removePack', cssClass: 'fas fa-trash text-danger', text: '',
                                        visible: $scope.productSelection == false,
                                        onClick: function (e) {
                                            var dataGrid = $("#packList").dxDataGrid("instance");
                                            $scope.selectedPackage = e.row.data;
                                            $scope.removePackage();
                                        }
                                    },
                                    {
                                        name: 'selectPack', cssClass: 'fas fa-play text-success', text: '',
                                        visible: $scope.productSelection == true,
                                        onClick: function (e) {
                                            var dataGrid = $("#packList").dxDataGrid("instance");
                                            $scope.selectedPackage = e.row.data;

                                            if ($scope.inverseSelectionList.some(m => m.SerialNo == $scope.selectedPackage.SerialNo)) {
                                                toastr.warning('Bu kolinin barkodu zaten okutulmuş.');
                                                return;
                                            }
                                            else {
                                                $scope.$emit('packageSelected', e.row.data);
                                                $scope.inverseSelectionList.push(e.row.data);
                                                var dataGrid = $("#packList").dxDataGrid("instance");
                                                dataGrid.refresh();
                                            }
                                        }
                                    }
                                ]
                            }
                        ],
                    });
                }
            }).catch(function (err) { console.log(err); });
    }

    $scope.loadSelectables = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Common/GetWarehouseList', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.warehouseList = resp.data;

                        resolve(resp.data);
                    }
                }).catch(function (err) { });
        });

        return prms;
    }

    // ON LOAD EVENTS
    $scope.loadSelectables().then(function () {
        $scope.bindList();
    });

    $scope.$on('loadProductList', function (e, d) {
        $scope.productSelection = d.productSelection;
        $scope.inverseSelectionList = d.list;

        $scope.setPage(0);
    });
});