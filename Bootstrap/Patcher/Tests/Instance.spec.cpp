#pragma once
#include "../../Utils/UnitTesting/TestHelper.h"
#include "../Instance.h"
#include "./Instance.spec.h"

bool patcher_instance_test_run()
{
	UnitTesting::Test TestSequence[] = {
		{
			"Creates instance of patch map",
			[]()
			{
				struct PatchMap* map = patcher_instance_create(1024);
				bool res = map != NULL;
				free(map);
				return res;
			}
		}
	};

	return UnitTesting::RunTests(TestSequence, sizeof(TestSequence) / sizeof(TestSequence[0]));
}
