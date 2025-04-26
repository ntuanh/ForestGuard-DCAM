namespace System.Mvc
{
    public partial interface IView
    {
        void Render(object model);
        object Content { get; }
    }

    public interface IAsyncView
    {
    }
}
