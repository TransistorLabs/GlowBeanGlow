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

char *substring(char *string, int position, int length) 
{
	// TODO: found this quickly on the internet; probably has bugs and/or memory leaks
	// Review for issues.
   char *pointer;
   int c;
 
   pointer = malloc(length+1);
 
   if (pointer == NULL)
   {
      printf("Unable to allocate memory.\n");
      exit(EXIT_FAILURE);
   }
 
   for (c = 0 ; c < position ; c++) 
      string++; 
 
   for (c = 0 ; c < length ; c++)
   {
      *(pointer+c) = *string;      
      string++;   
   }
 
   *(pointer+c) = '\0';

   return pointer;
}

void getHexByteStrFromPos(char *dest, char *source, int position)
{
	//TODO: make this less crappy/more bullet-proof
	char *result = malloc(5);
	char *tmp = malloc(5);
	strcpy(result, "0x");
	strcpy(tmp, "");
	
	tmp = substring(source, position, 2);

	strcat(result, tmp);
	memcpy(dest, result, strlen(dest)+1);

	free(result);
	free(tmp);
}

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

	char* redStr = malloc(5);
	char* greenStr = malloc(5);
	char* blueStr = malloc(5);
	char* bitStr = malloc(7);
	strcpy(redStr, "0x00");
	strcpy(greenStr, "0x00");
	strcpy(blueStr, "0x00");
	strcpy(bitStr, "0x0000");

	printf("argc:%d\n", argc);
	if(argc > 1)
	{
		if(argv[1][0] == '#')
		{
			printf("%s\n", argv[1]);
			printf("%d\n", (int)strlen(argv[1]));
			if(strlen(argv[1]) == 7)
			{
				getHexByteStrFromPos(redStr, argv[1], 1);
				getHexByteStrFromPos(greenStr, argv[1], 3);
				getHexByteStrFromPos(blueStr, argv[1], 5);				
			}

			if(argc > 2 && strlen(argv[2]) == 6)
			{
				memcpy(bitStr, argv[2], 7);
			}
		}
		
		glowbean_liveFrame frame;
		frame.color.red = strtol(redStr, NULL, 0);
		frame.color.green = strtol(greenStr, NULL, 0);
		frame.color.blue = strtol(blueStr, NULL, 0);
		frame.ledsOn = strtol(bitStr, NULL, 0);

		glowbean_setLiveFrame(&frame);

	}
	else
	{
		glowbean_liveFrame frame;
		frame.color.red = 0xff;
		frame.color.green = 0xff;
		frame.color.blue = 0xff;
		frame.ledsOn = 0xffff;

		glowbean_setFullColorFrame(NULL);
	}

	free(redStr);
	free(greenStr);
	free(blueStr);
	free(bitStr);

	glowbean_exit();

#ifdef WIN32
	system("pause");
#endif

	return 0;
}


