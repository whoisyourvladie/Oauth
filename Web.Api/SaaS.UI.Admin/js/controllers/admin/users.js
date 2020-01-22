(function () {
    'use strict';

    angular
        .module('app.controllers')
        .controller('adminUsersController', controller);

    controller.$inject = ['$scope', '$notify', '$api', '$form'];

    function controller($scope, $notify, $api, $form) {

        var viewEnum = {
            none: 0,
            addUser: 1,
            editUser: 2,
        };

        $scope.model = {
            users: [],
            pagination: {
                currentPage: 1,
                count: 15,
                maxSize: 4,
                users: [],
                limitFrom: function(){
                
                    return this.count;
                },
                limitTo: function () {

                    return this.count * (this.currentPage - 1);
                }
            },
            roles: null
        };

        $scope.status = null;
        $scope.isBusy = false;

        $scope.view = viewEnum.none;
        $scope.viewEnum = viewEnum;

        $scope.refresh = function () {

            $api.user.get().then(function (json) {

                $scope.model.users = json;
            });
        };
        $scope.addUser = function () {

            $scope.view = viewEnum.addUser;
            $scope.model.id = null;
            $scope.model.login = null;
            $scope.model.password = null;
            $scope.model.role = $scope.model.roles[0];
        };
        $scope.editUser = function (user) {

            $scope.view = viewEnum.editUser;
            $scope.model.id = user.id;
            $scope.model.login = user.login;
            $scope.model.password = null;
            $scope.model.role = user.role;
        };
        $scope.activateUser = function (user) {

            $api.user.activate(user.id).then(function (json) {

                user.isActive = json.isActive;
                $notify.info('User has been activated.');
            });
        };
        $scope.deactivateUser = function (user) {

            $api.user.deactivate(user.id).then(function (json) {

                user.isActive = json.isActive;
                $notify.info('User has been deactivated.');
            });
        };

        $scope.cancel = function () {

            $scope.view = viewEnum.none;
            $scope.refresh();
        };

        $scope.submit = function (form) {

            $form.submit($scope, form, function () {

                var handler = $scope.view == viewEnum.addUser ? $api.user.insert : $api.user.update;
                
                var model = {
                    id: $scope.model.id,
                    login: $scope.model.login,
                    password: $scope.model.password,
                    role: $scope.model.role
                };

                handler(model).then(function (json) {

                    $scope.view == viewEnum.addUser && $notify.info('User has been created.');
                    $scope.view == viewEnum.editUser && $notify.info('User has been changed.');

                    $scope.view = viewEnum.none;
                    $scope.refresh();

                }).finally(function () {

                    $scope.isBusy = false;
                });
            });
        };

        $scope.refresh();

        (function () {

            $scope.model.roles = ['agent', 'supervisior', 'manager', 'admin'];
        }());

        var buildPagination = function () {
            
            var begin = (($scope.model.pagination.currentPage - 1) * $scope.model.pagination.count);
            var end = begin + $scope.model.pagination.count;

            $scope.model.pagination.users = $scope.model.users.slice(begin, end);
        };

        $scope.$watch('model.users', buildPagination);
        $scope.$watch('model.pagination.currentPage + model.pagination.count', buildPagination);
    };
})();