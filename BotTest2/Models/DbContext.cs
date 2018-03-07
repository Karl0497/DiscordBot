using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BotTest2.Models
{
    class DbContext
    {
        public List<User> Users { get; set; }
        public string path { get; set; }
        public DbContext(){
            path = "../../../data.txt";
            Users = readObject();
            
        }

        public List<User> readObject()
        {
            List<User> result;
            if (!File.Exists(path)){
          
                writeObject(new List<User>());
            }
            using (StreamReader file = new StreamReader(path))
            {
                result = JsonConvert.DeserializeObject<List<User>>(file.ReadToEnd());
                file.Close();
            }
            return result;
        }
        public void writeObject(List<User> myObj)
        {
            using (StreamWriter myFile = new StreamWriter(path))
            {
                myFile.WriteLine(JsonConvert.SerializeObject(myObj));
            }
        }
    }
}
