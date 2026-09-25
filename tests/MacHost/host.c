#include <dlfcn.h>
#include <stdio.h>

int main(int argc, char **argv) {
	if (argc < 4) {
		return 1;
	}
	void *library = dlopen(argv[1], RTLD_NOW | RTLD_LOCAL);
	if (!library) {
		fprintf(stderr, "%s\n", dlerror());
		return 1;
	}
	int (*check)(const char *, const char *) = dlsym(library, "check_host");
	if (!check) {
		fprintf(stderr, "%s\n", dlerror());
		return 1;
	}
	return check(argv[2], argv[3]);
}
