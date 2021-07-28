#include "Sequence.h"

#include "Console/Debug.h"

bool Sequence::Run(std::vector<Element> elements)
{
	bool passed = true;
	auto len = elements.size();

	for (size_t i = 0; i < len; i++)
	{
		Debug::Msgf("%s", elements[i].step);

		if (!elements[i].callback())
		{
			passed = false;
			break;
		}
	}

	return passed;
}