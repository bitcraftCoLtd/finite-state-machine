var fsm = require('../StateMachine/stateManager.js');
var format = require('./format.js');

var ActionTokens = {
    NEXT: fsm.StateManager.makeToken('Next'),
    PREV: fsm.StateManager.makeToken('Prev')
};

var StateTokens = {
    BEGIN: fsm.StateManager.makeToken('First state'),
    UPDATE: fsm.StateManager.makeToken('Running state'),
    END: fsm.StateManager.makeToken('Last state')
};

var sm = new fsm.StateManager();
var sm2 = new fsm.StateManager();

var stateChangedSubscription = sm.registerOnStateChanged(this, function (sender, arg) {
    'use strict';
    console.log(format('onStateChanged: {0} -> {1}', arg.oldState ? arg.oldState.token.name : '(null)', arg.newState ? arg.newState.token.name : '(null)'));
});

var completedSubscription = sm.registerOnCompleted(this, function (sender) {
    'use strict';
    console.log('onCompleted');
});

// -----------------------------------------------

var check = 0;

var stateChangedSubscription2 = sm2.registerOnStateChanged(this, function (sender, arg) {
    'use strict';
    check += 1;
});

var completedSubscription2 = sm2.registerOnCompleted(this, function (sender) {
    'use strict';
    check += 1;
});

// -----------------------------------------------

sm.registerState({
    token: StateTokens.BEGIN,
    onInitialize: function () {
        'use strict';
        console.log(format('onInitialize({0})', this.token.name));
        this.registerActionHandler(ActionTokens.NEXT, this.onNext);
    },
    onEnter: function (eventArg) {
        'use strict';
        var fromState = eventArg.from ? eventArg.from.name : '(null)';
        var data = eventArg.data || '(null)';
        console.log(format('onEnter({0}) [from: {1}, data: {2}]', this.token.name, fromState, data));
    },
    onNext: function (data, cb) {
        'use strict';
        console.log(format('onNext({0}) [data: {1}]', this.token.name, data));
        cb(StateTokens.UPDATE);
    },
    onExit: function () {
        'use strict';
        console.log(format('onExit({0})', this.token.name));
    }
});

sm.registerState({
    token: StateTokens.UPDATE,
    counter: 0,
    onInitialize: function () {
        'use strict';
        console.log(format('onInitialize({0})', this.token.name));
        this.registerActionHandler(ActionTokens.NEXT, this.onNext);
    },
    onEnter: function (eventArg) {
        'use strict';
        var fromState = eventArg.from ? eventArg.from.name : '(null)';
        var data = eventArg.data || '(null)';
        console.log(format('onEnter({0}) [from: {1}, data: {2}]', this.token.name, fromState, data));
    },
    onNext: function (data, cb) {
        'use strict';
        console.log(format('onNext({0}) [data: {1}]', this.token.name, data));
        this.counter += 1;
        if (this.counter < 3) {
            cb(StateTokens.UPDATE);
        } else {
            cb(StateTokens.END, 51);
        }
    },
    onExit: function () {
        'use strict';
        console.log(format('onExit({0})', this.token.name));
    }
});

sm.registerState({
    token: StateTokens.END,
    onInitialize: function () {
        'use strict';
        console.log(format('onInitialize({0})', this.token.name));
        this.registerActionHandler(ActionTokens.NEXT, this.onNext);
        this.registerActionHandler(ActionTokens.PREV, this.onPrev);
    },
    onEnter: function (eventArg) {
        'use strict';
        var fromState = eventArg.from ? eventArg.from.name : '(null)';
        var data = eventArg.data || '(null)';
        console.log(format('onEnter({0}) [from: {1}, data: {2}]', this.token.name, fromState, data));
    },
    onPrev: function (data, cb) {
        'use strict';
        console.log(format('onPrev({0}) [data: {1}]', this.token.name, data));
        cb(StateTokens.UPDATE, undefined);
    },
    onNext: function (data, cb) {
        'use strict';
        console.log(format('onNext({0}) [data: {1}]', this.token.name, data));
        cb(null);
    },
    onExit: function () {
        'use strict';
        console.log(format('onExit({0})', this.token.name));
    }
});

sm.setInitialState(StateTokens.BEGIN);

console.log(fsm.ACTION_RESULT_TYPE.stringify(sm.performAction(ActionTokens.NEXT, '1'))); // begin -> update
console.log(fsm.ACTION_RESULT_TYPE.stringify(sm.performAction(ActionTokens.NEXT, '2'))); // update 1
console.log(fsm.ACTION_RESULT_TYPE.stringify(sm.performAction(ActionTokens.NEXT, '3'))); // update 2
console.log(fsm.ACTION_RESULT_TYPE.stringify(sm.performAction(ActionTokens.NEXT, '4'))); // update 3 -> end
console.log(fsm.ACTION_RESULT_TYPE.stringify(sm.performAction(ActionTokens.PREV, '5'))); // end -> update
console.log(fsm.ACTION_RESULT_TYPE.stringify(sm.performAction(ActionTokens.NEXT, '6'))); // update -> end
console.log(fsm.ACTION_RESULT_TYPE.stringify(sm.performAction(ActionTokens.NEXT, '7'))); // end -> [final]

if (check !== 0) {
    console.log('FAILED!');
}

console.log('done!');

stateChangedSubscription.unregister();
completedSubscription.unregister();

stateChangedSubscription2.unregister();
completedSubscription2.unregister();
