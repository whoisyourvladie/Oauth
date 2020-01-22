(function () {
    'use strict';

    angular
        .module('app.auth')
        .factory('$authInterceptor', authInterceptor)
        .config(['$httpProvider', function ($httpProvider) {

            $httpProvider.interceptors.push(['$rootScope', '$q', '$authBuffer', function ($rootScope, $q, $authBuffer) {

                var interceptor = {};

                interceptor.request = function (config) {
                    
                    config.headers = config.headers || {};

                    if (!config.disableOauth)
                    {
                        var accessToken = $.cookie('access_token');
                        if (accessToken)
                            config.headers.Authorization = 'Bearer ' + accessToken;
                    }

                    return config;
                };
                interceptor.responseError = function (rejection) {

                    var config = rejection.config || {};
                    if (!config.ignoreAuthModule) {

                        switch (rejection.status) {

                            case 401:
                                var deferred = $q.defer();
                                $authBuffer.append(config, deferred);
                                $rootScope.$broadcast('event:auth-loginRequired', rejection);
                                return deferred.promise;

                            case 403:
                                $rootScope.$broadcast('event:auth-forbidden', rejection);
                                break;
                        }
                    }
                    // otherwise, default behaviour
                    return $q.reject(rejection);
                }

                return interceptor;
            }]);
        }])

    authInterceptor.$inject = ['$rootScope', '$authBuffer'];

    function authInterceptor($rootScope, $authBuffer) {

        var service = {};

        /**
        * Call this function to indicate that authentication was successfull and trigger a
        * retry of all deferred requests.
        * @param data an optional argument to pass on to $broadcast which may be useful for
        * example if you need to pass through details of the user that was logged in
        * @param configUpdater an optional transformation function that can modify the
        * requests that are retried after having logged in.  This can be used for example
        * to add an authentication token.  It must return the request.
        */
        service.loginConfirmed = function (data, configUpdater) {

            var updater = configUpdater || function (config) {
                return config;
            };

            $rootScope.$broadcast('event:auth-loginConfirmed', data);
            $authBuffer.retryAll(updater);
        };

        /**
         * Call this function to indicate that authentication should not proceed.
         * All deferred requests will be abandoned or rejected (if reason is provided).
         * @param data an optional argument to pass on to $broadcast.
         * @param reason if provided, the requests are rejected; abandoned otherwise.
         */
        service.loginCancelled = function (data, reason) {

            $authBuffer.rejectAll(reason);
            $rootScope.$broadcast('event:auth-loginCancelled', data);
        }

        return service;
    };

})();