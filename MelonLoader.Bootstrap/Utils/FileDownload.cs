using System.Diagnostics;

namespace MelonLoader.Bootstrap.RuntimeHandlers.Dotnet;

internal class FileDownload
{
    private readonly string? URL;

    internal FileDownload(string url)
    {
        URL = url;
    }
    
    public (bool, string?) Attempt(string filePath)
        => AttemptAsync(filePath).GetAwaiter().GetResult();
    private async Task<(bool, string?)> AttemptAsync(string filePath)
    {
        var http = new HttpClient();
        http.DefaultRequestHeaders.Add("User-Agent", "MelonLoader");

        var filePathDir = Path.GetDirectoryName(filePath);
        try
        {
            if (!Directory.Exists(filePathDir))
                Directory.CreateDirectory(filePathDir!);
            
            var resp = await http.GetAsync(URL, HttpCompletionOption.ResponseHeadersRead);
            if (!resp.IsSuccessStatusCode)
                return (false, resp.ReasonPhrase);
            
            long? totalBytesNull = resp.Content.Headers.ContentLength;
            long totalBytes = totalBytesNull ?? 0;
            
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

            if (totalBytes <= 0)
            {
                Core.Logger.Msg("No Content Length");
                Core.Logger.Msg("0%");
            }

            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, bytesRead);

                totalBytesRead += bytesRead;
                
                if (totalBytes > 0)
                {
                    int progress = (int)((totalBytesRead * 100L) / totalBytes)!;
                    if (progress != lastProgress)
                    {
                        lastProgress = progress;
                        Core.Logger.Msg(progress + "%");
                    }
                }
            }
            contentStream.Close();
            fileStream.Close();

            if (!resp.IsSuccessStatusCode)
            {
                DeleteFile(filePath);
                return (false, resp.ReasonPhrase);
            }
            
            if (totalBytes <= 0)
                Core.Logger.Msg("100%");
            else if (lastProgress <= 99)
            {
                DeleteFile(filePath);
                return (false, "Failed to download " + filePath);
            }
        }
        catch (Exception ex)
        {
            DeleteFile(filePath);
            return (false, ex.ToString());
        }
        
        return (true, null);
    }

    private void DeleteFile(string filePath)
    {
        if (File.Exists(filePath))
            File.Delete(filePath);
    }
}