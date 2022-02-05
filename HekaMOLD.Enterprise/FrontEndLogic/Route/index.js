app.controller('routeCtrl', function ($scope, $http) {
    $scope.modelObject = { Id:0, RouteItems:[] };
    $scope.forexList = [];
    $scope.selectedForex = { Id: 0 };
    $scope.processList = [];

    $scope.saveStatus = 0;

    // GET SELECTABLE DATA
    $scope.loadSelectables = function () {
        var prmReq = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Route/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.forexList = resp.data.Forexes;
                        $scope.forexList.splice(0, 0, { Id: null, ForexTypeCode: '' });

                        $scope.processList = resp.data.Processes;

                        resolve(resp.data);
                    }
                }).catch(function (err) { });
        });

        return prmReq;
    }

    $scope.generateRouteCode = function () {
        if ($scope.modelObject.RouteItems == null
            || $scope.modelObject.RouteItems.length == 0) {
            toastr.error('Rota kodu üretmek için önce prosesleri seçmelisiniz.');
            return;
        }

        // INCLUDE SELECTED PROCESS CODES
        let generatedCode = '';
        for (var i = 0; i < $scope.modelObject.RouteItems.length; i++) {
            var routeItem = $scope.modelObject.RouteItems[i];
            generatedCode += routeItem.ProcessCode + '-';
        }

        // TRIM RIGHT
        if (generatedCode.length > 0)
            generatedCode = generatedCode.substr(0, generatedCode.length - 1);

        // ASSIGN TO MODEL OBJECT
        $scope.modelObject.RouteCode = generatedCode;
        toastr.success('Rota kodu üretildi.');
    }

    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0, RouteItems: [] };
        $scope.bindDetails();
    }

    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu rota tanımını silmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'Route/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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

        if (typeof $scope.selectedForex != 'undefined' && $scope.selectedForex != null)
            $scope.modelObject.ForexId = $scope.selectedForex.Id;
        else
            $scope.modelObject.ForexId = null;

        $http.post(HOST_URL + 'Route/SaveModel', $scope.modelObject, 'json')
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
            dataSource: $scope.processList,
            value: cellInfo.value,
            valueExpr: "Id",
            displayExpr: "ProcessCode",
            contentTemplate: function (e) {
                return $("<div>").dxDataGrid({
                    dataSource: $scope.processList,
                    remoteOperations: true,
                    columns: [
                        { dataField: 'ProcessCode', caption: 'Proses Kodu' },
                        { dataField: 'ProcessName', caption: 'Proses Adı' },
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
        $http.get(HOST_URL + 'Route/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;

                    // BIND EXTERNAL TYPES
                    if ($scope.modelObject.ForexId > 0)
                        $scope.selectedForex = $scope.forexList.find(d => d.Id == $scope.modelObject.ForexId);
                    else
                        $scope.selectedForex = {};

                    $scope.bindDetails();
                }
            }).catch(function (err) { });
    }

    $scope.bindDetails = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    if ($scope.modelObject.RouteItems == null)
                        $scope.modelObject.RouteItems = [];

                    return $scope.modelObject.RouteItems;
                },
                update: function (key, values) {
                    var obj = $scope.modelObject.RouteItems.find(d => d.Id == key);
                    if (obj != null) {
                        if (typeof values.ProcessId != 'undefined') {
                            var processObj = $scope.processList.find(d => d.Id == values.ProcessId);
                            obj.ProcessId = processObj.Id;
                            obj.ProcessCode = processObj.ProcessCode;
                            obj.ProcessName = processObj.ProcessName;
                            obj.ProcessUnitPrice = processObj.UnitPrice;
                            obj.ProcessForexType = processObj.ForexTypeCode;
                        }

                        if (typeof values.Explanation != 'undefined') { obj.Explanation = values.Explanation; }
                        if (typeof values.LineNumber != 'undefined') { obj.LineNumber = values.LineNumber; }
                    }
                },
                remove: function (key) {
                    var obj = $scope.modelObject.RouteItems.find(d => d.Id == key);
                    if (obj != null) {
                        $scope.modelObject.RouteItems.splice($scope.modelObject.Details.indexOf(obj), 1);
                    }
                },
                insert: function (values) {
                    var newId = 1;
                    if ($scope.modelObject.RouteItems.length > 0) {
                        newId = $scope.modelObject.RouteItems.map(d => d.Id).reduce((max, n) => n > max ? n : max)
                        newId++;
                    }

                    var processObj = $scope.processList.find(d => d.Id == values.ProcessId);

                    var newObj = {
                        Id: newId,
                        ProcessId: processObj.Id,
                        ProcessCode: processObj.ProcessCode,
                        ProcessName: processObj.ProcessName,
                        LineNumber: values.LineNumber,
                        Explanation: values.Explanation,
                        ProcessUnitPrice: processObj.UnitPrice,
                        ProcessForexType: processObj.ForexTypeCode,
                        NewDetail: true
                    };

                    $scope.modelObject.RouteItems.push(newObj);
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
            columns: [
                {
                    dataField: 'LineNumber', caption: 'Sıra', dataType: 'number',
                    format: { type: "fixedPoint", precision: 0 }, validationRules: [{ type: "required" }]
                },
                {
                    dataField: 'ProcessId', caption: 'Proses Kodu',
                    lookup: {
                        dataSource: $scope.processList,
                        valueExpr: "Id",
                        displayExpr: "ProcessCode"
                    },
                    allowSorting: false,
                    validationRules: [{ type: "required" }],
                    editCellTemplate: $scope.dropDownBoxEditorTemplate,
                    cellTemplate: function (container, options) {
                        if (typeof options.row.data.ProcessCode != 'undefined'
                            && options.row.data.ProcessCode != null && options.row.data.ProcessCode.length > 0)
                            container.text(options.row.data.ProcessCode);
                        else
                            container.text(options.displayValue);
                    }
                },
                { dataField: 'ProcessName', caption: 'Proses Adı', allowEditing: false, cssClass: 'bg-secondary', },
                {
                    cssClass:'bg-secondary',
                    dataField: 'ProcessUnitPrice', caption: 'Fiyatı', allowEditing: false, dataType: 'number',
                    format: { type: "fixedPoint", precision: 2 },
                },
                { dataField: 'ProcessForexType', caption: 'Döviz Cinsi', allowEditing: false, cssClass: 'bg-secondary', },
                { dataField: 'Explanation', caption: 'Açıklama' },
                {
                    type: "buttons",
                    buttons: [
                        {
                            name: 'delete', cssClass: '', text: '', onClick: function (e) {
                                $('#dataList').dxDataGrid('instance').deleteRow(e.row.rowIndex);
                            }
                        },
                    ]
                }
            ]
        });
    }

    // ON LOAD EVENTS
    DevExpress.localization.locale('tr');
    $scope.loadSelectables().then(function (data) {
        if (PRM_ID > 0)
            $scope.bindModel(PRM_ID);
        else
            $scope.bindModel(0);
    });
});