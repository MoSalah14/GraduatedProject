namespace OutbornE_commerce.BAL.Dto
{
    public class Response<T>
    {
        public int Status { get; set; }
        public bool IsError { get; set; }
        public string? Message { get; set; }
        public string? MessageAr { get; set; }
        public T? Data { get; set; }
    }
}