using System.Diagnostics;

namespace MelonLoader.Bootstrap.RuntimeHandlers.Dotnet;

internal class FileDownload
{
    public string? URL { get; private set; } = null!;

    internal FileDownload(string url)
    {
        URL = url;
    }
    
    public (bool, HttpResponseMessage?) Attempt(string filePath)
        => AttemptAsync(filePath).GetAwaiter().GetResult();
    private async Task<(bool, HttpResponseMessage?)> AttemptAsync(string filePath)
    {
        var http = new HttpClient();
        http.DefaultRequestHeaders.Add("User-Agent", "MelonLoader");

        var filePathDir = Path.GetDirectoryName(filePath);
        HttpResponseMessage? resp = null;
        try
        {
            if (!Directory.Exists(filePathDir))
                Directory.CreateDirectory(filePathDir!);
            
            resp = await http.GetAsync(URL, HttpCompletionOption.ResponseHeadersRead);
            if (!resp.IsSuccessStatusCode)
                return (false, resp);
            
            long? totalBytes = resp.Content.Headers.ContentLength;
            Stream contentStream = await resp.Content.ReadAsStreamAsync();
            FileStream fileStream = new(
                filePath,
                FileMode.CreateNew,
                FileAccess.ReadWrite,
                FileShare.None,
                bufferSize: 8192,
                useAsync: true);
            
            byte[] buffer = new byte[8192];
            long totalBytesRead = 0;
            int bytesRead;
            int lastProgress = -1;
            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, bytesRead);

                totalBytesRead += bytesRead;
                if (totalBytes is > 0)
                {
                    int progress = (int)((totalBytesRead * 100L) / totalBytes.Value);
                    if (progress != lastProgress)
                    {
                        lastProgress = progress;
                        Core.Logger.Msg(progress + "%");
                    }
                }
            }
            contentStream.Close();
            fileStream.Close();

            if ((totalBytes.HasValue && totalBytesRead != totalBytes.Value)
                || !resp.IsSuccessStatusCode)
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
                return (false, resp);
            }
        }
        catch (Exception ex)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
            throw ex;
        }
        
        return (true, resp);
    }
}
