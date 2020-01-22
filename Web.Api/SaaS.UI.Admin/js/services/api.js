(function () {
    'use strict';

    angular
        .module('app.services')
        .factory('$api', factory);

    factory.$inject = ['$q', '$http', '$notify', 'appSettings'];

    function factory($q, $http, $notify, appSettings) {

        var _apiHeaders = { 'Content-Type': 'application/x-www-form-urlencoded' };
        var _uri = function (method) {

            return appSettings.oauth.path + method;
        };
        var _get = function (url, json) {

            json = json || {};

            var deferred = $q.defer();

            $http.get(_uri(url + '?' + $.param(json))).then(function (response) {

                deferred.resolve(response.data);
            }, function (response) {

                $notify.serverError();
                deferred.reject(response.status);
            });

            return deferred.promise;
        };
        var _put = function (url, json) {

            json = json || {};

            var deferred = $q.defer();

            $http.put(_uri(url), json).then(function (response) {

                deferred.resolve(response.data);

            }, function (response) {

                var error = response.data ? response.data.error_description : '';

                error ? $notify.error(error) : $notify.serverError();

                deferred.reject(response.status);
            });

            return deferred.promise;
        };
        var _post = function (url, json) {

            json = json || {};

            var deferred = $q.defer();

            $http.post(_uri(url), json).then(function (response) {

                deferred.resolve(response.data);

            }, function (response) {

                var error = response.data ? response.data.error_description : '';

                error ? $notify.error(error) : $notify.serverError();

                deferred.reject(response.status);
            });

            return deferred.promise;
        };
        var _delete = function (uri) {

            var deferred = $q.defer();

            $http.delete(_uri(uri), {}).then(function (response) {

                deferred.resolve(response.data);

            }, function (response) {

                    if (response.data.message.includes("timeout")) {
                        $notify.info("This action took longer than expected. Please try again");
                    } else {
                        $notify.error(response.data.message);
                    }
                    
                //error ? $notify.error(error) : $notify.serverError();
                
                deferred.reject(response.status);
            });

            return deferred.promise;
        };

        var service = {};

        var accountStatusEnum = {
            none: 0,
            isActivated: 1 << 0,
            isAnonymous: 1 << 1,
            isBusiness: 1 << 2,
            isPreview: 1 << 3
        };;

        service.user = {

            get: function () {

                return _get('api/user');
            },
            insert: function (json) {

                return _post('api/user/insert', json);
            },
            update: function (json) {

                return _post('api/user/update', json);
            },
            changePassword: function (json) {

                return _post('api/user/change-password', json);
            },
            activate: function (id) {

                return _post('api/user/' + id + '/activate');
            },
            deactivate: function (id) {

                return _post('api/user/' + id + '/deactivate');
            }
        };
        service.account = {

            isActivated: function (account) {

                return !!(account.status & accountStatusEnum.isActivated);
            },
            isBusiness: function (account) {

                return !!(account.status & accountStatusEnum.isBusiness);
            },
            isPreview: function (account) {

                return !!(account.status & accountStatusEnum.isPreview);
            },

            get: function (json) {

                var url = 'api/account/'
                if (json.id) {
                    url += json.id;
                    delete json['id'];
                }

                return _get(url, json);
            },
            set: function (accountId, json) { return _post('api/account/' + accountId, json); },
            _delete: function (accountId) { return _delete('api/account/' + accountId); },
            _deleteGDPR: function (accountId) { return _delete('api/account/gdpr/' + accountId); },
            register: function (json) { return _put('api/account/', json); },
            uid: function (accountId) { return _get('api/account/' + accountId + '/uid/'); },
            confirmEmail: function (accountId, json) { return _post('api/account/' + accountId + '/confirm-email/?' + $.param(json)); },
            changePassword: function (accountId, json) { return _post('api/account/' + accountId + '/change-password/', json); },
            recoverPassword: function (accountId) { return _post('api/account/' + accountId + '/recover-password/'); },
            merge: function (accountId, accountIdFrom, accountIdPrimaryEmail) {

                return _post('api/account/' + accountId + '/merge/?' + $.param({

                    accountIdFrom: accountIdFrom,
                    accountIdPrimaryEmail: accountIdPrimaryEmail
                }));
            },

            external: {

                sessionTokens: function (accountId) { return _get('api/account/' + accountId + '/external/session-token/'); },
            },
            subEmails: function (accountId) { return _get('api/account/' + accountId + '/sub-email/'); },

            addProduct: function (accountId, json) { return _put('api/account/' + accountId + '/product/', json); },
            ownerProducts: function (accountId) {

                return _get('api/account/' + accountId + '/owner-product/');
            },
            ownerProductDetails: function (accountId, accountProductId) {

                return _get('api/account/' + accountId + '/owner-product/' + accountProductId);
            },
            ownerProductAssign: function (accountId, accountProductId, json) {

                return _post('api/account/' + accountId + '/owner-product/' + accountProductId + '/assign/', json);
            },
            ownerProductUnassign: function (accountId, accountProductId, targetAccountId) {

                return _post('api/account/' + accountId + '/owner-product/' + accountProductId + '/unassign/' + targetAccountId);
            },
            ownerProductDeactivate: function (accountId, accountProductId) {

                return _post('api/account/' + accountId + '/owner-product/' + accountProductId + '/deactivate/');
            },
            ownerProductResume: function (accountId, accountProductId) { return _post('api/account/' + accountId + '/owner-product/' + accountProductId + '/resume/'); },
            ownerProductSuspend: function (accountId, accountProductId) { return _post('api/account/' + accountId + '/owner-product/' + accountProductId + '/suspend/'); },
            ownerProductAllowed: function (accountId, accountProductId, allowed) { return _post('api/account/' + accountId + '/owner-product/' + accountProductId + '/allowed/?' + $.param({ allowed: allowed })); },
            ownerProductNextRebillDate: function (accountId, accountProductId, nextRebillDate) { return _post('api/account/' + accountId + '/owner-product/' + accountProductId + '/next-rebill-date/?' + $.param({ nextRebillDate: nextRebillDate.toUTCString() })); },
            ownerProductEndDate: function (accountId, accountProductId, endDate) { return _post('api/account/' + accountId + '/owner-product/' + accountProductId + '/end-date/?' + $.param({ endDate: endDate.toUTCString() })); }
        };
        service.log = {

            get: function (json) { return _get('api/log/', json); },
            csv: function (json) { window.open(_uri('api/log/') + '?' + $.param(json) + '&format=csv', '_self', ''); },
            logActionType: {
                get: function (json) { return _get('api/log/action-type/', json); }
            }
        };
        service.upclick = {

            products: function (json) {

                return _get('api/upclick/products', json);
            }
        };
        service.emails = {

            getTemplates: function () { return _get('api/emails/templates'); }
        };

        var productStatus = {};
        Object.defineProperties(productStatus, {

            none: { value: 0, writable: false },
            isDisabled: { value: 1 << 0, writable: false },
            isExpired: { value: 1 << 1, writable: false },
            isTrial: { value: 1 << 2, writable: false },
            isFree: { value: 1 << 3, writable: false },
            isMinor: { value: 1 << 4, writable: false },
            isDefault: { value: 1 << 5, writable: false },
            isPPC: { value: 1 << 6, writable: false },
            isUpgradable: { value: 1 << 7, writable: false },
            isNew: { value: 1 << 8, writable: false },
            isPaymentFailed: { value: 1 << 9, writable: false },
            isRenewal: { value: 1 << 10, writable: false },
            isOwner: { value: 1 << 11, writable: false },
            IsNotAbleToRenewCreditCartExpired: { value: 1 << 12, writable: false }
        });
        service.productStatus = productStatus;

        return service;
    }
})();