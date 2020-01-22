angular.module('app.controllers')
    .controller('accountIdentifierController', ['$scope', '$state', '$api', '$storage', '$form', '$brand',
        ($scope, $state, $api, $storage, $form, $brand) => {

            $scope.model = {
                email: $state.params.email || null
            };

            $scope.status = null;
            $scope.isBusy = false;

            $scope.subTitle = `with your ${$brand.currentName()} account`;
            $scope.errorNotFound = `Couldn't find your ${$brand.currentName()} account`;

            $scope.submit = (form) => {

                $scope.status = null;
                $scope.model.error = null;

                $form.submit($scope, form, () => {

                    return $api.account.get({ email: $scope.model.email })
                        .then((json) => {

                            $storage.addAccount(json);
                            $state.go('account/password', json);
                        },
                            (json) => {

                                if (json.status !== 404)
                                    return $state.go('account/error');

                                $scope.status = 404;
                            })
                        .finally(() => { $scope.isBusy = false; });
                });
            };
        }]);