using System;
using System.Collections.Generic;
using System.IO;
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
            log.LogInformation($"Starting {nameof(ProcessComunicadoTecnico)} at: {DateTime.Now}");
            await ProcessComunicadoTecnico();
            log.LogInformation($"Starting {nameof(ProcessDatosCsv)} at: {DateTime.Now}");
            await ProcessDatosCsv();
        }

        private static async Task ProcessComunicadoTecnico()
        {
            var uriWebPage = new Uri("https://www.gob.mx/salud/documentos/coronavirus-covid-19-comunicado-tecnico-diario-238449");
            var html = await WebUtils.DownoadWebPage(uriWebPage);
            var uris = WebUtils.FindUrisForFilesInWebPage(".pdf", html, uriWebPage);
            await WebUtils.DownloadAndSaveFiles(uris,
                fn => $"{fn.Substring(0, fn.IndexOf("_COVID"))}{Path.GetExtension(fn)}",
                async (data, fileName) => await SaveHistoryAndLast(data, fileName, "", "application/pdf"));
        }

        private static async Task ProcessDatosCsv()
        {
            var uriWebPage = new Uri("https://www.gob.mx/salud/documentos/datos-abiertos-152127");
            var html = await WebUtils.DownoadWebPage(uriWebPage);
            var uris = WebUtils.FindUrisForFilesInWebPage(".zip", html, uriWebPage)
                .Where(u => (new[] { "datos_abiertos_covid19.zip", "diccionario_datos_covid19.zip" }).Contains(u.Segments.Last()));
            await WebUtils.DownloadAndSaveFiles(uris, null,
                async (data, fileName) => await SaveHistoryAndLast(data, fileName, "csv/", "application/x-zip-compressed"));
        }

        private static async Task SaveHistoryAndLast(byte[] data, string fileName, string prefixForBlobName, string contentType)
        {
            var fecha = DateTime.UtcNow;
            var blobName1 = $"{prefixForBlobName}history/{fecha.Month:D2}/{fecha.Day:D2}/{fileName}";
            var blobName2 = $"{prefixForBlobName}last/{fileName}";
            var storage = new StorageUtils("-SAS en frmato URI con acceso de escritura al Blob donde se descargarán los documentos-");
            await storage.InsertBlobAsync(blobName1, data, contentType);
            await storage.InsertBlobAsync(blobName2, data, contentType);
        }


    }
}
