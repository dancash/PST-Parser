using PSTParse.LTP;
using PSTParse.NDB;

namespace PSTParse
{
    public static class PasswordReset
    {
        public static bool ResetPassword(PSTFile pst)
        {
            var pc = new PropertyContext(SpecialNIDs.NID_MESSAGE_STORE, pst);
            var offset = pc.BTH.Root.BlankPassword(pst);



            return false;
        }
            //SpecialNIDs.NID_MESSAGE_STORE
    }
}
