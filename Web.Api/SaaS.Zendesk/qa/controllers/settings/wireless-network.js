describe(`controller:settingsWirelessNetwork`, () => {

    var $rootScope, $scope;

    beforeEach(module('app'));
    beforeEach(inject(($injector) => {

        $rootScope = $injector.get('$rootScope');
        $scope = $rootScope.$new();
        $injector.get('$controller')('settingsWirelessNetworkController', {
            $scope: $scope
        });
    }));

    it('autoDisableWifis:toBeDefined', () => {

        expect($scope.autoDisableWifis).toBeDefined();
    });
    it('autoDisableWifis:toBeGreaterThan(0)', () => {

        expect($scope.autoDisableWifis.length).toBeGreaterThan(0);
    });
    it('model:toBeDefined', () => {

        expect($scope.model).toBeDefined();
    });
});