using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace EdityMcEditface.HtmlRenderer
{
    public class PageStackFileNotFoundException : FileNotFoundException
    {
        private List<String> searchLocations;

        public PageStackFileNotFoundException()
        {
        }

        public PageStackFileNotFoundException(string message) : base(message)
        {
        }

        public PageStackFileNotFoundException(string message, string fileName) : base(message, fileName)
        {
        }

        public PageStackFileNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public PageStackFileNotFoundException(string message, string fileName, Exception innerException) : base(message, fileName, innerException)
        {
        }

        protected PageStackFileNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public void AddSearchLocation(String searchLocation)
        {
            if (this.searchLocations == null)
            {
                this.searchLocations = new List<string>();
            }
            this.searchLocations.Add(searchLocation);
        }

        public IEnumerable<String> SearchLocations
        {
            get
            {
                return ReversedSearchLocations();
            }
        }

        private IEnumerable<String> ReversedSearchLocations()
        {
            for(int i = searchLocations.Count - 1; i >= 0; --i)
            {
                yield return searchLocations[i];
            }
        }
    }
}
