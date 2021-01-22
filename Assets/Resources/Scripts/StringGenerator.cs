using UnityEngine;
using System.Collections;

public static class StringGenerator {

	public static string RandomString(int length) {
		string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		char[] stringChars = new char[length];
		
		for (int i = 0; i < length; i++)
		{
			stringChars[i] = chars[Random.Range (0, chars.Length)];
		}
		
		return new string(stringChars);
	}
}
