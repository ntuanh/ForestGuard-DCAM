namespace System.Mvc
{
    public class ActionResult
    {
        public IView View { get; set; }
        public Controller Controller { get; set; }
        public bool Handled { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
    }
}
