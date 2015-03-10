using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeus.Helpers;

namespace Zeus.Engine
{
    public class Engine
    {
        private static Engine instance;

        private Engine() {
            
        }

        public static Engine Instance {
            get {
                if (instance == null) {
                    instance = new Engine();
                }
                return instance;
            }
        }
    }
}
