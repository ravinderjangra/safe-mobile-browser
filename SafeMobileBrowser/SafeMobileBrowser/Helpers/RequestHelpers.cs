using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using SafeApp;
using SafeApp.Utilities;
using SafeMobileBrowser.Models;

namespace SafeMobileBrowser.Helpers
{
    public static class RequestHelpers
    {
        // Generating encoded app request using app name, appid, vendor
        public static async Task<(uint, string)> GenerateEncodedAppRequestAsync()
        {
            Logger.Info("Generating application authentication request");
            var authReq = new AuthReq
            {
                AppContainer = true,
                App = new AppExchangeInfo
                {
                    Id = Constants.AppId,
                    Scope = string.Empty,
                    Name = Constants.AppName,
                    Vendor = Constants.Vendor
                },
                Containers = new List<ContainerPermissions>()
            };

            return await Session.EncodeAuthReqAsync(authReq);
        }

        public static List<ByteRangeHeader> GetRangeFromHeaderRangeValue(string rangeHeaderString, [Optional] ulong contentSize)
        {
            var byteRanges = new List<ByteRangeHeader>();

            var bytes = "bytes=";
            var rangeValues = rangeHeaderString.Remove(0, bytes.Length).Split(',');

            // rangeHeader contains the value of the Range HTTP Header and can have values like:
            //      Range: bytes=0-1            * Get bytes 0 and 1, inclusive
            //      Range: bytes=0-500          * Get bytes 0 to 500 (the first 501 bytes), inclusive
            //      Range: bytes=400-1000       * Get bytes 500 to 1000 (501 bytes in total), inclusive
            //      Range: bytes=-200           * Get the last 200 bytes
            //      Range: bytes=500-           * Get all bytes from byte 500 to the end
            //
            // Can also have multiple ranges delimited by commas, as in:
            //      Range: bytes=0-500,600-1000 * Get bytes 0-500 (the first 501 bytes), inclusive plus bytes 600-1000 (401 bytes) inclusive

            foreach (var item in rangeValues)
            {
                if (!string.IsNullOrEmpty(rangeHeaderString))
                {
                    // Remove "Ranges" and break up the ranges
                    string[] ranges = rangeHeaderString.Replace("bytes=", string.Empty).Split(",".ToCharArray());

                    for (int i = 0; i < ranges.Length; i++)
                    {
                        const int START = 0, END = 1;
                        ulong endByte, startByte;
                        string[] currentRange = ranges[i].Split("-".ToCharArray());

                        if (ulong.TryParse(currentRange[END], out ulong parsedValue))
                            endByte = parsedValue;
                        else
                            endByte = contentSize == 0 ? 0 : contentSize - 1;

                        if (ulong.TryParse(currentRange[START], out parsedValue))
                        {
                            startByte = parsedValue;
                        }
                        else
                        {
                            // No beginning specified, get last n bytes of file
                            // We already parsed end, so subtract from total and
                            // make end the actual size of the file
                            if (contentSize == 0)
                            {
                                startByte = 0;
                            }
                            else
                            {
                                startByte = contentSize - endByte;
                                endByte = contentSize - 1;
                            }
                        }

                        Logger.Info($"StartByte {startByte}; EndByte: {endByte}");
                        byteRanges.Add(new ByteRangeHeader(startByte, endByte));
                    }
                }
            }
            return byteRanges;
        }
    }
}
