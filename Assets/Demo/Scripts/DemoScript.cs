using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityAndroidOpenUrl;

public class DemoScript : MonoBehaviour
{
    private const string cacheFolderName = "CachedPDFs";
    private const string pdfUrl = "https://resources.betterkids.education/BackToSchool/1.%20Back%20to%20School/1.%20First%20Days%20of%20School/0.%20Community%20Bingo_free.pdf";

    private string templatePDFName;
    private static string pathToCacheFolder;
    private static string pathToTemplatePDF;

    public void Execute()
    {
        pathToCacheFolder = Path.Combine(Application.temporaryCachePath, cacheFolderName);
        templatePDFName = Path.GetFileName(pdfUrl);
        Debug.Log($"NAME: {templatePDFName}");
        pathToTemplatePDF = Path.Combine(pathToCacheFolder, templatePDFName);

        if (!Directory.Exists(pathToCacheFolder))
        {
            Directory.CreateDirectory(pathToCacheFolder);
        }

        StartCoroutine(CheckCacheAndDownloadPDF());
    }

    private IEnumerator CheckCacheAndDownloadPDF()
    {
        Debug.Log($"PATH: {pathToTemplatePDF}");
        // Check if the file already exists in the cache
        if (File.Exists(pathToTemplatePDF))
        {
            Debug.Log("PDF found in cache, opening it.");
            AndroidOpenUrl.OpenFile(pathToTemplatePDF);
        }
        else
        {
            Debug.Log("PDF not found in cache, downloading it.");
            UnityWebRequest request = UnityWebRequest.Get(pdfUrl);
            request.SendWebRequest();

            while (!request.isDone)
            {
                // Log the download progress
                Debug.Log($"Download Progress: {request.downloadProgress * 100:F2}%");
                yield return null;
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error while downloading the PDF: " + request.error);
                yield break;
            }

            // Save the downloaded PDF to cache
            File.WriteAllBytes(pathToTemplatePDF, request.downloadHandler.data);
            Debug.Log("Download complete, opening PDF.");
            AndroidOpenUrl.OpenFile(pathToTemplatePDF);
        }
    }
}
