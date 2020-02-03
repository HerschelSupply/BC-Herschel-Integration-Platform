using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinSCP;
using System.IO;
using System.Diagnostics;
using BC.Integration.Interfaces;

namespace BC.Integration.Utility
{
    /// <summary>
    /// Uses the WinSCP NuGet package 5.11.1
    /// https://winscp.net/eng/docs/library_examples
    /// </summary>
    public class SFtpIntegrator : IFtpIntegrator
    {
        #region Properties
        private Session session = new Session();
        private SessionOptions sessionOptions = null;
        private List<KeyValuePair<string, string>> configuration;
        private string ftpType = "";
        private string ftpHost = "";
        private string ftpPort = "";
        private string ftpUser = "";
        private string ftpPassword = "";
        private string ftpFolder = "";
        private string archiveFolder = "";
        private string sshHostKeyFingerpring = "";
        private string availableFilename = "";
        private string availableFileText = "";
        private string archiveFileName = "";
        private bool tracingEnabled = false;
        private string processName = "";

        public string AvailableFilename { get => availableFilename; }
        public string AvailableFileText { get => availableFileText; }
        public string ArchiveFileName { get => archiveFileName; }
        #endregion

        /// <summary>
        /// Populates local properties with values from the configuration object
        /// </summary>
        private void Setup()
        {
            if (configuration == null)
                throw new Exception("The BC.Integration.Utility.SFtpIntegrator needs the Configuration property populated before calling the services methods.");
            try
            {
                processName = Utilities.GetConfigurationValue(configuration, "ProcessName");
                ftpType = Utilities.GetConfigurationValue(configuration, "FtpType");
                ftpHost = Utilities.GetConfigurationValue(configuration, "FtpHost");
                ftpPort = Utilities.GetConfigurationValue(configuration, "FtpPort");
                ftpUser = Utilities.GetConfigurationValue(configuration, "FtpUser");
                ftpPassword = Utilities.GetConfigurationValue(configuration, "FtpPassword");
                ftpFolder = Utilities.GetConfigurationValue(configuration, "FtpFolder");
                sshHostKeyFingerpring = Utilities.GetConfigurationValue(configuration, "FtpSshHostKey");
                archiveFolder = Utilities.GetConfigurationValue(configuration, "ArchiveFolder");
                string val = Utilities.GetConfigurationValue(configuration, "TracingEnabled");
                if (val != "")
                {
                    if (val.ToLower() == "true" || val.ToLower() == "false")
                    {
                        tracingEnabled = Convert.ToBoolean(val);
                    }
                    if (val == "1" || val == "0")
                    {
                        tracingEnabled = Convert.ToBoolean(Convert.ToInt16(val));
                    }
                }
                Trace.WriteLineIf(tracingEnabled, "ftpFolder: " + ftpFolder);
            }
            catch (Exception ex)
            {
                throw new Exception("An exception occured in the BC.Integration.Utility.SFtpIntegrato.Setup method while trying to set the configuration values.", ex);
            }
        }


        /// <summary>
        /// Creates a session with remote ftp server.  Can create either a ftp or sFTP connection.
        /// </summary>
        public void CreateSession(List<KeyValuePair<string, string>> configuration)
        {
            this.configuration = configuration;
            Setup();
            try
            {
                //Create a WinSPCnet session object
                if (ftpType.ToLower() == "sftp")
                {
                    //If no SSH cert is available then throw an exception
                    if (sshHostKeyFingerpring == "")
                    {
                        Trace.WriteLine("SFtpIntegrator: sshHostKeyFingerpring property must be set to use the sFTP type");
                        throw new Exception("sshHostKeyFingerpring property must be set to use the sFTP type");
                    }

                    sessionOptions = new SessionOptions
                    {
                        Protocol = Protocol.Sftp,
                        HostName = ftpHost,
                        UserName = ftpUser,
                        Password = ftpPassword,
                        PortNumber = Convert.ToInt16(ftpPort),
                        SshHostKeyFingerprint = sshHostKeyFingerpring
                    };
                }
                else if (ftpType.ToLower() == "ftp")
                {
                    sessionOptions = new SessionOptions
                    {
                        HostName = ftpHost,
                        UserName = ftpUser,
                        Password = ftpPassword,
                        PortNumber = Convert.ToInt16(ftpPort),
                    };
                }
                else
                {
                    Trace.WriteLine("SFtpIntegrator: This method only supports FTP and sFTP connections");
                    //This exception will be caught in the try catch and the method and component details added.
                    throw new Exception("This method only supports FTP and sFTP connections");
                }
                // Connect session to server
                session.Open(sessionOptions);
                Trace.WriteLine("SFtpIntegrator: Created session with FTP server.");
            }
            catch (Exception ex)
            {
                session.Dispose();
                throw new Exception("Component BC.Integration.Utility.SFtpIntegrator raised a " + ex.Message + " in the CreateSession (" +
                    "hostName -" + ftpHost + ", " +
                    "port -" + ftpPort + ", " +
                    "userName -" + ftpUser + ", " +
                    "password -" + ftpPassword + ", " +
                    "type -" + ftpType + ").", ex);
            }
        }


