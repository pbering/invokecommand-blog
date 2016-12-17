using System.IO;

namespace Blog.Data.IO
{
    public sealed class File : FileBase
    {
        private readonly FileInfo _file;

        public File(FileInfo file)
        {
            _file = file;

            Name = _file.Name;
            Extension = _file.Extension;
            FullName = _file.FullName;
            Modified = _file.LastWriteTime;
        }

        public override Stream OpenRead()
        {
            return _file.OpenRead();
        }
    }
}