using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public abstract class IncCommEnums
    {
        public enum AddCheck
        { 
            cantadd=0,
            canadd=1,
            nolevel=2,
            hasaddlevel=3,
            ok=4
        }

        public enum DeleteCheck
        { 
            noexist=0,
            nodeletelevel=1,
            exist=3,
            hasdeletelevel=4,
            cantdelete=5,
            candelete=6,
            ok=7
        }

        public enum UpdateCheck
        { 
        
            noexist=0,
            exist=1,
            noupdatelevel=2,
            hasupdatelevel=3,
            canupdate=4,
            cantupdate=5,
            ok=6
        }
    }
}