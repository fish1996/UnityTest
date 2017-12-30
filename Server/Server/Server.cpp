#define _CRT_SECURE_NO_WARNINGS    

#include"Server.h"    
#include<cstdio>    
#include<cstdlib>    
#include<fstream>    
#include<cstring>    
#include<string>  
#include<windows.h>  
#include<iostream>  


void handleMessage(Message msg) {
	const int BUFFER_SIZE = 100;
	int id;
	char buffer[BUFFER_SIZE];
	while (1) {
		int ret;
		memset(buffer, 'Z', sizeof(buffer));
		ret = recv(msg.clientSocket, buffer, BUFFER_SIZE, 0);
		if (ret == SOCKET_ERROR) {
			printf("sorry receive failed\n");
		}
		else if (ret == 0) {
			printf("the client socket is closed\n");
		}
		else {
			//printf("sucessfully receive\n");
			for (int i = 0; i < BUFFER_SIZE && buffer[i]!='Z'; i+=2) {
				int status = (int)buffer[i];
				int value = (int)buffer[i+1];
				if (status == 0) {
					id = value;
					msg.socket[id] = msg.clientSocket;
					msg.isConnect[id] = true;
				}
				else {
					char* data = new char[3];
					
					data[0] = id;
					data[1] = buffer[i];
					data[2] = buffer[i + 1];
					msg.queue->Put(data);
				}
			}
		}
	}
}

void sendMessage(ClientMessage msg) {
	while (1) {
		char* data = msg.queue->Take();
		int id = data[0];
		std::cout << "id = " << id << std::endl;
		for (int i = 1; i <= msg.size; i++) {
			std::cout << "i = " << i;
			if(msg.isConnect[i]){
				std::cout << " isConnect\n";
			}
			else {
				std::cout << " noConnect\n";
			}
			if (i != id && msg.isConnect[i]) {
				int clientSocket = msg.socket[i];
				std::cout << "i = "<<i<<" clientSocket = " << clientSocket;
				int r = send(clientSocket, data, 3, 0);

				if (r == SOCKET_ERROR) {
					printf("send failed\n");
				}
				else {
					printf("send success\n");
				}
			}
		}

	}
}

Server::Server()
{
	WORD wVersionRequested;
	WSADATA wsaData;
	int ret;

	socketList = new int[MAX];
	isConnect = new bool[MAX];
	memset(isConnect, false, sizeof(isConnect));
	sq = new BlockingQueue<char*>();
	//WinSock初始化：    
	wVersionRequested = MAKEWORD(2, 2);//希望使用的WinSock DLL的版本    
	ret = WSAStartup(wVersionRequested, &wsaData);
	if (ret != 0)
	{
		printf("WSAStartup() failed!\n");
	}
	//确认WinSock DLL支持版本2.2：    
	if (LOBYTE(wsaData.wVersion) != 2 || HIBYTE(wsaData.wVersion) != 2)
	{
		WSACleanup();
		printf("Invalid Winsock version!\n");
	}

}

bool Server::start()
{
	int on = 1;
	memset(isActive, false, sizeof(isActive));
	//初始化服务器    
	memset(&serverChannel, 0, sizeof(serverChannel));
	serverChannel.sin_family = AF_INET;
	serverChannel.sin_addr.s_addr = htonl(INADDR_ANY);
	serverChannel.sin_port = htons(SERVER_PORT);

	//创建套接字    
	serverSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	if (serverSocket < 0) {
		printf("cannot create socket\n");
		return false;
	}
	else printf("successfully create socket\n");
	setsockopt(serverSocket, SOL_SOCKET, SO_REUSEADDR,
		(char*)&on, sizeof(on));

	//绑定    
	int b = bind(serverSocket, (sockaddr*)&serverChannel,
		sizeof(serverChannel));
	if (b < 0) {
		printf("bind error\n");
		return false;
	}
	else printf("successfully bind\n");
	//监听    
	int l = listen(serverSocket, 1);
	if (l < 0) {
		printf("listen failed\n");
		return false;
	}
	else printf("successfully listen\n");
	int len = sizeof(serverChannel);

	//开启广播线程
	ClientMessage msg;
	msg.socket = socketList;
	msg.queue = sq;
	msg.size = MAX;
	msg.isConnect = isConnect;
	sendThread = new std::thread(sendMessage, msg);

	//服务器等待连接    
	while (1) {
		printf("waiting for connection...\n");
		//接受一个连接    
		int clientSocket = accept(serverSocket, (sockaddr*)&serverChannel, &len);
		std::cout << "connect by " << clientSocket << std::endl;
		if (clientSocket < 0) {
			printf("accept failed\n");
		}
		else {
			printf("successfully connect\n");
			Message msg;
			msg.queue = sq;
			msg.clientSocket = clientSocket;
			msg.socket = socketList;
			msg.isConnect = isConnect;
		
			for (int i = 0; i < MAX; i++) {
				if (!isActive[i]) {
					isActive[i] = true;
					th[i] = new std::thread(handleMessage, msg);
					break;
				}
			}
		}
	}
}