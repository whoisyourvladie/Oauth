(function () {
    'use strict';

    angular
        .module('app.auth')
        .factory('$auth', auth);

    auth.$inject = ['$q', '$http', '$authStorage', '$notify', 'appSettings'];

    function auth($q, $http, $authStorage, $notify, appSettings) {

        var _apiHeaders = { 'Content-Type': 'application/x-www-form-urlencoded' };
        var _uri = function (method) {

            return appSettings.oauth.path + method;
        };

        var service = {};

        service.identity = appSettings.user.identity;

        service.signIn = function (login, password) {

            var data = [
                'grant_type=password',
                'username=' + encodeURIComponent(login),
                'password=' + encodeURIComponent(password)
            ];

            data = data.join('&');

            var deferred = $q.defer();
            
            $http.post(_uri('api/token'), data, { headers: _apiHeaders }).then(function (response) {

                $authStorage.signIn(response.data);
                deferred.resolve(response.data);

            }, function (response) {
                
                var error = response.data ? response.data.error_description : '';

                error ? $notify.error(error) : $notify.serverError();

                deferred.reject(response.status);
            });

            return deferred.promise;
        };
        service.refreshToken = function () {

            var data = [
                'grant_type=refresh_token',
                'refresh_token=' + $authStorage.refreshToken()
            ];

            data = data.join('&');

            var deferred = $q.defer();

            $http.post(_uri('api/token'), data, { headers: _apiHeaders }).then(function (response) {
                
                $authStorage.signIn(response.data);
                deferred.resolve(response.data);
            }, 
            function (error, status) {

                deferred.reject();
            });

            return deferred.promise;
        };

        service.logout = function (refreshToken) {

            if (refreshToken)
                return $http.delete(_uri('api/token/' + refreshToken), {});

            return $http.delete(_uri('api/token/logout')).finally(function () {

                $authStorage.logout()
                window.location = '/user/login';
            });
        };

        return service;
    };

})();