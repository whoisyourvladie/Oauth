(function () {
    'use strict';

    angular
        .module('app.controllers')
        .controller('accountProductsController', controller);

    controller.$inject = ['$rootScope', '$scope', '$notify', '$api', '$form'];

    function controller($rootScope, $scope, $notify, $api, $form) {

        var _statusEnum = $api.productStatus;

        $scope.model = {
            account: null,
            products: null
        };
        $scope.status = _statusEnum;

        $rootScope.$on('event:accountLoaded', function (event, json) {

            $scope.model.account = $scope.model.account || {};
            $scope.model.account.id = json.id;
            $scope.refresh();
        });
        $rootScope.$on('event:accountDeleted', function (event, json) {

            $scope.model.account = $scope.model.account || {};
            $scope.model.account.isDeleted = true;
        });

        var viewProduct = function (json) {

            var self = this;

            var _status, _allowed, _used, _plan, _spId;
            var _endDate, _nextRebillDate;

            var _accounts = [];
            var _dateTimeFormat = function (dateTime) {

                return dateTime.toLocaleDateString('en-US', { year: 'numeric', month: 'numeric', day: 'numeric' });
            };

            var minorAllowed = 0;
            var minorUsed = 0;

            if (!!(json.status & 16)) {

                var modules = json.modules;
                for (var index = 0; index < modules.length; index++) {

                    if ((modules[index].name == 'e-sign')) {
                        var module = modules[index];
                        if (module && module.allowed) {

                            minorAllowed = module.allowed;
                            minorUsed = module.allowed - module.used;
                        }
                    }
                }
            }

            Object.defineProperties(this, {
                allowedEditMode: { value: false, writable: true },
                nextRebillDateEdit: { value: false, writable: true },

                id: { value: json.id, writable: false },
                name: { value: json.name, writable: false },
                unitName: { value: json.unitName, writable: false },
                status: { get: function () { return _status; } },
                allowed: { get: function () { return _allowed; } },
                used: { get: function () { return _used; } },
                plan: { get: function () { return _plan; } },
                spId: { get: function () { return _spId; } },

                ownerEmail: { value: json.ownerEmail, writable: false },
                purchaseDate: { value: json.purchaseDate, writable: false },
                endDate: { get: function () { return _endDate; } },
                nextRebillDate: { get: function () { return _nextRebillDate; } },

                isLocalSubscription: { get: function () { return this.spId && this.spId.startsWith("D00000"); } },

                minorAllowed: { value: minorAllowed, writable: false },
                minorUsed: { value: json.minorUsed, writable: false },

                accounts: { get: function () { return _accounts; } }
            });

            this.update = function (json) {

                this.nextRebillDateEdit = json.nextRebillDate;
                this.endDateEdit = json.endDate;

                _status = json.status;
                _allowed = json.allowed;
                _used = json.used;
                _plan = json.plan;
                _spId = json.spId || null;

                //_modules = json.modules;

                //_purchaseDate = json.purchaseDate;
                //_endDate = json.endDate;
                _endDate = json.endDate;
                _nextRebillDate = json.nextRebillDate;


                var accounts = [];
                if (json.accounts && json.accounts.length) {

                    for (var index = 0; index < json.accounts.length; index++)
                        accounts.push(new viewAccount(self, json.accounts[index]));
                }
                _accounts = accounts;
            };
            this.update(json);
        };
        viewProduct.prototype.isStatus = function (status) {

            return !!(this.status & status);
        };
        viewProduct.prototype.isAssignable = function () {

            return this.isStatus(_statusEnum.isOwner) &&
                   !this.isStatus(_statusEnum.isFree) &&
                   !this.isStatus(_statusEnum.isTrial) &&
                   this.allowed > this.used;
        };
        viewProduct.prototype.isRenewal = function () {

            return !this.isLocalSubscription &&

                    this.isStatus(_statusEnum.isOwner) &&
                   !this.isStatus(_statusEnum.isFree) &&
                   !this.isStatus(_statusEnum.isTrial) &&
                    this.isStatus(_statusEnum.isRenewal);
        };
        viewProduct.prototype.isEndDate = function () {

            return this.isLocalSubscription &&

                    this.isStatus(_statusEnum.isOwner) &&
                   !this.isStatus(_statusEnum.isFree) &&
                   !this.isStatus(_statusEnum.isTrial) &&
                   !this.isStatus(_statusEnum.isPPC);
        };
        

        viewProduct.prototype.unassign = function () {

            var product = this;
            //$dialog.account.wouldLikeContinue().result.then(function (state) {

            //    state == 'ok' && $saasApi.ownerProductUnassign(product.id).then(function (json) {

            //        product.update(json);
            //        $scope.refresh();

            //    }, _handleError);
            //});
        };
        viewProduct.prototype.isExpandable = function () {

            if (this.isStatus(_statusEnum.isDisabled) || this.isStatus(_statusEnum.isFree) || this.isStatus(_statusEnum.isTrial))
                return false;

            return true;
        };
        viewProduct.prototype.expand = function () {

            if (!this.isExpandable())
                return;

            this.viewExpanded = !this.viewExpanded;
            this.viewExpanded && _ownerProductDetails(this);
        };

        viewProduct.prototype.isDeactivatable = function () {

            if (this.isStatus(_statusEnum.isDisabled) || this.isStatus(_statusEnum.isFree) || this.isStatus(_statusEnum.isTrial))
                return false;

            return true;
        };
        viewProduct.prototype.deactivate = function () {

            if (!this.isDeactivatable())
                return;

            $scope.deactivateProduct(this);
        };
        viewProduct.prototype.resume = function () {

            $scope.resumeProduct(this);
        };
        viewProduct.prototype.suspend = function () {

            $scope.suspendProduct(this);
        };
        viewProduct.prototype.changeNextRebillDate = function () {
            
            $scope.nextRebillDateProduct(this);
        };
        viewProduct.prototype.changeEndDate = function () {

            $scope.endDateProduct(this);
        };

        viewProduct.prototype.allowedEditShow = function () {

            this.allowedEditMode = true;
            this.allowedEdit = this.allowed;
        };
        viewProduct.prototype.allowedEditHide = function () {

            this.allowedEditMode = false;
            delete this.allowedEdit;
        };
        viewProduct.prototype.isAllowedChangeable = function () {

            if (this.isStatus(_statusEnum.isDisabled) ||
                this.isStatus(_statusEnum.isFree) ||
                this.isStatus(_statusEnum.isTrial) ||
                !this.isStatus(_statusEnum.isOwner))
                return false;

            return true;
        };

        var viewAccount = function (product, json) {

            var self = this;

            Object.defineProperties(this, {
                product: { value: product, writable: false },
                accountId: { value: json.accountId, writable: false },
                email: { value: json.email, writable: false }
            });

            this.update = function (json) { };
            this.update(json);
        };

        viewAccount.prototype.isUnAssignable = function () {

            return this.product.isStatus(_statusEnum.isOwner);
        };
        viewAccount.prototype.unassign = function () {

            var product = this.product;
            var targetAccountId = this.accountId;

            $api.account.ownerProductUnassign($scope.model.account.id, product.id, targetAccountId).then(function(json) {

                $notify.info("Product has been unassigned successfully.");
                product.update(json);
            });
        };

        var _ownerProducts = function () {

            $api.account.ownerProducts($scope.model.account.id).then(function(json) {

                var viewProducts = [];

                for (var index = 0; index < json.length; index++) {

                    var jsonProduct = json[index];
                    var product = new viewProduct(jsonProduct);

                    viewProducts.push(product);
                }

                $scope.model.products = viewProducts;
            })
            .finally(function() { $scope.isBusy = false; });
        };
        var _ownerProductDetails = function (product) {

            return $api.account.ownerProductDetails($scope.model.account.id, product.id).then(function(json) {

                product.update(json);
            });
        };

        $scope.refresh = function () {

            if($scope.isBusy) return;

            $scope.isBusy = true;
            $scope.model.products = null;

            _ownerProducts();
        };
        $scope.deactivateProduct = function (product) {

            $api.account.ownerProductDeactivate($scope.model.account.id, product.id).then(function(json) {

                $notify.info("Product has been deactivated successfully.");

                var index = $scope.model.products.indexOf(product);
                index !== - 1 && $scope.model.products.splice(index, 1);
            });
        };
        $scope.assign = function (product, form) {

            $form.submit(product, form, function () {

                return $api.account.ownerProductAssign($scope.model.account.id, product.id, { email: product.assignEmail }).then(function(json) {

                    $notify.info("Product has been assigned successfully.");
                    product.update(json);
                }).finally(function() {

                    delete product.assignEmail;
                    product.isBusy = false;
                    form.$setPristine();
                });
            });
        };
        $scope.allowed = function (product) {

            return $api.account.ownerProductAllowed($scope.model.account.id, product.id, product.allowedEdit).then(function (json) {

                $notify.info("Amount of licenses has been changed successfully.");
                product.update(json);
            }).finally(function () {

                product.allowedEditHide();
                product.isBusy = false;
            });
        };

        $scope.resumeProduct = function (product) {

            $api.account.ownerProductResume($scope.model.account.id, product.id).then(function(json) {

                $notify.info("Product has been resumed successfully.");
                product.update(json);
            })
            .finally(function () { product.isBusy = false; });
        };
        $scope.suspendProduct = function (product) {

            $api.account.ownerProductSuspend($scope.model.account.id, product.id).then(function(json) {

                $notify.info("Product has been suspended successfully.");
                product.update(json);
            })
            .finally(function () { product.isBusy = false; });
        };
        $scope.nextRebillDateProduct = function (product) {

            $api.account.ownerProductNextRebillDate($scope.model.account.id, product.id, product.nextRebillDateEdit).then(function (json) {

                $notify.info("Next rebill date has been changed successfully.");
                product.update(json);
                product.viewExpanded = false;
            })
            .finally(function () { product.isBusy = false; });
        };
        $scope.endDateProduct = function (product) {

            $api.account.ownerProductEndDate($scope.model.account.id, product.id, product.endDateEdit).then(function (json) {

                $notify.info("Expiry date has been changed successfully.");
                product.update(json);
                product.viewExpanded = false;
            })
            .finally(function () { product.isBusy = false; });
        };

        $scope.nextRebillDateOptions = {
            minDate: new Date()
        };
        $scope.endDateOptions = {
            minDate: new Date()
        };
        $scope.submit = function (form) {

            $form.submit($scope, form, function () {

                $scope.refresh();
            });
        };
    };
}) ();