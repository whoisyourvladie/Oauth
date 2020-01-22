(function () {
    'use strict';

    angular
        .module('app.controllers')
        .controller('adminEmailsController', controller);

    controller.$inject = ['$scope', '$timeout', '$api', '$form', '$auth', '$http', '$authStorage'];

    function controller($scope, $timeout, $api, $form, $auth, $http, $authStorage) {

        var _langs = [
            { name: 'all', id:'all' },
            { name: 'en', id:'en' },
            { name: 'fr', id:'fr' }
        ];
        var _emailTemplates = [
            { name: 'loading templates ...', id:"notemplates" }
            //{ name: 'All', id: null }
        ];

        $scope.model = {
            emailTo: 'nfotenyuk@lulusoftware.com',
            emailsTemplates: _emailTemplates
        };

        $timeout(function () {
            console.log($authStorage.accessToken());
        }, 0);
    };
})();