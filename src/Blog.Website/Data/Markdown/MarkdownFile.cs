using Markdig;
using System.Text;

namespace Blog.Website.Data.Markdown;

public class MarkdownFile
{
    private readonly FileInfo _file;

    public MarkdownFile(FileInfo file)
    {
        _file = file;

        Name = _file.Name.Replace(file.Extension, "");
        Fields = [];
    }

    public string Name { get; }
    public string Body { get; set; } = string.Empty;
    public Dictionary<string, string> Fields { get; }

    public void Parse(MarkdownPipeline markdownPipeline)
    {
        using TextReader reader = new StreamReader(_file.OpenRead(), Encoding.UTF8);
        var done = false;
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
                done = true;

                break;
            }

            var lineArray = line.Split(':');

            if (lineArray.Length < 2)
            {
                throw new InvalidFieldException("No : found");
            }

            if (lineArray.Length > 2)
            {
                throw new InvalidFieldException("More than one : found");
            }

            Fields.Add(lineArray[0].Trim().ToLowerInvariant(), lineArray[1].Trim());
        }

        if (!done && ((StreamReader)reader).EndOfStream)
        {
            throw new ParseException("Fields not parsed yet but we are at the end of the stream");
        }

        if (done && !Fields.Any())
        {
            throw new InvalidFieldException("No fields was found");
        }

        Body = Markdig.Markdown.ToHtml(reader.ReadToEnd(), markdownPipeline);
    }
}