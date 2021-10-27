using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Bazinga.Extensions
{
  public enum CryptProvider
  {
    Rijndael,
    RC2,
    DES,
    TripleDES
  }

  public class DataTransferSec

  {
    private CryptProvider _cryptProvider;
    private SymmetricAlgorithm _algorithm;

    private void SetIV()
    {
      switch (_cryptProvider)
      {
        case CryptProvider.Rijndael:

          _algorithm.IV = new byte[] { 0xf, 0x6f, 0x13, 0x2e, 0x35, 0xc2,
          0xcd, 0xf9, 0x5, 0x46, 0x9c, 0xea, 0xa8, 0x4b, 0x73, 0xcc };

          break;
        default:
          _algorithm.IV = new byte[] { 0xf, 0x6f, 0x13, 0x2e, 0x35, 0xc2, 0xcd, 0xf9 };
          break;
      }
    }

    public string Key { get; set; } = string.Empty;

    public DataTransferSec()
    {
      _algorithm = new RijndaelManaged
      {
        Mode = CipherMode.CBC
      };

      _cryptProvider = CryptProvider.Rijndael;
    }

    public DataTransferSec(string key)
    {
      Key = key;

      _algorithm = new RijndaelManaged
      {
        Mode = CipherMode.CBC
      };

      _cryptProvider = CryptProvider.Rijndael;
    }

    public DataTransferSec(CryptProvider cryptProvider)

    {
      switch (cryptProvider)
      {
        case CryptProvider.Rijndael:
          _algorithm = new RijndaelManaged();
          _cryptProvider = CryptProvider.Rijndael;
          break;

        case CryptProvider.RC2:
          _algorithm = new RC2CryptoServiceProvider();
          _cryptProvider = CryptProvider.RC2;
          break;

        case CryptProvider.DES:
          _algorithm = new DESCryptoServiceProvider();
          _cryptProvider = CryptProvider.DES;
          break;

        case CryptProvider.TripleDES:
          _algorithm = new TripleDESCryptoServiceProvider();
          _cryptProvider = CryptProvider.TripleDES;
          break;
      }

      _algorithm.Mode = CipherMode.CBC;

    }

    public virtual byte[] GetKey()
    {
      string salt = string.Empty;

      if (_algorithm.LegalKeySizes.Length > 0)
      {
        int keySize = Key.Length * 8;
        int minSize = _algorithm.LegalKeySizes[0].MinSize;
        int maxSize = _algorithm.LegalKeySizes[0].MaxSize;
        int skipSize = _algorithm.LegalKeySizes[0].SkipSize;

        if (keySize > maxSize) Key = Key.Substring(0, maxSize / 8);

        else if (keySize < maxSize)
        {
          int validSize = (keySize <= minSize) ? minSize
          : (keySize - keySize % skipSize) + skipSize;

          if (keySize < validSize)
          {
            Key = Key.PadRight(validSize / 8, '*');
          }
        }
      }

      PasswordDeriveBytes key = new PasswordDeriveBytes(Key,
      ASCIIEncoding.ASCII.GetBytes(salt));

      return key.GetBytes(Key.Length);
    }

    public virtual string Encrypt(string text)
    {
      byte[] plainByte = Encoding.UTF8.GetBytes(text);
      byte[] keyByte = GetKey();

      // Seta a chave privada
      _algorithm.Key = keyByte;

      SetIV();

      // Interface de criptografia / Cria objeto de criptografia
      ICryptoTransform cryptoTransform = _algorithm.CreateEncryptor();

      MemoryStream _memoryStream = new MemoryStream();

      CryptoStream _cryptoStream = new CryptoStream(_memoryStream,
      cryptoTransform, CryptoStreamMode.Write);

      // Grava os dados criptografados no MemoryStream
      _cryptoStream.Write(plainByte, 0, plainByte.Length);
      _cryptoStream.FlushFinalBlock();

      // Busca o tamanho dos bytes encriptados
      byte[] cryptoByte = _memoryStream.ToArray();

      // Converte para a base 64 string para uso posterior em um xml
      return Convert.ToBase64String(cryptoByte, 0, cryptoByte.GetLength(0));
    }

    public virtual string Decrypt(string text)
    {
      // Converte a base 64 string em num array de bytes
      byte[] cryptoByte = Convert.FromBase64String(text);
      byte[] keyByte = GetKey();

      // Seta a chave privada
      _algorithm.Key = keyByte;
      SetIV();

      // Interface de criptografia / Cria objeto de descriptografia
      ICryptoTransform cryptoTransform = _algorithm.CreateDecryptor();

      try
      {
        MemoryStream _memoryStream = new MemoryStream(cryptoByte, 0, cryptoByte.Length);

        CryptoStream _cryptoStream = new CryptoStream(_memoryStream,
        cryptoTransform, CryptoStreamMode.Read);

        // Busca resultado do CryptoStream
        StreamReader _streamReader = new StreamReader(_cryptoStream);
        return _streamReader.ReadToEnd();
      }
      catch
      {
        return null;
      }
    }
  }
}
