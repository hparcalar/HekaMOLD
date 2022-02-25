DevExpress.localization.locale('tr');

app.controller('deliveryPlanningCtrl', function deliveryPlanningCtrl($scope, $http) {
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

    $scope.getMachinePlans = function (machine) {
        return $scope.boardPlanList.filter(d => d.PlanDateStr == machine.PlanDateTitle);
    }

    $scope.toggleWaitingPlans = function () {
        $scope.waitingPlansVisible = !$scope.waitingPlansVisible;
    }

    $scope.selectPlan = function (planItem) {
        $scope.selectedPlan = planItem;
        $scope.selectedSource = $scope.machineList.find(d => d.Id == planItem.MachineId);
    }

    $scope.selectSource = function (planDate) {
        $scope.selectedSource = $scope.machineList.find(d => d.PlanDate == planDate);
    }

    $scope.getCurrentWeekNumber = function () {
        return moment().week();
    }

    // DATA GET METHODS
    $scope.loadMachineList = function () {
        $scope.machineList.splice(0, $scope.machineList.length);
        for (var i = 1; i < 7; i++) {
            var dateOfDay = moment().day(i).week(moment().week());
            $scope.machineList.push({ PlanDate: dateOfDay, PlanDateTitle: dateOfDay.format('DD.MM.YYYY') })
        }

        $scope.loadRunningBoard();
    }
    $scope.loadRunningBoard = function () {
        $http.get(HOST_URL + 'Delivery/GetProductionPlans', {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.boardPlanList = resp.data;
                    setTimeout(function () { $scope.bindBoardDragDropEvents(); }, 250);
                }
            }).catch(function (err) { });
    }
    $scope.loadWaitingPlanList = function () {
        $http.get(HOST_URL + 'Delivery/GetWaitingPlans', {}, 'json')
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
                //if (planObj.WorkOrder.WorkOrderStatus != 1) {
                //    toastr.error('Tamamlanmış bir planı değiştiremezsiniz.', 'Uyarı');
                //    return false;
                //}

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
            //else {
            //    var planObj = $scope.boardPlanList.find(d => d.Id == parseInt(planId));
            //    if (planObj.WorkOrder.WorkOrderStatus != 1) {
            //        toastr.error('Tamamlanmış bir planın önüne üretim alamazsınız.', 'Uyarı');
            //        return false;
            //    }
            //}

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
                    planObj.PlanDateStr = hoverPlanObj.PlanDateStr;

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
                    planObj.PlanDateStr = machineId;
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

        $http.post(HOST_URL + 'Delivery/SaveModel', planObj, 'json')
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

        $http.post(HOST_URL + 'Delivery/ReOrderPlan', planObj, 'json')
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
        $http.get(HOST_URL + 'Delivery/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;
                }
            }).catch(function (err) { });
    }

    // WAITING PLANS GRID
    $scope.lastProcessedPlanId = '';
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
            width: 450,
            onEditorPreparing: function (e) {
                if (e.parentType === "filterRow") {
                    let onValueChanged = e.editorOptions.onValueChanged
                    e.editorOptions.onValueChanged = function (args) {
                        e.component.columnOption(e.dataField, "filterValue", args.value.toUpperCase())
                    }
                }
            },
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
                                planObj.DeliveryPlanDateStr = machineId;
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
                        "<td>" + data.WorkOrderDateStr + "</td>" +
                        "<td>" + data.SaleOrderDeadline + "</td>" +
                        "<td>" + data.FirmName + "</td>" +
                        "<td>" + data.ProductCode + "</td>" +
                        "<td>" + data.ProductName + "</td>" +
                        "<td class=\"text-right\">" + data.Quantity.toFixed(2) + "</td>" +
                        "</tr>";

                container.append(markup);
            },
            columns: [
                {
                    dataField: 'WorkOrderDateStr',
                    caption: 'Tarih', dataType: 'date', format: 'dd.MM.yyyy'
                },
                {
                    dataField: 'SaleOrderDeadline',
                    caption: 'Termin', dataType: 'date', format: 'dd.MM.yyyy'
                },
                { dataField: 'FirmName', caption: 'Firma' },
                { dataField: 'ProductCode', caption: 'Ürün Kodu' },
                { dataField: 'ProductName', caption: 'Ürün Adı' },
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

    // TOASTR SETTINGS
    toastr.options.timeOut = 2000;
    toastr.options.progressBar = true;
    toastr.options.preventDuplicates = true;

    // EDIT PLAN DETAILS
    $scope.showPlanDetails = function (plan, uiElement) {
        $scope.$broadcast('loadEditPlan', { id: plan.Id });

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

    $scope.$on('editPlanEnd', function (e, d) {
        $('#dial-plan').dialog('close');

        $scope.loadMachineList();
        $scope.loadWaitingPlanList();
    });

    // KEYBOARD EVENTS
    $(document).keyup(function (event) {
        if (event.key == 'Delete') // DEL
        {
            if ($scope.selectedPlan != null && $scope.selectedPlan.Id > 0) {
                bootbox.confirm({
                    message: "Bu sevkiyat planını silmek istediğinizden emin misiniz?",
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
                            $http.post(HOST_URL + 'Delivery/DeletePlan', { rid: $scope.selectedPlan.Id }, 'json')
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
    });

    // ON LOAD EVENTS
    $scope.loadMachineList();
    $scope.loadWaitingPlanList();
});