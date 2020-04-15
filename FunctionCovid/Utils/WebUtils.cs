using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Utils
{
    public class WebUtils
    {
        public static List<Uri> FindUrisForFilesInWebPage(string extension, string htmlWebData, Uri uriWebPage) =>
            new Regex("href\\s*=\\s*(?:\"(?<1>[^\"]*)\"|(?<1>\\S+))", RegexOptions.IgnoreCase | RegexOptions.Compiled)
            .Matches(htmlWebData).Cast<Match>()
            .Select(m => m.Groups[1].Value)
            .Where(u => u.EndsWith(extension))
            .Select(u => u.StartsWith("http") ? u : !u.Contains("http") ? $"{uriWebPage.Scheme}://{uriWebPage.Host}{u}" : "")
            .Select(u => new Uri(u)).ToList();

        public static async Task<string> DownoadWebPage(Uri uri)
        {
            var response = await new HttpClient().GetAsync(uri);
            return response.StatusCode == System.Net.HttpStatusCode.OK ?
                await response.Content.ReadAsStringAsync()
                : string.Empty;
        }

        public static async Task DownloadAndSaveFiles(IEnumerable<Uri> uris, Func<string, string> transformFileName, Action<byte[], string> saveFile)
        {
            foreach (var uri in uris)
            {
                var response = await new HttpClient().GetAsync(uri);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var data = await response.Content.ReadAsByteArrayAsync();
                    var originalFileName = response.RequestMessage.RequestUri.Segments.Last();
                    var finalFileName = transformFileName is null ? originalFileName
                        : transformFileName(originalFileName);
                    saveFile?.Invoke(data, finalFileName);
                }
            }
        }
    }
}
