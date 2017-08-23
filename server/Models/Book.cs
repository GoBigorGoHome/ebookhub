using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ebookhub.Models
{
    public class Book
    {
        public ObjectId Id { get; set; }
        public string Title { get; set; }
        public List<Author> Authors { get; set; } = new List<Author>();
        public List<Tag> Tags { get; set; } = new List<Tag>();
        public DateTime PublishDateTime { get; set; }
        public List<string> Identifiers { get; set; }
        public List<EBookFile> Files { get; set; } = new List<EBookFile>();
    }
}
