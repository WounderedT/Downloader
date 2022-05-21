using Infra.Storage.Abstractions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Shared.Extensions;
using System.Collections.Concurrent;

namespace Infra.Storage
{
    public class StorageProvider : IStorageProvider
    {
        private const Char Separator = '\\';
        //Dot is a legal character in folder name, but CreateFolderAsync with CreationCollisionOption.GenerateUniqueName
        //throws "Value does not fall within the expected range." ArgumentException
        private static readonly Char[] _illegalChars = new Char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|', '.' };
        private readonly ILogger<StorageProvider> _logger;
        private readonly ConcurrentDictionary<String, Int16> _processedFiles;

        public StorageProvider(ILogger<StorageProvider> logger)
        {
            _logger = logger;
            _processedFiles = new ConcurrentDictionary<String, Int16>();
        }

        /// <inheritdoc />
        public String AppDirectory => AppDomain.CurrentDomain.BaseDirectory;

        #region Folder Management

        /// <inheritdoc />
        public String CreateFolder(String path, String folderName)
        {
            try
            {
                folderName = _illegalChars.Aggregate(folderName, (current, illegalChar) => current.Replace(illegalChar.ToString(), ""));
                _logger.LogDebug($"Creating folder [{folderName}] in [{path}].");
                return Directory.CreateDirectory(Path.Combine(path, folderName)).FullName;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"An exception was thrown while creating a folder [{folderName}] under [{path}].");
                _logger.LogException(ex);
                throw;
            }
        }

        /// <inheritdoc />
        public void CreatePath(String path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(nameof(path));
            }

            Directory.CreateDirectory(path);
        }

        /// <inheritdoc />
        public void DeleteFile(String path)
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An exception was caught while deleting the file: {ex.GetType().Name}: {ex.Message}");
                _logger.LogException(ex);
                throw;
            }
        }

        /// <inheritdoc />
        public Boolean DiretoryExists(String path)
        {
            return Directory.Exists(path);
        }

        /// <inheritdoc />
        public Boolean FileExists(String path)
        {
            return File.Exists(path);
        }

        /// <inheritdoc />
        public String? GetParent(String path)
        {
            DirectoryInfo? parent = Directory.GetParent(path);
            return parent == null ? null : parent.Name;
        }

        /// <inheritdoc />
        public void RenameFile(String path, String oldName, String newName)
        {
            File.Move(Path.Combine(path, oldName), Path.Combine(path, newName));
        }

        /// <inheritdoc />
        public Int64 GetFileLength(String path)
        {
            FileInfo fileInfo = new FileInfo(path);
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException($"Cannot find file {path}");
            }
            return fileInfo.Length;
        }

        #endregion

        #region Read Operations

        /// <inheritdoc />
        public async Task<Byte[]> ReadBytesAsync(String path, String filename, CancellationToken token = default)
        {
            var filePath = Path.Combine(path, filename);
            _logger.LogDebug($"Reading bytes from [{filePath}] directory.");
            try
            {
                return await File.ReadAllBytesAsync(filePath, token);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"An exception was caught reading bytes from [{filePath}].");
                _logger.LogException(ex);
                throw;
            }
        }

        /// <inheritdoc />
        public Stream OpenStreamForRead(String path, String filename)
        {
            var filePath = Path.Combine(path, filename);
            try
            {
                return File.OpenRead(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"An exception was caught while openning stream for reading from a file [{filePath}].");
                _logger.LogException(ex);
                throw;
            }
        }

        #endregion

        #region Write Operations

        /// <inheritdoc />
        public Stream OpenStreamForWrite(String path, String filename)
        {
            try
            {
                return File.OpenWrite(Path.Combine(path, filename));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"An exception was caught while openning stream for writing from a file [{Path.Combine(path, filename)}].");
                _logger.LogException(ex);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task WriteToFileAsync(Byte[] bytes, String path, String filename, Boolean overwrite = false, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug($"Writing [{bytes.Length}] bytes into file [{filename}] in [{path}] directory.");
            try
            {
                var indexedName = IndexFile(path, filename);
                if (overwrite)
                {
                    await File.WriteAllBytesAsync(Path.Combine(path, filename), bytes, cancellationToken);
                    return;
                }
                await File.WriteAllBytesAsync(Path.Combine(path, indexedName), bytes, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"An exception was caught writing bytes to file [{Path.Combine(path, filename)}].");
                _logger.LogException(ex);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task WriteToFileAsync(Stream stream, String path, String filename, Boolean overwrite = false, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug($"Writing strean into file [{filename}] in [{path}] directory.");
            try
            {
                var indexedName = IndexFile(path, filename);
                using FileStream fileStream = overwrite ? File.OpenWrite(Path.Combine(path, filename)) : File.OpenWrite(Path.Combine(path, indexedName));
                await stream.CopyToAsync(fileStream, cancellationToken);
                await fileStream.FlushAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"An exception was caught writing bytes to file [{Path.Combine(path, filename)}].");
                _logger.LogException(ex);
                throw;
            }
        }

        #endregion

        private String IndexFile(String filePath, String fileName)
        {
            var path = Path.Combine(filePath, fileName);
            var index = _processedFiles.AddOrUpdate(path, default(Int16), (path, index) => index++);
            return index == default(Int16) ? fileName : IndexFile(fileName, index);
        }

        private static String IndexFile(String filename, Int32 index)
        {
            if (String.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentException(nameof(filename));
            }
            var dotIndex = filename.LastIndexOf('.');
            return dotIndex > 0 ? $"{filename.Substring(0, dotIndex)} ({index}){filename.Substring(dotIndex)}" : $"{filename} ({index})";
        }
    }
}
