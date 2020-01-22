(function () {
    'use strict';

    angular
        .module('app.auth')
        .factory('$authStorage', authStorage);

    authStorage.$inject = ['appSettings'];

    function authStorage(appSettings) {

        var _accessToken = 'access_token';
        var _refreshToken = 'refresh_token';
        var _fullName = 'oauth_fullName';

        var _getSet = function (key, value) {

            if (typeof value == 'undefined')
                return $.cookie(key);

            $.cookie.raw = false;
            $.cookie(key, value, { expires: 1, path: '/' });
        };
        var _initIdentity = function () {

            var identity = appSettings.user.identity;

            identity.fullName = _getSet(_fullName) || identity.name;

            return identity;
        };

        var service = {};

        service.accessToken = function (value) {

            return _getSet(_accessToken, value);
        };
        service.refreshToken = function (value) {

            return _getSet(_refreshToken, value);
        };
        service.name = function (firstName, lastName) {

            var identity = _initIdentity();
            identity.fullName = (firstName || '') + ' ' + (lastName || '');

            _getSet(_fullName, identity.fullName);
        };

        service.signIn = function (json) {

            var fullName = (json.firstName || json.lastName) ? 
                (json.firstName || '') + ' ' + (json.lastName || '') : json.email;

            _getSet(_accessToken, json.access_token);
            _getSet(_refreshToken, json.refresh_token);
            _getSet(_fullName, fullName);

            var identity = _initIdentity();

            identity.name = json.email;
            identity.isAuthenticated = true;
        };
        service.logout = function () {

            $.cookie(_accessToken, null, { expires: -1, path: '/' });
            $.cookie(_refreshToken, null, { expires: -1, path: '/' });
            $.cookie(_fullName, null, { expires: -1, path: '/' });
        };


        _initIdentity();

        return service;
    };

})();