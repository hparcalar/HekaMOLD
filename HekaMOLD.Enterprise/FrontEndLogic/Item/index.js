app.controller('itemCtrl', function ($scope, $http) {
    $scope.modelObject = {};

    $scope.saveStatus = 0;

    $scope.selectedItemType = {};
    $scope.itemTypeList = [{ Id: 1, Text: 'Hammadde' }, { Id: 2, Text: 'Ticari Mal' },
        { Id: 3, Text: 'Yarı Mamul' }, { Id: 3, Text: 'Mamul' }];

    $scope.selectedCategory = {};
    $scope.categoryList = [];

    $scope.selectedGroup = {};
    $scope.groupList = [];

    // GET SELECTABLE DATA
    $scope.loadSelectables = function () {
        var prmReq = new Promise(function (resolve, reject) {
            $http.get(HOST_URL + 'Item/GetSelectables', {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.categoryList = resp.data.Categories;
                        $scope.groupList = resp.data.Groups;

                        resolve(resp.data);
                    }
                }).catch(function (err) { });
        });

        return prmReq;
    }

    // CRUD
    $scope.openNewRecord = function () {
        $scope.modelObject = { Id: 0 };
        $scope.selectedFirmType = {};
    }

    $scope.performDelete = function () {
        bootbox.confirm({
            message: "Bu stok tanımını silmek istediğinizden emin misiniz?",
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
                    $http.post(HOST_URL + 'Item/DeleteModel', { rid: $scope.modelObject.Id }, 'json')
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

        if (typeof $scope.selectedItemType != 'undefined' && $scope.selectedItemType != null)
            $scope.modelObject.ItemType = $scope.selectedItemType.Id;
        else
            $scope.modelObject.ItemType = null;

        if (typeof $scope.selectedCategory != 'undefined' && $scope.selectedCategory != null)
            $scope.modelObject.ItemCategoryId = $scope.selectedCategory.Id;
        else
            $scope.modelObject.ItemCategoryId = null;

        if (typeof $scope.selectedGroup != 'undefined' && $scope.selectedGroup != null)
            $scope.modelObject.ItemGroupId = $scope.selectedGroup.Id;
        else
            $scope.modelObject.ItemGroupId = null;

        $http.post(HOST_URL + 'Item/SaveModel', $scope.modelObject, 'json')
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
        $http.get(HOST_URL + 'Item/BindModel?rid=' + id, {}, 'json')
            .then(function (resp) {
                if (typeof resp.data != 'undefined' && resp.data != null) {
                    $scope.modelObject = resp.data;

                    // BIND EXTERNAL TYPES
                    if ($scope.modelObject.ItemType > 0)
                        $scope.selectedItemType = $scope.itemTypeList.find(d => d.Id == $scope.modelObject.ItemType);
                    else
                        $scope.selectedItemType = {};

                    if ($scope.modelObject.ItemCategoryId > 0)
                        $scope.selectedCategory = $scope.categoryList.find(d => d.Id == $scope.modelObject.ItemCategoryId);
                    else
                        $scope.selectedCategory = {};

                    if ($scope.modelObject.ItemGroupId > 0)
                        $scope.selectedGroup = $scope.groupList.find(d => d.Id == $scope.modelObject.ItemGroupId);
                    else
                        $scope.selectedGroup = {};
                }
            }).catch(function (err) { });
    }

    // ON LOAD EVENTS
    $scope.loadSelectables().then(function (data) {
        if (PRM_ID > 0)
            $scope.bindModel(PRM_ID);
    });
});