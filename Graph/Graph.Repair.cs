using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TbStb
{
    partial class Graph
    {
        public static void Repair(Stream input, Stream output)
        {
            StreamReader sr = new StreamReader(input, Encoding.UTF8);

            // Read number of vertices
            string line = sr.ReadLine();
            int noOfVert = int.Parse(line);

            HashSet<int>[] graph = new HashSet<int>[noOfVert];

            for (int vId = 0; vId < noOfVert; vId++)
            {
                graph[vId] = new HashSet<int>();
            }

            // Read neighbours of vertices
            for (int vId = 0; vId < noOfVert; vId++)
            {
                line = sr.ReadLine();
                string[] numbers = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                int neighCount = int.Parse(numbers[0]);

                for (int j = 0; j < neighCount; j++)
                {
                    int uId = int.Parse(numbers[j + 1]);
                    if (uId == vId) continue;
                    graph[vId].Add(uId);
                    graph[uId].Add(vId);
                }
            }

            sr = null;

            // Graph completely loaded.
            // Now write back into file.

            StreamWriter sw = new StreamWriter(output, Encoding.UTF8);
            sw.WriteLine(noOfVert);

            for (int vId = 0; vId < noOfVert; vId++)
            {
                HashSet<int> neighs = graph[vId];
                List<int> neighList = new List<int>(neighs);

                neighs = null;
                graph[vId] = null;

                neighList.Sort();

                int deg = neighList.Count;
                sw.Write(deg);

                for (int i = 0; i < deg; i++)
                {
                    sw.Write(" " + neighList[i].ToString());
                }
                sw.WriteLine();
            }

            sw.Flush();
            sw = null;
        }
    }
}
