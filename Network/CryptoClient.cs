using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace TbStb
{
    public class CryptoClient : ClientBase
    {
        const int KeySize = 32;
        const int IVSize = 16;

        byte[] aesKey;

        ECDiffieHellmanCng keyExchange;
        byte[] publicKey;

        List<byte[]> msgBuffer = new List<byte[]>();

        public CryptoClient() : base() { }

        protected internal CryptoClient(Socket socket)
        {
            Initialise(socket, false);

            if (!StartCrypto(null))
            {
                throw new Exception();
            }

            StartListening();
        }

        protected bool StartCrypto(IPEndPoint ipe)
        {
            keyExchange = new ECDiffieHellmanCng();
            keyExchange.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
            keyExchange.HashAlgorithm = CngAlgorithm.Sha256;

            publicKey = keyExchange.PublicKey.ToByteArray();

            if (ipe != null && !base.Connect(ipe, false)) return false;
            if (!base.SendMessage(publicKey)) return false;

            return true;
        }

        public override bool Connect(IPEndPoint ipe)
        {
            if (!StartCrypto(ipe)) return false;

            StartListening();

            return true;
        }

        protected override void OnMessageReceived(MessageReceivedEventArgs e)
        {
            lock (msgBuffer)
            {
                if (aesKey == null)
                {
                    byte[] pubKey = e.RawMessage;
                    CngKey cngKey = CngKey.Import(pubKey, CngKeyBlobFormat.EccPublicBlob);
                    aesKey = keyExchange.DeriveKeyMaterial(cngKey);

                    // Send all buffered messages.
                    for (int i = 0; i < msgBuffer.Count; i++)
                    {
                        SendMessage(msgBuffer[i]);
                    }
                    msgBuffer.Clear();

                    return;
                }
            }

            // Split message into IV and actual message.
            byte[] fullMsg = e.RawMessage;
            byte[] iv = new byte[IVSize];
            byte[] encMsg = new byte[fullMsg.Length - IVSize];

            Array.Copy(fullMsg, iv, iv.Length);
            Array.Copy(fullMsg, iv.Length, encMsg, 0, encMsg.Length);

            // Decrypt message.
            RijndaelManaged aes = new RijndaelManaged();
            aes.BlockSize = IVSize * 8;

            aes.Key = aesKey;
            aes.IV = iv;

            ICryptoTransform dec = aes.CreateDecryptor();
            byte[] msg = dec.TransformFinalBlock(encMsg, 0, encMsg.Length);

            dec.Dispose();
            aes.Dispose();

            base.OnMessageReceived(new MessageReceivedEventArgs(msg));
        }

        public override bool SendMessage(byte[] msg)
        {
            if (msg == null)
            {
                throw new ArgumentNullException();
            }

            lock (msgBuffer)
            {
                if (aesKey == null)
                {
                    // Add the message into a buffer until key exchange is done.
                    msgBuffer.Add(msg);
                    return false;
                }
            }

            // Encrypt message.
            RijndaelManaged aes = new RijndaelManaged();
            aes.BlockSize = IVSize * 8;

            aes.Key = aesKey;
            aes.GenerateIV();
            byte[] iv = aes.IV;

            ICryptoTransform enc = aes.CreateEncryptor();
            byte[] encMsg = enc.TransformFinalBlock(msg, 0, msg.Length);

            enc.Dispose();
            aes.Dispose();

            // Merge IV and encrypted message.
            byte[] fullMsg = new byte[iv.Length + encMsg.Length];
            Array.Copy(iv, fullMsg, iv.Length);
            Array.Copy(encMsg, 0, fullMsg, iv.Length, encMsg.Length);

            return base.SendMessage(fullMsg);
        }

    }
}
