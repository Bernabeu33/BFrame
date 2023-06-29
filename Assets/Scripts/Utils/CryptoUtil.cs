using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;

public static class CryptoUtil {

	/// <summary>
	/// 计算文件的MD5值
	/// </summary>
	public static string md5file (string file) {
		try {
			FileStream fs = new FileStream (file, FileMode.Open);
			System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider ();
			byte[] retVal = md5.ComputeHash (fs);
			fs.Close ();

			StringBuilder sb = new StringBuilder ();
			for (int i = 0; i < retVal.Length; i++) {
				sb.Append(retVal[i].ToString("x2"));
			}
			return sb.ToString();
		} catch (Exception ex) {
			throw new Exception("md5file() fail, error:" + ex.Message);
		}
	}

	/// <summary>
	/// 计算字符串的MD5值
	/// </summary>
	public static string md5text (string text) {
		string str = "";
		byte[] data = Encoding.GetEncoding("utf-8").GetBytes (text);
		System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider ();
		byte[] bytes = md5.ComputeHash (data);
		for (int i = 0; i < bytes.Length; i++) {
			str += bytes[i].ToString("x2");
		}
		return str;
	}

	/// <summary>
	/// 数据加密
	///
	/// 测试文件: 3.5M文本
	/// 测试结果(单位 ms/M):
	///
	///      操作    时间
	///    ------   ----
	///    读取文件   1
	///    加密文本   32
	///    覆盖文件   1
	///    打开写入   8
	///    解密文本   32
	/// </summary>
	public static Byte[] Encrypt (Byte[] data, string secret) {
		if (data.Length == 0)
			return data;

		Byte[] key = Encoding.Default.GetBytes (secret);

		return ToByteArray (Encrypt (ToUInt32Array (data, true), ToUInt32Array (key, false)), false);
	}

	/// <summary>
	/// 数据解密
	/// </summary>
	public static Byte[] Decrypt (Byte[] data, string secret) {
		if (data.Length == 0)
			return data;

		Byte[] key = Encoding.Default.GetBytes (secret);
		
		return ToByteArray (Decrypt (ToUInt32Array (data, false), ToUInt32Array (key, false)), true);

	}

	static UInt32[] Encrypt (UInt32[] v, UInt32[] k) {
		Int32 n = v.Length - 1;
		if (n < 1) {
			return v;
		}
		if (k.Length < 4) {
			UInt32[] Key = new UInt32[4];
			k.CopyTo (Key, 0);
			k = Key;
		}
		UInt32 z = v[n], y = v[0], delta = 0x9E3779B9, sum = 0, e;
		Int32 p, q = 6 + 52 / (n + 1);
		while (q-- > 0) {
			sum = unchecked (sum + delta);
			e = sum >> 2 & 3;
			for (p = 0; p < n; p++) {
				y = v[p + 1];
				z = unchecked (v[p] += (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z));
			}
			y = v[0];
			z = unchecked (v[n] += (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z));
		}
		return v;
	}

	static UInt32[] Decrypt (UInt32[] v, UInt32[] k) {
		Int32 n = v.Length - 1;
		if (n < 1) {
			return v;
		}
		if (k.Length < 4) {
			UInt32[] Key = new UInt32[4];
			k.CopyTo (Key, 0);
			k = Key;
		}
		UInt32 z = v[n], y = v[0], delta = 0x9E3779B9, sum, e;
		Int32 p, q = 6 + 52 / (n + 1);
		sum = unchecked ((UInt32)(q * delta));
		while (sum != 0) {
			e = sum >> 2 & 3;
			for (p = n; p > 0; p--) {
				z = v[p - 1];
				y = unchecked (v[p] -= (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z));
			}
			z = v[n];
			y = unchecked(v[0] -= (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z));
			sum = unchecked(sum - delta);
		}
		return v;
	}

	static UInt32[] ToUInt32Array (Byte[] data, Boolean includeLength) {
		Int32 n = (((data.Length & 3) == 0) ? (data.Length >> 2) : ((data.Length >> 2) + 1));

		UInt32[] result;
		if (includeLength) {
			result = new UInt32[n + 1];
			result[n] = (UInt32)data.Length;
		} else {
			result = new UInt32[n];
		}

		n = data.Length;
		for (Int32 i = 0; i < n; i++) {
			result[i >> 2] |= (UInt32)data[i] << ((i & 3) << 3);
		}

		return result;
	}

	static Byte[] ToByteArray (UInt32[] data, Boolean includeLength) {
		Int32 n;
		if (includeLength)
			n = (Int32)data[data.Length - 1];
		else
			n = data.Length << 2;

		Byte[] result = new Byte[n];
		for (Int32 i = 0; i < n; i++) {
			result[i] = (Byte)(data[i >> 2] >> ((i & 3) << 3));
		}

		return result;
	}
}