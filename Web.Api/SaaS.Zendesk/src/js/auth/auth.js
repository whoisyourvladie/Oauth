angular.module('app.auth')
    .factory('$auth', ['$q', '$state', '$injector', '$authStorage', '$brand', ($q, $state, $injector, $authStorage, $brand) => {


        var _apiHeaders = { 'Content-Type': 'application/x-www-form-urlencoded' };
        var service = {};

        var _getItem = () => {

            var iso = $brand.getIso();
            return $authStorage.getItem(iso);
        };

        service.isAuthenticated = () => {

            if ($brand.isEmpty())
                return false;

            return !!_getItem();
        };
        service.getAccessToken = () => {

            var item = _getItem();
            return item ? item.access_token : null;
        };
        service.getRefreshToken = () => {

            var item = _getItem();
            return item ? item.refresh_token : null;
        };

        service.signIn = (login, password) => {

            var $http = $injector.get("$http");

            var data = [
                'grant_type=password',
                'username=' + encodeURIComponent(login),
                'password=' + encodeURIComponent(password)
            ];

            data = data.join('&');

            var deferred = $q.defer();

            if ($brand.isEmpty()) {
                deferred.reject({ error_description: 'Brand is not supported!' });
            }
            else {
                var uri = $brand.getApiUri('api/token');
                $http.post(uri, data, { headers: _apiHeaders, asJson: true }).then((json) => {

                    var iso = $brand.getIso();

                    $authStorage.setItem(iso, json);
                    deferred.resolve(json);

                }, (error, status, headers) => {
                    deferred.reject(error.data);
                });
            }

            return deferred.promise;
        };
        service.refreshToken = () => {

            var $http = $injector.get("$http");

            var data = [
                'grant_type=refresh_token',
                'refresh_token=' + service.getRefreshToken(),
            ];

            data = data.join('&');

            var deferred = $q.defer();

            var uri = $brand.getApiUri('api/token');
            $http.post(uri, data, { headers: _apiHeaders, asJson: true }).then((json) => {

                var iso = $brand.getIso();

                $authStorage.setItem(iso, json);
                deferred.resolve(json);

            },
                (error, status, headers) => {

                    service.logout();
                    deferred.reject(status);
                });

            return deferred.promise;
        };
        service.logout = () => {

            var iso = $brand.getIso();
            $authStorage.removeItem(iso);

            $state.go('user/sign-in');
        };

        return service;
    }]);