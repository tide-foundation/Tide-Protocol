export {};

declare global {
  interface IBus {
    on(eventName: string, fn: (data: any) => void);

    off(eventName: string, fn: (data: any) => void);

    trigger(eventName: string, data: any);
  }

  interface UserPass {
    username: string;
    password: string;
  }

  // THESE ARE FROM BUTTON. WE NEED A @TYPES PROJECT
  interface Config {
    mode?: ModeType;
    homeUrl: string;
    serverUrl: string;
    chosenOrk: string;
    vendorPublic: string;
    hashedReturnUrl: string;
    vendorName: string;
    orks: string[];
    debug?: boolean;
    returnUrl: string;

    formData?: any;
    keepOpen: boolean;
    overrideText?: string;
    manualElementId?: string;
    demoMode: boolean;
    logo?: string;
    stylesheet?: string;
  }

  interface AuthResponse {
    publicKey: string;
    tideToken: string;
    vuid: string;
    action: AuthAction;
  }

  interface Account {
    username: string;
    vuid: string;
    tideToken: string;
    cvkPublic: string;
    encryptionKey?: string;
  }

  interface ReturnData {
    key: string;
    value: string;
  }

  type ModeType = "auto" | "button" | "frame" | "manual" | "redirect";
  type AuthAction = "Login" | "Register";
}
