namespace ContactBook
{
    public class ConfirmDeleteEventArgs : EventArgs
    {
        public string Message { get; }
        public bool Result { get; set; }

        public ConfirmDeleteEventArgs(string message)
        {
            Message = message;
        }
    }
}
