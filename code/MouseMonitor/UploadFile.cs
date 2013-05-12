using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MouseMonitor
{
    public class UploadFile
    {
        public string Name;
        public string Filename;
        public string ContentType;
        public string Stream;

        public Stream getStream()
        {
            using (FileStream stream = File.OpenRead(this.Filename))
            {
                stream.Lock(0, stream.Length);
                return stream;
            }
        }

        public void CopyTo(Stream dstStream)
        {
            using (FileStream stream = File.OpenRead(this.Filename))
            {
                stream.Lock(0, stream.Length);
                UploadFile.CopyStream(stream, dstStream);
            }
        }

        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[32768];
            while (true)
            {
                int read = input.Read(buffer, 0, buffer.Length);
                if (read <= 0)
                    return;
                output.Write(buffer, 0, read);
            }
        }
    }
}
