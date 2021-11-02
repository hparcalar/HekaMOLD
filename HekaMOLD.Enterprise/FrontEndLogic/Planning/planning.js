DevExpress.localization.locale('tr');

app.controller('workOrderPlanningCtrl', function planningCtrl($scope, $http) {
    $scope.modelObject = { Id: 0 };
    $scope.saveStatus = 0;

    $scope.waitingPlansVisible = true;

    $scope.machineList = [];
    $scope.selectedMachineList = [];
    $scope.boardPlanList = [];
    $scope.waitingPlanList = [];
    $scope.selectedPlan = { Id: 0 };
    $scope.copiedPlan = { Id: 0 };
    $scope.selectedSource = { Id:0 }; // Plan queue of a selected machine for highlighting

    $scope.getMachinePlans = function (machine, isActive = true) {
        return $scope.boardPlanList.filter(d => d.MachineId == machine.Id
            && ((isActive == true && d.WorkOrder.WorkOrderStatus != 6 && d.WorkOrder.WorkOrderStatus != 4)
                || (isActive == false && (d.WorkOrder.WorkOrderStatus == 6 || d.WorkOrder.WorkOrderStatus == 4))));
    }

    $scope.toggleWaitingPlans = function () {
        $scope.waitingPlansVisible = !$scope.waitingPlansVisible;
    }

    $scope.selectPlan = function (planItem) {
        $scope.selectedPlan = planItem;
        $scope.selectedSource = $scope.machineList.find(d => d.Id == planItem.MachineId);
    }

    $scope.selectSource = function (machineId) {
        $scope.selectedSource = $scope.machineList.find(d => d.Id == machineId);
    }

    // VISUAL TRIGGERS & METHODS
    $scope.showPlanDetails = function (plan, uiElement) {
        $scope.$broadcast('loadEditPlan', { id: plan.WorkOrderDetailId });

        $('#dial-plan').dialog({
            hide: true,
            modal: true,
            resizable: false,
            width: 600,
            show: true,
            draggable: false,
            closeText: "KAPAT"
        });
    }
    $scope.showProperProductName = function (workOrder) {
        return workOrder.ProductName.replace(workOrder.FirmName.split(' ')[0], '');
    }

    // DATA GET METHODS
    $scope.loadMachineList = function () {
        $http.get(HOST_URL + 'Planning/GetMachineList', {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.machineList = resp.data;

                    if ($scope.selectedMachineList.length == 0) {
                        $scope.machineList.forEach(d => {
                            $scope.selectedMachineList.push(d);
                        });
                    }

                    $scope.loadMachineSelection();
                    $scope.loadRunningBoard();
                }
            }).catch(function (err) { });
    }
    $scope.loadRunningBoard = function () {
        $http.get(HOST_URL + 'Planning/GetProductionPlans', {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.boardPlanList = resp.data;
                    setTimeout(function () { $scope.bindBoardDragDropEvents(); }, 250);
                }
            }).catch(function (err) { });
    }
    $scope.loadWaitingPlanList = function () {
        $http.get(HOST_URL + 'Planning/GetWaitingPlans', {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.waitingPlanList = resp.data;
                    $scope.loadWaitingPlans();
                }
            }).catch(function (err) { });
    }

    // BOARD DRAG & DROP EVENTS
    $scope.bindBoardDragDropEvents = function () {
        var takenPlanId = null;

        $('.plan-box').unbind('dragstart');
        $('.plan-box').unbind('dragenter');
        $('.plan-box').unbind('dragleave');
        $('.plan-box').unbind('drop');

        $('.machine-box').unbind('drop');
        $('.machine-box').unbind('dragover');

        $('.plan-box').on('dragstart', function (de) {
            var planId = $(this).attr('data-plan-id');
            takenPlanId = planId;

            var planObj = $scope.boardPlanList.find(d => d.Id == parseInt(planId));
            if (planObj != null) {
                if (planObj.WorkOrder.WorkOrderStatus > 3) {
                    toastr.error('Tamamlanmış bir planı değiştiremezsiniz.', 'Uyarı');
                    return false;
                }

                de.originalEvent.dataTransfer.setData('text/plain', planId);
            }
        });

        var refCounter = 0;
        $('.plan-box').on('dragenter', function (de) {
            de.preventDefault();

            refCounter++;

            var planId = $(this).attr('data-plan-id');
            if (takenPlanId == planId)
                return false;
            else {
                var planObj = $scope.boardPlanList.find(d => d.Id == parseInt(planId));
                if (planObj.WorkOrder.WorkOrderStatus > 3) {
                    toastr.error('Tamamlanmış bir planın önüne üretim alamazsınız.', 'Uyarı');
                    return false;
                }
            }

            $('.plan-box').removeClass('drag-over');
            $(this).addClass('drag-over');
        });

        $('.plan-box').on('dragleave', function (de) {
            //refCounter--;

            //if (refCounter == 0)
                //$(this).removeClass('drag-over');
        });

        $('.plan-box').on('drop', function (de) {
            de.preventDefault();

            var hoverPlanId = $(this).attr('data-plan-id');
            var hoverPlanObj = $scope.boardPlanList.find(d => d.Id == parseInt(hoverPlanId));

            if (hoverPlanObj != null) {
                var planId = de.originalEvent.dataTransfer.getData("text/plain");
                var planObj = $scope.boardPlanList.find(d => d.Id == parseInt(planId));
                if (planObj != null) {
                    planObj.OrderNo = hoverPlanObj.OrderNo;
                    planObj.MachineId = hoverPlanObj.MachineId;

                    $scope.reOrderPlan(planObj);
                }
            }

            refCounter = 0;
            $(this).removeClass('drag-over');
            return false;
        });

        $('.machine-box').on('drop', function (event) {
            event.preventDefault();

            refCounter = 0;

            var planId = event.originalEvent.dataTransfer.getData("text/plain");
            var machineId = $(this).attr('data-id');

            if (typeof machineId != 'undefined') {
                var planObj = $scope.boardPlanList.find(d => d.Id == parseInt(planId));
                if (typeof planObj != 'undefined') {
                    planObj.MachineId = parseInt(machineId);
                    planObj.OrderNo = -1; // SET AS LAST ORDER OF SELECTED MACHINE

                    $scope.reOrderPlan(planObj);
                }
            }
        });

        $('.machine-box').on('dragover', function (event) {
            event.preventDefault();
        });
    }

    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0 };
    }

    $scope.saveModel = function (planObj) {
        $scope.saveStatus = 1;

        $http.post(HOST_URL + 'Planning/SaveModel', planObj, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.saveStatus = 0;

                    if (resp.data.Result == true) {
                        toastr.success('İşlem başarılı.', 'Bilgilendirme');

                        $scope.loadMachineList();
                        $scope.loadWaitingPlanList();
                    }
                    else
                        toastr.error(resp.data.ErrorMessage, 'Hata');
                }
            }).catch(function (err) { });
    }

    $scope.reOrderPlan = function (planObj) {
        $scope.saveStatus = 1;

        $http.post(HOST_URL + 'Planning/ReOrderPlan', planObj, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.saveStatus = 0;

                    if (resp.data.Result == true) {
                        toastr.success('İşlem başarılı.', 'Bilgilendirme');

                        $scope.loadMachineList();
                        $scope.loadWaitingPlanList();

                        $scope.selectPlan(planObj);
                    }
                    else
                        toastr.error(resp.data.ErrorMessage, 'Hata');
                }
            }).catch(function (err) { });
    }

    $scope.bindModel = function (id) {
        $http.get(HOST_URL + 'Planning/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;
                }
            }).catch(function (err) { });
    }

    // WAITING PLANS GRID
    $scope.lastProcessedPlanId = 0;
    $scope.loadWaitingPlans = function () {
        var cFilter = null;

        try {
            var dataGrid = $("#waitingPlanList").dxDataGrid("instance");
            if (dataGrid != null) {
                cFilter = dataGrid.getCombinedFilter(true);
            }
        } catch (e) {

        }
        
        $('#waitingPlanList').dxDataGrid({
            dataSource: $scope.waitingPlanList,
            keyExpr: 'Id',
            showColumnLines: false,
            showRowLines: true,
            rowAlternationEnabled: true,
            focusedRowEnabled: true,
            cacheEnabled: false,
            allowColumnResizing: true,
            wordWrapEnabled: true,
            showBorders: true,
            filterRow: {
                visible: true
            },
            headerFilter: {
                visible: true
            },
            groupPanel: {
                visible: false
            },
            scrolling: {
                mode: "virtual"
            },
            height: 450,
            width:450,
            editing: {
                allowUpdating: false,
                allowDeleting: false
            },
            onContentReady: function () {
                $('.waiting-plan-row').on('dragstart', function (de) {
                    de.originalEvent.dataTransfer.setData('text/plain', $(this).attr('data-id'));

                    $('.machine-box').bind('drop', function (event) {
                        event.preventDefault();

                        var planId = event.originalEvent.dataTransfer.getData("text/plain");
                        if (planId == $scope.lastProcessedPlanId)
                            return;
                        else
                            $scope.lastProcessedPlanId = planId;

                        var machineId = $(this).attr('data-id');

                        if (typeof machineId != 'undefined') {
                            var planObj = $scope.waitingPlanList.find(d => d.Id == parseInt(planId));
                            if (typeof planObj != 'undefined') {
                                planObj.MachineId = parseInt(machineId);
                                $scope.saveModel(planObj);
                            }
                        }
                    });
                });

                $('.waiting-plan-row').on('drop', function (de) {
                    return false;
                });

                $('.machine-box').on('dragover', function (event) {
                    event.preventDefault();
                });
            },
            rowTemplate: function (container, item) {
                var data = item.data,
                    markup =
                        "<tr draggable=\"true\" class=\"dx-row dx-data-row dx-row-lines waiting-plan-row\" data-id=\"" + data.Id + "\">" +
                        "<td>" + data.OrderDateStr + "</td>" +
                        "<td>" + data.DeadlineDateStr + "</td>" +
                        "<td>" + data.FirmName + "</td>" +
                        "<td>" + data.ItemNo + "</td>" +
                        "<td>" + data.ItemName + "</td>" +
                        "<td class=\"text-right\">" + data.Quantity.toFixed(2) + "</td>" +
                        "</tr>";

                container.append(markup);
            },
            columns: [
                {
                    dataField: 'OrderDateStr',
                    caption: 'Tarih', dataType: 'date', format: 'dd.MM.yyyy'
                },
                {
                    dataField: 'DeadlineDateStr',
                    caption: 'Termin', dataType: 'date', format: 'dd.MM.yyyy'
                },
                { dataField: 'FirmName', caption: 'Firma' },
                { dataField: 'ItemNo', caption: 'Ürün Kodu' },
                { dataField: 'ItemName', caption: 'Ürün Adı' },
                {
                    dataField: 'Quantity', caption: 'Miktar',
                    dataType: 'number', format: { type: "fixedPoint", precision: 2 }
                }
            ]
        });

        var dGrid = $("#waitingPlanList").dxDataGrid("instance");
        if (cFilter != null)
            dGrid.filter(cFilter);
    }
    $scope.refreshList = function () {
        var dataGrid = $("#waitingPlanList").dxDataGrid("instance");
        dataGrid.refresh();
    }

    $scope.loadMachineSelection = function () {
        $("#machineSelection").dxDropDownBox({
            value: [],
            valueExpr: "Id",
            placeholder: "Makine Seçin",
            displayExpr: "MachineName",
            showClearButton: true,
            dataSource: $scope.machineList,
            contentTemplate: function (e) {
                var value = e.component.option("value"),
                    $dataGrid = $("<div>").dxDataGrid({
                        dataSource: $scope.machineList,
                        keyExpr: "Id",
                        columns: [{ dataField: 'MachineCode', caption: 'Kodu' }, { dataField: 'MachineName', caption: 'Adı' }],
                        hoverStateEnabled: true,
                        paging: { enabled: true, pageSize: 10 },
                        filterRow: { visible: true },
                        scrolling: { mode: "infinite" },
                        height: 345,
                        selection: { mode: "multiple" },
                        selectedRowKeys: $scope.selectedMachineList.map(m => m.Id),
                        onSelectionChanged: function (selectedItems) {
                            var keys = selectedItems.selectedRowKeys;

                            if ($scope.selectedMachineList.length > 0)
                                $scope.selectedMachineList.splice(0, $scope.selectedMachineList.length);

                            var selectedMachines = $scope.machineList.filter(d => keys.indexOf(d.Id) > -1);
                            selectedMachines.forEach(d => { $scope.selectedMachineList.push(d); });
                            e.component.option("value", keys);
                        }
                    });

                dataGrid = $dataGrid.dxDataGrid("instance");

                e.component.on("valueChanged", function (args) {
                    var value = args.value;
                    dataGrid.selectRows(value, false);

                    $scope.$apply();
                });

                return $dataGrid;
            }
        });
    }

    // TOASTR SETTINGS
    toastr.options.timeOut = 2000;
    toastr.options.progressBar = true;
    toastr.options.preventDuplicates = true;

    // KEYBOARD EVENTS
    $scope.ctrlDown = false;
    $(document).keyup(function (event) {
        if (event.key == 'Delete') // DEL
        {
            if ($scope.selectedPlan != null && $scope.selectedPlan.Id > 0) {
                bootbox.confirm({
                    message: "Bu üretim planını silmek istediğinizden emin misiniz?",
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
                            $http.post(HOST_URL + 'Planning/DeletePlan', { rid: $scope.selectedPlan.Id }, 'json')
                                .then(function (resp) {
                                    if (typeof resp.data != 'undefined' && resp.data != null) {
                                        if (resp.data.Result == true) {
                                            toastr.success('Plan başarıyla silindi.', 'Bilgilendirme');

                                            $scope.loadMachineList();
                                            $scope.loadWaitingPlanList();
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
        else if (event.key == 'Control') {
            $scope.ctrlDown = false;
        }
    });

    $(document).keydown(function (event) {
        if (event.key == 'Control')
            $scope.ctrlDown = true;
        else if (event.key == 'c' && $scope.ctrlDown) {
            if ($scope.selectedPlan.Id > 0) {
                $scope.copiedPlan = $scope.selectedPlan;
                toastr.success($scope.selectedPlan.WorkOrder.ProductName
                    + ' iş emri kopyalandı. Makine seçip CTRL + V ile yapıştırabilirsiniz.', 'Kopyalama');
            }
        }
        else if (event.key == 'v' && $scope.ctrlDown) {
            if ($scope.copiedPlan.Id > 0) {
                // DO BROADCAST
                $scope.$broadcast('loadPlanCopy');

                $('#dial-copy-plan').dialog({
                    width: window.innerWidth * 0.5,
                    height: window.innerHeight * 0.5,
                    hide: true,
                    modal: true,
                    resizable: false,
                    show: true,
                    draggable: false,
                    closeText: "KAPAT"
                });
            }
            else
                toastr.error('Yapıştırabilmek için önce bir iş emri kopyalamalısınız.', 'Uyarı');
        }
    });

    // EMIT ORDER DETAIL DATA
    $scope.$on('confirmCopy', function (e, d) {
        $scope.saveStatus = 1;

        $http.post(HOST_URL + 'Planning/CopyPlan', {
            fromPlanId: $scope.copiedPlan.Id,
            targetMachineId: $scope.selectedSource.Id,
            quantity: d.quantity,
            firmId: d.firmId,
        }, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.saveStatus = 0;

                    if (resp.data.Result == true) {
                        toastr.success('İşlem başarılı.', 'Bilgilendirme');

                        $scope.loadMachineList();
                        $scope.loadWaitingPlanList();
                    }
                    else
                        toastr.error(resp.data.ErrorMessage, 'Hata');
                }
            }).catch(function (err) { });

        $scope.copiedPlan = { Id: 0 };
        $('#dial-copy-plan').dialog('close');
    });
    $scope.$on('editPlanEnd', function (e, d) {
        $('#dial-plan').dialog('close');

        $scope.loadMachineList();
        $scope.loadWaitingPlanList();
    });

    // ON LOAD EVENTS
    $scope.loadMachineList();
    $scope.loadWaitingPlanList();
});