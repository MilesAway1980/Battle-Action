using UnityEngine;
using System.Collections;

public class ChatMessage
{

	string message;
	float startTime;
	float endTime;
	bool expired;

	public ChatMessage()
	{
		expired = true;
	}

	public void Update()
	{
		startTime += Time.fixedTime;
		if (startTime >= endTime)
		{
			expired = true;
		}
	}

	public void SetMessage(string newMessage)
	{
		if (expired)
		{
			message = newMessage;
			startTime = Time.fixedTime;
			expired = false;
		}
	}

	public void SetMessage(string newMessage, float lifeSpan)
	{
		if (expired)
		{
			message = newMessage;
			startTime = Time.fixedTime;
			expired = false;
			SetLifeSpan(lifeSpan);
		}
	}

	public void SetLifeSpan(float lifeSpan)
	{
		endTime = startTime + lifeSpan;
	}

	public bool GetExpired()
	{
		return expired;
	}

	public string GetMessage()
	{
		return message;
	}
}
