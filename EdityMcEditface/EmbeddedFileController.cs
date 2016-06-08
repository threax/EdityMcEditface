using CommonMark;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace Edity.McEditface
{
    public class EmbeddedFileController : ApiController
    {
        private static Assembly assembly = Assembly.GetExecutingAssembly();
        private static CommonMarkContentTypeProvider contentTypeProvider = new CommonMarkContentTypeProvider();

        public HttpResponseMessage Get(String file)
        {
            try
            {
                var mime = contentTypeProvider.Mappings[Path.GetExtension(file).ToLowerInvariant()];
                using (var stream = loadResource(file))
                {
                    if(stream == null)
                    {
                        throw new FileNotFoundException();
                    }
                    return this.streamResponse(stream, mime);
                }
            }
            catch(FileNotFoundException)
            {
                return this.statusCodeResponse(HttpStatusCode.NotFound);
            }
            catch(Exception)
            {
                return this.statusCodeResponse(HttpStatusCode.InternalServerError);
            }
        }

        private Stream loadResource(String name)
        {
            return assembly.GetManifestResourceStream(makeTemplateName(name));
        }

        private String makeTemplateName(String name)
        {
            return $"Edity.McEditface.Resources.{this.escapeTemplate(name)}";
        }
    }
}