using System;
using System.ComponentModel.DataAnnotations;

namespace TheCountBot.Data.Models
{
    public class MessageEntry
    {
        [Key]
        public int MessageEntryId
        {
            get;
            set;   
        }

        [MaxLength( 64 )]
        public string Username
        {
            get;
            set;   
        }

        public int Number
        {
            get;
            set;   
        }

        public bool Correct
        {
            get;
            set;   
        }

        public DateTime Timestamp
        {
            get;
            set;   
        }

        public override string ToString()
        {
            return $"{Username} {Correct} {Number} {Timestamp}\n";
        }
    }
}