namespace SafeMobileBrowser.WebFetchImplementation
{
    public static class WebFetchConstants
    {
        // webfetch typetag and path constants
        public const int TAG_TYPE_DNS = 15001;
        public const int TAG_TYPE_WWW = 15002;
        public const int NFS_FILE_START = 0;
        public const int NFS_FILE_END = 0;
        public const string INDEX_HTML = "index.html";

        // webfetch error codes
        public const int NoSuchPublicName = 1000;
        public const int NoSuchServiceName = 1001;

        // webfetch error messages
        public const string NoSuchPublicNameMessage = "No such public name exists";
        public const string NoSuchServiceNameMessage = "No such service name exists";
    }
}
