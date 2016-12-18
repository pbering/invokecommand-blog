using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CommonMark;

namespace Blog.Data.Markdown
{
    public class MarkdownFile
    {
        private readonly FileInfo _file;

        public MarkdownFile(FileInfo file)
        {
            _file = file;

            Headers = new Dictionary<string, string>();
            Name = _file.Name.Replace(file.Extension, "");
        }

        public string Name { get; internal set; }
        public string Body { get; internal set; }
        public Dictionary<string, string> Headers { get; }

        public void Parse()
        {
            using (TextReader reader = new StreamReader(_file.OpenRead(), Encoding.UTF8))
            {
                var headerDone = false;
                var line = reader.ReadLine();

                if (string.IsNullOrEmpty(line))
                {
                    throw new ParseException("Empty line");
                }

                if (!line.Equals("---"))
                {
                    throw new ParseException("Invalid first line");
                }

                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Equals("---"))
                    {
                        headerDone = true;

                        break;
                    }

                    var lineArray = line.Split(':');

                    if (lineArray.Length < 2)
                    {
                        throw new InvalidHeaderException("No : found");
                    }

                    if (lineArray.Length > 2)
                    {
                        throw new InvalidHeaderException("More than one : found");
                    }

                    Headers.Add(lineArray[0].Trim().ToLowerInvariant(), lineArray[1].Trim());
                }

                if (!headerDone && ((StreamReader)reader).EndOfStream)
                {
                    throw new ParseException("Headers not parsed yet but we are at the end of the stream");
                }

                if (headerDone && !Headers.Any())
                {
                    throw new InvalidHeaderException("No headers was found");
                }

                Body = CommonMarkConverter.Convert(reader.ReadToEnd(), CommonMarkSettings.Default);
            }
        }
    }
}