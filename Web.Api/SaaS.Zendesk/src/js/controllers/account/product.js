(() => {

    angular.module('app.controllers')
        .controller('accountProductController', ['$scope', '$state', '$api', ($scope, $state, $api) => {

            $scope.$api = $api;
            $scope.isLoading = true;

            $scope.model = {
                accountId: $state.params.accountId,
                accountProductId: $state.params.accountProductId,
                product: null
            };

            let query = { accountId: $scope.model.accountId, accountProductId: $scope.model.accountProductId }
            $api.account.getOwnerProductDetails(query).then((json) => {

                $scope.model.product = new viewProduct(json);
            });
        }]);

    let viewProduct = function (json) {

        Object.defineProperties(this, {
            id: { value: json.id, writable: false },
            name: { value: json.name, writable: false },
            unitName: { value: json.unitName, writable: false },
            plan: { value: json.plan, writable: false },
            ownerEmail: { value: json.ownerEmail, writable: false },
            allowed: { value: json.allowed, writable: false }
        });

        this.endDate = json.endDate;
        this.purchaseDate = json.purchaseDate;
        this.status = json.status;
        this.accounts = viewAcountBuilder.build(json);
    };

    let viewAccount = function (json) {

        Object.defineProperties(this, {
            accountId: { value: json.accountId, writable: false },
            email: { value: json.email, writable: false }
        });
    };

    let viewAcountBuilder = () => { };
    viewAcountBuilder.build = (json) => {

        let accounts = [];

        if (json.accounts) {

            for (let index = 0; index < json.accounts.length; index++) {

                let account = json.accounts[index];
                accounts.push(new viewAccount(account));
            }
        }

        return accounts;
    };
})();

