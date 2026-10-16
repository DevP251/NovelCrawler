using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NovelCrawler
{
    public class NovelCrawlerService
    {
        private readonly HttpClient _httpClient;

        public NovelCrawlerService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        }

        public async Task<StoryInfo> ScrapeStoryAsync(string storyUrl, IProgress<string> progress)
        {
            progress?.Report("Đang tải thông tin truyện...");
            var html = await _httpClient.GetStringAsync(storyUrl);

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var story = new StoryInfo();

            // Lấy chính xác tiêu đề từ thẻ <h3 class="text-center story-name">
            var titleNode = doc.DocumentNode.SelectSingleNode("//h3[contains(@class, 'story-name')]")
                         ?? doc.DocumentNode.SelectSingleNode("//*[contains(@class, 'story-name')]")
                         ?? doc.DocumentNode.SelectSingleNode("//meta[@property='og:title']");

            if (titleNode != null)
            {
                string rawTitle = titleNode.Name == "meta"
                    ? titleNode.GetAttributeValue("content", "")
                    : titleNode.InnerText;

                story.Title = WebUtility.HtmlDecode(rawTitle).Trim();
            }
            else
            {
                story.Title = "Truyen_Chua_Dat_Ten";
            }

            progress?.Report($"Đã tìm thấy truyện: {story.Title}");

            // Trích xuất danh sách chương
            var chapterNodes = doc.DocumentNode.SelectNodes("//a[contains(@href, '/chuong-')]")
                            ?? doc.DocumentNode.SelectNodes("//ul[contains(@class, 'list-chapter')]//a");

            if (chapterNodes != null)
            {
                var uriBase = new Uri(storyUrl);
                foreach (var node in chapterNodes)
                {
                    string href = node.GetAttributeValue("href", "");
                    string title = WebUtility.HtmlDecode(node.InnerText?.Trim());

                    if (!string.IsNullOrEmpty(href))
                    {
                        string fullUrl = new Uri(uriBase, href).AbsoluteUri;
                        story.Chapters.Add(new ChapterInfo { Title = title, Url = fullUrl });
                    }
                }
            }

            progress?.Report($"Tổng số chương tìm thấy: {story.Chapters.Count}");
            return story;
        }

        public async Task CrawlChapterContentAsync(ChapterInfo chapter)
        {
            var html = await _httpClient.GetStringAsync(chapter.Url);
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var contentNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'chapter-c')]")
                           ?? doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'chapter-content')]")
                           ?? doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'content')]")
                           ?? doc.DocumentNode.SelectSingleNode("//div[@id='chapter-content']");

            if (contentNode != null)
            {
                // Loại bỏ các thẻ quảng cáo/script
                var unwantedNodes = contentNode.SelectNodes(".//script|.//style|.//iframe|.//div[contains(@class, 'ads')]");
                if (unwantedNodes != null)
                {
                    foreach (var un in unwantedNodes) un.Remove();
                }

                string rawHtml = contentNode.InnerHtml;

                // Thay các thẻ xuống dòng/đoạn văn thành dấu xuống dòng đơn (\n)
                rawHtml = Regex.Replace(rawHtml, @"(?i)<br\s*/?>", "\n");
                rawHtml = Regex.Replace(rawHtml, @"(?i)</p>", "\n");

                var tempDoc = new HtmlAgilityPack.HtmlDocument();
                tempDoc.LoadHtml(rawHtml);

                string plainText = tempDoc.DocumentNode.InnerText;

                // Giải mã entity HTML
                plainText = WebUtility.HtmlDecode(plainText);

                // Tách từng dòng, loại bỏ dòng trống thừa
                var lines = plainText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                var cleanLines = new List<string>();

                foreach (var line in lines)
                {
                    string trimmed = line.Trim();
                    if (!string.IsNullOrEmpty(trimmed))
                    {
                        cleanLines.Add(trimmed);
                    }
                }

                // Nối các đoạn kề sát nhau
                chapter.Content = string.Join("\n", cleanLines);
            }
            else
            {
                chapter.Content = "[Không lấy được nội dung chương này]";
            }
        }
    }
}