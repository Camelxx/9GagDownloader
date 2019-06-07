using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _9gagDownloader
{
    public partial class Downloader : Form
    {
        int ReportCount = 0;
        int ReportMax = 5000;
        public Logger log;
        WebClient wc = new WebClient();

        public Downloader()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            log = LogManager.GetCurrentClassLogger();
        }

        private void BtnDownload_Click(object sender, EventArgs e)
        {
            if (cbCreatFolders.Checked)
            {
                if (!Directory.Exists(txtDownloadPath.Text + "\\Video")) { Directory.CreateDirectory(txtDownloadPath.Text + "\\Video"); }
                if (!Directory.Exists(txtDownloadPath.Text + "\\Images")) { Directory.CreateDirectory(txtDownloadPath.Text + "\\Images"); }
            }

            if (cbImage.Checked)
            {
                DownloadImage(txtUrl.Text);
            }
            else if(cbVideo.Checked)
            {
                DownloadVideo(txtUrl.Text);
            }
        }

        private void DownloadVideo(string videoUrl)
        {
            string dwnPath = txtDownloadPath.Text + "\\Video" + "\\" + txtFilename.Text + ".mp4";
            if (!File.Exists(dwnPath))
            {
                wc = new WebClient();
                string xUrl = videoUrl.Replace("webm", "");
                xUrl = xUrl.Substring(0, xUrl.Length - 4) + ".mp4";
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(VideoFileDownloadComp);
                Uri imgUrl = new Uri(xUrl);
                wc.DownloadFileAsync(imgUrl, dwnPath);
            }
            else { Report(true, "File already exist with the same name "); }
        }

        private void VideoFileDownloadComp(object sender, AsyncCompletedEventArgs e)
        {

            string dwnPath = txtDownloadPath.Text + "\\Video" + "\\" + txtFilename.Text + ".mp4";
            if (File.Exists(dwnPath))
            {
                Report(false, "Video File Complete !..");
                Reset();
            }
            else { Report(true, "Video File Failed !.."); }    
        }

        private void DownloadImage(string imageUrl)
        {
            string dwnPath = txtDownloadPath.Text + "\\Images" + "\\" + txtFilename.Text + ".jpg";
            if (!File.Exists(dwnPath))
            {
                wc = new WebClient();
                string xUrl = imageUrl.Replace("webp", "");
                xUrl = xUrl.Substring(0, xUrl.Length - 3) + ".jpg";
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(ImageFileDownloadComp);
                Uri imgUrl = new Uri(xUrl);
                wc.DownloadFileAsync(imgUrl, dwnPath);
            }
            else { Report(true, "File already exist with the same name "); }
        }

        private void ImageFileDownloadComp(object sender, AsyncCompletedEventArgs e)
        {
            //Check if file exist
            string ses = txtDownloadPath.Text + "\\Images" + "\\" + txtFilename.Text + ".jpg";
            if (File.Exists(ses))
            {
                Report(false, "Image Download Completed !...");
                Reset("img");
            }
            else { Report(true, "Image Download Failed !..."); }
        }

        private void Reset(string type =null)
        {
            if (type == "img")
            {
                txtUrl.Text = "";
                txtFilename.Text = "";
                cbImage.Checked = false;
            }
            else
            {
                txtUrl.Text = "";
                txtFilename.Text = "";
                cbVideo.Checked = false;
            }
        }

        #region Process/Info Feedback
        private void Report(string message)
        {
            Report(false, message);
        }
        private void Report(Exception ex)
        {
            Report(true, ex.Message);
        }
        private void Report(bool error, string message)
        {
            string displayMSG = DateTime.Now.ToString() + " : " + message;
            ReportCount++;
            if (InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate ()
                {
                    if (ReportCount > ReportMax)
                    {
                        lbInfo.Items.Clear();
                        ReportCount = 0;
                    }

                    if (error)
                    {
                        log.Error(message);
                        lbError.Items.Insert(0, displayMSG);
                    }
                    else
                    {
                        log.Info(message);
                        lbInfo.Items.Insert(0, displayMSG);
                    }
                });
            }
            else
            {
                if (ReportCount > ReportMax)
                {
                    lbInfo.Items.Clear();
                    ReportCount = 0;
                }

                if (error)
                {
                    log.Error(message);
                    lbError.Items.Insert(0, displayMSG);
                }
                else
                {
                    log.Info(message);
                    lbInfo.Items.Insert(0, displayMSG);
                }
            }
        }




        #endregion

        private void CbSelecet_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void BtnSaveSettings_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
            Report(false, "Settings Saved.");
        }

        private void BtOpen_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog theDialog = new FolderBrowserDialog();
        
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                txtDownloadPath.Text = theDialog.SelectedPath.ToString();
            }
        }
    }
}
