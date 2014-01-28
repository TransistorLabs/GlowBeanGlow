/*
	GlowBeanGlow V2 API 
	Library Header 
*/

#ifndef _GLOWBEANAPI
#define _GLOWBEANAPI


#include <stdio.h>
#include <wchar.h>
#include <string.h>
#include <stdlib.h>
#include "hidapi/hidapi/hidapi.h"

// Headers needed for sleeping.
#ifdef _WIN32
	#include <windows.h>
#else
	#include <unistd.h>
#endif


#ifdef __cplusplus
extern "C" {
#endif

#define VENDOR_ID	0X03EB
#define	PRODUCT_ID 	0x204f

typedef struct hid_device_ glowbean_device;
typedef unsigned char byte;
typedef unsigned short ledbits;

/*
public Action<byte[]> OnReportChange;
public Action OnModeButtonPressed;
public Action OnModeButtonReleased;
public Action<double, double> OnTempChange;
public Action OnUser1ButtonPressed;
public Action OnUser1ButtonReleased;
public Action OnUser2ButtonPressed;
public Action OnUser2ButtonReleased;
public Action<bool> OnProgramWriteComplete;
*/

int glowbean_init();
int glowbean_exit();
int glowbean_setframe(byte red, byte green, byte blue, ledbits ledsOn);
glowbean_device* glowbean_open(void);


#ifdef __cplusplus
}
#endif

#endif