using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindDuplicateFileApp.Entities
{
    public class FileInfo
    {   
        /// <summary>
        ///Record for criterias of user input data/meta data 
        /// (Date modified, checksum, file type etc.)
        /// </summary>

        public record FileRecord
        {
            public string FileName {  get; set; }
            public string DateCreated { get; set; }
            public string DateModified {  get; set; }
            public string FileType {  get; set; }
            public string Checksum {  get; set; }
            public double Size {  get; set; }
        }
    }
}
