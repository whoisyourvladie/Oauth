angular.module('app.controllers')
    .controller('brandNotSupportedController', ['$scope', '$brand', ($scope, $brand) => {

        $scope.model = {
            logo: $brand.get().logo.contentUrl
        };
    }]);