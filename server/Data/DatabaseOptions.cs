using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ebookhub.Data
{
    public class DatabaseOptions
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
    }
}