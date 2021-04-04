#pragma once
#include <map>
#include "./PatchData.h"

namespace Patcher
{	
	class Instance
	{
#ifdef PORT_DISABLE
	private:
		std::map<void*, PatchData*> PatchMap;
#endif
	};
}
