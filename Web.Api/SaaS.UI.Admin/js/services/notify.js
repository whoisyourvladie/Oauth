(function () {
    'use strict';

    angular
        .module('app.services')
        .factory('$notify', factory);

    factory.$inject = ['$rootScope'];

    function factory($rootScope) {

        var service = {};

        var _notify = function (type, options) {

            options.type = type;
            options.timeout = 2 * 1000;

            $rootScope.$emit('notify', options);
            angular.element('[notifybar]').css('position', 'fixed')
        };

        service.info = function (content) {

            _notify('info', { title: 'Information', content: content });
        };
        service.warning = function (content) {

            _notify('warning', { title: 'Warning', content: content });
        };
        service.success = function (content) {

            _notify('success', { title: 'Success', content: content });
        };
        service.error = function (content) {

            _notify('error', { title: 'Error', content: content });
        };
        service.serverError = function () {

            _notify('error', { title: 'Error', content: 'An error has occured. Please try again or contact us.' });
            
        };

        return service;
    };
})();