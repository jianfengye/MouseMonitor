using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace MouseMonitor
{
    class MouseState
    {
        public int leftClickCount;

        public int rightClickCount;
        
        public int middleClickCount;

        public void recordAction(int message)
        {
            // TODO：由于是多线程，需要加锁操作
            lock (this)
            {
                switch (message)
                {
                    case Message.WM_LBUTTONDOWN:
                        this.leftClickCount++;
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

        // 将action保存到文件中
        public void saveAction(DateTime time)
        {
            int addLeftClickCount;
            int addRightClickCount;
            int addMiddleClickCount;

            lock (this)
            {
                addLeftClickCount = this.leftClickCount;
                addRightClickCount = this.rightClickCount;
                addMiddleClickCount = this.middleClickCount;

                this.leftClickCount = 0;
                this.rightClickCount = 0;
                this.middleClickCount = 0;
            }

            // 储存到文件，比如20130203.xml
            string fileName = time.ToString("yyyyMMdd") + ".xml";
            if (!File.Exists(fileName))
            {
                File.Create(fileName);
            }

            XmlSerializer serializer = new XmlSerializer(typeof(MouseState));
            MemoryStream memStream = new MemoryStream(File.ReadAllBytes(fileName));
            MouseState fileDate = (MouseState)serializer.Deserialize(memStream);

            fileDate.leftClickCount += addLeftClickCount;
            fileDate.rightClickCount += addRightClickCount;
            fileDate.middleClickCount += addMiddleClickCount;

            XmlWriter writer = new XmlTextWriter(fileName, Encoding.UTF8);
            serializer.Serialize(writer, fileDate);
        }
    }
}
