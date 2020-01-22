(function () {
    'use strict';

    angular
        .module('app.controllers')
        .controller('adminLogsController', controller);

    controller.$inject = ['$scope', '$api', '$form'];

    function controller($scope, $api, $form) {

        var _from = new Date();
        var _to = new Date();

        var _users = [{ login: 'All', id: null }];
        var _logActionTypes = [{ name: 'All', id: null }];
        var _search = { userId: null, log: null, from: _from, to: _to };

        $scope.model = {
            logs: null,
            users: _users,
            logActionTypes: _logActionTypes,
            search: {
                user: _users[0],
                logActionType: _logActionTypes[0],
                from: _search.from,
                to: _search.to,
                log: _search.log
            }
        };

        $scope.status = null;
        $scope.isBusy = false;

        $scope.refresh = function () {

            $scope.isBusy = true;

            _search.from.setHours(0, 0, 0, 0);
            _search.to.setHours(23, 59, 59, 59);

            _search.from = _search.from.toUTCString();
            _search.to = _search.to.toUTCString();

            $api.log.get(_search).then(function (json) {

                $scope.model.logs = json;

            }).finally(function () {

                $scope.isBusy = false;
            });
        };
        $scope.csv = function () {

            if ($scope.isBusy)
                return;

            $api.log.csv(_search);
        };
        $scope.submit = function (form) {

            $form.submit($scope, form, function () {

                _search.userId = $scope.model.search.user.id;
                _search.logActionTypeId = $scope.model.search.logActionType.id;
                _search.log = $scope.model.search.log;

                if ($scope.model.search.from > $scope.model.search.to) {

                    _search.from = $scope.model.search.to;
                    _search.to = $scope.model.search.from;
                }
                else {

                    _search.from = $scope.model.search.from;
                    _search.to = $scope.model.search.to;
                }

                $scope.refresh();
            });
        };

        $api.user.get().then(function (json) {

            $scope.model.users = $scope.model.users.concat(json);
        });
        $api.log.logActionType.get().then(function (json) {

            $scope.model.logActionTypes = $scope.model.logActionTypes.concat(json);
        });
        $scope.refresh();
    };
})();