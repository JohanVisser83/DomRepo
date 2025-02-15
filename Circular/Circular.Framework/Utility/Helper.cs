using System.Text;
using System.Drawing;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Drawing.Imaging;
using System.IO;

namespace Circular.Framework.Utility
{
    public class Helper : IHelper
    {
        #region "Misc"
        public string GenerateRandomNumber(int numberOfCharacters ,string masterOtp,bool masterOtpEnabled, bool isAlphaNumeric)
        {
            if (masterOtpEnabled) 
                return masterOtp;
            if (numberOfCharacters > 10)
                numberOfCharacters = 10;
            if(numberOfCharacters < 3)
                numberOfCharacters = 3;

            var baseString = "0123456789";
            if(isAlphaNumeric)
                baseString = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            char[] charArr = baseString.ToCharArray();
            string otp = "";
            Random obj = new Random();
            for (int i = 0; i < numberOfCharacters; i++)
            {
                int pos = obj.Next(1, charArr.Length);
                if (!otp.Contains(charArr.GetValue(pos).ToString())) otp += charArr.GetValue(pos);
                else i--;
            }

            return otp;
        }
        public string GenerateRandomNumberone(int numberOfCharacters, string masterOtp, bool masterOtpEnabled, bool isAlphaNumeric)
        {
            if (masterOtpEnabled)
                return masterOtp;
            if (numberOfCharacters > 10)
                numberOfCharacters = 10;
            if (numberOfCharacters < 3)
                numberOfCharacters = 3;

            var baseString = "0123456789";
            if (isAlphaNumeric)
                baseString = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            char[] charArr = baseString.ToCharArray();
            string otp = "";
            Random obj = new Random();
            for (int i = 0; i < numberOfCharacters;)
            {
                int pos = obj.Next(0, charArr.Length);
                if (!otp.Contains(charArr[pos].ToString()))
                {
                    otp += charArr[pos];

                    i++;
                }
                  
            }

            return otp;
        }

        
        public string GetQRCode(string keyString, string fileNameWithExtension, ref string path)
        {
            try
            {
				if (!Directory.Exists(path))
					Directory.CreateDirectory(path);
				path = path + fileNameWithExtension;

				if (!File.Exists(path))
				{
					QRMaker qRMaker = new QRMaker();
					qRMaker.CreateQRCode(keyString, path);
				}
				return ConvertImageToBase64(path);
			}
            catch (Exception ex)
            {
                return "";
            }

        }
        #endregion

