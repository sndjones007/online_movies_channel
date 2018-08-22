using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumTest
{
    class HttpDownloadFile
    {
        /// <summary>
        /// The configuration for the class
        /// </summary>
        private HttpRequestConfiguration configuration = new HttpRequestConfiguration();

        /// <summary>
        /// The uri path sent for loading
        /// </summary>
        private Uri uriPath;

        private string localPath;

        public string LocalFile;

        /// <summary>
        /// Default constructor
        /// </summary>
        public HttpDownloadFile(string localPath) {
            this.localPath = localPath;
        }

        /// <summary>
        /// Set the <see cref="WebRequest"/> settings for Http pages
        /// Set the user agent to mimic a web browser
        /// </summary>
        /// <param name="httpWebRequestObj"></param>
        private void HttpWebRequestSettings(WebRequest webRequestObj)
        {
            var httpWebRequestObj = (HttpWebRequest)webRequestObj;
            httpWebRequestObj.Method = configuration.Method;
            httpWebRequestObj.Proxy.Credentials = configuration.Credrntials;
            httpWebRequestObj.AuthenticationLevel = configuration.AuthLevel;
            httpWebRequestObj.ImpersonationLevel = configuration.ImpersonationLevel;
            httpWebRequestObj.KeepAlive = configuration.KeepAlive;
            httpWebRequestObj.UserAgent = configuration.UserAgent;
        }

        /// <summary>
        /// Common Load method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="fManipulateWebStream"></param>
        /// <param name="xPath"></param>
        /// <returns></returns>
        public void Download(string url, bool isImage, ImageFormat format)
        {
            uriPath = new Uri(url);

            ServicePointManager.SecurityProtocol = configuration.SecurityProtocol;
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, error) =>
                {
                    return true;
                };
            var webRequestObj = WebRequest.Create(url);
            
            var webResponseObj = webRequestObj.GetResponse();
            var httpResponse = (HttpWebResponse)webResponseObj;

            LocalFile = Guid.NewGuid().ToString();
            var stream = webResponseObj.GetResponseStream();

            if (!isImage)
            {
                LocalFile += ".txt";
                LocalFile = Path.Combine(localPath, LocalFile);
                using (var reader = new StreamReader(stream))
                    File.WriteAllText(LocalFile, reader.ReadToEnd());
            }
            else
            {
                LocalFile += format.ToString();
                LocalFile = Path.Combine(localPath, LocalFile);
                using (var image = Image.FromStream(stream))
                {
                    // If you want it as Png
                    image.Save(LocalFile, format);
                }
            }
        }

        public string GetText(ImageFormat format)
        {
            return format.ToString();
        }

        public ImageFormat GetFormat(string text)
        {
            return (ImageFormat)Enum.Parse(typeof(ImageFormat), text);
        }
    }
}
