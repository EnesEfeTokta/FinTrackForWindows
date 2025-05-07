using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinTrack.Codes.Verifications
{
    public class CodeVerifier
    {
        private byte[] CodeGenerator(short codeLength)
        {
            Random random = new Random();

            byte[] code = new byte[codeLength];
            for (int i = 0; i < code.Length; i++)
            {
                code[i] = (byte)random.Next(0, 10);
            }
            return code;
        }
    }
}
