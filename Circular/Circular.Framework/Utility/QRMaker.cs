
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ZXing;
using ZXing.QrCode.Internal;

namespace Circular.Framework.Utility
{
    public class QRMaker
    {
        public static object Files { get; private set; }

        public bool CreateQRCode(string keyString, string path)
        {
            var QCwriter = new ZXing.BarcodeWriter();
            QCwriter.Format = ZXing.BarcodeFormat.QR_CODE;
            QCwriter.Options.Height = 400;
            QCwriter.Options.Width = 400;
            QCwriter.Options.PureBarcode = true;

            var result = QCwriter.Write(keyString);
            var barcodeBitmap = new Bitmap(result);
            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
                {
                    barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
            return true;
        }



    }
}

