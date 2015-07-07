﻿using System;

namespace Blogger.DataSource.Model
{
    public class BlogInfo
    {
        /// <summary>
        /// The unique key to identify the blog.
        /// </summary>
        public string BlogKey { get; set; }

        /// <summary>
        /// Checksum of the blog-info.
        /// </summary>
        public string Checksum { get; set; }

        /// <summary>
        /// The alternative sub-title for the blog.
        /// </summary>
        public string Subtitle { get; set; }

        /// <summary>
        /// The main title for the blog.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The date of last update of the blog.
        /// </summary>
        public DateTime? Updated { get; set; }

        /// <summary>
        /// The URL to the blog.
        /// </summary>
        public string Url { get; set; }
    }
}