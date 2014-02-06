
#include <unistd.h>
#include "glowbeanapi.h"



// static bool _attached;
// static bool _connectedToDriver;
// //static HidDevice _device;

// static bool _modeButtonLastState;
// static bool _user1ButtonLastState;
// static bool _user2ButtonLastState;

static glowbean_device *currentHandle;
static struct hid_device_info *devs, *cur_dev;

static byte getHighByte(unsigned short value);
static byte getLowByte(unsigned short value);


int glowbean_init(void)
{
	if (hid_init())
		return -1;
	return 0;
}

int glowbean_exit(void)
{
	hid_close(currentHandle);
	hid_exit();
	return 0;
}

int glowbean_setLiveFrame(const glowbean_liveFrame *const framedata)
{
	return glowbean_setLiveFrame_to(framedata, currentHandle);
}

int glowbean_setLiveFrame_to(const glowbean_liveFrame *const framedata, glowbean_device *handle)
{
	int res;
	unsigned char buf[9];
	memset(buf,0,sizeof(buf));

	// SetFrame (the first byte is the report number (0x00))
	buf[0] = 0x00;
	buf[1] = framedata->color.red;
	buf[2] = framedata->color.green;
	buf[3] = framedata->color.blue;
	buf[4] = getLowByte(framedata->ledsOn);
	buf[5] = getHighByte(framedata->ledsOn);

	res = hid_write(handle, buf, 9);
	if (res < 0) {
		printf("Unable to write()\n");
		printf("Error: %ls\n", hid_error(handle));
	}
	return 0;
}

int glowbean_setFullColorFrame(const glowbean_fullColorLiveFrame *const framedata)
{
	return glowbean_setFullColorFrame_to(framedata, currentHandle);
}

int glowbean_setFullColorFrame_to(const glowbean_fullColorLiveFrame *const framedata, glowbean_device *handle)
{
	int res;
	unsigned char buf[9];
	memset(buf,0,sizeof(buf));


	for(byte i = 0; i<6; i++)
	{
		byte metadata = 0x01; // 0x_1 is the full-color indicator
		metadata |= (i << 4);

		buf[0] = 0x00;
		
		// Color one
		buf[1] = 0x00;
		buf[2] = i*4+5;
		buf[3] = i;
		
		// Color two
		buf[4] = i+2;
		buf[5] = i+4;
		buf[6] = 0x00;
		
		buf[7] = 0xff; //onbits - i=0: lowbyte; i=1; highbyte
		buf[8] = metadata;
	
		res = hid_write(handle, buf, 9);
		if (res < 0) {
			printf("Unable to write()\n");
			printf("Error: %ls\n", hid_error(handle));
			return res;
		}

	}
	
	return 0;
}

glowbean_device* glowbean_open(void) 
{
	currentHandle = hid_open(VENDOR_ID, PRODUCT_ID, NULL);
	if (!currentHandle) {
		//printf("unable to open device\n");
 		return NULL;
	}
	return currentHandle;
}

static byte getHighByte(unsigned short value)
{
    byte s = (unsigned short)(value >> 8);
    return getLowByte(s);
}

static byte getLowByte(unsigned short value)
{
    return (byte)(value & 0xff);
}







