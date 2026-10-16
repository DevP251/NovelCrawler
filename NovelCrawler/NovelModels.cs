using System.Collections.Generic;

namespace NovelCrawler
{
    public class ChapterInfo
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Content { get; set; }
    }

    public class StoryInfo
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public List<ChapterInfo> Chapters { get; set; } = new List<ChapterInfo>();
    }
}