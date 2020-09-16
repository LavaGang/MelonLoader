#pragma once
#include <Windows.h>
#include <list>
#include <string>

class AnalyticsBlocker
{
public:
	static bool Enabled;
	static bool ShouldDAB;
	static bool Initialize();
	static void Hook();
	static bool CheckHostNames(const char* url);

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

private:
	static std::list<std::string> HostNames;
	static std::list<std::string> HostNames_DAB;
};