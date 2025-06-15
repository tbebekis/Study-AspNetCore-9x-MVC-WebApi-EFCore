

namespace StudyLib
{
    /// <summary>
    /// Password hasher. It uses the SHA512 algorithm.
    /// NOTE: Hash is one-way encryption, i.e. no decryption.
    /// </summary>
    static public class Hasher
    {
        /// <summary>
        /// The default size of the Salt.
        /// <para>A size of 64 produces a hashed password and a salt key of 89 characters length each.</para>
        /// <para>A size of 32 produces a hashed password and a salt key of 45 characters length each.</para>
        /// </summary>
        const int DefaultSaltSize = 64;
        /// <summary>
        /// The default size of the Password.
        /// <para>A size of 64 produces a hashed password and a salt key of 89 characters length each.</para>
        /// <para>A size of 32 produces a hashed password and a salt key of 45 characters length each.</para>
        /// </summary>
        const int DefaultPasswordSize = 64;
        /// <summary>
        /// The default number of iterations for the hash operation.
        /// </summary>
        const int DefaultIterations = 65535;

        /// <summary>
        /// Encodes a string value into a Base64 string using a specified Encoding.
        /// If Encoding is not specified, the Encoding.UTF8 is used.
        /// </summary>
        static public string StringToBase64(string PlainText, Encoding Enc = null)
        {
            if (Enc == null)
                Enc = Encoding.UTF8;

            byte[] Data = Enc.GetBytes(PlainText);
            return Convert.ToBase64String(Data);
        }
        /// <summary>
        /// Decodes a Base64 string value into a string using a specified Encoding.
        /// If Encoding is not specified, the Encoding.UTF8 is used.
        /// </summary>
        static public string Base64ToString(string Base64Text, Encoding Enc = null)
        {
            if (Enc == null)
                Enc = Encoding.UTF8;

            byte[] Data = Convert.FromBase64String(Base64Text);
            return Enc.GetString(Data);
        }
        /// <summary>
        /// Converts a Base64 string value into a byte array.
        /// </summary>
        static public byte[] Base64ToByteArray(string Base64Text)
        {
            byte[] Data = Convert.FromBase64String(Base64Text);
            return Data;
        }

        /// <summary>
        /// Generates a salt with a cryptographically strong random sequence of values and returns the value as a base64 string.
        /// <para>NOTE: The default size, which is 64, generates a base64 string of 89 characters length.</para>
        /// </summary>
        static public string GenerateSalt(int SaltSize)
        {
            byte[] Buffer = RandomNumberGenerator.GetBytes(SaltSize);
            return Convert.ToBase64String(Buffer);
        }
        /// <summary>
        /// Generates a salt with a cryptographically strong random sequence of values and returns the value as a base64 string.
        /// </summary>
        static public string GenerateSalt()
        {
            return GenerateSalt(DefaultSaltSize);
        }

        /// <summary>
        /// Generates and returns a hash base64 string of a Password specified in clear text, using a specified base64 Salt key string and the SHA512 algorithm. 
        /// <para>NOTE: Use the <see cref="GenerateSalt(int)"/> to generate the salt key.</para>
        /// <para>NOTE: An output size of 64 produces a hashed password and a salt key of 89 characters length each, in base64.</para>
        /// <para>NOTE: An output size of 32 produces a hashed password and a salt key of 45 characters length each, in base64.</para>
        /// </summary>
        static public string Hash(string PlainTextPassword, string Base64SaltKey, int OutputSize, int Iterations)
        {
            byte[] SaltBuffer = Base64ToByteArray(Base64SaltKey);
            byte[] HashBuffer = Rfc2898DeriveBytes.Pbkdf2(PlainTextPassword, SaltBuffer, Iterations, HashAlgorithmName.SHA512, OutputSize);
            return Convert.ToBase64String(HashBuffer);
        }
        /// <summary>
        /// Validates a specified plain text Password along with a base64 Salt key against a base64 hashed Password. Returns true if the passwords are the same.
        /// </summary>
        static public bool Validate(string PlainTextPassword, string Base64HashedPassword, string Base64SaltKey, int OutputSize, int Iterations)
        {
            byte[] SaltBuffer = Base64ToByteArray(Base64SaltKey);
            byte[] HashBuffer = Rfc2898DeriveBytes.Pbkdf2(PlainTextPassword, SaltBuffer, Iterations, HashAlgorithmName.SHA512, OutputSize);
            return HashBuffer.SequenceEqual(Convert.FromBase64String(Base64HashedPassword));
        }

        /// <summary>
        /// Generates and returns a hash base64 string of a Password specified in clear text, using a specified base64 Salt key string and the SHA512 algorithm. 
        /// <para>NOTE: This version uses the default Password size and iterations, defined in this class.</para>
        /// <para>NOTE: Use the <see cref="GenerateSalt(int)"/> to generate the salt key.</para>
        /// <para>NOTE: An output size of 64 produces a hashed password and a salt key of 89 characters length each, in base64.</para>
        /// <para>NOTE: An output size of 32 produces a hashed password and a salt key of 45 characters length each, in base64.</para>
        /// </summary>
        static public string Hash(string PlainTextPassword, string Base64SaltKey)
        {
            int OutputSize = DefaultPasswordSize;
            int Iterations = DefaultIterations;

            return Hash(PlainTextPassword, Base64SaltKey, OutputSize, Iterations);
        }
        /// <summary>
        /// Validates a specified plain text Password along with a base64 Salt key against a base64 hashed Password. Returns true if the passwords are the same.
        /// <para>NOTE: This version uses the default Password size and iterations, defined in this class.</para>
        /// </summary>
        static public bool Validate(string PlainTextPassword, string Base64HashedPassword, string Base64SaltKey)
        {
            int OutputSize = DefaultPasswordSize;
            int Iterations = DefaultIterations;

            return Validate(PlainTextPassword, Base64HashedPassword, Base64SaltKey, OutputSize, Iterations);
        }

    }
}
