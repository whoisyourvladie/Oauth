/// <reference path="bundle.js" />
if (!String.prototype.startsWith) {
    String.prototype.startsWith = function (searchString, position) {
        position = position || 0;
        return this.indexOf(searchString, position) === position;
    };
}


!function () {

    'use strict';

    angular.module('app.auth', []);
    angular.module('app.services', ['angularNotify']);
    angular.module('app.controllers', []);
    angular.module('app.directives', []);
    angular.module('app.filters', []);

    angular.module('app', ['angularNotify', 'ui.bootstrap', 'app.auth', 'app.services', 'app.controllers', 'app.directives', 'app.filters'])
        .config(['$httpProvider', '$locationProvider', function ($httpProvider, $locationProvider) {

            $locationProvider.html5Mode({
                enabled: true,
                requireBase: false,
                rewriteLinks: false
            });
            var regexIso8601 = /(\d{4})-(\d{2})-(\d{2})T(\d{2})\:(\d{2})\:(\d{2})Z/;

            var convertDateStringsToDates = function (input) {

                if (typeof input !== 'object')
                    return input;

                for (var key in input) {

                    if (!input.hasOwnProperty(key)) continue;

                    var value = input[key];
                    var match;
                    if (typeof value === "string" && (match = value.match(regexIso8601))) {

                        var milliseconds = Date.parse(match[0])
                        if (!isNaN(milliseconds))
                            input[key] = new Date(milliseconds);
                    }
                    else
                        if (typeof value === 'object')
                            convertDateStringsToDates(value);
                }
            }
            $httpProvider.interceptors.push(['$q', function ($q) {

                return {

                    request: function (config) {

                        config.headers['Accept-Language'] = 'en-US';

                        return config || $q.when(config);
                    }
                }
            }]);

            $httpProvider.defaults.transformResponse.push(function (responseData) {

                convertDateStringsToDates(responseData);
                return responseData;
            });
        }])
        .run(['$rootScope', '$authInterceptor', '$auth', function ($rootScope, $authInterceptor, $auth) {

            $rootScope.$on('event:auth-loginRequired', function () {
                
                return $auth.refreshToken().then($authInterceptor.loginConfirmed, $auth.logout);
            });
        }])

    
}();