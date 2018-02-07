/*
 * ref. http://mansoftdev.wordpress.com/2011/01/30/bing-image-download/
 * http://msdn.microsoft.com/ko-kr/goglobal/bb896001.aspx
 * 
 * 맵 URL 파라미터 참고 
 * http://msdn.microsoft.com/en-us/library/dn217138.aspx
 * http://www.bing.com/blogs/site_blogs/b/maps/archive/2008/04/10/live-search-maps-api.aspx
 * 
 * HTTP Client (nuget package)
 * http://msdn.microsoft.com/en-us/library/system.net.http.aspx
 * http://blogs.msdn.com/b/bclteam/archive/2013/02/18/portable-httpclient-for-net-framework-and-windows-phone.aspx
 */

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BingHelper
{
    public class BingImage
    {
        private const string BINGIMG_FORMAT = @".jpg";
        private const string BING_ROOT = "http://www.bing.com";
        private const string IMG_URL = "http://www.bing.com/HPImageArchive.aspx?format=xml&idx=0&n={0}&mkt={1}";
        private static string[] Markets = new string[] { 
            "en-US", 
            "zh-CN", 
            "ja-JP", 
            "en-AU", 
            "en-UK", 
            "de-DE", 
            "en-NZ", 
            "en-CA", 
            "fr-FR",
            "it-IT",
            "pl-PL",
            "en-IN",
            "vi-VN",
            "th-TH",
            "ko-KR",
            "ms-MY",
            "pt-PT",
            "es-ES",
            "tr-TR",
            "nl-NL",
            "hu-HU",
            "pt-BR",
            "ru-RU",
            "fi-FI",
#if false
            "zh-TW", 
            "cs-CZ", 
            "da-DK", 
            "nl-NL",
            "el-GR",
            "id-ID",
            "nb-NO",
            "pt-BR",
            "sv-SE",
#endif
        };
        public static string[] BingImageResolutionString = new string[]
        {
            "240x240", "240x400", "400x240", "958x512", "480x800", "720x1280", "768x1280", "1080x1920", "1024x768", "1280x720", "1366x768", "1920x1200",
        };
        private const int NUMBER_OF_IMAGES = 1;
        private BingImageResolution bingimageResolution = BingImageResolution.DEFAULT;

        //StorageFolder _folder = ApplicationData.Current.LocalFolder;

        public bool isDataLoaded { get; set; }

        public const string ERRORMSG = "[HttpRequestException]";
        public string HttpResponseError = string.Empty;

        //public delegate void UpdateEventHandler(object sender, object param);
        //public event UpdateEventHandler OnUpdated;

        public BingImage()
        {

        }

        #region 이미지 정보 얻기
        public async Task<BingUnit> GetTodayBingUnit(BingImageResolution res = BingImageResolution.PHONE_WVGA, string market = "en-US", double networkTimeout = 7)
        {
            BingUnit unit = new BingUnit();

            bingimageResolution = res;

            XDocument doc = null;

            // Form the URL based on market Since this will be run once per day only 1 image needs to be downloaded
            string url = string.Format(IMG_URL, NUMBER_OF_IMAGES, market);

            HttpClient client = new HttpClient();
            try
            {
#if true
                client.Timeout = TimeSpan.FromSeconds(networkTimeout);
                string data = await client.GetStringAsync(new Uri(url));
#else
                WebClient client = new WebClient();
                string data = await client.DownloadStringTask(new Uri(url));
#endif

                doc = XDocument.Parse(data);

                // Iterate through the image elements
                foreach (XElement image in doc.Descendants("image"))
                {
                    unit.Copyright = GetElmentValue(image, "copyright");
                    unit.StartDate = GetElmentValue(image, "startdate");
                    unit.EndDate = GetElmentValue(image, "enddate");

                    string imageUrl = string.Empty;

                    if (bingimageResolution == BingImageResolution.DEFAULT)
                    {
                        //기본사이즈인 958x512사이즈의 경우, urlBase가 아닌 그냥 url값을 사용하면 된다.
                        imageUrl = GetElmentValue(image, "url");
                    }
                    else
                    {
                        imageUrl = string.Format("{0}_{1}{2}", image.Element("urlBase").Value, BingImageResolutionString[(int)bingimageResolution], BINGIMG_FORMAT);
                    }

                    unit.Path = BING_ROOT + imageUrl;

                    bool bSearched = false;
                    foreach (XElement hotspots in image.Descendants("hotspots"))
                    {
                        foreach (XElement hotspot in hotspots.Descendants("hotspot"))
                        {
                            string strLink = hotspot.Element("link").Value;
                            if (strLink.Contains("maps"))
                            {
                                //unit.MapLink = ReplaceMapLevel(strLink, "4");
                                unit.MapLink = strLink;
                                bSearched = true;
                                break;
                            }
                        }

                        if (bSearched) break;
                    }
                }
            }
            catch (Exception ex)
            {
                client.CancelPendingRequests();
                client.Dispose();
                string message = ex.Message;
                return null;
            }

            return unit;
        }

        public async Task<List<BingUnit>> GetBingUnits(BingImageResolution res = BingImageResolution.PHONE_WVGA, string market = "en-US", int days = 1, double networkTimeout = 7)
        {
            List<BingUnit> units = new List<BingUnit>();

            bingimageResolution = res;

            XDocument doc = null;

            // Form the URL based on market Since this will be run once per day only 1 image needs to be downloaded
            string url = string.Format(IMG_URL, days, market);

            HttpClient client = new HttpClient();

            try
            {
#if true
                client.Timeout = TimeSpan.FromSeconds(networkTimeout);
                string data = await client.GetStringAsync(new Uri(url));
#else
                WebClient client = new WebClient();
                string data = await client.DownloadStringTask(new Uri(url));
#endif
                doc = XDocument.Parse(data);

                // Iterate through the image elements
                foreach (XElement image in doc.Descendants("image"))
                {
                    BingUnit unit = new BingUnit();

                    unit.Copyright = GetElmentValue(image, "copyright");
                    unit.StartDate = GetElmentValue(image, "startdate");
                    unit.EndDate = GetElmentValue(image, "enddate");

                    string imageUrl = string.Empty;

                    if (bingimageResolution == BingImageResolution.DEFAULT)
                    {
                        //기본사이즈인 958x512사이즈의 경우, urlBase가 아닌 그냥 url값을 사용하면 된다.
                        imageUrl = GetElmentValue(image, "url");
                    }
                    else
                    {
                        imageUrl = string.Format("{0}_{1}{2}", image.Element("urlBase").Value, BingImageResolutionString[(int)bingimageResolution], BINGIMG_FORMAT);
                    }

                    if (string.IsNullOrEmpty(imageUrl) == false)
                    {
                        unit.Path = BING_ROOT + imageUrl;


                        bool bSearched = false;
                        foreach (XElement hotspots in image.Descendants("hotspots"))
                        {
                            foreach (XElement hotspot in hotspots.Descendants("hotspot"))
                            {
                                string strLink = hotspot.Element("link").Value;
                                if (strLink.Contains("maps"))
                                {
                                    //unit.MapLink = strLink.Replace("&amp;", "&"); //처리 필요없음.
                                    //unit.MapLink = ReplaceMapLevel(strLink, "2"); // 값들이 무시되서 그냥 default로 처리.
                                    unit.MapLink = strLink;
                                    bSearched = true;
                                    break;
                                }
                            }

                            if (bSearched) break;
                        }

                        units.Add(unit);
                    }
                }
            }
            catch (Exception ex)
            {
                client.CancelPendingRequests();
                client.Dispose();
                string message = ex.Message;
                return null;
            }

            return units;
        }
        #endregion

        public string[] GetMarkets()
        {
            return Markets;
        }

        private string GetElmentValue(XElement parent, string name)
        {
            string result = string.Empty;

            try
            {
                result = parent.Element(name).Value;
            }
            catch
            {
            }

            return result;
        }

#if false //NOTUSED, 코드는 남겨둘것
        public async Task DownloadTodayImage(StorageFolder folder = null, BingImageResolution res = BingImageResolution.PHONE_WVGA, string market = "en-US")
        {
            folder = folder ?? ApplicationData.Current.LocalFolder;
            _folder = folder;
            bingimageResolution = res;

            // Make sure destination folder exists
            ValidateDownloadPath();

            XDocument doc = null;

            // Form the URL based on market Since this will be run once per day only 1 image needs to be downloaded
            string url = string.Format(IMG_URL, NUMBER_OF_IMAGES, market);

            try
            {
                WebClient client = new WebClient();
                string data = await client.DownloadStringTask(new Uri(url));

                doc = XDocument.Parse(data);

                // Iterate through the image elements
                foreach (XElement image in doc.Descendants("image"))
                {
                    //_copyright = image.Element("copyright").Value;

                    string imageUrl = string.Empty;

                    if (bingimageResolution == BingImageResolution.DEFAULT)
                    {
                        //기본사이즈인 958x512사이즈의 경우, urlBase가 아닌 그냥 url값을 사용하면 된다.
                        imageUrl = image.Element("url").Value;
                    }
                    else
                    {
                        imageUrl = string.Format("{0}_{1}{2}", image.Element("urlBase").Value, BingImageResolutionString[(int)bingimageResolution], BINGIMG_FORMAT);
                    }

                    string beforeImageUrl = await ReadFile("ImageUrl.txt");

                    if (beforeImageUrl.Equals(imageUrl))
                    {
                        if (OnUpdated != null)
                        {
                            OnUpdated(this, new EventArgs());
                        }

                        return;
                    }

                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        await SaveImage(imageUrl, image.Element("fullstartdate").Value, folder);

                        await WriteFile("ImageUrl.txt", imageUrl, CreationCollisionOption.ReplaceExisting);
                    }
                }

            }
            catch (Exception ex)
            {
                string message = ex.Message;
                HttpResponseError = ERRORMSG + ":" + message;

                isDataLoaded = false;
            }
        }

        /// <summary>
        /// Download images from Bing
        /// </summary>
        public async Task DownloadImages(StorageFolder folder = null, BingImageResolution res = BingImageResolution.PHONE_WVGA)
        {
            folder = folder ?? ApplicationData.Current.LocalFolder;
            _folder = folder;
            bingimageResolution = res;

            // Make sure destination folder exists
            ValidateDownloadPath();

            XDocument doc = null;

            // Because each market can have different images cycle through each of them
            foreach (string market in Markets)
            {
                // Form the URL based on market Since this will be run once per day only 1 image needs to be downloaded
                string url = string.Format(IMG_URL, NUMBER_OF_IMAGES, market);

                try
                {
                    WebClient client = new WebClient();
                    string data = await client.DownloadStringTask(new Uri(url));

                    doc = XDocument.Parse(data);

                    // Iterate through the image elements
                    foreach (XElement image in doc.Descendants("image"))
                    {
                        if (bingimageResolution == BingImageResolution.DEFAULT)
                        {
                            //기본사이즈인 958x512사이즈의 경우, urlBase가 아닌 그냥 url값을 사용하면 된다.
                            await SaveImage(image.Element("url").Value, res, folder);
                        }
                        else
                        {
                            string urlExt = string.Format("{0}_{1}{2}", image.Element("urlBase").Value, BingImageResolutionString[(int)bingimageResolution], BINGIMG_FORMAT);

                            if (!string.IsNullOrEmpty(urlExt))
                            {
                                await SaveImage(urlExt, res, folder);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                    HttpResponseError = ERRORMSG + ":" + message;

                    isDataLoaded = false;
                }
            }
        }

        #region 여러 마켓 이미지 다운로드
        private async Task SaveImage(string url, BingImageResolution res, StorageFolder folder = null)
        {
            try
            {
                folder = folder ?? ApplicationData.Current.LocalFolder;

                string filename = GetImageName(url, res);
                var file = await folder.CreateFileAsync(filename, CreationCollisionOption.FailIfExists);
                string imageURL = BING_ROOT + url;

                WebClient client = new WebClient();

                using (Stream stream = await client.OpenReadTask(new Uri(imageURL)))
                {
                    if (stream.CanRead)
                    {
                        byte[] img = new byte[stream.Length];
                        stream.Read(img, 0, (int)stream.Length);

                        if (img.Length > 0)
                        {
                            using (var fs = await file.OpenAsync(FileAccessMode.ReadWrite))
                            {
                                using (var outStream = fs.GetOutputStreamAt(0))
                                {
                                    using (var dataWriter = new DataWriter(outStream))
                                    {
                                        dataWriter.WriteBytes(img);

                                        await dataWriter.StoreAsync();
                                        dataWriter.DetachStream();
                                    }

                                    await outStream.FlushAsync();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //이미 다운받은 이미지를 다시 다운로드 하지 않기 위해
                //파일 생성 옵션을 CreationCollisionOption.FailIfExists로 설정했기 때문에 존재할 경우 Exception 처리.
                string msg = ex.Message;
            }
        }

        private string GetImageName(string url, BingImageResolution res)
        {
            string name = string.Empty;

            // URL is in this format /fd/hpk2/DiskoBay_EN-US1415620951.jpg
            // Extract the image number
            // 해상도에 따른 뒤에 넘버는 다를 수 있으니 참고할것.
            //Regex reg = new Regex(@"[0-9]+\w");
            //Match m = reg.Match(url);
            // Should now have 1415620951 from above example
            // Create path to save image to
            //if (bingimageResolution == BingImageResolution.DEFAULT)
            //{
            //    name = string.Format(@"{0}", m.Value);
            //}
            //else
            //{
            //    name = string.Format(@"{0}.jpg", m.Value);
            //}

            name = url.Substring(url.LastIndexOf('/') + 1);
            name = string.Format("{0}_{1}.jpg", name.Substring(0, name.IndexOf('_')), BingImageResolutionString[(int)res]);

            return name;
        }

        private string GetImageNameEx(string url)
        {
            string name = string.Empty;

            int n = url.LastIndexOf(@"/");

            if (bingimageResolution == BingImageResolution.DEFAULT)
            {
                name = string.Format(@"{0}{1}", url.Substring(n), BINGIMG_FORMAT);
            }
            else
            {
                name = string.Format(@"{0}", url.Substring(n));
            }

            return name;
        }
        #endregion

        private async Task SaveImage(string url, string date, StorageFolder folder = null)
        {
            try
            {
                folder = folder ?? ApplicationData.Current.LocalFolder;

                string filename = "today" + BINGIMG_FORMAT;
                var file = await folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

                string imageURL = BING_ROOT + url;

                WebClient client = new WebClient();

                using (Stream stream = await client.OpenReadTask(new Uri(imageURL)))
                {
                    if (stream.CanRead)
                    {
                        byte[] img = new byte[stream.Length];
                        stream.Read(img, 0, (int)stream.Length);

                        if (img.Length > 0)
                        {
                            using (var fs = await file.OpenAsync(FileAccessMode.ReadWrite))
                            {
                                using (var outStream = fs.GetOutputStreamAt(0))
                                {
                                    using (var dataWriter = new DataWriter(outStream))
                                    {
                                        dataWriter.WriteBytes(img);

                                        await dataWriter.StoreAsync();
                                        dataWriter.DetachStream();
                                    }

                                    await outStream.FlushAsync();
                                }
                            }
                        }
                    }
                }

                if (OnUpdated != null)
                {
                    OnUpdated(this, new EventArgs());
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        public async Task<BitmapImage> GetImage(string url)
        {
            BitmapImage bitmap = new BitmapImage();

            try
            {
                WebClient client = new WebClient();

                using (Stream stream = await client.OpenReadTask(new Uri(url)))
                {
                    if (stream.CanRead)
                    {
                        bitmap.SetSource(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return bitmap;
        }

        public async Task<BitmapImage> GetTodayImage()
        {
            var bmp = new BitmapImage();

            try
            {
                StorageFolder folder = _folder;

                var file = await folder.GetFileAsync("today.jpg");

                using (var fs = await file.OpenAsync(FileAccessMode.Read))
                {
                    using (Stream stream = fs.AsStream())
                    {
                        bmp.SetSource(stream);
                    }
                }

                return bmp;
            }
            catch (FileNotFoundException)
            {
                //file not existing is perfectly valid so simply return the default 
                return null;
                //throw;
            }
            catch (Exception)
            {
                //Unable to load contents of file
                return null;
            }
        }

        #region 이미지 절대경로 얻기
        public async Task<string> GetTodayPath(BingImageResolution res = BingImageResolution.PHONE_WVGA, string market = "en-US")
        {
            string path = string.Empty;

            bingimageResolution = res;

            // Make sure destination folder exists
            ValidateDownloadPath();

            XDocument doc = null;

            // Form the URL based on market Since this will be run once per day only 1 image needs to be downloaded
            string url = string.Format(IMG_URL, NUMBER_OF_IMAGES, market);

            try
            {
                WebClient client = new WebClient();
                string data = await client.DownloadStringTask(new Uri(url));

                doc = XDocument.Parse(data);

                // Iterate through the image elements
                foreach (XElement image in doc.Descendants("image"))
                {
                    //_copyright = image.Element("copyright").Value;

                    string imageUrl = string.Empty;

                    if (bingimageResolution == BingImageResolution.DEFAULT)
                    {
                        //기본사이즈인 958x512사이즈의 경우, urlBase가 아닌 그냥 url값을 사용하면 된다.
                        imageUrl = image.Element("url").Value;
                    }
                    else
                    {
                        imageUrl = string.Format("{0}_{1}{2}", image.Element("urlBase").Value, BingImageResolutionString[(int)bingimageResolution], BINGIMG_FORMAT);
                    }

                    path = BING_ROOT + "/" + imageUrl;
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }

            return path;
        }
        #endregion

        private async Task WriteFile(string fileName, string data, CreationCollisionOption creationCollisionOption)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;

            using (Stream stream = await folder.OpenStreamForWriteAsync(fileName, creationCollisionOption))
            {
                using (StreamWriter sw = new StreamWriter(stream))
                {
                    sw.WriteLine(data);
                }
            }
        }

        private async Task<string> ReadFile(string fileName)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            string data = string.Empty;

            try
            {
                using (Stream stream = await folder.OpenStreamForReadAsync(fileName))
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        data = sr.ReadLine();
                    }
                }
            }
            catch
            {
                return string.Empty;
            }

            return data;
        }
#endif
    }
}