        #region "Files"
        public string SaveFile(IFormFile file, string uploadFolder, HttpRequest request)
        {
            var path = "";
            var returnfilepath = "";

            string url = $"{request.Scheme}://{request.Host}{request.PathBase}" + uploadFolder;
            string filesPath = Directory.GetCurrentDirectory() + uploadFolder;
            if (!Directory.Exists(filesPath))
                Directory.CreateDirectory(filesPath);

            var uniqueFileName = GetUniqueFileName(file.FileName);
            //string fileName = Path.GetFileName(uniqueFileName);

            path = Path.Combine(filesPath, uniqueFileName);
            file.CopyToAsync(new FileStream(path, FileMode.Create));
            returnfilepath = Path.Combine(url, uniqueFileName);
            return returnfilepath;
        }
        public string ConvertBase64toDocument(string FileName, string base64Content, string RootfolderPath)
        {
            try
            {
                if (!Directory.Exists(RootfolderPath))
                    Directory.CreateDirectory(RootfolderPath);

                string fName = GetUniqueFileName(FileName);
                string FullPath = RootfolderPath + fName;
                // Convert base 64 string to byte[]
                byte[] PDFBytes = Convert.FromBase64String(base64Content);
                // Convert byte[] to PDF
                File.WriteAllBytes(FullPath, PDFBytes);
                return fName;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public string ConvertBase64toImage(string ImageName, string Imagebase64Content, string RootfolderPath, HttpRequest request)
        {
            try
            {
                var path = "";
                var returnfilepath = "";
                string fName = GetUniqueFileName(ImageName);


                string url = $"{request.Scheme}://{request.Host}{request.PathBase}" + RootfolderPath;
                string filesPath = Directory.GetCurrentDirectory() + RootfolderPath;
                if (!Directory.Exists(filesPath))
                    Directory.CreateDirectory(filesPath);

                if (Imagebase64Content.Contains(","))
                {
                    Imagebase64Content = Imagebase64Content.Split(',')[1].ToString();
                }
                // Convert base 64 string to byte[]
                byte[] imageBytes = Convert.FromBase64String(Imagebase64Content);
                // Convert byte[] to Image
                using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                {
                    Image image = Image.FromStream(ms, true);
                    filesPath = filesPath + "/" + fName;
                    image.Save((filesPath));

                    returnfilepath = Path.Combine(url, fName);
                    return returnfilepath;

                }
            
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public string ConvertBase64toMp4(string ImageName, string Imagebase64Content, string RootfolderPath, HttpRequest request)
        {
            try
            {
                var path = "";
                var returnfilepath = "";
                string fName = GetUniqueFileName(ImageName);
                string url = $"{request.Scheme}://{request.Host}{request.PathBase}" + RootfolderPath;
                string filesPath = Directory.GetCurrentDirectory() + RootfolderPath;
                if (!Directory.Exists(filesPath))
                    Directory.CreateDirectory(filesPath);


                //byte[] ret = Convert.FromBase64String(data);
                //string date = DateTime.Now.ToString().Replace(@"/", @"_").Replace(@":", @"_").Replace(@" ", @"_");
                //string path = HttpContext.Current.Server.MapPath("~/App_Data/Video/Film");
                //FileInfo fil = new FileInfo(path + date + ".mp4");
                //using (Stream sw = fil.OpenWrite())
                //{
                //    sw.Write(ret, 0, ret.Length);
                //    sw.Close();
                //}

                if (Imagebase64Content.Contains(","))
                {
                    Imagebase64Content = Imagebase64Content.Split(',')[1].ToString();
                }

                // Convert base 64 string to byte[]
                byte[] imageBytes = Convert.FromBase64String(Imagebase64Content);
                // Convert byte[] to video
                FileInfo fil = new FileInfo(filesPath + "/" + fName);
                using (Stream sw = fil.OpenWrite())
                {
                    sw.Write(imageBytes, 0, imageBytes.Length);
                    sw.Close();
                    returnfilepath = Path.Combine(url, fName);
                    return returnfilepath;
                }


                //using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                //{
                //    Image image = Image.FromStream(ms, true);
                //    filesPath = filesPath + "/" + fName;
                //    image.Save((filesPath));

                //    returnfilepath = Path.Combine(url, fName);
                //    return returnfilepath;

                //}

            }
            catch (Exception ex)
            {
                return "";
            }
        }


        public string ConvertImageToBase64(string ImagePath)
        {
            string path = ImagePath;
            using (System.Drawing.Image image = System.Drawing.Image.FromFile(path))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();
                    var base64String = Convert.ToBase64String(imageBytes);
                    return base64String;
                }
            }
        }

        public string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName).Replace(" ","_").Replace("(" , "").Replace(")","")
                      + "_"
                      + Guid.NewGuid().ToString().Substring(0, 4)
                      + "_" + DateTime.Now.ToString("yyyyMMddmmss")
                      + Path.GetExtension(fileName);
        }
        public string GetSameFileName(string filename)
        {
            if (!string.IsNullOrEmpty(filename))
            {
                var Fname = filename.Split('.')[0];
                var Fext = filename.Split('.')[1];

                return (Fname) + "." + Fext;
            }

            return "";

        }


        public string GenerateRandomCustomerId(int numberOfCharacters)
        {
            
            if (numberOfCharacters > 10)
                numberOfCharacters = 10;
            if (numberOfCharacters < 3)
                numberOfCharacters = 3;

            var baseString = "0123456789";
            //if (isAlphaNumeric)
            //    baseString = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            char[] charArr = baseString.ToCharArray();
            string otp = "";
            Random obj = new Random();
            for (int i = 0; i < numberOfCharacters; i++)
            {
                int pos = obj.Next(1, charArr.Length);
                if (!otp.Contains(charArr.GetValue(pos).ToString())) otp += charArr.GetValue(pos);
                else i--;
            }

            return otp;
        }
        #endregion

        #region "Encryption"
        public string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public  string SHA1HashStringForUTF8String(string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            using (var sha1 = SHA1.Create())
            {
                byte[] hashBytes = sha1.ComputeHash(bytes);

                return HexStringFromBytes(hashBytes);
            }

        }
        public  string HexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }
        public string EncryptUsingMD5(string toEncrypt, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
            string key = "SecurityKey";
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);



            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };



            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        public string DecryptUsingMD5(string cipherString, bool useHashing)
        {
            byte[] keyArray;
            cipherString = cipherString.Replace(" ", "+");
            byte[] toEncryptArray = Convert.FromBase64String(cipherString);
            string key = "SecurityKey";
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
            {
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
            }

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();

            return UTF8Encoding.UTF8.GetString(resultArray);
        }
        public string EncryptUsingSHA1Hashing(string value)
        {
            var hash = SHA1.Create();
            var encoder = new ASCIIEncoding();
            var combined = encoder.GetBytes(value ?? "");
            return BitConverter.ToString(hash.ComputeHash(combined)).ToLower().Replace("-", "");
        }

        #endregion


      
    }
}
