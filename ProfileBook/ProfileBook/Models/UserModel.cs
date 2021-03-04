using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace ProfileBook.Models
{
    [Table("User")]
    public class UserModel : IEntityBase
    {
        [PrimaryKey, AutoIncrement, Column("id")]
        public int Id { get; set; }

        [Unique, Column("login")]
        public string Login { get; set; }

        public string Password { get; set; }

    }
}

