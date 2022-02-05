using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCP.Fileupload
{
    class Program
    {
        /// <summary>
        /// https://medium.com/@asadikhan/uploading-csv-files-to-google-cloud-storage-using-c-net-9eaa951eabf2
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @"E:\Projects\GCP-Exp\GCP.Exp\GCP.Fileupload\Key\gcp-project-001-339305-ccbfe2b05d69.json");

            /*string projectId = “abcd - wfgh - 123456”;
            StorageClient storageClient = StorageClient.Create();
            string bucketName = “MyFirstBucket”;
            try
            {
                storageClient.CreateBucket(projectId, bucketName);
            }
            catch (Google.GoogleApiException e)
            when (e.Error.Code == 409)
            {
                // The bucket already exists.
                Console.WriteLine(e.Error.Message);
            }*/

            // Instantiates a client.
            var storage = StorageClient.Create();

            //Next I specified the parallelization options. I used the easy but powerful C# Parallel ForEach as you can see later.
            ParallelOptions parallelOptions = new ParallelOptions();
            parallelOptions.MaxDegreeOfParallelism = Environment.ProcessorCount * 2;

            //Next I get a list of filename from my upload folder and store the names of the files. 
            //You don’t need to do this. But if your upload gets disrupted and you do not wish to upload the same files again, 
            //this is the fastest way to filter our the files that have already been previously uploaded on GCP. Unfortunately the StorageClient.

            string bucketName = "gcp-project-001-bucket1";
            int count = 0;
            string directory = @"E:\Projects\GCP-Exp\GCP.Exp\GCP.Fileupload\Files";
            var files = Directory.GetFiles(directory);
            List<string> fileNames = new List<string>();
            foreach (var fileName in files)
            {
                fileNames.Add(fileName.Substring(fileName.LastIndexOf("\\") + 1));
            }

            //Next to get a list of files in the cloud, I did the following
            List<string> filesInCloudStorage = new List<string>();
            foreach (var storageObject in storage.ListObjects(bucketName, ""))
            {
                filesInCloudStorage.Add(storageObject.Name);
            }

            //A quick except method allows us to delist the files already uploaded.
            List<string> filesNotAlreadyInCloudStorage = new List<string>();
            filesNotAlreadyInCloudStorage = fileNames.Except(filesInCloudStorage).ToList<string>();

            //And finally the Parallel Foreach. This code will take your parallelization parameters from earlier on and start a multi-threaded execution of file uploads to GCS.
            Parallel.ForEach(filesNotAlreadyInCloudStorage, parallelOptions, fileName => {
                var fileToUpload = directory + "\\" +fileName;
                try
                {
                    UploadFile(bucketName, fileToUpload, ref storage);
                    Console.WriteLine("Uploaded file " +fileToUpload + " — " +count++);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            });

        }

        /// <summary>
        /// The last piece of the puzzle is this method that actually calls the UploadObject method from the Google Cloud Storage client library.
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="localPath"></param>
        /// <param name="storage"></param>
        /// <param name="objectName"></param>
        public static void UploadFile(string bucketName, string localPath, ref StorageClient storage, string objectName = null)
        {
            using (var f = File.OpenRead(localPath))
            {
                objectName = objectName ?? Path.GetFileName(localPath);
                storage.UploadObject(bucketName, objectName, null, f);
                Console.WriteLine($"Uploaded { objectName}.");
            }
        }
    }
}
