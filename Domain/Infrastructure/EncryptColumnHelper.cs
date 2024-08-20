using System.Security.Cryptography;
using System.Text;

namespace Domain.Infrastructure;

public static class EncryptColumnHelper
{
	#region Fields

	private static byte[] EncryptPublicKey;
	private static byte[] DecryptPublicKey;
	private static byte[] EncryptPrivateKey;
	private static byte[] DecryptPrivateKey;

	#endregion

	#region Ctor

	static EncryptColumnHelper()
	{
		string encryptPrivateKeyBase64 = "RUNLNkIAAAABnwOMgJ1xKcwr0kuxjWQKtWMva0lu0ARH7+1WuPQOTjkjeBCBHw2nALV2Hki59sy3TXhybKLeaGHXtUbh8O7qEZkBzyKzgtfrv3PK7K1Yphn0+gMFegsGanFd14qE5MAYJd8k0h+tsVuqAwVsAHlwOgwSKFhWt3kB+VYFOgNAjM5KGigAP4xMCvDH2VuKmaH+f3vEF6Xoheu3jUPU14i+bGazREbwYYaI4URCDRJCi0hpHm+aEJutkQbGOJU6y79uqcvv/TA=";
		string decryptPrivateKeyBase64 = "RUNLNkIAAAAAKo0jB6ZlolDzWMR4/Qp81Kxxg/i+iCh3qFkje4DpvYcTNVqxJmeKMT5wBOBDSOaV9DA0y7B7nxy1By9pf0JgeFQA9hvqa10kybhI+hsJvLc6DsWD9Mtrgd+TxiF9/c0REtoVXMTHBuRNRky/QVJA5pGpQ4JsXoTc6Ig13MxlLatlddkA1m1NaPBygNOC15lIxxeoWAwyrX2Mldk889f7+dT/dnN2oNsCRk5ioKgwfkgC+dRRx0y9sGc6GCVRJ1QpgzB1fmI=";

		EncryptPrivateKey = Convert.FromBase64String(encryptPrivateKeyBase64);
		DecryptPrivateKey = Convert.FromBase64String(decryptPrivateKeyBase64);

		GeneratePublicKeys();
	}

	#endregion

	#region Encryption

	public static byte[] Encrypt(string dataToEncrypt)
	{
		using (ECDiffieHellmanCng encryptObj = new ECDiffieHellmanCng(CngKey.Import(EncryptPrivateKey, CngKeyBlobFormat.EccPrivateBlob)))
		{
			CngKey decryptPublicKey = CngKey.Import(DecryptPublicKey, CngKeyBlobFormat.EccPublicBlob);
			byte[] encryptionKey = encryptObj.DeriveKeyMaterial(decryptPublicKey);

			using (Aes aes = Aes.Create())
			{
				//aes.Key = encryptionKey;
				aes.KeySize = 128; // Set AES key size to 256 bits
				aes.Key = encryptionKey.Take(aes.KeySize / 8).ToArray(); // Ensure key is the correct size
				byte[] iv = aes.IV;

				using (MemoryStream ciphertext = new MemoryStream())
				using (CryptoStream cs = new CryptoStream(ciphertext, aes.CreateEncryptor(), CryptoStreamMode.Write))
				{
					byte[] plaintextData = Encoding.UTF8.GetBytes(dataToEncrypt);
					cs.Write(plaintextData, 0, plaintextData.Length);
					cs.FlushFinalBlock();
					byte[] encryptedData = ciphertext.ToArray();

					// Combine IV and encrypted data
					byte[] result = new byte[iv.Length + encryptedData.Length];
					Array.Copy(iv, 0, result, 0, iv.Length);
					Array.Copy(encryptedData, 0, result, iv.Length, encryptedData.Length);

					return result;
				}
			}
		}
	}

	#endregion

	#region Decryption

	public static string Decrypt(byte[] encryptedDataWithIv)
	{
		byte[] iv = new byte[16];
		byte[] encryptedData = new byte[encryptedDataWithIv.Length - iv.Length];

		Array.Copy(encryptedDataWithIv, 0, iv, 0, iv.Length);
		Array.Copy(encryptedDataWithIv, iv.Length, encryptedData, 0, encryptedData.Length);

		using (ECDiffieHellmanCng decryptObj = new ECDiffieHellmanCng(CngKey.Import(DecryptPrivateKey, CngKeyBlobFormat.EccPrivateBlob)))
		{
			CngKey encryptPublicKey = CngKey.Import(EncryptPublicKey, CngKeyBlobFormat.EccPublicBlob);
			byte[] decryptionKey = decryptObj.DeriveKeyMaterial(encryptPublicKey);

			using (Aes aes = Aes.Create())
			{
				//aes.Key = decryptionKey;
				aes.KeySize = 128; // Set AES key size to 256 bits
				aes.Key = decryptionKey.Take(aes.KeySize / 8).ToArray(); // Ensure key is the correct size
				aes.IV = iv;

				using (MemoryStream plaintext = new())
				using (CryptoStream cs = new(plaintext, aes.CreateDecryptor(), CryptoStreamMode.Write))
				{
					cs.Write(encryptedData, 0, encryptedData.Length);
					cs.FlushFinalBlock();
					return Encoding.UTF8.GetString(plaintext.ToArray());
				}
			}
		}
	}
	#endregion

	#region Helper method

	public static void GeneratePublicKeys()
	{
		using (ECDiffieHellmanCng encryptObj = new ECDiffieHellmanCng(CngKey.Import(EncryptPrivateKey, CngKeyBlobFormat.EccPrivateBlob)))
		{
			encryptObj.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
			encryptObj.HashAlgorithm = CngAlgorithm.Sha256;
			//EncryptPublicKey = encryptObj.PublicKey.ExportSubjectPublicKeyInfo();
			EncryptPublicKey = encryptObj.PublicKey.ToByteArray();
		}

		using (ECDiffieHellmanCng decryptObj = new ECDiffieHellmanCng(CngKey.Import(DecryptPrivateKey, CngKeyBlobFormat.EccPrivateBlob)))
		{
			decryptObj.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
			decryptObj.HashAlgorithm = CngAlgorithm.Sha256;
			//DecryptPublicKey = decryptObj.PublicKey.ExportSubjectPublicKeyInfo();
			DecryptPublicKey = decryptObj.PublicKey.ToByteArray();
		}
	}

	#endregion
}