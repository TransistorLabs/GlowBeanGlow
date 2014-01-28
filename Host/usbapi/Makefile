# Makefile for "glowbeanapi" and "glowbeanwrite"

# Build arguments:
# - "OS=macosx"  -- build Mac version on Mac OS X
# - "OS=windows" -- build Windows version on Windows
# - "OS=linux"   -- build Linux version on Linux
# 
# Architecture is usually detected automatically, so normally just type "make"
#
# Dependencies: 
# - hidapi (included), which uses libusb on Linux-like OSes
#
# Platform-specific notes:
#
# Mac OS X 
#   - Install Xcode with "Unix Dev Support" and "Command-line tools" (in Preferences)
#   - make
#
# Windows XP/7  
#   - Install MinGW and MSYS (http://www.tdragon.net/recentgcc/ )
#   - make
#
# Linux (Ubuntu) 
#   - apt-get install build-essential pkg-config libusb-1.0-0-dev 
#   - make

# try to do some autodetecting
UNAME := $(shell uname -s)

ifeq "$(UNAME)" "Darwin"
	OS=macosx
endif

ifeq "$(OS)" "Windows_NT"
	OS=windows
endif

ifeq "$(UNAME)" "Linux"
	OS=linux
endif

ifeq "$(UNAME)" "FreeBSD"
	OS=freebsd
endif

ifeq "$(PKGOS)" ""
   PKGOS = $(OS)
endif


CC=gcc
#CC=clang

# pick low-level implemenation style
# "HIDAPI" type is best for Mac, Windows, Linux Desktop, 
#  but has dependencies on iconv, libusb-1.0, pthread, dl
#
# "HIDDATA" type is best for low-resource Linux, 
#  and the only dependencies it has is libusb-0.1
#

USBLIB_TYPE = HIDAPI

# uncomment for debugging HID stuff
#CFLAGS += -DDEBUG_PRINTF


#################  Mac OS X  ##################################################
ifeq "$(OS)" "macosx"
LIBTARGET = glowbeanapi.dylib
CFLAGS += -mmacosx-version-min=10.6

ifeq "$(USBLIB_TYPE)" "HIDAPI"
CFLAGS += -DUSE_HIDAPI
CFLAGS += -arch x86_64
#CFLAGS += -pthread
CFLAGS += -I./hidapi/hidapi 
OBJS = ./hidapi/mac/hid.o
endif

#ifeq "$(USBLIB_TYPE)" "HIDDATA"
#CFLAGS += -DUSE_HIDDATA
#OBJS = ./hiddata.o
#OPT_HOME := /opt/local/bin
#CFLAGS += `$(OPT_HOME)/libusb-config --cflags`
#LIBS   += `$(OPT_HOME)/libusb-config --libs`
#endif

LIBS += -framework IOKit -framework CoreFoundation

EXEFLAGS = 
#LIBFLAGS = -bundle -o $(LIBTARGET) -Wl,-search_paths_first $(LIBS)
LIBFLAGS = -dynamiclib -o $(LIBTARGET) -Wl,-search_paths_first $(LIBS)
EXE=

endif

#################  Windows  ##################################################
#ifeq "$(OS)" "windows"
#LIBTARGET = glowbeanapi.dll
##LIBS +=  -mwindows -lsetupapi -Wl,--enable-auto-import -static-libgcc -static-libstdc++ -lkernel32 
##LIBS +=  -mwindows -lsetupapi -Wl,-Bdynamic -lgdi32 -Wl,--enable-auto-import -static-libgcc -static-libstdc++ -lkernel32
#LIBS +=             -lsetupapi -Wl,--enable-auto-import -static-libgcc -static-libstdc++ 
#
#ifeq "$(USBLIB_TYPE)" "HIDAPI"
#CFLAGS += -DUSE_HIDAPI
#CFLAGS += -I./hidapi/hidapi 
#OBJS = ./hidapi/windows/hid.o
#endif
#
#ifeq "$(USBLIB_TYPE)" "HIDDATA"
#CFLAGS += -DUSE_HIDDATA
#OBJS = ./hiddata.o
#endif
#
#EXEFLAGS =
##LIBFLAGS = -shared -o $(LIBTARGET) -Wl,--add-stdcall-alias -Wl,--export-all-symbols -Wl,--out-implib,$(LIBTARGET).a $(LIBS)
#LIBFLAGS = -shared -o $(LIBTARGET) -Wl,--add-stdcall-alias -Wl,--export-all-symbols
#EXE= .exe
#
#endif


#####################  Common  ###############################################

#CFLAGS += -O -Wall -std=gnu99 -I ../hardware/firmware 
CFLAGS += -std=gnu99 
CFLAGS += -g

OBJS +=  glowbeanapi.o 

#all: msg glowbeanwrite blink1-server-simple
all: msg glowbeanwrite lib

msg: 
	@echo "building for OS=$(OS)"

# symbolic targets:
help:
	@echo "This Makefile works on multiple archs. Use one of the following:"
	#@echo "make OS=windows ... build Windows  glowbeanapi and glowbeanwrite" 
	#@echo "make OS=linux   ... build Linux    glowbeanapi and glowbeanwrite" 
	#@echo "make OS=freebsd ... build FreeBSD    glowbeanapi and glowbeanwrite" 
	#@echo "make OS=macosx  ... build Mac OS X glowbeanapi and glowbeanwrite" 
	#@echo "make OS=wrt     ... build OpenWrt glowbeanapi and glowbeanwrite"
	@echo "make USBLIB_TYPE=HIDDATA OS=linux ... build using low-dep method"
	#@echo "make lib        ... build glowbeanapi shared library"
	@echo "make package PKGOS=mac  ... zip up build, give it a name 'mac' "
	@echo "make clean ..... to delete objects and hex file"
	@echo

$(OBJS): %.o: %.c
	$(CC) $(CFLAGS) -c $< -o $@

glowbeanwrite: $(OBJS) glowbeanwrite.o glowbeanapi.o
	$(CC) $(CFLAGS) -c glowbeanwrite.c -o glowbeanwrite.o
	$(CC) $(CFLAGS) -c glowbeanapi.c -o glowbeanapi.o
	$(CC) $(CFLAGS) $(EXEFLAGS) -g $(OBJS) $(LIBS) glowbeanwrite.o -o glowbeanwrite$(EXE) 

lib: $(OBJS)
	$(CC) $(LIBFLAGS) $(CFLAGS) $(OBJS) $(LIBS)

clean: 
	rm -f $(OBJS)
#	rm -f $(LIBTARGET)
	rm -f glowbeanwrite.o glowbeanapi.o hiddata.o
	
distclean: clean
	rm -f glowbeanwrite$(EXE)
#	rm -f $(LIBTARGET) $(LIBTARGET).a

# show shared library use
# in general we want minimal to no dependecies for glowbeanwrite

# shows shared lib usage on Mac OS X
otool:
	otool -L glowbeanwrite
# show shared lib usage on Linux
ldd:
	ldd glowbeanwrite
# show shared lib usage on Windows
# FIXME: only works inside command prompt from
# Start->All Programs-> MS Visual Studio 2012 -> VS Tools -> Devel. Cmd Prompt
dumpbin: 
#	dumpbin.exe /exports $(LIBTARGET)
	dumpbin.exe /exports glowbeanwrite.exe

foo:
	@echo "OS=$(OS), USBFLAGS=$(USBFLAGS)"
