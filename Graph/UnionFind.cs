
namespace TbStb
{
    class UnionFind
    {
        int[] roots;
        int[] rootIndex;
        int rootCount;

        int[] parent;
        int[] rank;

        public UnionFind(int size)
        {
            parent = new int[size];
            rank = new int[size];

            roots = new int[size];
            rootIndex = new int[size];
            rootCount = 0;


            for (int i = 0; i < size; i++)
            {
                parent[i] = i;
                rank[i] = 0;

                rootIndex[i] = -1;
            }
        }

        public void Union(int x, int y)
        {
            int xRoot = Find(x);
            int yRoot = Find(y);

            if (xRoot == yRoot) return;


            // x and y are not already in the same set. Merge them.
            if (rank[xRoot] < rank[yRoot])
            {
                parent[xRoot] = yRoot;
                RemoveRoot(xRoot);
            }
            else if (rank[xRoot] > rank[yRoot])
            {
                parent[yRoot] = xRoot;
                RemoveRoot(yRoot);
            }
            else
            {
                parent[yRoot] = xRoot;
                rank[xRoot]++;
                RemoveRoot(yRoot);
            }
        }

        public int Find(int x)
        {
            if (parent[x] != x)
            {
                parent[x] = Find(parent[x]);
            }

            return parent[x];
        }

        public void MakeRoot(int x)
        {
            if (rootIndex[x] >= 0) return;

            rootIndex[x] = rootCount;
            roots[rootCount] = x;
            rootCount++;
        }

        public void RemoveRoot(int x)
        {
            int xInd = rootIndex[x];

            if (xInd < 0) return;

            rootCount--;

            if (rootCount <= 0) return;

            int lastRoot = roots[rootCount];
            roots[xInd] = lastRoot;
            rootIndex[lastRoot] = xInd;
            rootIndex[x] = -1;
        }

        public int FindRootIndex(int x)
        {
            int xRoot = Find(x);
            return rootIndex[xRoot];
        }

        public int GetRootCount()
        {
            return rootCount;
        }
    }
}
