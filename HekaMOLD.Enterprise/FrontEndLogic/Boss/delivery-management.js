app.controller('deliveryManagementCtrl', function ($scope, $http, $timeout) {
    DevExpress.localization.locale('tr');
    $scope.dataList = [];
    $scope.tabMode = 1; // 1: waiting for redirection, 2: ready to sync, 3: off the record

    $scope.setTabMode = function (mode) {
        $timeout(function () {
            $scope.tabMode = mode;

            $scope.loadReport();
        });

        try {
            $scope.$applyAsync();
        } catch (e) {

        }
    }

    // LIST FUNCTIONS
    $scope.loadReport = function () {
        let targetUri = 'ItemReceipt/GetOTRList?listType=1';
        if ($scope.tabMode == 2)
            targetUri = 'ItemReceipt/GetOTRList?listType=2';
        else if ($scope.tabMode == 3)
            targetUri = 'ItemReceipt/GetOTRList?listType=3';

        $http.get(HOST_URL + targetUri, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.dataList = resp.data;

                    $('#dataList').dxDataGrid({
                        dataSource: {
                            load: function () {
                                return $scope.dataList;
                            },
                            key: ['Id'],
                        },
                        showColumnLines: false,
                        showRowLines: true,
                        rowAlternationEnabled: true,
                        allowColumnResizing: true,
                        wordWrapEnabled: true,
                        focusedRowEnabled: true,
                        showBorders: true,
                        filterRow: {
                            visible: true,
                        },
                        headerFilter: {
                            visible: true,
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
                        height: parseInt($('body').height() * 0.75),
                        groupPanel: {
                            visible: false,
                        },
                        editing: {
                            allowUpdating: false,
                            allowDeleting: false
                        },
                        columns: [
                            /*{ dataField: 'ItemNo', caption: 'Ürün Kodu' },*/
                            { dataField: 'ReceiptDateStr', caption: 'Tarih' },
                            { dataField: 'ReceiptNo', caption: 'Fiş No' },
                            { dataField: 'ItemName', caption: 'Ürün' },
                            { dataField: 'FirmName', caption: 'Firma' },
                            {
                                dataField: 'SyncStatus', caption: 'Durum',
                                cssClass:'bg-light-danger text-dark',
                                visible: $scope.tabMode == 2 || $scope.tabMode == 3
                            },
                            { dataField: 'Quantity', caption: 'Miktar' },
                            {
                                visible: $scope.tabMode == 1,
                                type: "buttons",
                                buttons: [
                                    {
                                        name: 'preview', cssClass: 'btn btn-sm btn-light-primary', text: 'ONAYLA', onClick: function (e) {
                                            $scope.approvalDialog(e.row.data);
                                        }
                                    }
                                ]
                            },
                            {
                                visible: $scope.tabMode == 2 || $scope.tabMode == 3,
                                type: "buttons",
                                buttons: [
                                    {
                                        name: 'preview', cssClass: 'btn btn-sm btn-warning text-dark', text: 'GERİ AL', onClick: function (e) {
                                            if (e.row.data.SyncStatus == 1) {
                                                toastr.warning('Bu sevkiyatın transferi gerçekleştiği için geri alınamaz.', 'Uyarı');
                                                return;
                                            }

                                            bootbox.confirm({
                                                title: "Onay",
                                                message: "Bu onay durumunu geri almak istediğinizden emin misiniz?",
                                                size: "large",
                                                buttons: {
                                                    cancel: {
                                                        label: 'Hayır',
                                                        className: 'btn-danger'
                                                    },
                                                    confirm: {
                                                        label: 'Evet',
                                                        className: 'btn-success'
                                                    }
                                                },
                                                callback: function (result) {
                                                    if (result) {
                                                        $scope.changeStatus(e.row.data, 0);
                                                    }
                                                }
                                            });
                                        }
                                    }
                                ]
                            }
                        ],
                        summary: {
                            totalItems: [{
                                column: "Quantity",
                                summaryType: "sum",
                            }
                            ]
                        }
                    });
                }
            }).catch(function (err) { });
    }

    $scope.approvalDialog = function (rowData) {
        bootbox.dialog({
            title: 'Sevkiyat Onay Türünü Seçiniz',
            message: " ",
            size: 'large',
            buttons: {
                cancel: {
                    label: "ONAYLA",
                    className: 'btn-success',
                    callback: function () {
                        $scope.changeStatus(rowData, 2);
                    }
                },
                noclose: {
                    label: "OTR",
                    className: 'btn-primary',
                    callback: function () {
                        $scope.changeStatus(rowData, 3);
                    }
                },
                ok: {
                    label: "VAZGEÇ",
                    className: 'btn-danger',
                    callback: function () {
                        return true;
                    }
                }
            }
        });
    }

    $scope.changeStatus = function (rowData, status) {
        try {
            let receiptStatusType = 0;
            if (status == 1)
                receiptStatusType = 0;
            else if (status == 2)
                receiptStatusType = 7;
            else if (status == 3)
                receiptStatusType = 6;

            $http.post(HOST_URL + 'Boss/ChangeReceiptStatus', { itemReceiptDetailId: rowData.Id, otrStatus: receiptStatusType }, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.saveStatus = 0;

                        if (resp.data.Status == 1) {
                            toastr.success('İşlem başarılı.', 'Bilgilendirme');
                            $scope.loadReport();
                        }
                        else
                            toastr.error(resp.data.ErrorMessage, 'Hata');
                    }
                }).catch(function (err) { });
        } catch (e) {
            toastr.error(e.message, 'Hata');
        }
    }

    // ON LOAD EVENTS
    angular.element(document).ready(function () {
        $scope.loadReport();

    });
});
