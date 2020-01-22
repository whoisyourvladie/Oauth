angular.module('app.controllers')
    .controller('accountSubEmailsController', ['$scope', '$state', '$api', ($scope, $state, $api) => {

        $scope.isLoading = true;

        $scope.model = {
            accountId: $state.params.accountId,
            subEmails: null
        };

        $scope.refresh = () => {

            $api.account.getSubEmails({ accountId: $scope.model.accountId })
                .then((json) => {

                    $scope.model.subEmails = json;
                });
        };

        $scope.remove = (json) => {

            $api.account.removeSubEmail(json)
                .finally($scope.refresh);
        };

    }]);