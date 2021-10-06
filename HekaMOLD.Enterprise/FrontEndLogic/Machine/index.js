app.controller('machineCtrl', function ($scope, $http) {
    $scope.modelObject = {};

    $scope.saveStatus = 0;

    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0 };
    }

    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu makineyi silmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'Machine/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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

        $scope.modelObject.BackColor = $("#back-color").spectrum("get") != null ?
            $("#back-color").spectrum("get").toHexString() : "";
        $scope.modelObject.ForeColor = $("#fore-color").spectrum("get") != null ?
            $("#fore-color").spectrum("get").toHexString() : "";

        $http.post(HOST_URL + 'Machine/SaveModel', $scope.modelObject, 'json')
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
        $http.get(HOST_URL + 'Machine/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;

                    $("#back-color").spectrum("set", $scope.modelObject.BackColor);
                    $("#fore-color").spectrum("set", $scope.modelObject.ForeColor);

                    $scope.bindInstructionList();
                }
            }).catch(function (err) { });
    }

    $scope.bindInstructionList = function () {
        $('#instructionList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $scope.modelObject.Instructions;
                },
                update: function (key, values) {
                    var obj = $scope.modelObject.Instructions.find(d => d.Id == key);
                    if (obj != null) {
                        if (typeof values.UnitName != 'undefined') { obj.UnitName = values.UnitName; }
                        if (typeof values.PeriodType != 'undefined') { obj.PeriodType = values.PeriodType; }
                        if (typeof values.ToDoList != 'undefined') { obj.ToDoList = values.ToDoList; }
                        if (typeof values.Responsible != 'undefined') { obj.Responsible = values.Responsible; }
                    }
                },
                remove: function (key) {
                    var obj = $scope.modelObject.Instructions.find(d => d.Id == key);
                    if (obj != null) {
                        $scope.modelObject.Instructions.splice($scope.modelObject.Instructions.indexOf(obj), 1);
                    }
                },
                insert: function (values) {
                    var newId = 1;
                    if ($scope.modelObject.Instructions.length > 0) {
                        newId = $scope.modelObject.Instructions.map(d => d.Id).reduce((max, n) => n > max ? n : max)
                        newId++;
                    }

                    var newObj = {
                        Id: newId,
                        UnitName: values.UnitName,
                        PeriodType: values.PeriodType,
                        ToDoList: values.ToDoList,
                        Responsible: values.Responsible,
                        NewDetail: true
                    };

                    $scope.modelObject.Instructions.push(newObj);
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
                allowUpdating: true,
                allowDeleting: true,
                allowAdding: true,
                mode: 'cell'
            },
            columns: [
                { dataField: 'UnitName', caption: 'Ünite İsmi', width:100, validationRules: [{ type: "required" }] },
                {
                    width: 100,
                    dataField: 'PeriodType', caption: 'Bakım Periyodu',
                    allowSorting: false,
                    lookup: {
                        dataSource: [{ Text: 'G' }, { Text: 'H' }, { Text: 'A' }, {Text: 'AA'}],
                        valueExpr: "Text",
                        displayExpr: "Text"
                    },
                },
                { dataField: 'ToDoList', caption: 'Yapılacak İşlemler', editorType: 'dxTextArea' },
                { dataField: 'Responsible', width: 150, caption: 'Sorumlusu' },
            ]
        });
    }

    // ON LOAD EVENTS
    DevExpress.localization.locale('tr');
    if (PRM_ID > 0)
        $scope.bindModel(PRM_ID);
    else
        $scope.bindModel(0);

    $('#back-color').spectrum({
        type: 'component',
        cancelText: 'Vazgeç',
        chooseText: 'Seç',
    });

    $('#fore-color').spectrum({
        type: 'component',
        cancelText: 'Vazgeç',
        chooseText: 'Seç',
    });
});