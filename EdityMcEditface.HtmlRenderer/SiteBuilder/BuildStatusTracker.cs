using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdityMcEditface.HtmlRenderer.SiteBuilder
{
    /// <summary>
    /// Track the build status. Handles messages in a thread safe way.
    /// </summary>
    public class BuildStatusTracker : IBuildStatusTracker
    {
        private List<String> messages = new List<string>();

        /// <summary>
        /// Add a message.
        /// </summary>
        /// <param name="message">The message to add.</param>
        public void AddMessage(String message)
        {
            lock (messages)
            {
                messages.Add(message);
            }
        }

        /// <summary>
        /// Get a copy of the current messages. Will be safe to return from any thread, won't be modified.
        /// </summary>
        /// <returns>A copy of the messages list.</returns>
        public List<String> GetMessages()
        {
            lock (messages)
            {
                return messages.ToList();
            }
        }
    }
}
