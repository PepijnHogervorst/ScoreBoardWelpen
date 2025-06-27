using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfScoreboard
{
    public class Globals
    {
        public static Classes.MQTTClient MQTTClient         = new Classes.MQTTClient();
        public static Classes.Storage       Storage         = new Classes.Storage();

        public const int MaxNrOfGroups = 6;

        public static int GroupTurn = 1;

        public Globals()
        {
        }

    }
}
