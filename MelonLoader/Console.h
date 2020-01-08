#pragma once
#include <string>
#include <iostream>
#include <fstream>
#include <ostream>

class debugstream
{
public:
	std::ofstream coss;
	template <class T>
	debugstream& operator<< (T val)
	{
		if (coss.is_open())
			coss << val;
		std::cout << val;
		return *this;
	}
	debugstream& operator<< (std::ostream& (*pfun)(std::ostream&))
	{
		if (coss.is_open())
			pfun(coss);
		pfun(std::cout);
		return *this;
	}
};

class Console
{
public:
	static bool IsInitialized();
	static void Create();
	static void Destroy();
	static void Write(const char* txt);
	static void WriteLine(const char* txt);
	static debugstream scout;
};