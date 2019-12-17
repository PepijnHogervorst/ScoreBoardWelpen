using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreBoardWelpen
{
    public class Globals
    {
        public static Classes.Communication Communication   = new Classes.Communication();
        public static Classes.GPIO          GPIO            = new Classes.GPIO();
        public static Classes.Storage       Storage         = new Classes.Storage();

        public const int MaxNrOfGroups = 6;

        public static int GroupTurn = 1;

        public Globals()
        {
        }

    }
}
