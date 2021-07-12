using System;
using System.Collections.Generic;
using System.Text;

namespace TbStb.Server
{
    class TbAlgorithm : Algorithm
    {
        int[] clRadii = null;


        public TbAlgorithm() : base() { }
        public TbAlgorithm(IEnumerable<ClientBase> clients) : base(clients) { }


        protected override void OnPreprocess(Graph g)
        {
            base.OnPreprocess(g);

            int[][] layPart = g.LayeringPartition;
            clRadii = new int[layPart.Length];

            for (int i = 0; i < clRadii.Length; i++)
            {
                clRadii[i] = int.MaxValue;
            }
        }

        protected override bool Conclude()
        {
            int maxRadius = -1;

            foreach (int rad in clRadii)
            {
                maxRadius = Math.Max(maxRadius, rad);
            }

            this.Result = maxRadius;

            return true;
        }

        protected override byte[] GetNextTask(int vId)
        {
            string msg = string.Format
            (
                "{0}|clDist|{1}",
                GraphName,
                vId
            );

            return Encoding.UTF8.GetBytes(msg);
        }

        protected override void OnProcessMessages(int vId, byte[] msg)
        {
            const int I32Size = 4;


            // --- Decode Message. ---

            int pt1Len = BitConverter.ToInt32(msg, 0);
            string pt1 = Encoding.UTF8.GetString(msg, I32Size, pt1Len);

            string[] msgParts = pt1.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            if (msgParts == null || msgParts.Length != 3)
            {
                throw new Exception();
            }

            string graph = msgParts[0];
            string task = msgParts[1];
            int cVId = int.Parse(msgParts[2]);

            if
            (
                graph != GraphName ||
                task != "clDist" ||
                cVId != vId ||
                msg.Length != I32Size + pt1Len + I32Size * clRadii.Length
            )
            {
                throw new Exception();
            }


            // --- Read Message. ---

            for
            (
                int i = 0, pt2Ptr = I32Size + pt1Len;
                i < clRadii.Length;
                i++, pt2Ptr += 4
            )
            {
                int maxDis = BitConverter.ToInt32(msg, pt2Ptr);
                clRadii[i] = Math.Min(maxDis, clRadii[i]);
            }
        }

        protected override void OnRestart()
        {
            throw new NotImplementedException();
        }
    }
}
