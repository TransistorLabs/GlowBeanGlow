AM_CPPFLAGS = -I$(top_srcdir)/hidapi/

## Linux
if OS_LINUX
noinst_PROGRAMS = glowbeanwrite

glowbeanwrite_SOURCES = glowbeanwrite.cpp

else

# Other OS's
noinst_PROGRAMS = glowbeanwrite

glowbean_SOURCES = glowbeanwrite.cpp
glowbean_LDADD = $(top_builddir)/$(backend)/libhidapi.la

endif
