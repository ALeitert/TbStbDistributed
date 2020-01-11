using System.Collections.Generic;

namespace TbStb
{
    partial class Graph
    {
        private int[][] layPartition = null;

        public int[][] LayeringPartition
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

        private int[][] GetLayeringPartition(int startId)
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

            List<List<int>> layPart = new List<List<int>>();
            UnionFind uf = new UnionFind(Vertices);

            for (int l = layers.Count - 1; l > 0; l--)
            {
                // Determine connected components of current layer.
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

                int startInd = layPart.Count;
                int clusterCount = uf.GetRootCount();

                for (int i = 0; i < clusterCount; i++)
                {
                    layPart.Add(new List<int>());
                }

                foreach (int vId in layers[l])
                {
                    int clInd = uf.FindRootIndex(vId);
                    layPart[startInd + clInd].Add(vId);
                }
            }

            // Add lowest layer (i.e. start vertex).
            List<int> sCluster = new List<int>();
            sCluster.Add(startId);
            layPart.Add(sCluster);


            // Sort vertices in clusters.
            foreach (List<int> cluster in layPart)
            {
                cluster.Sort();
            }

            // Sort clusters
            layPart.Sort(new ClusterSorter());

            int[][] result = new int[layPart.Count][];

            for (int i = 0; i < layPart.Count; i++)
            {
                result[i] = layPart[i].ToArray();
            }

            return result;
        }

        private class ClusterSorter : IComparer<List<int>>
        {
            int IComparer<List<int>>.Compare(List<int> x, List<int> y)
            {
                return x[0].CompareTo(y[0]);
            }
        }
    }
}
