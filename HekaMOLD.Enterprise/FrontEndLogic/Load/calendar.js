app.controller('calendarCtrl', function ($scope, $http) {
    // #region COMPONENT VARIABLES
    $scope.mntData = [];
    // #endregion

    $scope.isWeekEnd = function (date) {
        var day = date.getDay();
        return day === 0 || day === 6;
    }

    $scope.loadScheduler = function () {
        $("#schedulerContainer").html('');

        $("#schedulerContainer").dxScheduler({
            dataSource: $scope.mntData,
            currentDate: moment(),
            views: ["day", "week", "month"],
            currentView: "month",
            firstDayOfWeek: 1,
            startDayHour: 8,
            endDayHour: 18,
            //showAllDayPanel: false,
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
            //onAppointmentFormOpening(data) {
            //    const { form } = data;
            //    let movieInfo = {}; //getMovieById(data.appointmentData.movieId) || {};
            //    let { startDate } = data.appointmentData;

            //    form.option('items', [{
            //        label: {
            //            text: 'Movie',
            //        },
            //        editorType: 'dxSelectBox',
            //        dataField: 'movieId',
            //        editorOptions: {
            //            items: [],
            //            displayExpr: 'text',
            //            valueExpr: 'id',
            //            onValueChanged(args) {
            //                //movieInfo = getMovieById(args.value);

            //                //form.updateData('director', movieInfo.director);
            //                //form.updateData('endDate', new Date(startDate.getTime() + 60 * 1000 * movieInfo.duration));
            //            },
            //        },
            //    }, {
            //        label: {
            //            text: 'Director',
            //        },
            //        name: 'director',
            //        editorType: 'dxTextBox',
            //        editorOptions: {
            //            value: movieInfo.director,
            //            readOnly: true,
            //        },
            //    }, {
            //        dataField: 'startDate',
            //        editorType: 'dxDateBox',
            //        editorOptions: {
            //            width: '100%',
            //            type: 'datetime',
            //            onValueChanged(args) {
            //                startDate = args.value;

            //                form.updateData('endDate', new Date(startDate.getTime() + 60 * 1000 * movieInfo.duration));
            //            },
            //        },
            //    }, {
            //        name: 'endDate',
            //        dataField: 'endDate',
            //        editorType: 'dxDateBox',
            //        editorOptions: {
            //            width: '100%',
            //            type: 'datetime',
            //            readOnly: true,
            //        },
            //    }, {
            //        dataField: 'price',
            //        editorType: 'dxRadioGroup',
            //        editorOptions: {
            //            dataSource: [5, 10, 15, 20],
            //            itemTemplate(itemData) {
            //                return `$${itemData}`;
            //            },
            //        },
            //    },
            //    ]);
            //},
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

    //$scope.dataPush = function (dataId) {
    //    $scope.mntData.forEach(element => data.push({ text: element.text, startDate: new Date( element.startDate), endDate: new Date( element.endDate), allDay: element.allDay }));
    //}

    // ON LOAD EVENTS
    DevExpress.localization.locale('tr');

    $scope.loadSelectables = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Load/GetLoadCalendarList', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.mntData = resp.data.Load;
                        $scope.loadScheduler();
                        resolve();
                    }
                }).catch(function (err) { });
        });

        return prms;
    }
    $scope.loadSelectables().then(function () {

    });
    $scope.loadScheduler();
    $scope.$on('loadCalendar', function (e, d) {

    });
});