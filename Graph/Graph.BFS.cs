using System.Collections.Generic;

namespace TbStb
{
    public struct BfsResult
    {
        public int StartId { get; set; }
        public int[] VertexOrder { get; set; }
        public int[] Distances { get; set; }
        public int[] ParentIds { get; set; }
    }

    partial class Graph
    {
        /// <summary>
        /// Runs a BFS.
        /// </summary>
        /// <param name="startId">
        /// The ID of the start vertex.
		/// </param>
        /// <returns>
        /// The order in which vertices were visited, their distance from the start vertex, and their parents in the BFS-tree.
		/// </returns>
        public BfsResult Bfs(int startId)
        {
            int[] distances = new int[Vertices];
            int[] parIds = new int[Vertices];

            // The queue is made manually to get the vertex order without extra data structure.
            int[] queue = new int[Vertices];

            for (int i = 0; i < Vertices; i++)
            {
                distances[i] = -1;
                parIds[i] = -1;
                queue[i] = -1;
            }


            // Pointers for the queue.
            // Indices for the first and last vertex in the queue. 
            int curPos = 0;
            int maxPos = 0;


            // Start vertex
            distances[startId] = 0;
            queue[curPos] = startId;


            // Iterate throw all vertices in queue until it is empty.
            for (; curPos <= maxPos; curPos++)
            {
                int vId = queue[curPos];
                int[] neighs = this[vId];

                for (int i = 0; i < neighs.Length; i++)
                {
                    int neigId = neighs[i];

                    if (distances[neigId] == -1)
                    {
                        distances[neigId] = distances[vId] + 1;
                        parIds[neigId] = vId;

                        // Add to queue.
                        maxPos++;
                        queue[maxPos] = neigId;
                    }
                }
            }

            return new BfsResult
            {
                StartId = startId,
                Distances = distances,
                VertexOrder = queue,
                ParentIds = parIds
            };
        }

        public int[] FindLargestCC()
        {
            int[] largestCC = new int[0];
            bool[] visited = new bool[Vertices];

            for (int vId = 0; vId < Vertices; vId++)
            {
                if (visited[vId]) continue;
                visited[vId] = true;

                List<int> queue = new List<int>();
                queue.Add(vId);

                for (int i = 0; i < queue.Count; i++)
                {
                    int uId = queue[i];
                    int[] neighs = this[uId];

                    for (int j = 0; j < neighs.Length; j++)
                    {
                        int neigId = neighs[j];

                        if (!visited[neigId])
                        {
                            visited[neigId] = true;
                            queue.Add(neigId);
                        }
                    }
                }

                if (queue.Count > largestCC.Length)
                {
                    largestCC = queue.ToArray();
                    if (largestCC.Length >= (Vertices + 1) / 2) break;
                }
            }

            return largestCC;
        }
    }
}
