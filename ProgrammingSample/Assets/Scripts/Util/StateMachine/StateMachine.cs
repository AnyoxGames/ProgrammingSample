namespace AnyoxGames.Util
{
    public class StateMachine<T>
    {
        private IStateMachineBehaviour<T> currentState;
        private IStateMachineBehaviour<T> overrideState;
        public bool IsInState => currentState != null;

        private readonly T target;

        public StateMachine(T target)
        {
            this.target = target;
        }

        public void SetState(IStateMachineBehaviour<T> state)
        {
            currentState?.ExitState(target);
            currentState = state;
            currentState?.EnterState(target);
        }

        public void ClearOverrideState()
        {
            overrideState?.ExitState(target);
            overrideState = null;
        }

        public void SetOverrideState(IStateMachineBehaviour<T> state)
        {
            overrideState?.ExitState(target);
            overrideState = state;
            overrideState?.EnterState(target);
        }

        public void UpdateState()
        {
            if (overrideState != null)
            {
                overrideState.UpdateState(target);
                return;
            }

            currentState?.UpdateState(target);
        }
    }
}