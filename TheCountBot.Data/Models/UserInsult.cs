using System;
using System.ComponentModel.DataAnnotations;

namespace TheCountBot.Data.Models
{
    public class UserInsult
    {
        [Key]
        public int Id
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }
    }
}