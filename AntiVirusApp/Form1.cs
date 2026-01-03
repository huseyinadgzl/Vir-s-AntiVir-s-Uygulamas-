using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

namespace AntiVirusApp
{
    public partial class Form1 : Form
    {
        private TextBox txtPath;
        private Button btnSelect;
        private Button btnCreateTest;
        private Button btnScan;
        private ListBox lstResults;
        private Label lblStatus;
        private ProgressBar progressBar1;

        private List<string> virusDatabase = new List<string>();
        public Form1()
        {
            this.Text = "Mini Antivirüs - Kod ile Arayüz Oluşturma";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            ArayuzuOlustur();

            virusDatabase.Add("BİLİNMEYEN_HASH_ÖRNEĞİ");
            InitializeComponent();
        }
        private void ArayuzuOlustur()
        {
            txtPath = new TextBox();
            txtPath.Location = new Point(12, 12);
            txtPath.Size = new Size(450, 25);
            txtPath.ReadOnly = true;
            this.Controls.Add(txtPath);

            btnSelect = new Button();
            btnSelect.Text = "Klasör Seç";
            btnSelect.Location = new Point(470, 10);
            btnSelect.Size = new Size(100, 27);
            btnSelect.Click += BtnSelect_Click;
            this.Controls.Add(btnSelect);

            btnCreateTest = new Button();
            btnCreateTest.Text = "Test Virüsü Oluştur";
            btnCreateTest.Location = new Point(12, 50);
            btnCreateTest.Size = new Size(275, 40);
            btnCreateTest.BackColor = Color.LightSalmon;
            btnCreateTest.Click += BtnCreateTest_Click;
            this.Controls.Add(btnCreateTest);

            btnScan = new Button();
            btnScan.Text = "TARAMAYI BAŞLAT";
            btnScan.Location = new Point(295, 50);
            btnScan.Size = new Size(275, 40);
            btnScan.BackColor = Color.LightGreen;
            btnScan.Font = new Font(this.Font, FontStyle.Bold);
            btnScan.Click += BtnScan_Click;
            this.Controls.Add(btnScan);

            lblStatus = new Label();
            lblStatus.Text = "Durum: Hazır";
            lblStatus.Location = new Point(12, 100);
            lblStatus.AutoSize = true;
            this.Controls.Add(lblStatus);

            progressBar1 = new ProgressBar();
            progressBar1.Location = new Point(12, 120);
            progressBar1.Size = new Size(560, 20);
            this.Controls.Add(progressBar1);

            lstResults = new ListBox();
            lstResults.Location = new Point(12, 150);
            lstResults.Size = new Size(560, 300);
            this.Controls.Add(lstResults);
        }


        private void BtnSelect_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtPath.Text = fbd.SelectedPath;
                }
            }
        }

        private void BtnCreateTest_Click(object sender, EventArgs e)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TestVirusu.txt");
            File.WriteAllText(path, "BU_DOSYA_ANTIVIRUS_TESTI_ICINDIR_ZARARSIZDIR");

            string signature = GetMD5Hash(path);
            if (!virusDatabase.Contains(signature))
            {
                virusDatabase.Add(signature);
            }
            MessageBox.Show("Masaüstünde 'TestVirusu.txt' oluşturuldu.\nİmzası veritabanına eklendi!", "Hazır");
        }

        private async void BtnScan_Click(object sender, EventArgs e)
        {
            string targetDirectory = txtPath.Text;

            if (string.IsNullOrWhiteSpace(targetDirectory) || !Directory.Exists(targetDirectory))
            {
                MessageBox.Show("Lütfen önce bir klasör seçin!");
                return;
            }

            lstResults.Items.Clear();
            string[] files;

            try
            {
                files = Directory.GetFiles(targetDirectory, "*.*", SearchOption.TopDirectoryOnly);
            }
            catch
            {
                MessageBox.Show("Klasöre erişilemedi."); return;
            }

            progressBar1.Maximum = files.Length;
            progressBar1.Value = 0;
            int virusCount = 0;

            await Task.Run(() =>
            {
                foreach (string file in files)
                {
                    string fileHash = GetMD5Hash(file);
                    string fileName = Path.GetFileName(file);

                    this.Invoke(new Action(() =>
                    {
                        if (virusDatabase.Contains(fileHash))
                        {
                            lstResults.Items.Add($"[!!! VİRÜS !!!] {fileName} -> KARANTİNA");
                            virusCount++;
                            QuarantineFile(file);
                            lstResults.BackColor = Color.MistyRose;
                        }
                        else
                        {
                            lstResults.Items.Add($"[TEMİZ] {fileName}");
                        }
                        progressBar1.Value++;
                        lblStatus.Text = $"İnceleniyor: {fileName}";
                    }));
                    System.Threading.Thread.Sleep(50);
                }
            });

            lblStatus.Text = "Tarama Tamamlandı.";
            if (virusCount == 0) lstResults.BackColor = Color.White;
            MessageBox.Show($"Tarama bitti. {virusCount} tehdit bulundu.");
        }

        private string GetMD5Hash(string filePath)
        {
            try
            {
                using (var md5 = MD5.Create())
                using (var stream = File.OpenRead(filePath))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
                }
            }
            catch { return "ERR"; }
        }

        private void QuarantineFile(string filePath)
        {
            try
            {
                string newPath = filePath + ".karantina";
                if (File.Exists(newPath)) File.Delete(newPath);
                File.Move(filePath, newPath);
            }
            catch { }
        }

       // [STAThread]
        //static void Main()
        //{
        //    Application.EnableVisualStyles();
        //    Application.SetCompatibleTextRenderingDefault(false);
        //    Application.Run(new Form1());
        //}

    }
}
