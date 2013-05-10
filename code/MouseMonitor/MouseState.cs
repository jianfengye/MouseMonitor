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
            string fileName = time.ToString("yyyyMMdd") + ".xml";

            MouseState fileDate;
            using (FileStream stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Read))
            {
                stream.Lock(0, stream.Length);
                XmlSerializer serializer = new XmlSerializer(typeof(MouseState));
                if (stream.Length != 0)
                {
                    fileDate = (MouseState)serializer.Deserialize(stream);
                }
                else
                {
                    fileDate = new MouseState();
                }
                this.leftClickCount = fileDate.leftClickCount;
                this.rightClickCount = fileDate.rightClickCount;
                this.middleClickCount = fileDate.middleClickCount;
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
            string fileName = time.ToString("yyyyMMdd") + ".xml";

            MouseState fileDate;
            using (FileStream stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                stream.Lock(0, stream.Length);
                XmlSerializer serializer = new XmlSerializer(typeof(MouseState));
                if (stream.Length != 0)
                {
                    fileDate = (MouseState)serializer.Deserialize(stream);
                }
                else
                {
                    fileDate = new MouseState();
                }

                fileDate.leftClickCount = leftClickCount;
                fileDate.rightClickCount = rightClickCount;
                fileDate.middleClickCount = middleClickCount;
                stream.Position = 0;

                XmlWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
                serializer.Serialize(writer, fileDate);
            }
        }
    }
}
