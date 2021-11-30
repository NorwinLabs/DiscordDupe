using System;
using System.Windows.Forms;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Threading;

namespace WindowsApplication2
{

    public partial class Form1 : Form
    {
        Thread ctThread = null;
        System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        NetworkStream serverStream = default(NetworkStream);
        string readData = null;
        string ipHost = null;
        BindingList<String> online = new BindingList<string>();
        

        public Form1()
        {
            InitializeComponent();
            ShowOnline();
        }

        private void sendButton_Click_1(object sender, EventArgs e)
        {
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(messageTextBox.Text + "$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
            messageTextBox.Clear();
        }

        private void getMessage()
        {
            while (true)
            {
                serverStream = clientSocket.GetStream();
                int buffSize = 0;
                byte[] inStream = new byte[10025];
                clientSocket.ReceiveBufferSize = 8192;
                buffSize = clientSocket.ReceiveBufferSize;

                serverStream.Read(inStream, 0, buffSize);
                string returndata = System.Text.Encoding.ASCII.GetString(inStream);
                readData = "" + returndata;

                if(returndata.IndexOf("Joined") > -1)
                {
                    string[] split = returndata.Split(' ');
                    string member = split[0];

                    online.Add(member);
                    this.Invoke(new Action(() => { online.ResetBindings(); }));

                }
                else if (returndata.IndexOf("disconnected") > -1)
                {
                    string[] split = returndata.Split(' ');
                    string member = split[0];

                    online.Remove(member);
                    this.Invoke(new Action(() => { online.ResetBindings(); }));

                }

                msg();
            }
        }

        private void msg()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(msg));
            }
            else
            {
                allTextBox.Text = allTextBox.Text + Environment.NewLine + " >> " + readData;
            }
        }


        private void allTextBox_TextChanged(object sender, EventArgs e)
        {
            // set the current caret position to the end
            allTextBox.SelectionStart = allTextBox.Text.Length;
            // scroll it automatically
            allTextBox.ScrollToCaret();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            readData = "Connected to Chat Server ...";


            msg();
            ipHost = "127.0.0.1";
            clientSocket.Connect(ipHost, 8888);
            serverStream = clientSocket.GetStream();

            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(nameTextBox.Text + "$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();

            ctThread = new Thread(getMessage);
            ctThread.Start();
            nameTextBox.ReadOnly = true;
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(nameTextBox.Text + " has disconnected" + "$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
            ctThread.Abort();
            serverStream.Close();
            clientSocket.Close();
            Application.Exit();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex == 0)
            {
                ipHost = "127.0.0.1";
            }
            if(comboBox1.SelectedIndex == 1)
            {
                ipHost = "DiscordDupe.ddns.net";
            }
        }

        private void messageTextBox_Enter(object sender, EventArgs e)
        {
            messageTextBox.TextAlign = HorizontalAlignment.Left;
            messageTextBox.ForeColor = System.Drawing.Color.Black;
            messageTextBox.Clear();
        }

        private void messageTextBox_Leave(object sender, EventArgs e)
        {
            messageTextBox.TextAlign = HorizontalAlignment.Center;
            messageTextBox.Text = "Message Server";
        }

        private void messageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                byte[] outStream = System.Text.Encoding.ASCII.GetBytes(messageTextBox.Text + "$");
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();
                messageTextBox.Clear();
            }
        }

        private void ShowOnline()
        {

            onlineListBox.DataSource = online;
        }
    }
}
