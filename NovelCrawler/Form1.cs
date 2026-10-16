using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace NovelCrawler
{
    public partial class Form1 : Form
    {
        private readonly NovelCrawlerService _scraperService;

        public Form1()
        {
            InitializeComponent();
            _scraperService = new NovelCrawlerService();
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            string url = txtUrl.Text.Trim();
            string saveFolder = txtSavePath.Text.Trim();

            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("Vui lòng nhập URL truyện!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(saveFolder) || !Directory.Exists(saveFolder))
            {
                MessageBox.Show("Vui lòng chọn thư mục lưu hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnStart.Enabled = false;
            rtbLog.Clear();

            var progress = new Progress<string>(logMessage =>
            {
                rtbLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {logMessage}\n");
                rtbLog.SelectionStart = rtbLog.Text.Length;
                rtbLog.ScrollToCaret();
            });

            try
            {
                var story = await _scraperService.ScrapeStoryAsync(url, progress);

                if (story.Chapters.Count == 0)
                {
                    ((IProgress<string>)progress).Report("Không tìm thấy chương nào hoặc cấu trúc trang thay đổi.");
                    return;
                }

                progressBar.Minimum = 0;
                progressBar.Maximum = story.Chapters.Count;
                progressBar.Value = 0;

                string safeTitle = string.Join("_", story.Title.Split(Path.GetInvalidFileNameChars()));
                string filePath = Path.Combine(saveFolder, $"{safeTitle}.txt");

                using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    await writer.WriteLineAsync($"TRUYỆN: {story.Title}");
                    await writer.WriteLineAsync(new string('=', 40));
                    await writer.WriteLineAsync();

                    for (int i = 0; i < story.Chapters.Count; i++)
                    {
                        var chap = story.Chapters[i];
                        ((IProgress<string>)progress).Report($"Đang cào: {chap.Title} ({i + 1}/{story.Chapters.Count})...");

                        await _scraperService.CrawlChapterContentAsync(chap);

                        await writer.WriteLineAsync($"=== {chap.Title} ===");
                        await writer.WriteLineAsync(chap.Content);
                        await writer.WriteLineAsync("\n\n");

                        progressBar.Value = i + 1;
                        await Task.Delay(300); // Tránh bị chặn IP
                    }
                }

                ((IProgress<string>)progress).Report($">>> HOÀN THÀNH! Đã lưu tại: {filePath}");
                MessageBox.Show("Cào dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ((IProgress<string>)progress).Report($"[LỖI] {ex.Message}");
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnStart.Enabled = true;
            }
        }

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtSavePath.Text = dialog.SelectedPath;
                }
            }
        }
    }
}