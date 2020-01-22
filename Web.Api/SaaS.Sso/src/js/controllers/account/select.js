angular.module('app.controllers')
    .controller('accountSelectController', ['$scope', '$state', '$utils', '$storage',
        ($scope, $state, $utils, $storage) => {

            let accounts = $storage.getAccounts();

            if (!accounts.length)
                return $state.go('index');

            $scope.model = {
                accounts: accounts
            };

            $scope.select = (account) => {

                return $state.go('account/password', account);
            };

            $scope.getFirstChar = (account) => {

                let value = account.email ||
                    account.firstName ||
                    account.lastName ||
                    ' ';

                return value[0];
            };
            $scope.getColor = (account) => {

                return $utils.stringToColor(account.email);
            };
        }]);