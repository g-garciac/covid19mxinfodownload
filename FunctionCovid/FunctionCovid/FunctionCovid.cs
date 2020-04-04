using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Utils;

namespace FunctionCovid
{
    public static class FunctionCovid
    {
        //0 10 5 * * * Ejecutarse cada día a las 5:10AM (UTC)
        [FunctionName("FunctionCovid")]
        public static async Task RunAsync([TimerTrigger("0 10 5 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            var storage = new StorageUtils("-SAS en frmato URI con acceso de escritura al Blob donde se descargarán los documentos-");
            var client = new HttpClient();
            var req = await client.GetAsync("https://www.gob.mx/salud/documentos/coronavirus-covid-19-comunicado-tecnico-diario-238449");
            if (req.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = await req.Content.ReadAsStringAsync();
                var r = new Regex("href\\s*=\\s*(?:\"(?<1>[^\"]*)\"|(?<1>\\S+))", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                for (var m = r.Match(data); m.Success; m = m.NextMatch())
                {
                    var uri = m.Groups[1].Value;
                    if (uri.EndsWith(".pdf"))
                    {
                        var reqUri = req.RequestMessage.RequestUri;
                        var uripdf = !uri.Contains("http") ? $"{reqUri.Scheme}://{reqUri.Host}{uri}" : "";
                        var client2 = new HttpClient();
                        var req2 = await client2.GetAsync(uripdf);
                        if (req2.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var pdf = await req2.Content.ReadAsByteArrayAsync();
                            var fecha = DateTime.UtcNow;
                            var pdfName = req2.RequestMessage.RequestUri.Segments.Last();
                            var pdfNameOnly = pdfName.Substring(0, pdfName.IndexOf("_COVID"));
                            var name = $"history/{fecha.Month:D2}/{fecha.Day:D2}/{pdfNameOnly}.pdf";
                            var nameLast = $"last/{pdfNameOnly}.pdf";
                            await storage.InsertBlobAsync(name, pdf, "application/pdf");
                            await storage.InsertBlobAsync(nameLast, pdf, "application/pdf");
                        }
                    }
                }
            }
        }
    }
}
