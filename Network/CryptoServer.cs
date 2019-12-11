using System.Net.Sockets;

namespace TbStb
{
    public class CryptoServer : ServerBase
    {
        protected override ClientBase CreateClient(Socket socket)
        {
            return new CryptoClient(socket);
        }
    }
}
