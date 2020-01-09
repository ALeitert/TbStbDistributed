using System;
using System.IO;
using System.Text;

namespace TbStb.Client
{
    partial class Graph
    {
        private int[][] graph;

        public Graph(Stream input)
        {
            StreamReader sr = new StreamReader(input, Encoding.UTF8);

            // Read number of vertices
            string line = sr.ReadLine();
            int noOfVert = int.Parse(line);

            graph = new int[noOfVert][];

            // Read neighbours of vertices
            for (int vId = 0; vId < noOfVert; vId++)
            {
                line = sr.ReadLine();
                string[] numbers = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                int neighCount = int.Parse(numbers[0]);
                graph[vId] = new int[neighCount];

                for (int j = 0; j < neighCount; j++)
                {
                    graph[vId][j] = int.Parse(numbers[j + 1]);
                }
            }

        }

        public int[] this[int vId]
        {
            get
            {
                return graph[vId];
            }
        }

        public int Vertices
        {
            get
            {
                return graph.Length;
            }
        }

    }
}
