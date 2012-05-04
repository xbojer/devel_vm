using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO.Pipes;
using System.IO;

namespace Devel_VM.Classes
{
    class SerialPipe //TODO: MEMLEAK?
    {
        static int num = 0;

        Thread thread;
        NamedPipeServerStream pipe;
        int curr_num = 0;
        string curr_pipe = "";

        string log = "";
        string l = "";

        Dictionary<string, string> challenges = new Dictionary<string, string>(1);

        public SerialPipe()
        {
            thread = new Thread(new ThreadStart(work));
        }
        public string Start()
        {
            curr_num = num++;
            curr_pipe = Properties.Settings.Default.serial_pipe.Replace("{0}", curr_num.ToString());
            thread.Start();
            return curr_pipe;
        }

        public void Stop()
        {
            thread.Abort();
        }

        public void addChallange(string req, string resp)
        {
            challenges[req] = resp;
        }

        void work()
        {
            pipe = new NamedPipeServerStream(curr_pipe, PipeDirection.InOut);

            while (true)
            {
                try
                {
                    pipe.WaitForConnection();
                    using (StreamWriter w = new StreamWriter(pipe))
                    {
                        w.AutoFlush = true;
                        using (StreamReader r = new StreamReader(pipe))
                        {
                            while(true) {
                                l = "";

                                while (!r.EndOfStream)
                                {
                                    char c = (char) r.Read();
                                    l += c;
                                    if (c == '\n')
                                    {
                                        log += l;
                                        Program.DBG.debugSet(log);
                                        l = "";
                                    }
                                    if (challenges.ContainsKey(l))
                                    {
                                        w.WriteLine(challenges[l]);
                                        w.Flush();
                                    }
                                }
                                
                                
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    //throw;
                }
            }
        }
    }
}
