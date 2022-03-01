app.controller('editVoyageCtrl', function ($scope, $http) {
    DevExpress.localization.locale('tr');
    $scope.waitingLoadList = [];

    $scope.modelObject = { Id: 0};

    $scope.save = function () {
        if ($scope.modelObject.Quantity <= 0) {
            toastr.error('Devam edebilmek için pozitif bir hedef miktar girmelisiniz.', 'Uyarı');
            return;
        }

        try {
            $http.post(HOST_URL + 'Planning/EditPlan', $scope.modelObject, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        if (resp.data.Result == true)
                            toastr.success('İşlem başarılı.', 'Bilgilendirme');
                        else
                            toastr.error('Hata: ' + resp.data.ErrorMessage, 'Uyarı');
                    }

                    $scope.$emit('editPlanEnd', $scope.modelObject);
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.completePlan = function () {
        bootbox.confirm({
            message: "Bu üretim planını kapatmak istediğinizden emin misiniz?",
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
                    try {
                        $http.post(HOST_URL + 'Planning/CompletePlan', { id: $scope.modelObject.Id }, 'json')
                            .then(function (resp) {
                                if (typeof resp.data != 'undefined' && resp.data != null) {
                                    if (resp.data.Result == true)
                                        toastr.success('İşlem başarılı.', 'Bilgilendirme');
                                    else
                                        toastr.error('Hata: ' + resp.data.ErrorMessage, 'Uyarı');
                                }

                                $scope.$emit('editPlanEnd', $scope.modelObject);
                            }).catch(function (err) { });
                    } catch (e) {

                    }
                }
            }
        });
    }

    $scope.bindWaitingLoadList = function () {
        $('#WaitingLoadList').dxDataGrid({
            dataSource: {
                load: function () {
                    if ($scope.waitingLoadList.length == 0)
                        $scope.waitingLoadList = $.getJSON(HOST_URL + 'LPlanning/GetWaitingLoads', function (data) {
                            data.forEach(d => {
                                d.IsChecked = false;
                            }
                            );
                        });

                    return $scope.waitingLoadList;
                },
                update: function (key, values) {
                    var obj = $scope.waitingLoadList.responseJSON.find(d => d.Id == key);
                    if (obj != null) {
                        obj.IsChecked = values.IsChecked;
                    }
                },
                key: 'Id'
            },
            showColumnLines: false,
            showRowLines: true,
            rowAlternationEnabled: true,
            focusedRowEnabled: true,
            showBorders: true,
            filterRow: {
                visible: true
            },
            headerFilter: {
                visible: true
            },
            paging: {
                enabled: true,
                pageSize: 13,
                pageIndex: 0
            },
            groupPanel: {
                visible: false
            },
            editing: {
                allowUpdating: true,
                allowDeleting: false,
                mode: 'cell'
            },
            columns: [
                { dataField: 'LoadCode', caption: 'Yük Kodu', allowEditing: false },
                { dataField: 'LoadingDateStr', caption: 'Yükleme Tarih', dataType: 'date', format: 'dd.MM.yyyy', allowEditing: false },
                { dataField: 'CustomerFirmName', caption: 'Firma', allowEditing: false },
                { dataField: 'OrderTransactionDirectionTypeStr', caption: 'İşlem Yönü', allowEditing: false },
                { dataField: 'OveralQuantity', caption: 'Miktar', allowEditing: false, dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'OveralWeight', caption: 'Toplam Ağırlık', allowEditing: false },
                { dataField: 'OveralLadametre', caption: 'Toplam Ladametre', allowEditing: false, dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'OveralVolume', caption: 'Toplam Hacim', allowEditing: false, dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'IsChecked', caption: 'Seç' }

           ]
        });
    }
    $scope.transferSelections = function () {
        if ($scope.waitingLoadList.responseJSON.filter(d => d.IsChecked == true).length == 0) {
            toastr.warning('Aktarmak için bir veya daha fazla yük seçmelisiniz.', 'Uyarı');
            return;
        }

        var selectedDetails = $scope.waitingLoadList.responseJSON.filter(d => d.IsChecked == true);
        $scope.$emit('editVoyageEnd', selectedDetails);
    }

    $scope.bindWaitingLoadList();
    // ON LOAD EVENTS
    $scope.$on('loadEditPlan', function (e, d) {
        $scope.modelObject.Id = d.id;
    });
    $scope.bindWaitingLoadList();
});