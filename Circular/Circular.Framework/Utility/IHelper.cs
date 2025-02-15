using Microsoft.AspNetCore.Http;
namespace Circular.Framework.Utility
{
    public interface IHelper
    {
         string GenerateRandomNumber(int numberOfCharacters, string masterOtp, bool masterOtpEnabled, bool isAlphaNumeric);
        string GenerateRandomNumberone(int numberOfCharacters, string masterOtp, bool masterOtpEnabled, bool isAlphaNumeric);
        string GetQRCode(string keyString, string fileNameWithoutExtension, ref string path);

         string ConvertBase64toDocument(string FileName, string base64Content, string RootfolderPath);
         string ConvertBase64toImage(string ImageName, string Imagebase64Content, string RootfolderPath, HttpRequest request);
        string ConvertBase64toMp4(string ImageName, string Imagebase64Content, string RootfolderPath, HttpRequest request);

        
         string ConvertImageToBase64(string ImagePath);
         string SaveFile(IFormFile file, string uploadFolder, HttpRequest request);
         string GetUniqueFileName(string fileName);
         string GetSameFileName(string filename);

         string Base64Encode(string plainText);
         string Base64Decode(string base64EncodedData);
         string SHA1HashStringForUTF8String(string s);
         string HexStringFromBytes(byte[] bytes);
         string EncryptUsingMD5(string toEncrypt, bool useHashing);
         string DecryptUsingMD5(string cipherString, bool useHashing);
         string EncryptUsingSHA1Hashing(string value);
         string GenerateRandomCustomerId(int numberOfCharacters);
    }
}
