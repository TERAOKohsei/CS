using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.Common.Communications {
    public class ReceivedCharacterEventArgs : EventArgs {
        public char ReceivedCharacter { get; private set; }

        public ReceivedCharacterEventArgs(char receivedCharacter) {
            ReceivedCharacter = receivedCharacter;
        }
    }

    public class ReceivedLineEventArgs : EventArgs {
        public string ReceivedLine { get; private set; }

        public ReceivedLineEventArgs(string receivedLine) {
            ReceivedLine = receivedLine;
        }
    }
}
