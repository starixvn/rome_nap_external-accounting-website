var StateManager = /** @class */ (function () {
    function StateManager(initialValue) {
        this.subscribers = [];
        this.value = initialValue;
    }
    StateManager.prototype.isSubscribed = function (callback) {
        return this.subscribers.some(function (item) { return item.prototype.name === callback.prototype.name; });
    };
    StateManager.prototype.setValue = function (newValue) {
        if (JSON.stringify(this.value) !== JSON.stringify(newValue)) {
            this.value = newValue;
            this.notifySubscribers();
        }
    };
    StateManager.prototype.getValue = function () {
        return this.value;
    };
    StateManager.prototype.subscribe = function (callback) {
        if (!this.isSubscribed(callback)) {
            this.subscribers.push(callback);
        }
    };
    StateManager.prototype.unsubscribe = function (callback) {
        this.subscribers = this.subscribers.filter(function (subscriber) { return subscriber !== callback; });
    };
    StateManager.prototype.notifySubscribers = function () {
        var _this = this;
        this.subscribers.forEach(function (subscriber) { return subscriber(_this.value); });
    };
    return StateManager;
}());
//# sourceMappingURL=StateManager.js.map