using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using FlazorTemplate.Helpers;

namespace FlazorTemplate.Authorization.Token
{
    public class TokenHandler : ITokenHandler
    {
        //
        // Class to encrypt and decrypt pernix authentication tokens
        // the encoded text contains the requested e-mail address, expiry time and a hash of the email
        // the hash was added when it was found that small changes to the token would produce nearly valid data
        //

        /// <summary>
        /// Return a hash based on hash algorith
        /// </summary>
        /// <param name="hashAlgorithm"> for this application we are using SHA256</param>
        /// <param name="input"> will be uid + email</param>
        /// <returns></returns>
        private string GetHash(HashAlgorithm hashAlgorithm, string input)
        {
            //
            // return a hash of email address string
            //
            // Convert the input string to a byte array and compute the hash.
            var data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (var i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        /// <summary>
        /// Verify a hash against an input string in our case uid+email string
        /// </summary>
        /// <param name="hashAlgorithm"></param>
        /// <param name="input">parameter from token uid and email</param>
        /// <param name="hash">hash from token</param>
        /// <returns></returns>
        private bool VerifyHash(HashAlgorithm hashAlgorithm, string input, string hash)
        {
            // Hash the input.
            var hashOfInput = GetHash(hashAlgorithm, input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            return comparer.Compare(hashOfInput, hash) == 0;
        }

        /// <summary>
        /// Create a Pernix token
        /// </summary>
        /// <param name="emailAddress"> email address to be encoded in one time toke (ott)</param>
        /// <param name="validFor">number of minutes token will be valid for</param>
        /// <returns>return a tuple containing the generated token plus a UID specific for that token</returns>
        public (string ott, string uid) GenerateOneTimeToken(string emailAddress, int validFor = 15)
        {
            string hash;
            var uid = Guid.NewGuid().ToString();
            using (var sha256Hash = SHA256.Create())
            {
                hash = GetHash(sha256Hash, uid + emailAddress);
            }

            var EncryptionKey = "Yeel4kohyuak6ai";
            string token;
            var clearBytes =
                Encoding.Unicode.GetBytes(uid + "--" + emailAddress +
                                          $"--{DateTime.UtcNow.AddMinutes(validFor)}--{hash}");
            using (var encryption = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes(EncryptionKey,
                    new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryption.Key = pdb.GetBytes(32);
                encryption.IV = pdb.GetBytes(16);
                using var ms = new MemoryStream();
                using var cs = new CryptoStream(ms, encryption.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(clearBytes, 0, clearBytes.Length);
                cs.Close();

                token = EncodingHelpers.UrlSafeEncode(ms.ToArray());
            }

            return (ott: token, uid);
        }

        /// <summary>
        /// Validate the ott by decoding it and sanity checking it.
        /// checks are:-
        /// -- can I find 4 text fields in token
        /// -- does the hash checksum pass for the uid + user address
        /// -- check expiry date
        /// </summary>
        /// <param name="ottToken"> token received from user</param>
        /// <returns>
        /// -- email recovered from token only set if all token checks succeed
        /// -- uid returned on success or token expired
        /// -- status true if all checks valid
        /// -- errtext returned if status false giving reason for failure
        /// </returns>
        public (string email, string uid, bool status, string errtext) VerifyOneTimeToken(string ottToken)
        {
            var EncryptionKey = "Yeel4kohyuak6ai";
            byte[] cipherBytes;
            try
            {
                cipherBytes = EncodingHelpers.UrlSafeDecode(ottToken);
            }
            catch (Exception)
            {
                return (email: "", uid: "", status: false, errtext: "Failed to decode token");
            }

            using (var encryption = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes(EncryptionKey,
                    new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryption.Key = pdb.GetBytes(32);
                encryption.IV = pdb.GetBytes(16);
                using var ms = new MemoryStream();
                using (var cs = new CryptoStream(ms, encryption.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.Close();
                }

                ottToken = Encoding.Unicode.GetString(ms.ToArray());
            }

            var splitText = ottToken.Split("--");
            //
            // We now have
            //
            // splitText[0]    UID
            // splitText[1]    email address
            // splitText[2]    expiry date
            // splitText[3]    hash of UID + email address
            //

            if (splitText.Length != 4)
                return (email: "", uid: "", status: false, errtext: "Failed to decode token");

            using (var sha256Hash = SHA256.Create())
            {
                if (!VerifyHash(sha256Hash, splitText[0] + splitText[1], splitText[3]))
                    return (email: "", uid: "", status: false, errtext: "token hash error");
            }

            var expiry = DateTime.Parse(splitText[2]);

            return expiry > DateTime.UtcNow
                ? (email: splitText[1], uid: splitText[0], status: true, errtext: "")
                : (email: "", uid: splitText[0], status: false, errtext: "Token expired");
        }
    }
}

