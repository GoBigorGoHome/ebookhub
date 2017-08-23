using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ebookhub.Models
{
    public class User
    {
        public ObjectId Id { get; set; }
        public string KindleMail { get; set; }
        public string Name { get; set; }
    }
}
