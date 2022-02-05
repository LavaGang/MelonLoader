#pragma once
#include <vector>

#include "Console/Console.h"

class Sequence
{
public:
	struct Element
	{
		const char* step;
		bool(*callback)();
	};

	static bool Run(std::vector<Element>);
};
