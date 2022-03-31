app.controller('entryQualityPlanCtrl', function ($scope, $http) {
    $scope.modelObject = { Id:0, Details: [] };

    $scope.saveStatus = 0;

    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0, Details: [] };
        $scope.bindDetails();
    }

    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu giriş kalite planını silmek istediğinizden emin misiniz?",
            closeButton:false,
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
                    $http.post(HOST_URL + 'EntryQuality/DeletePlanModel', { rid: $scope.modelObject.Id }, 'json')
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

        $http.post(HOST_URL + 'EntryQuality/SavePlanModel', $scope.modelObject, 'json')
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
        $http.get(HOST_URL + 'EntryQuality/BindPlanModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;

                    $scope.bindDetails();
                }
            }).catch(function (err) { });
    }

    $scope.bindDetails = function () {
        $('#detailList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $scope.modelObject.Details;
                },
                update: function (key, values) {
                    var obj = $scope.modelObject.Details.find(d => d.Id == key);
                    if (obj != null) {
                        if (typeof values.CheckProperty != 'undefined') { obj.CheckProperty = values.CheckProperty; }
                        if (typeof values.OrderNo != 'undefined') { obj.OrderNo = values.OrderNo; }
                        if (typeof values.PeriodType != 'undefined') { obj.PeriodType = values.PeriodType; }
                        if (typeof values.AcceptanceCriteria != 'undefined') { obj.AcceptanceCriteria = values.AcceptanceCriteria; }
                        if (typeof values.ControlDevice != 'undefined') { obj.ControlDevice = values.ControlDevice; }
                        if (typeof values.Method != 'undefined') { obj.Method = values.Method; }
                        if (typeof values.Responsible != 'undefined') { obj.Responsible = values.Responsible; }
                        if (typeof values.CheckType != 'undefined') { obj.CheckType = values.CheckType; }
                        if (typeof values.ToleranceMin != 'undefined') { obj.ToleranceMin = values.ToleranceMin; }
                        if (typeof values.ToleranceMax != 'undefined') { obj.ToleranceMax = values.ToleranceMax; }
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

                    var newObj = {
                        Id: newId,
                        CheckProperty: values.CheckProperty,
                        OrderNo: values.OrderNo,
                        PeriodType: values.PeriodType,
                        AcceptanceCriteria: values.AcceptanceCriteria,
                        ControlDevice: values.ControlDevice,
                        Method: values.Method,
                        Responsible: values.Responsible,
                        CheckType: values.CheckType,
                        ToleranceMin: values.ToleranceMin,
                        ToleranceMax: values.ToleranceMax,
                        IsRequired: false,
                        NewDetail: true
                    };

                    $scope.modelObject.Details.push(newObj);
                },
                key: 'Id'
            },
            showColumnLines: true,
            showRowLines: true,
            rowAlternationEnabled: true,
            focusedRowEnabled: false,
            showBorders: true,
            wordWrapEnabled: true,
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
            height: 300,
            editing: {
                allowUpdating: true,
                allowDeleting: true,
                allowAdding: true,
                mode: 'cell'
            },
            columns: [
                { dataField: 'OrderNo', caption: 'Sıra', width:75, dataType: 'number', format: { type: "fixedPoint", precision: 0 } },
                { dataField: 'CheckProperty', caption: 'Özellik' },
                { dataField: 'PeriodType', caption: 'Periyot' },
                { dataField: 'AcceptanceCriteria', caption: 'Kabul / Red Kriteri' },
                { dataField: 'ControlDevice', caption: 'Cihaz' },
                { dataField: 'Method', caption: 'Yöntem' },
                { dataField: 'Responsible', caption: 'Sorumlu' },
                {
                    dataField: 'CheckType', caption: 'Seçim Türü',
                    allowSorting: false,
                    lookup: {
                        dataSource: [{ Id: 1, Text: 'Check' }, { Id: 2, Text: 'Sayısal' }, { Id:3, Text:'Metin' }],
                        valueExpr: "Id",
                        displayExpr: "Text"
                    }
                },
                { dataField: 'ToleranceMin', caption: 'Tölerans Min', width: 75, dataType: 'number', format: { type: "fixedPoint", precision: 0 } },
                { dataField: 'ToleranceMax', caption: 'Tölerans Max', width: 75, dataType: 'number', format: { type: "fixedPoint", precision: 0 } },
            ]
        });
    }

    // ON LOAD EVENTS
    DevExpress.localization.locale('tr');
    if (PRM_ID > 0)
        $scope.bindModel(PRM_ID);
});