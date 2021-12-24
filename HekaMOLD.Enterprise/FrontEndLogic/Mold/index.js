app.controller('moldCtrl', function ($scope, $http) {
    $scope.modelObject = { Id:0, Products: [] };

    $scope.saveStatus = 0;

    $scope.itemList = [];
    $scope.firmList = [];
    $scope.warehouseList = [];
    $scope.movementList = [];
    $scope.selectedFirm = {};
    $scope.selectedWarehouse = {};

    // #region CRUD
    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0, Products: [] };
    }

    $scope.loadSelectables = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Mold/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.itemList = resp.data.Items;
                        $scope.firmList = resp.data.Firms;
                        $scope.warehouseList = resp.data.Warehouses;

                        var emptyFirmObj = { Id: 0, FirmCode: '-- Seçiniz --' };
                        $scope.firmList.splice(0, 0, emptyFirmObj);
                        $scope.selectedFirm = emptyFirmObj;

                        var emptyWrObj = { Id: 0, WarehouseName: '-- Seçiniz --' };
                        $scope.warehouseList.splice(0, 0, emptyWrObj);
                        $scope.selectedWarehouse = emptyWrObj;

                        resolve();
                    }
                }).catch(function (err) { });
        });

        return prms;
    }

    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu kalıp tanımını silmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'Mold/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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

        // ASSIGN SELECTABLES TO STORE
        if (typeof $scope.selectedFirm != 'undefined' && $scope.selectedFirm != null && $scope.selectedFirm.Id > 0)
            $scope.modelObject.FirmId = $scope.selectedFirm.Id;
        else
            $scope.modelObject.FirmId = null;

        if (typeof $scope.selectedWarehouse != 'undefined' && $scope.selectedWarehouse != null && $scope.selectedWarehouse.Id > 0)
            $scope.modelObject.InWarehouseId = $scope.selectedWarehouse.Id;
        else
            $scope.modelObject.InWarehouseId = null;

        $http.post(HOST_URL + 'Mold/SaveModel', $scope.modelObject, 'json')
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
    // #endregion

    // #region MOLD STATUS MANAGEMENT
    $scope.setMoldStatus = function (status) {
        if (status != $scope.modelObject.MoldStatus) {
            if (status == 2) { // IF NEW STATUS IS TO SEND FIRM FOR REVISIONS
                var inputOpts = [];
                for (var i = 0; i < $scope.firmList.length; i++) {
                    var firm = $scope.firmList[i];

                    if (typeof firm.FirmName != 'undefined') {
                        inputOpts.push({
                            text: firm.FirmName,
                            value: firm.Id,
                            group: '',
                        });
                    }
                }

                bootbox.prompt({
                    title: "Kalıbı revizyon için göndereceğiniz firmayı seçin: ",
                    inputType: 'select',
                    inputOptions: inputOpts,
                    closeButton: false,
                    buttons: {
                        confirm: {
                            label: 'Devam Et',
                            className: 'btn-primary'
                        },
                        cancel: {
                            label: 'Vazgeç',
                            className: 'btn-light'
                        }
                    },
                    callback: function (result) {
                        if (result > 0) {
                            bootbox.prompt({
                                title: "Çıkış tarihi: ",
                                inputType: 'date',
                                value: moment().format('YYYY-MM-DD'),
                                closeButton: false,
                                buttons: {
                                    confirm: {
                                        label: 'Çıkış Yap',
                                        className: 'btn-primary'
                                    },
                                    cancel: {
                                        label: 'Vazgeç',
                                        className: 'btn-light'
                                    }
                                },
                                callback: function (resultDate) {
                                    if (resultDate) {
                                        $http.post(HOST_URL + 'Mold/SendToRevision',
                                            { rid: $scope.modelObject.Id, firmId: result, moveDate: resultDate }, 'json')
                                            .then(function (resp) {
                                                if (typeof resp.data != 'undefined' && resp.data != null) {
                                                    $scope.saveStatus = 0;

                                                    if (resp.data.Status == 1) {
                                                        toastr.success('Kalıp revizyona gönderildi.', 'Bilgilendirme');
                                                        $scope.changeMoldStatus(status);
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
                });
            } else {
                if ($scope.modelObject.MoldStatus == 2) { // IF OLD STATUS IS WAITING AT FIRM FOR REVISIONS
                    bootbox.confirm({
                        message: "Bu kalıp revizyonda görünüyor. Durumunu değiştirerek firmadan teslim alındığını onaylıyor musunuz?",
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
                                bootbox.prompt({
                                    title: "Giriş tarihi: ",
                                    inputType: 'date',
                                    value: moment().format('YYYY-MM-DD'),
                                    closeButton: false,
                                    buttons: {
                                        confirm: {
                                            label: 'Giriş Yap',
                                            className: 'btn-primary'
                                        },
                                        cancel: {
                                            label: 'Vazgeç',
                                            className: 'btn-light'
                                        }
                                    },
                                    callback: function (resultDate) {
                                        if (resultDate) {
                                            $http.post(HOST_URL + 'Mold/BackFromRevision',
                                                { rid: $scope.modelObject.Id, moveDate: resultDate }, 'json')
                                                .then(function (resp) {
                                                    if (typeof resp.data != 'undefined' && resp.data != null) {
                                                        $scope.saveStatus = 0;

                                                        if (resp.data.Status == 1) {
                                                            toastr.success('Kalıp firmadan teslim alındı.', 'Bilgilendirme');
                                                            $scope.changeMoldStatus(status);
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
                    });
                }
                else {
                    bootbox.confirm({
                        message: "Bu kalıbın durumunu değiştirmek istediğinizden emin misiniz?",
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
                                $scope.changeMoldStatus(status);
                            }
                        }
                    });
                }
            }
        }
    }

    $scope.changeMoldStatus = function (status) {
        $http.post(HOST_URL + 'Mold/ChangeMoldStatus',
            { rid: $scope.modelObject.Id, status: status }, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.saveStatus = 0;

                    if (resp.data.Status == 1) {
                        toastr.success('Durum başarıyla değiştirildi.', 'Bilgilendirme');

                        $scope.bindModel($scope.modelObject.Id);
                    }
                    else
                        toastr.error(resp.data.ErrorMessage, 'Hata');
                }
            }).catch(function (err) { });
    }
    // #endregion

    // #region MOLD HISTORY
    $scope.showMoldHistory = function () {
        try {
            $http.get(HOST_URL + 'Mold/GetMoldMovementHistory?moldId=' + $scope.modelObject.Id, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.movementList = resp.data;
                    }

                    $('#dial-history').dialog({
                        width: window.innerWidth * 0.9,
                        height: window.innerHeight * 0.9,
                        hide: true,
                        modal: true,
                        resizable: false,
                        show: true,
                        draggable: false,
                        open: function (event, ui) {
                            $scope.bindHistoryTable();
                        },
                        closeText: "KAPAT"
                    });
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.bindHistoryTable = function () {
        $('#dtMovementHistory').dxDataGrid({
            dataSource: {
                load: function () {
                    return $scope.movementList;
                },
                key: 'ItemReceiptDetailId'
            },
            showColumnLines: false,
            showRowLines: true,
            rowAlternationEnabled: true,
            focusedRowEnabled: true,
            allowColumnResizing: true,
            wordWrapEnabled: true,
            showBorders: true,
            filterRow: {
                visible: true
            },
            headerFilter: {
                visible: true
            },
            /*height: 400,*/
            paging: {
                enabled: false,
                pageSize: 10,
                pageIndex: 0
            },
            groupPanel: {
                visible: true
            },
            editing: {
                allowUpdating: false,
                allowDeleting: false
            },
            columns: [
                { dataField: 'ReceiptDateStr', caption: 'Tarih' },
                { dataField: 'FirmName', caption: 'Firma' },
                { dataField: 'WarehouseName', caption: 'Depo' },
                { dataField: 'ReceiptTypeStr', caption: 'Hareket Türü', },
                {
                    type: "buttons",
                    buttons: [
                        {
                            name: 'showoperations', cssClass: 'fas fa-list', text: '', onClick: function (e) {
                                var dataGrid = $("#dtMovementHistory").dxDataGrid("instance");
                                dataGrid.deselectAll();
                                dataGrid.selectRowsByIndexes([e.row.rowIndex]);

                                // e.row.data.Id
                            },
                        }
                    ]
                },
                {
                    type: "buttons",
                    buttons: [
                        {
                            name: 'showreceipt', cssClass: 'fas fa-edit', text: '', onClick: function (e) {
                                var dataGrid = $("#dtMovementHistory").dxDataGrid("instance");
                                dataGrid.deselectAll();
                                dataGrid.selectRowsByIndexes([e.row.rowIndex]);

                                window.open(HOST_URL + 'ItemReceipt?rid=' + e.row.data.ItemReceiptId + '&receiptCategory=' +
                                    e.row.data.ReceiptCategory, '_blank');
                            }
                        }
                    ]
                },
                
            ]
        });
    }
    // #endregion

    $scope.printLabel = function () {
        $('#dial-print-label').dialog({
            width: 520,
            height: 520,
            hide: true,
            modal: true,
            resizable: false,
            show: true,
            draggable: false,
            closeText: "KAPAT"
        });
    }

    $scope.printMoldLabel = function () {
        var printContents = document.getElementById('mold-label').innerHTML;
        var originalContents = document.body.innerHTML;
        document.body.innerHTML = printContents;
        window.print();
        document.body.innerHTML = originalContents;
        window.location.reload();
    }

    $scope.dropDownBoxEditorTemplate = function (cellElement, cellInfo) {
        return $("<div>").dxDropDownBox({
            dropDownOptions: { width: 500 },
            dataSource: $scope.itemList,
            value: cellInfo.value,
            valueExpr: "Id",
            displayExpr: "ItemNo",
            contentTemplate: function (e) {
                return $("<div>").dxDataGrid({
                    dataSource: $scope.itemList,
                    remoteOperations: true,
                    columns: [
                        { dataField: 'ItemNo', caption: 'Ürün Kodu' },
                        { dataField: 'ItemName', caption: 'Ürün Adı' },
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

    $scope.bindDetails = function () {
        $('#productList').dxDataGrid({
            dataSource: {
                load: function () {
                    return $scope.modelObject.Products;
                },
                update: function (key, values) {
                    var obj = $scope.modelObject.Products.find(d => d.Id == key);
                    if (obj != null) {

                        if (typeof values.ItemId != 'undefined') {
                            var itemObj = $scope.itemList.find(d => d.Id == values.ProductId);
                            obj.ProductId = itemObj.Id;
                            obj.ProductCode = itemObj.ItemNo;
                            obj.ProductName = itemObj.ItemName;
                        }

                        if (typeof values.LineNumber != 'undefined') { obj.LineNumber = values.LineNumber; }
                    }
                },
                remove: function (key) {
                    var obj = $scope.modelObject.Products.find(d => d.Id == key);
                    if (obj != null) {
                        $scope.modelObject.Products.splice($scope.modelObject.Products.indexOf(obj), 1);
                    }
                },
                insert: function (values) {
                    var newId = 1;
                    if ($scope.modelObject.Products.length > 0) {
                        newId = $scope.modelObject.Products.map(d => d.Id).reduce((max, n) => n > max ? n : max)
                        newId++;
                    }

                    var itemObj = $scope.itemList.find(d => d.Id == values.ProductId);

                    var newObj = {
                        Id: newId,
                        ProductId: itemObj.Id,
                        ProductCode: itemObj.ItemNo,
                        ProductName: itemObj.ItemName,
                        LineNumber: newId,
                        NewDetail: true
                    };

                    $scope.modelObject.Products.push(newObj);
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
            onInitNewRow: function (e) {
                e.data.UnitPrice = 0;
                e.data.TaxIncluded = 0;
            },
            repaintChangesOnly: true,
            columns: [
                {
                    allowEditing: false, caption: '#', width:50,
                    cellTemplate: function (cellElement, cellInfo) {
                        cellElement.text(cellInfo.row.rowIndex + 1)
                    }
                },
                {
                    dataField: 'ProductId', caption: 'Ürün Kodu',
                    lookup: {
                        dataSource: $scope.itemList,
                        valueExpr: "Id",
                        displayExpr: "ItemNo"
                    },
                    allowSorting: false,
                    validationRules: [{ type: "required" }],
                    editCellTemplate: $scope.dropDownBoxEditorTemplate,
                    cellTemplate: function (container, options) {
                        if (typeof options.row.data.ProductCode != 'undefined'
                            && options.row.data.ProductCode != null && options.row.data.ProductCode.length > 0)
                            container.text(options.row.data.ProductCode);
                        else
                            container.text(options.displayValue);
                    }
                },
                { dataField: 'ProductName', caption: 'Ürün Adı', allowEditing: false },
            ]
        });
    }

    $scope.bindModel = function (id) {
        $http.get(HOST_URL + 'Mold/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;

                    if ($scope.modelObject.Id <= 0) {
                        $scope.modelObject.CreatedDateStr = moment().format('DD.MM.YYYY');
                        $scope.modelObject.Details = [];
                    }
                    else {
                        if ($scope.modelObject.CreatedDateStr == null)
                            $scope.modelObject.CreatedDateStr = moment().format('DD.MM.YYYY');
                    }

                    // BIND SELECTED ITEMS
                    if (typeof $scope.modelObject.FirmId != 'undefined' && $scope.modelObject.FirmId != null)
                        $scope.selectedFirm = $scope.firmList.find(d => d.Id == $scope.modelObject.FirmId);
                    else
                        $scope.selectedFirm = $scope.firmList[0];

                    refreshArray($scope.firmList);

                    if (typeof $scope.modelObject.InWarehouseId != 'undefined' && $scope.modelObject.InWarehouseId != null)
                        $scope.selectedWarehouse = $scope.warehouseList.find(d => d.Id == $scope.modelObject.InWarehouseId);
                    else
                        $scope.selectedWarehouse = $scope.warehouseList[0];

                    refreshArray($scope.warehouseList);

                    $scope.bindDetails();
                }
            }).catch(function (err) { });
    }

    // #region MNT CALENDAR
    $scope.showMntCalendar = function () {
        $('#dial-mnt-calendar').dialog({
            width: window.innerWidth * 0.9,
            height: window.innerHeight * 0.9,
            hide: true,
            modal: true,
            resizable: false,
            show: true,
            draggable: false,
            open: function (event, ui) {
                $scope.loadScheduler();
            },
            closeText: "KAPAT"
        });
    }

    $scope.mntData = [];

    $scope.isWeekEnd = function (date) {
        var day = date.getDay();
        return day === 0 || day === 6;
    }

    $scope.loadScheduler = function () {
        $("#schedulerContainer").html('');

        $("#schedulerContainer").dxScheduler({
            dataSource: $scope.mntData,
            currentDate: moment(),
            views: ["month"],
            currentView: "month",
            firstDayOfWeek: 1,
            startDayHour: 8,
            endDayHour: 18,
            showAllDayPanel: false,
            height: 600,
            onAppointmentAdded: function (e) {
                $scope.saveWork(e.appointmentData);
            },
            onAppointmentUpdated: function (e) {
                $scope.saveWork(e.appointmentData);
            },
            onAppointmentDeleted: function (e) {
                if (typeof e.appointmentData != 'undefined')
                    $scope.deleteWork(e.appointmentData.Id);
            },
            onInitialized: function (objE) {
            },
            //resources: [
            //    {
            //        fieldExpr: "AssigneeId",
            //        allowMultiple: false,
            //        dataSource: $scope.schedulerEmployees,
            //        label: "Personel"
            //    }
            //],
            //resourceCellTemplate: function (cellData) {
            //    var name = $("<div>")
            //        .addClass("name")
            //        .css({ backgroundColor: cellData.color })
            //        .append($("<h2>")
            //            .text(" "));

            //    var avatar = $("<div>")
            //        .addClass("avatar")
            //        .html("<img class=\"rounded\" height=\"120\" src=\"" + (cellData.data.PictureBase64.length > 0 ? cellData.data.PictureBase64 : "") + "\">")
            //        .attr("title", cellData.text);

            //    var info = $("<div>")
            //        .addClass("info")
            //        .css({ color: cellData.color })
            //        .html("Adı: " + cellData.data.EmployeeName + " " + cellData.data.EmployeeSurname + "<br/><b>" + cellData.data.UserGroupName + "</b>");

            //    return $("<div>").append([name, avatar, info]);
            //},
            //dataCellTemplate: function (cellData, index, container) {
            //    var employeeID = cellData.groups.AssigneeId,
            //        currentWork = $scope.getCurrentWork(cellData.startDate.getDate(), employeeID);

            //    var wrapper = $("<div>")
            //        .toggleClass("employee-weekend-" + employeeID, $scope.isWeekEnd(cellData.startDate)).appendTo(container)
            //        .addClass("employee-" + employeeID)
            //        .addClass("dx-template-wrapper");

            //    wrapper.append($("<div>")
            //        .text(cellData.text)
            //        .addClass(currentWork)
            //        .addClass("day-cell")
            //    );

            //}
        });
    }

    $scope.saveWork = function (dataModel) {
        var foundData = null;

        $scope.mntData.push(dataModel);
        //if (typeof dataModel.Id == 'undefined' || dataModel.Id == 0) {
        //    foundData = {
        //        Id: 0,
        //        StartDate: dataModel.startDate,
        //        EndDate: dataModel.endDate,
        //        Explanation: dataModel.text
        //    };
        //}
        //else {
        //    dataModel.StartDate = dataModel.startDate;
        //    dataModel.EndDate = dataModel.endDate;
        //    dataModel.Explanation = dataModel.text;

        //    foundData = dataModel;
        //}

        //delete foundData.startDate;
        //delete foundData.endDate;
    }

    $scope.deleteWork = function (dataId) {
        
    }
    // #endregion

    // ON LOAD EVENTS
    DevExpress.localization.locale('tr');
    $scope.loadSelectables().then(function () {
        if (PRM_ID > 0)
            $scope.bindModel(PRM_ID);
        else
            $scope.bindModel(0);
    });
});