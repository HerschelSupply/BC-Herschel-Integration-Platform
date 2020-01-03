using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BC.Integration.Interfaces
{
    public interface IFtpIntegrator
    {
        string AvailableFilename { get; }
        string AvailableFileText { get; }
        string ArchiveFileName { get; }

        /// <summary>
        /// Creates a session with remote ftp server.  Can create either a ftp or sFTP connection.
        /// </summary>
        /// <param name="configuration">The configuarion collection to be used by the component to config the component.</param>
        void CreateSession(List<KeyValuePair<string, string>> configuration);

        /// <summary>
        /// Gets the locked file from the remote server and saves it to a local folder.  Reads the file and
        /// moves it to an achived folder with a GUID filename post fix.  Updates the IsFileDownloadSuccessful
        /// in the batch file persistence record. 
        /// </summary>
        /// <param name="archiveFolder">Local archieve folder to store the downloaded files in</param>
        /// <returns>Returns the string representation of the batch file.</returns>
        bool GetFile();

        /// <summary>
        /// This method is used to upload files to an FTP server
        /// </summary>
        /// <param name="filePath">Path to source file including file name.</param>
        /// <param name="fileName">Name to be given to the file on the FTP server.</param>
        /// <returns>Boolean to show success.  A failure will result in an exception.</returns>
        bool UploadFile(string filePath, string fileName);

        /// <summary>
        /// Closes the session with the FTP server.  This method should be called in the finally block to ensure the resources
        /// are disposed of.  It should also be called as soon as the GetFile mothod has been called.
        /// </summary>
        void CloseSession();
         
    }
}
