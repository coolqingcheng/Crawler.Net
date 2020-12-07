using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Crawler.Net.Extensions
{
    public class MimeUtility
    {
        /// <summary>
        /// The "octet-stream" subtype is used to indicate that a body contains arbitrary binary data.
        /// See <a href="https://www.iana.org/assignments/media-types/application/octet-stream">application/octet-stream</a>
        /// </summary>
        public const string UnknownMimeType = "application/octet-stream";

        static Lazy<ReadOnlyDictionary<string, string>> _lazyDict = new Lazy<ReadOnlyDictionary<string, string>>(
            () => new ReadOnlyDictionary<string, string>(KnownMimeTypes.ALL_EXTS.Value.ToDictionary(e => e, e => KnownMimeTypes.LookupType(e)))
        );

        /// <summary>
        /// Dictionary of all available types (lazy loaded on first call)
        /// </summary>
        public static ReadOnlyDictionary<string, string> TypeMap => _lazyDict.Value;

        /// <param name="file">The file extensions (ex: "zip"), the file name, or file path</param>
        /// <returns>The mime type string, returns "application/octet-stream" if no known type was found</returns>
        public static string GetMimeMapping(string file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            if (string.IsNullOrEmpty(file))
                return UnknownMimeType;

            var fileExtension = file.Contains(".")
                ? Path.GetExtension(file).Substring(1)
                : file;

            return KnownMimeTypes.LookupType(fileExtension.ToLowerInvariant()) ?? UnknownMimeType;
        }
        /// <summary>
        /// 用Mime获取扩展名
        /// </summary>
        /// <param name="mime"></param>
        /// <param name="defaultExtensions"></param>
        /// <returns></returns>
        public static string GetExtensionByMime(string mime)
        {
            #region 不规范的MIME特殊处理
            if (mime == "image/jpg")
            {
                return "jpg";
            }
            #endregion
            var type = TypeMap.Where(a => a.Value == mime).FirstOrDefault().Key;
            if (type.IsNull())
            {
                return UnknownMimeType;
            }
            return type;
        }
    }
}
