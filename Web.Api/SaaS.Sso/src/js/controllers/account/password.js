angular.module('app.controllers')
    .controller('accountPasswordController', ['$scope', '$state', '$utils', '$api', '$sso', '$form',
        ($scope, $state, $utils, $api, $sso, $form) => {

            var params = $state.params;
            if (!params.email)
                $state.go('index');

            $scope.model = {
                firstName: params.firstName,
                lastName: params.lastName,
                email: params.email
            };

            $scope.status = null;
            $scope.isBusy = false;

            $scope.getFirstChar = () => {

                let value = $scope.model.email ||
                    $scope.model.firstName ||
                    $scope.model.lastName ||
                    ' ';

                return value[0];
            };
            $scope.getColor = (account) => {

                return $utils.stringToColor($scope.model.email);
            };

            $scope.submit = (form) => {

                $scope.status = null;
                $scope.model.error = null;

                $form.submit($scope, form, () => {

                    return $api.tokenJwt({ email: $scope.model.email, password: $scope.model.password })
                        .then($sso.signInJwt,
                            (json) => {

                                if (json.status !== 404)
                                    return $state.go('account/error');

                                $scope.status = 404;
                            })
                        .finally(() => { $scope.isBusy = false; });
                });
            };
        }]);