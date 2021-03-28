#include "./Suite.spec.h"
#include "../../Utils/UnitTesting/TestHelper.h"
#include "./Instance.spec.h"
namespace Patcher
{
	bool TestAll()
	{
		UnitTesting::Test TestSequence[] = {
			{
				"Instance",
				patcher_instance_test_run
			}
		};

		return UnitTesting::RunTests(TestSequence, sizeof(TestSequence) / sizeof(TestSequence[0]));
	}
}
