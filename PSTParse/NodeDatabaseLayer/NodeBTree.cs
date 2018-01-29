namespace PSTParse.NodeDatabaseLayer
{
    public class NodeBTree
    {
        public BREF RootLocation;
        public BTPage Root;

        public NodeBTree(BREF root)
        {
            this.RootLocation = root;
        }
    }
}
