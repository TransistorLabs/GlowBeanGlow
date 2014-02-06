//
//  AppDelegate.m
//  GlowBeanGlow Animator
//
//  Created by Paul Trandem on 2/2/14.
//  Copyright (c) 2014 Transistor Labs. All rights reserved.
//

#import "AppDelegate.h"
#include "glowbeanapi.h"

@interface AppDelegate()

@property BOOL ledsOn;
@property int currentRed;
@property int currentGreen;
@property int currentBlue;

@end


@implementation AppDelegate

- (void)applicationDidFinishLaunching:(NSNotification *)aNotification
{
    // Insert code here to initialize your application
    self.currentRed = 0x00;
    self.currentBlue = 0x00;
    self.currentGreen = 0x00;
    self.ledsOn = NO;
    [self render];
    
}

- (IBAction)onLedToggle:(id)sender {
    if(self.ledsOn)
    {
        self.ledsOn = NO;
    }
    else
    {
        self.ledsOn = YES;
    }
    [self render];
}

- (IBAction)onRedChange:(id)sender {
    NSSlider *slider = sender;
    self.currentRed = [slider intValue];
    [self render];
}
- (IBAction)onGreenChange:(id)sender {
    NSSlider *slider = sender;
    self.currentGreen = [slider intValue];
    [self render];
}

- (IBAction)onBlueChange:(id)sender {
    NSSlider *slider = sender;
    self.currentBlue = [slider intValue];
    [self render];
}

- (void)render
{
    unsigned short ledBits = 0x0000;
    if(self.ledsOn)
    {
        ledBits = 0xffff;
    }
    
    if(!glowbean_init())
    {
        glowbean_open();
        glowbean_setframe((byte)self.currentRed,
                          (byte)self.currentGreen,
                          (byte)self.currentBlue,
                          ledBits);
        glowbean_exit();
    }
}

@end
