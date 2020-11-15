using System;
using System.Diagnostics;
using System.Text;
using System.Security.Cryptography;


namespace rsa
{
    class Program
    {
        private static string toHex(ReadOnlySpan<byte> span)
        {
            StringBuilder hex = new StringBuilder(span.Length * 2);
            foreach (byte b in span)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }


        private static void generateKeys()
        {
            try
            {
                // generate rsa public-private key pair
                System.Diagnostics.Process.Start("openssl", "genrsa -out private_key.pem 2048").WaitForExit();
                // extract public key in pem format
                System.Diagnostics.Process.Start("openssl", "rsa -in private_key.pem -outform PEM -pubout -out public_key.pem").WaitForExit();
                // extract public key in der format
                System.Diagnostics.Process.Start("openssl", "rsa -in private_key.pem -outform DER -RSAPublicKey_out -out public_key.der").WaitForExit();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"call to openssl failed: {ex}");
            }
        }


        private static void cryptDecrypt()
        {
            // .Net 5.0 required for method ImportFromPem 
            var privateKey = RSA.Create();
            privateKey.ImportFromPem(System.IO.File.ReadAllText("private_key.pem"));
            var publicKey = RSA.Create();
            publicKey.ImportFromPem(System.IO.File.ReadAllText("public_key.pem"));

            string message = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.";
            Console.WriteLine($"original message: {message}");
            var byteStream = Encoding.UTF8.GetBytes(message);
            Console.WriteLine($"original message hex:");
            Console.WriteLine(toHex(byteStream));
            if (byteStream.Length > publicKey.KeySize)
                throw new Exception("message too long");
            var encoded = publicKey.Encrypt(byteStream, RSAEncryptionPadding.Pkcs1);
            Console.WriteLine($"encoded message hex:");
            Console.WriteLine(toHex(encoded));
            var decoded = privateKey.Decrypt(encoded, RSAEncryptionPadding.Pkcs1);
            Console.WriteLine($"decoded message hex:");
            Console.WriteLine(toHex(decoded));
            string decodedMessage = Encoding.UTF8.GetString(decoded);
            Console.WriteLine($"decoded message: {decodedMessage}");
        }


        private static void cryptDecrypt2()
        {
            // works also with older .Net versions
            var privateKey = RSA.Create();
            var privateKeyBase64 = System.IO.File.ReadAllText("private_key.pem").Replace("-----BEGIN RSA PRIVATE KEY-----", "").Replace("-----END RSA PRIVATE KEY-----", "");
            privateKey.ImportRSAPrivateKey(Convert.FromBase64String(privateKeyBase64), out var bytesRead);
            var publicKey = RSA.Create();
            publicKey.ImportRSAPublicKey(System.IO.File.ReadAllBytes("public_key.der"), out bytesRead);

            string message = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.";
            Console.WriteLine($"original message: {message}");
            var byteStream = Encoding.UTF8.GetBytes(message);
            Console.WriteLine($"original message hex:");
            Console.WriteLine(toHex(byteStream));
            if (byteStream.Length > publicKey.KeySize)
                throw new Exception("message too long");
            var encoded = publicKey.Encrypt(byteStream, RSAEncryptionPadding.Pkcs1);
            Console.WriteLine($"encoded message hex:");
            Console.WriteLine(toHex(encoded));
            var decoded = privateKey.Decrypt(encoded, RSAEncryptionPadding.Pkcs1);
            Console.WriteLine($"decoded message hex:");
            Console.WriteLine(toHex(decoded));
            string decodedMessage = Encoding.UTF8.GetString(decoded);
            Console.WriteLine($"decoded message: {decodedMessage}");

        }


        public static void Main(string[] args)
        {
            generateKeys();
            Console.WriteLine();
            cryptDecrypt();
            Console.WriteLine();
            cryptDecrypt2();
        }
    }
}
