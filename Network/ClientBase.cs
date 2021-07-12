using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TbStb
{
    public class ClientBase
    {
        private Socket socket;

        public event MessageReceivedEventHandler MessageReceived;
        public event EventHandler ConnectionEnded;

        public ClientBase()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        protected internal ClientBase(Socket socket)
        {
            Initialise(socket, true);
        }

        protected void Initialise(Socket socket, bool startListening)
        {
            if (socket == null)
            {
                throw new ArgumentNullException();
            }

            // ToDo: Verify socket.

            this.socket = socket;

            if (startListening)
            {
                StartListening();
            }
        }

        public bool IsConnected
        {
            get
            {
                return socket != null
                    && socket.Connected
                    && StillConnected;
            }
        }

        public Socket Socket
        {
            get
            {
                return socket;
            }
        }

        protected bool StillConnected
        {
            get
            {
                // Checks if socket is still connected.
                // Do not change order! (Causes false detection of disconnect.)
                // See https://stackoverflow.com/a/722265.
                return !socket.Poll(1, SelectMode.SelectRead) || socket.Available != 0;
            }
        }

        public virtual bool Connect(IPEndPoint ipe)
        {
            return Connect(ipe, true);
        }

        protected bool Connect(IPEndPoint ipe, bool startListening)
        {
            if (ipe == null)
            {
                throw new ArgumentNullException("ipe");
            }

            if (IsConnected)
            {
                throw new InvalidOperationException();
            }

            try
            {
                socket.Connect(ipe);
                if (startListening) StartListening();
            }
            catch
            {
                return false;
            }

            return true;
        }

        protected void StartListening()
        {
            Thread listenThread = new Thread(Listen);
            listenThread.Start();
        }

        private void Listen()
        {
            while (true)
            {
                try
                {
                    // States if socket is still connected.
                    // Avoids ending up in an endless loop in case the connection is lost.
                    bool isConn = true;

                    // Read a message-length.
                    byte[] header = new byte[4];
                    for (int bytesRead = 0; isConn && bytesRead < 4; isConn = StillConnected)
                    {
                        bytesRead += socket.Receive(header, bytesRead, 4 - bytesRead, SocketFlags.None);
                    }

                    if (!isConn) break;

                    int msgLength = BitConverter.ToInt32(header, 0);

                    // Read message.
                    byte[] message = new byte[msgLength];
                    for (int bytesRead = 0; isConn && bytesRead < msgLength; isConn = StillConnected)
                    {
                        bytesRead += socket.Receive(message, bytesRead, msgLength - bytesRead, SocketFlags.None);
                    }

                    if (!isConn) break;

                    OnMessageReceived(new MessageReceivedEventArgs(message));
                }
                catch (SocketException)
                {
                    // ToDo: Handle exception.
                    break;
                }
            }

            Close();
        }

        protected virtual void OnMessageReceived(MessageReceivedEventArgs e)
        {
            MessageReceived?.Invoke(this, e);
        }

        public virtual bool SendMessage(byte[] message)
        {
            if (message == null || message.Length == 0)
            {
                throw new ArgumentNullException();
            }

            try
            {
                int msgLength = message.Length;

                // Send message length.
                byte[] header = BitConverter.GetBytes(msgLength);
                for (int bytesSend = 0; bytesSend < 4;)
                {
                    bytesSend += socket.Send(header, bytesSend, 4 - bytesSend, SocketFlags.None);
                }

                // Send message.
                for (int bytesSend = 0; bytesSend < msgLength;)
                {
                    bytesSend += socket.Send(message, bytesSend, msgLength - bytesSend, SocketFlags.None);
                }

                return true;
            }
            catch (SocketException)
            {
                // ToDo: Handle exception.
                return false;
            }
        }

        public void Close()
        {
            lock (this)
            {
                if (socket == null) return;

                socket.Close();
                ConnectionEnded?.Invoke(this, new EventArgs());
                socket = null;
            }
        }
    }

    public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);

    public class MessageReceivedEventArgs : EventArgs
    {
        public byte[] RawMessage { get; protected set; }

        public MessageReceivedEventArgs(byte[] message)
        {
            RawMessage = message;
        }
    }
}
