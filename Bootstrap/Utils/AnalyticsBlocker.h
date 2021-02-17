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
	static bool CheckHostNames(const char* url);
	static bool IsInBlockedHostNameList(const char* url);

	class wsock32
	{
	public:
		static HMODULE Module;
		static bool Initialize();

		class Exports
		{
		public:
			static bool Initialize();
			typedef void* (__stdcall* gethostbyname_t) (const char* name);
			static gethostbyname_t Gethostbyname;
		};

		class Hooks
		{
		public:
			static void Initialize();
			static void* Gethostbyname(const char* name);
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
			typedef int(__stdcall* getaddrinfo_t) (PCSTR pNodeName, PCSTR pServiceName, void* pHints, void* ppResult);
			static getaddrinfo_t Getaddrinfo;
		};

		class Hooks
		{
		public:
			static void Initialize();
			static int Getaddrinfo(PCSTR pNodeName, PCSTR pServiceName, void* pHints, void* ppResult);
		};
	};
#endif

private:
	static const char* HostNames[];
	static std::list<const char*> HostNames_DAB;
};