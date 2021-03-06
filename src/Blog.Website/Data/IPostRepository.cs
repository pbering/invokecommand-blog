using System.Collections.Generic;
using Blog.Website.Models;

namespace Blog.Website.Data
{
    public interface IPostRepository
    {
        IEnumerable<PostModel> Get();
    }
}