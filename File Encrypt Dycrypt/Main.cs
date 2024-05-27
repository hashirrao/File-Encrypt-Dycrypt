using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace File_Encrypt_Dycrypt
{
    public partial class Main : Form
    {

        bool move;
        int x, y;
        String[] files;
        public Main()
        {
            InitializeComponent();
        }

        private void browseBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Multiselect = true;
            if (od.ShowDialog() == DialogResult.OK)
            {
                files = od.FileNames;
                pathBox.Clear();
                foreach(string s in files)
                {
                    pathBox.Text += (", " + s);
                }
            }
        }

        private void encryptRB_CheckedChanged(object sender, EventArgs e)
        {
            if (encryptRB.Checked)
            {
                decryptRB.Checked = false;
            }
        }

        private void decryptRB_CheckedChanged(object sender, EventArgs e)
        {
            if (decryptRB.Checked)
            {
                encryptRB.Checked = false;
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            encryptRB.Checked = true;
            new EDC();
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
            if (backgroundWorker.IsBusy)
            {
                return;
            }
            progressBar1.Minimum = 0;
            progressBar1.Value = 0;
            backgroundWorker.RunWorkerAsync();
        }

        private void topBar_MouseDown(object sender, MouseEventArgs e)
        {
            move = true;
            x = e.X;
            y = e.Y;
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void closeBtn_MouseEnter(object sender, EventArgs e)
        {
            closeBtn.BackgroundImage = Properties.Resources.closeEnter;
        }

        private void closeBtn_MouseLeave(object sender, EventArgs e)
        {
            closeBtn.BackgroundImage = Properties.Resources.close;
        }

        private void closeBtn_MouseDown(object sender, MouseEventArgs e)
        {
            closeBtn.BackgroundImage = Properties.Resources.closePress;
        }

        private void topBar_MouseUp(object sender, MouseEventArgs e)
        {
            move = false;
        }

        private void topBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (move)
            {
                this.SetDesktopLocation(MousePosition.X - x, MousePosition.Y - y);
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            minimizeBtn.BackgroundImage = Properties.Resources.minimizePress;
        }

        private void minimizeBtn_MouseEnter(object sender, EventArgs e)
        {
            minimizeBtn.BackgroundImage = Properties.Resources.minimizeEnter;
        }

        private void minimizeBtn_MouseUp(object sender, MouseEventArgs e)
        {
            minimizeBtn.BackgroundImage = Properties.Resources.minimizeEnter;
        }

        private void minimizeBtn_MouseLeave(object sender, EventArgs e)
        {
            minimizeBtn.BackgroundImage = Properties.Resources.minimize;
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (String.IsNullOrEmpty(passwordBox.Text))
            {
                MessageBox.Show("Password empty. Please enter your password");
                return;
            }

            // Get file content and key for encrypt/decrypt
            int index = 1;
            int process = files.Length;
            try
            {
                foreach (String s in files)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (!File.Exists(s))
                        {
                            MessageBox.Show("File does not exist.");
                            return;
                        }
                        byte[] result;
                        if (encryptRB.Checked)
                        {
                            result = EDC.Encrypt(s, passwordBox.Text);
                        }
                        // Decrypt
                        else
                        {
                            result = EDC.Decrypt(s, passwordBox.Text);
                        }
                        // Save result to new file with the same extension
                        String fileExt = Path.GetExtension(pathBox.Text);
                        File.WriteAllBytes(s, result);
                    }
                    if (!backgroundWorker.CancellationPending)
                    {
                        backgroundWorker.ReportProgress(index++ * 100 / process);
                    }
                }
            }
            catch
            {
                MessageBox.Show("File is in use.\nClose other program is using this file and try again.");
                return;
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(e.Error == null)
            {
                Thread.Sleep(10);
                if(encryptRB.Checked)
                    MessageBox.Show("File encrypted successfully...");
                else
                    MessageBox.Show("File decrypted successfully...");
            }
        }

        private void closeBtn_MouseUp(object sender, MouseEventArgs e)
        {
            closeBtn.BackgroundImage = Properties.Resources.closeEnter;
        }
    }
}
