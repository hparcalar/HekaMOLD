app.controller('systemSettingsCtrl', function ($scope, $http) {
    DevExpress.localization.locale('tr');

    $scope.settingList = [];

    $scope.fetchSettings = function () {
        var prms = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'SystemSettings/GetSettingsList', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.settingList = resp.data;
                        resolve();
                    }
                }).catch(function (err) { });
        });

        return prms;
    }

    $scope.bindList = function () {
        $scope.fetchSettings().then(function () {
            $('#dataList').dxDataGrid({
                dataSource: {
                    load: function () {
                        return $scope.settingList;
                    },
                    update: function (key, values) {
                        var obj = $scope.settingList.find(d => d.Id == key);
                        if (obj != null) {
                            if (typeof values.PrmCode != 'undefined') { obj.PrmCode = values.PrmCode; }
                            if (typeof values.PrmValue != 'undefined') { obj.PrmValue = values.PrmValue; }
                        }
                    },
                    remove: function (key) {
                        
                    },
                    insert: function (values) {
                        
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
                    allowDeleting: false,
                    allowAdding: false,
                    mode: 'cell'
                },
                columns: [
                    { dataField: 'PrmCode', caption: 'Parametre', allowEditing:false, },
                    { dataField: 'PrmValue', caption: 'Değer' },
                ]
            });
        });
    }

    $scope.saveModel = function () {
        $scope.saveStatus = 1;

        $http.post(HOST_URL + 'SystemSettings/SaveSettings', $scope.settingList, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.saveStatus = 0;

                    if (resp.data.Status == 1) {
                        toastr.success('Kayıt başarılı.', 'Bilgilendirme');

                        $scope.bindList();
                    }
                    else
                        toastr.error(resp.data.ErrorMessage, 'Hata');
                }
            }).catch(function (err) { });
    }

    // ON LOAD EVENTS
    $scope.bindList();
});
