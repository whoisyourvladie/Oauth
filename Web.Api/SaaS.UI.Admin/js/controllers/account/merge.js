(function () {
    'use strict';

    angular
        .module('app.controllers')
        .controller('accountMergeController', controller);

    controller.$inject = ['$rootScope', '$scope', '$timeout', '$notify', '$api', '$form'];

    function controller($rootScope, $scope, $timeout, $notify, $api, $form) {

        $scope.model = { accounts: null, openCustomerInfoAfterMerge: true };

        $scope.status = null;
        $scope.isBusy = false;

        var _checkSelectedItem = function () {

            var accounts = $scope.model.accounts;
            if (!accounts.length)
                return;

            var activeAccounts = accounts.filter(function (item) { return item.isActive; });
            if (activeAccounts.length != 1)
                $scope.select(0);

            var activeEmailAccounts = accounts.filter(function (item) { return item.isActiveEmail; });
            if (activeEmailAccounts.length != 1)
                $scope.selectEmail(0);
        };

        $scope.add = function (account) {

            $scope.isVisible = true;
            $scope.model.accounts = $scope.model.accounts || [];

            var accounts = $scope.model.accounts.filter(function (item) { return item.id == account.id });
            if (!accounts.length)
                $scope.model.accounts.push(account);

            while ($scope.model.accounts.length > 2)
                $scope.remove(0);

            _checkSelectedItem();
        };
        $scope.remove = function (index) {

            var accounts = $scope.model.accounts;

            accounts.splice(index, 1);

            _checkSelectedItem();

            if (!accounts.length)
                $scope.close();
        };
        $scope.select = function (selectedIndex) {

            var accounts = $scope.model.accounts;

            for (var index = 0; index < accounts.length; index++)
                $scope.model.accounts[index].isActive = index == selectedIndex;
        };
        $scope.selectEmail = function (selectedIndex) {

            var accounts = $scope.model.accounts;

            for (var index = 0; index < accounts.length; index++)
                $scope.model.accounts[index].isActiveEmail = index == selectedIndex;
        };

        $scope.cancel = function () {

            $scope.close();
        };

        $scope.submit = function (form) {

            $form.submit($scope, form, function (form) {

                var accounts = $scope.model.accounts;
                var activeAccounts = accounts.filter(function (item) { return item.isActive; });
                var activeEmailAccounts = accounts.filter(function (item) { return item.isActiveEmail; });
                var notActiveAccounts = accounts.filter(function (item) { return !item.isActive; });

                $api.account.merge(activeAccounts[0].id, notActiveAccounts[0].id, activeEmailAccounts[0].id)
                .then(function (json) {

                    $notify.info("Customer's has been merged.");
                    if ($scope.model.openCustomerInfoAfterMerge) {

                        var url = '/account/?id=' + activeAccounts[0].id;
                        $timeout(function () {

                            window.open(url, '_self', '');
                        }, 1500);

                        return;
                    }

                    $rootScope.$broadcast('event:accountMergeComplete', {});

                }).finally(function () { $scope.isBusy = false; });
            });
        };
        $scope.close = function () {

            $scope.isVisible = false;
            $scope.model.accounts = null;
        };
        $rootScope.$on('event:accountMerge', function (event, json) {

            !$scope.isBusy && $scope.add(json);
        });
        $rootScope.$on('event:accountMergeComplete', function (event, json) {

            $scope.close();
        });
    };
})();