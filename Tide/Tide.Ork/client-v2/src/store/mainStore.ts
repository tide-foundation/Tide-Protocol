import { Store } from "./main";

// @ts-ignore
import Tide from "../../../../Tide.Js/src/export/TideAuthentication";

interface MainState extends Object {
  tide?: any;
  config: Config;
  action: AuthAction;
  account?: Account;
  encryptionKey?: string;
}

class MainStore extends Store<MainState> {
  protected data(): MainState {
    return {
      config: {} as Config,
      tide: undefined,
      action: "Login",
      account: undefined,
      encryptionKey: undefined,
    };
  }

  initialize(config: Config) {
    this.state.config = config;
    this.state.tide = new Tide("VendorId", config.serverUrl, config.orks, config.vendorPublic);
  }

  async login(user: UserPass) {
    this.state.action = "Login";
    const serverTime = await (await fetch(`${mainStore.getState.config?.serverUrl}/tide-utility/servertime`)).json();
    this.state.account = await this.state.tide.loginJwt(user.username, user.password, serverTime);

    if (this.state.account == null) throw new Error("Invalid account");
    return this.state.account;
  }

  redirectToVendor() {
    const authResponse: AuthResponse = {
      publicKey: this.state.account!.cvkPublic,
      tideToken: this.state.account!.tideToken,
      action: this.state.action,
      vuid: this.state.account!.vuid.toString(),
    };

    let returnUrl = `${this.state.config.returnUrl}?data=${encodeURIComponent(JSON.stringify(authResponse))}`;

    window.location.replace(returnUrl);
  }
}

const mainStore: MainStore = new MainStore();

export default mainStore;
