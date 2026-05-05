using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace CommonClassLibrary.Controllers
{
    public class FileController
    {
        public string CreateFileWithNumber(string pathSave, string fileName, string typeFile) //test-1
        {
            int count = 1;
            string nameCheck;
            string result;

            nameCheck = pathSave + fileName + typeFile;


            while (File.Exists(nameCheck) == true)
            {
                nameCheck = pathSave + fileName + "-" + count + typeFile;
                count += 1;
            };

            result = nameCheck;
            File.Create(result).Dispose();
            return result;
        }

        public string CreateFile(string pathSave, string fileName, string typeFile)
        {
            string nameCheck;
            string result;

            nameCheck = pathSave + fileName + typeFile;

            result = nameCheck;
            File.Create(result).Dispose();
            return result;
        }


        public void CreateDirectory(string pathSave)
        {
            if (Directory.Exists(pathSave) == false)
            {
                Directory.CreateDirectory(pathSave);
            }
        }




    }
}