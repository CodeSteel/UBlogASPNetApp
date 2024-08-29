using MySql.Data.MySqlClient;
using UBlog.Models;

namespace UBlog.Contexts
{
    public class PostContext
    {
        public string ConnectionString { get; set; }

        public PostContext(string connString)
        {
            this.ConnectionString = connString;
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public List<Post> GetAllPosts(SortingOrder? idSortOrder, SortingOrder? createdAtSortOrder, SortingOrder? lastEditedSortingOrder)
        {
            List<Post> posts = new List<Post>();

            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM Post", conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            posts.Add(new Post(reader.GetInt32("id"), reader.GetString("title"), reader.GetString("body"), reader.GetDateTime("createdAt"), reader.GetDateTime("lastEditedAt")));
                        }
                    }
                }
            } catch (Exception _)
            {
                return new List<Post>();
            }

            if (idSortOrder != null)
            {
                switch (idSortOrder)
                {
                    case SortingOrder.Ascending:
                        posts = posts.OrderBy(post => post.Id).ToList();
                        break;
                    case SortingOrder.Descending:
                        posts = posts.OrderByDescending(post => post.Id).ToList();
                        break;
                }
            }

            if (createdAtSortOrder != null)
            {
                switch (createdAtSortOrder)
                {
                    case SortingOrder.Ascending:
                        posts = posts.OrderBy(post => post.CreatedAt).ToList();
                        break;
                    case SortingOrder.Descending:
                        posts = posts.OrderByDescending(post => post.CreatedAt).ToList();
                        break;
                }
            }

            if (lastEditedSortingOrder != null)
            {
                switch (lastEditedSortingOrder)
                {
                    case SortingOrder.Ascending:
                        posts = posts.OrderBy(post => post.LastEditedAt).ToList();
                        break;
                    case SortingOrder.Descending:
                        posts = posts.OrderByDescending(post => post.LastEditedAt).ToList();
                        break;
                }
            }

            return posts;
        }

        public Post GetPostById(int id)
        {
            Post post = new Post();
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand($"SELECT * FROM Post WHERE Id = {id}", conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        post = new Post(reader.GetInt32("id"), reader.GetString("title"), reader.GetString("body"), reader.GetDateTime("createdAt"), reader.GetDateTime("lastEditedAt"));
                    }
                }
                cmd.ExecuteNonQuery();
            }
            return post;
        }

        public void SavePost(Post post)
        {
            post.LastEditedAt = DateTime.Now;

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd;
                if (post.IsSaved)
                {
                    cmd = new MySqlCommand($"UPDATE Post SET title = '{post.Title}', body = '{post.Body}', lastEditedAt = '{post.LastEditedAt.ToString("yyyy-MM-dd HH:mm")}' WHERE Id = '{post.Id}'", conn);

                } else
                {
                    cmd = new MySqlCommand($"INSERT INTO Post(title,body,createdAt,lastEditedAt) VALUES('{post.Title}', '{post.Body}', '{post.CreatedAt.ToString("yyyy-MM-dd HH:mm")}', '{post.LastEditedAt.ToString("yyyy-MM-dd HH:mm")}')", conn);
                }
                cmd.ExecuteNonQuery();
            }
        }

        public void DeletePost(int id)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand($"DELETE FROM Post WHERE Id = {id}", conn);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
