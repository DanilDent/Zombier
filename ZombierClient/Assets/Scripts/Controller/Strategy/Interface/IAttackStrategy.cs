namespace Prototype.Controller
{
    public interface IAttackStrategy
    {
        public void Subscribe();
        public void Execute();
        public void Unsubscribe();
    }
}