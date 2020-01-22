(function () {
    'use strict';

    angular
        .module('app.services')
        .factory('$query', query);

    query.$inject = [];

    function query() {

        var service = {};

        service.getJson = function (str) {

            try {

                str = str || document.location.search;

                return str.replace(/(^\?)/, '').split("&").map(function (item) {

                    var index = item.indexOf('=');
                    if (index != -1) {

                        var key = item.substring(0, index);
                        var value = item.substr(index + 1);
                        this[key.toLowerCase()] = decodeURIComponent(value);
                    }

                    return this;
                }.bind({}))[0];

            } catch (e) {
                return {};
            }
        };
        service.getQuery = function (str) {

            str = str || document.location.search;

            return str.slice(str.indexOf('?') + 1);
        };

        service.getHash = function (removeHash) {

            var hash = window.location.hash;

            if (!hash || !hash.startsWith('#'))
                return null;

            if (hash.indexOf("#/") != -1)
                hash = hash.replace('#/', '#');

            if (removeHash === true)
                hash = hash.replace('#', '');

            return hash.toLowerCase();
        };

        return service;
    };
})();