#pragma once
#include <filesystem>

class Directory
{
public:
	static bool Exists(const char* path)
	{
		struct stat Stat;
		return ((stat(path, &Stat) == 0) && (Stat.st_mode & S_IFDIR));
	};
};