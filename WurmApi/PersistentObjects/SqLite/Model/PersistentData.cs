using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.WurmApi.PersistentObjects.SqLite.Model
{
    public class PersistentData
    {
        public string CollectionId { get; set; }
        public string ObjectId { get; set; }
        public string Data { get; set; }
    }
}
