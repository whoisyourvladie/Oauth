angular.module('app.controllers')
    .controller('accountController', ['$rootScope', '$q', '$scope', '$state', '$api', '$zendesk',
        ($rootScope, $q, $scope, $state, $api, $zendesk) => {

            $scope.model = {
                accountId: null,
                account: null
            };

            var _getAccount = () => {

                var params = $state.params;
                var deferred = $q.defer();
                if (params.accountId) {

                    $api.account.get({ accountId: params.accountId })
                        .then(
                            deferred.resolve,
                            () => { deferred.reject({ state: 'account/not-found', params: { accountId: params.accountId } }); });
                }
                else {

                    $zendesk.get(['ticket.requester'])
                        .then(
                            (response) => {

                                var requester = response['ticket.requester'];
                                if (!requester.email)
                                    deferred.reject({ state: 'account/email-is-empty' });
                                else {

                                    $api.account.get({ email: requester.email })
                                        .then(
                                            deferred.resolve,
                                            () => { deferred.reject({ state: 'account/not-found', params: { email: requester.email } }); });
                                }
                            },
                            () => { deferred.reject({ state: 'ticket/requester-email-is-empty', }); });

                }

                return deferred.promise;
            };


            $rootScope.$broadcast('event:spinner-show');

            _getAccount()
                .then((json) => {
                    $scope.model.account = json;
                    $scope.model.accountId = json.id;
                },
                    (error) => { $state.go(error.state, error.params); })
                .finally(() => { $rootScope.$broadcast('event:spinner-hide'); });
        }]);