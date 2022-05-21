namespace Infra.Storage.Abstractions
{
    /// <summary>
    /// Type provides methods to access local file storage.
    /// </summary>
    public interface IStorageProvider
    {
        /// <summary>
        /// Gets the base directory that the assembly resolver uses to probe for assemblies.
        /// </summary>
        String AppDirectory { get; }

        #region Folder Management

        //Task<StorageFolder> GetFolderFromPathAsync(String path);

        /// <summary>
        /// Creates all directories and subdirectories in the specified path unless they
        /// already exist.
        /// </summary>
        /// <param name="path">The path to create directory in.</param>
        /// <param name="folderName">The directory to create.</param>
        /// <returns>Full path to the directory. This path is returned regardless of 
        /// whether a directory at the specified path already exists.
        /// </returns>
        String CreateFolder(String path, String folderName);

        /// <summary>
        /// Creates all directories and subdirectories in the specified path unless they 
        /// already exist.
        /// </summary>
        /// <param name="path">The path to create.</param>
        void CreatePath(String path);

        /// <summary>
        /// Deletes the specified file.
        /// </summary>
        /// <param name="path">The name of the file to be deleted. Wildcard characters are not supported.</param>
        void DeleteFile(String path);

        /// <summary>
        /// Determines whether the given path refers to an existing directory on disk.
        /// </summary>
        /// <param name="path">The path to test.</param>
        /// <returns>
        /// true if path refers to an existing directory; false if the directory does not
        /// exist or an error occurs when trying to determine if the specified directory
        /// exists.
        /// </returns>   
        Boolean DiretoryExists(String path);

        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <param name="path">The file to check.</param>
        /// <returns>
        /// true if the caller has the required permissions and path contains the name of
        /// an existing file; otherwise, false. This method also returns false if path is
        /// null, an invalid path, or a zero-length string. If the caller does not have sufficient
        /// permissions to read the specified file, no exception is thrown and the method
        /// returns false regardless of the existence of path.
        /// </returns>  
        Boolean FileExists(String path);

        /// <summary>
        /// Retrieves the absolute path of the parent directory of the specified path.
        /// </summary>
        /// <param name="path">The path for which to retrieve the parent directory.</param>
        /// <returns>
        /// The parent directory, or null if path is the root directory, including the root
        /// of a UNC server or share name.
        /// </returns>
        String? GetParent(String path);

        /// <summary>
        /// Renames the file.
        /// </summary>
        /// <param name="path">Path to the file to rename.</param>
        /// <param name="oldName">Current file name.</param>
        /// <param name="newName">New file name.</param>
        void RenameFile(String path, String oldName, String newName);

        /// <summary>
        /// Gets the size, in bytes, of the current file.
        /// </summary>
        /// <param name="path">
        /// The fully qualified name of the new file, or the relative file name to get length of. Do not end the path with the directory separator character.
        /// </param>
        /// <returns>The size of the current file in bytes.</returns>
        /// <exception cref="IOException">System.IO.FileSystemInfo.Refresh cannot update the state of the file or directory.</exception>
        /// <exception cref="FileNotFoundException">The file does not exist. -or- The Length property is called for a directory.</exception>
        Int64 GetFileLength(String path);

        #endregion

        #region Read Operations

        /// <summary>
        /// Asynchronously opens a binary file, reads the contents of the file into a byte
        /// array, and then closes the file.
        /// </summary>
        /// <param name="path">The file path to open for reading.</param>
        /// <param name="fileName">The file name to open for reading.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is System.Threading.CancellationToken.None.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation, which wraps the byte
        /// ation, which wraps the byte
        /// </returns>
        Task<Byte[]> ReadBytesAsync(String path, String fileName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Opens an existing file for reading.
        /// </summary>
        /// <param name="path">The file path to be opened for reading.</param>
        /// <param name="fileName">The file name to be opened for reading.</param>
        /// <returns>A read-only stream on the specified path.</returns>
        Stream OpenStreamForRead(String path, String fileName);

        #endregion

        #region Write Operations

        /// <summary>
        /// Asynchronously creates a new file, writes the specified byte array to the file,
        /// and then closes the file. If the target file already exists <paramref name="overwrite"/>
        /// controls whether file is overwritten or indexed.
        /// </summary>
        /// <param name="bytes">The bytes to write to the file.</param>
        /// <param name="path">The file path to write to.</param>
        /// <param name="filename">The file name to write to.</param>
        /// <param name="overwrite">The flag to control file overwriting logic.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is System.Threading.CancellationToken.None.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task WriteToFileAsync(Byte[] bytes, String path, String filename, Boolean overwrite = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously creates a new file, writes the specified stream content to the file,
        /// and then closes the file. If the target file already exists <paramref name="overwrite"/>
        /// controls whether file is overwritten or indexed.
        /// </summary>
        /// <param name="stream">The stream to write to the file.</param>
        /// <param name="path">The file path to write to.</param>
        /// <param name="filename">The file name to write to.</param>
        /// <param name="overwrite">The flag to control file overwriting logic.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is System.Threading.CancellationToken.None.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task WriteToFileAsync(Stream stream, String path, String filename, Boolean overwrite = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Opens an existing file or creates a new file for writing.
        /// </summary>
        /// <param name="path">The file path to be opened for writing.</param>
        /// <param name="fileName">The file name to be opened for writing.</param>
        /// <returns>
        /// An unshared System.IO.FileStream object on the specified path with System.IO.FileAccess.Write
        /// access.
        /// </returns>
        Stream OpenStreamForWrite(String path, String fileName);

        #endregion
    }
}
