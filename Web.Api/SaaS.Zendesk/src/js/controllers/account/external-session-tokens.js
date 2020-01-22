angular.module('app.controllers')
    .controller('accountExternalSessionTokensController', ['$scope', '$state', '$api', ($scope, $state, $api) => {

        $scope.isLoading = true;

        $scope.model = {
            accountId: $state.params.accountId,
            externalSessionTokens: null
        };

        $api.account.getExternalSessionTokens({ accountId: $scope.model.accountId }).then((json) => {

            $scope.model.externalSessionTokens = json;
        });

    }]);