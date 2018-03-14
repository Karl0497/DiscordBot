using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BotTest2.Models
{
 
    public class User
    {
       
        public ulong Id { get; set; }
        public OverwatchProfile OverwatchProfile { get; set; }

    }
}
