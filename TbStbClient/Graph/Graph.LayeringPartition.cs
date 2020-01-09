using System.Collections.Generic;

namespace TbStb.Client
{
    partial class Graph
    {
        private int[][][] layPartition = null;

        public int[][][] LayeringPartition
        {
            get
            {
                if (layPartition == null)
                {
                    layPartition = GetLayeringPartition(0);
                }

                return layPartition;
            }
        }

        private int[][][] GetLayeringPartition(int startId)
        {
            BfsResult bfs = Bfs(startId);

            // Partition vertices into layers
            List<List<int>> layers = new List<List<int>>();

            for (int vId = 0; vId < Vertices; vId++)
            {
                int dist = bfs.Distances[vId];
                if (dist < 0) continue;

                while (layers.Count <= dist)
                {
                    layers.Add(new List<int>());
                }

                layers[dist].Add(vId);
            }

            int[][][] layPart = new int[layers.Count][][];

            UnionFind uf = new UnionFind(Vertices);

            for (int l = layers.Count - 1; l > 0; l--)
            {

                // Determine connected components of current layer
                foreach (int vId in layers[l])
                {
                    uf.MakeRoot(vId);
                }

                foreach (int vId in layers[l])
                {
                    foreach (int neighId in graph[vId])
                    {
                        if (bfs.Distances[neighId] < l) continue;
                        uf.Union(vId, neighId);
                    }
                }

                int clusterCount = uf.GetRootCount();
                List<int>[] clusterList = new List<int>[clusterCount];

                for (int i = 0; i < clusterCount; i++)
                {
                    clusterList[i] = new List<int>();
                }

                foreach (int vId in layers[l])
                {
                    int clInd = uf.FindRootIndex(vId);
                    clusterList[clInd].Add(vId);
                }

                // All connected components are now in clusterList.
                // Transform in output format.

                layPart[l] = new int[clusterList.Length][];

                for (int i = 0; i < clusterList.Length; i++)
                {
                    layPart[l][i] = clusterList[i].ToArray();
                }
            }

            // Add lowest layer (i.e. start vertex).
            layPart[0] = new int[][] { new int[] { startId } };

            return layPart;
        }
    }
}
