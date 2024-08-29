namespace UBlog.Models
{
    public class Post
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastEditedAt { get; set; }

        public Post()
        {
            Id = -1;
            Title = string.Empty;
            Body = string.Empty;
            CreatedAt = DateTime.Now;
            LastEditedAt = DateTime.Now;
        }

        public Post(int id, string title, string body, DateTime createdAt, DateTime lastEditedAt)
        {
            Id = id;
            Title = title;
            Body = body;
            CreatedAt = createdAt;
            LastEditedAt = lastEditedAt;
        }

        public bool IsSaved => Id != -1;
        public bool IsValid => !String.IsNullOrEmpty(Title) && !String.IsNullOrEmpty(Body);
    }
}
