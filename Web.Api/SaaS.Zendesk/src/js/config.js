angular.module('app')
    .config(['$httpProvider', function ($httpProvider) {

        $httpProvider.interceptors.push([() => {

            var interceptor = {};

            interceptor.response = function (response) {

                var config = response.config || {};

                if (config.asJson === true)
                    return response.data;

                return response;
            };

            return interceptor;
        }]);
    }]);