(() => {

    angular.module('app.controllers')
        .controller('accountProductsController', ['$scope', '$state', '$api', ($scope, $state, $api) => {

            $scope.$api = $api;
            $scope.isLoading = true;

            $scope.model = {
                accountId: $state.params.accountId,
                products: null
            };

            $api.account.getOwnerProducts({ accountId: $scope.model.accountId }).then((json) => {

                $scope.model.products = viewProductBuilder.build(json, $api);
            });

        }]);

    var viewProduct = function (json, $api) {

        Object.defineProperties(this, {
            id: { value: json.id, writable: false },
            name: { value: json.name, writable: false },
            unitName: { value: json.unitName, writable: false },
            plan: { value: json.plan, writable: false },
            allowed: { value: json.allowed, writable: false },
            purchaseDate: { value: json.purchaseDate, writable: false },
            status: { value: json.status, writable: false }
        });

        this.$api = $api;
    };
    viewProduct.prototype.getTableCssClass = function () {

        var $api = this.$api;

        if ($api.product.isDisabled(this))
            return 'table-danger';

        if ($api.product.isFree(this) || $api.product.isTrial(this))
            return 'table-primary';

        return '';
    };

    var viewProductBuilder = () => { };
    viewProductBuilder.build = (json, $api) => {

        const products = [];

        for (let index = 0; index < json.length; index++)
            products.push(new viewProduct(json[index], $api));

        return products;
    };
})();

