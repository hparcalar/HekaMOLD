app.controller('loadFinanceCtrl', function ($scope, $http) {
    $scope.modelObject = {Id: 0,Finaces: []};
    $scope.forexList = [];
    $scope.firmList = [];
    $scope.selectedInvoiceType = {};
    $scope.saveStatus = 0;

    $scope.loadSelectables = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Load/GetInvoiceSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.firmList = resp.data.Firms;
                        $scope.forexList = resp.data.Forexes;
                        $scope.serviceItemList = resp.data.ServiceItems;
                        resolve();
                    }
                }).catch(function (err) { });
        });

        return prms;
    }
    $scope.saveModel = function () {
        $scope.saveStatus = 1;

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
            dropDownOptions: { width: 500 },
            dataSource: $scope.firmList,
            value: cellInfo.value,
            valueExpr: "Id",
            displayExpr: "FirmName",
            contentTemplate: function (e) {
                return $("<div>").dxDataGrid({
                    dataSource: $scope.firmList,
                    remoteOperations: true,
                    columns: [
                        { dataField: 'FirmCode', caption: 'Firma Kodu' },
                        { dataField: 'FirmName', caption: 'Firma Adı' },
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
                    refreshArray($scope.firmList);
                    $scope.bindInvoices();
                }
            }).catch(function (err) { });
    }

    $scope.calculateRow = function (row) {
        if (typeof row != 'undefined' && row != null) {
            try {
                row.TaxAmount = row.SubTotal * (row.TaxRate / 100);
                row.OverallTotal = row.TaxAmount + row.SubTotal;
            } catch (e) {

            }
        }
    }

    $scope.bindInvoices = function () {
        $('#dataList').dxDataGrid({
            dataSource: {
                load: function () {
                    $scope.modelObject.Invoices.forEach(d => {
                        if (d.TaxIncluded == true)
                            d.TaxIncluded = 1;
                        else if (d.TaxIncluded == false)
                            d.TaxIncluded = 0;
                    });

                    return $scope.modelObject.Invoices;
                },
                update: function (key, values) {
                    var obj = $scope.modelObject.Invoices.find(d => d.Id == key);
                    if (obj != null) {
                        let calculateRowAgain = false;
                        if (typeof values.FirmId != 'undefined') {
                            var firmObj = $scope.firmList.find(d => d.Id == values.FirmId);
                            obj.FirmId = firmObj.Id;
                            obj.FirmName = firmObj.FirmName;
                        }
                        if (typeof values.InvoiceType != 'undefined') {
                            var firmObj = $scope.firmList.find(d => d.Id == values.FirmId);
                            obj.FirmId = firmObj.Id;
                            obj.FirmName = firmObj.FirmName;
                        }
                        if (typeof values.InvoiceDateStr != 'undefined') { obj.InvoiceDateStr = values.InvoiceDateStr; obj.InvoiceDate = values.InvoiceDateStr; }
                        if (typeof values.Explanation != 'undefined') { obj.Explanation = values.Explanation; }
                        //if (typeof values.Quantity != 'undefined') { obj.Quantity = values.Quantity; calculateRowAgain = true; }
                        if (typeof values.TaxRate != 'undefined') { obj.TaxRate = values.TaxRate; calculateRowAgain = true; }
                        if (typeof values.SubTotal != 'undefined') { obj.SubTotal = values.SubTotal; calculateRowAgain = true; }
                        //if (typeof values.TaxIncluded != 'undefined') { obj.TaxIncluded = values.TaxIncluded; calculateRowAgain = true; }
                        //if (typeof values.UnitPrice != 'undefined') { obj.UnitPrice = values.UnitPrice; calculateRowAgain = true; }
                        if (typeof values.ForexRate != 'undefined') { obj.ForexRate = values.ForexRate; calculateRowAgain = true; }
                        if (typeof values.DocumentNo != 'undefined') { obj.DocumentNo = values.DocumentNo; }
                        if (typeof values.Integration != 'undefined') { obj.Integration = values.Integration; }
                        if (typeof values.ForexUnitPrice != 'undefined') {
                            obj.ForexUnitPrice = values.ForexUnitPrice;
                            if (typeof obj.ForexId != 'undefined' && obj.ForexId != null) {
                                obj.UnitPrice = obj.ForexUnitPrice * obj.ForexRate;
                                calculateRowAgain = true;
                            }
                        }
                        if (typeof values.ServiceItemId != 'undefined') {
                            var serviceItemObj = $scope.serviceItemList.find(d => d.Id == values.ServiceItemId);
                            obj.ServiceItemId = serviceItemObj.Id;
                            obj.ServiceItemName = serviceItemObj.ServiceItemName;
                        }
                        if (typeof values.ForexId != 'undefined') {
                            obj.ForexId = values.ForexId;
                            var forexObj = $scope.forexList.find(d => d.Id == obj.ForexId);
                            $http.get(HOST_URL + 'Common/GetForexRate?forexCode=' + forexObj.ForexTypeCode
                                + '&forexDate=' + obj.InvoiceDateStr, {}, 'json')
                                .then(function (resp) {
                                    if (typeof resp.data != 'undefined' && resp.data != null) {
                                        if (typeof resp.data.SalesForexRate != 'undefined') {
                                            obj.ForexRate = resp.data.SalesForexRate;
                                            $scope.calculateRow(obj);
                                        }
                                    }
                                }).catch(function (err) { });
                        }

                        if (calculateRowAgain)
                            $scope.calculateRow(obj);
                    }
                },
                remove: function (key) {
                    var obj = $scope.modelObject.Invoices.find(d => d.Id == key);
                    if (obj != null) {
                        $scope.modelObject.Invoices.splice($scope.modelObject.Invoices.indexOf(obj), 1);
                    }
                },
                insert: function (values) {
                    var newId = 1;
                    if ($scope.modelObject.Invoices.length > 0) {
                        newId = $scope.modelObject.Invoices.map(d => d.Id).reduce((max, n) => n > max ? n : max)
                        newId++;
                    }

                    var firmObj = $scope.firmList.find(d => d.Id == values.FirmId);

                    var newObj = {
                        Id: newId,
                        InvoiceType: values.InvoiceType,
                        FirmId: typeof firmObj != 'undefined' && firmObj != null ? firmObj.Id : null,
                        FirmName: typeof firmObj != 'undefined' && firmObj != null ? firmObj.FirmName  : null,
                        //Quantity: values.Quantity,
                        Integration: values.Integration,
                        DocumentNo: values.DocumentNo,
                        SubTotal: values.SubTotal,
                        ServiceItemId: values.ServiceItemId,
                        InvoiceDate: values.InvoiceDateStr,
                        InvoiceDateStr: values.InvoiceDateStr,
                        TaxRate: values.TaxRate,
                        TaxIncluded: values.TaxIncluded,
                        UnitPrice: values.UnitPrice,
                        ForexRate: values.ForexRate,
                        ForexUnitPrice: values.ForexUnitPrice,
                        ForexId: values.ForexId,
                        ItemOrderDetailId: null,
                        Explanation: values.Explanation,
                        NewDetail: true
                    };

                    $scope.modelObject.Invoices.push(newObj);
                    $scope.calculateRow(newObj);
                },
                key: 'Id'
            },
            showColumnLines: true,
            showRowLines: true,
            //rowAlternationEnabled: true,
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
            scrolling: {
                mode: "virtual"
            },
            height: 400,
            editing: {
                allowUpdating: true,
                allowDeleting: true,
                allowAdding: true,
                mode: 'cell'
            },
            columns: [

                //{ dataField: 'FirmName', caption: 'Firma Adı' },
                {
                    dataField: 'InvoiceType', caption: 'Fatura Türü',
                    allowSorting: false,
                    lookup: {
                        dataSource: [{ Id: 1, Text: 'Alış' }, { Id: 2, Text: 'Satış' }],
                        valueExpr: "Id",
                        displayExpr: "Text"
                    },
                    validationRules: [{ type: "required" }]
                },
                {
                    dataField: 'FirmId', caption: 'Firma',
                    lookup: {
                        dataSource: $scope.itemList,
                        valueExpr: "Id",
                        displayExpr: "FirmName"
                    },
                    allowSorting: false,
                    validationRules: [{ type: "required" }],
                    editCellTemplate: $scope.dropDownBoxEditorTemplate,
                    cellTemplate: function (container, options) {
                        if (typeof options.row.data.FirmName != 'undefined'
                            && options.row.data.FirmName != null && options.row.data.FirmName.length > 0)
                            container.text(options.row.data.FirmName);
                        else
                            container.text(options.displayValue);
                    }
                },
                {
                    dataField: 'ServiceItemId', caption: 'Hizmet Kalem',
                    allowSorting: false,
                    lookup: {
                        dataSource: $scope.serviceItemList,
                        valueExpr: "Id",
                        displayExpr: "ServiceItemName"
                    }
                },
                { dataField: 'InvoiceDateStr', caption: 'Fatura Tarihi', dataType: 'date', format: 'dd.MM.yyyy', validationRules: [{ type: "required" }] },
                { dataField: 'SubTotal', caption: 'Ara Tutar', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, validationRules: [{ type: "required" }]},
                //{ dataField: 'Quantity', caption: 'Miktar', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, validationRules: [{ type: "required" }]  },
                { dataField: 'TaxRate', caption: 'Kdv %', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, validationRules: [{ type: "required" }] },
                //{
                //    dataField: 'TaxIncluded', caption: 'Kdv D/H',
                //    allowSorting: false,
                //    lookup: {
                //        dataSource: [{ Id: 1, Text: 'Dahil' }, { Id: 0, Text: 'Hariç' }],
                //        valueExpr: "Id",
                //        displayExpr: "Text"
                //    },
                //    validationRules: [{ type: "required" }]
                //},
                //{ dataField: 'UnitPrice', caption: 'Birim Fiyat', dataType: 'number', format: { type: "fixedPoint", precision: 2 }, /*validationRules: [{ type: "required" }]*/ },
                {
                    dataField: 'ForexId', caption: 'Döviz Cinsi', validationRules: [{ type: "required" }],
                    allowSorting: false,
                    lookup: {
                        dataSource: $scope.forexList,
                        valueExpr: "Id",
                        displayExpr: "ForexTypeCode"
                    }
                },
                { dataField: 'ForexRate', caption: 'Döviz Kuru', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'ForexUnitPrice', caption: 'Döviz Fiyatı', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'TaxAmount', allowEditing: false, caption: 'Kdv Tutarı', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'OverallTotal', allowEditing: false, caption: 'Fatura Tutarı', dataType: 'number', format: { type: "fixedPoint", precision: 2 } },
                { dataField: 'DocumentNo', allowEditing: true, caption: 'Belge No', dataType: 'text' },
                { dataField: 'Integration', allowEditing: false, caption: 'Entegrasyon', dataType: 'boolean' },
                { dataField: 'Explanation', caption: 'Açıklama' }
            ],
            summary: {
                totalItems: [{
                    column: 'OverallTotal',
                    summaryType: 'sum',
                },
                {
                    column: 'SubTotal',
                    summaryType: 'sum',
                    format: { type: "fixedPoint", precision: 2 }
                },
                {
                    column: 'TaxAmount',
                    summaryType: 'sum',
                    format: { type: "fixedPoint", precision: 2 }
                }]
            },

        });
    }


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