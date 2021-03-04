using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace ProfileBook.Models
{
    [Table("Profile")]
    public class ProfileModel : IEntityBase
    {
        [PrimaryKey, AutoIncrement, Column("id")]
        public int Id { get; set; }
        public string nick_name { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string image_path { get; set; }
        public int user_id { get; set; }
        public DateTime date { get; set; }
    }
}
