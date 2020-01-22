angular.module('app.controllers')
    .controller('accountRemoveController', ['$scope', '$state', '$utils', '$storage',
        ($scope, $state, $utils, $storage) => {

            let accounts = $storage.getAccounts();

            if (!accounts.length)
                return $state.go('index');

            $scope.model = {
                accounts: accounts
            };

            $scope.remove = (account) => {

                $storage.removeAccount(account);
                $scope.model.accounts = $storage.getAccounts();
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