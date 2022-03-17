app.controller('reportTemplateListCtrl', function ($scope, $http) {
    $scope.templateList = [];
    $scope.selectedReportTypes = [];
    $scope.selectedTemplateId = 0;

    $scope.loadTemplateList = function () {
        try {
            var reportTypeStr = '';
            for (var i = 0; i < $scope.selectedReportTypes.length; i++) {
                reportTypeStr += $scope.selectedReportTypes[i] + ',';
            }

            if (reportTypeStr.length > 0)
                reportTypeStr = reportTypeStr.substr(0, reportTypeStr.length - 1);

            $http.get(HOST_URL + 'Common/GetReportTemplateList?reportTypes='
                + reportTypeStr, {}, 'json')
                .then(function (resp) {
                    if (typeof resp.data != 'undefined' && resp.data != null) {
                        $scope.templateList = resp.data;
                    }
                }).catch(function (err) { });
        } catch (e) {

        }
    }

    $scope.selectTemplate = function (templateId) {
        $scope.selectedTemplateId = templateId;
    }

    $scope.exportAs = function (exportType) {
        if ($scope.selectedTemplateId <= 0) {
            toastr.error('Önce listeden bir şablon seçmelisiniz.');
            return;
        }

        if (exportType == 1) // PDF
        {
            $scope.$emit('printTemplate', { templateId: $scope.selectedTemplateId, exportType: 'PDF' });
        }
        else if (exportType == 2) // PRINTER
        {
            $scope.$emit('printTemplate', { templateId: $scope.selectedTemplateId, exportType: 'PRINTER' });
        }
        else if (exportType == 3) // EXCEL
        {
            $scope.$emit('printTemplate', { templateId: $scope.selectedTemplateId, exportType: 'EXCEL' });
        }
    }

    // ON LOAD EVENTS
    $scope.$on('loadTemplateList', function (e, d) {
        $scope.selectedTemplateId = 0;

        $scope.selectedReportTypes = d;
        $scope.loadTemplateList();
    });
});