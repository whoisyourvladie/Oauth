(function () {
    'use strict';

    angular
        .module('app.controllers')
        .controller('adminRolesController', controller);

    controller.$inject = ['$scope'];

    var roleEnum = {
        none: 0,

        addUser: 1 << 0,
        editUser: 1 << 1,
        deleteUser: 1 << 2,

        addAccount: 1 << 3,
        editAccount: 1 << 4,
        deleteAccount: 1 << 5,

        addProduct: 1 << 6,
        editProduct: 1 << 7,
        deleteProduct: 1 << 8
    };

    function controller($scope) {

        $scope.model = {
            roles: [
                { title: 'Add/Edit an account', description: 'Create or modify customer accounts', status: roleEnum.addAccount | roleEnum.editAccount },
                { title: 'Delete an account', description: 'Delete only accounts that have no purchases accossiated to them', status: roleEnum.deleteAccount },

                { title: 'Add a product', description: 'Add products to an existing account', status: roleEnum.addProduct },
                { title: 'Edit a product', description: 'modify products and billing within an account', status: roleEnum.editProduct },

                { title: 'Add/Edit/Delete an user', description: 'create a with permission to allow use of the Admin portal', status: roleEnum.addUser | roleEnum.editUser | roleEnum.deleteUser }
            ],
            groups: [
                { title: 'Agent', status: roleEnum.addAccount | roleEnum.editAccount | roleEnum.deleteAccount | roleEnum.editProduct },
                { title: 'Supervisior', status: roleEnum.addAccount | roleEnum.editAccount | roleEnum.deleteAccount | roleEnum.editProduct },
                { title: 'Manager', status: roleEnum.addAccount | roleEnum.editAccount | roleEnum.deleteAccount | roleEnum.editProduct | roleEnum.addProduct },
                { title: 'Admin', status: roleEnum.addAccount | roleEnum.editAccount | roleEnum.deleteAccount | roleEnum.editProduct | roleEnum.addProduct | roleEnum.addUser | roleEnum.editUser | roleEnum.deleteUser }
            ]
        };

        $scope.isStatus = function (group, role) {

            return !!(group.status & role.status);
        };
    };
})();
