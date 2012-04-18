using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

namespace Devel_VM
{

    public class Packet
    {
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
            Reset,
            Reserved,
            Null
        }
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
    public class Network_listener
    {
        IPEndPoint ep_server = new IPEndPoint(IPAddress.Any, 21544);
        UdpClient sock;

        #region Delegates
        public delegate void InfoEvent(String auth, String msg);
        public delegate void ErrorEvent(String auth, String msg);
        public delegate void KillEvent(String auth, String msg);
        public delegate void PingEvent(String auth, String msg);
        public delegate void PongEvent(String auth, String msg);
        public delegate void UpdateEvent(String auth, String ver);
        public delegate void ExecuteEvent(String auth, String cmd);
        public delegate void ChatEvent(String auth, String msg);
        public delegate void ResetEvent(String auth);
        public delegate void AnyEvent(String type, String auth, String msg);

        public InfoEvent OnInfo = null;
        public ErrorEvent OnError = null;
        public KillEvent OnKill = null;
        public PingEvent OnPing = null;
        public PongEvent OnPong = null;
        public UpdateEvent OnUpdate = null;
        public ExecuteEvent OnExecute = null;
        public ChatEvent OnChat = null;
        public ResetEvent OnReset = null;
        public AnyEvent OnAny = null;
        #endregion
        #region Events
        private void onInfoEvent(String auth, String msg)
        {
            if (OnInfo != null)
            {
                OnInfo(auth, msg);
            }
        }
        private void onErrorEvent(String auth, String msg)
        {
            if (OnError != null)
            {
                OnError(auth, msg);
            }
        }
        private void onKillEvent(String auth, String msg)
        {
            if (OnKill != null)
            {
                OnKill(auth, msg);
            }
        }
        private void onPingEvent(String auth, String msg)
        {
            if (OnPing != null)
            {
                OnPing(auth, msg);
            }
        }
        private void onPongEvent(String auth, String msg)
        {
            if (OnPong != null)
            {
                OnPong(auth, msg);
            }
        }
        private void onUpdateEvent(String auth, String ver)
        {
            if (OnUpdate != null)
            {
                OnUpdate(auth, ver);
            }
        }
        private void onExecuteEvent(String auth, String cmd)
        {
            if (OnUpdate != null)
            {
                OnUpdate(auth, cmd);
            }
        }
        private void onChatEvent(String auth, String msg)
        {
            if (OnChat != null)
            {
                OnChat(auth, msg);
            }
        }
        private void onResetEvent(String auth)
        {
            if (OnReset != null)
            {
                OnReset(auth);
            }
        }
        private void onAnyEvent(String type, String auth, String msg)
        {
            if (OnAny != null)
            {
                OnAny(type, auth, msg);
            }
        }
        #endregion
        public Network_listener()
        {
            sock = new UdpClient(ep_server);
            sock.EnableBroadcast = true;

            UdpState s = new UdpState();
            s.e = ep_server;
            s.u = sock;

            sock.BeginReceive(new AsyncCallback(this.ReceiveCallback), s);
        }

        private class UdpState
        {
            public IPEndPoint e;
            public UdpClient u;
        }
        public void ReceiveCallback(IAsyncResult ar)
        {
            UdpClient u = (UdpClient)((UdpState)(ar.AsyncState)).u;
            IPEndPoint e = (IPEndPoint)((UdpState)(ar.AsyncState)).e;

            Packet packet = new Packet(u.EndReceive(ar, ref e));

            onAnyEvent(packet.dataIdentifier.ToString(), packet.auth, packet.message);

            switch (packet.dataIdentifier)
            {
                case Packet.DataIdentifier.Info:
                    onInfoEvent(packet.auth, packet.message);
                    break;
                case Packet.DataIdentifier.Error:
                    onErrorEvent(packet.auth, packet.message);
                    break;
                case Packet.DataIdentifier.Kill:
                    onKillEvent(packet.auth, packet.message);
                    break;
                case Packet.DataIdentifier.Ping:
                    onPingEvent(packet.auth, packet.message);
                    break;
                case Packet.DataIdentifier.Pong:
                    onPongEvent(packet.auth, packet.message);
                    break;
                case Packet.DataIdentifier.Update:
                    onUpdateEvent(packet.auth, packet.message);
                    break;
                case Packet.DataIdentifier.Execute:
                    onExecuteEvent(packet.auth, packet.message);
                    break;
                case Packet.DataIdentifier.Chat:
                    onChatEvent(packet.auth, packet.message);
                    break;
                case Packet.DataIdentifier.Reset:
                        onResetEvent(packet.auth + ":" + packet.message);
                    break;
                case Packet.DataIdentifier.Reserved:
                    break;
                case Packet.DataIdentifier.Null:
                    break;
                default:
                    break;
            }

            u.BeginReceive(new AsyncCallback(this.ReceiveCallback), ((UdpState)(ar.AsyncState)));
        }
    }

    public class Network_Broadcast
    {
        public static int port = 21544;

        public static void send(Packet pack)
        {
            send(pack.GetDataStream());
        }
        public static void send(byte[] packet)
        {
            UdpClient s = new UdpClient();
            s.EnableBroadcast = true;
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("255.255.255.255"), port);
            s.Send(packet, packet.Length, ep);
        }
    }
}
