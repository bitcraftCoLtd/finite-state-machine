# StateMachine

## Information

Last update: 2016/11/22

All the solutions and projects in this repository are made with Microsoft Visual Studio 2013 Community Edition, and open seamlessly with Visual Studio 2015 Community Edition.

Not yet tested with Visual Studio 2017 RC Community Edition.

## Overview

The StateMachine provided by [bitcraft](https://www.bitcraft.co.jp) contains a runtime library for C#, C++ and JavaScript (a TypeScript implementation trial is also available, but has never been tested), and a tool to generate finite state machine source code (C# only) from a .graphml diagram file.

A state machine is composed of a state manager (`StateManager` class) and states (`StateBase` abstract class). Each state have handlers that perform transitions based on actions.

The states and actions are identified by what is called a *token*. They are respectively `StateToken` and `ActionToken` classes. (It differs for JavaScript runtime)

When a state machine is instanced the first time, it instances all its states and set the initial state.
Then, when an action occurs, the state machine tells the current state about the action.
In the current state, the handler corresponding to the action is called, and it has to decide to which state to move on, based on that action. The next state to move on is identified thanks to its `StateToken`, not to its instance.

Then the process repeats.

## The short version first

Create tokens to identify states and actions, using respectively the `StateToken` class and the `ActionToken` class.

Then create states inheriting from the `StateBase` class. Implement the state transitions in each state in the virtual `OnInitialized` method, using the `RegisterActionHandler` method.
In your action handlers, use the callback provided to tell the state machine where to transition to.

Finally, instanciate a `StateManager` class, register the states using its `RegisterState` method, and call its `SetInitialState` method to provide an entry point to the state machine.

Once all that is done, you are good to go and can call `PerformAction` method on the `StateManager` class to start having your state machine do its job.

## Runtime libraries overview

### C\#

The C# runtime library is created using .NET 3.5 in order to run on a wide range of platforms.
It builds with `Any CPU` configuration, so it is totally architecture independent.

The runtime is in the folder `Runtime\CSharp` and tools are in the `Tools` folder.

### C++

The C++ runtime library uses the STL for map, and that's pretty much it in term of high level features. The code should build on almost any platforms and architectures.

The runtime is in the folder `Runtime\Cpp`.

### JavaScript

The JavaScript implementation uses only one `module.exports`, so commenting it out makes the state machine ready for browsers or embedded JavaScript engines.

The runtime is in the folder `Runtime\JavaScript`.

### TypeScript

This is a trial for fun and have never been tested.

The runtime is in the folder `Runtime\TypeScript`.

## Runtime

This section focuses on the C# runtime, but the same (almost) applies to the other runtime libraries.
The C++ and JavaScript runtime libraries have a simple sample that demonstrates how they work.
Specific cases for C++ and JavaScript runtimes are described briefly in the annex section at the end of this document.

### The creation process

In the following document, a shopping cart is used as sample for the purpose of explanation.

![Sample shop state machine](https://www.bitcraft.co.jp/pub/github/finite-state-machine/shop_sample_state_machine.png "Sample shop state machine")

All the runtime types are located in the namespace `Bitcraft.StateMachine`.

    using Bitcraft.StateMachine;

#### StateManager

First, you need a `StateManager` class.

The `StateManager` class is a concrete class, so it is not necessary to create a child class that inherit from it, however it is recommended to do so because soon or late you may need to enrich it.

#### Tokens

Then you need tokens, to identify the states and actions.
The `StateToken` and `ActionToken` classes inherit from the `Token` class, and are used for strong typing purpose only.

The recommended way is to use separated containers, and named matching the related state machine, as follow:

    public static class BasketStateTokens
    {
        public static readonly StateToken ProductList = new StateToken("Product List");
        public static readonly StateToken Payment = new StateToken("Payment");
        public static readonly StateToken Confirmation = new StateToken("Confirmation");
        public static readonly StateToken ThankYouScreen = new StateToken("Thank you!");
        ...
    }

The string provided to the `Token` constructor is purely informative, this is a display name and absolutely not the `Token`'s identity.
`Token` identity is based on the `Guid` type.

The constructor of `ActionToken` can also take an informative string as parameter.

    public static class BasketActionTokens
    {
        public static readonly ActionToken GoToProductList = new ActionToken();
        public static readonly ActionToken GoToPaymentScreen = new ActionToken();
        public static readonly ActionToken GoToConfirmation = new ActionToken();
        public static readonly ActionToken PurchaseConfirmed = new ActionToken();
        public static readonly ActionToken PurchaseCancelled = new ActionToken();
        ...
    }

#### States

Eventually you need states, represented by the `StateBase` abstract class.
Our recommendation is to create another abstract base class inheriting from `StateBase` that will contain the common code for all states related to a given state machine.
Then to create concrete specific states based on that common base state instead of directly inheriting from `StateBase`.

For a state machine that implements a shopping cart, you may create the following class hierarchy:

    public abstract class BasketStateBase : StateBase
    {
        protected BasketStateBase(StateToken token)
            : base(token)
        {
        }

        ...
    }

Then:

    public class ProductListBasketState : BasketStateBase
    {
        public ProductListBasketState()
            : base(BasketStateTokens.ProductList)
        {
        }

        ...
    }

    public class PaymentBasketState : BasketStateBase
    {
        public PaymentBasketState()
            : base(BasketStateTokens.Payment)
        {
        }

        ...
    }

    public class ConfirmationBasketState : BasketStateBase
    {
        public ConfirmationBasketState()
            : base(BasketStateTokens.Confirmation)
        {
        }

        ...
    }

With an intermediate state, you can implement the *any state* pattern easily.

As you can see, each specific state describes its own identity through a `StateToken` instance.

A state have to register action handlers, they tell the state machine of which actions the current state is aware of.
This is best done in the `OnInitialized()` virtual method.

    public class PaymentBasketState : BasketStateBase
    {
        public PaymentBasketState()
            : base(BasketStateTokens.Payment)
        {
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            RegisterActionHandler(BasketActionTokens.GoToConfirmation, OnGoToConfirmationAction);
        } //                                                           |
          //         +-------------------------------------------------+
          //         |
          //         V
        private void OnGoToConfirmationAction(object data, Action<StateToken> callback)
        {
            callback(BasketStateTokens.Confirmation);
        }

        ...
    }

Two things here. First the `OnInitialized()` virtual method overridden with registration of an action handler.
This registration says "if you ask me to go to confirmation screen, I know *what to do*, otherwise you will not go any further".

The second thing is the "what to do" from the previous sentence.
The method `OnGoToConfirmationAction` is called when the `PerformAction()` method of the state machine is called with `BasketActionTokens.GoToConfirmation` parameter given and when in the `PaymentBasketState` state.
The method body says "OK, go to confirmation state".
It could go to another state based on a condition or whatever, this is purely up to you.

The *callback* provided to the handler can be called later.
During this delay phase, all subsequent calls to the `PerformAction()` method will return the `ActionResultType.ErrorAlreadyPerformingAction` value.

Note that the delayed transition feature is only available in the C# and JavaScript runtimes.
For the C++ and the TypeScript runtimes, the state machine have to be pulsed.

One very very important thing here is, when a handler is called, the callback **MUST** be called at least once.
If you call it more than once, the subsequent calls are ignored, but if you do not call it at all, the state machine keeps waiting and thus gets locked because, once again, all subsequent calls to the `PerformAction()` method will return the `ActionResultType.ErrorAlreadyPerformingAction` value, and thus no other transitions will be permitted.

One last thing about action handlers, it is possible to register an action handler that allows you to provide custom data to the target state, by registering the action handler as follow:

    protected override void OnInitialized()
    {
        base.OnInitialized();
        RegisterActionHandler(BasketActionTokens.GoToConfirmation, OnGoToConfirmationAction);
    }

    //                                                                    ------ additional argument here
    private void OnGoToConfirmationAction(object data, Action<StateToken, object> callback)
    {
        //                                       ----------------- a different custom value can be provided
        callback(BasketStateTokens.Confirmation, anotherCustomData);
    }

#### All together

The last step is to stick everything together.
Instanciate a `StageManager` object, register states and set the initial state, as follow:

    var fsm = new StateManager();

    fsm.RegisterState(new ProductListBasketState());
    fsm.RegisterState(new PaymentBasketState());
    fsm.RegisterState(new ConfirmationBasketState());

    fsm.SetInitialState(BasketStateTokens.ProductList);

If you opted for a custom child `StateManager` class, you can register states in the constructor:

    public class BasketStateMachine : StateManager
    {
        public BasketStateMachine()
        {
            RegisterState(new ProductListBasketState());
            RegisterState(new PaymentBasketState());
            RegisterState(new ConfirmationBasketState());
        }
    }

Calling the `SetInitialState()` method in the constructor is not recommended because you may need to set a different initial state according to some conditions, for example for testing and debugging purpose you may want to start directly from a certain state to save time.

#### Using the state machine

Once everything is in place, what remains is simply to use the state machine in order to make it do something.
The way to use the state machine is simply to call one method, `PerformAction()` and that's it.
When using a finite state machine, you are not telling the state machine in which state to move on, you just tell it what you are doing, and it is up to the state machine to tell you if you are doing things good or not.

When you call the `PerformAction()` method, you give it an `ActionToken` parameter in order to describe what you are doing.
If needed you can give an additional data, this is described in a section bellow.

The `PerformAction()` method returns an `ActionResultType` enumeration value, which can be:

- `Success` meaning the current state was aware of the action you performed and that transition happened
- `ErrorUnknownAction` meaning the current state in unaware of what you want to do
- `ErrorAlreadyPerformingAction` meaning a state transition is in progress and decision has been purposely delayed and still under way
- `ErrorForbiddenFromSpecialEvents` meaning that calls to `PerformAction()` are forbidden during `OnInitialized`, `OnEnter`, `OnExit`, `OnStateChanged` and `OnCompleted` events.

#### More details

There are some more details you may be interested in, such as:

- the possibility for all states to share a context for data exchange purpose
- the possibility to immediately redirect from a state to another
- to pass specific data to a state when requesting the state machine to evaluate an action
- to be notified of what is happening in the state machine

##### Context

It is possible to share a context object among all states.
In some cases it it necessary for a state to give feedback to another, in a clean and contained way.

The context is optional, and if used, is passed once at the constructor of the `StateManager` class.
If you use a custom child `StateManager` class, you may need to expose the constructor that takes a context object as parameter.

The context is then automatically accessible to all state through their `Context` property.

##### Data

It is possible to pass specific data to a state when requesting a transition.
When calling the `PerformAction()` method, along with the action you are doing, you can pass an instance of object as custom data.

Be careful to not be confused between data and context, since they are both of `object` type.

When you need to pass several properties, it is recommended to create a specific class or structure with the required properties.
If you feel lazy to do so, we then recommend using a `Dictionary<TKey, TValue>` instead of a `Tuple`, but this purely up to you.

A specific data can also be provided to the state machine when calling the `SetInitialState()` method, so the data is provided to the initial state.

Eventually, data can be provided by a state to another.
When a state is given data, and that state redirects to another state, the same data is automatically forwarded to the redirected state.
You can always change this behaviour if needed.

##### Overrides

The `StateManager` and `StateBase` classes have virtual methods that you can override for your convenience.

The `StateManager` class has the following virtual methods:

- `OnStateChanged()` is called each time the state machine transition from a state to another.
    - This method receives a `StateChangedEventArgs` argument that contains:
        - `OldState` that represent the state that was active before the transition.
        - `NewState` that represent the state that is active after the transaition.
- `OnCompleted()` is called when the state machine has reach its terminal state and is over.

The `StateBase` class has the following virtual methods:

- `OnInitialized()` that is called once the state has been attached to a state machine.
- `OnEnter()` is called just after the state machine has changed its internal state to the current state.
    - This method receives a `StateEnterEventArgs` argument that contains:
        - `From` telling the origin state from which the transition is happening.
        - `Data` which is an optional custom data.
        - `Redirect` that allows immediate redirection to another state. (more information in "Fast redirection" section below)
- `OnExit()` is called just before the state machine changes to another state.
    - This method receives a `StateExitEventArgs` argument that contains:
        - `To` telling the destination state to which the transition is happening.
        - `Data` which is an optional custom data.

You basically uses the `OnInitialized()` method to register action handlers, the `OnEnter()` method to start initializing what is needed for the current state life cycle, and `OnExit()` to clean up the current state related things.

##### Partial methods

Beside regular overridable methods, there are convenient partial methods available to further extend state class without disrupting integration with automatically generated code.

The automatically generated state classes have the two partial methods for you to implement (or not), called from the `OnInitialized` method.

- `PreInitialized` is called before the `base.OnInitialized` method, and thus before registering the action handlers.
- `PostInitialized` is called after the `base.OnInitialized` method, and thus after registering the action handlers.

From your point of view as the implementer, you should see calls in the following order:

- `PreInitialized` (if implemented)
- `base.OnInitialized` (if implemented in a parent class)
- Automatically generated `RegisterActionHandler` calls
- `PostInitialized` (if implemented)

##### Fast redirection

When you need to change state to another state from within the `OnEnter()` method call, you cannot call `PerformAction()`.

Instead, you have to tell the state machine to directly redirect to a given state by setting the `TargetStateToken` property of the `Redirect` property of the event given as parameter.
Hereafter is an example.

    public class PaymentBasketState : BasketStateBase
    {
        public PaymentBasketState()
            : base(BasketStateTokens.Payment)
        {
        }

        ...

        protected override void OnEnter(StateEnterEventArgs e)
        {
            base.OnEnter(e);

            if (isFastBuyOptionActivated)
            {
                e.Redirect.TargetStateToken = BasketStateTokens.ThankYouScreen;
                // e.Redirect.TargetStateData = ... you can also optionally set a data to be forwarded
            }
        }

        ...
    }

Here, when the `PaymentBasketState` becomes active, it checks whether the user has activated the fast buy option, and if yes, it requests the state machine to directly move to the "Thank you" screen, skipping the confirmation state.

Fast redirection feature does not allow delaying state transition, the operation can only be synchronous.

## Pulsing the state machine

Pulsing the state machine is not a feature, it is simply a useful technique to delay state transition.
When you need to delay a transition, hereafter are the steps to follow:

1. Define a *PULSE* action. (or whatever name you like)
2. Register the *PULSE* action handler in your state.
3. When time has come, call the `PerformAction()` method and pass it the *PULSE* action.
4. Call the transition callback from the *pulsed* action handler.
5. Repeat from step 3 as much as needed.

## Tools

[bitcraft](https://www.bitcraft.co.jp) offers a tool to generate base state machine code (sub class of state manager, state tokens, action tokens, specific states and handler registration) from a .graphml diagram file.

Note that the tool only generates C# code.
Generating JavaScript makes few sense, and generating C++ is not planned to be developed, so if you need this feature, please implement it by yourself, based on what the tool already provides.

The diagram tool used is yEd from yWorks: [Download page](http://www.yworks.com/products/yed/download)

For the moment, no other tool has been tested, neither other .graphml files generated with other tools.

### Libraries overview

The `Tools` folder contains the code generator tool, split into several projects.
Hereafter is the description of each project:

- `SampleFiles`
    - Contains sample .graphml file that represent possibly real state machines.
- `Bitcraft.ToolKit.CodeGeneration`
    - Contains high level abstraction primitives for code generation and a C# oriented implementation.
    - Code generation has been abstracted as much as possible, but some languages have few things in common, and thus it doesn't make that much sense to try to abstract that.
    - The [Roslyn](https://github.com/dotnet/roslyn) library could probably be used, but maybe a bit overkill for this usage, though could have been very interesting to investigate and learn.
- `Bitcraft.StateMachineTool.Core`
    - Contains the contracts the tool needs to parse and load oriented graphs data.
- `Bitcraft.StateMachineTool.CodeGenerators`
    - Contains the classes that generate specific code from an oriented graph.
      This library is aware of the language (output) but not of the file format (input), because it is abstracted by the `Bitcraft.StateMachineTool.Core` library.
- `Bitcraft.StateMachineTool.yWorks`
    - Contains the implementation of the contracts in `Bitcraft.StateMachineTool.Core` that support the yEd .graphml files.
- `Bitcraft.StateMachineTool`
    - The executable that stick everything together.
    - Implements a simple (and dirty) command line arguments parser to take user's choices into account.

### Create a graph

How to use the yEd graph editor tool is beyond the scope of this documentation.
However yEd is very straightforward and intuitive to use so no need documentation in my opinion.

First, in yEd you can create custom properties that you will be able to set on your nodes and transitions.
Let's add an *IsInitialState* and *IsFinalState* properties.
Click on the *Edit* menu, then *Manage Custom Properties...* menu.
Then you should see the following screen.

![Custom Properties](https://www.bitcraft.co.jp/pub/github/finite-state-machine/doc01.png "Custom Properties")

In the *Node Properties* category at top of the window, click the green '+' button in order to add a new property.
Set their name to *IsInitialState* and *IsFinalState*, and set their type to *Integer*, which is the closest type to what is desired, *boolean*.
Eventually, set their default value to 0 if it is not.

Then, create some nodes and links, which will be the states and transitions in your state machine.
In the *Properties View* of yEd, when you select a node, you should see a *General* section containing the *Text* property, this one you can set anything you want, it is purely for visual representation.
Then in the *Data* section, the *Description* property is what is really used to give a name to the state class in the code.
The *IsInitialState* property is set to 1, which means true, to tell the code generator that this is the initial state of the state machine.
The *IsFinalState* property remains set to 0.

![State properties](https://www.bitcraft.co.jp/pub/github/finite-state-machine/doc02.png "State properties (and initial state)")

You can also set a state as a final state, by selecting the state and setting the *IsFinalState* property of the *Data* section to 1, which means true.

![State properties](https://www.bitcraft.co.jp/pub/github/finite-state-machine/doc03.png "State properties (and final state)")

Note that in this particular case, *Description* is useless.
You can set one if you want but it will just be ignored.
The *Text* property is not set, but anyway it is optional in all other cases.

You can also set properties on transitions, but they do not have custom properties.
When you select one, you should see the *Text* property in the *General* section containing what is displayed on the transition, and *Description* in the *Data* section containing the real name of the transition in the generated source code.

![Transition properties](https://www.bitcraft.co.jp/pub/github/finite-state-machine/doc04.png "Transition properties")

Finally, the same applies to the whole graph itself too.
Select nothing (click in the workspace blank part) and then the *Properties View* shows the graph properties, as follow:

![Graph properties](https://www.bitcraft.co.jp/pub/github/finite-state-machine/doc05.png "Graph properties")

Here the *Description* property in the *Data* section represent the name of the state machine in the generated source code.
In the *General* section, the *Number of Nodes* and *Number of Edges* properties are read-only and just informative.

### Using the generator tool

Hereafter is the usage of the tool you get when providing it the *-help* argument.

       -version    Shows current version number.
          -help    Shows this help.

          -file    <file> sets the input graph description file.

            -ns    <namespace> sets the namespace of generated files.
                   If not set, the classes are generated without namespace.

          -name    <name> sets the name of the state machine.
                   It is used to prefix some classes or other code elements.
                   If it is not set, the name defined in the graph file is used.
                   When both are not defined, an error is displayed and code
                   generation is aborted.

           -out    <folder> sets the output folder where code is generated.
                   If it is not set, the output folder is the folder where the graph
                   file is located.

        -fromwd    If -out parameter is set, then:
                       If <folder> is absolute, the flag -fromwd is ignored.
                       If <folder> is relative, then:
                           If -fromwd flag is not set, then <folder> is relative to the graph file directory.
                           If -fromwd flag is set, then <folder> is relative to the current working directory.
                   If -out parameter is not set, then:
                       If -fromwd flag is set, then the output folder is the current working directory.
                       If -fromwd flag is not set, then the output folder is the graph file directory.

          -init    <state> generates the SetInitialState(<state>) call.
                   If not set, the IsInitialState flag from the .graphml file is used, if any.

     -statebase    Make all generated state classes to inherit from
                   Bitcraft.StateMachine.StateBase class instead of from
                   <name>StateBase, where <name> is given by -name option.

      -internal    Makes all exposed types internal instead of public.

Only the **-file** argument is mandatory, all others are optional.

- If the *-name* argument is provided, it overrides the name of the state machine set in the graph file.
- If the *-statebase* argument is provided, it tells the generator that you do not want an intermediate state class for each states of the state machine.

All the default values are set to what people should need in most of cases, so that allow you to simply drag and drop you graph file on the tool executable to get your code generated.

Maybe only the *-ns* parameter can be a problem, because in most of cases you want a namespace, but it is impossible for the tool to guess the good default value for you.
If like me you feel lazy, you can patch the code to force the *-ns* value to what you want so you can still get the code you want with a simple drag and drop.

## Annex

### JavaScript runtime

At the end of the `stateManager.js` file, remove the `module.exports = fsm;` statement if you run the code in a browser or embedded JavaScript engine.
Keep the code as is if you run it in NodeJS.

A sample is provided in the folder `Runtime\JavaScript\Tester` and can be run with NodeJS with the following command:

    > node app.js

#### Token

The namespace of the JavaScript runtime is `fsm`, and there is only an `ACTION\_RESULT\_TYPE` enum and the `StateManager` class.

In the JavaScript runtime, there is no base classes such as `Token` or `StateBase`, so the way to describe a `token` is to create an `object` with a `number` member named `id` and a `string` member named `name`.

    var stateToken1 = { id: 3, name: 'My State' }; // state token
    var actionToken1 = { id: 51, name: 'My Action' }; // action token

You can use helper functions such as `fsm.StateManager.nextId()` to get the next unique identifier.

    var stateToken2 = { id: fsm.StateManager.nextId(), name: 'Another State' };

Or you can even directly create a token using the `fsm.StateManager.makeToken()` helper function.

    var stateToken3 = fsm.StateManager.makeToken('Yet Another State');
    var actionToken2 = fsm.StateManager.makeToken('Yet Another Action');

The latter is recommended because you should not try to control the value of the `id` by yourself, it is for state machine internal usage only.

#### State

A state in the JavaScript runtime is represented by an object that contains at least a `token` member set with a value that matches the `token` requirement explained in the section above.

Then you can add an `onInitialize` function that will be called when the state is registered in a state machine and is ready to be used.
There are also the `onEnter` and `onExit` functions that are called when the state gets activate or inactive.

The remaining functions you may need are injected into the state by the state manager when it is registered to it.

#### Action handlers

In an action handler function of a state where you receive the data and the decision callback, you can call the decision callback by either providing it the state token of the next state to transition to, or the state token and a new custom data to be provided to the next state.

Let's see with simple examples:

    onNext: function (data, cb) {
        'use strict';
        cb(StateTokens.UPDATE);
    }

The way the callback is used means the state machine transition to the `UPDATE` state.
The fact you do not provide data does not mean `null` or `undefined` is passed, it means the given custom data is passed through to the next state automatically.

    onNext: function (data, cb) {
        'use strict';
        cb(StateTokens.UPDATE, 51);
    }

The way the callback is used in this case means the state machine transition to the `UPDATE` state, and the new 51 custom data is given to the next state.

If you have no data to provide but you do not want the previous custom data to flow through, then you have to explicitly pass `null` or `undefined`, like this:

    onNext: function (data, cb) {
        'use strict';
        cb(StateTokens.UPDATE, null);
    }

### C++ runtime

#### Action handlers

For language reason, the C++ runtime cannot delay state transition decision.
The transition to the next state must be decided in the action handler.

When you declare an action handler, you have to provide a non class member static function, as follow:

    static void OnGotoState1(StateBase* self, StateData* data, TransitionInfo* result)

And register it this way:

    RegisterActionHandler(TestActionTokens::GoToState1Action, OnGotoState1);

The state machine automatically provides an instance of the current state to the function when it calls it, so you can make it look like it was a member method.

So in the handler, you have to cast *self* into the type of your state, not a big deal.
You can still access the non public members if the function is declared into the class.

    class TestState3 : public StateBase
    {
    public:

        void OnInitialized() override
        {
            RegisterActionHandler(TestActionTokens::GoToState1Action, OnGotoState1);
        }

        static void OnGotoState1(StateBase* self, StateData* data, TransitionInfo* result)
        {
            TestState3* that = (TestState3*)self;
            that->test = 1; // <- access allowed

            result->TargetStateToken = TestStateTokens::StateToken1;
        }

    private:

        int test = 0;
    };
