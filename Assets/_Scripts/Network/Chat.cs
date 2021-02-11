using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chat
{
	//int messageHistory;
	float lifeSpan;

	ChatMessage[] messages;

	public void SetMessageHistorySize(int newSize)
	{
		//messageHistory = newSize;
		messages = new ChatMessage[newSize];
		for (int i = 0; i < messages.Length; i++)
		{
			messages[i] = new ChatMessage();
		}
	}

	public void NewMessage(string newMessage)
	{
		if (messages != null)
		{
			for (int i = 0; i < messages.Length; i++)
			{
				if (messages[i].GetExpired())
				{
					messages[i].SetMessage(newMessage, lifeSpan);
					break;
				}
			}
		}
	}

	public void SetMessageLifeSpan(float time)
	{
		lifeSpan = time;
	}

	public string[] GetMessages()
	{

		if (messages == null)
		{
			return null;
		}

		int count = 0;
		for (int i = 0; i < messages.Length; i++)
		{
			if (messages[i].GetExpired() == false)
			{
				count++;
			}
		}

		string[] allMessages = new string[count];

		count = 0;
		for (int i = 0; i < messages.Length; i++)
		{
			if (messages[i].GetExpired() == false)
			{
				allMessages[count++] = messages[i].GetMessage();
			}
		}

		return allMessages;
	}

	public void UpdateTimers()
	{
		if (messages == null)
		{
			return;
		}

		for (int i = 0; i < messages.Length; i++)
		{
			messages[i].Update();
		}
	}
}
