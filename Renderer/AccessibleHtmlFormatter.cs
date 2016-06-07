using CommonMark;
using CommonMark.Formatters;
using CommonMark.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkTools.Renderer
{
    public class AccessibleHtmlFormatter : HtmlFormatter
    {
        public AccessibleHtmlFormatter(TextWriter target, CommonMarkSettings settings)
            :base(target, settings)
        {

        }

        //
        // Summary:
        //     Ensures that the output ends with a newline. This means that newline character
        //     will be written only if the writer does not currently end with a newline.
        public void ensureNewLine()
        {
            this.EnsureNewLine();
        }
        //
        // Summary:
        //     Writes the specified text to the target writer.
        public void write(string text)
        {
            this.Write(text);
        }
        //
        // Summary:
        //     Writes the specified text to the target writer.
        public void write(StringContent text)
        {
            this.Write(text);
        }
        //
        // Summary:
        //     Writes the specified character to the target writer.
        public void write(char c)
        {
            this.Write(c);
        }
        //
        // Summary:
        //     Encodes the given text with HTML encoding (ampersand-encoding) and writes the
        //     result to the target writer.
        public void writeEncodedHtml(StringContent text)
        {
            this.WriteEncodedHtml(text);
        }
        //
        // Summary:
        //     Encodes the given text with HTML encoding (ampersand-encoding) and writes the
        //     result to the target writer.
        public void writeEncodedHtml(string text)
        {
            this.WriteEncodedHtml(text);
        }
        //
        // Summary:
        //     Encodes the given text with URL encoding (percent-encoding) and writes the result
        //     to the target writer. Note that the result is intended to be written to HTML
        //     attribute so this also encodes & character as &amp;.
        public void writeEncodedUrl(string url)
        {
            this.WriteEncodedUrl(url);
        }
        //
        // Summary:
        //     Writes a newline to the target writer.
        public void writeLine()
        {
            this.WriteLine();
        }
        //
        // Summary:
        //     Writes the specified text and a newline to the target writer.
        public void writeLine(string text)
        {
            this.WriteLine(text);
        }
        //
        // Summary:
        //     Writes a data-sourcepos="start-end" attribute to the target writer. This method
        //     should only be called if CommonMark.CommonMarkSettings.TrackSourcePosition is
        //     set to true. Note that the attribute is preceded (but not succeeded) by a single
        //     space.
        public void writePositionAttribute(Block block)
        {
            this.WritePositionAttribute(block);
        }
        //
        // Summary:
        //     Writes a data-sourcepos="start-end" attribute to the target writer. This method
        //     should only be called if CommonMark.CommonMarkSettings.TrackSourcePosition is
        //     set to true. Note that the attribute is preceded (but not succeeded) by a single
        //     space.
        public void writePositionAttribute(Inline inline)
        {
            this.WritePositionAttribute(inline);
        }

        public CommonMarkSettings TheSettings
        {
            get
            {
                return this.Settings;
            }
        }

        public Stack<bool> TheRenderTightParagraphs
        {
            get
            {
                return this.RenderTightParagraphs;
            }
        }

        public Stack<bool> TheRenderPlainTextInlines
        {
            get
            {
                return this.RenderPlainTextInlines;
            }
        }
    }
}
