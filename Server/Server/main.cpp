#include "Server.h"
#pragma(lib,"ws2_32.lib")  

int main() {
	Server server;
	server.start();
}