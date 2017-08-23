using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ebookhub.Models
{
    public class Tag
    {
        public string Name { get; set; }
    }
}
