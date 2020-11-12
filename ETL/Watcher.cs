using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL
{
    class Watcher
    {
        FileSystemWatcher fileSystemWatcher;
        Logger logger;
        string sourceDir;
        string targetDir;

        public Watcher(string sourceDir, string targetDir)
        {
            this.sourceDir = sourceDir;
            this.targetDir = targetDir;

            Directory.CreateDirectory(sourceDir);
            Directory.CreateDirectory(targetDir);

            logger = new Logger(targetDir);

            fileSystemWatcher = new FileSystemWatcher(sourceDir);
            fileSystemWatcher.Filter = "*.txt";
            fileSystemWatcher.Created += OnCreated;
        }

        private void OnCreated(object sender, FileSystemEventArgs e) 
        {
            string path = e.FullPath;
            try
            {
                FileInfo fileInfo = new FileInfo(path);
                DateTime dateTime = fileInfo.LastWriteTime;
                string year = dateTime.ToString("yyyy");
                string month = dateTime.ToString("MM");
                string day = dateTime.ToString("dd");

                byte[] key, iv;
                (key, iv) = Encryptor.CreateKeyAndIV();
                
                //encryting
                byte[] content = File.ReadAllBytes(path);
                content = Encryptor.Encrypt(content, key, iv);
                File.WriteAllBytes(path, content);

                string name = Path.GetFileNameWithoutExtension(path);
                string extension = Path.GetExtension(path);
                string fileName = Path.GetFileName(path);
                string gzPath = Path.Combine(targetDir, name + ".gz");
                string newPath = Path.Combine(targetDir, name + extension);

                //compressing and copying to targetDir
                Archive.Compress(path, gzPath);

                //decompressing
                Archive.Decompress(gzPath, newPath);

                //additional archive
                string newArchiveDir = Path.Combine(targetDir, $@"archive\{year}\{month}\{day}");
                string newArchiveName = dateTime.ToString($@"{name}_yyyy_MM_dd_HH_mm_ss") + extension;
                string newArchivePath = Path.Combine(newArchiveDir, newArchiveName);

                Directory.CreateDirectory(newArchiveDir);
                File.Copy(newPath, newArchivePath);

                //decrypting
                content = File.ReadAllBytes(newPath);
                content = Encryptor.Decrypt(content, key, iv);
                File.WriteAllBytes(newPath, content);

                logger.Log($"File \"{fileName}\" is moved from \"{sourceDir}\" to \"{targetDir}\" successfully!");
            }
            catch 
            {
                logger.Log($"File isn't moved...");
            }
        }

        public void Start()
        {
            fileSystemWatcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            fileSystemWatcher.EnableRaisingEvents = false;
        }
    }
}
