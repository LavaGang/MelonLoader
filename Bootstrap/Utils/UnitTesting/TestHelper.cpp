#include "./TestHelper.h"

#ifdef __ANDROID__
#include <android/log.h>
#endif

namespace UnitTesting
{
	void InternalLog(Console::Color color, const char* message)
	{
#ifdef __ANDROID__
		__android_log_print(ANDROID_LOG_INFO, "MelonLoader", (std::string(Console::ColorToAnsi(color)) + message + Console::ColorToAnsi(Console::Color::Reset)).c_str());
#endif
	}

	bool RunTests(struct Test* tests, size_t len)
	{
		bool passed = true;

		for (size_t i = 0; i < len; i++)
		{
			InternalLog(Console::Gray, (std::string("Testing: ") + tests[i].it).c_str());

			if (tests[i].callback())
				InternalLog(Console::Green, "Passed");
			else
			{
				passed = false;
				InternalLog(Console::Red, "Failed");
				break;
			}
		}

		return passed;
	}
}
