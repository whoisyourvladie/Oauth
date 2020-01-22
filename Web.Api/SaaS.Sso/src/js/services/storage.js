angular
    .module('app.services')
    .factory('$storage', ['localStorageService', ($localStorageService) => {

        let service = {};

        let _equalAccount = (account1, account2) => {

            let email1 = account1.email || '';
            let email2 = account2.email || '';

            return email1.toLowerCase() === email2.toLowerCase();
        };

        service.getAccounts = () => {

            if (!$localStorageService.isSupported)
                return [];

            return $localStorageService.get('accounts') || [];
        };
        service.addAccount = (account) => {

            if (!$localStorageService.isSupported)
                return;

            service.removeAccount(account);

            let accounts = service.getAccounts();

            accounts.unshift({
                firstName: account.firstName,
                lastName: account.lastName,
                email: account.email
            });

            $localStorageService.set('accounts', accounts);
        };
        service.removeAccount = (account) => {

            if (!$localStorageService.isSupported)
                return;

            let accounts = service.getAccounts();
            for (let index = 0; index < accounts.length; ++index) {

                if (_equalAccount(accounts[index], account))
                    accounts.splice(index--, 1);
            };

            $localStorageService.set('accounts', accounts);
        };

        function viewAccount(json) {

            this.firstName = json.firstName;
            this.lastName = json.lastName;
            this.email = json.email;
        };

        return service;
    }]);