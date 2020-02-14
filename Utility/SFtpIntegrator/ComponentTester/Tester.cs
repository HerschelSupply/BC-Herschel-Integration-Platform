using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace BC.Integration.Utility
{
    public partial class Tester : Form
    {
        public Tester()
        {
            InitializeComponent();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            List<KeyValuePair<string, string>> configuration = new List<KeyValuePair<string, string>>();
            configuration.Add(new KeyValuePair<string, string>("FtpType", "sFTP"));
            configuration.Add(new KeyValuePair<string, string>("FtpHost", "140.207.97.148"));
            configuration.Add(new KeyValuePair<string, string>("FtpPort", "18622"));
            configuration.Add(new KeyValuePair<string, string>("FtpUser", "herschel_test"));
            configuration.Add(new KeyValuePair<string, string>("FtpPassword", "Herschel12345#"));
            configuration.Add(new KeyValuePair<string, string>("ftpFolder", "upload"));
            configuration.Add(new KeyValuePair<string, string>("FtpSshHostKey", "ecdsa-sha2-nistp256 256 5d:23:1e:e3:6e:bb:e9:d4:9b:14:cb:4b:50:d2:2d:02"));
            configuration.Add(new KeyValuePair<string, string>("ArchiveFolder", @"C:\Temp\FTP Test"));

            SFtpIntegrator ftp = new SFtpIntegrator();
            try
            {
                //Create session
                ftp.CreateSession(configuration);
                bool result = ftp.GetFile();
                txtResponse.Text = ftp.AvailableFileText;
            }
            catch (Exception ex)
            {
                txtResponse.Text = ex.Message;
            }
            finally
            {
                //Close and Dispose of session
                //ftp.CloseSession();
            }

        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            List<KeyValuePair<string, string>> configuration = new List<KeyValuePair<string, string>>();
            configuration.Add(new KeyValuePair<string, string>("FtpType", "sFTP"));
            configuration.Add(new KeyValuePair<string, string>("FtpHost", "transfer.shoppertrak.com"));
            configuration.Add(new KeyValuePair<string, string>("FtpPort", "22"));
            configuration.Add(new KeyValuePair<string, string>("FtpUser", "HerschelSupply"));
            configuration.Add(new KeyValuePair<string, string>("FtpPassword", "8:Rs5[1}.S"));
            configuration.Add(new KeyValuePair<string, string>("ftpFolder", ""));
            configuration.Add(new KeyValuePair<string, string>("FtpSshHostKey", "ssh-rsa 2048 1a:31:b3:2c:c7:65:71:0c:fd:d6:9d:fd:e8:82:99:50"));
            configuration.Add(new KeyValuePair<string, string>("ArchiveFolder", @"C:\Temp\FTP Test"));

            SFtpIntegrator ftp = new SFtpIntegrator();
            try
            {
                //Create session
                ftp.CreateSession(configuration);
                bool result = ftp.UploadFile(@"C:\Temp\Test.txt", "ShortStory.txt");
            }
            catch (Exception ex)
            {
                txtResponse.Text = ex.Message;
            }
            finally
            {
                //Close and Dispose of session
                //ftp.CloseSession();
            }

        }
    }

    //            //Test server details...  https://www.rebex.net/sftp.net/default.aspx
    //            //UserID/Pwd = demo/password

    //            //string targetUri = @"sftp://test.rebex.net:22";
    //            //string targetUri = @"test.rebex.net:22";
    //            //string targetUri = @"ftp://test.rebex.net:21";
    //           // string targetUri = @"ftp://demo.wftpserver.com:2222";
    //            //string targetUri = @"ftp://demo.wftpserver.com:21";

    //            string targetUri = @"ftp://140.207.97.148:18622";

    //            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(targetUri);
    //            //request.Method = WebRequestMethods.Ftp.GetFileSize;
    //            request.Method = WebRequestMethods.Ftp.ListDirectory;//.ListDirectoryDetails;//.DownloadFile;
    //            //request.Credentials = new NetworkCredential("demo", "password");
    //            //request.Credentials = new NetworkCredential("demo-user", "demo-user");
    //            request.EnableSsl = true;
    //            request.Credentials = new NetworkCredential("herschel_test", "Herschel12345#");
    //            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

    //            //WebClient client = new WebClient();
    //            //client.Credentials = new NetworkCredential("demo", "password");
    //            ////client.Encoding = Encoding.;
    //            //client.
    //            //byte[] newFileData = client.DownloadData(targetUri);
    //            //string fileString = System.Text.Encoding.UTF8.GetString(newFileData);
    //            //txtResponse.Text = fileString;


    //            int i = 1;

    ////            Location: demo.wftpserver.com
    //            //Username: demo - user
    //            //Password: demo - user
    //            //FTP Port: 21
    //            //FTPS Port: 990
    //            //SFTP Port: 2222
}
