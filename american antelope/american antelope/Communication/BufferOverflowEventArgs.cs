using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.Common.Communication {
    public class BufferOverflowEventArgs : EventArgs {
        public char OverflowedCharacter { get; private set; }

        public BufferOverflowEventArgs(char overflowedCharacter) {
            OverflowedCharacter = overflowedCharacter;
        }
    }
}
