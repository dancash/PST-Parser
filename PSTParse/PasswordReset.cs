using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSTParse.Message_Layer;
using PSTParse.NDB;

namespace PSTParse
{
    public static class PasswordReset
    {
        public static bool ResetPassword()
        {
            var pc = new PropertyContext(SpecialNIDs.NID_MESSAGE_STORE);
            var offset = pc.BTH.Root.BlankPassword();



            return false;
        }
            //SpecialNIDs.NID_MESSAGE_STORE
    }
}
