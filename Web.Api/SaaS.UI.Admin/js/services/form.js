(function () {
    'use strict';

    angular
        .module('app.services')
        .factory('$form', form);

    form.$inject = [];

    function form() {

        var service = {};

        service.submit = function (entity, form, callback) {

            if (form.$valid !== true) {

                angular.forEach(form, function (value, key) {

                    if (typeof value === 'object' && value.hasOwnProperty('$modelValue'))
                        value.$setDirty();
                });
            }
            if (service.isReady(entity, form) === false)
                return;

            callback(form);
        };
        service.isReady = function (entity, form) {

            if (entity.isBusy === true || form.$valid !== true)
                return false;

            entity.isBusy = true;

            return true;
        };

        return service;
    };
})();