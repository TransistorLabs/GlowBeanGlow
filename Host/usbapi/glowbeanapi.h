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
typedef unsigned short ushort;

typedef struct {
	byte red;
	byte green;
	byte blue;
} glowbean_ledColor;


typedef struct {
	glowbean_ledColor color;
	ushort ledsOn;
} glowbean_liveFrame;

typedef struct {
	glowbean_ledColor colors[12];
	ushort ledsOn;
} glowbean_fullColorLiveFrame;

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

int glowbean_setLiveFrame(const glowbean_liveFrame *const framedata);
int glowbean_setLiveFrame_to(const glowbean_liveFrame *const framedata, glowbean_device *handle);

int glowbean_setFullColorFrame(const glowbean_fullColorLiveFrame *const framedata);
int glowbean_setFullColorFrame_to(const glowbean_fullColorLiveFrame *const framedata, glowbean_device *handle);

glowbean_device* glowbean_open(void);


#ifdef __cplusplus
}
#endif

#endif