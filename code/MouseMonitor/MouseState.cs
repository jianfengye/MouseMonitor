using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace MouseMonitor
{
    public class MouseState
    {
        public int leftClickCount;

        public int rightClickCount;
        
        public int middleClickCount;

        private const string logFile = "log.xml";

        public void recordAction(int message)
        {
            // 由于是多线程，需要加锁操作
            lock (this)
            {
                switch (message)
                {
                    case Message.WM_LBUTTONDOWN:
                        this.leftClickCount++;;
                        break;
                    case Message.WM_RBUTTONDOWN:
                        this.rightClickCount++;
                        break;
                    case Message.WM_MBUTTONDOWN:
                        this.middleClickCount++;
                        break;
                }
            }
        }

        public void loadClick(DateTime time)
        {
            // 储存到文件，比如20130203.xml
            string fileName = MouseState.logFile;
            string today = time.ToString("yyyyMMdd");
            SerializableDictionary fileDatas = new SerializableDictionary();

            using (FileStream stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Read))
            {
                stream.Lock(0, stream.Length);
                XmlSerializer serializer = new XmlSerializer(typeof(SerializableDictionary));
                if (stream.Length != 0)
                {
                    fileDatas = (SerializableDictionary)serializer.Deserialize(stream);
                    if (!fileDatas.ContainsKey(today))
                    {
                        fileDatas[today] = new MouseState();
                    }
                }
                else
                {
                    fileDatas = new SerializableDictionary();
                    fileDatas[today] = new MouseState();
                }
                this.leftClickCount = fileDatas[today].leftClickCount;
                this.rightClickCount = fileDatas[today].rightClickCount;
                this.middleClickCount = fileDatas[today].middleClickCount;
            }
        }

        // 将action保存到文件中
        public void saveAction(DateTime time)
        {
            int leftClickCount;
            int rightClickCount;
            int middleClickCount;

            lock (this)
            {
                leftClickCount = this.leftClickCount;
                rightClickCount = this.rightClickCount;
                middleClickCount = this.middleClickCount;

            }

            // 储存到文件，比如20130203.xml
            string fileName = MouseState.logFile;
            string today = time.ToString("yyyyMMdd");

            SerializableDictionary fileDatas = new SerializableDictionary();
            MouseState fileData = new MouseState();
            using (FileStream stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                stream.Lock(0, stream.Length);
                XmlSerializer serializer = new XmlSerializer(typeof(SerializableDictionary));
                if (stream.Length != 0)
                {
                    fileDatas = (SerializableDictionary)serializer.Deserialize(stream);
                    if (!fileDatas.ContainsKey(today))
                    {
                        fileDatas[today] = new MouseState();
                    }
                }
                else
                {
                    fileDatas = new SerializableDictionary();
                    fileDatas[today] = new MouseState();
                }


                fileDatas[today].leftClickCount = leftClickCount;
                fileDatas[today].rightClickCount = rightClickCount;
                fileDatas[today].middleClickCount = middleClickCount;
                stream.Position = 0;

                XmlWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
                serializer.Serialize(writer, fileDatas);
            }
        }
    }
}
