class StateManager<T> {
    private subscribers: ((value: T) => void)[] = [];
    private value: T;

    constructor(initialValue: T) {
        this.value = initialValue;
    }

    private isSubscribed(callback: (value: T) => void): boolean {
        return this.subscribers.some(item => item.prototype.name === callback.prototype.name);
    }

    public setValue(newValue: T) {
        if (JSON.stringify(this.value) !== JSON.stringify(newValue)) { 
            this.value = newValue;
            this.notifySubscribers();
        }
    }

    public getValue(): T {
        return this.value;
    }

    public subscribe(callback: (value: T) => void) {
        if (!this.isSubscribed(callback)) {
            this.subscribers.push(callback);
        }
    }

    public unsubscribe(callback: (value: T) => void) {
        this.subscribers = this.subscribers.filter(subscriber => subscriber !== callback);
    }

    private notifySubscribers() {
        this.subscribers.forEach(subscriber => subscriber(this.value));
    }
}