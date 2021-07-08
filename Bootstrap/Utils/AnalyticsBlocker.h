#pragma once
#include <Windows.h>
#include <list>
#include <string>

class AnalyticsBlocker
{
public:
	static bool ShouldDAB;
	static bool Initialize();
	static void Hook();
	static bool CheckHostNameOrIP(const char* host_name_or_ip);
	static bool ShouldBlock(const char* host_name_or_ip);
	static bool HasDabbed(const char* host_name_or_ip);

	class wsock32
	{
	public:
		static HMODULE Module;
		static bool Initialize();

		class Exports
		{
		public:
			static bool Initialize();
			typedef hostent* (__stdcall* gethostbyname_t) (const char* name);
			static gethostbyname_t Gethostbyname;
		};

		class Hooks
		{
		public:
			static void Initialize();
			static hostent* Gethostbyname(const char* name);
		};
	};

#ifdef _WIN64
	class ws2_32
	{
	public:
		static HMODULE Module;
		static bool Initialize();

		class Exports
		{
		public:
			static bool Initialize();
			typedef DWORD (__stdcall* getaddrinfo_t) (PCSTR pNodeName, PCSTR pServiceName, const void* pHints, void* ppResult);
			static getaddrinfo_t Getaddrinfo;
		};

		class Hooks
		{
		public:
			static void Initialize();
			static DWORD Getaddrinfo(PCSTR pNodeName, PCSTR pServiceName, const void* pHints, void* ppResult);
		};
	};
#endif

private:
	static const char* BlockedList[];
	static std::list<std::string> DABList;
};