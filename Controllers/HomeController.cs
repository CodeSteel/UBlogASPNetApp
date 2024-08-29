using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UBlog.Contexts;
using UBlog.Models;

namespace UBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            string? idSortString = HttpContext.Session.GetString("IdSortOrder");
            SortingOrder? idSortOrder = string.IsNullOrEmpty(idSortString) ? null : Enum.Parse<SortingOrder>(idSortString);

            string? createdAtSortString = HttpContext.Session.GetString("CreatedAtSortOrder");
            SortingOrder? createdAtSortOrder = string.IsNullOrEmpty(createdAtSortString) ? null : Enum.Parse<SortingOrder>(createdAtSortString);

            string? lastEditedAtSortString = HttpContext.Session.GetString("LastEditedAtSortOrder");
            SortingOrder? lastEditedAtSortOrder = string.IsNullOrEmpty(lastEditedAtSortString) ? null : Enum.Parse<SortingOrder>(lastEditedAtSortString);

            return GetPostContext(out PostContext context) ? View(context.GetAllPosts(idSortOrder, createdAtSortOrder, lastEditedAtSortOrder)) : View();
        }

        public IActionResult CreateEditPost(int? id)
        {
            if (id != null)
            {
                if (!GetPostContext(out PostContext context)) return View();
                var post = context.GetPostById(id.Value);
                return View(post);
            }

            return View();
        }

        #region Post Actions

        public IActionResult SavePost(Post post)
        {
            if (!post.IsValid) return RedirectToAction("Index");

            if (GetPostContext(out PostContext context))
            {
                if (!post.IsSaved)
                {
                    post.CreatedAt = DateTime.Now;
                }
                context.SavePost(post);
            }
            return RedirectToAction("Index");
        }

        public IActionResult DeletePost(int id)
        {
            if (GetPostContext(out PostContext context))
            {
                context.DeletePost(id);
            }
            return RedirectToAction("Index");
        }

        #endregion

        #region Sorting

        public IActionResult ToggleSortOrderById()
        {
            string sortOrder = HttpContext.Session.GetString("IdSortOrder");
            if (string.IsNullOrEmpty(sortOrder))
            {
                HttpContext.Session.SetString("IdSortOrder", SortingOrder.Ascending.ToString());
            }
            else
            {
                HttpContext.Session.SetString("IdSortOrder", Enum.Parse<SortingOrder>(HttpContext.Session.GetString("IdSortOrder")) == SortingOrder.Ascending ? SortingOrder.Descending.ToString() : SortingOrder.Ascending.ToString());
            }
            HttpContext.Session.Remove("CreatedAtSortOrder");
            HttpContext.Session.Remove("LastEditedAtSortOrder");
            return RedirectToAction("Index");
        }

        public IActionResult ToggleSortOrderByCreatedAt()
        {
            string sortOrder = HttpContext.Session.GetString("CreatedAtSortOrder");
            if (string.IsNullOrEmpty(sortOrder))
            {
                HttpContext.Session.SetString("CreatedAtSortOrder", SortingOrder.Ascending.ToString());
            }
            else
            {
                HttpContext.Session.SetString("CreatedAtSortOrder", Enum.Parse<SortingOrder>(HttpContext.Session.GetString("CreatedAtSortOrder")) == SortingOrder.Ascending ? SortingOrder.Descending.ToString() : SortingOrder.Ascending.ToString());
            }
            HttpContext.Session.Remove("IdSortOrder");
            HttpContext.Session.Remove("LastEditedAtSortOrder");
            return RedirectToAction("Index");
        }

        public IActionResult ToggleSortOrderByLastEditedAt()
        {
            string sortOrder = HttpContext.Session.GetString("LastEditedAtSortOrder");
            if (string.IsNullOrEmpty(sortOrder))
            {
                HttpContext.Session.SetString("LastEditedAtSortOrder", SortingOrder.Ascending.ToString());
            }
            else
            {
                HttpContext.Session.SetString("LastEditedAtSortOrder", Enum.Parse<SortingOrder>(HttpContext.Session.GetString("LastEditedAtSortOrder")) == SortingOrder.Ascending ? SortingOrder.Descending.ToString() : SortingOrder.Ascending.ToString());
            }
            HttpContext.Session.Remove("IdSortOrder");
            HttpContext.Session.Remove("CreatedAtSortOrder");
            return RedirectToAction("Index");
        }

        #endregion

        private bool GetPostContext(out PostContext context)
        {
            PostContext? _context = HttpContext.RequestServices.GetService(typeof(PostContext)) as PostContext;
            if (_context == null)
            {
                context = new PostContext(string.Empty);
                return false;
            }
            context = _context;
            return true;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}