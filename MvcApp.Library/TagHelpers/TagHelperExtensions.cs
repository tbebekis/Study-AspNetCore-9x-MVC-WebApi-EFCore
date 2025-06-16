namespace MvcApp.Library
{
    /// <summary>
    /// Extensions
    /// </summary>
    static public class TagHelperExtensions
    {
        /// <summary>
        /// Returns the encoded html text of a tag
        /// </summary>
        static public string ToHtml(this TagBuilder Tag)
        {
            using (var writer = new StringWriter())
            {
                if (Tag is HtmlTag)
                    (Tag as HtmlTag).WriteTo(writer, HtmlEncoder.Default);
                else
                    Tag.WriteTo(writer, HtmlEncoder.Default);
                return writer.ToString();
            }
        }
        /// <summary>
        /// Returns the inner html of a specified tag as string
        /// </summary>
        static public string InnerHtmlToString(this TagBuilder Tag)
        {
            using (var writer = new StringWriter())
            {
                Tag.InnerHtml.WriteTo(writer, HtmlEncoder.Default);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Writes the tag by encoding it with the specified encoder to the specified  writer.
        /// </summary>
        static public void WriteTo(this TagBuilder Tag, TextWriter writer, HtmlEncoder encoder, TagRenderMode? tagRenderMode = null)
        {
            TagRenderMode TagRenderMode = tagRenderMode.HasValue ? tagRenderMode.Value : Tag.TagRenderMode;

            switch (TagRenderMode)
            {
                case TagRenderMode.StartTag:
                    writer.Write("<");
                    writer.Write(Tag.TagName);
                    Tag.WriteAttributesTo(writer, encoder);
                    writer.Write(">");
                    break;
                case TagRenderMode.EndTag:
                    writer.Write("</");
                    writer.Write(Tag.TagName);
                    writer.Write(">");
                    break;
                case TagRenderMode.SelfClosing:
                    writer.Write("<");
                    writer.Write(Tag.TagName);
                    Tag.WriteAttributesTo(writer, encoder);
                    writer.Write(" />");
                    break;
                default:
                    writer.Write("<");
                    writer.Write(Tag.TagName);
                    Tag.WriteAttributesTo(writer, encoder);
                    writer.Write(">");
                    if (Tag.InnerHtml != null)
                    {
                        Tag.InnerHtml.WriteTo(writer, encoder);
                    }

                    if (Tag is HtmlTag)
                    {
                        if ((Tag as HtmlTag).InnerTags.Count > 0)
                        {
                            (Tag as HtmlTag).InnerTags.ForEach(item => item.WriteTo(writer, encoder));
                        }
                    }

                    writer.Write("</");
                    writer.Write(Tag.TagName);
                    writer.Write(">");
                    break;
            }
        }
        /// <summary>
        /// Writes tag's attributes by encoding them with the specified encoder to the specified  writer.
        /// </summary>
        static public void WriteAttributesTo(this TagBuilder Tag, TextWriter writer, HtmlEncoder encoder)
        {
            if (Tag.Attributes != null && Tag.Attributes.Count > 0)
            {
                foreach (var attribute in Tag.Attributes)
                {
                    var key = attribute.Key;
                    if (string.Equals(key, "id", StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(attribute.Value))
                    {
                        continue;
                    }

                    writer.Write(" ");
                    writer.Write(key);
                    writer.Write("=\"");
                    if (attribute.Value != null)
                    {
                        encoder.Encode(writer, attribute.Value);
                    }

                    writer.Write("\"");
                }
            }
        }
 
        /// <summary>
        /// Removes all css classes from a specified tag
        /// </summary>
        static public void RemoveCssClasses(this TagBuilder Tag)
        {
            if (Tag.Attributes != null && Tag.Attributes.Count > 0 && Tag.Attributes.ContainsKey("class"))
            {
                Tag.Attributes.Remove("class");
            }
        }
        /// <summary>
        /// Removes all the data-val-* attributes Asp.Net Core sets to a specified tag.
        /// </summary>
        static public void RemoveClientDataValidationAttributes(this TagBuilder Tag)
        {
            if (Tag.Attributes != null && Tag.Attributes.Count > 0)
            {
                List<KeyValuePair<string, string>> List = new List<KeyValuePair<string, string>>();
                foreach (var Entry in Tag.Attributes)
                {
                    if (Entry.Key.StartsWith("data-val-") || Entry.Key == "data-val")
                        List.Add(Entry);
                }

                foreach (var Item in List)
                    Tag.Attributes.Remove(Item);

            }
        }

        /// <summary>
        /// Clones a source tag and returns the new <see cref="HtmlTag"/>
        /// </summary>
        static public HtmlTag Clone(this TagBuilder Source)
        {
            HtmlTag Result = new HtmlTag(Source.TagName);
            Result.TagRenderMode = Source.TagRenderMode;

            if (Source.Attributes != null && Source.Attributes.Count > 0)
            {
                foreach (var A in Source.Attributes)
                {
                    Result.Attribute(A.Key, A.Value);
                }
            }

            Source.InnerHtml.CopyTo(Result.InnerHtml);

            if (Source is HtmlTag)
            {
                foreach (var SourceInnerTag in (Source as HtmlTag).InnerTags)
                {
                    Result.Add(SourceInnerTag.Clone());
                }
            }


            return Result;
        }
    }
}
