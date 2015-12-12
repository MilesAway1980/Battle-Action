using UnityEngine;
using System.Collections;

public class ChatMessage {

	string message;
	float startTime;
	float endTime;
	bool expired;

	public ChatMessage() {
		expired = true;
	}

	public void update() {
		startTime += Time.fixedTime;
		if (startTime >= endTime) {
			expired = true;
		}
	}

	public void setMessage(string newMessage) {
		if (expired) {
			message = newMessage;
			startTime = Time.fixedTime;
			expired = false;
		}
	}

	public void setMessage(string newMessage, float lifeSpan) {
		if (expired) {
			message = newMessage;
			startTime = Time.fixedTime;
			expired = false;
			setLifeSpan (lifeSpan);
		}
	}

	public void setLifeSpan(float lifeSpan) {
		endTime = startTime + lifeSpan;
	}

	public bool getExpired() {
		return expired;
	}

	public string getMessage() {
		return message;
	}
}

public class Chat  {
	//int messageHistory;
	float lifeSpan;

	ChatMessage[] messages;

	public void setMessageHistorySize(int newSize) {
		//messageHistory = newSize;
		messages = new ChatMessage[newSize];
		for (int i = 0; i < messages.Length; i++) {
			messages[i] = new ChatMessage();
		}
	}

	public void newMessage(string newMessage) {
		if (messages != null) {
			for (int i = 0; i < messages.Length; i++) {
				if (messages [i].getExpired ()) {
					messages [i].setMessage (newMessage, lifeSpan);
					break;
				}
			}
		}
	}

	public void setMessageLifeSpan(float time) {
		lifeSpan = time;
	}

	public string[] getMessages() {

		if (messages == null) {
			return null;
		}

		int count = 0;
		for (int i = 0; i < messages.Length; i++) {
			if (messages[i].getExpired() == false) {
				count++;
			}
		}

		string[] allMessages = new string[count];

		count = 0;
		for (int i = 0; i < messages.Length; i++) {
			if (messages[i].getExpired() == false) {
				allMessages[count++] = messages[i].getMessage();
			}
		}

		return allMessages;
	}

	public void updateTimers() {
		if (messages == null) {
			return;
		}

		for (int i = 0; i < messages.Length; i++) {
			messages[i].update();
		}
	}
}
