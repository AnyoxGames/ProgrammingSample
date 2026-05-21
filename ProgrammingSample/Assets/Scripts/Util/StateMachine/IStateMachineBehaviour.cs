namespace AnyoxGames.Util
{
    public interface IStateMachineBehaviour<in TU>
    {
        public void EnterState(TU target);
        public void UpdateState(TU target);
        public void ExitState(TU target);
    }
}