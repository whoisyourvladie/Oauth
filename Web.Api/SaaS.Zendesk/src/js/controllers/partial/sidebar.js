angular.module('app.controllers')
    .controller('partialSidebarController', ['$rootScope', '$scope', '$auth', '$state', '$api', '$form',
        ($rootScope, $scope, $auth, $state, $api, $form) => {

            $scope.$auth = $auth

            var _search = { filter: null };

            $scope.model = {
                search: { filter: null }
            };

            $scope.status = null;
            $scope.isBusy = false;

            $scope.search = (form) => {

                $form.submit($scope, form, () => {

                    $scope.status = null;

                    _search.filter = $scope.model.search.filter;

                    if (!_search.filter)
                        return;

                    var query = {};
                    if (_search.filter.indexOf('@') !== -1)
                        query.email = _search.filter;
                    else
                        query.transactionOrderUid = _search.filter;

                    $scope.isBusy = true;
                    $api.account.get(query)
                        .then(
                            (json) => {

                                $rootScope.$broadcast('event:closeSidebar');
                                return $state.go('account', { accountId: json.id });
                            },
                            (error) => { $scope.status = error.status; })
                        .finally(() => { $scope.isBusy = false; });
                });
            };
            $scope.signIn = () => {

                $state.go('user/sign-in');
                $rootScope.$broadcast('event:closeSidebar');
            };
            $scope.logout = () => {

                $auth.logout();
                $rootScope.$broadcast('event:closeSidebar');
            };
        }]);