using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FindDuplicateFileApp.Entities.FileInfo;

namespace FindDuplicateFileApp.Entities
{
    //Delete logic for files
    //Comparison logic for files
    public class DuplicateFileFinder
    {
        private List<FileRecord> userSelFiles;
        public DuplicateFileFinder() 
        {
            userSelFiles = new List<FileRecord>();
        }

        //Add file to list of file records
        public bool AddFile(FileRecord file)
        {
            if (file != null)
            {
                userSelFiles.Add(file);
                return true;
            }

            return false;
        }

        //Delete file from record list
        public bool DeleteFileAt(int index)
        {
            if(index != -1)
            {
                userSelFiles.RemoveAt(index);
                return true;
            }
            return false;
        }

        public FileRecord GetFileRecordAt(int index)
        {
            FileRecord fileRec = null;

            if (index!= -1)
            {
                fileRec = userSelFiles[index];
            }

            return fileRec;
        }

        public int Count()
        {
            return userSelFiles.Count;
        }

        /// <summary>
        /// Compare criterias of two filnames at a time
        /// Each file will be compared to each other
        /// If all criterias are equal, add to tuple list
        /// </summary>
        /// <returns>a list of FileRecord tuples (filename1, filename2)</returns>
        public List<(FileRecord, FileRecord)> GetAllIdenticalFiles()
        {
            List<(FileRecord, FileRecord)> identicalPairOfFiles = new List<(FileRecord, FileRecord)>();

            for (int i = 0; i < userSelFiles.Count; i++)
            {
                FileRecord fileCmpr1 = userSelFiles[i];

                if (fileCmpr1 == null)
                    continue;

                for(int j = i+1; j<userSelFiles.Count; j++)
                {
                    FileRecord fileCmpr2 = userSelFiles[j];

                    if(fileCmpr2 == null)
                        continue;

                    if(fileCmpr1.DateCreated == fileCmpr2.DateCreated &&
                       fileCmpr1.DateModified == fileCmpr2.DateModified &&
                       fileCmpr1.FileType == fileCmpr2.FileType &&
                       fileCmpr1.Checksum == fileCmpr2.Checksum &&
                       fileCmpr1.Size == fileCmpr2.Size)
                    {
                        identicalPairOfFiles.Add((fileCmpr1, fileCmpr2));
                        Console.WriteLine($"Pair of equal files: {fileCmpr1.FileName} and {fileCmpr2.FileName}");
                    }
                }
            }

            return identicalPairOfFiles;
        }

        /// <summary>
        /// Prepare list with files to delete
        /// </summary>
        /// <returns>list of filepaths</returns>
        /*public List<string> FilesToDelete()
        {
            List<(FileRecord, FileRecord)> identicalPairOfFiles = GetAllIdenticalFiles();
            List<string> filesToDelete = new List<string>();

            for (int i = 0; i < identicalPairOfFiles.Count; i++)
            {
                filesToDelete.Add(identicalPairOfFiles[i].Item2.FileName);
            }

            return filesToDelete;
        }*/
    }
}
