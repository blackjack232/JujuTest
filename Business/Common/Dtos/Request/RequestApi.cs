namespace Business.Common.Dtos.Request
{
    public class CustomerCreate
    {
        public string Name { get; set; }
    }

    public class CustomerUpdate
    {
        public string Name { get; set; }
    }
    public class PostCreate
    {
        public int CustomerId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int Type { get; set; }
        public string Category { get; set; }

    }
}
