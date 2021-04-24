#pragma once
#include "Console/Console.h"

class Sequence
{
public:
	struct Element
	{
		const char* step;
		bool(*callback)();
	};

	static bool Run(struct Element* tests, size_t len);
};
