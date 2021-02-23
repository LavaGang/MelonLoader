#pragma once

#include <string>
#include <stdio.h>
#include <sys/mman.h>
#include <byteswap.h>
#include <unistd.h>
#include <vector>

using bytes = std::vector<uint8_t>;

class Patch
{
public:
	static Patch *Setup(void* _target, char *data, size_t len);
	static Patch *Setup(void* _target, uint32_t data);
	static Patch *Setup(void* _target, std::string data);

	virtual void Apply();
	virtual void Reset();

private:
	Patch() = default;
	Patch(void* _target, char *data, size_t len);
	~Patch();

protected:
	void* _t_addr;
	size_t _patchSize;
	bytes _patchBytes;
	bytes _origBytes;
};