using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackKeyDemo
{
        public enum UIState
        {
            // <application specific> define required states here
            One,
            Two,
            Three,
            Four
        }

        public enum Command
        {
            // <application specific> define required commands here
            PlusOne,
            PlusTwo,
            MinusOne,
            MinusTwo,
            Reset
        }

        public class UIStateMachine : INotifyPropertyChanged, IBackable
        {
            class StateTransition
            {
                readonly UIState CurrentState;
                readonly Command Command;

                public StateTransition(UIState currentState, Command command)
                {
                    CurrentState = currentState;
                    Command = command;
                }

                public override int GetHashCode()
                {
                    return 19 + 79 * CurrentState.GetHashCode() + 10 * Command.GetHashCode();
                }

                public override bool Equals(object obj)
                {
                    StateTransition other = obj as StateTransition;
                    return other != null && this.CurrentState == other.CurrentState && this.Command == other.Command;
                }
            }

            private Dictionary<StateTransition, UIState> _transitions;
            public UIState CurrentState { get; private set; }
            public bool IsPlusOneEnabled { get; private set; }
            public bool IsPlusTwoEnabled { get; private set; }
            public bool IsMinusOneEnabled { get; private set; }
            public bool IsMinusTwoEnabled { get; private set; }

            private Stack<UIState> _stateStack = new Stack<UIState>();

            // Declare the PropertyChanged event.
            public event PropertyChangedEventHandler PropertyChanged;

            // NotifyPropertyChanged will fire the PropertyChanged event, 
            // passing the source property that is being updated.
            public void NotifyPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
                }
            }

            public UIStateMachine()
            {
                CurrentState = UIState.One;
                NotifyPropertyChanged("CurrentState");                
                _transitions = new Dictionary<StateTransition, UIState>
                {
                    // <application specific> defines possible transitions triggered by commands
                    { new StateTransition(UIState.One, Command.PlusOne), UIState.Two },
                    { new StateTransition(UIState.One, Command.PlusTwo), UIState.Three },
                    { new StateTransition(UIState.Two, Command.MinusOne), UIState.One },
                    { new StateTransition(UIState.Two, Command.PlusOne), UIState.Three },
                    { new StateTransition(UIState.Two, Command.PlusTwo), UIState.Four },
                    { new StateTransition(UIState.Three, Command.MinusOne), UIState.Two },
                    { new StateTransition(UIState.Three, Command.MinusTwo), UIState.One },
                    { new StateTransition(UIState.Three, Command.PlusOne), UIState.Four },
                    { new StateTransition(UIState.Four, Command.MinusOne), UIState.Three },
                    { new StateTransition(UIState.Four, Command.MinusTwo), UIState.Two },
                    { new StateTransition(UIState.One, Command.Reset), UIState.One },
                    { new StateTransition(UIState.Two, Command.Reset), UIState.One },
                    { new StateTransition(UIState.Three, Command.Reset), UIState.One },
                    { new StateTransition(UIState.Four, Command.Reset), UIState.One },
                };
                UpdateProperties();
            }

            public void TransitTo(UIState nextState, bool isMovingBack = false)
            {

                DoTransition(nextState);
                CurrentState = nextState;
                NotifyPropertyChanged("CurrentState");
                UpdateProperties();
            }

            private void DoTransition(UIState nextState)
            {
                // <application specific> render transition
            }

            private void UpdateProperties()
            {
                UIState temp;
                StateTransition transition;

                transition = new StateTransition(CurrentState, Command.MinusOne);
                IsMinusOneEnabled = _transitions.TryGetValue(transition, out temp) ? true : false;
                NotifyPropertyChanged("IsMinusOneEnabled");
                transition = new StateTransition(CurrentState, Command.MinusTwo);
                IsMinusTwoEnabled = _transitions.TryGetValue(transition, out temp) ? true : false;
                NotifyPropertyChanged("IsMinusTwoEnabled");
                transition = new StateTransition(CurrentState, Command.PlusOne);
                IsPlusOneEnabled = _transitions.TryGetValue(transition, out temp) ? true : false;
                NotifyPropertyChanged("IsPlusOneEnabled");
                transition = new StateTransition(CurrentState, Command.PlusTwo);
                IsPlusTwoEnabled = _transitions.TryGetValue(transition, out temp) ? true : false;
                NotifyPropertyChanged("IsPlusTwoEnabled");
            }

            private bool IsBackable(UIState nextState)
            {
                // <application specific> logic between CurrentState and nextState
                return true;
            }

            public UIState NextState(Command command)
            {
                StateTransition transition = new StateTransition(CurrentState, command);
                UIState nextState;
                if (_transitions.TryGetValue(transition, out nextState))
                {
                    return nextState;
                }
                else {
                    return CurrentState;
                }
            }

            public UIState Perform(Command command)
            {
                UIState nextState = NextState(command);
                UpdateBackStack(command,nextState);
                if (!CurrentState.Equals(nextState))
                {
                    TransitTo(nextState);
                }
                return CurrentState;
            }

            private void UpdateBackStack(Command command, UIState nextState){
                // <application specific> 
                if (command.Equals(Command.Reset)){
                    _stateStack.Clear();
                    return;
                }
                if (IsBackable(nextState) && !CurrentState.Equals(nextState))
                {
                    _stateStack.Push(CurrentState);
                }
            }

            public bool GoBack()
            {
                bool ret = false;
                if (_stateStack.Count > 0) {
                    TransitTo(_stateStack.Pop(), true);
                    ret = true;
                }
                return ret;
            }

            public Stack<IBackable> BackStack
            {
                get;
                set;
            }
        }
}