        /// <summary>
        /// Gets the locked file from the remote server and saves it to a local folder.  Reads the file and
        /// moves it to an achived folder with a GUID filename post fix.  Updates the IsFileDownloadSuccessful
        /// in the batch file persistence record. 
        /// </summary>
        /// <returns>Returns the string representation of the batch file.</returns>
        public bool GetFile()
        {
            availableFilename = GetAvailableFileName();
            Trace.WriteLineIf(tracingEnabled, "Available file to download: " + availableFilename);
            if (availableFilename == "")
                return false; //exit code if there are no files to process.

            //place file in archive folder and deletes file form server.
            Trace.WriteLineIf(tracingEnabled, "SFtpIntegrator: Start getting file.");
            archiveFolder = archiveFolder.Replace("#ProcessName#", processName);
            if (!Directory.Exists(archiveFolder))
                Directory.CreateDirectory(archiveFolder);
            TransferOperationResult result = session.GetFiles(ftpFolder + @"/" + availableFilename, archiveFolder + @"\" + availableFilename, true);
            if (!result.IsSuccess)
                throw new Exception("The method BC.Integration.Utility.SFtpIntegrator.GetFile failed to get the file from the FTP server.  This " +
                    "error can occur if the destination directory does not exist. Archive folder was set to: " + archiveFolder + ".");
            //Read file and return a string
            Trace.WriteLineIf(tracingEnabled, "SFtpIntegrator: Start reading file.");
            availableFileText = File.ReadAllText(archiveFolder + @"\" + availableFilename);

            SaveFtpFileToLocalFolder();

            return true;
        }

        /// <summary>
        /// This method is used to upload a file to a FTP server.
        /// </summary>
        /// <param name="filePath">Path and file name of source file in local directory</param>
        /// <param name="fileName">Desired file name for file on the FTP server.</param>
        /// <returns></returns>
        public bool UploadFile(string filePath, string fileName)
        {
            try
            {
                TransferOperationResult result = session.PutFiles(filePath, fileName);
                if(result.IsSuccess)
                {
                    return true;
                }
                else
                {
                    string msg = "Exception Messages from WinSCP.net client: ";
                    SessionRemoteExceptionCollection exCol = result.Failures;
                    for(int i = 0; i < exCol.Count; i++)
                    {
                        msg = "Inner Exception Message: " + exCol[i].Message + "\r\n";
                    }
                    throw new Exception(msg);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("SFtpIntegrator: An error occurred trying to upload the file ( " + filePath + ") to the ShopperTrak FTP site. Please " +
                    "review the inner exceptions for details. Exception raised at BC.Integration.Utility.SFtpIntegrator.UploadFile method.  " +
                    "Exception Message: " + ex.Message);
                throw new Exception("An error occurred trying to upload the file ( " + filePath + ") to the ShopperTrak FTP site. Please " +
                    "review the inner exceptions for details. Exception raised at BC.Integration.Utility.SFtpIntegrator.UploadFile method.", ex);
            }
        }
        
        /// <summary>
        /// Closes the session with the FTP server.  This method should be called in the finally block to ensure the resources
        /// are disposed of.  It should also be called as soon as the GetFile mothod has been called.
        /// </summary>
        public void CloseSession()
        {
            try
            {
                session.Close();
                session.Dispose();
                Trace.WriteLineIf(tracingEnabled, "SFtpIntegrator: Session closed and disposed of.");
            }
            catch (InvalidOperationException ex)
            {
                //Suppress exception as we do not want to block the raising of exceptions generated while creating a session, by
                //trying to closing a session that failed to be opened. 
                Trace.WriteLineIf(tracingEnabled, "SFtpIntegrator: Session close raised an error probably becouse it didn't exist to be closed. Exception message: " + ex.Message);
            }
        }

        /// <summary>
        /// This method is used to move the downloaded FTP file from a default local file folder to an archieve folder.
        /// </summary>
        private void SaveFtpFileToLocalFolder()
        {
            string path = archiveFolder;
            path += "\\" + DateTime.Now.ToString("yyyyMMdd");
            path += "\\" + availableFilename.Substring(0, availableFilename.IndexOf('.'));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            archiveFileName = path + "\\" + availableFilename.Substring(0, availableFilename.IndexOf('.')) + "-" + Guid.NewGuid().ToString(); // + availableFilename.Substring(availableFilename.IndexOf('.'));
            File.Move(archiveFolder + "\\" + availableFilename, archiveFileName + ".txt");
            Trace.WriteLineIf(tracingEnabled, "SFtpIntegrator: FTP batch file moved to archived folder (" + archiveFileName + ".txt).");
        }

        /// <summary>
        /// Determines which files are available on the ftp server and finds the first file that is not actively being processed
        /// according to the batch file persistence DB.  Enters a record in the batch persistence file to lock file.
        /// </summary>
        /// <param name="remoteFolder">Folder on the remote server to look for files</param>
        /// <returns>Boolean to indicate if there is a file available for the integration to process</returns>
        private string GetAvailableFileName()
        {
            try
            {
                RemoteDirectoryInfo directory = session.ListDirectory(ftpFolder);
                foreach (RemoteFileInfo fileInfo in directory.Files)
                {
                    if (fileInfo.Name == ".." || fileInfo.Name == ".")
                        continue;
                    return fileInfo.Name;
                }
                return "";
            }
            catch (Exception ex)
            {
                session.Dispose();
                throw new Exception("Component BC.Integration.Utility.SFtpIntegrator raised an exception in the GetAvailableFileName method.", ex);
            }
        }

    }
}
