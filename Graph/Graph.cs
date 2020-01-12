using System;
using System.IO;
using System.Text;

namespace TbStb
{
    partial class Graph
    {
        private int[][] graph;
        private int totalEdges = 0;

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
                totalEdges += neighCount;

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

        public string Name { get; set; }

        public int Vertices
        {
            get
            {
                return graph.Length;
            }
        }

        public int Edges
        {
            get
            {
                return totalEdges;
            }
        }

        public void Print(StreamWriter writer)
        {
            writer.WriteLine(Vertices);

            for (int vId = 0; vId < Vertices; vId++)
            {
                int deg = graph[vId].Length;
                writer.Write(deg);

                for (int i = 0; i < deg; i++)
                {
                    int uId = graph[vId][i];
                    writer.Write(" " + uId.ToString());
                }
                writer.WriteLine();
            }

            writer.Flush();
        }
    }
}
