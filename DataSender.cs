using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;


namespace KinectModule
{
    class KinectDataSender
    {
        private int portNum;
        private string ipAddress;

        private double sendDelay;
        public double Delay
        {
            set
            {
                if (value < 0)
                    sendDelay = 0;
                sendDelay = value;
            }
        }

        private DateTime preSendTime;

        private Mutex NetMutex;
        private Socket clientSocket;
        private IPEndPoint serverInfo;
        private string hostName;

        private uint lastMessageID;

        public KinectDataSender(string ipAddress, int portNum)
        {
            this.ipAddress = ipAddress;
            this.portNum = portNum;

            //manager = null;
            sendDelay = 0;
            preSendTime = DateTime.Now;
            //설정
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPAddress _ipaddr = IPAddress.Parse(ipAddress);
            serverInfo = new IPEndPoint(_ipaddr, portNum);
            hostName = System.Net.Dns.GetHostName();

            lastMessageID = 0;

            NetMutex = new Mutex(false, "KinectClientNetMutex");
        }

        public void Reset(int portNum, string ipAddr)
        {
            Mutex _mutex = new Mutex(false, "KinectClientNetMutex");
            _mutex.WaitOne();
            this.portNum = portNum;
            this.ipAddress = ipAddr;
            //새로지정
            IPAddress _ipAddr = IPAddress.Parse(ipAddress);
            serverInfo = new IPEndPoint(_ipAddr, portNum);

            _mutex.ReleaseMutex();
        }

