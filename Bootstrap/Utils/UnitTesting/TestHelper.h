#pragma once
#include "../Console/Console.h"

namespace UnitTesting
{
	struct Test
	{
		const char* it;
		bool(*callback)();
	};

	void InternalLog(Console::Color color, const char* message);

	bool RunTests(struct Test* tests, size_t len);
}
