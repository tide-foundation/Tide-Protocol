import { Store } from "./main";
import router from "@/router/router";

// @ts-ignore
import Tide from "../../../../Tide.Js/src/export/TideAuthentication";
import { SESSION_ACCOUNT_KEY } from "@/assets/ts/Constants";

interface MainState extends Object {
  tide?: any;
  config: Config;
  action: AuthAction;
  account?: Account;
}

class MainStore extends Store<MainState> {
  protected data(): MainState {
    return {
      config: {} as Config,
      tide: undefined,
      action: "Login",
      account: undefined,
    };
  }

  initialize(config: Config) {
    this.state.config = config;
    this.state.tide = new Tide("VendorId", config.serverUrl, config.orks, config.vendorPublic);
  }

  async login(user: UserPass) {
    this.state.action = "Login";
    const serverTime = await GetServerTime();

    this.setAccount(await this.state.tide.loginJwt(user.username, user.password, serverTime));

    if (this.state.account == null) throw new Error("Invalid account");
    return this.state.account;
  }

  async registerAccount(user: UserPass) {
    this.state.action = "Register";

    const serverTime = await GetServerTime();

    const re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    var isEmail = re.test(String(user.username).toLowerCase());

    this.setAccount(
      await this.state.tide.registerJwt(
        user.username,
        user.password,
        isEmail ? user.username : "noemail@noemail.com",
        this.state.config.orks,
        serverTime,
        this.state.config.orks.length
      )
    );

    return this.state.account;
  }

  setAccount(account: Account) {
    this.state.account = account;
  }

  authenticationComplete() {
    // Go to form if data is available
    if (mainStore.getState.config.formData != null) {
      sessionStorage.setItem(SESSION_ACCOUNT_KEY, JSON.stringify(this.state.account));
      return router.push("/form");
    }

    // If it gets here it was a standard authentication. Let's redirect
    const authResponse: AuthResponse = {
      publicKey: this.state.account!.cvkPublic,
      tideToken: this.state.account!.tideToken,
      action: this.state.action,
      vuid: this.state.account!.vuid.toString(),
    };

    let returnUrl = `${this.state.config.returnUrl}${this.state.config.returnUrl.includes("?") ? "&" : "?"}data=${encodeURIComponent(
      JSON.stringify(authResponse)
    )}`;

    sessionStorage.removeItem(SESSION_ACCOUNT_KEY);
    window.location.replace(returnUrl);
  }

  returnFormData(fields: any[]) {
    var returnData = fields.map((f) => {
      return {
        key: f.field,
        value: f.value,
      } as ReturnData;
    });

    let returnUrl = `${this.state.config.returnUrl}${this.state.config.returnUrl.includes("?") ? "&" : "?"}data=${encodeURIComponent(
      JSON.stringify(returnData)
    )}`;
    window.location.replace(returnUrl);
  }
}

const mainStore: MainStore = new MainStore();

export default mainStore;

async function GetServerTime(): Promise<number> {
  return await (await fetch(`${mainStore.getState.config?.serverUrl}/tide-utility/servertime`)).json();
}
