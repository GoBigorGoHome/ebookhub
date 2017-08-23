using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ebookhub.Models
{
    public class Author
    {
        public string FullName { get; set; }
    }
}
