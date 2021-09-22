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
    email?: string;
  }

  interface Account {
    username: string;
    vuid: string;
    tideToken: string;
    cvkPublic: string;
    encryptionKey: string;
  }

  interface Alert {
    type: AlertType;
    msg: string;
  }

  type AlertType = "error" | "success" | "info";
}
