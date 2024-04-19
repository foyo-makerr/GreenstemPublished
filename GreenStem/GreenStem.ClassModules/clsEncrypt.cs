using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenStem.ClassModules
{
   
        public class clsEncrypt
        {
            private static string fCrypt(string Action, string Key, string Src)
            {
                string dest = "";
                int KeyPos = 0;
                int KeyLen = Key.Length;
                int SrcAsc, offset;
                object TmpSrcAsc;
                int SrcPos;

                if (Action == "E")
                {
                    Random random = new Random();
                    offset = random.Next(1, 255);
                    dest = offset.ToString("X2");

                    for (SrcPos = 0; SrcPos < Src.Length; SrcPos++)
                    {
                        SrcAsc = (Convert.ToInt32(Src[SrcPos]) + offset) % 255;

                        if (KeyPos < KeyLen)
                            KeyPos++;
                        else
                            KeyPos = 1;

                        SrcAsc ^= Convert.ToInt32(Key[KeyPos - 1]);
                        dest += SrcAsc.ToString("X2");
                        offset = SrcAsc;
                    }
                }
                else if (Action == "D")
                {
                    if (!string.IsNullOrEmpty(Src))
                    {
                        offset = Convert.ToInt32(Src.Substring(0, 2), 16);

                        for (SrcPos = 2; SrcPos < Src.Length; SrcPos += 2)
                        {
                            SrcAsc = Convert.ToInt32(Src.Substring(SrcPos, 2), 16);

                            if (KeyPos < KeyLen)
                                KeyPos++;
                            else
                                KeyPos = 1;

                            TmpSrcAsc = SrcAsc ^ Convert.ToInt32(Key[KeyPos - 1]);

                            if ((int)TmpSrcAsc <= offset)
                                TmpSrcAsc = 255 + (int)TmpSrcAsc - offset;
                            else
                                TmpSrcAsc = (int)TmpSrcAsc - offset;

                            dest += (char)Convert.ToInt32(TmpSrcAsc);
                            offset = SrcAsc;
                        }
                    }
                }

                return dest;
            }

            public static string gfDeCrypt(string Key, string Src)
            {
                return fCrypt("D", Key.Trim(), Src.Trim());
            }
        
    }
}
