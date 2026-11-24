namespace Common;

public static class ReplaceNodeExt
{
  extension(ICollection<ReplaceNode> nodes)
  {
    public void Add ((string lf, string rw) t) => nodes.Add(new ReplaceNode(t.lf, t.rw));
    public void Add (string lookFor, string replaceWith) => nodes.Add(new ReplaceNode(lookFor, replaceWith));
  }
  extension(IEnumerable<ReplaceNode> nodes)
  {
    public string Invoke (string input, StringComparison sc) => nodes.Aggregate(input, (text, node) => text = node.ReplaceText(text, sc));
  }
}
