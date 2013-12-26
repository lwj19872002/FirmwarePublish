using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FirmwarePublish
{
    class Program
    {
        static void Main(string[] args)
        {
            Publish Publisher = new Publish();

            Publisher.DoPublish("publish.ini", "PublishLog.txt");
        }
    }
}
