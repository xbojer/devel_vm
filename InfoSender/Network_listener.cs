using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

namespace Devel_VM
{
    class Network_listener
    {
        IPEndPoint ep_server = new IPEndPoint(IPAddress.Any, 21543);
        UdpClient sock;
        
        public Network_listener()
        {
            sock = new UdpClient(ep_server);
            sock.EnableBroadcast = true;

            UdpState s = new UdpState();
            s.e = ep_server;
            s.u = sock;

            sock.BeginReceive(new AsyncCallback(ReceiveCallback), s);
        }
        public enum DataIdentifier
        {
            Info,
            Error,
            Kill,
            Ping,
            Pong,
            Update,
            Execute,
            Chat,
            Reserved,
            Null
        }
        public class Packet
        {
            public DataIdentifier dataIdentifier;
            public string auth;
            public string message;

            #region Methods

            // Default Constructor
            public Packet()
            {
                this.dataIdentifier = DataIdentifier.Null;
                this.message = null;
                this.auth = null;
            }

            public Packet(byte[] dataStream)
            {
                // Read the data identifier from the beginning of the stream (4 bytes)
                this.dataIdentifier = (DataIdentifier)BitConverter.ToInt32(dataStream, 0);

                // Read the length of the auth (4 bytes)
                int nameLength = BitConverter.ToInt32(dataStream, 4);

                // Read the length of the message (4 bytes)
                int msgLength = BitConverter.ToInt32(dataStream, 8);

                // Read the auth field
                if (nameLength > 0)
                    this.auth = Encoding.UTF8.GetString(dataStream, 12, nameLength);
                else
                    this.auth = null;

                // Read the message field
                if (msgLength > 0)
                    this.message = Encoding.UTF8.GetString(dataStream, 12 + nameLength, msgLength);
                else
                    this.message = null;
            }

            // Converts the packet into a byte array for sending/receiving
            public byte[] GetDataStream()
            {
                List<byte> dataStream = new List<byte>();

                // Add the dataIdentifier
                dataStream.AddRange(BitConverter.GetBytes((int)this.dataIdentifier));

                // Add the auth length
                if (this.auth != null)
                    dataStream.AddRange(BitConverter.GetBytes(this.auth.Length));
                else
                    dataStream.AddRange(BitConverter.GetBytes(0));

                // Add the message length
                if (this.message != null)
                    dataStream.AddRange(BitConverter.GetBytes(this.message.Length));
                else
                    dataStream.AddRange(BitConverter.GetBytes(0));

                // Add the auth
                if (this.auth != null)
                    dataStream.AddRange(Encoding.UTF8.GetBytes(this.auth));

                // Add the message
                if (this.message != null)
                    dataStream.AddRange(Encoding.UTF8.GetBytes(this.message));

                return dataStream.ToArray();
            }
            #endregion
        }
        private class UdpState
        {
            public IPEndPoint e;
            public UdpClient u;
        }

        public static void ReceiveCallback(IAsyncResult ar)
        {
            UdpClient u = (UdpClient)((UdpState)(ar.AsyncState)).u;
            IPEndPoint e = (IPEndPoint)((UdpState)(ar.AsyncState)).e;

            Packet receivedData = new Packet(u.EndReceive(ar, ref e));
           

        }
    }
}
