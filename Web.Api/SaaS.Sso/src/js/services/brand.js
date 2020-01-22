angular.module('app.services')
    .factory('$brand', ['$utils', '$location', ($utils, $location) => {

        let brandEnum = {
            sodaPdf: 'sodaPdf',
            pdfArchitect: 'pdfArchitect'
        };

        let _current = null;
        let _settings = {
            ssoLink: {},
            oauthLink: {}
        };

        _settings.ssoLink[brandEnum.sodaPdf] = 'https://sso.sodapdf.com';
        _settings.ssoLink[brandEnum.pdfArchitect] = 'https://sso.pdfarchitect.org';

        _settings.oauthLink[brandEnum.sodaPdf] = 'https://oauth.sodapdf.com';
        _settings.oauthLink[brandEnum.pdfArchitect] = 'https://oauth.pdfarchitect.org';

        let _getSettings = (key) => {

            return _settings[key][service.current()];
        };

        let service = {};

        service.current = () => {

            return _current;
        };
        service.currentName = () => {

            switch (service.current()) {

                case brandEnum.sodaPdf: return 'Soda PDF';
                case brandEnum.pdfArchitect: return 'PDF Architect';

                default: return '';
            }
        };

        service.ssoLink = () => { return _getSettings('ssoLink') };
        service.oauthLink = () => {

            return _getSettings('oauthLink')
        };

        service.validate = () => {

            let query = $utils.query();
            let host = $location
                .host()
                .toLowerCase();

            switch (host) {

                case 'localhost':
                case 'sso.sodapdf.com': _current = brandEnum.sodaPdf; break;
                case 'sso.pdfarchitect.org': _current = brandEnum.pdfArchitect; break;

                case 'sso.lulusoft.com':

                    if (query.brand_id === '360002010612')
                        _current = brandEnum.sodaPdf;

                    _current = _current || brandEnum.pdfArchitect;

                    return false;
            }

            return true;
        };
        service.redirect = () => {

            document.body.style.display = 'none';
            window.location = service.ssoLink() + document.location.search + document.location.hash;
        };
        service.logo = () => {

            switch (service.current()) {

                case brandEnum.sodaPdf: return {
                    width: 120,
                    src: 'https://www.sodapdf.com/images/logo.svg'
                };

                case brandEnum.pdfArchitect: return {
                    width: 200,
                    src: 'https://myaccount.pdfarchitect.org/images/logo.png'

                };

                default: return {};
            }
        };

        return service;
    }]);