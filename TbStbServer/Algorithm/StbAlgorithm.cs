using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TbStb.Server
{
    class StbAlgorithm : Algorithm
    {
        private struct Dependent
        {
            public int vId;
            public int ccInd;
        }

        private int rho = 1;

        int[][][] potPartners;
        int[][] potPartCtrs;

        BitArray isValid;


        public StbAlgorithm() : base() { }
        public StbAlgorithm(IEnumerable<ClientBase> clients) : base(clients) { }


        protected override void OnPreprocess()
        {
            base.OnPreprocess();
            potPartners = new int[GraphSize][][];
            potPartCtrs = new int[GraphSize][];
            isValid = new BitArray(GraphSize, false);
        }

        protected override bool Conclude()
        {
            List<Dependent>[] dependents = FindDependents();
            Eliminate(dependents);

            // Anything left?
            for (int i = 0; i < GraphSize; i++)
            {
                if (isValid[i])
                {
                    Result = rho;
                    return true;
                }
            }

            return false;
        }

        private List<Dependent>[] FindDependents()
        {
            List<Dependent>[] dependents = new List<Dependent>[GraphSize];

            for (int vId = 0; vId < GraphSize; vId++)
            {
                if (potPartners[vId] == null)
                {
                    isValid[vId] = false;
                    continue;
                }

                if (!isValid[vId])
                {
                    // Should never happen.
                    potPartners[vId] = null;
                    potPartCtrs[vId] = null;
                    continue;
                }

                int[][] CCs = potPartners[vId];

                // Iterate over components.
                for (int ccInd = 0; ccInd < CCs.Length; ccInd++)
                {
                    int[] partners = CCs[ccInd];

                    for (int i = 0; i < partners.Length; i++)
                    {
                        int uId = partners[i];

                        if (dependents[uId] == null)
                        {
                            dependents[uId] = new List<Dependent>();
                        }

                        // Add vertex-ID and index of CC.
                        Dependent dep;
                        dep.vId = vId;
                        dep.ccInd = ccInd;

                        dependents[uId].Add(dep);
                    }
                }
            }

            return dependents;
        }

        // Based on Holly's code.
        private void Eliminate(List<Dependent>[] dependents)
        {
            Queue<int> q = new Queue<int>();

            // Iterate over all potPart and add vect to q if there is an empty component.
            for (int i = 0; i < GraphSize; i++)
            {
                if (potPartners[i] == null)
                {
                    isValid[i] = false;
                    continue;
                }

                if (!isValid[i])
                {
                    // Should never happen.
                    potPartners[i] = null;
                    potPartCtrs[i] = null;
                    throw new Exception();
                }

                for (int j = 0; j < potPartners[i].Length; j++)
                {
                    int[] partners = potPartners[i][j];

                    for (int p = 0; p < partners[1] + 2; p++)
                    {
                        int pId = partners[p];
                        if (isValid[pId]) continue;

                        // Invalid partner; decrease counter.
                        potPartCtrs[i][j]--;
                    }

                    if (potPartCtrs[i][j] == 0)
                    {
                        // No valid partners for this component.
                        potPartners[i] = null;
                        potPartCtrs[i] = null;
                        isValid[i] = false;
                        q.Enqueue(i);
                        break;
                    }
                    else
                    {
                        // From now on, only counter needed.
                        potPartners[i][j] = null;
                    }
                }
            }

            while (q.Count > 0)
            {
                int vId = q.Dequeue();

                foreach (Dependent dep in dependents[vId])
                {
                    int uId = dep.vId;

                    if (!isValid[uId]) continue;

                    potPartCtrs[uId][dep.ccInd]--;

                    if (potPartCtrs[uId][dep.ccInd] == 0)
                    {
                        isValid[uId] = false;
                        potPartCtrs[uId] = null;
                        q.Enqueue(uId);
                    }
                }
            }

        }

        protected override byte[] GetNextTask(int vId)
        {
            isValid[vId] = true;
            string msg = string.Format(
                "{0}|partner|{1}|{2}",
                GraphName,
                vId,
                rho
            );

            return Encoding.UTF8.GetBytes(msg);
        }

        protected override void OnProcessMessages(int vId, byte[] msg)
        {
            int pt1Len = BitConverter.ToInt32(msg, 0);
            string pt1 = Encoding.UTF8.GetString(msg, 4, pt1Len);

            string[] msgParts = pt1.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            if (msgParts == null || msgParts.Length != 4)
            {
                throw new Exception();
            }

            string graph = msgParts[0];
            string task = msgParts[1];
            int cVId = int.Parse(msgParts[2]);
            int cRho = int.Parse(msgParts[2]);

            if (graph != GraphName ||
                task != "partner" ||
                cVId != vId ||
                cRho != rho)
            {
                throw new Exception();
            }

            // Read 
            int pt2Ptr = 4 + pt1Len;
            if (pt2Ptr >= msg.Length)
            {
                // Invalid vertex (i.e. some CCs without potential partners).
                isValid[vId] = false;
                return;
            }

            int ccLen = BitConverter.ToInt32(msg, pt2Ptr);
            pt2Ptr += 4;

            int[][] partners = new int[ccLen][];
            int[] counters = new int[ccLen];

            for (int cc = 0; cc < ccLen; cc++)
            {
                int pps = BitConverter.ToInt32(msg, pt2Ptr);
                pt2Ptr += 4;

                if (pps == 0)
                {
                    // Should be filtered out already.
                    isValid[vId] = false;
                    return;
                }

                List<int> ppLst = new List<int>(pps);

                for (int i = 0; i < pps; i++)
                {
                    int pId = BitConverter.ToInt32(msg, pt2Ptr);
                    pt2Ptr += 4;

                    if (!isValid[pId]) continue;

                    ppLst.Add(pId);
                }

                if (ppLst.Count == 0)
                {
                    isValid[vId] = false;
                    return;
                }

                counters[cc] = ppLst.Count;
                partners[cc] = ppLst.ToArray();
            }

            potPartners[vId] = partners;
            potPartCtrs[vId] = counters;
        }

        protected override void OnRestart()
        {
            rho++;
        }
    }
}
