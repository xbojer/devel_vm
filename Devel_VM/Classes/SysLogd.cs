using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace Devel_VM.Classes
{
    public class SysLogd
    {
        //DETERMINES WHAT PORT TO RUN ON (DEFAULT IS 514)
        private int mPort = 514;
        public int Port
        {
            get
            {
                return mPort;
            }
            set
            {
                mPort = value;
            }
        }

        //USED TO THROW AN EVENT WHEN DATA IS RECEIVED
        public delegate void MessageReceived(MessageStruct Message);
        private MessageReceived mCallback = null;

        //USED TO RUN THE BACKGROUND THREAD READING FOR SYSLOG PACKETS
        private System.Threading.Thread mThread;
        private bool mRunning = false;

        public void StartListening(MessageReceived CallBack)
        {
            //STORE THE CALLBACK
            mCallback = CallBack;

            //CREATE A NEW THREAD USING OUR THREADSTART METHOD MAKE SURE ITS A BACKGROUND THREAD SO WHEN WE EXIT IT DOES TO
            mThread = new System.Threading.Thread(ThreadStart);
            mRunning = true;
            mThread.IsBackground = true;

            //START LISTENING FOR SYSLOG PACKETS
            mThread.Start();
        }
        public void StopListening()
        {
            //FLAG OUR RUNNING VARIABLE TO FALSE SO WE WILL STOP LISTENING
            mRunning = false;
        }
        public void SendMessage(MessageStruct Message, string RemoteHost)
        {
            UdpClient sendSocket = new UdpClient(RemoteHost, this.Port);
            byte[] output = System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("<{0}>{1} {2}:{3}", ((int)Message.Pri.Facility * 8) + (int)Message.Pri.Severity, Message.TimeStamp.ToString("MMM dd hh:mm:ss"), System.Environment.MachineName, Message.Message));

            //MAKE SURE THE FINAL BUFFER IS LESS THEN 1024 BYTES AND IF SO THEN SEND THE DATA
            if (output.Length < 1024)
            {
                sendSocket.Send(output, output.Length);
                sendSocket.Close();
            }
            else
            {
                throw new InsufficientMemoryException("The data in which you are trying to send does not comply to syslog standards.\nThe total message size must be less then 1024 bytes.");
            }
        }

        private void ThreadStart()
        {
            Socket ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint LocalEndPoint = new IPEndPoint(IPAddress.Any, 514);
            byte[] buffer = new byte[1024];

            //BIND TO THE SOCKET SO WE CAN START READING FOR DATA
            ListenSocket.Bind(LocalEndPoint);
            while (mRunning)
            {
                try
                {
                    //READ THE DATA AND IF THERE ISNT ANY DATA THEN WAIT UNTIL THERE IS
                    EndPoint remoteEP = LocalEndPoint;
                    int BytesRead = ListenSocket.ReceiveFrom(buffer, 0, 1024, SocketFlags.None, ref remoteEP);
                    string msg = System.Text.ASCIIEncoding.ASCII.GetString(buffer, 0, BytesRead);

                    //PARSE THE MESSAGE AND RAISE THE CALL BACK
                    MessageStruct tmpReturn = new MessageStruct(msg, remoteEP);
                    mCallback(tmpReturn);
                }
                catch (Exception e)
                {
                    Console.Write(e.ToString());
                }
            }

            //CLOSE THE SOCKET SINCE WE ARE DONE WITH IT
            ListenSocket.Close();
        }

        public enum FacilityEnum : int
        {
            Kernel = 0,
            User = 1,
            Mail = 2,
            System = 3,
            Security = 4,
            Internally = 5,
            Printer = 6,
            News = 7,
            UUCP = 8,
            cron = 9,
            Security2 = 10,
            Ftp = 11,
            Ntp = 12,
            Audit = 13,
            Alert = 14,
            Clock2 = 15,
            local0 = 16,
            local1 = 17,
            local2 = 18,
            local3 = 19,
            local4 = 20,
            local5 = 21,
            local6 = 22,
            local7 = 23,
        }
        public enum SeverityEnum : int
        {
            Emergency = 0,
            Alert = 1,
            Critical = 2,
            Error = 3,
            Warning = 4,
            Notice = 5,
            Info = 6,
            Debug = 7,
        }

        public struct PriStruct
        {
            public FacilityEnum Facility;
            public SeverityEnum Severity;
            public PriStruct(string strPri)
            {
                int intPri = Convert.ToInt32(strPri);
                int intFacility = intPri >> 3;
                int intSeverity = intPri & 0x7;
                this.Facility = (FacilityEnum)Enum.Parse(typeof(FacilityEnum),
                   intFacility.ToString());
                this.Severity = (SeverityEnum)Enum.Parse(typeof(SeverityEnum),
                   intSeverity.ToString());
            }
            public override string ToString()
            {
                //EXPORT VALUES TO A VALID PRI STRUCTURE
                return string.Format("{0}.{1}", this.Facility, this.Severity);
            }
        }
        public struct MessageStruct
        {
            public PriStruct Pri { get; set; }
            public DateTime TimeStamp { get; set; }
            public EndPoint Source { get; set; }
            public string Message { get; set; }

            public MessageStruct(PriStruct PRI, string Message)
                : this()
            {
                this.Pri = PRI;
                this.TimeStamp = DateTime.Now;
                this.Source = null;
                this.Message = Message;
            }

            public MessageStruct(string Message, EndPoint RemoteEP)
                : this()
            {
                Regex mRegex = new Regex("<(?<PRI>([0-9]{1,3}))>(?<Message>.*)", RegexOptions.Compiled);
                Match tmpMatch = mRegex.Match(Message);
                this.Pri = new PriStruct(tmpMatch.Groups["PRI"].Value);
                this.Message = tmpMatch.Groups["Message"].Value;
                this.TimeStamp = DateTime.Now;
                this.Source = RemoteEP;
            }

            public override string ToString()
            {
                return string.Format("{0} {1} : {2} : {3} : {4}", Pri.Facility, Pri.Severity, Source.ToString().Split(':')[0], TimeStamp.ToString(), Message);
            }
        }
    }
}
