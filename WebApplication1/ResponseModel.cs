namespace WebApplication1
{
    public class ResponseModel
    {
        public bool IsError => !string.IsNullOrWhiteSpace(Error);

        public string Error { get; set; }

        public object Result { get; set; } = null;
    }
}