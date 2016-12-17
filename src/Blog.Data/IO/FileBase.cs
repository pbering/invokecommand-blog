using System;
using System.IO;

namespace Blog.Data.IO
{
    public abstract class FileBase
    {
        public virtual string Name { get; set; }
        public virtual string Extension { get; set; }
        public virtual string FullName { get; set; }
        public virtual DateTime Modified { get; set; }
        public abstract Stream OpenRead();
    }
}