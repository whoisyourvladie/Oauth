angular.module('app.services')
    .factory('$brand', () => {

        var _brands = {
            sodapdf: {
                api: 'https://cp-oauth.sodapdf.com'
            },
            pdfarchitect: {
                api: 'https://cp-oauth.pdfarchitect.org'
            }
        };
        var _brand = null;

        var service = {};

        service.isEmpty = () => {

            return !_brand;
        };
        service.set = (brand) => {

            _brand = brand;
        };
        service.get = () => {

            return _brand;
        };

        service.isSupport = () => {

            var iso = service.getIso(_brand);
            return !!_brands[iso];
        };
        service.getIso = () => {

            return _brand.subdomain;
        };
        service.getApiUri = (method) => {

            var iso = service.getIso();
            var api = _brands[iso].api;

            return `${api}/${method}`;
        };
        service.getLogo = () => {

            if (service.isEmpty())
                return null;

            return _brand.logo.contentUrl;
        };

        return service;
    });