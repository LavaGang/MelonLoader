#pragma once
#ifdef PORT_DISABLE
#include "./Detour.h"
namespace Patcher
{
	class PatchData
	{
	private:
		void* target;
		Detour* patches;
		size_t detourCount;
	};
}
#endif