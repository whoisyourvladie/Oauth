angular.module('app.controllers')
    .controller('userSignInController', ['$scope', '$brand', '$state', '$auth', '$form', '$zendesk',
        ($scope, $brand, $state, $auth, $form, $zendesk) => {

            $scope.model = {
                logo: $brand.getLogo(),
                avatar: null,
                error: null
            };

            $scope.status = null;
            $scope.isBusy = false;

            !$zendesk.isEmpty() && $zendesk.get('currentUser').then((response) => {

                $scope.model.login = response.currentUser.email;
                $scope.model.avatar = response.currentUser.avatarUrl;
                $scope.$apply();
            });

            $scope.submit = (form) => {

                $scope.model.error = null;
                $form.submit($scope, form, () => {

                    return $auth.signIn($scope.model.login, $scope.model.password)
                        .then(() => {
                            $state.go('account');
                        }, (json) => {

                            $scope.model.error = json.error_description;
                        })
                        .finally(() => { $scope.isBusy = false });
                });
            };
        }]);