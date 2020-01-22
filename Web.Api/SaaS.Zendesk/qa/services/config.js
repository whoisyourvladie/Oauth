describe('service:$config', () => {

    var $config;

    beforeEach(module('app'));
    beforeEach(inject(($injector) => {

        $config = $injector.get('$config');
    }));

    it('getRemoteHost:toBeDefined', () => {

        expect($config.getRemoteHost).toBeDefined();
    });
    it('getRemoteHost:not.toBeNull', () => {

        expect($config.getRemoteHost()).not.toBeNull();
    });
});