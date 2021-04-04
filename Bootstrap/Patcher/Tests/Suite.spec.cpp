#include "./Suite.spec.h"

#include <string>
#include <android/log.h>


#include "../../Utils/UnitTesting/TestHelper.h"
#include "./Instance.spec.h"
#include "../../Utils/UnitTesting/TestHelper.h"
#include "../../Base/funchook/include/funchook.h"

namespace Patcher
{
	class TestV
	{
	public:
		typedef bool (*OriginalMethod_A_t) ();
		static OriginalMethod_A_t OriginalMethod_A;
		
		static bool OriginalMethod()
		{
			UnitTesting::InternalLog(Console::Red, "Unpatched method");
			return false;
		}
	};

	TestV::OriginalMethod_A_t TestV::OriginalMethod_A = NULL;

	bool PatchWorks()
	{
		UnitTesting::InternalLog(Console::Red, "Intercepted");
		bool res = TestV::OriginalMethod_A();
		return !res;
	}
	
	bool TestAll()
	{
		UnitTesting::Test TestSequence[] = {
			// {
			// 	"Patcher",
			// 	[]()
			// 	{
			// 		void* trampoline = NULL;
			// 		A64HookFunction((void*)OriginalMethod, (void*)PatchWorks, &trampoline);
			// 		UnitTesting::InternalLog(Console::Cyan, "Patch Created");
			// 		return (trampoline != NULL) && OriginalMethod();
			// 	}
			// }
			// {
			// 	"funchook",
			// 	[]()
			// 	{
			// 		TestV::OriginalMethod_A = TestV::OriginalMethod;
			// 		
			// 		funchook_t* funchook = funchook_create();
			//
			// 		UnitTesting::InternalLog(Console::Red, "Funchook started");
			//
			// 		int fherrno;
			// 		
			// 		__android_log_print(ANDROID_LOG_INFO, "MelonLoader", "%p %p %p %p", PatchWorks, TestV::OriginalMethod_A, &TestV::OriginalMethod_A, &TestV::OriginalMethod);
			//
			// 		
			// 		fherrno = funchook_prepare(funchook, (void**)&TestV::OriginalMethod_A, (void*)PatchWorks);
			// 		if (fherrno != FUNCHOOK_ERROR_SUCCESS)
			// 		{
			// 			UnitTesting::InternalLog(Console::DarkRed, (std::string("Failed with code ") + std::to_string(fherrno)).c_str());
			// 			return false;
			// 		}
			//
			// 		UnitTesting::InternalLog(Console::Red, "Hook created");
			//
			// 		fherrno = funchook_install(funchook, 0);
			// 		if (fherrno != FUNCHOOK_ERROR_SUCCESS)
			// 		{
			// 			UnitTesting::InternalLog(Console::DarkRed, (std::string("Failed with code ") + std::to_string(fherrno)).c_str());
			// 			return false;
			// 		}
			//
			// 		UnitTesting::InternalLog(Console::Red, "Hook applied");
			//
			// 		bool res = TestV::OriginalMethod();
			//
			// 		UnitTesting::InternalLog(Console::Cyan, "Executed");
			//
			// 		fherrno = funchook_destroy(funchook);
			// 		if (fherrno != FUNCHOOK_ERROR_SUCCESS)
			// 		{
			// 			UnitTesting::InternalLog(Console::DarkRed, (std::string("Failed with code ") + std::to_string(fherrno)).c_str());
			// 			return false;
			// 		}
			//
			// 		UnitTesting::InternalLog(Console::Red, "Funchook cleaned");
			//
			// 		return res;
			// 	}
			// },
			// {
			// 	"auto fail",
			// 	[]()
			// 	{
			// 		return false;
			// 	}
			// }
		};

		return UnitTesting::RunTests(TestSequence, sizeof(TestSequence) / sizeof(TestSequence[0]));
	}
}
