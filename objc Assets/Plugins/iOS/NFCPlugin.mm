#import <Foundation/Foundation.h>
#import <CoreNFC/CoreNFC.h>

@interface NFCPluginDelegate : NSObject<NFCTagReaderSessionDelegate>
@property (nonatomic, strong) NFCTagReaderSession *session;
@end

@implementation NFCPluginDelegate

- (void)startReading {
    if (@available(iOS 13.0, *)) {
        self.session = [[NFCTagReaderSession alloc] 
            initWithPollingOption:NFCPollingISO14443 
            delegate:self 
            queue:dispatch_get_main_queue()];
        
        self.session.alertMessage = @"Halte dein iPhone an den NFC-Tag";
        [self.session beginSession];
    }
}

- (void)tagReaderSession:(NFCTagReaderSession *)session didDetectTags:(NSArray<__kindof id<NFCTag>> *)tags {
    if (tags.count > 0) {
        id<NFCTag> tag = tags.firstObject;
        
        [session connectToTag:tag completionHandler:^(NSError *error) {
            if (error) {
                UnitySendMessage("NFCManager", "OnNFCError", 
                    [[error localizedDescription] UTF8String]);
                [session invalidateSession];
                return;
            }
            
            // Lese NDEF-Daten (falls vorhanden)
            if ([tag conformsToProtocol:@protocol(NFCISO7816Tag)]) {
                id<NFCISO7816Tag> iso7816Tag = (id<NFCISO7816Tag>)tag;
                NSString *identifier = [[iso7816Tag identifier] 
                    base64EncodedStringWithOptions:0];
                
                UnitySendMessage("NFCManager", "OnNFCTagRead", 
                    [identifier UTF8String]);
                [session invalidateSession];
            }
        }];
    }
}

- (void)tagReaderSession:(NFCTagReaderSession *)session didInvalidateWithError:(NSError *)error {
    if (error.code != NFCReaderSessionInvalidationErrorUserCanceled) {
        UnitySendMessage("NFCManager", "OnNFCError", 
            [[error localizedDescription] UTF8String]);
    }
}

@end

static NFCPluginDelegate *pluginDelegate = nil;

extern "C" {
    void _StartNFCReading() {
        if (pluginDelegate == nil) {
            pluginDelegate = [[NFCPluginDelegate alloc] init];
        }
        [pluginDelegate startReading];
    }
    
    bool _IsNFCAvailable() {
        if (@available(iOS 13.0, *)) {
            return NFCTagReaderSession.readingAvailable;
        }
        return false;
    }
}