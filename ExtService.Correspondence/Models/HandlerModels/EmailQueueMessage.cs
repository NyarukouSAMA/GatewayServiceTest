namespace ExtService.Correspondence.Models.HandlerModels
{
    public class EmailQueueMessage
    {
        public string RecipientListSemikolonSeparated { get; set; }
        public string Subject { get; set; }
        public int BodyType { get; set; } = 1;
        public string Message { get; set; }
    }
}
