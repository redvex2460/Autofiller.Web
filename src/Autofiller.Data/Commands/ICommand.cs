namespace Autofiller.Data.Commands
{
    public interface ICommand<T>
    {
        public T Result { get; set; }

        public ICommand<T> Execute();
    }
}