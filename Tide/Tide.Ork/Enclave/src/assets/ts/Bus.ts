interface Event {
  name: string;
  functions: { (data: any): void }[];
}

class Bus implements IBus {
  events: Event[] = [];

  on(eventName: string, fn: (data: any) => void) {
    var event = this.getEvent(eventName);
    if (event == null) this.events.push({ name: eventName, functions: [fn] });
    else event.functions.push(fn);
  }

  off(eventName: string, fn: (data: any) => void) {
    var event = this.getEvent(eventName);
    if (event != null) event.functions = event.functions.filter((e) => e != fn);
  }

  trigger(eventName: string, data: any) {
    var event = this.getEvent(eventName);
    if (event != null) {
      event.functions.forEach((fn) => {
        fn(data);
      });
    }
  }

  private getEvent(name: string): Event | undefined {
    return this.events.find((e) => e.name == name);
  }
}

export default new Bus();
