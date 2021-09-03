import { Store } from "./main";

interface MainState extends Object {
  user: any;
}

class MainStore extends Store<MainState> {
  protected data(): MainState {
    return {
      user: null,
    };
  }

  async waitForInitialization() {
    //this.state.client.login();
  }
}

const mainStore: MainStore = new MainStore();

export default mainStore;
