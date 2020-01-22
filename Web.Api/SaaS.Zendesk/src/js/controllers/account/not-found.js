angular.module('app.controllers')
    .controller('accountNotFoundController', ['$scope', '$state', '$brand', ($scope, $state, $brand) => {

        $scope.model = {
            email: $state.params.email,
            logo: $brand.get().logo.contentUrl
        };
    }]);