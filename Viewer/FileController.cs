using CommonMark;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Viewer.TemplateFormatter;
using Viewer.TemplateFormatter.HtmlRenderers;

namespace OwinSelfhostSample
{
    public class FileController : ApiController
    {
        public HttpResponseMessage Get(String file)
        {
            bool parse = true;

            if(file.EndsWith(".text", StringComparison.InvariantCultureIgnoreCase))
            {
                file = file.Substring(0, file.Length - 5) + ".md";
                parse = false;
            }
            else if (!Path.GetExtension(file).Equals(".md", StringComparison.InvariantCultureIgnoreCase))
            {
                var autoFile = file + ".md";
                if (File.Exists(autoFile))
                {
                    file = autoFile;
                }
                else
                {
                    return statusCodeResponse(HttpStatusCode.NotFound);
                }
            }

            var identifier = new DefaultHtmlTagIdentiifer();
            var renderers = new TemplatedHtmlRenderer();
            renderers.openDoc("template.html");
            var tagMap = new HtmlTagMap(renderers.getRenderer);
            CommonMarkSettings.Default.OutputDelegate = (doc, output, settings) => new FileTemplateHtmlFormatter(tagMap, identifier, output, settings).WriteDocument(doc);

            try
            {
                String content;
                using (var reader = new StreamReader(file))
                {
                    if(parse)
                    {
                        using (var writer = new StringWriter())
                        {
                            CommonMarkConverter.Convert(reader, writer);
                            content = writer.ToString();
                        }
                    }
                    else
                    {
                        content = $"<html><head></head><body><pre>{HttpUtility.HtmlEncode(reader.ReadToEnd())}</pre></body></html>";
                    }
                }

                return htmlResponse(content);
            }
            catch(FileNotFoundException)
            {
                return statusCodeResponse(HttpStatusCode.NotFound);
            }
            catch(Exception)
            {
                return statusCodeResponse(HttpStatusCode.InternalServerError);
            }
        }

        public HttpResponseMessage statusCodeResponse(HttpStatusCode code)
        {
            var response = new HttpResponseMessage();
            response.StatusCode = code;
            return response;
        }

        public HttpResponseMessage htmlResponse(String content, String mimeType = "text/html")
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent(content);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
            return response;
        }
    }
}