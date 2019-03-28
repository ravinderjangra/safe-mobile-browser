using MimeMapping;
using SafeApp;
using SafeApp.Utilities;
using SafeMobileBrowser.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SafeMobileBrowser.Services
{
    public class WebFetch
    {
        private const int TAG_TYPE_DNS = 15001;
        private const int TAG_TYPE_WWW = 15002;
        private const int NFS_FILE_START = 0;
        private const int NFS_FILE_END = 0;
        private const string INDEX_HTML = "index.html";

        public static Session session;

        public WebFetch()
        {
            session = App.AppSession;
        }

        public async Task<WebFetchResponse> FetchAsync(string url, WebFetchOptions options = null)
        {
            var response = new WebFetchResponse();
            var uri = new Uri(url);

            var hostname = uri.Host;
            var path = uri.AbsolutePath ?? $"/{INDEX_HTML}";

            string[] hostparts = hostname.Split('.');
            var lookupname = hostparts.Last();
            var servicename = hostparts.First();

            try
            {
                var serviceMd = await GetContainerFromPublicId(lookupname, servicename);
                var file = await TryDifferentPaths(serviceMd, path);
                var filedata = await ReadContentFromFile(serviceMd, file, options);
                response.Data = filedata.Item1.ToArray();
                response.MimeType = file.MimeType;
                response.Headers.Add("Content-Type", file.MimeType);
                if (options != null)
                {
                    response.Headers.Add("Content-Range", $"bytes {filedata.Item2}-{filedata.Item3}/{filedata.Item4}");
                    response.Headers.Add("Content-Length", $"{filedata.Item4}");
                }
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return response;
        }

        // Helper function to read fetch the Container
        // from a public ID and service name provided
        public static async Task<MDataInfo> GetContainerFromPublicId(string pubName, string servName)
        {
            (List<byte>, ulong) serviceInfo = (default(List<byte>), default(ulong));
            try
            {

                List<byte> address = await SafeApp.Misc.Crypto.Sha3HashAsync(pubName.ToUtfBytes());
                MDataInfo servicesContainer = new MDataInfo
                {
                    TypeTag = TAG_TYPE_DNS,
                    Name = address.ToArray()
                };
                serviceInfo = await session.MData.GetValueAsync(servicesContainer, (servName ?? "www").ToUtfBytes());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (serviceInfo.Item1.Count == 0)
            {
                throw new Exception("No data");
            }


            MDataInfo serviceMd;
            try
            {
                serviceMd = await session.MDataInfoActions.DeserialiseAsync(serviceInfo.Item1);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                serviceMd = new MDataInfo
                {
                    TypeTag = TAG_TYPE_WWW,
                    Name = serviceInfo.Item1.ToArray()
                };
            }
            return serviceMd;
        }

        // private Helper function to try private different paths private to find and
        // fetch the index file from a web site container
        public static async Task<WebFile> TryDifferentPaths(MDataInfo fileMdInfo, string initialPath)
        {
            var file = new File() { DataMapName = initialPath.ToUtfBytes().ToArray() };
            var filePath = initialPath == "/" ? INDEX_HTML : initialPath.Trim('/');
            try
            {
                (file, _) = await session.NFS.DirFetchFileAsync(fileMdInfo, filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            string mimeType = MimeUtility.GetMimeMapping(filePath);
            return new WebFile { File = file, MimeType = mimeType };
        }

        // Helper function to read the file's content, and return an
        // http compliant response based on the mime-type and options provided
        public static async Task<(List<byte>, ulong, ulong, ulong)> ReadContentFromFile(MDataInfo fileMdInfo, WebFile openedFile, WebFetchOptions options = null)
        {
            try
            {
                var filehandle = await session.NFS.FileOpenAsync(fileMdInfo, openedFile.File, SafeApp.Misc.NFS.OpenMode.Read);
                var filesize = await session.NFS.FileSizeAsync(filehandle);

                if (options == null)
                {
                    var filedata = await session.NFS.FileReadAsync(filehandle, 0, filesize - 1);
                    return (filedata, 0, filesize - 1, filesize);
                }
                else
                {
                    var partStartIndex = (ulong)(options?.Range[0].Start > 0 ? options?.Range[0].Start : 0);
                    var partendIndex = (ulong)0;

                    if (options?.Range[0].End > 0)
                    {
                        partendIndex = (ulong)options?.Range[0].End;
                    }
                    else
                    {
                        partendIndex = filesize - 1;
                    }

                    var partSize = partendIndex - partStartIndex + 1;
                    var filedata = await session.NFS.FileReadAsync(filehandle, partStartIndex, partSize);
                    return (filedata, partStartIndex, partendIndex, partSize);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (null, 0, 0, 0);
        }
    }
}
