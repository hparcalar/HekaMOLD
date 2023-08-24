app.controller('preventiveActionCtrl', function ($scope, $http, $timeout) {
    $scope.modelObject = { Id: 0, FormDateText: moment().format('DD.MM.YYYY'), Details: [], FormResult: 0 };

    $scope.actionTypeList = [
        { Id: 1, Code: 'Düzeltici Faaliyet' },
        { Id: 2, Code: 'Önleyici Faaliyet' },
    ];
    $scope.selectedActionType = $scope.actionTypeList[0];

    $scope.resultTypeList = [
        { Id: 0, Code: 'Bekliyor' },
        { Id: 1, Code: 'Risk Ortadan Kaldırıldı' },
        { Id: 2, Code: 'Süreç Başarısız' },
    ];
    $scope.selectedResultType = $scope.resultTypeList[0];

    $scope.userList = [];

    $scope.selectedRow = { Id: 0 };

    $scope.saveStatus = 0;

    // RECEIPT FUNCTIONS
    $scope.getNextFormNo = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'PreventiveAction/GetNextFormNo', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        if (resp.data.Result) {
                            resolve(resp.data.ReceiptNo);
                        }
                        else {
                            toastr.error('Sıradaki form numarası üretilemedi. Lütfen ekranı yenileyip tekrar deneyiniz.', 'Uyarı');
                            resolve('');
                        }
                    }
                }).catch(function (err) { });
        });

        return prms;
    }
    $scope.getToFixed = function (data, points) {
        try {
            if (typeof data != 'undefined')
                return data.toFixed(points);
        } catch (e) {

        }

        return '';
    }

    // CRUD
    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0, FormDateText: moment().format('DD.MM.YYYY'), Details: [], FormResult: 0 };

        $scope.getNextFormNo().then(function (rNo) {
            $scope.modelObject.FormNo = rNo;
            $scope.$apply();
        });
        $scope.bindDetails();
    }

    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu formu silmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'PreventiveAction/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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

        if (typeof $scope.selectedActionType != 'undefined' && $scope.selectedActionType != null)
            $scope.modelObject.ActionType = $scope.selectedActionType.Id;
        else
            $scope.modelObject.ActionType = null;

        if (typeof $scope.selectedResultType != 'undefined' && $scope.selectedResultType != null
            && $scope.modelObject.ApproverUserId > 0)
            $scope.modelObject.FormResult = $scope.selectedResultType.Id;
        else
            $scope.modelObject.FormResult = null;

        $scope.modelObject.Details.forEach(d => {
            d.DeadlineDate = d.DeadlineDateText;
        });

        $http.post(HOST_URL + 'PreventiveAction/SaveModel', $scope.modelObject, 'json')
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
            dataSource: $scope.userList,
            value: cellInfo.value,
            valueExpr: "Id",
            displayExpr: "UserName",
            contentTemplate: function (e) {
                return $("<div>").dxDataGrid({
                    dataSource: $scope.userList,
                    remoteOperations: true,
                    columns: [
                        { dataField: 'UserCode', caption: 'Kullanıcı Kodu' },
                        { dataField: 'UserName', caption: 'Kullanıcı Adı' },
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

    $scope.formatNumber = function (numberData) {
        try {
            var formatter = new Intl.NumberFormat('tr', {
                style: 'decimal',
                currency: 'TRY',
            });

            return formatter.format(numberData);
        } catch (e) {
            return 0;
        }
    }

    $scope.bindModel = function (id) {
        $http.get(HOST_URL + 'PreventiveAction/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;
                    //$scope.modelObject.DateOfNeed = $scope.modelObject.DateOfNeedStr;
                    //$scope.modelObject.OrderDate = $scope.modelObject.OrderDateStr;

                    if (typeof $scope.modelObject.ActionType != 'undefined' && $scope.modelObject.ActionType != null)
                        $scope.selectedActionType = $scope.actionTypeList.find(d => d.Id == $scope.modelObject.ActionType);
                    else
                        $scope.selectedActionType = $scope.actionTypeList[0];

                    if (typeof $scope.modelObject.FormResult != 'undefined' && $scope.modelObject.FormResult != null)
                        $scope.selectedResultType = $scope.resultTypeList.find(d => d.Id == $scope.modelObject.FormResult);
                    else
                        $scope.selectedResultType = $scope.actionTypeList[0];

                    $scope.bindDetails();
                }
            }).catch(function (err) { });
    }

    $scope.approveForm = function () {
        bootbox.confirm({
            message: "Bu formu " + ($scope.modelObject.ApproverUserId == null ? 'onaylayıp yürürlüğe girmesini' : 'onayını geri almak') + "istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'PreventiveAction/ApproveForm', { rid: $scope.modelObject.Id }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Status == 1) {
                                    toastr.success('Başarıyla onaylandı.', 'Bilgilendirme');

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

    $scope.closeForm = function () {
        bootbox.confirm({
            message: "Bu formu " + ($scope.modelObject.ClosingUserId == null ? 'sonlandırmak' : 'geri açmak') +" istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'PreventiveAction/CloseForm', { rid: $scope.modelObject.Id }, 'json')
                        .then(function (resp) {
                            if (typeof resp.data != 'undefined' && resp.data != null) {
                                $scope.saveStatus = 0;

                                if (resp.data.Status == 1) {
                                    toastr.success('Başarıyla kapatıldı.', 'Bilgilendirme');

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

    $scope.bindDetails = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    $scope.modelObject.Details.forEach(d => {
                        d.DeadlineDateText = moment(d.DeadlineDateText, 'DD.MM.YYYY').toDate();
                    });
                    return $scope.modelObject.Details;
                },
                update: function (key, values) {
                    var obj = $scope.modelObject.Details.find(d => d.Id == key);
                    if (obj != null) {
                        if (typeof values.ResponsibleUserId != 'undefined') {
                            var itemObj = $scope.userList.find(d => d.Id == values.ResponsibleUserId);
                            obj.ResponsibleUserId = itemObj.Id;
                            obj.ResponsibleUserName = itemObj.UserName;
                        }

                        if (typeof values.ActionText != 'undefined') { obj.ActionText = values.ActionText; }
                        if (typeof values.Notes != 'undefined') { obj.Notes = values.Notes; }
                        if (typeof values.DeadlineDateText != 'undefined') { obj.DeadlineDateText = values.DeadlineDateText; }
                        if (typeof values.ActionStatus != 'undefined') { obj.ActionStatus = values.ActionStatus; }
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
                    var lastRow = null;
                    if ($scope.modelObject.Details.length > 0) {
                        newId = $scope.modelObject.Details.map(d => d.Id).reduce((max, n) => n > max ? n : max)
                        newId++;
                        lastRow = $scope.modelObject.Details[$scope.modelObject.Details.length - 1];
                    }

                    var itemObj = $scope.userList.find(d => d.Id == values.ResponsibleUserId);

                    var newObj = {
                        Id: newId,
                        ResponsibleUserId: itemObj ? itemObj.Id : 0,
                        ResponsibleUserName: itemObj ? itemObj.UserName : '',
                        ActionText: values.ActionText,
                        Notes: values.Notes,
                        DeadlineDateText: values.DeadlineDateText,
                        ActionStatus: values.ActionStatus ?? 0,
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
            height: 350,
            editing: {
                allowUpdating: true,
                allowDeleting: true,
                allowAdding: true,
                mode: 'cell'
            },
            allowColumnResizing: true,
            wordWrapEnabled: true,
            onCellPrepared: function (e) {
                if (e.rowType === "data") {
                    if (e.data.ActionStatus == 2) {
                        e.cellElement.css("background-color", "#81d048");
                    }
                }
            },
            onInitNewRow: function (e) {
                e.data.UnitPrice = 0;
                e.data.TaxIncluded = 0;

                if ($scope.modelObject.Details.length > 0) {
                    var lastRow = $scope.modelObject.Details[$scope.modelObject.Details.length - 1];
                    e.data.ForexId = lastRow.ForexId;
                }
            },
            
            columns: [
                { dataField: 'ActionText', caption: 'Aksiyon', validationRules: [{ type: "required" }], },
                { dataField: 'Notes', caption: 'Notlar' },
                {
                    dataField: 'ResponsibleUserId', caption: 'Sorumlu Kişi',
                    lookup: {
                        dataSource: $scope.userList,
                        valueExpr: "Id",
                        displayExpr: "UserName"
                    },
                    allowSorting: false,
                    editCellTemplate: $scope.dropDownBoxEditorTemplate,
                    cellTemplate: function (container, options) {
                        if (typeof options.row.data.UserName != 'undefined'
                            && options.row.data.UserName != null && options.row.data.UserName.length > 0)
                            container.text(options.row.data.UserName);
                        else
                            container.text(options.displayValue);
                    }
                },
                {
                    dataField: 'DeadlineDateText', caption: 'Termin', dataType: 'date',
                    format: 'dd.MM.yyyy'   },
                {
                    dataField: 'ActionStatus', caption: 'Durum',
                    allowSorting: false,
                    lookup: {
                        dataSource: [
                            { Id: 0, Code: 'Bekliyor' },
                            { Id: 1, Code: 'Çalışılıyor' },
                            { Id: 2, Code: 'Tamamlandı' },
                            { Id: 3, Code: 'İptal Edildi' }
                        ],
                        valueExpr: "Id",
                        displayExpr: "Code"
                    }
                },
                {
                    type: "buttons",
                    buttons: [
                        {
                            name: 'delete', cssClass: '', text: '', onClick: function (e) {
                                $('#dataList').dxDataGrid('instance').deleteRow(e.row.rowIndex);
                            }
                        },
                        {
                            name: 'preview', cssClass: 'btn btn-sm btn-light-primary py-0 px-1', text: '...', onClick: function (e) {
                                var dataGrid = $("#dataList").dxDataGrid("instance");
                                $scope.selectedRow = e.row.data;
                                $scope.showRowMenu();
                            }
                        },
                    ]
                }
            ]
        });
    }

    $scope.loadSelectables = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'PreventiveAction/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.userList = resp.data.Users;

                        resolve();
                    }
                }).catch(function (err) { });
        });

        return prms;
    }

    // ON LOAD EVENTS
    DevExpress.localization.locale('tr');
    $scope.loadSelectables().then(function () {
        if (PRM_ID > 0)
            $scope.bindModel(PRM_ID);
        else {
            $scope.getNextFormNo().then(function (rNo) {
                $scope.modelObject.FormNo = rNo;

                $timeout(function () {
                    $scope.$applyAsync();
                });

                $scope.bindDetails();
            });
        }
    });
});