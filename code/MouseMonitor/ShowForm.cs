using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections.Specialized;

namespace MouseMonitor
{
    public partial class ShowForm : Form
    {
        public ShowForm()
        {
            InitializeComponent();

            // 发送POST请求到mouseMonitor.funaio.com
            BrowseShow();
        }

        private void BrowseShow()
        {
            List<UploadFile> files = new List<UploadFile>();
            UploadFile file = new UploadFile();
            file.Name = "log";
            file.Filename = "log.xml";
            file.ContentType = "text/xml";

            files.Add(file);
            string header;
            byte[] data = this.PrepareUploadFiles(files, new NameValueCollection(), out header);
            this.webBrowser1.Navigate("http://mouseMonitor.funaio.com/show/static", "", data, header);
        }

        public byte[] PrepareUploadFiles(IEnumerable<UploadFile> files, NameValueCollection values, out string header)
        {

            using (var requestStream = new MemoryStream())
            {
                var boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x"); //, NumberFormatInfo.InvariantInfo);
                header = "multipart/form-data; boundary=" + boundary;
                var boundaryBuffer2 = Encoding.ASCII.GetBytes(header);
                requestStream.Write(boundaryBuffer2, 0, boundaryBuffer2.Length);
                boundary = "--" + boundary;
                // Write the values
                foreach (string name in values.Keys)
                {
                    var buffer = Encoding.ASCII.GetBytes(boundary + Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.ASCII.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"{1}{1}", name, Environment.NewLine));
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.UTF8.GetBytes(values[name] + Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                }

                // Write the files
                foreach (var file in files)
                {
                    var buffer = Encoding.ASCII.GetBytes(boundary + Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.UTF8.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"{2}", file.Name, file.Filename, Environment.NewLine));
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.ASCII.GetBytes(string.Format("Content-Type: {0}{1}{1}", file.ContentType, Environment.NewLine));
                    requestStream.Write(buffer, 0, buffer.Length);

                    file.CopyTo(requestStream);

                    buffer = Encoding.ASCII.GetBytes(Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                }

                var boundaryBuffer = Encoding.ASCII.GetBytes(boundary + "--");
                requestStream.Write(boundaryBuffer, 0, boundaryBuffer.Length);

                return requestStream.ToArray();

            }
        }

        

    }
}
