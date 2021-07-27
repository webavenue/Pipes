#import <Foundation/Foundation.h>
#include "UTNotificationsTools.h"

#define NO_NOTIFICATION_ID ((int)(-10000001))

static int localNotificationWasClicked = NO_NOTIFICATION_ID;
static NSString* pushNotificationWasClicked = nil;

extern "C"
{
    int _UT_GetIconBadgeNumber()
    {
        return (int)[UIApplication sharedApplication].applicationIconBadgeNumber;
    }

    void _UT_SetIconBadgeNumber(int value)
    {
        [UIApplication sharedApplication].applicationIconBadgeNumber = value;
    }

    void _UT_HideAllNotifications()
    {
        int oldBadgeNumber = _UT_GetIconBadgeNumber();
        if (oldBadgeNumber == 0)
        {
            _UT_SetIconBadgeNumber(1);
        }
        _UT_SetIconBadgeNumber(0);

        if (oldBadgeNumber != 0)
        {
            _UT_SetIconBadgeNumber(oldBadgeNumber);
        }
    }
        
    bool _UT_LocalNotificationWasClicked(int id)
    {
        if (localNotificationWasClicked == id)
        {
            _UT_SetLocalNotificationWasClicked(nil);
            return true;
        }
        else
        {
            return false;
        }
    }
    
    bool _UT_PushNotificationWasClicked(const char* body)
    {
        if (pushNotificationWasClicked && body)
        {
            bool clicked = !strcmp(body, [pushNotificationWasClicked UTF8String]);
            if (clicked)
            {
                _UT_SetPushNotificationWasClicked(nil);
                return true;
            }
        }

        return false;
    }

    bool _UT_NotificationsAllowed()
    {
        if ([[UIApplication sharedApplication] respondsToSelector:@selector(registerUserNotificationSettings:)])
        {
            // iOS8+
            bool remoteNotificationsEnabled = [UIApplication sharedApplication].isRegisteredForRemoteNotifications;

            UIUserNotificationSettings* userNotificationSettings = [UIApplication sharedApplication].currentUserNotificationSettings;

            return remoteNotificationsEnabled || userNotificationSettings.types != UIUserNotificationTypeNone;

        }
        else
        {
            // iOS7 and below
            UIRemoteNotificationType enabledRemoteNotificationTypes = [UIApplication sharedApplication].enabledRemoteNotificationTypes;

            return enabledRemoteNotificationTypes != UIRemoteNotificationTypeNone;
        }
    }
}

void _UT_SetLocalNotificationWasClicked(NSDictionary* userInfo)
{
    if (!userInfo)
    {
        localNotificationWasClicked = NO_NOTIFICATION_ID;
    }
    else
    {
        localNotificationWasClicked = [[userInfo objectForKey:@"id"] intValue];
    }
}

void _UT_SetPushNotificationWasClicked(NSDictionary* userInfo)
{
    if (!userInfo)
    {
        pushNotificationWasClicked = nil;
    }
    else
    {
        NSObject* alert = [[userInfo objectForKey:@"aps"] objectForKey:@"alert"];
        if (alert)
        {
            NSString* body;
            if ([alert isKindOfClass:[NSString class]])
            {
                body = (NSString*)alert;
            }
            else
            {
                body = [(NSDictionary*)alert objectForKey:@"body"];
            }
            
            pushNotificationWasClicked = body;
        }
        else
        {
            pushNotificationWasClicked = nil;
        }
    }
}
