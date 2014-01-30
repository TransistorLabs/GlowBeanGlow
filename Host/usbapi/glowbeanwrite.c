#include <stdio.h>
#include <wchar.h>
#include <string.h>
#include <stdlib.h>
#include "glowbeanapi.h"

// Headers needed for sleeping.
#ifdef _WIN32
	#include <windows.h>
#else
	#include <unistd.h>
#endif


int main(int argc, char* argv[])
{
	
	#define MAX_STR 255
	wchar_t wstr[MAX_STR];
	
	int i;

#ifdef WIN32
	UNREFERENCED_PARAMETER(argc);
	UNREFERENCED_PARAMETER(argv);
#endif

	glowbean_init();
	glowbean_open();

	printf("argc:%d\n", argc);
	if(argc > 1)
	{
		if(strcmp(argv[1], "clear\n"))
		{
			glowbean_setframe(0,0,0,0);
		}
		
		if(argc > 4)
		{
			glowbean_setframe(strtol(argv[1], NULL, 0), strtol(argv[2], NULL, 0), strtol(argv[3], NULL, 0), strtol(argv[4], NULL, 0));
		}
		else if(argc > 3)
		{
			glowbean_setframe(strtol(argv[1], NULL, 0), strtol(argv[2], NULL, 0), strtol(argv[3], NULL, 0), 0xffff);
		}


	}
	else
	{
		glowbean_setframe(0xff,0xff,0xff,0xffff);
	}

	glowbean_exit();

#ifdef WIN32
	system("pause");
#endif

	return 0;
}
