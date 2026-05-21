namespace AnyoxGames.Util
{
    public class StateMachine<T>
    {
        private IStateMachineBehaviour<T> CurrentState;
        private IStateMachineBehaviour<T> OverrideState;
        public bool IsInState => CurrentState != null;

        public T Target;

        public StateMachine(T target)
        {
            Target = target;
        }

        public void SetState(IStateMachineBehaviour<T> state)
        {
            CurrentState?.ExitState(Target);
            CurrentState = state;
            CurrentState?.EnterState(Target);
        }

        public void ClearOverrideState()
        {
            OverrideState?.ExitState(Target);
            OverrideState = null;
        }

        public void SetOverrideState(IStateMachineBehaviour<T> state)
        {
            OverrideState?.ExitState(Target);
            OverrideState = state;
            OverrideState?.EnterState(Target);
        }

        public void UpdateState()
        {
            UpdateState(out _);
        }

        public void UpdateState(out IStateMachineBehaviour<T> currentState)
        {
            if (OverrideState != null)
            {
                currentState = OverrideState;
                OverrideState.UpdateState(Target);
                return;
            }

            currentState = CurrentState;
            CurrentState?.UpdateState(Target);
        }
    }
}