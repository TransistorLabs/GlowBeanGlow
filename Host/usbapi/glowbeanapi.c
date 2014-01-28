
#include "glowbeanapi.h"


// static bool _attached;
// static bool _connectedToDriver;
// //static HidDevice _device;

// static bool _modeButtonLastState;
// static bool _user1ButtonLastState;
// static bool _user2ButtonLastState;

static glowbean_device *handle;
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
	hid_close(handle);
	hid_exit();
	return 0;
}

int glowbean_setframe(byte red, byte green, byte blue, ledbits ledsOn)
{
	int res;
	unsigned char buf[9];
	memset(buf,0,sizeof(buf));

	// SetFrame (the first byte is the report number (0x00))
	buf[0] = 0x00;
	buf[1] = red;
	buf[2] = green;
	buf[3] = blue;
	buf[4] = getLowByte(ledsOn);
	buf[5] = getHighByte(ledsOn);

	res = hid_write(handle, buf, 9);
	if (res < 0) {
		printf("Unable to write()\n");
		printf("Error: %ls\n", hid_error(handle));
	}
	return 0;
}

glowbean_device* glowbean_open(void) 
{
	handle = hid_open(VENDOR_ID, PRODUCT_ID, NULL);
	if (!handle) {
		//printf("unable to open device\n");
 		return NULL;
	}
	return handle;
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







