#pragma once
  
#include"blockingqueue.h"
#include<winsock.h>    
#include<thread>  
#include<Vector>
class Server
{
private:
	enum {
		SERVER_PORT = 4547,
		BUFFER_SIZE = 13000,
		QUEUE_SIZE = 10,
		MAX = 2
	};
	
	sockaddr_in serverChannel;
	std::thread* sendThread;

	char rootDir[50];
	bool isActive[MAX];
	bool* isConnect;
	std::thread* th[MAX];
	char name[50];
	int serverSocket; //socket   
	int* socketList = new int[MAX];
	BlockingQueue<char*>* sq;
public:
	Server();
	bool start();//¿ªÆô·þÎñÆ÷    
};

class Message {
public:
	BlockingQueue<char*>* queue;
	int clientSocket;
	int* socket;
	bool* isConnect;
};


class ClientMessage {
public:
	BlockingQueue<char*>* queue;
	int* socket;
	bool* isConnect;
	int size;
};