using System.Text.RegularExpressions;

namespace Shared.Models.Work
{
    public class UnitOfWork
    {
        private const String FilenameRegexPattern =
            "^https?://.*/([\u0000-\u024F]*|[0-9\\.()%_~\\[\\]-]*(\\.[a-zA-Z]{1}([a-zA-Z]{1})?([a-zA-Z]{1})?([a-zA-Z]{1})?))$";

        private const String IndexedFilenameRegexPattern =
            "([\u0000-\u024F]*|[0-9\\.()%_~\\[\\]-]*) \\(([0-9]{1,})\\)\\.([a-zA-Z]{1}([a-zA-Z]{1})?([a-zA-Z]{1})?([a-zA-Z]{1})?)";

        //Dot is a legal character in folder name, but CreateFolderAsync with CreationCollisionOption.GenerateUniqueName
        //throws "Value does not fall within the expected range." ArgumentException
        //private static readonly Char[] _illegalChars = new Char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|', '.' };
        private static readonly Char[] _illegalChars = new Char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
        private static readonly Regex _filenameRegex = new Regex(FilenameRegexPattern);

        private String _fileName;

        public String Filename
        {
            get => _fileName;
            private set => _fileName = String.IsNullOrWhiteSpace(value) ? value : _illegalChars.Aggregate(value, (current, illegalChar) => current.Replace(illegalChar.ToString(), ""));
        }

        public Boolean IsDownloaded { get; set; }

        public Uri Url { get; }

        public Guid Id { get; } = Guid.NewGuid();

        private Int32 _index;

        public UnitOfWork(String url) : this(url, ProcessInternally)
        {
        }

        public UnitOfWork(String url, String filename)
        {
            Url = new Uri(url);
            Filename = filename;
        }

        public UnitOfWork(String url, Func<String, String> processingFunc)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (processingFunc == null)
            {
                throw new ArgumentNullException(nameof(processingFunc));
            }

            Url = new Uri(url);
            Filename = processingFunc(url);
        }

        private static String ProcessInternally(String url)
        {
            Match? match = _filenameRegex.Match(url);
            if (!match.Success)
            {
                throw new ArgumentException($"Failed to get filename from url [{url}] using regex pattern [{FilenameRegexPattern}]");
            }

            return System.Web.HttpUtility.UrlDecode(match.Groups[1].Value, System.Text.Encoding.UTF8);
        }

        public UnitOfWork IndexFilename()
        {
            Filename = _index > 0
                ? Regex.Replace(Filename, IndexedFilenameRegexPattern, $"$1 ({++_index}).$3")
                : Filename.Insert(Filename.LastIndexOf('.'), $" ({++_index})");

            return this;
        }
    }
}
