using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ImageDuplication
{
    class Program
    {
        static void Main(string[] args)
        {
            DetectDuplicate(@"E:\Dev_Applications\AP_SEEDS\AP_SEEDS\Assets\Images");
            Console.ReadLine();
        }

        public static void DetectDuplicate(string path)
        {
            try
            {
                ConsoleKeyInfo cki;
                double totalSize = 0;

                //Get all files from given directory
                var fileLists = Directory.GetFiles(path);
                int totalFiles = fileLists.Length;

                List<FileDetails> finalDetails = new List<FileDetails>();
                List<string> ToDelete = new List<string>();
                finalDetails.Clear();
                //loop through all the files by file hash code
                foreach (var item in fileLists)
                {
                    using (var fs = new FileStream(item, FileMode.Open, FileAccess.Read))
                    {
                        finalDetails.Add(new FileDetails()
                        {
                            FileName = item,
                            FileHash = BitConverter.ToString(SHA1.Create().ComputeHash(fs)),
                        });
                    }
                }
                //group by file hash code
                var similarList = finalDetails.GroupBy(f => f.FileHash)
                    .Select(g => new { FileHash = g.Key, Files = g.Select(z => z.FileName).ToList() });


                //keeping first item of each group as is and identify rest as duplicate files to delete
                ToDelete.AddRange(similarList.SelectMany(f => f.Files.Skip(1)).ToList());
                Console.WriteLine("Total duplicate files - {0}", ToDelete.Count);
                //list all files to be deleted and count total disk space to be empty after delete
                if (ToDelete.Count > 0)
                {
                    Console.WriteLine("Files to be deleted - ");
                    foreach (var item in ToDelete)
                    {
                        Console.WriteLine(item);
                        FileInfo fi = new FileInfo(item);
                        totalSize += fi.Length;
                    }
                }
                //Console.ForegroundColor = ConsoleColor.Red;
                //Console.WriteLine("Total space free up by -  {0}mb", Math.Round((totalSize / 1000000), 6).ToString());
                //Console.ForegroundColor = ConsoleColor.White;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

    }


    public class FileDetails
    {
        public string FileName { get; set; }
        public string FileHash { get; set; }
    }

}