        public void SendMessage(string device, Dictionary<int, KinectPoseInputMessage> data)
        {
            NetMutex.WaitOne();
            //장치명
            Byte[] _device = Encoding.Unicode.GetBytes(device);
            Byte[] _deviceLen = System.BitConverter.GetBytes(_device.Length);
            //호스트명
            Byte[] _host = Encoding.Unicode.GetBytes(hostName);
            Byte[] _hostLen = System.BitConverter.GetBytes(_host.Length);
            //메시지 번호
            Byte[] _messageID = System.BitConverter.GetBytes(lastMessageID++);
            //데이터 수
            Byte[] _count = System.BitConverter.GetBytes(data.Count);
            //차례대로 데이터
            ArrayList _mem = new ArrayList();
            int _dataLen = 0;
            foreach (KeyValuePair<int, KinectPoseInputMessage> _pair in data)
            {
                Byte[] _id = System.BitConverter.GetBytes(_pair.Key);
                Byte[] _data = _pair.Value;

                int _imLen = _id.Length + _data.Length;
                Byte[] _sumData = new Byte[_imLen];

                Array.Copy(_id, 0, _sumData, 0, _id.Length);
                Array.Copy(_data, 0, _sumData, _id.Length, _data.Length);

                _mem.Add(_sumData);
                _dataLen += _imLen;

                // DEBUG
                System.Diagnostics.Trace.WriteLine("DeviceName:" + _pair.Value.DeviceName);
                System.Diagnostics.Trace.WriteLine("HostName:" + _pair.Value.HostName);
                for (int i = 0; i < 25; i++)
                    System.Diagnostics.Trace.WriteLine("UserBody:" + _pair.Value.UserBody[i].ToString());
                System.Diagnostics.Trace.WriteLine("UserFeature:\n" + _pair.Value.UserFeature.ToString());
            }

            //결과 배열
            int _total = _device.Length + _host.Length + _messageID.Length + _count.Length * 4 + _dataLen;
            Byte[] _totalLen = System.BitConverter.GetBytes(_total);
            Byte[] _sendBuf = new Byte[_total];
            //배열에 데이터 삽입.
            int _nextPos = 0;
            //총 길이
            Array.Copy(_totalLen, 0, _sendBuf, 0, _totalLen.Length);
            _nextPos = _totalLen.Length;
            //장치명
            Array.Copy(_deviceLen, 0, _sendBuf, _nextPos, _deviceLen.Length);
            _nextPos += _deviceLen.Length;
            Array.Copy(_device, 0, _sendBuf, _nextPos, _device.Length);
            _nextPos += _device.Length;
            //호스트명
            Array.Copy(_hostLen, 0, _sendBuf, _nextPos, _hostLen.Length);
            _nextPos += _hostLen.Length;
            Array.Copy(_host, 0, _sendBuf, _nextPos, _host.Length);
            _nextPos += _host.Length;
            //메시지번호
            Array.Copy(_messageID, 0, _sendBuf, _nextPos, _messageID.Length);
            _nextPos += _messageID.Length;
            //데이터 수
            Array.Copy(_count, 0, _sendBuf, _nextPos, _count.Length);
            _nextPos += _count.Length;
            //데이터들을 차례로 삽입
            foreach (Byte[] _byteStr in _mem)
            {
                Array.Copy(_byteStr, 0, _sendBuf, _nextPos, _byteStr.Length);
                _nextPos += _byteStr.Length;
            }

            //전송
            clientSocket.SendTo(_sendBuf, serverInfo);

            NetMutex.ReleaseMutex();
        }
        /*public void SendMessage(string device, Dictionary<int, InputMessage> data)
        {
            NetMutex.WaitOne();
            //장치
            Byte[] _device = Encoding.Unicode.GetBytes(device);
            Byte[] _deviceLen = System.BitConverter.GetBytes(_device.Length);
            //호스트
            Byte[] _host = Encoding.Unicode.GetBytes(hostName);
            Byte[] _hostLen = System.BitConverter.GetBytes(_host.Length);
            //메시지번호
            Byte[] _messageID = System.BitConverter.GetBytes(lastMessageID++);
            //수량
            Byte[] _count = System.BitConverter.GetBytes(data.Count);
            //차례대로 메모리
            ArrayList _mem = new ArrayList();
            int _dataLen = 0;
            foreach (KeyValuePair<int, InputMessage> _pair in data)
            {
                Byte[] _id = System.BitConverter.GetBytes(_pair.Key);
                Byte[] _data = _pair.Value;

                int _imLen = _id.Length + _data.Length;
                Byte[] _sumData = new Byte[_imLen];

                Array.Copy(_id, 0, _sumData, 0, _id.Length);
                Array.Copy(_data, 0, _sumData, _id.Length, _data.Length);

                _mem.Add(_sumData);
                _dataLen += _imLen;
            }

            //결과 배열
            int _total = _device.Length + _host.Length + _messageID.Length + _count.Length * 4 + _dataLen;
            Byte[] _totalLen = System.BitConverter.GetBytes(_total);
            Byte[] _sendBuf = new Byte[_total];
            //배열에 데이터 삽입.
            int _nextPos = 0;
            //총 길이
            Array.Copy(_totalLen, 0, _sendBuf, 0, _totalLen.Length);
            _nextPos = _totalLen.Length;
            //디바이스
            Array.Copy(_deviceLen, 0, _sendBuf, _nextPos, _deviceLen.Length);
            _nextPos += _deviceLen.Length;
            Array.Copy(_device, 0, _sendBuf, _nextPos, _device.Length);
            _nextPos += _device.Length;
            //호스트
            Array.Copy(_hostLen, 0, _sendBuf, _nextPos, _hostLen.Length);
            _nextPos += _hostLen.Length;
            Array.Copy(_host, 0, _sendBuf, _nextPos, _host.Length);
            _nextPos += _host.Length;
            //메시지번호
            Array.Copy(_messageID, 0, _sendBuf, _nextPos, _messageID.Length);
            _nextPos += _messageID.Length;
            //데이터 수
            Array.Copy(_count, 0, _sendBuf, _nextPos, _count.Length);
            _nextPos += _count.Length;
            //데이터들을 차례로 삽입
            foreach (Byte[] _byteStr in _mem)
            {
                Array.Copy(_byteStr, 0, _sendBuf, _nextPos, _byteStr.Length);
                _nextPos += _byteStr.Length;
            }

            //전송
            clientSocket.SendTo(_sendBuf, serverInfo);

            NetMutex.ReleaseMutex();
        }*/
    }
}
