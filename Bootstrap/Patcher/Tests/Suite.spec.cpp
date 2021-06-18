#include "./Suite.spec.h"

#include <string>
#include <android/log.h>


#include "../../Utils/UnitTesting/TestHelper.h"
#include "../../Utils/UnitTesting/TestHelper.h"
#include "../../Base/funchook/include/funchook.h"
#include "../../Managers/Hook.h"

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
			{
				"funchook",
				[]()
				{
					TestV::OriginalMethod_A = TestV::OriginalMethod;
					//
					// funchook* funchook = funchook_create();
					// funchook_prepare(funchook, (void**)&TestV::OriginalMethod_A, (void*)PatchWorks);
					// funchook_install(funchook, 0);
					//
					Hook::Attach((void**)&TestV::OriginalMethod_A, (void*)PatchWorks);
			
					return TestV::OriginalMethod();
				}
			},
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
