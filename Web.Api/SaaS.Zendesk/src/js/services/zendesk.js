angular.module('app.services')
    .factory('$zendesk', [() => {

        var _zClient = ZAFClient.init();

        var service = {};
        service.init = function () {

            _zClient && _zClient.invoke('resize', { width: '100%', height: '400px' });
        };

        service.isEmpty = () => {

            return !_zClient;
        };

        service.on = (key, callback) => {

            return _zClient.on(key, callback);
        };
        service.get = (key) => {

            return _zClient.get(key);
        };

        return service;
    }]);