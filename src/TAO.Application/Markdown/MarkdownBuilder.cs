namespace TAO.Application.Markdown;

using System.Text;

public sealed class MarkdownBuilder
{
    private readonly StringBuilder _builder = new();

    public MarkdownBuilder Heading(string text, int level = 1)
    {
        _builder.AppendLine($"{new string('#', level)} {text}");
        _builder.AppendLine();

        return this;
    }

    public MarkdownBuilder Paragraph(string text)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            _builder.AppendLine(text);
            _builder.AppendLine();
        }

        return this;
    }

    public MarkdownBuilder BulletList(IEnumerable<string> items)
    {
        foreach (var item in items.Where(x => !string.IsNullOrWhiteSpace(x)))
        {
            _builder.AppendLine($"- {item}");
        }

        _builder.AppendLine();

        return this;
    }

    public MarkdownBuilder Separator()
    {
        _builder.AppendLine("---");
        _builder.AppendLine();

        return this;
    }

    public MarkdownDocument Build()
    {
        return new MarkdownDocument
        {
            Content = _builder.ToString().Trim()
        };
    }
}