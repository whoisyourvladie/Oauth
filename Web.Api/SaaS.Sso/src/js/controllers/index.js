angular.module('app.controllers')
    .controller('indexController', ['$state', '$storage', ($state, $storage) => {

        let accounts = $storage.getAccounts();

        if (accounts.length > 1)
            return $state.go('account/select');

        return $state.go('account/identifier', {
            email: accounts.length ? accounts[0].email : null
        });
    }]);