#include "Sequence.h"

#include "Console/Logger.h"
#include "Console/Debug.h"

bool Sequence::Run(struct Element* elements, size_t len)
{
	bool passed = true;

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