namespace ESC.WebApp.Models
{
    public class ApiResponse<T>
    {
        public bool ok;
        public T value;
        public string correlationId;
        public string message;
    }
}
