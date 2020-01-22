(function () {
    'use strict';

    angular
        .module('app.controllers')
        .controller('accountSearchController', controller);

    controller.$inject = ['$rootScope', '$scope', '$notify', '$api', '$form'];

    function controller($rootScope, $scope, $notify, $api, $form) {

        var _search = { filter: null, globalOrderId: null };

        $scope.model = {
            accounts: null,
            search: { filter: null, globalOrderId: null }
        };

        $scope.status = null;
        $scope.isBusy = false;

        $scope.refresh = function () {

            if (!_search.filter && !_search.globalOrderId) {
                $scope.model.accounts = [];
                return;
            }

            $scope.isBusy = true;
            $api.account.get(_search).then(function (json) {

                $scope.model.accounts = json;

            }).finally(function () { $scope.isBusy = false; });
        };
        $scope.merge = function (account) {

            $rootScope.$broadcast('event:accountMerge', account);
        };

        $scope.submit = function (form) {

            $form.submit($scope, form, function () {

                _search.filter = $scope.model.search.filter;
                _search.globalOrderId = $scope.model.search.globalOrderId;

                $scope.refresh();
            });
        };

       $rootScope.$on('event:accountMergeComplete', function (event, json) {

            $scope.model.accounts = [];
            $scope.refresh();
        });
    };
})();