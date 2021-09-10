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

  interface Account {
    username: string;
    vuid: string;
    tideToken: string;
    cvkPublic: string;
    encryptionKey?: string;
  }
}
