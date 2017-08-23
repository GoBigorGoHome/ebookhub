using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ebookhub.Models
{
    public enum FileType
    {
        mobi,
        epub,
        unknown
    }
    public class EBookFile
    {
        public string RelativeFilePath { get; set; }
        public FileType FileType { get; set; }
    }
}