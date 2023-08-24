app.controller('maintenancePlanCtrl', function ($scope, $http) {
    // #region COMPONENT VARIABLES
    $scope.mntData = [];
    $scope.machineList = [];
    $scope.userList = [];
    // #endregion

    // #region COMPONENT FUNCTIONS
    $scope.bindScheduleModel = function () {
        try {
            $http.get(HOST_URL + 'Common/GetMachineList', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.machineList = resp.data;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }

        try {
            $http.get(HOST_URL + 'User/SearchUserList?roleFilter=Bakım', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.userList = resp.data;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.getPlanById = async function (planId) {
        return null;
    }
    // #endregion

    // #region MAINTENANCE CALENDAR
    $scope.isWeekEnd = function (date) {
        var day = date.getDay();
        return day === 0 || day === 6;
    }

    $scope.loadScheduler = function () {
        $("#schedulerContainer").html('');

        $("#schedulerContainer").dxScheduler({
            dataSource: $scope.mntData,
            currentDate: moment(),
            views: ["day","week","month"],
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
            onAppointmentFormOpening: async function(data) {
                const { form } = data;
                let planInfo = (await $scope.getPlanById(data.appointmentData.Id))
                    || {
                        PlanStatus: 0,
                        StartDate: moment().toDate(),
                };
                data.appointmentData = planInfo;
                data.appointmentData.startDate = planInfo.StartDate;
                data.appointmentData.endDate = planInfo.EndDate;
                console.log(data);
                //let { startDate, endDate } = data.appointmentData;

                form.option('items', [
                    {
                        label: {
                            text: 'Makine',
                        },
                        editorType: 'dxSelectBox',
                        dataField: 'MachineId',
                        editorOptions: {
                            items: $scope.machineList,
                            displayExpr: 'MachineName',
                            valueExpr: 'Id',
                            onValueChanged(args) {
                                //movieInfo = getMovieById(args.value);

                                //form.updateData('director', movieInfo.director);
                                //form.updateData('endDate', new Date(startDate.getTime() + 60 * 1000 * movieInfo.duration));
                            },
                        },
                    },
                    {
                        label: {
                            text: 'Personel',
                        },
                        editorType: 'dxSelectBox',
                        dataField: 'ResponsibleUserId',
                        editorOptions: {
                            items: $scope.userList,
                            displayExpr: 'UserName',
                            valueExpr: 'Id',
                            onValueChanged(args) {
                                //movieInfo = getMovieById(args.value);

                                //form.updateData('director', movieInfo.director);
                                //form.updateData('endDate', new Date(startDate.getTime() + 60 * 1000 * movieInfo.duration));
                            },
                        },
                    },
                    {
                    label: {
                        text: 'Konu',
                    },
                    name: 'Subject',
                    editorType: 'dxTextBox',
                    editorOptions: {
                        value: planInfo.Subject,
                    },
                    },
                    {
                        label: {
                            text: 'Açıklama',
                        },
                        name: 'Explanation',
                        editorType: 'dxTextBox',
                        editorOptions: {
                            width: '100%',
                            value: planInfo.Explanation,
                        },
                    },
                    {
                        label: {
                            text: 'Başlangıç'
                        },
                    dataField: 'StartDate',
                    editorType: 'dxDateBox',
                    editorOptions: {
                        width: '100%',
                        type: 'datetime',
                        onValueChanged(args) {
                            data.appointmentData.startDate = args.value;
                        },
                    },
                    }, {
                        label: {
                            text: 'Bitiş'
                        },
                    dataField: 'EndDate',
                    editorType: 'dxDateBox',
                    editorOptions: {
                        width: '100%',
                        type: 'datetime',
                        onValueChanged(args) {
                            data.appointmentData.endDate = args.value;
                        },
                    },
                    },
                    
                    {
                        label: {
                            text: 'Durum',
                        },
                    dataField: 'PlanStatus',
                    editorType: 'dxRadioGroup',
                    editorOptions: {
                        dataSource: [{Id:0, Text:'Bekleniyor'}, {Id:1, Text:'Başlandı'}, {Id:2, Text:'Tamamlandı'}],
                        displayExpr: 'Text',
                        valueExpr: 'Id',
                        itemTemplate(itemData) {
                            return `${itemData.Text}`;
                        },
                    },
                },
                ]);
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
    $scope.bindScheduleModel();
    $scope.loadScheduler();

    $scope.$on('loadMaintenancePlan', function (e, d) {
        $scope.bindScheduleModel();
        $scope.loadScheduler();
    });
});